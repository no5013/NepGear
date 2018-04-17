using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandling : MonoBehaviour {

    Animator anim;

    public float ikWeight = 1;

    public Transform LeftIKTarget;
    public Transform RightIKTarget;


	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikWeight);

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, LeftIKTarget.position);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, RightIKTarget.position);
        Debug.Log("TEST");
    }
}
