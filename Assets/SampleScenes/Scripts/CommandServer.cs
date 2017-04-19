using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System;
using System.Security.AccessControl;

public class CommandServer : MonoBehaviour
{
    private WaypointAndRemoteCarControl WaypointAndRemoteCarControl;


    private SocketIOComponent _socket;
    private int token;

    // Use this for initialization
    void Start()
    {
        WaypointAndRemoteCarControl = GameObject.Find("Parameters").GetComponent<Parameters>().adasCarGameObject.GetComponent<WaypointAndRemoteCarControl>();
        _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        _socket.On("toUnityMsg", OnToUnityMsg);
        token = 1;
    }
    // Update is called once per frame
    void Update()

    {
    }

    void OnToUnityMsg(SocketIOEvent obj)
    {
        JSONObject jsonObject = obj.data;
        string messageHeader = jsonObject.GetField("messageHeader").str;

        if (messageHeader.Equals("driveInfo"))
        {
            driveInfoReceived(obj);
        }
        if (messageHeader.Equals("emptyInfo"))
        {
            emptyInfoReceived(obj);
        }
    }


    void driveInfoReceived(SocketIOEvent obj)
    {
        JSONObject jsonObject = obj.data;
        //int msgSize = int.Parse(jsonObject.GetField("messageSize").str);
        //float steering = float.Parse(jsonObject.GetField("0").str);

        WaypointAndRemoteCarControl.Acceleration = float.Parse(jsonObject.GetField("1").str);
        WaypointAndRemoteCarControl.Pedestrian = float.Parse(jsonObject.GetField("2").str);
        token++;
    }

    void emptyInfoReceived(SocketIOEvent obj)
    {
        JSONObject jsonObject = obj.data;
    }

    public void sendTelemetry(int msgSize, float[] paramArray)
    {
        if (token > 0)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Dictionary<string, string> data = new Dictionary<string, string>();

                data["messageHeader"] = "telemetry";
                data["messageSize"] = msgSize.ToString();

                for (int i = 0; i < msgSize; i++)
                {
                    data[i.ToString()] = paramArray[i].ToString("N4");
                }

                _socket.Emit("toExtMsg", new JSONObject(data));
            });
            token--;
        }
    }


    public void sendCameraImg(byte[] image)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            int msgSize = 1;
            data["messageHeader"] = "cameraImg";
            data["messageSize"] = msgSize.ToString();

            data["0"] = Convert.ToBase64String(image);
            _socket.Emit("toExtMsg", new JSONObject(data));
        });

    }

    public void sendLidarInfo(int msgSize, float [] matrix)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["messageHeader"] = "lidarInfo";
            data["messageSize"] = msgSize.ToString();


            for(int i=0; i< msgSize; i++)
            {
                data[i.ToString()] = matrix[i].ToString("N4");
            }
            _socket.Emit("toExtMsg", new JSONObject(data));
        });

    }


}