using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Parameters : MonoBehaviour {
    public string tracePath;
    public bool enableRecording;
    public GameObject adasCarGameObject;
    public Text distanceToWalkerText;
    public Text warningText;
    public Text debugText;
    private string traceFolder;
    // Use this for initialization
    void Start () {
        traceFolder = tracePath +"\\"+ System.DateTime.Now.ToString("yyyy-MM-ddTHH.mm.ss");        
        Directory.CreateDirectory(traceFolder);
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

}
