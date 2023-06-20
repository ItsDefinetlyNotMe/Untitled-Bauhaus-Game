using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 0.001f;
    [SerializeField] int damage = 10;

    private Rigidbody2D rb;
    public AudioSource ArrowSound;
    public GameObject ArrowHitSound;
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = -transform.up * speed;
        ArrowSound.Play();
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.name == "CrossbowStand(Clone)")
            return;

        HittableObject script  = hitInfo.GetComponent<HittableObject>();
        if(script == null)
        {
            ArrowSound.Stop();
            ArrowHitSound.GetComponent<RandomSound>().PlayRandom1();
            Destroy(gameObject);
            return;
        }
        script.GetHit(damage,transform.position + transform.up,1f,gameObject,false);
        Destroy(gameObject);
        
    }

    public void ArrowSoundEffect()
    {
        ArrowSound.Play();
    }
}
