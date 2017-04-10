using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Lidar_model : MonoBehaviour
{
    private Parameters_Lidar parameterLidarScript;
    private bool lidarEnable;
    private bool Velodyne_HDL_64E;
    private int numberOfChannel;
    private float[] fieldOfViewVertical;
    private float angularResolutionVertical;
    private float[] fieldOfViewHorizontal;
    private float angularResolutionHorizontal;
    private float measurementRange;
    private float accuracy;
    private string tracePath;
    private string lidarTracePath;
    private bool showRay;
    private bool printDebug;
    private GameObject headGameObject;
    private bool useRayCast = true;
    private string[] lines;
    private int lineIterator = 0;
    private int numberOfRayVertical;
    private int numberOfRayHorizontal;
    private float fieldOfViewTotalHorizontal;
    private bool enableMovementFromKeyboard = false;
    public const string LidarDir = "LIDAR_OUTPUT";
    private bool firstFrame = true;



    // Use this for initialization
    void Start()
    {
        parameterLidarScript = GameObject.Find("Parameters").GetComponent<Parameters_Lidar>();
        lidarEnable = parameterLidarScript.lidarEnable;
        Debug.Log("[Lidar_model][Start][1]");
        if (lidarEnable)
        {
            initialize();
        }
    }

    private void initialize()
    {

        initializeLidarParameters();
        //Initialization of variable
        if (Velodyne_HDL_64E == true)
        {
            loadLidarModel("Velodyne_HDL-64E");
        }
        int i = 0;
        headGameObject = new GameObject();
        while (headGameObject.name != "Head")
        {
            headGameObject = this.transform.GetChild(i).gameObject;
            if (printDebug)
            {
                Debug.Log("[Lidar_model][initialize][1] laserGameObject found at index=" + i);
            }
            i++;
        }
        numberOfRayVertical = numberOfChannel;
        fieldOfViewTotalHorizontal = fieldOfViewHorizontal[1] - fieldOfViewHorizontal[0];
        numberOfRayHorizontal = (int)(fieldOfViewTotalHorizontal / angularResolutionHorizontal);
        lines = new string[numberOfRayVertical * numberOfRayHorizontal];
        if (printDebug)
        {
            printLidarParameters();
        }
    }

    private void initializeLidarParameters()
    {

        Velodyne_HDL_64E = parameterLidarScript.Velodyne_HDL_64E;
        numberOfChannel = parameterLidarScript.numberOfChannel;
        fieldOfViewVertical = parameterLidarScript.fieldOfViewVertical;
        angularResolutionVertical = parameterLidarScript.angularResolutionVertical;
        fieldOfViewHorizontal = parameterLidarScript.fieldOfViewHorizontal;
        angularResolutionHorizontal = parameterLidarScript.angularResolutionHorizontal;
        measurementRange = parameterLidarScript.measurementRange;
        accuracy = parameterLidarScript.accuracy;
        showRay = parameterLidarScript.showRay;
        printDebug = parameterLidarScript.printDebug;
    }

    private void loadLidarModel(string lidarModel)
    {
        if (lidarModel == "Velodyne_HDL-64E")
        {
            numberOfChannel = 64;
            numberOfChannel = 1;
            fieldOfViewVertical = new float[] { 2, (float)-24.9 };
            angularResolutionVertical = (float)0.4;
            fieldOfViewHorizontal = new float[] { 0, 360 };
            angularResolutionHorizontal = (float)0.35;
            measurementRange = 120;
            accuracy = (float)0.02;
        }
    }

    private void printLidarParameters()
    {
        Debug.Log("[Lidar_model][printLidarParameters] numberOfChannel=" + numberOfChannel + " numberOfRayHorizontal=" + numberOfRayHorizontal + " fieldOfViewVertical=[" + fieldOfViewVertical[0] + ";] angularResolutionVertical=" + angularResolutionVertical + " fieldOfViewHorizontal=[" + fieldOfViewHorizontal[0] + ";" + fieldOfViewHorizontal[1] + "] angularResolutionHorizontal=" + angularResolutionHorizontal + " measurementRange=" + measurementRange + " accuracy" + accuracy);
    }

    private float angleHorizontalToAngleVertical(float angle)
    {
        return ((float)90 - (angle));
    }

    private void initializeOnFirstFrame()
    {
        Debug.Log("[Lidar_model][initializeOnFirstFrame][1]");
        tracePath = GameObject.Find("Parameters").GetComponent<Parameters>().getTraceFolder();
        Debug.Log("[Lidar_model][initializeOnFirstFrame][1] tracePath=" + tracePath);
        lidarTracePath = Path.Combine(tracePath, LidarDir);
        Debug.Log("[Lidar_model][initializeOnFirstFrame][1] lidarTracePath=" + lidarTracePath);
        Directory.CreateDirectory(lidarTracePath);
    }

    private void createRayCast()
    {
        if (numberOfRayHorizontal > 0 && numberOfRayVertical > 0)
        {
            Vector3 positionLidar = headGameObject.transform.position;

            int rayCastNumber = 0;

            //As defined in the LIDAR Spec, FOV vertical is angle with horizon, so we change it to angle with vertical

            for (int verticalAngleIt = 0; verticalAngleIt < numberOfRayVertical; verticalAngleIt++)
            {
                float verticalAngle = angleHorizontalToAngleVertical(fieldOfViewVertical[0] - verticalAngleIt * angularResolutionVertical);
                //Debug.Log("[Lidar_model][createRays][2] fieldOfViewHorizontal[0]=" + angleHorizontalToAngleVertical(fieldOfViewHorizontal[0]) + " fieldOfViewHorizontal[1]=" + angleHorizontalToAngleVertical(fieldOfViewHorizontal[1]) + " angularResolutionVertical=" + angularResolutionVertical+ " verticalAngle="+ verticalAngle);
                //numberOfRayHorizontal = 1;
                for (int horizontalAngleIt = 0; horizontalAngleIt < numberOfRayHorizontal; horizontalAngleIt++)
                {
                    float horizontalAngle = fieldOfViewHorizontal[0] + horizontalAngleIt * angularResolutionHorizontal;
                    rayCastNumber++;
                    if (rayCastNumber > lines.Length)
                    {
                        Debug.LogError("[Lidar_model][createRays] frameCount=" + Time.frameCount + " rayCastNumber =" + rayCastNumber + " > lines.Length=" + lines.Length + " verticalAngle=" + verticalAngle + " horizontalAngle=" + horizontalAngle + " fieldOfViewHorizontal[0]=" + angleHorizontalToAngleVertical(fieldOfViewHorizontal[0]) + " fieldOfViewHorizontal[1]=" + angleHorizontalToAngleVertical(fieldOfViewHorizontal[1]) + " angularResolutionVertical=" + angularResolutionVertical);
                    }

                    float distanceX = (float)Math.Cos((float)((horizontalAngle) * Math.PI / 180));
                    float distanceZ = (float)Math.Sin((float)((horizontalAngle) * Math.PI / 180));
                    float distanceY = (float)Math.Cos((float)((verticalAngle) * Math.PI / 180));
                    Vector3 direction = headGameObject.transform.rotation * new Vector3(distanceX, distanceY, distanceZ);
                    if (printDebug)
                    {
                        Debug.Log("[Lidar_model][createRays][3] verticalAngle=" + verticalAngle + " horizontalAngle=" + horizontalAngle + " verticalAngleIt=" + verticalAngleIt + " horizontalAngleIt=" + horizontalAngleIt + " distanceX=" + distanceX + " distanceZ=" + distanceZ + " distanceY=" + distanceY + " direction=" + direction + " laserGameObject.transform.rotation=" + headGameObject.transform.localRotation);
                    }
                    RaycastHit hit;

                    //if (Physics.Raycast(positionLidar, direction, out hit, distance))
                    if (Physics.Raycast(positionLidar, direction, out hit, measurementRange))
                    {
                        if (hit.collider.tag != "Player")
                        {
                            //Debug.Log("[Lidar_model][createRays][1] RayCast collide rayCastNumber="+ rayCastNumber + " distance=" + hit.distance + " collider=" + hit.distance + " at point=" + hit.point + " lineIterator=" + lineIterator);
                            Vector3 point = hit.point - positionLidar;
                            //All the points here are in the referencial of the scene. It should be the referencial of the lidar
                            point = Quaternion.Inverse(headGameObject.transform.rotation) * point;
                            //Rotate again in order to have x in from of the Lidar
                            point = Quaternion.Euler(0, 90, 0) * point;
                            if (showRay)
                            {
                                Debug.DrawLine(positionLidar, hit.point);
                            }
                            int reflectance = 0;
                            //lines[lineIterator] = point.x.ToString() + " " + point.y.ToString() + " " + point.z.ToString() + " " + reflectance;
                            lines[lineIterator] = point.x.ToString() + " " + point.z.ToString() + " " + point.y.ToString() + " " + reflectance;
                            //lines[lineIterator] = point.x.ToString() + " " + point.y.ToString() + " " + point.z.ToString() + " " + reflectance + " "+ hit.collider.name+" "+ hit.collider.tag;
                            //Debug.Log("[Lidar_model][createRays][5] frameCount=" + Time.frameCount + " lineIterator=" + lineIterator + " lines[lineIterator]=" + lines[lineIterator]);
                            lineIterator++;
                        }
                    }
                }
            }

            if (rayCastNumber != numberOfRayVertical * numberOfRayHorizontal)
            {
                int numberExpected = numberOfRayVertical * numberOfRayHorizontal;
                Debug.LogError("[Lidar_model][createRays] frameCount=" + Time.frameCount + " rayCastNumber =" + rayCastNumber + " != numberExpected=" + numberExpected);
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (lidarEnable)
        {
            if (firstFrame)
            {
                initializeOnFirstFrame();
                firstFrame = false;
            }
            if (useRayCast)
            {
                lineIterator = 0;
                createRayCast();
            }
            if (enableMovementFromKeyboard)
            {
                //Function called right after the Frame is finished        
                float moveHorizontal = Input.GetAxis("Horizontal");
                float moveVertical = Input.GetAxis("Vertical");
                Vector3 movement = new Vector3(moveHorizontal * 10, 0.0f, -moveVertical * 10);
                Transform transform = GetComponent<Transform>();
                transform.position += movement;
            }
            writeFile(Time.frameCount);
        }
    }

    void writeFile(int frame)
    {
        string outputFile = lidarTracePath +"\\"+ frame.ToString().PadLeft(10, '0') + ".txt";
        //Debug.Log("[Lidar_model][writeFile] frameCount=" + Time.frameCount + " Writing lines to output file:" + outputFile);
        System.IO.File.WriteAllLines(outputFile, lines);
    }

    public void setVelodyne_HDL_64E(Boolean a)
    {
        Velodyne_HDL_64E = a;
    }
    public void setNumberOfChannel(int a)
    {
        this.numberOfChannel = a;
    }
    public void setFieldOfViewVertical(float[] a)
    {
        fieldOfViewVertical = a;
    }
    public void setAngularResolutionVertical(float a)
    {
        angularResolutionVertical = a;
    }
    public void setFieldOfViewHorizontal(float[] a)
    {
        fieldOfViewHorizontal = a;
    }
    public void setAngularResolutionHorizontal(float a)
    {
        angularResolutionHorizontal = a;
    }
    public void setMeasurementRange(float a)
    {
        measurementRange = a;
    }
    public void setAccuracy(float a)
    {
        accuracy = a;
    }
    public void setOutputPath(string a)
    {
        tracePath = a;
    }
    public void setShowRay(bool a)
    {
        showRay = a;
    }
    public void setPrintDebug(bool a)
    {
        printDebug = a;
    }

}
