using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour {

    private LineRenderer lr;
    private const float lineWidth = 0.04f;
    public const float maxDistance = 10f;
    public bool isCurve;
	// Use this for initialization
	void Start () {
        lr = GetComponent<LineRenderer>();
        //if(isCurve)
        //{
        //    Vector3[] positions = new Vector3[3];
        //    positions[0] = new Vector3(0f, 0.0f, 0.0f);
        //    positions[1] = new Vector3(0.0f, -0.5f, 5.0f);
        //    positions[2] = new Vector3(0.0f, -2.0f, 10.0f);
        //    lr.positionCount = positions.Length;
        //    lr.SetPositions(positions);
        //}
  
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (!isCurve)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
            {
                lr.SetPosition(1, new Vector3(0, 0, hit.distance + 50f));
            }
            else
            {
                lr.SetPosition(1, new Vector3(0, 0, maxDistance));
            }
        }
        //else
        //{
        //    AnimationCurve curve = new AnimationCurve();
        //    if (isCurve)
        //    {
        //        curve.AddKey(0.0f, 0.0f);
        //        curve.AddKey(1.0f, 1.0f);
        //    }
        //    else
        //    {
        //        curve.AddKey(0.0f, 1.0f);
        //        curve.AddKey(1.0f, 1.0f);
        //    }

        //    lr.widthCurve = curve;
        //    lr.widthMultiplier = lineWidth;
        //}
		
	}
}
