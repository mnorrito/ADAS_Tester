using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Parameters : MonoBehaviour {
    public string tracePath;
    public GameObject Walker1;
    public GameObject adasCarGameObject;
    public Text distanceToWalkerText;
    public Text warningText;
    public Text debugText;
    private string traceFolder;
    private int frameRate;
    [SerializeField]
    [Range(0, 3)]
    public int DebugLevel;
    private static int minimumFrameRate = 60;
    // Use this for initialization
    void Start () {
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-ddTHH.mm.ss");
        traceFolder = Path.Combine(tracePath, timeStamp);
        Directory.CreateDirectory(traceFolder);
        computeFrameRate();     
        Time.fixedDeltaTime = ((float)1) / ((float)frameRate);

        log("[Parameters][Start] Application.frameRate=" + Application.targetFrameRate + "frameRate=" + frameRate + " QualitySettings.vSyncCount=" + QualitySettings.vSyncCount + " Time.captureFramerate=" + Time.captureFramerate + " Time.fixedDeltaTime=" + Time.fixedDeltaTime,1);     
    }

    private void computeFrameRate()
    {
        int lidarRotationRateHz = 1;
        int onBoardCameraFps = 1;
        int sideCameraFps = 1;
        if (this.GetComponent<Parameters_Lidar>().isLidarEnabled())
        { 
            lidarRotationRateHz = this.GetComponent<Parameters_Lidar>().getRotationRateHz();
        }
        if(this.GetComponent<Parameters_CarCamera>().isOnBoardCameraEnabled())
        {
            onBoardCameraFps = this.GetComponent<Parameters_CarCamera>().getOnBoardCameraFps();
        }
        if (this.GetComponent<Parameters_CarCamera>().isSideCameraEnabled())
        {
            sideCameraFps = this.GetComponent<Parameters_CarCamera>().getSideCameraFps();
        }

        int cameraFramRate = PPCM(onBoardCameraFps, sideCameraFps);
        frameRate = PPCM(cameraFramRate, lidarRotationRateHz);
        while (frameRate < minimumFrameRate)
        {
            frameRate = 2 * frameRate;
        }

    }

    public int getFrameRate()
    {
        return frameRate;
    }

	// Update is called once per frame
	void Update () {
		
	}
    public string getTraceFolder()
    {
        return traceFolder;
    }
    public GameObject getAdasCarGameObject()
    {
        return adasCarGameObject;
    }
    public static int PPCM(int a, int b)
    {
        return a * b / (PGCD(a, b));
    }
    public static int PGCD(int a, int b)
    {
        int temp = a % b;
        if (temp == 0)
            return b;
        return PGCD(b, temp);
    }

    public void log(string logs, int level)
    {
        if (DebugLevel >= level)
        {
            Debug.Log(logs);
        }
        if(level == -1)
        {
            //Then print as error
            Debug.LogError(logs);
        }
    }

}
