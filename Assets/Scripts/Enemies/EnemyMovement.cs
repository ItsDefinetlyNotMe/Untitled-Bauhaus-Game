using Pathfinding;
using UnityEngine;

namespace Enemies
{
    public abstract class EnemyMovement : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] protected Transform target;
        protected Rigidbody2D rb;
    
        [Header("Movement")]
        protected readonly float movementSpeed = 1f;
    
        [Header("Pathfinding")]
        Path path;
        Seeker seeker;
        int currentWaypoint;
        public float nextWayPointDistance = 3f;
        readonly float reachedWayPointDistance = .4f;
        bool reachedEndofPath;
    
        [Header("Target")]
        private bool targeting;
        /// <summary> To be called on Start, getting basic Components </summary>
        protected virtual void StartUp()
        {
            target = (GameObject.FindGameObjectsWithTag("Player"))[0].transform;
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            seeker.StartPath(rb.position,target.position,OnPathComplete); 
            InvokeRepeating(nameof(UpdatePath),0f,.5f);
        }
        void UpdatePath()
        {
            if(targeting)
            {
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

        protected void StartTargeting(){
            targeting = true;
        }

        protected void StopTargeting(){
            targeting = false;
        }
    }
}
