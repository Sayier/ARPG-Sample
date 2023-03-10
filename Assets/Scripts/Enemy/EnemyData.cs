using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
    public class EnemyData : ScriptableObject
    {
        public float healthPoints;
        public float moveSpeed;

        public float fovRadius;
        
        public float attackRange;
        public float attackRate;
        public float attackDamage;
    }
}