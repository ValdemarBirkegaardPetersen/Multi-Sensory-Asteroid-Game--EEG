using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


// TODO: The UDP GameObject must be made DontDestroyOnLoad() so that it persists when changing from start screen to game screen
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

    public bool eegActive = false;
    private bool alreadyStarted = false;

    public static udp Instance;

    private string logging;
    private string filePath;

    private int deathCounter; // used for logging the count of the death being logged

    List<float> EEGvalues = new List<float>();

    private Coroutine averageCoroutine;
    List<float> eegValuesList;

    private float startTime;

    public float spawnRateEEG;


    // dont destroy on load
    void Awake()
    {
        // Check if an instance of this object already exists
        if (Instance == null)
        {
            // Set the current object as the instance
            Instance = this;
            // Prevent this object from being destroyed when a new scene is loaded
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy this object if another instance already exists
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        Debug.Log("Unicorn UDP Receiver Example");
        Debug.Log("----------------------------");

        // setup logging
        logging = "time,delta,theta,alpha,betalow,betamid,betahigh,gamma,engagement\n";
        string dateTimeString = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        filePath = "C:/Users/DjVlad/Desktop/EEG_Logging/filename_" + dateTimeString + ".txt";
        deathCounter = 1;

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
        
        eegValuesList = new List<float>();
        averageCoroutine = StartCoroutine(CalculateAverageCoroutine());
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
        if (alreadyStarted == false)
        {
            if (socket.Poll(0, SelectMode.SelectRead))
            {
                Debug.Log("EEG signal found.. Starting client loop");
                StartCoroutine(CommunicationCoroutine());
                startTime = Time.time;
                alreadyStarted = true;
            }
            else
            {
                //Debug.Log("EEG signal not found... ");
            }
        }

        if(alreadyStarted) {

        float deltaAvg = EEGvalues[56];
        float thetaAvg = EEGvalues[57];
        float alphaAvg = EEGvalues[58];
        float betaLowAvg = EEGvalues[59];
        float betaMidAvg = EEGvalues[60];
        float betaHighAvg = EEGvalues[61];
        float gammaAvg = EEGvalues[62];

        // engagement formula (Beta/(Alpha + Theta))
        // see article: http://www.fdg2015.org/papers/fdg2015_paper_54.pdf

        float avgBeta = (betaLowAvg + betaMidAvg + betaHighAvg) / 3;
        float engagement = (avgBeta) / (alphaAvg + thetaAvg);

        float logTime = Time.time - startTime;

        // adding engagement to the EEGvalues list which is averaged every 5 seconds and used to adjust difficulty
        eegValuesList.Add(engagement);

        logging += logTime.ToString() + "," + deltaAvg.ToString() + "," + thetaAvg.ToString() + "," + alphaAvg.ToString() + "," + betaLowAvg.ToString() 
            + "," + betaMidAvg.ToString() + "," + betaHighAvg.ToString() + "," + gammaAvg.ToString() + "," + engagement.ToString() + "\n";

        Debug.Log("Time: " + logTime + ", Delta: " + deltaAvg + ", Theta: " + thetaAvg + ", Alpha: " + alphaAvg + ", Beta Low: " + betaLowAvg 
            + ", Beta Mid: " + betaMidAvg + ", Beta High: " + betaHighAvg + ", Gamma: " + gammaAvg + ", Engagement: " + engagement);

        // clearing signal array
        EEGvalues.Clear();
        }
    }

    // UDP BANDPOWER VERSION //

    private IEnumerator CommunicationCoroutine()
    {
        while (true)
        {
            try { 
                int numberOfBytesReceived = socket.Receive(receiveBufferByte);
                if (numberOfBytesReceived > 0)
                {
                    eegActive = true; // sets eeg state to active
                    byte[] messageByte = new byte[numberOfBytesReceived];
                    Array.Copy(receiveBufferByte, messageByte, numberOfBytesReceived);
                    string message = System.Text.Encoding.ASCII.GetString(messageByte);


                    string[] splitValues = message.Split(',');

                    foreach (string value in splitValues)
                    {
                        float floatValue;
                        if (float.TryParse(value, out floatValue))
                        {
                            EEGvalues.Add(floatValue);
                        }
                        else
                        {
                            // Handle invalid values or parsing errors
                        }
                    }

                } 
                else
                {
                    eegActive = false; // sets eeg state to non-active
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

    private IEnumerator CalculateAverageCoroutine()
    {
        while (true)
        {
            // Wait for 1 second
            yield return new WaitForSeconds(5f);

            // Calculate the average of the EEG values
            float average = 0f;
            if (eegValuesList.Count > 0)
            {
                average = eegValuesList.Sum() / eegValuesList.Count;
            }

            // Do something with the average value (e.g., display or process it)
            Debug.Log("Average EEG value: " + average);
            Debug.Log("Average EEG array length: " + eegValuesList.Count);

            spawnRateEEG = average;

            // Clear the list for the next iteration
            eegValuesList.Clear();
        }
    }

    public void logDeath()
    {
        logging += Time.realtimeSinceStartup.ToString() + "," + "player dies" + deathCounter.ToString() + ",,,,,,,\n";
        deathCounter += 1;
    }

    public void logStart()
    {
        logging += Time.realtimeSinceStartup.ToString() + "," + "player started" + deathCounter.ToString() + ",,,,,,,\n";
    }


    private void OnDestroy()
    {
        // Clean up the UDP client when the object is destroyed
        socket.Close();

        File.WriteAllText(filePath, logging);
    }


}
