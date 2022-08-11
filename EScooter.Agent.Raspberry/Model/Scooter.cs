using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public class Scooter
{
    private static readonly Fraction _standbyThreshold = Fraction.FromPercentage(10);
    private static readonly Speed _standbyMaxSpeed = Speed.FromKilometersPerHour(15);
    private readonly ScooterHardware _hardware;

    public Scooter(ScooterHardware hardware, ScooterDesiredState initialDesiredState)
    {
        _hardware = hardware;
        CurrentReportedState = new(
            Locked: true,
            Standby: false,
            MaxSpeed: Speed.FromKilometersPerHour(25),
            UpdateFrequency: TimeSpan.FromSeconds(10));
        CurrentDesiredState = initialDesiredState;
        SetDesiredState(initialDesiredState);
        CurrentSensorsState = ReadAllSensors();
    }

    public ScooterReportedState CurrentReportedState { get; private set; }

    public ScooterDesiredState CurrentDesiredState { get; private set; }

    public ScooterSensorsState CurrentSensorsState { get; private set; }

    public void UpdateSensorsState()
    {
        CurrentSensorsState = ReadAllSensors();
        _hardware.BatteryDisplay.SetValue(CurrentSensorsState.BatteryLevel);
        _hardware.SpeedDisplay.SetValue(CurrentSensorsState.Speed);

        TrackReportedChanges(() => ManageStandbyPolicies(CurrentSensorsState.BatteryLevel));
        SensorsStateChanged?.Invoke(CurrentSensorsState);
    }

    private ScooterSensorsState ReadAllSensors()
    {
        var battery = _hardware.Battery.ReadValue();
        var position = _hardware.Gps.ReadValue();
        var speed = _hardware.Speedometer.ReadValue();

        return new ScooterSensorsState(battery, speed, position);
    }

    private void ManageStandbyPolicies(Fraction battery)
    {
        void SetStandby(bool standby)
        {
            ApplyDesiredMaxSpeed(standby ? _standbyMaxSpeed : CurrentDesiredState.MaxSpeed);
            _hardware.StandbyIndicator.SetValue(standby);
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
        TrackReportedChanges(() =>
        {
            ApplyDesiredLockedState(desiredState.Locked);
            if (CurrentReportedState.Standby)
            {
                return;
            }
            ApplyDesiredMaxSpeed(desiredState.MaxSpeed);
        });
    }

    private void ApplyDesiredLockedState(bool desiredLockedState)
    {
        _hardware.MagneticBrakes.SetValue(desiredLockedState);
        CurrentReportedState = CurrentReportedState with
        {
            Locked = desiredLockedState
        };
    }

    private void ApplyDesiredMaxSpeed(Speed desiredMaxSpeed)
    {
        _hardware.MaxSpeedEnforcer.SetValue(desiredMaxSpeed);
        CurrentReportedState = CurrentReportedState with
        {
            MaxSpeed = desiredMaxSpeed
        };
    }

    private void TrackReportedChanges(Action update)
    {
        var oldState = CurrentReportedState;

        update();

        if (CurrentReportedState != oldState)
        {
            ReportedStateChanged?.Invoke(CurrentReportedState);
        }
    }

    public event Action<ScooterReportedState>? ReportedStateChanged;

    public event Action<ScooterSensorsState>? SensorsStateChanged;
}
