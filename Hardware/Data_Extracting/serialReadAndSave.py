import serial
import csv
import time

# Configuration
PORT = "COM3"  # Change this to match your ESP32's serial port
BAUD_RATE = 115200
SAMPLE_COUNT = 50  # Number of readings per sensor (adjust as needed)
NUM_SENSORS = 6  # Number of EMG sensors
GESTURE_LABEL = "neutral"  # Label for the gesture being recorded
FILENAME = "emg_training_data_neutral.csv"

# Connect to Serial
ser = serial.Serial(PORT, BAUD_RATE, timeout=1)
time.sleep(2)  # Allow time for ESP32 to initialize

# Prepare CSV file
header = [f"sensor{i+1}_reading{j+1}" for i in range(NUM_SENSORS) for j in range(SAMPLE_COUNT)]
header.append("gesture")  # Last column for label

with open(FILENAME, mode="a", newline="") as file:
    writer = csv.writer(file)
    
    # Write header only if the file is empty
    if file.tell() == 0:
        writer.writerow(header)

    while True:
        input("Press Enter to start recording a new sample...")
        print(f"Recording {SAMPLE_COUNT} samples per sensor...")

        sample_data = []  # Store all sensor readings

        for _ in range(SAMPLE_COUNT):
            try:
                line = ser.readline().decode("utf-8").strip()
                values = list(map(int, line.split(",")))  # Convert readings to integers

                if len(values) == NUM_SENSORS:
                    sample_data.extend(values)  # Append readings
                else:
                    print("Invalid data received, skipping...")
                    continue

                time.sleep(0.02)  # 20ms delay (adjust as needed)

            except Exception as e:
                print(f"Error: {e}")
                break

        if len(sample_data) == SAMPLE_COUNT * NUM_SENSORS:
            sample_data.append(GESTURE_LABEL)  # Append gesture label
            writer.writerow(sample_data)
            print(f"Sample saved for gesture: {GESTURE_LABEL}")
        else:
            print("Incomplete sample, retrying...")

