import asyncio
import websockets
import os
import joblib
import serial

script_dir = os.path.dirname(os.path.abspath(__file__))
os.chdir(script_dir)
print("Current working directory:", os.getcwd())

# Load the trained model
model = joblib.load('../Machine_Learning/model.pkl')

# Initialize Serial connection
serialCom = serial.Serial('COM11', 9600)  # Replace 'COM3' with your actual COM port

# Define the window size
window_size = 50  # Adjust based on your data

# Initialize variables
sensor1_data = []
sensor2_data = []
sensor3_data = []

# Initialize the clients set
clients = set()

async def handle_client(websocket, path):
    global clients
    clients.add(websocket)
    try:
        while True:
            await asyncio.sleep(0.1)
    except websockets.ConnectionClosed:
        print("Client disconnected")
    finally:
        clients.remove(websocket)

async def serial_to_websocket():
    global sensor1_data, sensor2_data, sensor3_data  # Declare global variables
    while True:
        try:
            s_bytes = serialCom.readline()
            decoded_bytes = s_bytes.decode("utf-8").strip()
            print(f"Serial Data: {decoded_bytes}")

            # Split the string into individual numbers
            values = list(map(int, decoded_bytes.split()))
            print(f"Parsed Values: {values}")

            # Assuming you have three sensors
            sensor1_data.append(values[0])
            sensor2_data.append(values[1])
            sensor3_data.append(values[2])

            # Maintain the window size
            if len(sensor1_data) > window_size:
                sensor1_data.pop(0)
                sensor2_data.pop(0)
                sensor3_data.pop(0)

            # Perform prediction using the model
            if len(sensor1_data) == window_size:
                input_data = sensor1_data + sensor2_data + sensor3_data
                prediction = model.predict([input_data])
                print(f"Prediction: {prediction}")

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