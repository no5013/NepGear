﻿using System.Collections;
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

    [SyncVar]
    public string characterID;

    [SyncVar]
    public string team;

    [SerializeField] private Character charFrame;

    protected void Start()
    {
        SetFrame(Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetCharacter(characterID));
        lifeStock = 3;

        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<FirstPersonController>();
        ih = GetComponent<InputHandler>();
        mainCamera = Camera.main.gameObject;
    }

    public void Initialize(Character frame)
    {
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
    }

    public void SetFrame(Character frame)
    {
        charFrame = Instantiate(frame);
        Initialize(charFrame);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isServer)
            GameManager.AddPlayer(gameObject, team);
    }

    public override void OnNetworkDestroy()
    {
        GameManager.instance.RemovePlayer(gameObject);
    }

    /*public override void OnStartServer()
    {
        GameManager.AddPlayer(gameObject);
        base.OnStartServer();
    }*/

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
                stamina += Math.Abs(stamina+10) * Time.fixedDeltaTime;
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
        float horizontal = 0;
        float vertical = 0;
        float staminaUsed = 0;

        if(ih != null)
        {
            horizontal = ih.horizontal;
            vertical = ih.vertical;
        }

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
        if (isLocalPlayer)
        {
            //uiManager.SetHitpoint(currentHealth, currentHealth * 1.0f / maxHitPoint * 1.0f);
        }
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
