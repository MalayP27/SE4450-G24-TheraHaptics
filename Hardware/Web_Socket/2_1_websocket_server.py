import serial
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import load_model
from sklearn.preprocessing import StandardScaler
import time
import asyncio
import websockets

# Load trained CNN model
model = load_model("../Machine_Learning/emg_gesture_cnn_model.h5")

# Serial port configuration
PORT = "COM11"
BAUD_RATE = 115200
ser = serial.Serial(PORT, BAUD_RATE, timeout=1)
time.sleep(2)  # Allow time for ESP32 to initialize

# Define sensors and time steps
NUM_SENSORS = 6
TIME_STEPS = 50

# Initialize scaler (same preprocessing as training)
scaler = StandardScaler()

# Buffer to hold real-time data
data_buffer = np.zeros((TIME_STEPS, NUM_SENSORS))

# Gesture labels (adjust to match training labels)
gesture_labels = ["neutral", "thumbup", "close", "fist"]  # Ensure order matches LabelEncoder

# WebSocket server setup
CLIENTS = set()

async def websocket_handler(websocket, path):
    """Handles new WebSocket clients."""
    CLIENTS.add(websocket)
    try:
        async for _ in websocket:
            pass  # Keep connection open
    finally:
        CLIENTS.remove(websocket)

async def send_predictions():
    """Reads from serial and sends predictions to WebSocket clients."""
    print("Listening for EMG data and broadcasting predictions...")

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
                    input_data = scaler.fit_transform(data_buffer)
                    input_data = np.expand_dims(input_data, axis=0)  # Reshape to (1, 50, 6)

                    # Make a prediction
                    prediction = model.predict(input_data)
                    predicted_label = gesture_labels[np.argmax(prediction)]

                    # Print and send prediction to WebSocket clients
                    print(f"Predicted Gesture: {predicted_label}")

                    if CLIENTS:
                        await asyncio.wait([client.send(predicted_label) for client in CLIENTS])

        except KeyboardInterrupt:
            print("\nStopping WebSocket server...")
            ser.close()
            break
        except Exception as e:
            print(f"Error: {e}")

async def main():
    """Runs WebSocket server and prediction loop."""
    server = await websockets.serve(websocket_handler, "localhost", 8765)
    print("WebSocket server running on ws://localhost:8765")
    
    await asyncio.gather(send_predictions(), server.wait_closed())

# Start the server
asyncio.run(main())
