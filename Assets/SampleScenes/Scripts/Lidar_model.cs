using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Lidar_model : MonoBehaviour
{
    private Parameters_Lidar parameterLidarScript;
    private Parameters parameterScript;
    private CommandServer commandServerScript;
    private bool lidarEnable;
    private bool Velodyne_HDL_64E;
    private bool LightVelodyne_HDL_64E;
    public bool Velodyne_HDL_64EDualMode;
    private int rotationRateHz;
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
    private GameObject headGameObject;
    private bool useRayCast = true;
    private string[] lines;
    private float[] floatLines;
    private int lineIterator = 0;
    private int numberOfRayVertical;
    private int numberOfRayHorizontal;
    private float fieldOfViewTotalHorizontal;
    //private int lastRecordedFrame;
    public const string LidarDir = "LIDAR_OUTPUT";
    private int fixedUpdateCounter = -1;
    private bool recordingEnable;
    private int frameRate;
    private static int numberOfPointsPerSec_SingleMode = 1300000;
    private static int numberOfPointsPerSec_DualMode = 2200000;

    // Use this for initialization
    void Start()
    {
        parameterScript = GameObject.Find("Parameters").GetComponent<Parameters>();
        parameterLidarScript = GameObject.Find("Parameters").GetComponent<Parameters_Lidar>();
        recordingEnable = parameterLidarScript.isRecordingEnabled();
        //lastRecordedFrame = -1;

        lidarEnable = parameterLidarScript.isLidarEnabled();
        if (lidarEnable)
        {
            initialize();
        }
        commandServerScript = GameObject.Find("IOScriptingTools").GetComponentInChildren<CommandServer>();
    }

    private void initialize()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject gameObject = this.transform.GetChild(i).gameObject;
            if (gameObject != null)
            {
                if (gameObject.name == "Head")
                {
                    headGameObject = gameObject;
                    parameterScript.log("[Lidar_model][initialize][1] laserGameObject found at index=" + i, 2);
                    break;
                }
            }
        }
    }

    private void initializeLidarParameters()
    {
        Velodyne_HDL_64E = parameterLidarScript.Velodyne_HDL_64E;
        LightVelodyne_HDL_64E = parameterLidarScript.LightVelodyne_HDL_64E;
        if (Velodyne_HDL_64E || LightVelodyne_HDL_64E)
        {
            rotationRateHz = parameterLidarScript.rotationRateHz;
            numberOfChannel = 64;
            if (LightVelodyne_HDL_64E) { 
                numberOfChannel = parameterLidarScript.numberOfChannel;
            }
            fieldOfViewVertical = new float[] { 2, (float)-24.9 }; ;
            angularResolutionVertical = (float)0.4;
            if (LightVelodyne_HDL_64E)
            {
                angularResolutionVertical = parameterLidarScript.angularResolutionVertical;
            }
            fieldOfViewHorizontal = new float[] { 0, 360 };
            Velodyne_HDL_64EDualMode = parameterLidarScript.Velodyne_HDL_64EDualMode;
            fieldOfViewTotalHorizontal = fieldOfViewHorizontal[1] - fieldOfViewHorizontal[0];
            //Compute angularResolutionHorizontal
            angularResolutionHorizontal = computeAngularResolutionHorizontal_Velodyne_HDL_64E(Velodyne_HDL_64EDualMode, rotationRateHz, numberOfChannel, fieldOfViewTotalHorizontal);
            if (LightVelodyne_HDL_64E)
            {
                angularResolutionHorizontal = parameterLidarScript.angularResolutionHorizontal;
            }
            //angularResolutionHorizontal = 1;
            measurementRange = 120;
            accuracy = (float)0.02;
            showRay = parameterLidarScript.showRay;
            frameRate = parameterScript.getFrameRate();
        }
        else
        {
            rotationRateHz = parameterLidarScript.rotationRateHz;
            numberOfChannel = parameterLidarScript.numberOfChannel;
            fieldOfViewVertical = parameterLidarScript.fieldOfViewVertical;
            angularResolutionVertical = parameterLidarScript.angularResolutionVertical;
            fieldOfViewHorizontal = parameterLidarScript.fieldOfViewHorizontal;
            angularResolutionHorizontal = parameterLidarScript.angularResolutionHorizontal;
            measurementRange = parameterLidarScript.measurementRange;
            accuracy = parameterLidarScript.accuracy;
            showRay = parameterLidarScript.showRay;
            frameRate = parameterScript.getFrameRate();
            fieldOfViewTotalHorizontal = fieldOfViewHorizontal[1] - fieldOfViewHorizontal[0];
        }
        numberOfRayVertical = numberOfChannel;        
        numberOfRayHorizontal = (int)(fieldOfViewTotalHorizontal / angularResolutionHorizontal);
        lines = new string[numberOfRayVertical * numberOfRayHorizontal];
        floatLines = new float[numberOfRayVertical * numberOfRayHorizontal * 4];        
        printLidarParameters();        
    }

    private float computeAngularResolutionHorizontal_Velodyne_HDL_64E(bool dualReturnModeEnabled,  int rotationRate, int verticalRayNumber, float fieldOfView)
    {
        int numberOfPointsPerSec = dualReturnModeEnabled ? numberOfPointsPerSec_DualMode : numberOfPointsPerSec_SingleMode;
        int numberOfPointsPerRotation = numberOfPointsPerSec / rotationRate;
        int numberOfHorizontalScan = numberOfPointsPerRotation / verticalRayNumber;
        float angularResolution = ((float)fieldOfView) / ((float) numberOfHorizontalScan);
        parameterScript.log("[Lidar_model][computeAngularResolutionHorizontal_Velodyne_HDL_64E] dualReturnModeEnabled=" + dualReturnModeEnabled + " rotationRate=" + rotationRate + " verticalRayNumber=" + verticalRayNumber + " fieldOfView=" + fieldOfView + " numberOfPointsPerSec=" + numberOfPointsPerSec + " numberOfPointsPerRotation=" + numberOfPointsPerRotation + " numberOfHorizontalScan=" + numberOfHorizontalScan + " angularResolution=" + angularResolution, 1);
        return angularResolution;
    }

    private void printLidarParameters()
    {
        int numberOfPointsPerScan = numberOfChannel * numberOfRayHorizontal;
        int numberOfPointsPerSec = numberOfPointsPerScan * rotationRateHz;
        parameterScript.log("[Lidar_model][printLidarParameters] rotationRateHz=" + rotationRateHz + " frameRate=" + frameRate + " numberOfChannel=" + numberOfChannel + " numberOfRayHorizontal=" + numberOfRayHorizontal + " numberOfPointsPerScan="+ numberOfPointsPerScan + " numberOfPointsPerSec=" + numberOfPointsPerSec + " fieldOfViewVertical=[" + fieldOfViewVertical[0] + ";] angularResolutionVertical=" + angularResolutionVertical + " fieldOfViewHorizontal=[" + fieldOfViewHorizontal[0] + ";" + fieldOfViewHorizontal[1] + "] angularResolutionHorizontal=" + angularResolutionHorizontal + " measurementRange=" + measurementRange + " accuracy" + accuracy, 1);
    }

    private float angleHorizontalToAngleVertical(float angle)
    {
        return ((float)90 - (angle));
    }

    private void initializeOnFirstFrame()
    {
        initializeLidarParameters();
        tracePath = parameterScript.getTraceFolder();
        lidarTracePath = Path.Combine(tracePath, LidarDir);
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
                for (int horizontalAngleIt = 0; horizontalAngleIt < numberOfRayHorizontal; horizontalAngleIt++)
                {
                    float horizontalAngle = fieldOfViewHorizontal[0] + horizontalAngleIt * angularResolutionHorizontal;
                    rayCastNumber++;
                    if (rayCastNumber > lines.Length)
                    {
                        parameterScript.log("[Lidar_model][createRays] frameCount=" + Time.frameCount + " rayCastNumber =" + rayCastNumber + " > lines.Length=" + lines.Length + " verticalAngle=" + verticalAngle + " horizontalAngle=" + horizontalAngle + " fieldOfViewHorizontal[0]=" + angleHorizontalToAngleVertical(fieldOfViewHorizontal[0]) + " fieldOfViewHorizontal[1]=" + angleHorizontalToAngleVertical(fieldOfViewHorizontal[1]) + " angularResolutionVertical=" + angularResolutionVertical, -1);
                    }

                    float distanceX = (float)Math.Cos((float)((horizontalAngle) * Math.PI / 180));
                    float distanceZ = (float)Math.Sin((float)((horizontalAngle) * Math.PI / 180));
                    float distanceY = (float)Math.Cos((float)((verticalAngle) * Math.PI / 180));
                    Vector3 direction = headGameObject.transform.rotation * new Vector3(distanceX, distanceY, distanceZ);
                    parameterScript.log("[Lidar_model][createRays][3] verticalAngle=" + verticalAngle + " horizontalAngle=" + horizontalAngle + " verticalAngleIt=" + verticalAngleIt + " horizontalAngleIt=" + horizontalAngleIt + " distanceX=" + distanceX + " distanceZ=" + distanceZ + " distanceY=" + distanceY + " direction=" + direction + " laserGameObject.transform.rotation=" + headGameObject.transform.localRotation, 3);                    
                    RaycastHit hit;

                    //if (Physics.Raycast(positionLidar, direction, out hit, distance))
                    if (Physics.Raycast(positionLidar, direction, out hit, measurementRange))
                    {
                        if (hit.collider.tag != "Player")
                        {
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
                            parameterScript.log("[Lidar_model][createRays][5] frameCount=" + Time.frameCount + " lineIterator=" + lineIterator + " lines[lineIterator]=" + lines[lineIterator], 3);
                            floatLines[lineIterator * 4 + 0] = point.x;
                            floatLines[lineIterator * 4 + 1] = point.z;
                            floatLines[lineIterator * 4 + 2] = point.y;
                            floatLines[lineIterator * 4 + 3] = (float)reflectance;

                            lineIterator++;
                        }
                    }
                }
            }

            if (rayCastNumber != numberOfRayVertical * numberOfRayHorizontal)
            {
                int numberExpected = numberOfRayVertical * numberOfRayHorizontal;
                parameterScript.log("[Lidar_model][createRays] frameCount=" + Time.frameCount + " rayCastNumber =" + rayCastNumber + " != numberExpected=" + numberExpected, -1);
            }
        }
    }
    void Update()
    {
        parameterScript.log("[Lidar_model][Update] frame=" + Time.frameCount + " fixedTime=" + Time.fixedTime + " fixedDeltaTime=" + Time.fixedDeltaTime + " time=" + Time.time, 2);

    }
    void FixedUpdate()
    {
        fixedUpdateCounter++;
        if (lidarEnable)
        {
            if (fixedUpdateCounter == 0)
            {
                initializeOnFirstFrame();
            }
            bool lidarFrame = isFrameLidarFrame(fixedUpdateCounter);
            parameterScript.log("[Lidar_model][FixedUpdate] fixedUpdateCounter="+ fixedUpdateCounter + " frame=" + Time.frameCount + " isFrameLidarFrame=" + lidarFrame + " fixedTime=" + Time.fixedTime + " fixedDeltaTime=" + Time.fixedDeltaTime + " time=" + Time.time, 2);
            if (lidarFrame)
            {                
                if (useRayCast)
                {
                    lineIterator = 0;
                    createRayCast();
                }
                //commandServerScript.sendLidarInfo(4 * lineIterator, floatLines);
                commandServerScript.sendMsg(CommandServer.MsgHeaderType.lidarInfo, 4 * lineIterator, floatLines);
                if (recordingEnable == true)
                {
                    writeFile(Time.frameCount, Time.time);
                }
            }
        }
    }

    void writeFile(int frame, float time)
    {        
        //int s = (int)(time);
        //int ms = (int) ((time - (float) s)* ((float) 1000));        
        //string outputFile = lidarTracePath + "\\" +s.ToString().PadLeft(4, '0') + "s_"+ms.ToString().PadLeft(4,'0')+ "ms.txt";

        int time_ms = (int)(Time.time * 1000);
        string outputFile = lidarTracePath + "\\" + time_ms.ToString().PadLeft(10, '0')+".txt";

        parameterScript.log("[Lidar_model][writeFile] frameCount=" + Time.frameCount + " Writing lines to output file:" + outputFile, 2);
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

    private bool isFrameLidarFrame(int frame)
    {
        bool isFrameLidarFrame = false;
        if (parameterLidarScript.useRotationHz() == true)
        {


            //int frameIndexInTheSecond =frame % frameRate;
            if (frame % (frameRate / rotationRateHz) == 0)
            {
                isFrameLidarFrame = true;
            }
        }
        else
        {
            return true;
        }
        parameterScript.log("[Lidar_model][isFrameLidarFrame] frame=" + frame + " isFrameLidarFrame=" + isFrameLidarFrame + " rotationRateHz=" + rotationRateHz, 3);
        return isFrameLidarFrame;
    }
}
