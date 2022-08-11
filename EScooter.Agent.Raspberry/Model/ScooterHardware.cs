namespace EScooter.Agent.Raspberry.Model;

public record ScooterHardware(
    ISpeedometer Speedometer,
    IGpsSensor Gps,
    IBatterySensor Battery,
    IMagneticBrake MagneticBrakes,
    IMaxSpeedEnforcer MaxSpeedEnforcer,
    ISpeedDisplay SpeedDisplay,
    IBatteryDisplay BatteryDisplay);
