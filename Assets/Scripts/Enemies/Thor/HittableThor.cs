using Enemies.Thor;
using UnityEngine;

public class HittableThor : HittableObject
{
    private ThorScript thorScript;
    //[SerializeField] private Slider thorHealthBar;
    [SerializeField] private GameObject healthBar;
    private PlayerHealthBar thorHealthbar;
    
    public delegate void ThorDeathDelegate();
    public static ThorDeathDelegate onThorDeath;
    protected override void Start()
    {
        base.Start();
        thorScript = GetComponent<ThorScript>();
        thorHealthbar = healthBar.GetComponent<PlayerHealthBar>();
    }

    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier, GameObject damageSource, bool heavy)
    {
        base.GetHit(damage, damageSourcePosition, knockbackMultiplier, damageSource, heavy);
    }

    protected override void TakeDamage(int damage, GameObject damageSource)
    {
        base.TakeDamage(damage, damageSource);
        UpdateHealthBar();
        if(currentHealth <= (2f/3)*maxHealth)
            thorScript.SetPhase(1);
        if(currentHealth <= (1f/3)*maxHealth)
            thorScript.SetPhase(2);
    }

    protected override void Die(GameObject damageSource)
    {
        onThorDeath?.Invoke();
        base.Die(damageSource);
    }

    private void UpdateHealthBar()
    {
        thorHealthbar.UpdateHealthBar(currentHealth, maxHealth);
    }
}
