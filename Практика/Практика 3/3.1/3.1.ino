// Реализовать подключение к сети Wi-Fi, вывод полученного IP адреса и вывод результата функции пинга для заданного хоста. 
// Вывод осуществляется в последовательный порт (Serial).

#include <SPI.h>
#include <WiFi.h>
#include <string>

// Указываем SSID и пароль для подключения к WiFi
String ssid = "ASOIU";
String password = "kaf.asoiu.48";
int status = WL_IDLE_STATUS; // Переменная для хранения статуса подключения
 
void setup() 
{
  Serial.begin(9600);
  while (!Serial) 
  {
    continue; 
  }

  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("WiFi Shield not present");
    // don't continue:
    while (true);
  }

  while ( status != WL_CONNECTED) {
    Serial.print("Attempting to connect to WPA SSID: ");
    Serial.println(ssid.c_str()); // Выводим сообщение о попытке подключения к сети
    status = WiFi.begin(ssid.c_str(), password.c_str());  // Пытаемся подключиться к Wi-Fi сети с указанными параметрами

    delay(5000);
  }

  Serial.println("You're connected to the network");
  Serial.println(WiFi.localIP()); // Выводим локальный IP-адрес
}

void loop() 
{
  if (Serial.available())
  {
    std::string hostname;
    hostname = Serial.readString().c_str(); // Считываем строку с именем хоста из последовательного порта
    Serial.print("Pinging ");
    Serial.print(hostname.substr(0, hostname.size() - 1).c_str()); // Выводим сообщение о попытке пинга указанного хоста
    Serial.print(": ");

    int pingResult = WiFi.ping(hostname.substr(0, hostname.size() - 1).c_str()); // Пингуем указанный хост

    if (pingResult >= 0) {
      Serial.print("SUCCESS! RTT = ");
      Serial.print(pingResult);  //  Выводим результа, если пинг успешен
      Serial.println(" ms");
    } else {
      Serial.print("FAILED! Error code: ");
      Serial.println(pingResult); // Если пинг не удался, выводим код ошибки
    }
    delay(5000); // Ждем 5 секунд перед следующей попыткой
  }
} 

