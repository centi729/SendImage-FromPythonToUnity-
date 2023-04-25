import cv2
import socket
import struct
import zlib

# Initialize the UDP socket
udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
ip_address = '127.0.0.1'
port = 50000

# Capture image 'frameRate/1000' per sec
frameRate = 1000

# Set up the webcam capture
cap = cv2.VideoCapture(0)

# Adjust frame size
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 300)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 300)

# Set the compression parameters
encode_param = [int(cv2.IMWRITE_JPEG_QUALITY), 90]

while True:
    # Capture an image from the webcam
    ret, frame = cap.read()

    # Compress the image
    _, compressed = cv2.imencode('.jpg', frame, encode_param)

    # Compress the image further with zlib
    compressed = zlib.compress(compressed, 9)

    # Convert the compressed image to bytes
    image_bytes = struct.pack('!I', len(compressed)) + compressed

    # Send the image bytes over UDP
    udp_socket.sendto(image_bytes, (ip_address, port))

    key = cv2.waitKey(frameRate)

    # Wait for a keypress to exit the program
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release the resources
cap.release()
cv2.destroyAllWindows()