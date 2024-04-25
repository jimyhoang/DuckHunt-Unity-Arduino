/*
  + Duck Hunt
  + Ver : 2.0 - 04/2024
  + 11/2020
  + Hoang Minh Quan
  + www.khoahocvui.vn Youtube Channel: Khoa Học Vui
*/

// digital pins :
int sensor = 2; // sensor to pin 2.
int trigger = 3; // trigger of the gun pin 3.
bool freeEvent;

// the setup routine runs once when you press reset:
void setup() {
  pinMode(sensor, INPUT);
  pinMode(trigger, INPUT);
  Serial.begin(9600);
  while(!Serial);
}

// the loop routine runs over and over again forever:
void loop() {
  int triggerState = digitalRead(trigger); // 1 or 0
  int sensorState = digitalRead(sensor); // 1 or 0;
  //if(triggerState == 1){
    char str[3];
    sprintf(str, "%d,%d",triggerState,sensorState );
    Serial.println(str);
 // }
  delay(1);
}
