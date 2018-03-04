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
    private CharacterController characterController;
    private FirstPersonController firstPersonController;
    private Rigidbody rigidbody;
    public Frame frame;

    private float walkSpeed;
    private float runSpeed;

    [SyncVar(hook = "OnChangeHealth")]
    public float hitPoint;
    private float maxHitPoint;
    private float stamina;
    private float maxStamina;
    private float ultimateCharge;
    private bool m_isDashing;
    private float lerpTime;
    private float currentLerpTime;
    private bool isRunning;

    private float m_DashDistance;
    private Vector3 m_CharacterDashStartPos;
    private Vector3 m_CharacterDashEndPos;
    private float timePressedKey;

    private float floatSpeed;
    private bool m_Float;

    private float boostChargeTime;

    private float ultimateActiveDuration;
    private bool isUltimateActived;

    public RectTransform healthBar;

    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;

    GameObject mainCamera;

    UIManager uiManager;
    InputHandler ih;

    protected void Start()
    {
        frame = new BasicFrame();
        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<FirstPersonController>();
        ih = GetComponent<InputHandler>();
        SetFrame(true);
        currentLerpTime = 0f;
        timePressedKey = 0f;
        m_Float = false;
        isUltimateActived = false;
        uiManager = FindObjectOfType<UIManager>();
        healthBar.sizeDelta = new Vector2(hitPoint, healthBar.sizeDelta.y);

        mainCamera = Camera.main.gameObject;

        EnablePlayer();
    }

    public override void OnStartLocalPlayer()
    {
        /*GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;*/
    }

    void DisablePlayer()
    {
        if (isLocalPlayer)
            mainCamera.SetActive(true);

        onToggleShared.Invoke(false);

        if (isLocalPlayer)
            onToggleLocal.Invoke(false);
        else
            onToggleRemote.Invoke(false);
    }

    void EnablePlayer()
    {
        if (isLocalPlayer)
            mainCamera.SetActive(false);

        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);
    }

    private void SetFrame(bool isStart = false)
    {
        walkSpeed = frame.GetSpeed();
        runSpeed = frame.GetSpeed() * 1.5f;
        maxHitPoint = frame.GetHitPoint();
        maxStamina = frame.GetStamina();
        ultimateCharge = 0f;
        floatSpeed = frame.GetSpeed() * 1.25f;
        lerpTime = .25f;
        m_DashDistance = frame.GetSpeed();
        if (isStart)
        {
            hitPoint = maxHitPoint;
            stamina = maxStamina;
        }

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
            if (ultimateActiveDuration >= frame.GetUltimateDuration())
            {
                frame.UltimateEnd();
                ultimateActiveDuration = 0f;
                ultimateCharge = 0f;
                isUltimateActived = false;
                SetFrame();
            } 
        }
        uiManager.SetStamina(stamina, stamina*1.0f / maxStamina*1.0f);
        uiManager.SetUltimate(ultimateCharge, ultimateCharge / frame.GetUltimateMaxCharge());
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
                stamina += (frame.GetStamina()) * Time.fixedDeltaTime;
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
        if (ultimateCharge <= frame.GetUltimateMaxCharge())
        {
            ultimateCharge += (frame.GetUltimateMaxCharge()) * Time.fixedDeltaTime;
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
            frame.Ultimate();
            ultimateCharge = 0f;
            SetFrame();
            uiManager.SetUltimate(ultimateCharge, ultimateCharge / frame.GetUltimateMaxCharge());
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
    public Frame GetFrame()
    {
        return frame;
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

    private void Die()
    {
        DisablePlayer();
    }

    void OnChangeHealth (float currentHealth)
    {
        //Debug.Log("Change Health" + hitPoint.ToString());
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }
}
