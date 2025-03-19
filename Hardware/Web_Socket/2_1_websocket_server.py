import socket
import serial
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import load_model
from sklearn.preprocessing import StandardScaler
import threading

# Load trained CNN model
model = load_model("../Machine_Learning/emg_gesture_cnn_model.h5")

# Serial port configuration
PORT = "COM11"
BAUD_RATE = 115200
ser = serial.Serial(PORT, BAUD_RATE, timeout=1)

# Define sensors and time steps
NUM_SENSORS = 6
TIME_STEPS = 50

# Initialize scaler
scaler = StandardScaler()

# Buffer to hold real-time data
data_buffer = np.zeros((TIME_STEPS, NUM_SENSORS))

# Gesture labels
gesture_labels = ["thumbup", "fist"]

# Server Configuration
HOST = "127.0.0.1"
PORT = 9090
clients = []  # List to store connected clients
lock = threading.Lock()  # Prevent race conditions

def handle_client(client_socket, addr):
    """Handles a single client connection."""
    print(f"🔗 Client connected: {addr}")
    with lock:
        clients.append(client_socket)  # Store client

    try:
        while True:
            data = client_socket.recv(1024)
            if not data:
                break  # Client disconnected
    except (ConnectionResetError, BrokenPipeError):
        print(f"❌ Client {addr} disconnected.")
    finally:
        with lock:
            clients.remove(client_socket)
        client_socket.close()

def start_server():
    """Starts the TCP server to accept client connections."""
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((HOST, PORT))
    server_socket.listen(5)
    print(f"🚀 Server running on {HOST}:{PORT}")

    while True:
        client_socket, addr = server_socket.accept()
        threading.Thread(target=handle_client, args=(client_socket, addr), daemon=True).start()

def send_predictions():
    """Reads EMG data, predicts gestures, and sends predictions to clients."""
    global data_buffer

    print("📡 Listening for EMG data and sending predictions...")

    while True:
        try:
            line = ser.readline().decode("utf-8").strip()
            if line:
                values = list(map(int, line.split(",")))

                if len(values) == NUM_SENSORS:
                    data_buffer = np.roll(data_buffer, -1, axis=0)
                    data_buffer[-1] = values

                    input_data = scaler.fit_transform(data_buffer)
                    input_data = np.expand_dims(input_data, axis=0)  # Reshape to (1, 50, 6)

                    prediction = model.predict(input_data)
                    predicted_label = gesture_labels[np.argmax(prediction)]

                    print(f"🎯 Predicted Gesture: {predicted_label}")

                    # Send prediction to all connected clients
                    with lock:
                        for client in clients[:]:  # Copy list to prevent modification issues
                            try:
                                client.send(predicted_label.encode('utf-8'))
                            except (ConnectionResetError, BrokenPipeError):
                                clients.remove(client)  # Remove disconnected client

        except Exception as e:
            print(f"❌ Error: {e}")

if __name__ == "__main__":
    threading.Thread(target=start_server, daemon=True).start()
    send_predictions()  # Runs in the main thread
