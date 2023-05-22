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

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    public delegate void PlayerDeathDelegate();
    public static PlayerDeathDelegate onPlayerDeath;

    public bool isAlreadyDestroyed { private get; set; } = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        if (scene.name == "HUB")
        {
            currentHealth = maxHealth;
        }

        if (scene.name == "Valhalla")
        {
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
        Vector3 t = transform.localScale;
        transform.localScale *= 1.05f;
        spriteRenderer.color = new Color(255,0,0,255);
        yield return new WaitForSeconds(.1f);
        transform.localScale = t;
        spriteRenderer.color = new Color(255,255,255,255);
    }

    protected override void Die(GameObject damageSource)
    {
        transform.position = new Vector3(-4.5f, -1.5f, 0);

        onPlayerDeath?.Invoke();

        //base.Die(); TODO talk about correct resetting of player on death
        //gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
