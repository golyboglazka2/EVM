// 2 ЧАСТЬ ЗАДАНИЯ: Светодиод подключенным к пину D0 загорается при нажатии на кнопку подключенную к пину D1;
//          1 вариант: светодиод горит при зажатой кнопки и не горит, если отпустить.
// void setup(){
//   pinMode(0, OUTPUT);
//   pinMode(1, INPUT_PULLUP);
// }
//  void loop(){
//   if(digitalRead(1) == HIGH){
//     digitalWrite(0, LOW);
//   } else{
//     digitalWrite(0, HIGH)
//   }
//  }

//          2 вариант: светодиод переключает состояние по кнопке.
#include <EncButton.h>

Button btn1(1);

void setup() {
  pinMode(0, OUTPUT);
}

void loop() {
  // опрос кнопки происходит здесь
  btn1.tick();
  // клик по кнопке - переключить светодиод на D0 пине
  if (btn1.click()) digitalWrite(0, !digitalRead(0));
}