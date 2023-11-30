// Реализовать вывод показаний с климатических датчиков SHT31 и LPS25HB по сети Wi-Fi в браузер.

#include <SPI.h>
#include <WiFi.h>
#include <string>
#include <TroykaIMU.h>
#include <TroykaMeteoSensor.h>

std::string ssid = "ASOIU";     
std::string password = "kaf.asoiu.48";   
//String ssid = "ASOIU";
//String password = "kaf.asoiu.48";  
int status = WL_IDLE_STATUS;  

Barometer barometer;
TroykaMeteoSensor meteoSensor;

WiFiServer server(80);
int keyIndex = 0; // Индекс для хранения ключа Wi-Fi

void setup() 
{
  Serial.begin(9600);
  while (!Serial)
  {
    continue; // Ожидаем, пока не установится связь с монитором
  }

  if (WiFi.status() == WL_NO_SHIELD) 
  {
    Serial.println("WiFi Shield not present"); // Если нет модуля Wi-Fi, выводим сообщение и завершаем программу
    // don't continue:
    while (true);
  }

  while ( status != WL_CONNECTED) {
    Serial.print("Attempting to connect to WPA SSID: ");
    Serial.println(ssid.c_str());
    status = WiFi.begin(ssid.c_str(), password.c_str()); // Подключаемся к Wi-Fi сети

    delay(5000);
  }

  Serial.println("You're connected to the network");
  Serial.println(WiFi.localIP()); // Выводим локальный IP-адрес устройства
  server.begin(); // Запускаем сервер
  meteoSensor.begin(); // Инициализируем метео-датчик
  barometer.begin(); // Инициализируем барометр
}

void loop() 
{ 
	int stateSensor = meteoSensor.read(); // Считываем данные
  float pressureMillimetersHg = barometer.readPressureMillimetersHg(); // Считываем давление
  float temperature = barometer.readTemperatureC(); // Считываем температуру
	WiFiClient client = server.available(); // Проверяем, есть ли подключенные клиенты к серверу
	
  if (client) 
  { // Если есть подключенные клиенты
		Serial.println("Новый клиент");

		bool currentLineIsBlank = true;

		while (client.connected()) 
    {
			if (client.available()) {
				char c = client.read(); // Считываем символ из запроса клиента
				Serial.write(c);

				if (c == '\n' && currentLineIsBlank)  // Если пришла пустая строка, значит запрос завершен
        {
					client.println("HTTP/1.1 200 OK"); // Отправляем HTTP-заголовки
					client.println("Content-Type: text/html");
					client.println("Connection: close");   // Закрыть соединение после получения ответа
					client.println("Refresh: 5");   // Автоматическое обновление ответа каждые 5 секунд
					client.println();
					client.println("<!DOCTYPE HTML>"); // Отправляем HTML-код с данными
					client.println("<html>");
          client.println("<p>Meteo Sensor Data</p>");
					client.println("<p>Temperature = " + String(meteoSensor.getTemperatureC()) +  " C</p>");
          client.println("<p>Humidity = " + String(meteoSensor.getHumidity()) +  " %</p>");
          client.println("<p>Barometr Data</p>");
          client.println("<p>Pressure = " + String(pressureMillimetersHg) +  " mmHg</p>");
          client.println("<p>Temperature = " + String(temperature) +  " C</p>");
					client.println("</html>");
					break;
				}

				if (c == '\n') 
        {
						
					currentLineIsBlank = true; // Начата новая линия
				} else if (c != '\r') 
        {
						
					currentLineIsBlank = false;// Получен символ на текущей линии
				}
			}
		}

		delay(100);
		client.flush(); // Закрываем соединение
	}
}