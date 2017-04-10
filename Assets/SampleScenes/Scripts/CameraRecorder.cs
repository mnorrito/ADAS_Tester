using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraRecorder : MonoBehaviour {

    [SerializeField]
    private Camera OnBoardCamera;
    private string m_saveLocation = "";

    public bool enableRecording;
    public string outputFolder;
    public const string scenarioDir = "ADAS_Tester_Scenario";
    public const string camOutDir = "CAMERA_OUTPUT";

    void Start () {
        OpenFolder(outputFolder);
        print("Open Folder" + outputFolder);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        string timeStamp = System.DateTime.Now.ToString("HH_mm_ss_fff");
        if (enableRecording == true) { 
        WriteImage(OnBoardCamera, "OnBoardCamera", timeStamp);
        }
    }

    private void OpenFolder(string location)
    {
        m_saveLocation = location;
        string timeStamp = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
        m_saveLocation = Path.Combine(m_saveLocation, scenarioDir + "_" + timeStamp);
        print(m_saveLocation);
        Directory.CreateDirectory(m_saveLocation);
        Directory.CreateDirectory(Path.Combine(m_saveLocation, camOutDir));
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
