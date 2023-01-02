using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private float healthPoints = 6f;


        public void TakeDamage(float damageAmount)
        {
            healthPoints -= damageAmount;

            if(healthPoints <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}