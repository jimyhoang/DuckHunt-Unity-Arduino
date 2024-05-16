/*
  + Duck Hunt
  + Ver : 2.0 - 04/2024
  + Hoang Minh Quan
  + www.khoahocvui.vn Youtube Channel: Khoa Học Vui
*/

// digital pins :
int sensor = 2; // sensor to pin 2.
int trigger = 3; // trigger of the gun pin 3.
int motor = 4; // motor to vibrate
int eTime = 0; // time delay to control motor
bool freeEvent;

// the setup routine runs once when you press reset:
void setup() {
  pinMode(sensor, INPUT);
  pinMode(trigger, INPUT);
  pinMode(motor, OUTPUT);
  Serial.begin(9600);
  while(!Serial);
}

// the loop routine runs over and over again forever:
void loop() {
  int triggerState = digitalRead(trigger); // 1 or 0
  int sensorState = digitalRead(sensor); // 1 or 0;
  char str[3];
  sprintf(str, "%d,%d",triggerState,sensorState);
  Serial.println(str);
  runMotor();
  delay(1);
}

// Limit the time for the motor to vibrate 
void runMotor(){
  int triggerState = digitalRead(trigger); // 1 or 0
  if(eTime > 0){
     eTime++;
  }
  if((triggerState == 1) && (eTime == 0)){
    eTime = 1;
    digitalWrite(motor,1); // run motor
  }
  if(eTime == 50){  // Delay time for stop motor
    digitalWrite(motor,0);
  }
  if(eTime > 80){  // Delay time for run motor
     eTime = 0;
  }
}