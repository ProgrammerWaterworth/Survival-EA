using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// The Information Port for sending and receiving.
/// </summary>
public class InformationPort : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public int port = 6000;
    Thread receiveThread;
    private Socket client;
    [SerializeField]
    private float[] dataOut; //debugging

    [SerializeField]
    private float[] myArray; //debugging
    private void Update()
    {
    }

    private void Start()
    {
        InitUDP();
    }
    //Initialises Thread to run parallel to the game
    private void InitUDP()
    {
        print("UDP Initialized");
        receiveThread = new Thread(new ThreadStart(SendAndReceive));
        receiveThread.IsBackground = true; //Runs parallel to the game
        receiveThread.Start();
    }

    public void SetDataOut(float _out)
    {
        dataOut[0] = _out;
        Debug.Log("data out: " + dataOut[0]);
    }

    public float[] GetArray()
    {
        return myArray;
    }

    /// <summary> 
    /// Send data to port, receive data from port.
    /// </summary>
    /// <param name="dataOut">Data to send</param>
    /// <returns></returns>
    private void SendAndReceive()
    {
       
       
        while (true)
        {
            Debug.Log("SendAndReceive");
            //initialize socket
            float[] floatsReceived;
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(ip, port);
            if (!client.Connected)
            {
                Debug.LogError("Connection Failed");
            }
            else
            {
                try
                {
                    //convert floats to bytes, send to port
                    var byteArray = new byte[dataOut.Length * 4];

                    //copies float dataOut into byteArray as bytes. 4 bytes in float.

                    Debug.Log("data out: " + dataOut[0]);
                    Buffer.BlockCopy(dataOut, 0, byteArray, 0, byteArray.Length);
                    client.Send(byteArray);
                    Debug.Log("Sending");
                    dataOut[0] = 0;
                    Debug.Log("Data now: " + dataOut[0]);

                    //allocate and receive bytes
                    byte[] bytes = new byte[4000];
                    int idxUsedBytes = client.Receive(bytes);

                    if (idxUsedBytes > 0)
                    {
                        //print(idxUsedBytes + " new bytes received.");
                        Debug.Log("used bytes:" + idxUsedBytes);

                        //convert bytes to floats
                        floatsReceived = new float[idxUsedBytes / 4];
                        Debug.Log("Receiving");
                        Buffer.BlockCopy(bytes, 0, floatsReceived, 0, idxUsedBytes);
                        foreach (float f in floatsReceived)
                        {
                            Debug.Log(f);
                        }
                        //Debug.Log(floatsReceived);

                        myArray = floatsReceived;
                    }
                    client.Close();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
                
        }
        
    }

}
