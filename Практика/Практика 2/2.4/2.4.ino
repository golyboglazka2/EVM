/*ЧАСТЬ 4: Используя библиотеки TroykaMeteoSensor и Troyka-IMU, а также последовательны порт (Serial) реализовать вывод показаний 
с климатических датчиков SHT31 и LPS25HB. Шина I2C.*/

#include <Wire.h>                   // Подключаем библиотеку для работы с I2C
#include <TroykaMeteoSensor.h>      // Подключаем библиотеку для работы с метеодатчиком
#include <TroykaIMU.h>              // Подключаем библиотеку для работы с акселерометром

TroykaMeteoSensor meteoSensor;     // Создаём объект для работы с метеодатчиком
Barometer barometer;               // Создаём объект для работы с барометром

void setup() {
  Serial.begin(9600);             // Инициализируем коммуникацию с монитором через Serial порт
  Wire.begin();                   // Инициализируем I2C шину
  meteoSensor.begin();            // Инициализируем датчик метео
  barometer.begin();              // Инициализируем барометр
}

void meteoInfo(){
  int stateSensor = meteoSensor.read();
  switch (stateSensor) {
    case SHT_OK:

      // выводим показания влажности и температуры
      Serial.print("Температура на SHT = ");
      Serial.print(meteoSensor.getTemperatureC());
      Serial.println(" C \t");
  
      Serial.print("Влажность = ");
      Serial.print(meteoSensor.getHumidity());
      Serial.println(" %\r\n");
      break;
    default:
       Serial.println("Ошибка в данных или датчик не подключен");
  }
}

void barometerInfo(){

   // Вывод данных в Serial-порт
    Serial.print("Давление: ");
    Serial.print(barometer.readPressureMillimetersHg());
    Serial.print("  мм рт. ст.");
    
    Serial.print("Высота над уровнем моря: ");
    Serial.print(barometer.readAltitude());
    Serial.print(" m \t");

    Serial.print("Температура на LPS: ");
    Serial.print(barometer.readTemperatureC());
    Serial.println(" C");
    Serial.println(" \n");
}

void loop() {
  meteoInfo();
  barometerInfo();
  delay(1000);
}
