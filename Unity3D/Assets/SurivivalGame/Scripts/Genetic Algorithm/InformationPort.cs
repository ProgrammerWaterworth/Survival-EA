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
    public int port = 60000;
    private Socket client;
    [SerializeField]
    private float[] dataOut, dataIn; //debugging

    [SerializeField]
    private float[] myArray; //debugging
    private void Update()
    {
    }

    /// <summary>
    /// Helper function for sending and receiving.
    /// </summary>
    /// <param name="dataOut">Data to send</param>
    /// <returns></returns>
    public  float[] ServerRequest(float[] dataOut)
    {
        //print("request");
        this.dataOut = dataOut; //debugging
        this.dataIn = SendAndReceive(dataOut); //debugging
        return this.dataIn;
    }

    /// <summary> 
    /// Send data to port, receive data from port.
    /// </summary>
    /// <param name="dataOut">Data to send</param>
    /// <returns></returns>
    private float[] SendAndReceive(float[] dataOut)
    {
        //initialize socket
        float[] floatsReceived;
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(ip, port);
        if (!client.Connected)
        {
            Debug.LogError("Connection Failed");
            return null;
        }

        //convert floats to bytes, send to port
        var byteArray = new byte[dataOut.Length * 4];
        //copies float dataOut into byteArray as bytes. 4 bytes in float.
        Buffer.BlockCopy(dataOut, 0, byteArray, 0, byteArray.Length);
        client.Send(byteArray);

        //allocate and receive bytes
        byte[] bytes = new byte[4000];
        int idxUsedBytes = client.Receive(bytes);
        //print(idxUsedBytes + " new bytes received.");
        Debug.Log(idxUsedBytes);
        
        //convert bytes to floats
        floatsReceived = new float[idxUsedBytes / 4];
        Buffer.BlockCopy(bytes, 0, floatsReceived, 0, idxUsedBytes);
        foreach (float f in floatsReceived)
        {
            Debug.Log(f);
        }
        Debug.Log(floatsReceived);
        client.Close();
        return floatsReceived;
    }

}
