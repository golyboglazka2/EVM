// 1 ЧАСТЬ ЗАДАНИЯ: Светодиод подключенным к пину D0 мигает каждые 5 секунд;
// void setup() {
//   // put your setup code here, to run once:
  pinMode (0, OUTPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
  digitalWrite(0, HIGH);
  delay(500);
  digitalWrite(0, LOW);
  delay(500);
}
