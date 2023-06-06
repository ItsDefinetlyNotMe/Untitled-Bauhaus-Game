using System.Collections;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

namespace Enemies
{
    public class HittableEnemy : HittableObject
    {
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;

        public GameObject HitSound;
        public GameObject DeathHitSound;

        private EnemyMovement enemyMovement;
        //called on deathanimation
        public bool dying;
        
        public delegate void EnemyDeathDelegate();
        public static EnemyDeathDelegate onEnemyDeath;
        protected override void Start()
        {
            base.Start();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            enemyMovement = GetComponent<EnemyMovement>();
        }
        public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier,GameObject damageSource)
        {
            //Debug.Log("Ohh no it hurts, Oh no pls stop");
            //visual Feedback
            StartCoroutine(HitFeedback());
            HitSound.GetComponent<RandomSound>().PlayRandom1();

            base.GetHit(damage,damageSourcePosition, knockbackMultiplier,damageSource);
        
            //knockback
            float sizeMultiplier;
            //knockback enemy
            if(size == 0){//imoveable object
                sizeMultiplier = 0;
            }else{
                sizeMultiplier = 1 / size;
            }
            Vector2 knockbackDirection = new Vector2(transform.position.x,transform.position.y) - damageSourcePosition;
            rb.velocity = knockbackDirection.normalized * (sizeMultiplier * knockbackMultiplier);

        }
        private IEnumerator HitFeedback(){
            Vector3 t = transform.localScale;
            transform.localScale *= 1.05f;
            spriteRenderer.color = new Color(255,0,0,255);
            yield return new WaitForSeconds(.1f);
            transform.localScale = t;
            spriteRenderer.color = new Color(255,255,255,255);
        }

        protected override void Die(GameObject damageSource)
        {
            DeathHitSound.GetComponent<RandomSound>().PlayRandom1();
            onEnemyDeath?.Invoke();
            if(enemyMovement != null)
                enemyMovement.OnDeath();
            StartCoroutine(DeathAnim());
            base.Die(damageSource);
            //TODO: Disable, play death animation and then destroy gameObject
        }

        private IEnumerator DeathAnim()
        {
            if (animator != null)
            {
                animator.SetTrigger("OnDeath");
                yield return new WaitUntil(() => dying);
                yield return new WaitUntil(() => !dying);
            }

            Destroy(gameObject);
        }
    }
}
