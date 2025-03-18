import serial
import csv
import time
import sys

# Configuration
PORT = "COM11"  # Change to match your ESP32's port
BAUD_RATE = 115200
SAMPLE_COUNT = 50  # Number of time steps per sample
NUM_SENSORS = 6  # Number of EMG sensors
GESTURE_LABEL = "thumbup"  # Label for the gesture being recorded
FILENAME = "emg_training_data_thumbup_time_series.csv"

# Connect to Serial
try:
    ser = serial.Serial(PORT, BAUD_RATE, timeout=1)
    print(f"Connected to {PORT} at {BAUD_RATE} baud.")
except serial.SerialException as e:
    print(f"⚠ Error: Could not open serial port {PORT}. {e}")
    sys.exit(1)

time.sleep(2)  # Allow ESP32 to initialize

# Prepare CSV file
header = [f"sensor{i+1}_t{j+1}" for j in range(SAMPLE_COUNT) for i in range(NUM_SENSORS)]
header.append("gesture")  # Last column for label

with open(FILENAME, mode="a", newline="") as file:
    writer = csv.writer(file)
    
    # Write header only if the file is empty
    if file.tell() == 0:
        writer.writerow(header)

    while True:
        print("\nPress Enter to start recording a new sample (or Ctrl+C to exit)...")
        sys.stdin.read(1)  # Wait for a keypress (compatible with VS Code)

        print(f"Recording {SAMPLE_COUNT} time steps per sensor...\n")

        sample_data = []  # Store the 50 time-step readings for all sensors

        for i in range(SAMPLE_COUNT):
            try:
                line = ser.readline().decode("utf-8").strip()
                
                # Debugging: Print raw serial output
                if line:
                    print(f"Raw Serial Data: {line}")

                values = list(map(int, line.split(",")))  # Convert readings to integers

                if len(values) == NUM_SENSORS:
                    sample_data.extend(values)  # Append sensor values for this time step
                    print(f"Time Step {i+1}/{SAMPLE_COUNT}: {values}")  # Print current reading
                else:
                    print("⚠ Invalid data received, skipping...")
                    continue

                time.sleep(0.02)  # 20ms delay (adjust for different sampling rates)

            except ValueError:
                print("⚠ Non-integer data received, skipping...")
            except Exception as e:
                print(f"⚠ Error: {e}")
                break

        if len(sample_data) == SAMPLE_COUNT * NUM_SENSORS:
            sample_data.append(GESTURE_LABEL)  # Append gesture label at the end
            writer.writerow(sample_data)
            print(f"✅ Sample saved for gesture: {GESTURE_LABEL}\n")
        else:
            print("⚠ Incomplete sample, retrying...\n")
