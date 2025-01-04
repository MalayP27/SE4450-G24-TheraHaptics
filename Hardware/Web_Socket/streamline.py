import asyncio
import websockets
from serial.tools import list_ports
import serial
import time
import os

ports = list_ports.comports()
for port in ports:
    print(port)