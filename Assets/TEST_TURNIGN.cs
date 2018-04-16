using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_TURNIGN : MonoBehaviour {

    [SerializeField]
    GameObject top;
    [SerializeField]
    GameObject buttom;
    [SerializeField]
    float buttonX;

    [SerializeField]
    float buttonY;

    bool ispressX;
    bool ispressY;
	// Use this for initialization
	void Start () {
		
	}


    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        { if(buttonY<1)buttonY +=0.1f; }
        if (Input.GetKey(KeyCode.A))
        { if (buttonY > -1) buttonX -= 0.1f; }
        if (Input.GetKey(KeyCode.S))
        { if (buttonY > -1) buttonY -= 0.1f; }
        if (Input.GetKey(KeyCode.D))
        { if (buttonX < 1) buttonX += 0.1f; }

       buttom.transform.rotation = Quaternion.RotateTowards(buttom.transform.rotation, Quaternion.LookRotation(new Vector3(buttonX, 0, buttonY)), 20);

    }

    // Update is called once per frame
    void Update () {

       
        
            if (Input.GetKey(KeyCode.Q))
        {
            top.transform.eulerAngles = new Vector3(0, top.transform.eulerAngles.y-1, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            top.transform.eulerAngles = new Vector3(0, top.transform.eulerAngles.y + 1, 0);
        }


    }
}
