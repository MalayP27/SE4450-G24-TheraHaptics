from serial.tools import list_ports
import serial
import time
import csv
import os

ports = list_ports.comports()
for port in ports: print(port)

output_dir = "Hardware/Data Processing"
os.makedirs(output_dir, exist_ok=True)

f = open(os.path.join(output_dir, "output.csv"), mode="w", newline='')
f.truncate()

serialCom = serial.Serial("COM12", 9600)

serialCom.setDTR(False)
time.sleep(1)
serialCom.flushInput()
serialCom.setDTR(True)

kmax = 15
for k in range(kmax):
    try:
        s_bytes = serialCom.readline()
        decoded_bytes = s_bytes.decode("utf-8").strip('\r\n')
        #print(decoded_bytes)

        if k == 0:
            values = decoded_bytes.split(",")
        else:
            values = [float(x) for x in decoded_bytes.split()]
        print(values)

        writer = csv.writer(f, delimiter=",")
        writer.writerow(values)
    except:
        print("Failed to read from serial port")

f.close()