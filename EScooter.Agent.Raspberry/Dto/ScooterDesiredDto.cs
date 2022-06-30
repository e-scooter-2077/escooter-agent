using Newtonsoft.Json;

namespace EScooter.Agent.Raspberry.Dto;

public record ScooterDesiredDto(
    bool? Locked,
    TimeSpan? UpdateFrequency,
    double? MaxSpeed)
{
    public static ScooterDesiredDto FromJson(string json) =>
        JsonConvert.DeserializeObject<ScooterDesiredDto>(json);
}
