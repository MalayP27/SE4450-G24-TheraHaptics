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

