using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Transform target;
    float movementspeed = 1f;
    public float nextWayPointDistance = 3f;
    Vector3 lastTargetPosition = new Vector3(0f,0f,0f);
    Path path;
    int currentWaypoint = 0;
    float reachedWayPointDistance = .4f;
    bool reachedEndofPath = false;
    Seeker seeker;
    float attackRange = 1.3f;
    float vel;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();       
        seeker.StartPath(rb.position,target.position,OnPathComplete); 
        InvokeRepeating("UpdatePath",0f,.5f);
    }
    void UpdatePath()
    {
        if(lastTargetPosition != target.position)//only if player moved
        {
            lastTargetPosition = target.position;
            if(seeker.IsDone())
                seeker.StartPath(rb.position,target.position,OnPathComplete);
        }
    }
    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    void FixedUpdate()
    {
        vel = rb.velocity.magnitude;
        if(path == null)
            return;
        if(currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log(path.vectorPath.Count + ":"+ currentWaypoint);
            reachedEndofPath = true;
            return;
        }else
        {
            reachedEndofPath = false;
        }
        if(Vector2.Distance(rb.position,target.position) <= attackRange * 2f/3f){
            Attack();
            //Debug.Log("ATTACKRANGE");
            reachedEndofPath = true;
        }
        if(!reachedEndofPath)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * movementspeed;
            rb.velocity = force;
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if(distance < reachedWayPointDistance)
                ++currentWaypoint;
        }
    }
    private void Attack()
    {
        //Debug.Log("AttackPlayer");
        //do something
    }
}
