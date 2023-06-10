using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable Unity.InefficientPropertyAccess

public class HitablePlayer : HittableObject
{
    private GameObject healthBar;

    private Slider healthSlider;
    private TMP_Text healthText;

    private Vector3 scale;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;
    private PlayerStats stats;
    public delegate void PlayerDeathDelegate();
    public static PlayerDeathDelegate onPlayerDeath;

    private Animator animator;
    public bool isAlreadyDestroyed { private get; set; } = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void Start()
    {
        base.Start();
        scale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();

        maxHealth = stats.getMaxHealth();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }
    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier,GameObject damageSource)
    {
        //visual Feedback
        StartCoroutine(HitFeedback());
        CameraShake.Instance.ShakeCamera(0.5f,.7f,true);
        
        base.GetHit(damage,damageSourcePosition, knockbackMultiplier,damageSource);
        
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

            healthSlider = healthBar.GetComponent<Slider>();
            healthText = healthBar.GetComponentInChildren<TMP_Text>();

            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
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
        StartCoroutine(OnPlayerDeath());
        
        //base.Die(); TODO talk about correct resetting of player on death
        //gameObject.SetActive(false);
    }

    IEnumerator OnPlayerDeath()
    {
        healthBar.SetActive(false);
        animator.SetTrigger("onDeath");

        yield return new WaitUntil(() => isDying);

        playerMovement.canMove = false;
        objectCollider.enabled = false;

        yield return new WaitUntil(() => !isDying);

        playerMovement.canMove = true;
        objectCollider.enabled = true;
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
