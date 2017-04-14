using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_Car : MonoBehaviour {
    public bool standalone;
    public bool recordingEnable;
    public int fps ;

    public GameObject Walker1;
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public int getFps()
    {
        if (fps > 0)
        {
            return fps;
        }else
        {
            fps = 24;
            return fps;
        }

    }
}
