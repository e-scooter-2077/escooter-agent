using EScooter.Agent.Raspberry;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(config =>
    {
        config.AddEnvironmentVariables("ESCOOTER_");
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<ScooterWorker>();
    })
    .Build();

await host.RunAsync();
