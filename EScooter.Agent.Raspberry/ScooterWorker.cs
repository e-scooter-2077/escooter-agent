using EScooter.Agent.Raspberry.IotHub;
using EScooter.Agent.Raspberry.Model;
using System.Threading.Channels;

namespace EScooter.Agent.Raspberry;

public class ScooterWorker : BackgroundService
{
    private readonly Channel<Func<Task>> _scheduledTasks;
    private readonly ScooterDevice _scooterDevice;
    private readonly IotHubScooterWrapper _iotHubScooter;
    private Timer? _timer;

    public ScooterWorker(ScooterDevice scooterDevice, IotHubScooterWrapper iotHubScooter)
    {
        _scheduledTasks = Channel.CreateUnbounded<Func<Task>>(new UnboundedChannelOptions
        {
            SingleReader = true
        });
        _scooterDevice = scooterDevice;
        _iotHubScooter = iotHubScooter;
    }

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
        await ScheduleTask(() => _scooterDevice.SetDesiredState(desired));
    }

    private async void OnNewTimerTick()
    {
        await ScheduleTask(async () =>
        {
            var sensorsState = await _scooterDevice.ReadSensorsState();
            await _iotHubScooter.SendSensorsTelemetry(sensorsState);
        });
    }

    private async Task SetInitialState(CancellationToken stoppingToken)
    {
        var desired = await _iotHubScooter.GetDesiredState(stoppingToken);
        _timer = new Timer(_ => OnNewTimerTick(), null, TimeSpan.Zero, desired.UpdateFrequency);

        var sensorsState = await _scooterDevice.ReadSensorsState();

        await OnDesiredPropertiesUpdate(desired);
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}
