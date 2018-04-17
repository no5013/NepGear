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

    [SerializeField] ToggleEvent onToggleRenderer;
    [SerializeField] ToggleEvent onToggleControl;

    private CharacterController characterController;
    private FirstPersonController firstPersonController;
    private Rigidbody rigidbody;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [SyncVar]
    public float lifeStock;

    public bool enabledControl = false;

    private float respawnTime = 5f;

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

    private GameObject mainCamera;

    public UIManager uiManager;
    private InputHandler ih;
    private RagdollManager ragdollManager;

    [SyncVar]
    public string characterID;

    [SyncVar]
    public string team;

    [SyncVar]
    public bool dead = false;

    [SerializeField]
    private Character charFrame;

    [SerializeField]
    private ParticleSystem explosionParticle;
    [SerializeField]
    private AudioClip explosionSound;
    [SerializeField]
    private float timeBeforeExplode = 2f;

    private CatapultManager catapult;
    [SyncVar]
    public float stagger;
    private float staggerRecovery;
    private float staggerLimit;
    [SyncVar]
    public bool isStaggering;

    protected void Start()
    {
        SetFrame(Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetCharacter(characterID));
        lifeStock = 3;

        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<FirstPersonController>();
        ih = GetComponent<InputHandler>();
        ragdollManager = GetComponent<RagdollManager>();

        //Debug.Log("Can find ui?? " + GetComponentInChildren<UIManager>().ToString());
        //uiManager = GetComponentInChildren<UIManager>();
        //uiManager = uiObject.GetComponent<UIManager>();
        mainCamera = Camera.main.gameObject;
    }

    public void Initialize(Character frame)
    {
        walkSpeed = frame.startingSpeed;
        runSpeed = frame.boostSpeed;

        maxHitPoint = frame.maxHitPoint;
        maxStamina = frame.maxStamina;
        ultimateCharge = 0f;
        floatSpeed = frame.jumpForce;
        lerpTime = 0.25f;
        m_DashDistance = frame.startingSpeed;
        hitPoint = maxHitPoint;
        stamina = maxStamina;
        ultimate = frame.ultimate;
        stagger = 0f;
        staggerLimit = frame.staggerLimit;
        staggerRecovery = frame.staggerRecovery;
        isStaggering = false;

        currentLerpTime = 0f;
        timePressedKey = 0f;
        m_Float = false;
        isUltimateActived = false;
        //uiManager = FindObjectOfType<UIManager>();
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
        SetFrameActive(false);

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
        SetFrameActive(true);
        ragdollManager.DisableRagdoll();

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

        if (ih.dashing)
        {
            timePressedKey += Time.deltaTime;
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

        if (uiManager != null)
        {
            uiManager.SetStamina(stamina * 1.0f / maxStamina * 1.0f);
            uiManager.SetStagger(stagger * 1.0f / staggerLimit * 1.0f);
        }
           
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
        if (stagger > 0f)
        {
            stagger -= staggerRecovery*Time.fixedDeltaTime;
            if (isStaggering)
            {
                stagger -= staggerRecovery * Time.fixedDeltaTime;
            }
            if (stagger < 0f)
            {
                stagger = 0f;
                if(isStaggering)
                {
                    EnableControl();
                    isStaggering = false;
                }
            }
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

        if (!ih.dashing && 0.0f < timePressedKey && timePressedKey < 0.30f && !IsExhausted() && !IsDashing())
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

        if (ih.dashing && timePressedKey >= 0.30f && !IsExhausted())
        {
            isRunning = true;
            staminaUsed += 1f;
        }
        else
        {
            isRunning = false;
        }
        if (ih.jumping && !IsExhausted())
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
            //uiManager.SetUltimate(ultimateCharge, ultimateCharge / ultimate.maxCharge);
        }

        if (!ih.dashing)
        {
            timePressedKey = 0f;
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

        Debug.Log("DASH");
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
        if (dead)
            return;

        RpcTakeDamage(damage);
        hitPoint -= damage;

        if (hitPoint <=0)
        {
            hitPoint = 0;

            Die();
        }
    }
    [Server]
    public void Staggering(float staggerDamage)
    {
        if (dead)
            return;
        if (isStaggering)
            return;
        RpcStaggering(staggerDamage);
        stagger += staggerDamage;

        if(stagger >= staggerLimit)
        {
            stagger = staggerLimit;

            Stagger();
        }
    }

    [Server]
    public void Stagger()
    {
        isStaggering = true;
        DisableControl();
        Debug.Log("Staggering");
    }

    [Server]
    public void Die()
    {
        dead = true;
        lifeStock--;
        RpcDie();
        if (!isOutOfStock())
        {
            Invoke("Respawn", respawnTime);
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(float damage)
    {

    }

    [ClientRpc]
    public void RpcStaggering(float damage)
    {

    }

    [ClientRpc]
    void RpcDie()
    {
        Explode();
        DisableControl();
        ragdollManager.EnableRagdoll();
        Invoke("FrameExplode", timeBeforeExplode);
    }

    [Server]
    void Respawn()
    {
        dead = false;
        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn()
    {
        ResetPlayerStatus();

        catapult.SetupFrame(this.gameObject);
        EnablePlayer();
        DisableControl();

        catapult.launch();
    }

    void Explode()
    {
        explosionParticle.Play();
        AudioSource.PlayClipAtPoint(explosionSound, transform.position, 10f);

    }

    void FrameExplode()
    {
        Explode();
        SetFrameActive(false);
        DisablePlayer();
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
            if(uiManager != null)
                uiManager.SetHealth(currentHealth * 1.0f / maxHitPoint * 1.0f);
        }
    }

    public void TickIndicator(string dir)
    {
        if (uiManager != null)
            uiManager.TickDamage(dir);
    }

    public void EnableControl()
    {
        onToggleControl.Invoke(true);
        enabledControl = true;
    }

    public void DisableControl()
    {
        onToggleControl.Invoke(false);
        enabledControl = false;
    }

    public bool isDead()
    {
        return (hitPoint <= 0);
    }

    public bool isOutOfStock()
    {
        return (lifeStock <= 0);
    }

    private void SetFrameActive(bool active)
    {
        onToggleRenderer.Invoke(active);
    }

    public void SetCatapult(CatapultManager newCatapult)
    {
        this.catapult = newCatapult;
    }
}
