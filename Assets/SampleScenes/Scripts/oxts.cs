using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class oxts : MonoBehaviour {
    public string path = "C:/kitti_Database/simuTest/";
    private string fileName = "";
    private string fullFileName;
    private TextWriter writer;

    // Use this for initialization
    void Start () {
        fileName = "oxts_" + name + ".txt";
        fullFileName = path + fileName;
        writer = new StreamWriter(fullFileName);
        writer.Close();
    }
	
	// Update is called once per frame
	void Update () {
        
        string tmp = Time.frameCount.ToString();
        tmp = tmp + " " + transform.position.x;
        tmp = tmp + " " + transform.position.z;
        tmp = tmp + " " + transform.position.y;
        tmp = tmp + " " + transform.rotation.x;
        tmp = tmp + " " + transform.rotation.z;
        tmp = tmp + " " + transform.rotation.y;
        Debug.Log(tmp);
        writer = File.AppendText(fullFileName);
        writer.WriteLine(tmp);
        writer.Close();
    }
}
