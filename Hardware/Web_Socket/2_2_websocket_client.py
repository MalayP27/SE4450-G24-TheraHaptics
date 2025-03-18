import asyncio
import websockets

async def receive_predictions():
    """Connects to the WebSocket server and receives predictions."""
    uri = "ws://localhost:8765"
    async with websockets.connect(uri) as websocket:
        print("Connected to WebSocket server. Listening for predictions...\n")
        while True:
            try:
                prediction = await websocket.recv()
                print(f"Received Prediction: {prediction}")
            except websockets.exceptions.ConnectionClosed:
                print("WebSocket connection closed.")
                break

# Run client
asyncio.run(receive_predictions())
