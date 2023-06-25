using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable Unity.InefficientPropertyAccess

public class HitablePlayer : HittableObject
{
    [SerializeField] private AudioMixer audioMixer;
    private GameObject healthBar;

    private Slider healthSlider;
    private TMP_Text healthText;

    private Vector3 scale;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;
    private PlayerStats stats;
    private Collider2D bodycol;
    public delegate void PlayerDeathDelegate();
    public static PlayerDeathDelegate onPlayerDeath;
    public GameObject StopMusic;
    public GameObject PostProcess;
    public float slowMotionDuration = 0.5f;

    private Animator animator;
    public bool isAlreadyDestroyed { private get; set; } = false;
    public GameObject HitCharacterSound;
    public Camera mainCamera;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Update()
    {
        StopMusic = GameObject.FindGameObjectWithTag("StopMusic");
    }

    protected override void Start()
    {
        base.Start();
        scale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();
        bodycol = transform.GetChild(4).GetComponent<Collider2D>();
        
        maxHealth = stats.getMaxHealth();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }
    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier,GameObject damageSource,bool heavy)
    {
        //visual Feedback
        StartCoroutine(HitFeedback());
        CameraShake.Instance.ShakeCamera(0.5f,.7f,true);
        StartCoroutine(ResetCameraRotation(0.2f));
        Invoke("ResetRotation", 0.2f);
        HitCharacterSound.GetComponent<RandomSound>().PlayRandom1();

        base.GetHit(damage,damageSourcePosition, knockbackMultiplier,damageSource,heavy);
        
        //knockback
        float sizeMultiplier;
        //knockback enemy
        if(size == 0){//imoveable object
            sizeMultiplier = 0;
        }else{
            sizeMultiplier = 1 / size;
        }

        var position = transform.position;
        Vector2 knockbackDirection = new Vector2(position.x,position.y) - damageSourcePosition;
        rb.velocity = knockbackDirection.normalized * (sizeMultiplier * knockbackMultiplier);

        UpdateHealthBar();

        //Post Process
        PostProcess = GameObject.FindGameObjectWithTag("Volume");
        PostProcess.GetComponent<PostProcessEffects>().CharacterHit();

        //Slow Motion Hit
        StartCoroutine(ChangeGameMovementSlow(1f, 0.2f, 0f, 0.1f, 800));
        StartCoroutine(ChangeGameMovementNormal(0.1f));
    }

    public void ResetRotation()
    {
        mainCamera.transform.rotation = Quaternion.identity;
    }

    private IEnumerator ResetCameraRotation(float duration)
    {
        Quaternion startRotation = mainCamera.transform.rotation;
        Quaternion endRotation = Quaternion.identity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        mainCamera.transform.rotation = endRotation;
    }

    IEnumerator ChangeGameMovementSlow(float startIntensity, float endIntensity, float elapsedTime, float duration,  float musicLowPass)
    {
        audioMixer.SetFloat("musicLowPass", musicLowPass);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            Time.timeScale = Mathf.Lerp(startIntensity, endIntensity, t);
            yield return null;
        }

        Time.timeScale = endIntensity;

        yield return null;
    }

    IEnumerator ChangeGameMovementNormal(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(ChangeGameMovementSlow(0.2f, 1f, 0f, 0.1f, 22000));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //set this variable in DontDestroyOnLoad script if this instance of player is a copy --> cleaner approach would be nice
        if (isAlreadyDestroyed)
            return;

        if (scene.name == "Valhalla")
        {
            maxHealth = stats.getMaxHealth();
            currentHealth = maxHealth;

            healthBar = GameObject.Find("/InGameCanvas/HealthBar");

            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.GetComponent<PlayerHealthBar>().UpdateHealthBar(currentHealth, maxHealth);
    }
    private IEnumerator HitFeedback(){
        transform.localScale *= 1.1f;
        spriteRenderer.color = new Color(255,0,0,255);
        yield return new WaitForSeconds(.1f);
        transform.localScale = scale;
        spriteRenderer.color = new Color(255,255,255,255);
    }

    protected override void Die(GameObject damageSource)
    {
        //transform.position = new Vector3(-4.5f, -1.5f, 0);
        Debug.Log("NOW YOU DIE");
        StartCoroutine(OnPlayerDeath());
        
        //base.Die(); TODO talk about correct resetting of player on death
        //gameObject.SetActive(false);
    }

    IEnumerator OnPlayerDeath()
    {
        healthBar.SetActive(false);
        print("HEALTHBAR DEACTIVATED");
        animator.SetTrigger("onDeath");
        print("DEATH ANIM STARTED");
        Destroy(StopMusic);
        print("NO MUSIC FOR YOU");
        PostProcess = GameObject.FindGameObjectWithTag("Volume");
        PostProcess.GetComponent<PostProcessEffects>().CharacterDeath();

        yield return new WaitUntil(() => isDying);

        playerMovement.canMove = false;
        objectCollider.enabled = false;
        bodycol.enabled = false;

        yield return new WaitUntil(() => !isDying);

        playerMovement.canMove = true;
        objectCollider.enabled = true;
        bodycol.enabled = true;
        onPlayerDeath?.Invoke();

    }
    public void LoadStats()
    {
        maxHealth = stats.getMaxHealth();

        UpdateHealthBar();
    }

    public void HealByPercentage(float percentage)
    {
        currentHealth += (int)(maxHealth * percentage);
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
