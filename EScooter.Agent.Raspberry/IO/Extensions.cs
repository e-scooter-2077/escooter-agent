using EScooter.Agent.Raspberry.IO.Actuators.Gpio;
using EScooter.Agent.Raspberry.IO.Actuators.Mock;
using EScooter.Agent.Raspberry.IO.Sensors.Gpio;
using EScooter.Agent.Raspberry.IO.Sensors.Mock;
using EScooter.Agent.Raspberry.Model;
using Iot.Device.CharacterLcd;
using System.Device.Gpio;

namespace EScooter.Agent.Raspberry.IO;

public static class Extensions
{
    public static IServiceCollection AddMockHardware(this IServiceCollection services)
    {
        services.AddSingleton<ISpeedometer, MockSpeedometer>();
        services.AddSingleton<IBatterySensor, MockBatterySensor>();
        services.AddSingleton<IGpsSensor, MockGpsSensor>();

        services.AddSingleton<IMagneticBrake, MockMagneticBrake>();
        services.AddSingleton<IMaxSpeedEnforcer, MockMaxSpeedEnforcer>();
        services.AddSingleton<ISpeedDisplay, MockSpeedDisplay>();
        services.AddSingleton<IBatteryDisplay, MockBatteryDisplay>();
        services.AddSingleton<IStandbyIndicator, MockStandbyIndicator>();

        return services;
    }

    public static IServiceCollection AddGpioHardware(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ => new GpioController(PinNumberingScheme.Board));

        // TODO: when we have an ADC, use Gpio implementations instead of the Mock ones.
        services.AddSingleton<ISpeedometer, MockSpeedometer>();
        services.AddSingleton<IBatterySensor, MockBatterySensor>();
        services.AddSingleton<IGpsSensor>(_ => new GpioGpsSensorV2(configuration.GetValue<string>("Gps:SerialPortName")));

        services.AddSingleton<IMagneticBrake>(p => new GpioMagneticBrake(
            configuration.GetValue<int>("Brakes:Pin"),
            p.GetRequiredService<GpioController>()));

        services.AddSingleton<IStandbyIndicator>(p => new GpioStandbyIndicator(
            configuration.GetValue<int>("StandbyIndicator:Pin"),
            p.GetRequiredService<GpioController>()));

        services.AddSingleton(p => LcdInterface.CreateGpio(
            registerSelectPin: configuration.GetValue<int>("Lcd:RegisterSelectPin"),
            enablePin: configuration.GetValue<int>("Lcd:EnablePin"),
            dataPins: configuration.GetSection("Lcd:DataPins").Get<int[]>(),
            controller: p.GetRequiredService<GpioController>(),
            shouldDispose: false));
        services.AddSingleton<Lcd1602>();
        services.AddSingleton<LcdDisplay>();

        services.AddSingleton<IMaxSpeedEnforcer, GpioMaxSpeedEnforcer>();
        services.AddSingleton<IBatteryDisplay, GpioBatteryDisplay>();
        services.AddSingleton<ISpeedDisplay, GpioSpeedDisplay>();

        return services;
    }
}
