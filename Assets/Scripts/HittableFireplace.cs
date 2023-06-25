using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class HittableFireplace : HittableObject
{
    private Animator animator;
    SpriteRenderer spriteRenderer;
    private int stage;
    private Transform layerRoot;
    [SerializeField] private Light2D light2D;
    public bool dying;
    private bool dead;
    private int currentStage;

    public float speed = 4f;
    public float noiseScale = 0.1f;
    private float noiseOffset = 0.6f;
    private float baseLightintensity;
    public GameObject HitSound;
    public AudioSource FireSoundStop;
    private static readonly int NextStage = Animator.StringToHash("nextStage");
    private static readonly int OnDeath = Animator.StringToHash("OnDeath");


    protected override void Start()
    {
        base.Start();
        layerRoot = transform.Find("LayerRoot");
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        noiseOffset = Random.Range(0f, 100f);
        baseLightintensity = light2D.intensity;
    }

    private void Update()
    {
        if (!dead)
        {
            float noiseValue = Mathf.PerlinNoise(Time.time * speed, noiseOffset) * noiseScale;
            light2D.intensity = baseLightintensity + noiseValue - noiseScale / 2;
        }
    }

    protected override void TakeDamage(int damage,GameObject damageSource)
    {
        base.TakeDamage(damage,damageSource);
        if (currentHealth <= maxHealth * 1f / 3f && currentHealth > 0 && stage == 0)
        {
            stage = 1;
            //priteRenderer.sprite = destroyedSecondSprite;
            animator.SetTrigger(NextStage);
            currentStage++;
            HitSound.GetComponent<RandomSound>().PlayRandom1();
        }
        else if (currentHealth <= maxHealth * 2f / 3f && currentHealth > 0)
        {
            stage = 2;
            animator.SetTrigger(NextStage);
            currentStage++;
            HitSound.GetComponent<RandomSound>().PlayRandom1();
        }else if (currentHealth <= 0)
        {
            stage = 3;
            currentStage++;
        }

    }
    protected override void Die(GameObject damageSource)
    {
        HitSound.GetComponent<RandomSound>().PlayRandom2();
        FireSoundStop.Stop();
        StartCoroutine(LightDown());
        base.Die(damageSource);
    }

    IEnumerator LightDown()
    {
        dead = true;
        if(currentStage != stage)
            animator.SetTrigger(OnDeath);
        
        animator.SetTrigger(NextStage);
        
        yield return new WaitWhile(() => !dying);
        float startintensity = light2D.intensity;
        float duration = 0.1f; 
        float t = 0f;
        noiseScale = 0f;
       
        while (dying) 
        {
            t += Time.deltaTime / duration;
            light2D.intensity = Mathf.Lerp(startintensity,0, t);
            yield return new WaitForSeconds(0.1f);
        }

        light2D.intensity = 0f;
    }
}
