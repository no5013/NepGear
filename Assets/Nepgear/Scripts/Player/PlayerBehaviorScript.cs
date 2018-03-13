using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }

public class PlayerBehaviorScript : NetworkBehaviour
{

    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;

    private CharacterController characterController;
    private FirstPersonController firstPersonController;
    private Rigidbody rigidbody;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [SyncVar]
    public float lifeStock;

    [SerializeField]
    private float respawnTime = 2f;

    [SyncVar(hook = "OnChangeHealth")]
    public float hitPoint;
    [HideInInspector] public float maxHitPoint;
    [HideInInspector] public float stamina;
    [HideInInspector] public float maxStamina;
    [HideInInspector] public float ultimateCharge;
    private UltimateAbility ultimate;
    private bool m_isDashing;
    private float lerpTime;
    private float currentLerpTime;
    private bool isRunning;

    private float m_DashDistance;
    private Vector3 m_CharacterDashStartPos;
    private Vector3 m_CharacterDashEndPos;
    private float timePressedKey;

    [SerializeField] private float floatSpeed;
    private bool m_Float;

    private float boostChargeTime;

    private float ultimateActiveDuration;
    private bool isUltimateActived;

    public RectTransform healthBar;

    GameObject mainCamera;

    UIManager uiManager;
    InputHandler ih;

    [SerializeField] private Character charFrame;

    protected void Start()
    {

        //Remove When Character Select is done
        //Initialize(charFrame);
    }

    public void Initialize(Character frame)
    {
        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<FirstPersonController>();
        ih = GetComponent<InputHandler>();

        walkSpeed = frame.startingSpeed;
        runSpeed = frame.startingSpeed * 1.5f;
        maxHitPoint = frame.maxHitPoint;
        maxStamina = frame.maxStamina;
        ultimateCharge = 0f;
        floatSpeed = frame.startingSpeed * 1.25f;
        lerpTime = 0.25f;
        m_DashDistance = frame.startingSpeed;
        hitPoint = maxHitPoint;
        stamina = maxStamina;
        ultimate = frame.ultimate;

        currentLerpTime = 0f;
        timePressedKey = 0f;
        m_Float = false;
        isUltimateActived = false;
        uiManager = FindObjectOfType<UIManager>();
        healthBar.sizeDelta = new Vector2(hitPoint, healthBar.sizeDelta.y);

        lifeStock = GameManager.instance.playerLifeStock;

        mainCamera = Camera.main.gameObject;
    }

    public override void OnStartLocalPlayer()
    {
        /*GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;*/
        //EnablePlayer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isServer)
            GameManager.AddPlayer(gameObject);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameManager.AddPlayer(gameObject);
    }

    public void DisablePlayer()
    {
        if (isLocalPlayer)
            mainCamera.SetActive(true);

        onToggleShared.Invoke(false);

        if (isLocalPlayer)
            onToggleLocal.Invoke(false);
        else
            onToggleRemote.Invoke(false);
    }

