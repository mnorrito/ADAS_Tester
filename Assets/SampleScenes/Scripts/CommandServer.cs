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
    private Parameters parameterScript;

    private SocketIOComponent _socket;
    private int token;

    public enum MsgHeaderType
    {
        telemetry = 0,
        lidarInfo = 1,
        cameraImg = 2,
        driveInfo = 3,
        emptyInfo = 4
    }

    // Use this for initialization
    void Start()
    {
        parameterScript = GameObject.Find("Parameters").GetComponent<Parameters>();
        WaypointAndRemoteCarControl = GameObject.Find("Parameters").GetComponent<Parameters>().adasCarGameObject.GetComponent<WaypointAndRemoteCarControl>();
        _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        _socket.On("toUnityMsg", OnToUnityMsg);
        token = 2;
    }
    // Update is called once per frame
    void Update()

    {
    }

    void OnToUnityMsg(SocketIOEvent obj)
    {
        JSONObject jsonObject = obj.data;
        int messageHeader = int.Parse(jsonObject.GetField("messageHeader").str);

        if (messageHeader.Equals((int)MsgHeaderType.driveInfo))
        {
            driveInfoReceived(obj);
        }
        if (messageHeader.Equals((int)MsgHeaderType.emptyInfo))
        {
            emptyInfoReceived(obj);
        }
    }


    void driveInfoReceived(SocketIOEvent obj)
    {

        //print(">>> driveInfoReceived");
        JSONObject jsonObject = obj.data;
        //int msgSize = int.Parse(jsonObject.GetField("messageSize").str);
        //float steering = float.Parse(jsonObject.GetField("0").str);

        WaypointAndRemoteCarControl.Acceleration = float.Parse(jsonObject.GetField("1").str);
        WaypointAndRemoteCarControl.Pedestrian = float.Parse(jsonObject.GetField("2").str);
        token++;
    }

    void emptyInfoReceived(SocketIOEvent obj)
    {
        //print(">>> emptyInfoReceived");
        JSONObject jsonObject = obj.data;
        token++;
    }

    public void sendMsg(MsgHeaderType msgHeaderType, int msgSize, float[] dataArray)
    {
        if (token > 0)
        {
            //print("<<< Sending msgHeaderType=" + msgHeaderType.ToString());

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Dictionary<string, string> data = new Dictionary<string, string>();

                data["messageHeader"] = ((int)msgHeaderType).ToString();
                data["messageSize"] = msgSize.ToString();

                for (int i = 0; i < msgSize; i++)
                {
                    data[i.ToString()] = dataArray[i].ToString("N4");
                }

                _socket.Emit("toExtMsg", new JSONObject(data));
            });
            token--;
        }
    }

    public void sendCameraImg(string name, byte[] image)
    {
        MsgHeaderType msgHeaderType = MsgHeaderType.cameraImg;
        int msgSize = 2;
        //print("<<< Sending msgHeaderType=" + msgHeaderType.ToString());

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["messageHeader"] = ((int)msgHeaderType).ToString();
            data["messageSize"] = msgSize.ToString();

            data["0"] = name;
            data["1"] = Convert.ToBase64String(image);
            _socket.Emit("toExtMsg", new JSONObject(data));
        });
    }

}