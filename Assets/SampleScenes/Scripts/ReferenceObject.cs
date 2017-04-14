using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReferenceObject : MonoBehaviour
{
    private string fileName = "\\tracklets.txt";
    private string fullFileName = "";
    private Parameters parameter;
    public List<string> listTags = new List<string>()
        {
            "Player",
            "car",
            "pedestrian",
            "sign",
            "Untagged"
        };

    public bool objectTracking = true;
    private List<Collider> colliderList = new List<Collider>();
    private List<GameObject> objectList = new List<GameObject>();
    private Hashtable objectBox = new Hashtable();
    private int myCameraFps;
    private int frameRate;
    private int fixedUpdateCounter = -1;

    void Start()
    {      
        parameter = GameObject.Find("Parameters").GetComponent<Parameters>();

        fullFileName = parameter.getTraceFolder() + fileName;
        writer = new StreamWriter(fullFileName);
        writer.Close();
    }

    private List<GameObject> allObject;
    TextWriter writer;

    // Update is called once per frame
    private void FixedUpdate()
    {

        fixedUpdateCounter++;
        if (fixedUpdateCounter == 0)
        {
            myCameraFps = GameObject.Find("Parameters").GetComponent<Parameters_CarCamera>().getSideCameraFps();
            frameRate = parameter.getFrameRate();
            findObject();
            objectPosition();
        }
        if (objectTracking == true)
        {
            bool isCameraFrame = isFrameCameraFrame(fixedUpdateCounter);
            if (isCameraFrame == true)
            {
                objectPosition();                
            }
        }
    }

    private void objectPosition()
    {
        GameObject[] allObject;
        foreach (string tag in listTags)
        {
            allObject = GameObject.FindGameObjectsWithTag(tag);
            if (allObject.Length > 0)
            {
                for (int i = 0; i < allObject.Length; i++)
                {
                    if (objectBox.ContainsKey(allObject[i].name))
                    {
                        List<Vector3> tmp = objectBox[allObject[i].name] as List<Vector3>;
                        printTraclets(Time.frameCount, allObject[i]);
                        drawBox(tmp, allObject[i].transform.position, allObject[i].transform.rotation.eulerAngles, Color.yellow);
                    }
                }
            }

        }
    }

    private void findObject( )
    {
        GameObject[] allObject;
        //player      
        foreach (string tag in listTags)
        {
            
            allObject = GameObject.FindGameObjectsWithTag(tag);
            Debug.Log("Tag " + tag+" "+allObject.Length);
            if (allObject.Length > 0)
            {
                for (int i = 0; i < allObject.Length; i++)
                {
                    Debug.Log(allObject[i]);
                    objectList.Add(allObject[i]);
                    calculateBoxSize(allObject[i]);
                    
                }
            }

        }

     
        
    }
 

    private void printTraclets(int frame, GameObject gameObject)
    {
        writer = File.AppendText(fullFileName);
        //writer.WriteLine("###########################################");
        //writer.WriteLine("Frame : " + Time.frameCount+" Time : "+ (int)(Time.time * 1000));
        //writer.WriteLine("\tTag : " + gameObject.transform.tag + "\tName : " + gameObject.name + "\tType : " + gameObject.GetType());
        //writer.WriteLine("\t\tPosition      : " + gameObject.transform.position.ToString());
        //writer.WriteLine("\t\tRotation      : " + gameObject.transform.rotation.eulerAngles.ToString());
        //writer.WriteLine("\t\tLocal Position      : " + gameObject.transform.localPosition.ToString());
        //writer.WriteLine("\t\tLocal Rotation      : " + gameObject.transform.localRotation.eulerAngles.ToString());
        writer.WriteLine(Time.frameCount + ";" + (int)(Time.time * 1000)+";"+ gameObject.transform.tag + ";" + gameObject.name + ";" + gameObject.GetType() + ";" + gameObject.transform.position.ToString() + ";" + gameObject.transform.rotation.eulerAngles.ToString());



        writer.WriteLine("-------------------------------------------------------------------");
        writer.Close();

    }


    private void findAllCollider(GameObject currentObject)

    {
        colliderList.Clear();
        if (currentObject.GetComponent("Collider") != null)
        {
            Collider collider = currentObject.GetComponent<Collider>();
            List<float> sublist = new List<float>();
            Debug.Log("Collider " + collider.name);

            colliderList.Add(collider);
        }
        int childNumber = currentObject.transform.GetChildCount();
        if (childNumber > 0)
        {
            for (int child = 0; child < childNumber; child++)
            {
                GameObject childObject = currentObject.transform.GetChild(child).gameObject;
                findChildCollider(childObject);
            }
        }
    }

    private void findChildCollider(GameObject currentObject)
    {
        List<List<float>> tmp = new List<List<float>>();
        int childNumber = currentObject.transform.GetChildCount();
        if (currentObject.GetComponent("Collider") != null)
        {
            Collider collider = currentObject.GetComponent<Collider>();
            List<float> sublist = new List<float>();
            if (!(collider is WheelCollider))
            {
                Debug.Log("Collider " + collider.name);
                colliderList.Add(collider);
            }
        }
        if (childNumber > 0)
        {
            for (int child = 0; child < childNumber; child++)
            {
                GameObject childObject = currentObject.transform.GetChild(child).gameObject;
                //tmp.AddRange(findChildCollider(childObject));
                findChildCollider(childObject);
            }
        }
    }

    private List<Vector3> calculateBoxSize(GameObject sceneObject)
    {
        List<Vector3> tmp = new List<Vector3>();

        Quaternion rot = sceneObject.transform.rotation;
        sceneObject.transform.rotation = Quaternion.identity;
        findAllCollider(sceneObject);
        float minX = 100000;
        float maxX = -100000;
        float minY = 100000;
        float maxY = -100000;
        float minZ = 100000;
        float maxZ = -100000;

        //list of collider in this object
        for (int coll = 0;coll < colliderList.Count; coll++)
        {
            Debug.Log(colliderList[coll].name);
            Vector3 boundsMin =colliderList[coll].bounds.min - sceneObject.transform.position; ;
            Vector3 boundsMax = colliderList[coll].bounds.max - sceneObject.transform.position; ;

            if (boundsMin.x < minX) minX = boundsMin.x;
            if (boundsMin.y < minY) minY = boundsMin.y;
            if (boundsMin.z < minZ) minZ = boundsMin.z;

            if (boundsMax.x > maxX) maxX = boundsMax.x;
            if (boundsMax.y > maxY) maxY = boundsMax.y;
            if (boundsMax.z > maxZ) maxZ = boundsMax.z;
        }

        Vector3 point1 = new Vector3(minX, minY, minZ);
        Vector3 point2 = new Vector3(maxX, maxY, maxZ);
        //point1 = point1 - sceneObject.transform.position;
        //point2 = point2 - sceneObject.transform.position;
        Vector3 point3 = new Vector3(point1.x, point1.y, point2.z);
        Vector3 point4 = new Vector3(point1.x, point2.y, point1.z);
        Vector3 point5 = new Vector3(point2.x, point1.y, point1.z);
        Vector3 point6 = new Vector3(point1.x, point2.y, point2.z);
        Vector3 point7 = new Vector3(point2.x, point1.y, point2.z);
        Vector3 point8 = new Vector3(point2.x, point2.y, point1.z);

        //center 
        Vector3 center = new Vector3( (maxX+minX)/2 , (maxY+minY)/2,(maxZ+minZ)/2);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);






        List<Vector3> listPoint = new List<Vector3>();
        listPoint.Add(point1);
        listPoint.Add(point2);
        listPoint.Add(point3);
        listPoint.Add(point4);
        listPoint.Add(point5);
        listPoint.Add(point6);
        listPoint.Add(point7);
        listPoint.Add(point8);
        listPoint.Add(center);
        objectBox.Add(sceneObject.name, listPoint);
        sceneObject.transform.rotation = rot;



        //
        writer = File.AppendText(fullFileName);
        writer.WriteLine("###########################################");
        writer.WriteLine(sceneObject.name);
        writer.WriteLine(sceneObject.tag);


        for (int i = 0; i < 9; i++)
            writer.WriteLine("\t" + listPoint[i].ToString());

        writer.WriteLine("-------------------------------------------------------------------");
        writer.Close();

        //drawBox(listPoint, Vector3.zero, Vector3.zero,Color.cyan);
        //drawBox(listPoint,sceneObject.transform.position,sceneObject.transform.rotation.eulerAngles,Color.red);

        return tmp;
    }


    private void debugLogBox(List<Vector3> boxpoint, Color color)
    {
        Color lineColor = color;
        // rectangular cuboid
        // top of rectangular cuboid (6-2-8-4)
        Debug.DrawLine(boxpoint[5], boxpoint[1], lineColor);
        Debug.DrawLine(boxpoint[1], boxpoint[7], lineColor);
        Debug.DrawLine(boxpoint[7], boxpoint[3], lineColor);
        Debug.DrawLine(boxpoint[3], boxpoint[5], lineColor);

        // bottom of rectangular cuboid (3-7-5-1)
        Debug.DrawLine(boxpoint[2], boxpoint[6], lineColor);
        Debug.DrawLine(boxpoint[6], boxpoint[4], lineColor);
        Debug.DrawLine(boxpoint[4], boxpoint[0], lineColor);
        Debug.DrawLine(boxpoint[0], boxpoint[2], lineColor);

        // legs (6-3, 2-7, 8-5, 4-1)
        Debug.DrawLine(boxpoint[5], boxpoint[2], lineColor);
        Debug.DrawLine(boxpoint[1], boxpoint[6], lineColor);
        Debug.DrawLine(boxpoint[7], boxpoint[4], lineColor);
        Debug.DrawLine(boxpoint[3], boxpoint[0], lineColor);
    }

    private void drawBox(List<Vector3> box , Vector3 translation , Vector3 rotation,Color color)
    {
        Debug.Log("Draw Box " + translation.ToString()+" "+rotation.ToString());
        //debugLogBox(box, Color.green);
        List<Vector3> newPoint = new List<Vector3>();
        //Quaternion rotate = Quaternion.Euler(rotation);
        for (int i = 0; i < box.Count-1; i++)
        {
            newPoint.Add(Quaternion.Euler(rotation)*(box[i]-box[8])+box[8]+translation);
        }
        debugLogBox(newPoint,color);
    }
    private bool isFrameCameraFrame(int frame)
    {
        bool isFrameCameraFrame = false;
        if (frame % (frameRate / myCameraFps) == 0)
        {
            isFrameCameraFrame = true;
        }
        return isFrameCameraFrame;
    }
}


