using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class udp : MonoBehaviour
{
    int port;
    Socket socket;
    byte[] receiveBufferByte;
    float[] receiveBufferFloat;

    private float eegChannel1;
    private float eegChannel2;
    private float eegChannel3;
    private float eegChannel4;
    private float eegChannel5;
    private float eegChannel6;
    private float eegChannel7;
    private float eegChannel8;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Unicorn UDP Receiver Example");
        Debug.Log("----------------------------");


        StartCoroutine(CommunicationCoroutine());

        try
        {
            port = 1000;
            Debug.Log("Destination Port: " + port);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Debug.Log("IP Adress: " + ip);
            IPEndPoint endPoint = new IPEndPoint(ip, port);

            // Initialize udp socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(endPoint);
            receiveBufferByte = new byte[1024];
            receiveBufferFloat = new float[receiveBufferByte.Length / sizeof(float)];
            Debug.Log(receiveBufferByte.Length / sizeof(float));
            


        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
        }
    }

    /*
    Channels 1-8: EEG data
    Channels 9-11: Accelerometer data
    Channels 12-14: Gyroscope data
    Channel 15: Battery Level
    Channel 16: Counter
    Channel 17: Validation
    */

    private void Update()
    {
        eegChannel1 = receiveBufferFloat[0];
        eegChannel2 = receiveBufferFloat[1];
        eegChannel3 = receiveBufferFloat[2];
        eegChannel4 = receiveBufferFloat[3];
        eegChannel5 = receiveBufferFloat[4];
        eegChannel6 = receiveBufferFloat[5];
        eegChannel7 = receiveBufferFloat[6];
        eegChannel8 = receiveBufferFloat[7];

        Debug.Log(eegChannel1);
    }

    private IEnumerator CommunicationCoroutine()
    {
        while (true)
        {
            // Send a message to the UDP port
            try
            {
                int numberOfBytesReceived = socket.Receive(receiveBufferByte);

                if (numberOfBytesReceived > 0)
                {
                    for (int i = 0; i < numberOfBytesReceived / sizeof(float); i++)
                    {
                        receiveBufferFloat[i] = BitConverter.ToSingle(receiveBufferByte, i * sizeof(float));
                        if (i + 1 < numberOfBytesReceived / sizeof(float))
                        {
                            //Debug.Log(receiveBufferFloat[i].ToString("n2"));
                        }
                        else
                        {
                            
                            //Debug.Log(receiveBufferFloat[i].ToString("n2"));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.Log("Error: " + ex.Message);
            }


            // Wait for one frame before sending another message
            yield return null;
        }
    }

    private void OnDestroy()
    {
        // Clean up the UDP client when the object is destroyed
        socket.Close();
    }


}
