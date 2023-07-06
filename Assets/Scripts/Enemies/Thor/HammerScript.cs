using Unity.Mathematics;
using UnityEngine;

public class HammerScript : MonoBehaviour
{
    private Vector3 flyDirection;
    private Collider2D col;
    private float speed = 10f;
    private int damage = 10;
    
    private Rigidbody2D rb;

    [SerializeField] private GameObject lightning;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(col,GameObject.FindGameObjectWithTag("Thor").GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = flyDirection * speed;
    }

    public void SetDirection(Vector2 direction)
    {
        flyDirection = direction;
        int invert = 0;
        if (direction.x > 0)
            invert = 360;
            transform.Rotate(  Vector3.forward *Mathf.Abs(invert - Vector2.Angle(Vector3.up, flyDirection)));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 position = transform.position;
        //KILL HIM
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponent<HitablePlayer>().GetHit(damage,transform.position,20000f,gameObject,false);
            //Spawn lightning
            position = other.transform.position;
        }
        CameraShake.Instance.ShakeCamera(0.3f,.7f,false);

        var light = Instantiate(lightning, position, quaternion.identity);
        light.transform.localScale *= 2; 
        Destroy(gameObject);
    }
}
