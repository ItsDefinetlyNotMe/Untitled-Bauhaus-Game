using Pathfinding;
using UnityEngine;

namespace Enemies
{
    public class EnemyScript : MonoBehaviour
    {
        [Header("Pathing")]
        private Path path;
        private Seeker seeker;
        private int currentWaypoint;
        private readonly float reachedWayPointDistance = .4f;
        private bool reachedEndofPath;
        
        [Header("Stats")]
        [SerializeField]float movementSpeed = 1f;
        
        [Header("Physics")]
        Rigidbody2D rb;
        
        [Header("Target")]
        Vector3 lastTargetPosition;
        [SerializeField] Transform target;
        public bool targeting;

        void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();       
            seeker.StartPath(rb.position,target.position,OnPathComplete); 
            InvokeRepeating(nameof(UpdatePath),0f,.5f);
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
}
