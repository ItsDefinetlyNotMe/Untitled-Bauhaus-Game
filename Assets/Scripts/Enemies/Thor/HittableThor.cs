using Cinemachine;
using Enemies.Thor;
using TestRandomWorldGeneration;
using UnityEngine;

public class HittableThor : HittableObject
{
    private ThorScript thorScript;
    //[SerializeField] private Slider thorHealthBar;
    [SerializeField] private GameObject healthBar;
    private PlayerHealthBar thorHealthbar;
    
    public delegate void ThorDeathDelegate();
    public static ThorDeathDelegate onThorDeath;
    public GameObject HitSound;
    protected override void Start()
    {
        base.Start();
        thorScript = GetComponent<ThorScript>();
        thorHealthbar = healthBar.GetComponent<PlayerHealthBar>();
    }

    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier, GameObject damageSource, bool heavy)
    {

        base.GetHit(damage, damageSourcePosition, knockbackMultiplier, damageSource, heavy);
        HitSound.GetComponent<RandomSound>().PlayRandom1();
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
        GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 2f;
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<HitablePlayer>().isInvulnerable = true;

        thorScript.isDead = true;
        thorScript.SetPhase(3);
        thorScript.StopLasers();

        GetComponent<Animator>().Play("ThorDeath");

        GameObject.Find("/ThorDeathEvent").transform.GetChild(0).gameObject.SetActive(true);

        Door door = GameObject.Find("DoorToHUB").GetComponent<Door>();
        door.GetComponent<Animator>().SetTrigger("open");
        door.GetComponent<Door>().ActivateDoor();

        base.Die(damageSource);
    }

    private void UpdateHealthBar()
    {
        thorHealthbar.UpdateHealthBar(currentHealth, maxHealth);
    }
}
