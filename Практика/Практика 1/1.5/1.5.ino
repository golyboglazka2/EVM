//Реализовать задание из пункта №3, но с использованием встроенного аппаратного таймера вместо нажатия на кнопку (модель сфетофора);
#define LED0 D0
#define LED1 D1
#define LED2 D2

unsigned long last_time;
int step = 0;


void setup() {
  pinMode(LED0, OUTPUT);
  pinMode(LED1, OUTPUT);
  pinMode(LED2, OUTPUT);
}

void loop() {
  if(millis() - last_time > 3000){
    last_time = millis();
    switch (step){
      case 0:
        digitalWrite(LED0, 1);
        digitalWrite(LED1, 0);
        digitalWrite(LED2, 0);
        break;
      case 1:
        digitalWrite(LED0, 0);
        digitalWrite(LED1, 1);
        digitalWrite(LED2, 0);
        break;
      case 2:
        digitalWrite(LED0, 0);
        digitalWrite(LED1, 0);
        digitalWrite(LED2, 3);
        break;  
    }
    ++step;
    if (step >= 3) step = 0;
  }
}
