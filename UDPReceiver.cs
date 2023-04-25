using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Ionic.Zlib;

public class UDPReceiver : MonoBehaviour
{
    
    public int port = 50000;
    public string serverIP = "127.0.0.1";

    private Texture2D texture;

    UdpClient client;
    IPEndPoint serverEP;

    bool isReceived = false;

    private void Start()
    {
        texture = new Texture2D(2, 2);

        client = new UdpClient(port);
        serverEP = new IPEndPoint(IPAddress.Parse(serverIP), port);

        client.BeginReceive(ReceiveCallback, null);
    }

    public void PressButton()
    {
        if (isReceived)
        {
            StartCoroutine(ReceiveLoop(client, serverEP));
        }
    }

    //The loop will not start until you get first UDP packet to prevent freezing
    void ReceiveCallback(IAsyncResult ar)
    {
        isReceived = true;
    }

    private IEnumerator ReceiveLoop(UdpClient client, IPEndPoint serverEP)
    {
        byte[] data = client.Receive(ref serverEP);
        
        while(true)
        {
            // Read the length of the compressed image from the first 4 bytes
            int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, 0));

            // Decompress the image using zlib
            byte[] compressed = new byte[data.Length - 4];
            Array.Copy(data, 4, compressed, 0, compressed.Length);
            byte[] decompressed = ZlibStream.UncompressBuffer(compressed);
            
            // Convert the decompressed bytes to a texture
            texture.LoadImage(decompressed);
            texture.Apply();

            // Display the texture
            GetComponent<Renderer>().material.mainTexture = texture;

            yield return null;
            
            data = client.Receive(ref serverEP);
        }
    }
}
