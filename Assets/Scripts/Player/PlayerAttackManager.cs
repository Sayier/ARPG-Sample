using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAttackManager : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;

        private static readonly int ENEMY_LAYER = 1 << 6;

        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;

        public void PlayerHitLeft(int comboStep)
        {
            PlayerHit(comboStep, leftHand);
        }

        public void PlayerHitRight(int comboStep)
        {
            PlayerHit(comboStep, rightHand);
        }

        public void PlayerHit(int comboStep, Transform hand)
        {
            Collider[] closeEnemies = Physics.OverlapSphere(hand.position, playerData.attackRange, ENEMY_LAYER);

            foreach(Collider enemy in closeEnemies)
            {
                Enemy.EnemyController closeEnemy = enemy.GetComponent<Enemy.EnemyController>();
                if (closeEnemy != null)
                {
                    closeEnemy.TakeDamage(playerData.attackDamage);
                }
            }
        }
    }
}
