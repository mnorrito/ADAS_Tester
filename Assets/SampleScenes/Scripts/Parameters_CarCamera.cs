using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_CarCamera : MonoBehaviour {
    [SerializeField]
    private bool cameraEnable;
    [SerializeField]
    private bool recordingEnable;
    [SerializeField]
    [Range(1, 120)]
    private int m_fps=6 ;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
		
	}
    public bool isCameraEnabled()
    {
        return cameraEnable;
    }

    public bool isRecordingEnabled()
    {
        return recordingEnable;
    }

    public int getFps()
    {
        return m_fps;

    }
}
