using System;
using System.Reflection;
using Enemies;
using UnityEngine;

namespace Test
{
    public class TestScript : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private GameObject victim;
        [SerializeField] private Vector2 position;
        [Header("Hit")]
        [SerializeField] private int damage;
        [SerializeField] private bool heavy;
        [Header("CC")]
        [SerializeField] private float duration;
        [SerializeField] private float knockback;
        //[Header("Camera")] 
        private GameObject camPos;

        private void Start()
        {
            camPos = GameObject.Find("Camerafocus");
        }

        private void Update()
        {
            if (camPos != null && victim != null)
                camPos.transform.position = victim.transform.position;
        }

        public void DamageVictim()
        {
            HittableObject hit = GetVictim();
            if(hit !=null)
                hit.GetHit(damage,position,knockback,gameObject,heavy);
            else
            {
                Debug.Log("No Enemy Given");
            }
        }

        public void Knockback()
        {
            var hit = GetVictim();
            try
            {
                HittableEnemy en = (HittableEnemy)hit;
                en.Knockback(duration, position, knockback);
            }
            catch (Exception e)
            {
                Debug.Log("Not an enemy. It has no knockbackfunction:" + e.Message);
            }
        }

        public void Stun()
        {
            var en = victim.GetComponent<EnemyMovement>();
            MethodInfo methodInfo = typeof(EnemyMovement).GetMethod("Stun", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] args = { duration };
            if(methodInfo != null)
                methodInfo.Invoke(en,args);
        }

        public void StopTargeting()
        {
            var en = victim.GetComponent<EnemyMovement>();
            MethodInfo methodInfo = typeof(EnemyMovement).GetMethod("StopTargeting", BindingFlags.NonPublic | BindingFlags.Instance);
            //object[] args = new object[] { duration, position, knockback };
            if(methodInfo != null)
                methodInfo.Invoke(en,null );
        }

        public void StartTargeting()
        {
            if(victim == null)
                return;
            var en = victim.GetComponent<EnemyMovement>();
            MethodInfo methodInfo = typeof(EnemyMovement).GetMethod("StartTargeting", BindingFlags.NonPublic | BindingFlags.Instance);
            //object[] args = new object[] { duration, position, knockback };
            if(methodInfo != null)
                methodInfo.Invoke(en,null );
        }
        /*
     * MethodInfo methodInfo = typeof(EnemyMovement).GetMethod("StopTargeting", BindingFlags.NonPublic | BindingFlags.Instance);
        //object[] args = new object[] { duration, position, knockback };
        if(methodInfo != null)
            methodInfo.Invoke(this,null );
     */

        private HittableObject GetVictim()
        {
            return victim.GetComponent<HittableObject>();
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(position,.3f);
        }
    }
}
