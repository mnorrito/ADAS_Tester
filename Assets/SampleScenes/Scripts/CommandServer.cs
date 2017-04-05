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
    public WaypointAndRemoteCarControl WaypointAndRemoteCarControl;
    //public CarRemoteControl CarRemoteControl;
	public Camera CarCamera;
    public GameObject Walker1;
    public GameObject Car;


    private SocketIOComponent _socket;
	private CarController _carController;

	// Use this for initialization
	void Start()
	{
		_socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
		_socket.On("open", OnOpen);
		_socket.On("steer", OnSteer);
		_socket.On("manual", onManual);
        _carController = WaypointAndRemoteCarControl.GetComponent<CarController>();
    }

	// Update is called once per frame
	void Update()
	{
	}

	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
		EmitTelemetry(obj);
	}

	// 
	void onManual(SocketIOEvent obj)
	{
		EmitTelemetry (obj);
	}

	void OnSteer(SocketIOEvent obj)
	{
        JSONObject jsonObject = obj.data;
        //print(float.Parse(jsonObject.GetField("steering_angle").str));
        //WaypointAndRemoteCarControl.SteeringAngle = float.Parse(jsonObject.GetField("steering_angle").str);
        WaypointAndRemoteCarControl.Acceleration = float.Parse(jsonObject.GetField("throttle").str);
        WaypointAndRemoteCarControl.Pedestrian = float.Parse(jsonObject.GetField("pedestrian").str);


        EmitTelemetry(obj);
	}

	void EmitTelemetry(SocketIOEvent obj)
	{
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			print("Attempting to Send...");
			// send only if it's not being manually driven
			if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
				_socket.Emit("telemetry", new JSONObject());
			}
			else {
				// Collect Data from the Car
				Dictionary<string, string> data = new Dictionary<string, string>();
				data["steering_angle"] = _carController.CurrentSteerAngle.ToString("N4");
				data["throttle"] = _carController.AccelInput.ToString("N4");
				data["speed"] = _carController.CurrentSpeed.ToString("N4");
				data["image"] = Convert.ToBase64String(CameraHelper.CaptureFrame(CarCamera));
                data["distanceToWalker"] = (Car.transform.position - Walker1.transform.position).magnitude.ToString("N4");

                _socket.Emit("telemetry", new JSONObject(data));
			}
		});
	}
}