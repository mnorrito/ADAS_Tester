﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_Lidar : MonoBehaviour
{
    public bool lidarEnable;
    public bool Velodyne_HDL_64E;
    public int numberOfChannel;
    public float[] fieldOfViewVertical;
    public float angularResolutionVertical;
    public float[] fieldOfViewHorizontal;
    public float angularResolutionHorizontal;
    public float measurementRange;
    public float accuracy;
    public bool showRay;
    public bool printDebug;    

    // Use this for initialization
    void Start()
    {
        GameObject adasCar = this.GetComponent<Parameters>().getAdasCarGameObject();
        GameObject lidarGameObject = null;
        Lidar_model lidarScript = null;
        for (int i = 0; i < adasCar.transform.childCount; i++)
        {
            GameObject gameObject = adasCar.transform.GetChild(i).gameObject;
            if (gameObject != null)
            {
                if (gameObject.name == "Lidar_model")
                {

                    lidarGameObject = gameObject;
                    lidarScript = lidarGameObject.GetComponent<Lidar_model>();
                    break;
                }
            }
        }

        if (lidarEnable == false && lidarGameObject != null)
        {
            lidarGameObject.SetActive(false);
        }
        else
        {
            if (lidarGameObject != null && lidarScript != null)
            {
                lidarGameObject.SetActive(true);
                lidarScript.setVelodyne_HDL_64E(Velodyne_HDL_64E);
            }
        }
    }

    private void initialize()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }


}
