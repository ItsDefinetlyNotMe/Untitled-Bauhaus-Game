using System;
using System.Collections;
using System.Drawing;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

// ReSharper disable Unity.InefficientPropertyAccess

namespace Enemies
{
    public class HittableEnemy : HittableObject
    {
        private float lastTimeTouchedFloor = 0;
        [SerializeField]
        private GameObject floatingDamage;
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;
        protected Rigidbody2D rb;

        public GameObject HitSound;
        public GameObject DeathHitSound;
        [SerializeField] public Slider healthBar;

        protected EnemyMovement enemyMovement;
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

            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }

        private void Update()
        {
            if (lastTimeTouchedFloor > 10f)
            {
                print("OUCH");
                TakeDamage(3,gameObject);
            }

            lastTimeTouchedFloor += Time.deltaTime;
        }

        public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier,GameObject damageSource,bool heavy)
        {
            
            //visual Feedback
            StartCoroutine(HitFeedback());
            HitSound.GetComponent<RandomSound>().PlayRandom1();
            
            base.GetHit(damage,damageSourcePosition, knockbackMultiplier,damageSource,heavy);
            
            GameObject points = Instantiate(floatingDamage, transform.position, quaternion.identity) as GameObject;
            points.transform.GetChild(0).GetComponent<TMP_Text>().text =  damage.ToString();
            byte colorgradient = (byte)(255 - 255 * (damage / (maxHealth / 2.0f))); 
            points.transform.GetChild(0).GetComponent<TMP_Text>().faceColor = new Color32(255,colorgradient,colorgradient,255);

            if (healthBar.value == maxHealth)
                healthBar.gameObject.SetActive(true);

            healthBar.value = currentHealth;
            if(!isDead)
                Knockback(0.3f,damageSourcePosition,knockbackMultiplier);//TODO duration
        }

        protected virtual IEnumerator HitFeedback(){
            Vector3 t = transform.localScale;
            transform.localScale *= 1.2f;
            if (spriteRenderer.enabled)
                spriteRenderer.color = new Color(255,0,0,255);
            yield return new WaitForSeconds(.1f);
            transform.localScale = t;
            if(spriteRenderer.enabled)
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
                animator.SetTrigger(OnDeath);
                yield return new WaitUntil(() => dying);
                yield return new WaitUntil(() => !dying);
            }
            Destroy(gameObject);
        }

        public void Knockback(float duration,Vector2 damageSourcePosition,float knockbackMultiplier)
        {
            
            if(size==0)
                return;
            float sizeMultiplier = 1 / size;
            float knockbackStrength = knockbackMultiplier * sizeMultiplier;
            if(enemyMovement != null)
                enemyMovement.Knockback(duration,damageSourcePosition,knockbackStrength);
            else
            {
                Debug.Log("no Enemy Movementscript");
            }
        }

        public void TouchingFloor()
        {
            lastTimeTouchedFloor = 0;
        }

    }
}
