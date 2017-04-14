using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters_Car : MonoBehaviour {
    [SerializeField]
    private bool standalone;


    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool isStandalone()
    {
        return standalone;
    }

}
