using EScooter.Agent.Raspberry.Model;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Threading.Channels;

namespace EScooter.Agent.Raspberry;

public class ScooterWorker : BackgroundService
{
    private readonly DeviceClient _deviceClient;
    private readonly Channel<Func<Task>> _scheduledTasks;
    private Scooter? _scooter;
    private Timer? _timer;

    public ScooterWorker(IConfiguration configuration)
    {
        _deviceClient = CreateClient(configuration);
        _scheduledTasks = Channel.CreateUnbounded<Func<Task>>(new UnboundedChannelOptions
        {
            SingleReader = true
        });
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
        _scooter = new Scooter(await GetInitialState());
        _timer = new Timer(_ => OnNewTimerTick(), null, TimeSpan.Zero, _scooter.CurrentState.UpdateFrequency.ToTimeSpan());
        await _deviceClient.SetDesiredPropertyUpdateCallbackAsync((t, _) => OnDesiredPropertiesUpdate(t), null, stoppingToken);
        await foreach (var task in _scheduledTasks.Reader.ReadAllAsync(stoppingToken))
        {
            await task();
        }
    }

    private async Task ScheduleTask(Func<Task> task)
    {
        await _scheduledTasks.Writer.WriteAsync(task);
    }

    private async Task OnDesiredPropertiesUpdate(TwinCollection desiredProperties)
    {
        await ScheduleTask(async () =>
        {
            _scooter.UpdateState(s => s with
            {
                
            })
        });
    }

    private async void OnNewTimerTick()
    {
        await ScheduleTask(async () =>
        {
        });
    }

    private async Task<ScooterState> GetInitialState()
    {

    }

    public override void Dispose()
    {
        _deviceClient.Dispose();
        base.Dispose();
    }
}
