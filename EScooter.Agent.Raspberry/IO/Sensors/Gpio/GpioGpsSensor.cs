using EScooter.Agent.Raspberry.Model;
using Geolocation;
using Iot.Device.Common;
using Iot.Device.Nmea0183;
using System.IO.Ports;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Sensors.Gpio;

public class GpioGpsSensor : ISensor<Coordinate>, IDisposable
{
    private readonly SerialPort _serialPort;

    private Coordinate _currentPosition;

    public GpioGpsSensor(string serialPortName)
    {
        _serialPort = new SerialPort(serialPortName);
    }

    public Task Setup()
    {
        _serialPort.Open();

        var stream = _serialPort.BaseStream;

        var parser = new NmeaParser("GPS", stream, stream);
        parser.OnNewPosition += OnNewPosition;
        parser.StartDecode();

        return Task.CompletedTask;
    }

    private void OnNewPosition(GeographicPosition position, Angle? track, Speed? speed)
    {
        lock (this)
        {
            _currentPosition = new Coordinate(position.Latitude, position.Longitude);
        }
    }

    public Task<Coordinate> ReadValue()
    {
        lock (this)
        {
            return Task.FromResult(_currentPosition);
        }
    }

    public void Dispose()
    {
        _serialPort.Dispose();
    }
}
