using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class HittableFireplace : HittableObject
{
    private Animator animator;
    SpriteRenderer spriteRenderer;
    private int stage;
    private Transform layerRoot;
    [SerializeField] private Light2D light2D;
    public bool dying;

    public float speed = 4f;
    public float noiseScale = 0.5f;
    private float noiseOffset = 0.6f;
    private float maxValue = 0.6f;

    protected override void Start()
    {
        base.Start();
        layerRoot = transform.Find("LayerRoot");
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        noiseOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float noiseValue = Mathf.PerlinNoise(Time.time * speed, noiseOffset) * noiseScale;

        noiseValue = Mathf.Min(0.8f, noiseValue);
        noiseValue = Mathf.Max(maxValue,noiseValue);
        light2D.intensity = noiseValue;
    }

    protected override void TakeDamage(int damage,GameObject damageSource)
    {
        base.TakeDamage(damage,damageSource);
        if (currentHealth <= maxHealth * 1f / 3f && currentHealth > 0 && stage == 0)
        {
            stage = 1;
            //priteRenderer.sprite = destroyedSecondSprite;
            animator.SetTrigger("nextStage");
        }
        else if (currentHealth <= maxHealth * 2f / 3f && currentHealth > 0)
        {
            stage = 2;
            animator.SetTrigger("nextStage");

        }

    }
    protected override void Die(GameObject damageSource)
    {
        //change sprite
        //spriteRenderer.sprite = destroyedSprite;
        StartCoroutine(LightDown());
        //light2D.intensity = 0f;
        //layerRoot.position += Vector3.up * 0.3f;
        base.Die(damageSource);
    }

    IEnumerator LightDown()
    {
        animator.SetTrigger("nextStage");
        yield return new WaitWhile(() => !dying);
        float startintensity = light2D.intensity;
        float duration = 0.02f; 
        float t = 0f;
        noiseScale = 0f;
       
        while (dying) 
        {
            t += Time.deltaTime / duration;
            light2D.intensity = Mathf.Lerp(startintensity,0, t);
            Debug.Log(Mathf.Lerp(startintensity,0, t)); 
            yield return new WaitForSeconds(0.1f);
        }
    }
    //protected override void TakeDamage(int damage)
    //{
    //    base.TakeDamage(damage);
    //}
}
