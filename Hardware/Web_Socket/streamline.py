import asyncio
import websockets
from serial.tools import list_ports
import serial
import time
import os

ports = list_ports.comports()
for port in ports:
    print(port)

serialCom = serial.Serial("COM12", 9600)
serialCom.setDTR(False)
time.sleep(1)
serialCom.flushInput()
serialCom.setDTR(True)

async def handle_client(websocket, path):
    global clients
    clients.add(websocket)
    try:
        async for message in websocket:
            print(f"Received message from client: {message}")
    except websockets.ConnectionClosed:
        print("Client disconnected")
    finally:
        clients.remove(websocket)

async def serial_to_websocket():
    while True:
        try:
            s_bytes = serialCom.readline()
            decoded_bytes = s_bytes.decode("utf-8").strip('\r\n')
            print(f"Serial Data: {decoded_bytes}")

            if clients:
                await asyncio.gather(
                    *[client.send(decoded_bytes) for client in clients]
                )
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