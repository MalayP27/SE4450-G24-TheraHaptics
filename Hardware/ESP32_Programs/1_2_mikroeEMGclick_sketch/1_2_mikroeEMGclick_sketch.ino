#define EMG_PIN 34

void setup() {
  Serial.begin(115200);
  pinMode(EMG_PIN, INPUT);
}

void loop() {
  // Read raw analog value (12-bit ADC: 0-4095)
  int rawValue = analogRead(EMG_PIN);

  // Convert raw value to voltage
  float voltage = (rawValue / 4095.0) * 3.3;

  // Print the results
  Serial.print("Raw Value: ");
  Serial.print(rawValue);
  Serial.print(" | Voltage: ");
  Serial.print(voltage, 3);  // 3 decimal places
  Serial.println(" V");

  delay(50);  // Adjust delay for sampling rate (e.g., 50ms = 20Hz)
}
