using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using Color = UnityEngine.Color;

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
        private static readonly int OnDeath = Animator.StringToHash("OnDeath");

        protected override void Start()
        {
            base.Start();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            enemyMovement = GetComponent<EnemyMovement>();
        }
        public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier,GameObject damageSource,bool heavy)
        {
            
            //visual Feedback
            StartCoroutine(HitFeedback());
            //HitSound.GetComponent<RandomSound>().PlayRandom1();

            base.GetHit(damage,damageSourcePosition, knockbackMultiplier,damageSource,heavy);
        
            Knockback(0.05f,damageSourcePosition,knockbackMultiplier);//TODO duration
        }
        private IEnumerator HitFeedback(){
            Vector3 t = transform.localScale;
            transform.localScale *= 1.05f;
            if (spriteRenderer.enabled)
                spriteRenderer.color = new Color(255,0,0,255);
            yield return new WaitForSeconds(.1f);
            transform.localScale = t;
            if(spriteRenderer.enabled)
                spriteRenderer.color = new Color(255,255,255,255);
        }

        protected override void Die(GameObject damageSource)
        {
            //DeathHitSound.GetComponent<RandomSound>().PlayRandom1();
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
                animator.SetTrigger(OnDeath);
                yield return new WaitUntil(() => dying);
                yield return new WaitUntil(() => !dying);
            }

            Destroy(gameObject);
        }

        public void Knockback(float duration,Vector2 damageSourcePosition,float knockbackMultiplier)
        {
            
            if(size==0)
            {
                return;
            }
            float sizeMultiplier = 1 / size;
            float knockbackStrength = knockbackMultiplier * sizeMultiplier;
            if(enemyMovement != null)
                enemyMovement.Knockback(duration,damageSourcePosition,knockbackStrength);
            else
            {
                Debug.Log("no Enemy Movementscript");
            }
        }

    }
}
