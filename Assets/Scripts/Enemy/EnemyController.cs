using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyData;

        private float currentHealthPoints;

        public void Awake()
        {
            currentHealthPoints = enemyData.healthPoints;
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealthPoints -= damageAmount;

            if(currentHealthPoints <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}