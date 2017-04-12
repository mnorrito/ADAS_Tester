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
    //public CarRemoteControl CarRemoteControl;
    private Camera CarCamera;
    private GameObject Walker1;
    private GameObject Car;


    private SocketIOComponent _socket;
    private CarController _carController;

    // Use this for initialization
    void Start()
    {
        Car = GameObject.Find("Parameters").GetComponent<Parameters>().adasCarGameObject;
        Walker1 = GameObject.Find("Parameters").GetComponent<Parameters_Car>().Walker1;
        WaypointAndRemoteCarControl = GameObject.Find("Parameters").GetComponent<Parameters>().adasCarGameObject.GetComponent<WaypointAndRemoteCarControl>();
        for (int i = 0; i < Car.transform.childCount; i++)
        {
            GameObject gameObject = Car.transform.GetChild(i).gameObject;
            if (gameObject != null)
            {
                if (gameObject.name == "OnBoardCamera")
                {
                    CarCamera = gameObject.GetComponent<Camera>();
                }
            }
        }
        _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        _socket.On("toUnityMsg", OnToUnityMsg);
        _carController = WaypointAndRemoteCarControl.GetComponent<CarController>();
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
        int msgSize = int.Parse(jsonObject.GetField("messageSize").str);

        float steering = float.Parse(jsonObject.GetField("0").str);
        WaypointAndRemoteCarControl.Acceleration = float.Parse(jsonObject.GetField("1").str);
        WaypointAndRemoteCarControl.Pedestrian = float.Parse(jsonObject.GetField("2").str);

        sendTelemetry();
        sendCameraImg();
    }

    void emptyInfoReceived(SocketIOEvent obj)
    {
        JSONObject jsonObject = obj.data;
        sendTelemetry();
        sendCameraImg();
    }

    void sendTelemetry()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            int msgSize = 4;
            data["messageHeader"] = "telemetry";
            data["messageSize"] = msgSize.ToString();

            data["0"] = _carController.CurrentSteerAngle.ToString("N4");
            data["1"] = _carController.AccelInput.ToString("N4");
            data["2"] = _carController.CurrentSpeed.ToString("N4");
            data["3"] = (Car.transform.position - Walker1.transform.position).magnitude.ToString("N4");

            _socket.Emit("toExtMsg", new JSONObject(data));
        });
    }


    void sendCameraImg()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            int msgSize = 1;
            data["messageHeader"] = "cameraImg";
            data["messageSize"] = msgSize.ToString();

            data["0"] = Convert.ToBase64String(CameraHelper.CaptureFrame(CarCamera));
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