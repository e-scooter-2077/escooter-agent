using EScooter.Agent.Raspberry.Model;
using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockSensors : IGpsSensor, ISpeedometer, IBatterySensor, IMagneticBrake
{
    private static readonly TimeSpan _batteryDuration = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan _tripDuration = TimeSpan.FromMinutes(3);
    private static readonly Coordinate _initial = new(44.143043, 12.247474);
    private static readonly Coordinate _final = new(44.142935, 12.2386884);
    private static readonly Speed _speed = Length.FromMiles(GeoCalculator.GetDistance(_initial, _final)) / _tripDuration;

    private static readonly TimeSpan _period = TimeSpan.FromSeconds(1);

    private readonly IMagneticBrake _magneticBrake;
    private Timer? _timer;
    private bool _isLocked = true;
    private Coordinate _currentPosition = _initial;
    private Fraction _currentBatteryLevel = Fraction.FromFraction(1);
    private TimeSpan _elapsed = TimeSpan.Zero;

    public MockSensors(IMagneticBrake magneticBrake)
    {
        _magneticBrake = magneticBrake;
    }

    public void SetValue(bool value)
    {
        if (_isLocked != value)
        {
            if (!value)
            {
                _timer = new Timer(_ => Update(), null, TimeSpan.Zero, _period);
            }
            else
            {
                _timer?.Dispose();
                _timer = null;
            }
            _isLocked = value;
        }
        _magneticBrake.SetValue(value);
    }

    private void Update()
    {
        lock (this)
        {
            _elapsed += _period;

            var latDst = _final.Latitude - _initial.Latitude;
            var lonDst = _final.Longitude - _initial.Longitude;
            var dstProp = _elapsed / _tripDuration;
            _currentPosition = new(_initial.Latitude + latDst * dstProp, _initial.Longitude + lonDst * dstProp);

            var batteryProp = _elapsed / _batteryDuration;
            var remaining = Math.Max(1 - batteryProp, 0);
            _currentBatteryLevel = Fraction.FromFraction(remaining);
        }
    }

    Coordinate ISensor<Coordinate>.ReadValue()
    {
        lock (this)
        {
            return _currentPosition;
        }
    }

    Fraction ISensor<Fraction>.ReadValue()
    {
        lock (this)
        {
            return _currentBatteryLevel;
        }
    }

    Speed ISensor<Speed>.ReadValue() => _isLocked ? Speed.Zero : _speed;
}
