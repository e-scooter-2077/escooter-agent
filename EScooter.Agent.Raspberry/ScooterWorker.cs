using EScooter.Agent.Raspberry.Dto;
using EScooter.Agent.Raspberry.Model;
using Microsoft.Azure.Devices.Client;
using System.Threading.Channels;
using UnitsNet;

namespace EScooter.Agent.Raspberry;

public class ScooterWorker : BackgroundService
{
    private const bool DefaultLockedState = false;

    private static readonly Speed _defaultMaxSpeed = Speed.FromKilometersPerHour(25);
    private static readonly TimeSpan _defaultUpdateFrequency = TimeSpan.FromSeconds(10);

    private readonly DeviceClient _deviceClient;
    private readonly Channel<Func<Task>> _scheduledTasks;
    private readonly ScooterDevice _scooterDevice;
    private ScooterState? _scooterState;
    private Timer? _timer;

    public ScooterWorker(IConfiguration configuration, ScooterDevice scooterDevice)
    {
        _deviceClient = CreateClient(configuration);
        _scheduledTasks = Channel.CreateUnbounded<Func<Task>>(new UnboundedChannelOptions
        {
            SingleReader = true
        });
        _scooterDevice = scooterDevice;
    }

    private DeviceClient CreateClient(IConfiguration configuration)
    {
        var scooterId = configuration["Scooter:Id"];
        var key = configuration["Scooter:SymmetricKey"];
        var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(scooterId, key);

        var hostName = configuration["IoTHubHostName"];
        return DeviceClient.Create(hostName, authMethod, TransportType.Mqtt);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SetInitialState(stoppingToken);
        await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(
            (t, _) => OnDesiredPropertiesUpdate(ScooterDesiredDto.FromJson(t.ToJson())),
            null,
            stoppingToken);
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

    private async Task OnDesiredPropertiesUpdate(ScooterDesiredDto desired)
    {
        await ScheduleTask(async () =>
        {
            UpdateScooterState(s => s with
            {

            });
        });
    }

    private async void OnNewTimerTick()
    {
        await ScheduleTask(async () =>
        {
        });
    }

    private async Task SetInitialState(CancellationToken stoppingToken)
    {
        var twin = await _deviceClient.GetTwinAsync(stoppingToken);
        var desired = ScooterDesiredDto.FromJson(twin.Properties.Desired.ToJson());
        _timer = new Timer(_ => OnNewTimerTick(), null, TimeSpan.Zero, desired.UpdateFrequency ?? _defaultUpdateFrequency);

        var sensorsState = await _scooterDevice.ReadSensorsState();

        _scooterState = new ScooterState(
            DesiredMaxSpeed: desired.MaxSpeed is null ? _defaultMaxSpeed : Speed.FromMetersPerSecond(desired.MaxSpeed.Value),
            Locked: DefaultLockedState,
            BatteryLevel: sensorsState.BatteryLevel,
            Speed: sensorsState.Speed,
            Position: sensorsState.Position);

        await OnDesiredPropertiesUpdate(desired);
    }

    private void UpdateScooterState(Func<ScooterState, ScooterState> updateAction)
    {
        _scooterState = updateAction(_scooterState!);
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        _deviceClient.Dispose();
        base.Dispose();
    }
}
