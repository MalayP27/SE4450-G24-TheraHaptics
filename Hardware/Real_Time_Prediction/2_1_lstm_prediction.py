import serial
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import load_model
from sklearn.preprocessing import StandardScaler
import time

# Load trained model
model = load_model("../Machine_Learning/emg_gesture_lstm_model.h5")

# Serial port configuration
PORT = "COM11"  # Change if needed
BAUD_RATE = 115200
ser = serial.Serial(PORT, BAUD_RATE, timeout=1)
time.sleep(2)  # Allow time for the ESP32 to initialize

# Define the number of sensors and time steps
NUM_SENSORS = 6
TIME_STEPS = 50

# Initialize scaler (same preprocessing as training)
scaler = StandardScaler()

# Buffer to hold real-time data
data_buffer = np.zeros((TIME_STEPS, NUM_SENSORS))

# Gesture labels (adjust based on training labels)
gesture_labels = ["thumbup", "fist"]  # Ensure order matches LabelEncoder

print("Listening for EMG data...")

while True:
    try:
        line = ser.readline().decode("utf-8").strip()
        
        if line:
            values = list(map(int, line.split(",")))

            if len(values) == NUM_SENSORS:
                # Shift buffer and insert new reading
                data_buffer = np.roll(data_buffer, -1, axis=0)
                data_buffer[-1] = values

                # Preprocess input (normalize using scaler)
                input_data = scaler.fit_transform(data_buffer)  # Normalize using live data
                input_data = np.expand_dims(input_data, axis=0)  # Reshape to (1, 50, 6)

                # Make a prediction
                prediction = model.predict(input_data)
                predicted_label = gesture_labels[np.argmax(prediction)]

                # Print the predicted gesture
                print(f"Predicted Gesture: {predicted_label}")

    except KeyboardInterrupt:
        print("\nStopping gesture recognition...")
        ser.close()
        break
    except Exception as e:
        print(f"Error: {e}")
