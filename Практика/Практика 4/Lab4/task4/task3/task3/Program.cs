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
//////////////////////////////////task 4
if (DeviceHelper.GetIna219Devices() is [I2cConnectionSettings settings])
{
    //Создание объекта Workbook 
    Workbook workbook = new Workbook();

    //Получение первой рабочей страницы
    Worksheet worksheet = workbook.Worksheets[0];

    worksheet.Range[1, 1].Value = "mA";
    worksheet.Range[1, 2].Value = "mV";
    worksheet.Range[1, 3].Value = "mW";

    using Ina219 ina = new Ina219(settings);

    while (indexer < 6)
    {
        double I = ina.ReadCurrent().Milliamperes;
        double V = ina.ReadBusVoltage().Millivolts;
        double P = (double)ina.ReadPower().Milliwatts;

        worksheet.Range[indexer + 2, 1].Value = I.ToString();
        worksheet.Range[indexer + 2, 2].Value = V.ToString();
        worksheet.Range[indexer + 2, 3].Value = P.ToString();

        Console.WriteLine($"mesurment {indexer+1}: I = {I}mA, V = {V}mV, P = {P}mW");
    }
    workbook.SaveToFile("WriteToCells.xlsx", ExcelVersion.Version2016);
}


///////////////////task5
const int D0 = 24;
const int D1 = 25;

if (DeviceHelper.GetGpioExpanderDevices() is [I2cConnectionSettings settings1])
{
    // Инициализация объекта GpioExpander с использованием настроек I2C
    using GpioExpander gpioExpander = new(settings1);

    // Установка частоты ШИМ на 25 кГц
    gpioExpander.SetPwmFrequency(Frequency.FromKilohertz(25));

    // Инициализация объекта ScaledQuadratureEncoder
    using ScaledQuadratureEncoder encoder = new ScaledQuadratureEncoder(
        pinA: DeviceHelper.WiringPiToBcm(D0),
        pinB: DeviceHelper.WiringPiToBcm(D1),
        PinEventTypes.Falling,
        pulsesPerRotation: 20,
        pulseIncrement: 1,
        rangeMin: 0.0,
        rangeMax: 255.0);

    // Установка времени дебаунса для энкодера
    encoder.Debounce = TimeSpan.FromMilliseconds(2);

    if (DeviceHelper.I2cDeviceSearch(new ReadOnlySpan<int>(1), new ReadOnlySpan<int>(0x2C)) is [I2cConnectionSettings settings2])
    {
        using I2cDevice pico  = I2cDevice.Create(settings2);
        // Обработчик события изменения значения энкодера
        encoder.ValueChanged += (o, e) =>
        {
            // запись шим на пику
            pico.WriteByte((byte)e.Value);
        };
    }
    Console.ReadKey();
}


