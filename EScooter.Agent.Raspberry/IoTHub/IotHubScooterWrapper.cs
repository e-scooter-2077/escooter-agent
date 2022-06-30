using EScooter.Agent.Raspberry.Dto;
using EScooter.Agent.Raspberry.Model;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;

namespace EScooter.Agent.Raspberry.IotHub;

public class IotHubScooterWrapper : IDisposable
{
    private readonly DeviceClient _deviceClient;

    private IotHubScooterWrapper(DeviceClient deviceClient)
    {
        _deviceClient = deviceClient;
    }

    public static IotHubScooterWrapper CreateFromConfiguration(IConfiguration configuration)
    {
        var scooterId = configuration["Scooter:Id"];
        var key = configuration["Scooter:SymmetricKey"];
        var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(scooterId, key);

        var hostName = configuration["IoTHubHostName"];
        return new(DeviceClient.Create(hostName, authMethod, TransportType.Mqtt));
    }

    public async Task RegisterForDesiredPropertyUpdates(Func<ScooterDesiredState, Task> callback, CancellationToken stoppingToken = default) =>
        await _deviceClient.SetDesiredPropertyUpdateCallbackAsync((t, _) => callback(ToDesiredState(t.ToJson())), null, stoppingToken);

    public async Task<ScooterDesiredState> GetDesiredState(CancellationToken stoppingToken = default)
    {
        var twin = await _deviceClient.GetTwinAsync(stoppingToken);
        return ToDesiredState(twin.Properties.Desired.ToJson());
    }

    public async Task SendTelemetry(ScooterTelemetryDto telemetry)
    {
        using var message = CreateMessageFromJson(JsonConvert.SerializeObject(telemetry));
        await _deviceClient.SendEventAsync(message);
    }

    private Message CreateMessageFromJson(string jsonSerializedTelemetry)
    {
        return new Message(Encoding.UTF8.GetBytes(jsonSerializedTelemetry))
        {
            ContentEncoding = Encoding.UTF8.WebName,
            ContentType = "application/json",
        };
    }

    private ScooterDesiredState ToDesiredState(string json) => ScooterDesiredDto.FromJson(json).ToDesiredState();

    public void Dispose() => _deviceClient.Dispose();
}
