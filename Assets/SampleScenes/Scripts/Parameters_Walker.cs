using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_Walker : MonoBehaviour
{
    public enum MoveDirection
    {
        R=-1,
        N=0,
        F=1
    }

    [SerializeField]
    private bool walkerEnable;
    [SerializeField]
    private bool moveEnable;
    [SerializeField]
    private float walkSpeedMultiplier;
    [SerializeField]
    private MoveDirection xMovedirection = MoveDirection.N;
    [SerializeField]
    private MoveDirection zMovedirection = MoveDirection.N;
    [SerializeField]
    private float distToStartPoint;

    

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

    public MoveDirection getXMoveDirection()
    {
        return xMovedirection;
    }
    public MoveDirection getZMoveDirection()
    {
        return zMovedirection;
    }


    public float getDistToStartPoint()
    {
        return distToStartPoint;
    }
}
