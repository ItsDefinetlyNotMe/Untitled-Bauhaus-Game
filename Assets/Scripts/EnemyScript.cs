using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    Rigidbody2D rb;
    //AI 
    [SerializeField] private Transform target;
    private float baseMovementspeed = 1f;
    private float nextWaypointDistance = .2f;
    int currentWaypoint = 0;
    private bool isThere = false;
    Path path;
    Seeker seeker;
    WeaponScript weaponScript;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath",0f,.5f);
    }
    void UpdatePath()
    {
        if(seeker.IsDone())
            seeker.StartPath(rb.position, target.position,OnPathComplete);        
            
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
        if(path == null)
            return;
        if(currentWaypoint >= path.vectorPath.Count)
        {
            isThere = true;
        }else{
            isThere = false;
        }
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = rb.velocity.normalized  + (direction * baseMovementspeed).normalized;
        rb.velocity = force.normalized;
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
            ++currentWaypoint;
    }
    //pathfinding etc general behaviour
}
