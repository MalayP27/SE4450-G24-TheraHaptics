#define EMG_PIN 34

void setup() {
  Serial.begin(9600);
}

void loop() {
  // Read raw analog value (12-bit ADC: 0-4095)
  Serial.println(analogRead(EMG_PIN));

  // Temporary, for use with Serial Plotter
  Serial.print("900, ");
  Serial.print("1100, ");
  
  delay(50);
}
