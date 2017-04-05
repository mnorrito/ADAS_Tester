using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarCameraRecoder : MonoBehaviour {

    public const string CSVFileName = "driving_log.csv";
    public const string DirFrames = "IMG";

    public float AccelInput { get; set; }
    public float BrakeInput { get; private set; }
    public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }

    private Queue<CarSample> carSamples;
    private int TotalSamples;
    private bool isSaving;
    private string m_saveLocation = "C:\\Users\\mnorrito\\MyFolder\\Juliette\\ADAS\\Unity\\Standard Assets Example Project\\Output";
    private Vector3 saved_position;
    private Quaternion saved_rotation;
    private Rigidbody m_Rigidbody;
    private float m_SteerAngle;
    private float m_MaximumSteerAngle;


    [SerializeField]  private Camera CenterCamera;

    private bool m_isRecording;


    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_isRecording = true;
    }



    public bool IsRecording
    {
        get
        {
            return m_isRecording;
        }

        set
        {
            m_isRecording = value;
            if (value == true)
            {
                Debug.Log("Starting to record");
                carSamples = new Queue<CarSample>();
                StartCoroutine(Sample());
            }
            else
            {
                Debug.Log("Stopping record");
                StopCoroutine(Sample());
                Debug.Log("Writing to disk");
                //save the cars coordinate parameters so we can reset it to this properly after capturing data
                saved_position = transform.position;
                saved_rotation = transform.rotation;
                //see how many samples we captured use this to show save percentage in UISystem script
                TotalSamples = carSamples.Count;
                isSaving = true;
                StartCoroutine(WriteSamplesToDisk());

            };
        }

    }


    public IEnumerator WriteSamplesToDisk()
    {
        yield return new WaitForSeconds(0.000f); //retrieve as fast as we can but still allow communication of main thread to screen and UISystem
        if (carSamples.Count > 0)
        {
            //pull off a sample from the que
            CarSample sample = carSamples.Dequeue();

            //pysically moving the car to get the right camera position
            transform.position = sample.position;
            transform.rotation = sample.rotation;

            // Capture and Persist Image
            string centerPath = WriteImage(CenterCamera, "center", sample.timeStamp);
            //string leftPath = WriteImage(LeftCamera, "left", sample.timeStamp);
            //string rightPath = WriteImage(RightCamera, "right", sample.timeStamp);

            string row = string.Format("{0},{1},{2},{3},{4}\n", centerPath, sample.steeringAngle, sample.throttle, sample.brake, sample.speed);
            File.AppendAllText(Path.Combine(m_saveLocation, CSVFileName), row);
        }
        if (carSamples.Count > 0)
        {
            //request if there are more samples to pull
            StartCoroutine(WriteSamplesToDisk());
        }
        else
        {
            //all samples have been pulled
            StopCoroutine(WriteSamplesToDisk());
            isSaving = false;

            //need to reset the car back to its position before ending recording, otherwise sometimes the car ended up in strange areas
            transform.position = saved_position;
            transform.rotation = saved_rotation;
            m_Rigidbody.velocity = new Vector3(0f, -10f, 0f);
            //Move(0f, 0f, 0f, 0f);

        }
    }

    public IEnumerator Sample()
    {
        // Start the Coroutine to Capture Data Every Second.
        // Persist that Information to a CSV and Perist the Camera Frame
        yield return new WaitForSeconds(0.0666666666666667f);

        if (m_saveLocation != "")
        {
            CarSample sample = new CarSample();

            sample.timeStamp = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            sample.steeringAngle = m_SteerAngle / m_MaximumSteerAngle;
            sample.throttle = AccelInput;
            sample.brake = BrakeInput;
            sample.speed = CurrentSpeed;
            sample.position = transform.position;
            sample.rotation = transform.rotation;

            carSamples.Enqueue(sample);

            sample = null;
            //may or may not be needed
        }

        // Only reschedule if the button hasn't toggled
        if (IsRecording)
        {
            StartCoroutine(Sample());
        }

    }


    private void OpenFolder(string location)
    {
        //m_saveLocation = location;
        Directory.CreateDirectory(Path.Combine(m_saveLocation, DirFrames));
    }

    private string WriteImage(Camera camera, string prepend, string timestamp)
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
        string directory = Path.Combine(m_saveLocation, DirFrames);
        string path = Path.Combine(directory, prepend + "_" + timestamp + ".jpg");
        File.WriteAllBytes(path, image);
        image = null;
        return path;
    }

    internal class CarSample
    {
        public Quaternion rotation;
        public Vector3 position;
        public float steeringAngle;
        public float throttle;
        public float brake;
        public float speed;
        public string timeStamp;
    }



}
