using Raven.Iot.Device;
using Raven.Iot.Device.GpioExpander;
using System.Device.I2c;
using System.Device.Gpio;
using UnitsNet;
using Iot.Device.OneWire;
using Iot.Device.Adc;
using Spire.Xls;
using Iot.Device.RotaryEncoder;

var devOneWire = OneWireThermometerDevice.EnumerateDevices().FirstOrDefault();

int indexer = 10;

while (indexer > 0)
{
    double temp = devOneWire.ReadTemperatureAsync().Result.DegreesCelsius;
    Console.WriteLine($"Temperature = {Math.Round(temp, 2, MidpointRounding.AwayFromZero)}");
    indexer--;
    await Task.Delay(2000);
}


