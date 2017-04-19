using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_Walker : MonoBehaviour
{
    [SerializeField]
    private bool walkerEnable;
    [SerializeField]
    private bool moveEnable;
    [SerializeField]
    private float walkSpeedMultiplier;
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public float getWalkSpeedMultiplier()
    {
        return walkSpeedMultiplier;
    }

    public bool isWalkerEnabled()
    {
        return walkerEnable;
    }

    public bool isMoveEnabled()
    {
        return moveEnable;
    }
}
