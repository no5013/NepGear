using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public Transform stareAt;

	// Update is called once per frame
	void Update () {
        if(stareAt != null)
            transform.LookAt(2 * transform.position - stareAt.position);
    }
}
