using Pathfinding;
using UnityEngine;

namespace Enemies
{
    public abstract class EnemyMovement : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] protected Transform target;
        protected Rigidbody2D rb;
        protected Collider2D col;

        [Header("Movement")]
        public float movementSpeed = 1f;
        protected bool isStunned;
        private float stunnedTimeStamp;
    
        [Header("Pathfinding")]
        Path path;
        Seeker seeker;
        int currentWaypoint;
        public float nextWayPointDistance = 3f;
        readonly float reachedWayPointDistance = .4f;
        bool reachedEndofPath;
        private Vector2 direction = new Vector2(0f, 0f);
        private Vector2 lastDirection = new Vector2(0f, 0f);

        protected Vector2 feetPositionOffset;
    
        [Header("Target")]
        protected bool targeting;
        /// <summary> To be called on Start, getting basic Components </summary>
        protected virtual void StartUp()
        {
            col = GetComponent<Collider2D>();
            target = (GameObject.FindGameObjectsWithTag("Target"))[0].transform;
            
            if (target == null)
            {
                target = transform;
                InvokeRepeating(nameof(FindPlayer),0.1f,0.3f);
            }

            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            seeker.StartPath(rb.position+feetPositionOffset,target.position,OnPathComplete); 
            InvokeRepeating(nameof(UpdatePath),0f,.5f);
        }
        void UpdatePath()
        {
            if(targeting)
            {
                if(seeker.IsDone())
                    seeker.StartPath(rb.position+feetPositionOffset,target.position,OnPathComplete);
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
        virtual protected void FixedUpdate()
        {
           // print(isStunned+ ":" + path!=null + ":" + (currentWaypoint >= path.vectorPath.Count) + targeting);
            if (isStunned)
            {
                if (stunnedTimeStamp > Time.time)
                {
                    return;
                }
                if(ShouldTarget())
                    StartTargeting();   
                isStunned = false;
            }
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
                if(direction.magnitude > 0.1f)
                    lastDirection = direction;
                direction = ((Vector2)path.vectorPath[currentWaypoint] - (rb.position+feetPositionOffset)).normalized;
                Vector2 force = direction * movementSpeed;
//                print(rb.velocity + ":base" );
                rb.velocity = force;
                float distance = Vector2.Distance(rb.position+feetPositionOffset, path.vectorPath[currentWaypoint]);
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

        protected void FindPlayer()
        {
            Transform player = (GameObject.FindGameObjectsWithTag("Target"))[0].transform;
            if (player != null)
                target = player;
            CancelInvoke(nameof(FindPlayer));
        }
        public virtual void OnDeath()
        {
            movementSpeed = 0;
            rb.velocity = new Vector3(0f,0f,0f);
        }

        protected virtual Vector2 GetDirection()
        {
            if (direction.magnitude > 0.1f)
                return direction; 
            return lastDirection;
            //For some reason the rb stops every now and then
        }
        
        protected virtual void IndefStun()
        {
            isStunned = true;
            StopTargeting();
            rb.velocity = Vector2.zero;
        }
        protected virtual void UnStun()
        {
            isStunned = false;
            StartTargeting();
        }
        protected virtual void Stun(float duration)
        {
            isStunned = true;
            StopTargeting();
            rb.velocity = Vector2.zero;
            stunnedTimeStamp = Time.time + duration;
        }

        public virtual void Knockback(float duration,Vector2 damageSourcePosition,float knockbackStrength)
        {
            Stun(duration);
            //knockback enemy
            Vector2 knockbackDirection = new Vector2(transform.position.x,transform.position.y) - damageSourcePosition;
            //Debug.Log( knockbackDirection.normalized * knockbackStrength);
            rb.AddForce( knockbackDirection.normalized * knockbackStrength);
        }

        protected virtual bool ShouldTarget()
        {
            return true;
        } 
    }
}
