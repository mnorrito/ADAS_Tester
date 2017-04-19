﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraRecorder : MonoBehaviour {

    [SerializeField]
    private Camera OnBoardCamera;
    private string m_saveLocation = "";
    private Parameters parameterScript;
    private bool recordingEnable;
    private bool cameraEnable;
    private string tracePath;
    public const string camOutDir = "CAMERA_OUTPUT";
    private int fixedUpdateCounter = -1;
    private int frameRate;
    private int framePerSecond;
    private CommandServer commandServerScript;

    void Start () {
        parameterScript = GameObject.Find("Parameters").GetComponent<Parameters>();
        initialize();
        commandServerScript = GameObject.Find("IOScriptingTools").GetComponentInChildren<CommandServer>();
    }

    private void initialize()
    {
        recordingEnable = GameObject.Find("Parameters").GetComponent<Parameters_CarCamera>().isRecordingEnabled();
        cameraEnable = GameObject.Find("Parameters").GetComponent<Parameters_CarCamera>().isCameraEnabled();
        framePerSecond = GameObject.Find("Parameters").GetComponent<Parameters_CarCamera>().getFps();
        frameRate = parameterScript.getFrameRate();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        fixedUpdateCounter++;
        if (fixedUpdateCounter==0)
        {
            initializeOnFirstFrame();
        }
        if (cameraEnable == true)
        {
            bool cameraFrame = isFrameCameraFrame(fixedUpdateCounter);
            parameterScript.log("[CameraRecorder][FixedUpdate] fixedUpdateCounter="+ fixedUpdateCounter + " frame=" + Time.frameCount + " isFrameCameraFrame="+ cameraFrame + " fixedTime=" + Time.fixedTime + " fixedDeltaTime=" + Time.fixedDeltaTime + " time=" + Time.time, 2);

            if (cameraFrame == true)
            {
                byte[] image = CameraHelper.CaptureFrame(OnBoardCamera);
                commandServerScript.sendCameraImg(image);
            }
            if (recordingEnable == true && cameraFrame)
            {
                int s = (int)(Time.time);
                int ms = (int)((Time.time - (float)s) * ((float)1000));
                string name = s.ToString().PadLeft(4, '0') + "s_" + ms.ToString().PadLeft(4, '0') + "ms";
                WriteImage(OnBoardCamera, name);
            }
        }
    }

    private void OpenFolder(string location)
    {
        m_saveLocation = location;
        Directory.CreateDirectory(m_saveLocation);
        Directory.CreateDirectory(Path.Combine(m_saveLocation, camOutDir));
    }
    private void initializeOnFirstFrame()
    {
        tracePath = GameObject.Find("Parameters").GetComponent<Parameters>().getTraceFolder();
        OpenFolder(tracePath);
        parameterScript.log("[CameraRecorder][initializeOnFirstFrame] Open Folder=" + tracePath, 2);
    }
    private void WriteImage(Camera camera,  string timestamp)
    {
        //needed to force camera update 
        camera.Render();
        RenderTexture targetTexture = camera.targetTexture;
        RenderTexture.active = targetTexture;
        Texture2D texture2D = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        texture2D.Apply();
        byte[] image = texture2D.EncodeToJPG();
        UnityEngine.Object.DestroyImmediate(texture2D);
        string directory = Path.Combine(m_saveLocation, camOutDir);
        string path = Path.Combine(directory,   timestamp + ".jpg");
        File.WriteAllBytes(path, image);
        image = null;
    }

    private bool isFrameCameraFrame(int frame)
    {
        bool isFrameCameraFrame = false;
        
        if (frame % (frameRate / framePerSecond) == 0)
        {
            isFrameCameraFrame = true;
        }
        parameterScript.log("[CameraRecorder][isFrameCameraFrame] frame=" + frame + " isFrameCameraFrame=" + isFrameCameraFrame + " framePerSecond=" + framePerSecond, 3);
        return isFrameCameraFrame;
    }
}
