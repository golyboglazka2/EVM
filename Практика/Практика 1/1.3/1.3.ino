/* 3 ЧАСТЬ ЗАДАНИЯ: Светодиоды подключенные к пинам D0, D1, D2 загораются по очереди. 
Смена очереди происходит при нажатии на кнопку подключенную к пину D3 (Модель светофора);*/
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

}

void loop() {
  // put your main code here, to run repeatedly:
  if (digitalRead(BTN) == LOW) {
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
    
    delay(200);
  }
}