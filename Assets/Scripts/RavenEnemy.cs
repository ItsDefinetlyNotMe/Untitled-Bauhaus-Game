using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class RavenEnemy : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Transform target;
    [SerializeField] GameObject playerhitscript;
    [SerializeField] Transform enemyTransform;
    float movementspeed = 1.5f;
    public float nextWayPointDistance = 3f;
    Vector3 lastTargetPosition = new Vector3(0f, 0f, 0f);
    Path path;
    int currentWaypoint = 0;
    float reachedWayPointDistance = .4f;
    bool reachedEndofPath = false;
    Seeker seeker;
    float attackRange = 0.5f;
    float vel;
    public GameObject Rave;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        seeker.StartPath(rb.position, target.position, OnPathComplete);
        InvokeRepeating("UpdatePath", 0f, .5f);
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void UpdatePath()
    {
        if (lastTargetPosition != target.position)//only if player moved
        {
            lastTargetPosition = target.position;
            if (seeker.IsDone())
                seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    void FixedUpdate()
    {
        vel = rb.velocity.magnitude;
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log(path.vectorPath.Count + ":" + currentWaypoint);
            reachedEndofPath = true;
            return;
        }
        else
        {
            reachedEndofPath = false;
        }
        if (Vector2.Distance(rb.position, target.position) <= attackRange * 2f / 3f)
        {

            reachedEndofPath = true;
        }
        if (!reachedEndofPath)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            direction.x = 0; 
            Vector2 force = direction * movementspeed;
            rb.velocity = force;
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < reachedWayPointDistance)
                ++currentWaypoint;
        }

    }

    public void Update()
    {
        if (target.position.x < Rave.transform.position.x)
        {
            Rave.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            Rave.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Attack(HitablePlayer hp)
    {
        hp.GetHit(20, transform.position, 10f);
    }
}