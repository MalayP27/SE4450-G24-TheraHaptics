#include <SPI.h>

// SPI
#define CS_PIN   5   // Chip Select pin
#define MOSI_PIN 23  // Master Out Slave In
#define MISO_PIN 19  // Master In Slave Out
#define SCK_PIN  18  // Serial Clock

void setup() {
  Serial.begin(115200);

  pinMode(CS_PIN, OUTPUT);
  digitalWrite(CS_PIN, HIGH); // CS is inactive at the start

  SPI.begin(SCK_PIN, MISO_PIN, MOSI_PIN, CS_PIN); 
}

void loop() {
  SPI.beginTransaction(SPISettings(1000000, MSBFIRST, SPI_MODE0)); // 1 MHz, MSB first, Mode 0
  
  digitalWrite(CS_PIN, LOW); // Activate CS
  delayMicroseconds(10);     // Short delay to allow slave to recognize CS
  
  uint8_t receivedData = SPI.transfer(0x42); // Send 0x42, read response
  
  digitalWrite(CS_PIN, HIGH); // Deactivate CS
  SPI.endTransaction();

  Serial.print("Received: 0x");
  Serial.println(receivedData, HEX);

  delay(1000); // Wait 1 second before next transmission
}
