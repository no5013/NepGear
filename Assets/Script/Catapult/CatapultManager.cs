using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultManager : MonoBehaviour {

    public Transform startPosition;
    public Transform endPosition;

    public Transform target;
    public GameObject stand;

    public float maxSpeed = 100f;

    public bool launching;

    private bool reached;

    private float minDistance = 0.2f;
    private float minSpeedPercent = 0.5f;

    private Vector3 moveDirection;

    private Rigidbody rb;
    private CharacterController characterController;
    private Animator animator;

    public GameObject player;

    private float yOffset = 3f;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();

        ResetCatapult();
        SetCatapultTarget(target.position);

        if(player != null)
            SetupFrame(player);
	}

    public void SetupFrame(GameObject frame)
    {
        ResetCatapult();

        frame.transform.parent = stand.transform;
        Vector3 newPosition = Vector3.zero;
        newPosition.y = yOffset;
        frame.transform.localPosition = newPosition;
        frame.transform.localRotation = Quaternion.identity;

        if (animator != null)
        {
            animator.SetTrigger("launch");
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!launching)
        {
            return;
        }

        Vector3 destinationPosition = new Vector3(endPosition.position.x, stand.transform.position.y, endPosition.position.z);
        float distance = Vector3.Distance(stand.transform.position, destinationPosition);
        if (distance < minDistance)
        {
            reached = true;
            detachFrame();
            this.enabled = false;
        }

        if (!reached)
        {
            moveDirection = CalculateMoveDirection();
            Vector3 moveVector = new Vector3(moveDirection.x * maxSpeed, 0f, moveDirection.z * CalculateCurrentSpeed());
            stand.transform.position = Vector3.MoveTowards(stand.transform.position, destinationPosition, CalculateCurrentSpeed() * Time.fixedDeltaTime);
        }
	}

    public void ResetCatapult()
    {
        stand.transform.position = new Vector3(startPosition.position.x, stand.transform.position.y, startPosition.position.z);
        reached = false;
        launching = false;
    }

    public void SetCatapultTarget(Vector3 target)
    {
        Vector3 targetPostition = new Vector3(target.x, transform.position.y, target.z);
        transform.LookAt(targetPostition);
    }

    private Vector3 CalculateMoveDirection()
    {
        Vector3 destinationPosition = new Vector3(endPosition.position.x, stand.transform.position.y, endPosition.position.z);
        Vector3 moveDirection = (destinationPosition - stand.transform.position).normalized;

        return moveDirection;
    }

    private float CalculateCurrentSpeed()
    {
        float distanceStartEnd = Vector3.Distance(startPosition.position, endPosition.position);
        float distanceStandStart = Vector3.Distance(stand.transform.position, new Vector3(startPosition.position.x, stand.transform.position.y, startPosition.position.z));
        float percentDistance = (distanceStandStart + minSpeedPercent / distanceStartEnd) > 1f ? 1f : (distanceStandStart + minSpeedPercent / distanceStartEnd);
        return maxSpeed * percentDistance;
    }

    private void detachFrame()
    {
        PlayerBehaviorScript player = stand.GetComponentInChildren<PlayerBehaviorScript>();
        player.transform.parent = null;

        FrameMover frameMover = player.GetComponent<FrameMover>();
        frameMover.maxSpeed = maxSpeed;
        frameMover.target = target;
        frameMover.enabled = true;
    }

    public void launch()
    {
        this.enabled = true;
        launching = true;
    }
}
