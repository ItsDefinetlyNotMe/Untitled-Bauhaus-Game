using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Transform target;
    [SerializeField] GameObject playerHittScript;
    float movementSpeed = 1f;
    public float nextWayPointDistance = 3f;
    Vector3 lastTargetPosition = new Vector3(0f,0f,0f);
    Path path;
    int currentWaypoint = 0;
    float reachedWayPointDistance = .4f;
    bool reachedEndofPath = false;
    Seeker seeker;
    float vel;
    public bool targeting = false;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();       
        seeker.StartPath(rb.position,target.position,OnPathComplete); 
        InvokeRepeating("UpdatePath",0f,.5f);
    }
    void UpdatePath()
    {
        if(lastTargetPosition != target.position && targeting)//only if player moved and we are currently targeting the player
        {
            lastTargetPosition = target.position;
            if(seeker.IsDone())
                seeker.StartPath(rb.position,target.position,OnPathComplete);
        }
    }
    void OnPathComplete(Path newPath)
    {
        if(!newPath.error)
        {
            path = newPath;
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
            reachedEndofPath = true;
            return;
        }else
        {
            reachedEndofPath = false;
        }
        if(!reachedEndofPath && targeting)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * movementSpeed;
            rb.velocity = force;
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if(distance < reachedWayPointDistance)
                ++currentWaypoint;
        }
    }
}
