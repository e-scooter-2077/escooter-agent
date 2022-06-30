using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public class ScooterDevice
{
    private static readonly Fraction _standbyThreshold = Fraction.FromPercentage(10);
    private static readonly Speed _standbyMaxSpeed = Speed.FromKilometersPerHour(15);

    public ScooterDevice(
        ISensor<Speed> speedometer,
        ISensor<Coordinate> gps,
        ISensor<Fraction> battery,
        IActuator<bool> magneticBrakes,
        IActuator<Speed> maxSpeedEnforcer,
        ScooterDesiredState initialDesiredState)
    {
        Speedometer = speedometer;
        Gps = gps;
        Battery = battery;
        MagneticBrakes = magneticBrakes;
        MaxSpeedEnforcer = maxSpeedEnforcer;
        CurrentDesiredState = initialDesiredState;
        CurrentReportedState = new(
            Locked: true,
            Standby: false,
            MaxSpeed: Speed.FromKilometersPerHour(25),
            UpdateFrequency: TimeSpan.FromSeconds(10));
        ManageStandbyPolicies(Battery.ReadValue());
        SetDesiredState(initialDesiredState);
    }

    public ISensor<Speed> Speedometer { get; }

    public ISensor<Coordinate> Gps { get; }

    public ISensor<Fraction> Battery { get; }

    public IActuator<bool> MagneticBrakes { get; }

    public IActuator<Speed> MaxSpeedEnforcer { get; }

    public ScooterReportedState CurrentReportedState { get; private set; }

    public ScooterDesiredState CurrentDesiredState { get; private set; }

    public ScooterSensorsState UpdateSensorsState()
    {
        var battery = Battery.ReadValue();
        var position = Gps.ReadValue();
        var speed = Speedometer.ReadValue();
        ManageStandbyPolicies(battery);

        return new ScooterSensorsState(battery, speed, position);
    }

    private void ManageStandbyPolicies(Fraction battery)
    {
        void SetStandby(bool standby)
        {
            ApplyDesiredMaxSpeed(standby ? _standbyMaxSpeed : CurrentDesiredState.MaxSpeed);
            CurrentReportedState = CurrentReportedState with
            {
                Standby = standby
            };
        }
        if (battery <= _standbyThreshold && !CurrentReportedState.Standby)
        {
            SetStandby(true);
        }
        else if (battery > _standbyThreshold && CurrentReportedState.Standby)
        {
            SetStandby(false);
        }
    }

    public void SetDesiredState(ScooterDesiredState desiredState)
    {
        CurrentDesiredState = desiredState;
        ApplyDesiredLockedState(desiredState.Locked);
        if (CurrentReportedState.Standby)
        {
            return;
        }
        ApplyDesiredMaxSpeed(desiredState.MaxSpeed);
    }

    private void ApplyDesiredLockedState(bool desiredLockedState)
    {
        MagneticBrakes.SetValue(desiredLockedState);
        CurrentReportedState = CurrentReportedState with
        {
            Locked = desiredLockedState
        };
    }

    private void ApplyDesiredMaxSpeed(Speed desiredMaxSpeed)
    {
        MaxSpeedEnforcer.SetValue(desiredMaxSpeed);
        CurrentReportedState = CurrentReportedState with
        {
            MaxSpeed = desiredMaxSpeed
        };
    }
}
