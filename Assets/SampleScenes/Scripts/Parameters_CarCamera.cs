using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_CarCamera : MonoBehaviour {
    [SerializeField]
    private bool onBoardCamEnable;
    [SerializeField]
    private bool onBoardCamRecEnable;
    [SerializeField]
    [Range(1, 120)]
    private int m_onBoardCamFps = 6;

    [SerializeField]
    private bool sideCamEnable;
    [SerializeField]
    private bool sideCamRecEnable;
    [SerializeField]
    [Range(1, 120)]
    private int m_sideCamFps = 6;


    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
		
	}
    public bool isOnBoardCameraEnabled()
    {
        return onBoardCamEnable;
    }

    public bool isOnBoardCameraRecordingEnabled()
    {
        return onBoardCamRecEnable;
    }

    public int getOnBoardCameraFps()
    {
        return m_onBoardCamFps;

    }

    public bool isSideCameraEnabled()
    {
        return sideCamEnable;
    }

    public bool isSideCameraRecordingEnabled()
    {
        return sideCamRecEnable;
    }

    public int getSideCameraFps()
    {
        return m_sideCamFps;

    }
}
