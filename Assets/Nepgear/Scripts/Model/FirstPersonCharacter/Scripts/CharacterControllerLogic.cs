using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerLogic : MonoBehaviour {
    [SerializeField] private Animator animator;
    [SerializeField] private float directionDampTime = 0.25f;

    private float speed = 0.0f;
    private float h = 0.0f;
    private float v = 0.0f;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        if(animator.layerCount >= 2)
        {
            animator.SetLayerWeight(1, 2);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (animator)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            speed = new Vector2(h, v).sqrMagnitude;

            Debug.Log(speed);

            animator.SetFloat("Speed", speed);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }
	}
}
