using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraRecorder : MonoBehaviour {

    [SerializeField]
    private Camera OnBoardCamera;
    private string m_saveLocation = "";

    private bool enableRecording;
    private string tracePath;
    public const string camOutDir = "CAMERA_OUTPUT";
    private bool firstFrame = true;

    void Start () {
        initialize();
        
    }

    private void initialize()
    {
        enableRecording = GameObject.Find("Parameters").GetComponent<Parameters>().enableRecording;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (firstFrame)
        {
            initializeOnFirstFrame();
            firstFrame = false;
        }
        string timeStamp = System.DateTime.Now.ToString("HH_mm_ss_fff");
        if (enableRecording == true) { 
        WriteImage(OnBoardCamera, "OnBoardCamera", timeStamp);
        }
    }

    private void OpenFolder(string location)
    {
        m_saveLocation = location;
        //print(m_saveLocation);
        Directory.CreateDirectory(m_saveLocation);
        Directory.CreateDirectory(Path.Combine(m_saveLocation, camOutDir));
    }
    private void initializeOnFirstFrame()
    {
        tracePath = GameObject.Find("Parameters").GetComponent<Parameters>().getTraceFolder();
        OpenFolder(tracePath);
        //print("Open Folder" + tracePath);
    }
    private void WriteImage(Camera camera, string prepend, string timestamp)
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
        string path = Path.Combine(directory, prepend + "_" + timestamp + ".jpg");
        File.WriteAllBytes(path, image);
        image = null;
    }
}
