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
};
Console.WriteLine("Выбрать единицу измерения: 0 - Цельсия, 1 - Фаренгейт, 3 - Кельвин");
ConsoleKey key = Console.ReadKey().Key;

while (flag)
{
    foreach(var dev in OneWireThermometerDevice.EnumerateDevices())
    {
        Console.WriteLine($"Имя: {dev.DeviceId}");
        switch (key)
        {
            case ConsoleKey.D0:
                Console.WriteLine($"Температура: {dev.ReadTemperature().DegreesCelsius}");
                break;
            case ConsoleKey.D1:
                Console.WriteLine($"Температура: {dev.ReadTemperature().DegreesFahrenheit}");
                break;
            case ConsoleKey.D2:
                Console.WriteLine($"Температура: {dev.ReadTemperature().Kelvins}");
                break;
        }
        Console.WriteLine();
    }
}


