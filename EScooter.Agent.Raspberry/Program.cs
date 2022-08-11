using EScooter.Agent.Raspberry;
using EScooter.Agent.Raspberry.IO;
using EScooter.Agent.Raspberry.IotHub;
using EScooter.Agent.Raspberry.Model;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<ScooterWorker>();

        if (context.HostingEnvironment.IsDevelopment())
        {
            services.AddMockHardware();
        }
        else
        {
            services.AddGpioHardware(context.Configuration);
        }

        services.AddSingleton<ScooterHardware>();

        services.AddSingleton(_ => new IotHubScooterWrapper(context.Configuration["IotHubConnectionString"]));
    })
    .Build();

await host.RunAsync();
