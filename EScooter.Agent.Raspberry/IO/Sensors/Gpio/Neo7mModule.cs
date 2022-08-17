using EScooter.Agent.Raspberry.Model;
using Geolocation;
using Iot.Device.Nmea0183;
using Iot.Device.Nmea0183.Sentences;
using System.IO.Ports;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Sensors.Gpio;

public class Neo7mModule : ISpeedometer, IGpsSensor, IDisposable
{
    private readonly CancellationTokenSource _cts;
    private readonly SerialPort _serialPort;
    private RecommendedMinimumNavigationInformation? _lastRmcSentence;

    public Neo7mModule(string serialPortName)
    {
        _serialPort = new SerialPort(serialPortName)
        {
            NewLine = "\r\n"
        };
        _cts = new CancellationTokenSource();

        Task.Run(() => Reader(_cts.Token));
    }

    private void Reader(CancellationToken cancellationToken)
    {
        _serialPort.Open();

        _serialPort.ReadLine();

        var lastMessageTime = DateTimeOffset.UtcNow;
        while (!cancellationToken.IsCancellationRequested)
        {
            ReadNextLine(ref lastMessageTime);
        }
    }

    private void ReadNextLine(ref DateTimeOffset lastMessageTime)
    {
        var line = _serialPort.ReadLine();
        var sentence = TalkerSentence.FromSentenceString(line, out _);
        var typedSentence = sentence?.TryGetTypedValue(ref lastMessageTime);
        if (typedSentence is RecommendedMinimumNavigationInformation rmc)
        {
            OnNewSatelliteData(rmc);
        }
    }

    private void OnNewSatelliteData(RecommendedMinimumNavigationInformation rmc)
    {
        if (rmc.Status is not NavigationStatus.Valid)
        {
            return;
        }

        lock (this)
        {
            _lastRmcSentence = rmc;
        }
    }

    Coordinate ISensor<Coordinate>.ReadValue()
    {
        lock (this)
        {
            return _lastRmcSentence is null || !_lastRmcSentence.Position.ContainsValidPosition()
                ? new Coordinate(0, 0)
                : new Coordinate(_lastRmcSentence.Position.Latitude, _lastRmcSentence.Position.Longitude);
        }
    }

    Speed ISensor<Speed>.ReadValue()
    {
        lock (this)
        {
            return _lastRmcSentence is null
                ? Speed.Zero
                : _lastRmcSentence.SpeedOverGround;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _serialPort.Dispose();
        GC.SuppressFinalize(this);
    }
}
