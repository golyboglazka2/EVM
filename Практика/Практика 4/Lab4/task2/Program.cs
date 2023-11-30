using Raven.Iot.Device;
using Raven.Iot.Device.GpioExpander;
using System.Device.I2c;
using Iot.Device.RotaryEncoder;
using System.Device.Gpio;
using UnitsNet;

//Сдано!
const int D0 = 24;
const int D1 = 25;
const int D2 = 0;
const int D3 = 1;

// Проверка наличия подключенных устройств с использованием I2C
if (DeviceHelper.GetGpioExpanderDevices() is [I2cConnectionSettings settings])
{
    // Инициализация объекта GpioExpander с использованием настроек I2C
    using GpioExpander gpioExpander = new(settings);

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

    // Обработчик события изменения значения энкодера
    encoder.ValueChanged += (o, e) =>
    {
        // Установка значения ШИМ для соответствующих пинов
        gpioExpander.AnalogWrite(D2, (int)e.Value);
        gpioExpander.AnalogWrite(D3, (int)e.Value);
    };

    Console.ReadKey();
}


