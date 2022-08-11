using EScooter.Agent.Raspberry.Dto;
using EScooter.Agent.Raspberry.Model;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace EScooter.Agent.Raspberry.IotHub;

public class IotHubScooterWrapper : IDisposable
{
    private readonly DeviceClient _deviceClient;

    public IotHubScooterWrapper(string connectionString)
    {
        _deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }

    public async Task RegisterForDesiredPropertyUpdates(Func<ScooterDesiredState, Task> callback, CancellationToken stoppingToken = default) =>
        await _deviceClient.SetDesiredPropertyUpdateCallbackAsync((t, _) => callback(ToDesiredState(t.ToJson())), null, stoppingToken);

    public async Task<ScooterDesiredState> GetDesiredState(CancellationToken stoppingToken = default)
    {
        var twin = await _deviceClient.GetTwinAsync(stoppingToken);
        return ToDesiredState(twin.Properties.Desired.ToJson());
    }

    public async Task SendSensorsTelemetry(ScooterSensorsState sensorsState)
    {
        using var message = CreateMessageFromJson(JsonConvert.SerializeObject(ScooterTelemetryDto.FromSensorsState(sensorsState)));
        await _deviceClient.SendEventAsync(message);
    }

    public Task SendReportedState(ScooterReportedState state)
    {
        var twinCollection = new TwinCollection(JsonConvert.SerializeObject(ScooterReportedDto.FromReportedState(state)));
        return _deviceClient.UpdateReportedPropertiesAsync(twinCollection);
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

    public void Dispose()
    {
        _deviceClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
