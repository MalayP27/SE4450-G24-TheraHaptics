#define NUM_SENSORS 6
const int EMG_PINS[NUM_SENSORS] = {39, 34, 35, 4, 2, 15};

void setup() {
  Serial.begin(115200);
  // Set ADC resolution (ESP32 defaults to 12-bit, so this is optional)
  analogReadResolution(12);  
}

void loop() {
  for (int i = 0; i < NUM_SENSORS; i++) {
    Serial.print(analogRead(EMG_PINS[i]));
    if (i < NUM_SENSORS - 1) Serial.print(", ");
  }
  
  Serial.println();  // Move to next line for Serial Plotter
  
  delay(5); // Adjust delay to control data frequency (1 ms may be too fast)
}