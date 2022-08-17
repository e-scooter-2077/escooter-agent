using EScooter.Agent.Raspberry.Model;
using Geolocation;
using Iot.Device.Common;
using Iot.Device.Nmea0183;
using Iot.Device.Nmea0183.Sentences;
using System.IO.Ports;

namespace EScooter.Agent.Raspberry.IO.Sensors.Gpio;

public class GpioGpsSensorV2 : IGpsSensor, IDisposable
{
    private readonly CancellationTokenSource _cts;
    private readonly SerialPort _serialPort;
    private Coordinate _currentPosition;

    public GpioGpsSensorV2(string serialPortName)
    {
        _serialPort = new SerialPort(serialPortName)
        {
            NewLine = "\r\n"
        };
        _serialPort.Open();
        _serialPort.ReadLine();
        _cts = new CancellationTokenSource();

        Task.Run(() => Reader(_cts.Token));
    }

    private void Reader(CancellationToken cancellationToken)
    {
        var lastMessageTime = DateTimeOffset.UtcNow;
        while (!cancellationToken.IsCancellationRequested)
        {
            var line = _serialPort.ReadLine();
            Console.WriteLine($"Read from GPS sensor: {line}");

            var sentence = TalkerSentence.FromSentenceString(line, out _);

            if (sentence == null)
            {
                continue;
            }

            var typedSentence = sentence.TryGetTypedValue(ref lastMessageTime);
            if (typedSentence is null)
            {
                Console.WriteLine($"Unknown sentence type '{sentence.Id}'");
            }
            else if (typedSentence is not RecommendedMinimumNavigationInformation rmc)
            {
                Console.WriteLine($"Sentence type '{sentence.Id}' will be ignored");
            }
            else
            {
                OnNewPosition(rmc.Position);
            }
        }
    }

    private void OnNewPosition(GeographicPosition position)
    {
        if (!position.ContainsValidPosition())
        {
            Console.WriteLine("Message contains invalid position");
            return;
        }

        lock (this)
        {
            _currentPosition = new Coordinate(position.Latitude, position.Longitude);
        }
    }

    public Coordinate ReadValue()
    {
        lock (this)
        {
            return _currentPosition;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _serialPort.Dispose();
    }
}
