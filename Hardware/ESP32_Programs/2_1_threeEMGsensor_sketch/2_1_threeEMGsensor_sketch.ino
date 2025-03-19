#define EMG_PINone 39
#define EMG_PINtwo 34
#define EMG_PINthree 35

void setup() {
  Serial.begin(115200);
}

void loop() {
  // Read raw analog value (12-bit ADC: 0-4095)
  Serial.print(analogRead(EMG_PINone));
  Serial.print(", ");

  Serial.print(analogRead(EMG_PINtwo));
  Serial.print(", ");

  Serial.print(analogRead(EMG_PINthree));
  Serial.print(", ");

  // Temporary, for use with Serial Plotter
  Serial.println();
  
  delay(50);
}