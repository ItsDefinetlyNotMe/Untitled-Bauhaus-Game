using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Image frontBar;
    [SerializeField] Image backBar;

    [SerializeField] TMP_Text healthText;

    private float health;
    private float maxHealth;

    private float lerpTimer;
    private float chipSpeed = 2f;

    public void UpdateHealthBar(int newHealth, int newMaxHealth)
    {
        health = newHealth;
        maxHealth = newMaxHealth;

        healthText.text = (int)health + " / " + maxHealth;

        lerpTimer = 0;
    }

    private void Update()
    {
        float frontFill = frontBar.fillAmount;
        float backFill = backBar.fillAmount;
        float healthFraction = health / maxHealth;

        if (backFill > healthFraction)
        {
            frontBar.fillAmount = healthFraction;
            backBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float lerpProgress = lerpTimer / chipSpeed;
            backBar.fillAmount = Mathf.Lerp(backFill, healthFraction, lerpProgress);
        }
        
        if (frontFill < healthFraction)
        {
            backBar.fillAmount = healthFraction;
            backBar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float lerpProgress = lerpTimer / chipSpeed;
            lerpProgress *= lerpProgress;
            frontBar.fillAmount = Mathf.Lerp(frontFill, backBar.fillAmount, lerpProgress);
        }
    }
}
