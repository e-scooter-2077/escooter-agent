using EScooter.Agent.Raspberry.Model;
using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockSpeedAndGpsSensor : IGpsSensor, IMagneticBrake, ISpeedometer
{
    private static readonly TimeSpan _period = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan _duration = TimeSpan.FromMinutes(4);
    private static readonly Coordinate _initial = new(44.143043, 12.247474);
    private static readonly Coordinate _final = new(44.142935, 12.2386884);

    private readonly IMagneticBrake _magneticBrake;
    private Timer? _timer;
    private bool _isLocked = true;
    private Coordinate _currentPosition = _initial;
    private TimeSpan _elapsed = TimeSpan.Zero;

    public MockSpeedAndGpsSensor(IMagneticBrake magneticBrake)
    {
        _magneticBrake = magneticBrake;
    }

    public void SetValue(bool value)
    {
        if (_isLocked != value)
        {
            if (value)
            {
                _timer = new Timer(_ => UpdatePosition(), null, TimeSpan.Zero, _period);
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

    private void UpdatePosition()
    {
        lock (this)
        {
            _elapsed += _period;
            var latDst = _final.Latitude - _initial.Latitude;
            var lonDst = _final.Longitude - _initial.Longitude;
            var prop = _elapsed / _duration;
            _currentPosition = new(_initial.Latitude + latDst * prop, _initial.Longitude + lonDst * prop);
        }
    }

    Coordinate ISensor<Coordinate>.ReadValue()
    {
        lock (this)
        {
            return _currentPosition;
        }
    }

    Speed ISensor<Speed>.ReadValue() => _isLocked || _elapsed < _period
        ? Speed.Zero
        : Length.FromMiles(GeoCalculator.GetDistance(_initial, _final)) / _elapsed;
}
