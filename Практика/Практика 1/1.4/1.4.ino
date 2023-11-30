
/*4 ЧАСТЬ ЗАДАНИЯ: Реализовать задание из пункта №3, но с использованием аппаратного прерывания при нажатии на кнопку вместо использования функции digitalRead; (модель сфетофора)*/
#define BTN D3
#define LED0 D0
#define LED1 D1
#define LED2 D2

uint8_t CurrentLED = 0;

void setup() {
  // put your setup code here, to run once:
  pinMode(LED0, OUTPUT);
  pinMode(LED1, OUTPUT);
  pinMode(LED2, OUTPUT);
  pinMode(BTN, INPUT_PULLUP);
  attachInterrupt(BTN, hend, FALLING);
}

void loop() {}

void hend()
{
  if (CurrentLED == 0)
    {
      digitalWrite(LED0, 1);
      digitalWrite(LED1, 0);
      digitalWrite(LED2, 0);
      CurrentLED = 1;
    }
    else {
      if (CurrentLED == 1)
      {
        digitalWrite(LED0, 0);
        digitalWrite(LED1, 1);
        digitalWrite(LED2, 0);
        CurrentLED = 2;
      }
      else {
        if (CurrentLED == 2)
        {
          digitalWrite(LED0, 0);
          digitalWrite(LED1, 0);
          digitalWrite(LED2, 1);
          CurrentLED = 3;
        }
        else 
        {
          digitalWrite(LED0, 0);
          digitalWrite(LED1, 0);
          digitalWrite(LED2, 0);
          CurrentLED = 0;
        }
      }
    }
}