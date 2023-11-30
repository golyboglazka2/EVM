using Raven.Iot.Device;
using Raven.Iot.Device.GpioExpander;
using System.Device.I2c;
using Iot.Device.RotaryEncoder;
using System.Device.Gpio;
using UnitsNet;

const int D0 = 24;
const int D1 = 25;
const int D2 = 0;

// Проверка наличия устройств с GPIO-расширителями
if (DeviceHelper.GetGpioExpanderDevices() is [I2cConnectionSettings settings])
{
    // Использование GPIO-расширителя с указанными настройками
    using GpioExpander gpioExpander = new(settings);

    gpioExpander.Calibrate(Angle.FromDegrees(180), TimeSpan.FromMicroseconds(600), TimeSpan.FromMicroseconds(2600));

    // Использование энкодера с заданными параметрами
    using ScaledQuadratureEncoder encoder = new ScaledQuadratureEncoder(
        pinA: DeviceHelper.WiringPiToBcm(D0),
        pinB: DeviceHelper.WiringPiToBcm(D1),
        PinEventTypes.Falling,
        pulsesPerRotation: 20,
        pulseIncrement: 1,
        rangeMin: 0.0,
        rangeMax: 180.0);

    // Установка времени дребезга для энкодера
    encoder.Debounce = TimeSpan.FromMilliseconds(2);

    // Обработчик события изменения значения энкодера
    encoder.ValueChanged += (o, e) =>
    {
        // Запись значения угла в указанный выход GPIO-расширителя
        gpioExpander.WriteAngle(D2, Angle.FromDegrees(e.Value));
    };

    Console.ReadKey();
}


