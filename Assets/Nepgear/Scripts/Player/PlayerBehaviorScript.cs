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

    public float maxLifeStock = 3f;

    [SyncVar(hook = "OnChangeLife")]
    public float lifeStock;

    public bool enabledControl = false;

    private float respawnTime = 8f;

    [SyncVar(hook = "OnChangeHealth")]
    public float hitPoint;

    [HideInInspector] public float maxHitPoint;
    [HideInInspector] public float stamina;
    [HideInInspector] public float maxStamina;
    [HideInInspector] public float ultimateCharge;
    public UltimateAbility ultimate;
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

    //private float ultimateActiveDuration;
    private bool isUltimateActived;

    public RectTransform healthBar;

    private GameObject mainCamera;

    public UIManager uiManager;
    private InputHandler ih;
    private RagdollManager ragdollManager;
    private Animator animator;

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
    private ParticleSystem shockParticle;
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

    public bool debug = false;

    protected void Start()
    {
        if (isServer)
        {
            lifeStock = maxLifeStock;
        }

        if (Prototype.NetworkLobby.LobbyManager.s_Singleton != null)
        {
            Character frame = Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetCharacter(characterID);
            SetFrame(Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetCharacter(characterID));
        }
        else
        {
            Character basicFrame = new Character();
            SetFrame(basicFrame);
        }

        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<FirstPersonController>();
        ih = GetComponent<InputHandler>();
        ragdollManager = GetComponent<RagdollManager>();
        animator = GetComponent<Animator>();

        //Debug.Log("Can find ui?? " + GetComponentInChildren<UIManager>().ToString());
        //uiManager = GetComponentInChildren<UIManager>();
        //uiManager = uiObject.GetComponent<UIManager>();
        mainCamera = Camera.main.gameObject;

        if (debug)
        {
            EnablePlayer();
        }
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
        ultimate.Initialize(this.gameObject);
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

        if (isLocalPlayer || debug)
            mainCamera.SetActive(true);

        onToggleShared.Invoke(false);

        if (isLocalPlayer || debug)
            onToggleLocal.Invoke(false);
        else
            onToggleRemote.Invoke(false);
    }

    public void EnablePlayer()
    {
        SetFrameActive(true);
        ragdollManager.DisableRagdoll();

        if (isLocalPlayer || debug)
            mainCamera.SetActive(false);

        onToggleShared.Invoke(true);

        if (isLocalPlayer || debug)
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
        //if (isUltimateActived)
        //{
        //    ultimateActiveDuration += Time.deltaTime;
        //    if (ultimateActiveDuration >= ultimate.duration)
        //    {
        //        ultimate.TriggerAbilityEnd();
        //        ultimateActiveDuration = 0f;
        //        ultimateCharge = 0f;
        //        isUltimateActived = false;

        //    }
        //}

        if (uiManager != null)
        {
            uiManager.SetStamina(stamina * 1.0f / maxStamina * 1.0f);
            uiManager.SetStagger(stagger * 1.0f / staggerLimit * 1.0f);
        }

    }
    private void FixedUpdate()
    {
        if (isServer)
        {
            if (stagger > 0f)
            {
                stagger -= staggerRecovery * Time.fixedDeltaTime;
                if (isStaggering)
                {
                    stagger -= staggerRecovery * Time.fixedDeltaTime;
                }
                if (stagger < 0f)
                {
                    stagger = 0f;
                    if (isStaggering)
                    {
                        RpcOnStaggerFinish();
                        isStaggering = false;
                    }
                }
            }
        }

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
        if (HasUltimate())
        {
            if (ultimateCharge < ultimate.maxCharge)
            {
                // For test purpose;
                //ultimateCharge += (ultimate.maxCharge) * Time.fixedDeltaTime;
                ultimateCharge = ultimate.maxCharge;
                if (ultimateCharge > ultimate.maxCharge)
                {
                    ultimateCharge = ultimate.maxCharge;
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
        if (ih.ultimate && !isUltimateActived && IsUltimateAvailable())
        {
            StartCoroutine(UltimateCoroutine());
            //uiManager.SetUltimate(ultimateCharge, ultimateCharge / ultimate.maxCharge);
        }

        if (!ih.dashing)
        {
            timePressedKey = 0f;
        }

        stamina -= staminaUsed;
    }

    IEnumerator UltimateCoroutine()
    {
        isUltimateActived = true;
        ultimate.TriggerAbility();
        ultimateCharge = 0f;
        yield return new WaitForSeconds(ultimate.duration);
        ultimate.TriggerAbilityEnd();
        isUltimateActived = false;
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

    [Command]
    public void CmdOverclock(float multiplier)
    {
        //floatSpeed *= multiplier;
        //runSpeed *= multiplier;
        //walkSpeed *= multiplier;
        RpcOverclock(multiplier);
    }
    [Command]
    public void CmdStopOverClock(float multiplier)
    {
        RpcStopOverclock(multiplier);
    }
    [ClientRpc]
    public void RpcOverclock(float multiplier)
    {
        floatSpeed *= multiplier;
        runSpeed *= multiplier;
        walkSpeed *= multiplier;
    }
    [ClientRpc]
    public void RpcStopOverclock(float multiplier)
    {
        floatSpeed /= multiplier;
        runSpeed /= multiplier;
        walkSpeed /= multiplier;
    }

    [Server]
    public void Stagger()
    {
        RpcOnStagger();
        isStaggering = true;
    }

    [ClientRpc]
    private void RpcOnStagger()
    {
        DisableControl();
        animator.SetTrigger("Stun");
        if(shockParticle != null)
            shockParticle.Play();
    }

    [ClientRpc]
    private void RpcOnStaggerFinish()
    {
        EnableControl();
    }

    [Server]
    public void Die()
    {
        dead = true;
        lifeStock--;
        //GameManager.instance.OnPlayerDie();

        RpcDie();

        if (!isOutOfStock())
        {
            StartCoroutine(Respawning());
        }
    }

    public void OnChangeLife(float lifeStock)
    {
        this.lifeStock = lifeStock;
        GameManager.UpdateTeamScore();
    }

    private IEnumerator Respawning()
    {
        float remainingTime = respawnTime;
        int floorTime = Mathf.FloorToInt(remainingTime);

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;

                if (floorTime > 0)
                {
                    RpcSetRespawningTime(floorTime);
                }
            }
        }
        Respawn();
    }

    [ClientRpc]
    public void RpcSetRespawningTime(float respawnTime)
    {
        if(uiManager != null)
        {

        }
        else
        {
            Debug.Log(respawnTime);
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
        if(!isLocalPlayer)
            SetFrameActive(false);
        //DisablePlayer();
    }

    void ResetPlayerStatus()
    {
        hitPoint = maxHitPoint;
        stamina = maxStamina;
        stagger = 0f;
        isStaggering = false;
    }

    void OnChangeHealth (float currentHealth)
    {
        hitPoint = currentHealth;
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

    public void WinGame()
    {
        Debug.Log("WIN");
    }

    public void LoseGame()
    {
        Debug.Log("LOSE");
    }

    private bool HasUltimate()
    {
        return ultimate != null;
    }

    private bool IsUltimateAvailable()
    {
        return ultimateCharge >= ultimate.maxCharge;
    }
}
