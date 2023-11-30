using Raven.Iot.Device;
using Raven.Iot.Device.GpioExpander;
using System.Device.I2c;
using System.Device.Gpio;
using UnitsNet;
using Iot.Device.OneWire;
using Iot.Device.Adc;
using Spire.Xls;
using Iot.Device.RotaryEncoder;

//var devOneWire = OneWireThermometerDevice.EnumerateDevices().FirstOrDefault();

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


