import socket

# Server details
HOST = "127.0.0.1"
PORT = 9090

def receive_predictions():
    """Connects to the TCP server and receives predictions."""
    print("🔄 Attempting to connect to server...")

    try:
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client_socket:
            client_socket.connect((HOST, PORT))
            print("✅ Connected to server. Listening for predictions...\n")

            while True:
                prediction = client_socket.recv(1024).decode('utf-8')
                if not prediction:
                    print("❌ Connection closed by server.")
                    break  # Exit loop if server closes connection
                print(f"🎯 Received Prediction: {prediction}")

    except ConnectionRefusedError:
        print("🚨 Error: Could not connect to server. Is it running?")
    except Exception as e:
        print(f"❌ Connection error: {e}")

if __name__ == "__main__":
    receive_predictions()
