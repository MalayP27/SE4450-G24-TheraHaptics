import asyncio
import websockets

async def echo(websocket, path):  # Accepts both websocket and path
    await websocket.send("Hello Client!")

async def main():
    server = await websockets.serve(echo, "127.0.0.1", 9090)
    await server.wait_closed()

asyncio.run(main())
