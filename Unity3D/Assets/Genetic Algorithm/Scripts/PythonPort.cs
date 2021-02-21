using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;

//Checks player inputs for matching with match objects
public class PythonPort : MonoBehaviour
{
    //Port vairables - Unity <-> Python
    Thread receiveThread;
    UdpClient client;
    int port;

    [SerializeField] string message;

    bool connectedToPython = false;

    // Start is called before the first frame update
    void Start()
    {
        //Setup port for receiving information
        port = 6000;
        InitUDP();
    }

    private void Update()
    {
        //error.text = message;
    }

    //Initialises Thread to run parallel to the game
    private void InitUDP()
    {
        print("UDP Initialized");
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true; //Runs parallel to the game
        receiveThread.Start();
        //message = "Initializing UDP";
    }

    //Recieves information from port on a thread running parallel to the game.
    private void ReceiveData()
    {
        Debug.Log("RecieveDataCalled");
        client = new UdpClient(port); //assign port
        while (true) //set to variable if you don't want data to be recieved.
        {
            try
            {             
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //where inputs are declared
                byte[] data = client.Receive(ref anyIP); //data recieved stored in binary form

                string text = Encoding.UTF8.GetString(data); //data encoded as utf-8 string format
                //print("OpenCV-Python Information: " + text);
                //Set to connected as input is being recieved
                connectedToPython = true;
                message = text;
                Debug.Log("RecieveDataCalled");
            }
            catch (Exception e)
            {
                print(e.ToString()); //log exceptions to console
                connectedToPython = false;
                message = e.ToString();
            }
        }
    }
}
