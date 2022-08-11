using EScooter.Agent.Raspberry;
using EScooter.Agent.Raspberry.IO.Actuators.Mock;
using EScooter.Agent.Raspberry.IO.Sensors.Mock;
using EScooter.Agent.Raspberry.IotHub;
using EScooter.Agent.Raspberry.Model;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<ScooterWorker>();

        services.AddSingleton<ISpeedometer, MockSpeedometer>();
        services.AddSingleton<IGpsSensor, MockGpsSensor>();
        services.AddSingleton<IBatterySensor, MockBatterySensor>();

        services.AddSingleton<IMagneticBrake, MockMagneticBrake>();
        services.AddSingleton<IMaxSpeedEnforcer, MockMaxSpeedEnforcer>();
        services.AddSingleton<ISpeedDisplay, MockSpeedDisplay>();
        services.AddSingleton<IBatteryDisplay, MockBatteryDisplay>();

        services.AddSingleton<ScooterHardware>();

        services.AddSingleton(_ => new IotHubScooterWrapper(context.Configuration["IotHubConnectionString"]));
    })
    .Build();

await host.RunAsync();
