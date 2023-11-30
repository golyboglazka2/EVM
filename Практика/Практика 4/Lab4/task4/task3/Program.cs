using Raven.Iot.Device;
using Raven.Iot.Device.GpioExpander;
using System.Device.I2c;
using System.Device.Gpio;
using UnitsNet;
using Iot.Device.OneWire;
using Iot.Device.Adc;
using Spire.Xls;
using Iot.Device.RotaryEncoder;

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


        double I = ina.ReadCurrent().Milliamperes;
        double V = ina.ReadBusVoltage().Millivolts;
        double P = (double)ina.ReadPower().Milliwatts;

        worksheet.Range[indexer + 2, 1].Value = I.ToString();
        worksheet.Range[indexer + 2, 2].Value = V.ToString();
        worksheet.Range[indexer + 2, 3].Value = P.ToString();

        Console.WriteLine($"mesurment {indexer+1}: I = {I}mA, V = {V}mV, P = {P}mW");
    
    workbook.SaveToFile("WriteToCells.xlsx");
}


