using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OnBoardCameraRecorder : MonoBehaviour
{

    private Parameters parameterScript;

    private Camera myCamera;
    private bool myCameraEnable;
    private bool myCameraRecordingEnable;
    private int myCameraFps;
    private string myCameraName;

    private string tracePath;
    public const string camOutDir = "CAMERA_OUTPUT";
    private int fixedUpdateCounter = -1;
    private int frameRate;

    private CommandServer commandServerScript;

    void Start()
    {
        parameterScript = GameObject.Find("Parameters").GetComponent<Parameters>();
        commandServerScript = GameObject.Find("IOScriptingTools").GetComponentInChildren<CommandServer>();
        myCamera = this.GetComponent<Camera>();
        myCameraEnable = GameObject.Find("Parameters").GetComponent<Parameters_CarCamera>().isOnBoardCameraEnabled();
        myCameraRecordingEnable = GameObject.Find("Parameters").GetComponent<Parameters_CarCamera>().isOnBoardCameraRecordingEnabled();
        myCameraName = this.name;
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        fixedUpdateCounter++;
        if (fixedUpdateCounter == 0)
        {
            initializeOnFirstFrame();
        }
        if (myCameraEnable == true)
        {
            bool isCameraFrame = isFrameCameraFrame(fixedUpdateCounter);
            if (isCameraFrame == true)
            {

                //int s = (int)(Time.time);
                //int ms = (int)((Time.time - (float)s) * ((float)1000));
                //string name = myCameraName+"_" + s.ToString().PadLeft(4, '0') + "s_" + ms.ToString().PadLeft(4, '0') + "ms";

                int time_ms = (int)(Time.time * 1000);
                string name = myCameraName + "_" + time_ms.ToString().PadLeft(10, '0') ;
                byte[] image = CameraHelper.CaptureFrame(myCamera);
                commandServerScript.sendCameraImg(name, image);

                if (myCameraRecordingEnable)
                {
                    WriteImage(myCamera, name);
                }
            }
        }
    }

    private void OpenFolder(string location)
    {
        Directory.CreateDirectory(location);
        Directory.CreateDirectory(Path.Combine(location, camOutDir));
    }
    private void initializeOnFirstFrame()
    {
        tracePath = GameObject.Find("Parameters").GetComponent<Parameters>().getTraceFolder();
        OpenFolder(tracePath);
        parameterScript.log("[CameraRecorder][initializeOnFirstFrame] Open Folder=" + tracePath, 2);

        myCameraFps = GameObject.Find("Parameters").GetComponent<Parameters_CarCamera>().getOnBoardCameraFps();
        frameRate = parameterScript.getFrameRate();
        parameterScript.log("[CameraRecorder] myCamera=" + myCamera + " myCameraFps=" + myCameraFps + " frameRate=" + frameRate, 1);
    }
    private void WriteImage(Camera camera, string fileName)
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
        string directory = Path.Combine(tracePath, camOutDir);
        string path = Path.Combine(directory, fileName + ".jpg");
        File.WriteAllBytes(path, image);
        image = null;
    }

    private bool isFrameCameraFrame(int frame)
    {
        bool isFrameCameraFrame = false;
        if (frame % (frameRate / myCameraFps) == 0)
        {
            isFrameCameraFrame = true;
        }
        return isFrameCameraFrame;
    }
}
