using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour {

    private LineRenderer lr;
    public float maxDistance = 10f;
	// Use this for initialization
	void Start () {
        lr = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            lr.SetPosition(1, new Vector3(0, 0, hit.distance + 50f));
        }
        else
        {
            lr.SetPosition(1, new Vector3(0, 0, maxDistance));
        }
	}
}
