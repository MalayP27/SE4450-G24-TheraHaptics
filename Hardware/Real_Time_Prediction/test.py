import joblib
import numpy as np
import serial
import asyncio
import websockets
import os

script_dir = os.path.dirname(os.path.abspath(__file__))
os.chdir(script_dir)
print("Current working directory:", os.getcwd())

# Load the trained model
model = joblib.load('../Machine_Learning/model.pkl')

# Initialize Serial connection
serialCom = serial.Serial('COM3', 115200)  # Replace 'COM3' with your actual COM port

# Define the window size
window_size = 50  # Adjust based on your data

# Initialize variables
sensor1_data = []
sensor2_data = []
sensor3_data = []

# Function to extract features
def extract_features(data):
    variance = np.var(data)
    mean = np.mean(data)
    std_dev = np.std(data)
    return [variance, mean, std_dev]

clients = set()

async def handle_client(websocket, path):
    clients.add(websocket)
    try:
        async for message in websocket:
            print(f"Received message from client: {message}")
    except websockets.ConnectionClosed:
        print("Client disconnected")
    finally:
        clients.remove(websocket)

async def serial_to_websocket():
    global sensor1_data, sensor2_data, sensor3_data

    while True:
        try:
            s_bytes = serialCom.readline()
            decoded_bytes = s_bytes.decode("utf-8").strip('\r\n')
            print(f"Serial Data: {decoded_bytes}")

            sensor1, sensor2, sensor3 = map(int, decoded_bytes.split(','))

            # Append data to the lists
            sensor1_data.append(sensor1)
            sensor2_data.append(sensor2)
            sensor3_data.append(sensor3)

            # Check if we have enough data for a window
            if len(sensor1_data) >= window_size:
                # Extract features for each sensor
                features1 = extract_features(sensor1_data[-window_size:])
                features2 = extract_features(sensor2_data[-window_size:])
                features3 = extract_features(sensor3_data[-window_size:])

                # Combine features into a single feature vector
                features = np.array(features1 + features2 + features3).reshape(1, -1)

                # Predict using the model
                prediction = model.predict(features)
                print(f'Prediction: {prediction[0]}')

                # Send the prediction to WebSocket clients
                if clients:
                    await asyncio.gather(
                        *[client.send(str(prediction[0])) for client in clients]
                    )

                # Optionally, clear the data lists or use a sliding window approach
                sensor1_data = sensor1_data[-window_size:]
                sensor2_data = sensor2_data[-window_size:]
                sensor3_data = sensor3_data[-window_size:]

        except Exception as e:
            print(f"Error: {e}")
        await asyncio.sleep(0.1)

async def main():
    websocket_server = websockets.serve(handle_client, "0.0.0.0", 8765)

    await asyncio.gather(
        websocket_server,
        serial_to_websocket()
    )

if __name__ == "__main__":
    asyncio.run(main())