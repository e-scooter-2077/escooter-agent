using EScooter.Agent.Raspberry;
using EScooter.Agent.Raspberry.IO.Actuators.Mock;
using EScooter.Agent.Raspberry.IO.Sensors.Mock;
using EScooter.Agent.Raspberry.IotHub;
using EScooter.Agent.Raspberry.Model;
using Geolocation;
using UnitsNet;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(config =>
    {
        config.AddEnvironmentVariables("ESCOOTER_");
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<ScooterWorker>();

        services.AddSingleton<ISensor<Speed>, MockSpeedometer>();
        services.AddSingleton<ISensor<Coordinate>, MockGpsSensor>();
        services.AddSingleton<ISensor<Fraction>, MockBatterySensor>();

        services.AddSingleton<IActuator<bool>, MockMagneticBrake>();
        services.AddSingleton<IActuator<Speed>, MockMaxSpeedEnforcer>();

        services.AddSingleton<ScooterHardware>();

        services.AddSingleton(_ => new IotHubScooterWrapper(context.Configuration["IotHubConnectionString"]));
    })
    .Build();

await host.RunAsync();
