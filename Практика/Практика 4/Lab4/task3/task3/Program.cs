using Raven.Iot.Device;
using Raven.Iot.Device.GpioExpander;
using System.Device.I2c;
using System.Device.Gpio;
using UnitsNet;
using Iot.Device.OneWire;
using Iot.Device.Adc;
using Spire.Xls;
using Iot.Device.RotaryEncoder;

bool flag = true;

Console.CancelKeyPress += (sender, EventArgs) =>
{
    flag = false;
};;

while (flag)
{
    foreach(var dev in OneWireThermometerDevice.EnumerateDevices())
    {
        Console.WriteLine($"Имя: {dev.DeviceId}");
        Console.WriteLine($"Температура: {dev.ReadTemperature():g}");
        Console.WriteLine();
    }
}


