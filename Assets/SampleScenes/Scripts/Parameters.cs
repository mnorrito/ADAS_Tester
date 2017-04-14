﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Parameters : MonoBehaviour {
    public string tracePath;
    public GameObject adasCarGameObject;
    public Text distanceToWalkerText;
    public Text warningText;
    public Text debugText;
    private string traceFolder;
    private int frameRate = -1;
    [SerializeField]
    [Range(0, 3)]
    public int DebugLevel;
    // Use this for initialization
    void Start () {
        traceFolder = tracePath +"\\"+ System.DateTime.Now.ToString("yyyy-MM-ddTHH.mm.ss");        
        Directory.CreateDirectory(traceFolder);
        computeFrameRate();
        Time.fixedDeltaTime = ((float)1) / ((float)frameRate);

        log("[Parameters][Start] Application.frameRate=" + Application.targetFrameRate + " QualitySettings.vSyncCount=" + QualitySettings.vSyncCount + " Time.captureFramerate=" + Time.captureFramerate + " Time.fixedDeltaTime=" + Time.fixedDeltaTime,1);     
    }

    private void computeFrameRate()
    {
        int lidarRotationRateHz = this.GetComponent<Parameters_Lidar>().getRotationRateHz();
        int fpsCamera = this.GetComponent<Parameters_Car>().getFps();
        if (lidarRotationRateHz != 0 && fpsCamera!=0)
        {
            frameRate = PPCM(fpsCamera, lidarRotationRateHz);
        }
        else
        {
            frameRate = fpsCamera;
        }
    }

    public int getFrameRate()
    {
        if(frameRate == -1)
        {
            computeFrameRate();
        }
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
