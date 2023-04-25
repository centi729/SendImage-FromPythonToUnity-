# SendImage-FromPythonToUnity

## What does this do anyway?

Client Side (python)
1. Capture webcam image
2. encode image to jpeg bytecode
3. compress it with [zlib](https://www.zlib.net/)
4. send it to server using UDP protocol

Server Side (unity3D)
1. receive the compressed bytecode
2. decompress it
3. make it as a texture

## How to use this?

1. You need zlib in unity. I used [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) and installed [Iconic.Zlib.Netstandard](https://github.com/HelloKitty/Iconic.Zlib.Netstandard)
2. Download the unity script and add this script component to a **Quad** Object which will display received image
3. Add a UI button object to call 'PressButton' function
4. excute python script and then play unity

if you just want it to be excuted at start, use this


    public int port = 50000;
    public string serverIP = "127.0.0.1";

    private Texture2D texture;

    UdpClient client;
    IPEndPoint serverEP;

    private void Start()
    {
        texture = new Texture2D(2, 2);

        client = new UdpClient(port);
        serverEP = new IPEndPoint(IPAddress.Parse(serverIP), port);

        StartCoroutine(ReceiveLoop(client, serverEP));
    }

    private IEnumerator ReceiveLoop(UdpClient client, IPEndPoint serverEP)
    {
        byte[] data = client.Receive(ref serverEP);
        
        while(true)
        {
            int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, 0));

            byte[] compressed = new byte[data.Length - 4];
            Array.Copy(data, 4, compressed, 0, compressed.Length);
            byte[] decompressed = ZlibStream.UncompressBuffer(compressed);
            
            texture.LoadImage(decompressed);
            texture.Apply();

            GetComponent<Renderer>().material.mainTexture = texture;

            yield return null;
            
            data = client.Receive(ref serverEP);
        }
    }



## Note

- **IMPORTANT** Unity editor gets stuck when python code is not running

- If you want to send an image captured from Raspberry Pi to unity3D , unity asset so-called [FMETP STREAM](https://assetstore.unity.com/packages/tools/video/fmetp-stream-3-0-221362) might be another option.

- This script was created with the help of Chat-GPT