    public void EnablePlayer()
    {
        if (isLocalPlayer)
            mainCamera.SetActive(false);

        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);
    }


    protected void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (CrossPlatformInputManager.GetButton("Dash"))
        {
            timePressedKey += Time.deltaTime;
        }
        else
        {
            timePressedKey = 0f;
        }
        // Dash can only be used in Update
        if (m_isDashing)
        {
            Dash();
        }
        if (isUltimateActived)
        {
            ultimateActiveDuration += Time.deltaTime;
            if (ultimateActiveDuration >= ultimate.duration)
            {
                ultimate.TriggerAbilityEnd();
                ultimateActiveDuration = 0f;
                ultimateCharge = 0f;
                isUltimateActived = false;

            }
        }
        uiManager.SetStamina(stamina, stamina*1.0f / maxStamina*1.0f);
        uiManager.SetUltimate(ultimateCharge, ultimateCharge / ultimate.maxCharge);
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        GetInput();
        if (characterController.isGrounded && !IsDashing() && !IsRunning())
        {
            if (stamina < maxStamina && boostChargeTime < Time.time)
            {
                stamina += (stamina) * Time.fixedDeltaTime;
                if (stamina > maxStamina)
                {
                    stamina = maxStamina;
                }
            }
        }
        else
        {
            boostChargeTime = Time.time + 1f;
        }
        if (ultimateCharge <= ultimate.maxCharge)
        {
            ultimateCharge += (ultimate.maxCharge) * Time.fixedDeltaTime;
        }

    }
    private void GetInput()
    {
        //        bool walking = true;
        float horizontal = ih.horizontal;
        float vertical = ih.vertical;
        float staminaUsed = 0;

        if (CrossPlatformInputManager.GetButtonUp("Dash") && timePressedKey < 0.30f && !IsExhausted() && !IsDashing())
        {
            m_isDashing = true;
            m_CharacterDashStartPos = characterController.transform.position;
            Vector2 magnitude = new Vector2(horizontal, vertical);
            if (magnitude.sqrMagnitude > 1)
            {
                magnitude.Normalize();
            }
            Vector3 desiredMove = transform.forward * magnitude.y + transform.right * magnitude.x;
            desiredMove *= m_DashDistance;
            m_CharacterDashEndPos = characterController.transform.position + desiredMove;
            staminaUsed += 10f;
            //           walking = false;
        }

        if (CrossPlatformInputManager.GetButton("Dash") && timePressedKey >= 0.30f && !IsExhausted())
        {
            isRunning = true;
            staminaUsed += 1f;
        }
        else
        {
            isRunning = false;
        }
        if (CrossPlatformInputManager.GetButton("Jump") && !IsExhausted())
        {
            m_Float = true;
            staminaUsed += 1f;
        }
        else
        {
            m_Float = false;
        }
        if (CrossPlatformInputManager.GetButtonUp("Ultimate") && !isUltimateActived)
        {
            isUltimateActived = true;
            ultimate.TriggerAbility();
            ultimateCharge = 0f;
            uiManager.SetUltimate(ultimateCharge, ultimateCharge / ultimate.maxCharge);
        }
        //       if (Input.GetKey(KeyCode.Space))
        //       {
        //           Debug.Log("Float!!!!!!");
        //           m_Float = true;
        //      }
        //       else
        //       {
        //           m_Float = false;
        //       }
        stamina -= staminaUsed;
    }
    private void Dash()
    {
        m_isDashing = true;
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
        }
        float perc = currentLerpTime / lerpTime;
        perc = perc * perc * (3f - 2f * perc);
        characterController.transform.position = Vector3.Lerp(m_CharacterDashStartPos, m_CharacterDashEndPos, perc);
        firstPersonController.SendMessage("UpdateCameraPosition", 1f);
        if (perc > 0.98f)
        {
            m_isDashing = false;
            currentLerpTime = 0f;
        }
    }
    public bool IsDashing()
    {
        return m_isDashing;
    }
    public float GetWalkSpeed()
    {
        return walkSpeed;
    }
    public float GetRunSpeed()
    {
        return runSpeed;
    }
    public bool Floating()
    {
        return m_Float;
    }
    public float GetFloatSpeed()
    {
        return floatSpeed;
    }
    public bool IsRunning()
    {
        return isRunning;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Unpassable Object"))
        {
            m_isDashing = false;
        }
        //else if (other.tag.Equals("Bullet"))
        //{
        //    hitPoint -= other.gameObject.GetComponent<Bullet>().GetDamage();
        //    uiManager.SetHitpoint(hitPoint, hitPoint/maxHitPoint);
        //    Destroy(other.gameObject);
        //}
    }
    public bool IsExhausted()
    {
        return stamina <= 0;
    }

    [Server]
    public void TakeDamage(float damage)
    {
        hitPoint -= damage;
        if (hitPoint <=0)
        {
            hitPoint = 0;
            Die();
        }
    }

    [Server]
    private void Die()
    {
        lifeStock--;
        RpcOnDie();
    }

    [ClientRpc]
    void RpcOnDie()
    {
        DisablePlayer();

        if (!isOutOfStock())
        {
            Debug.Log("RESPAWN");
            Invoke("Respawn", respawnTime);
        }
    }

    void Respawn()
    {
        if (isLocalPlayer)
        {
            Transform spawn = NetworkManager.singleton.GetStartPosition();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }
        ResetPlayerStatus();
        EnablePlayer();
    }

    void ResetPlayerStatus()
    {
        hitPoint = maxHitPoint;
        stamina = maxStamina;
}

    void OnChangeHealth (float currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    public bool isDead()
    {
        return (hitPoint <= 0);
    }

    public bool isOutOfStock()
    {
        return (lifeStock <= 0);
    }
}
