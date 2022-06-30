using EScooter.Agent.Raspberry.Model;
using Newtonsoft.Json;
using UnitsNet;

namespace EScooter.Agent.Raspberry.Dto;

public record ScooterDesiredDto(
    bool? Locked,
    TimeSpan? UpdateFrequency,
    double? MaxSpeed)
{
    public static ScooterDesiredDto FromJson(string json) =>
        JsonConvert.DeserializeObject<ScooterDesiredDto>(json);

    public ScooterDesiredState ToDesiredState() => new(
        Locked ?? true,
        UpdateFrequency ?? TimeSpan.FromSeconds(10),
        MaxSpeed is null ? Speed.FromKilometersPerHour(25) : Speed.FromMetersPerSecond(MaxSpeed.Value));
}
