using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class CharacterControllerLogic : NetworkBehaviour {
    [SerializeField] private Animator animator;
    [SerializeField] private float directionDampTime = 0.25f;

    private float speed = 0.0f;
    private float h = 0.0f;
    private float v = 0.0f;
    private float yRot = 0.0f;

    private bool dashing = false;
    private bool dough = false;
    private bool floating = false;

    private InputHandler inputHandler;
    private PlayerBehaviorScript playerBehaviorScript;
    private Thruster[] thrusters;

    public bool debugThruster;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        inputHandler = GetComponent<InputHandler>();
        playerBehaviorScript = GetComponent<PlayerBehaviorScript>();
        thrusters = GetComponentsInChildren<Thruster>(true);

        if(animator.layerCount >= 2)
        {
            animator.SetLayerWeight(1, 2);
        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdateState();

        if (animator && isLocalPlayer)
        {
            h = inputHandler.horizontal;
            v = inputHandler.vertical;
            yRot = inputHandler.yRot;

            speed = new Vector2(h, v).sqrMagnitude;

           // Debug.Log(v);

            animator.SetFloat("Speed", speed);
            animator.SetFloat("Direction", h);
            animator.SetFloat("Forward", v);
            animator.SetBool("Dash", dashing);

            /*if(v <0)
            {
                transform.eulerAngles = new Vector3(0,0,-15);
            }
            else if (v>0)
            {
                transform.eulerAngles = new Vector3(0, 0, 15);

            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);

            }*/
            animator.SetBool("Dough", dough);
            animator.SetBool("Float", floating);
            animator.SetFloat("Turn", yRot);

          //  Debug.Log(dashing);
        }
        EmitThrusters();
	}

    void UpdateState ()
    {
        dashing = playerBehaviorScript.IsRunning();
        dough = playerBehaviorScript.IsDashing();
        floating = playerBehaviorScript.Floating();
    }

    void EmitThrusters()
    {
        if(animator.GetBool("Dash") || animator.GetBool("Dough") || animator.GetBool("Float") || debugThruster)
        {
            foreach(Thruster thruster in thrusters)
            {
                thruster.Emit();
            }
        }
    }
}
