using EScooter.Agent.Raspberry.IotHub;
using EScooter.Agent.Raspberry.Model;
using System.Threading.Channels;

namespace EScooter.Agent.Raspberry;

public class ScooterWorker : BackgroundService
{
    private readonly Channel<Func<Task>> _scheduledTasks;
    private readonly ScooterHardware _scooterHardware;
    private readonly IotHubScooterWrapper _iotHubScooter;
    private Scooter? _scooter;
    private Timer? _timer;

    public ScooterWorker(ScooterHardware scooterHardware, IotHubScooterWrapper iotHubScooter)
    {
        _scheduledTasks = Channel.CreateUnbounded<Func<Task>>(new UnboundedChannelOptions
        {
            SingleReader = true
        });
        _scooterHardware = scooterHardware;
        _iotHubScooter = iotHubScooter;
    }

    private Scooter Scooter => _scooter!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SetInitialState(stoppingToken);
        await _iotHubScooter.RegisterForDesiredPropertyUpdates(OnDesiredPropertiesUpdate, stoppingToken);
        await foreach (var task in _scheduledTasks.Reader.ReadAllAsync(stoppingToken))
        {
            await task();
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        await base.StopAsync(cancellationToken);
    }

    private async Task ScheduleTask(Func<Task> task)
    {
        await _scheduledTasks.Writer.WriteAsync(task);
    }

    private async Task OnDesiredPropertiesUpdate(ScooterDesiredState desired)
    {
        await ScheduleTask(() =>
        {
            Scooter.SetDesiredState(desired);
            return Task.CompletedTask;
        });
    }

    private async void OnNewTimerTick()
    {
        await ScheduleTask(() =>
        {
            Scooter.UpdateSensorsState();
            return Task.CompletedTask;
        });
    }

    private async Task SetInitialState(CancellationToken stoppingToken)
    {
        var desired = await _iotHubScooter.GetDesiredState(stoppingToken);
        _scooter = new Scooter(_scooterHardware, desired);
        _scooter.ReportedStateChanged += OnReportedStateChanged;
        _scooter.SensorsStateChanged += OnSensorsStateChanged;

        Scooter.UpdateSensorsState();

        _timer = new Timer(_ => OnNewTimerTick(), null, TimeSpan.Zero, desired.UpdateFrequency);
    }

    private async void OnReportedStateChanged(ScooterReportedState reported) =>
        await ScheduleTask(() => _iotHubScooter.SendReportedState(reported));

    private async void OnSensorsStateChanged(ScooterSensorsState sensorsState) =>
        await ScheduleTask(() => _iotHubScooter.SendSensorsTelemetry(sensorsState));

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}
