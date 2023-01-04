using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(SphereCollider))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyData;

        private enum EnemyState
        {
            Idle,
            Move,
            Attack,
            Return,
            Die
        }

        #region Variables: FSM
        private EnemyState currentState;
        private Transform moveTarget;
        private Vector3 startPosition;
        #endregion

        #region Variables: General
        private float currentHealthPoints;
        #endregion

        private NavMeshAgent navAgent;

        public void Awake()
        {
            GetComponent<SphereCollider>().radius = enemyData.fovRadius;
            navAgent = GetComponent<NavMeshAgent>();
            currentHealthPoints = enemyData.healthPoints;

            startPosition = transform.position;

            currentState = EnemyState.Idle;
        }

        public void Update()
        {
            if (currentState == EnemyState.Move)
            {
                Move();
            }
            if (currentState == EnemyState.Return)
            {
                Return();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                moveTarget = other.transform;
                navAgent.destination = moveTarget.position;
                transform.rotation = Quaternion.LookRotation(moveTarget.position - transform.position, Vector3.up);

                currentState = EnemyState.Move;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                moveTarget = null;
                navAgent.destination = startPosition;
                navAgent.velocity = Vector3.zero;
                transform.rotation = Quaternion.LookRotation(startPosition - transform.position, Vector3.up);

                currentState = EnemyState.Return;
            }
        }

        private void Move()
        {
            if (navAgent.remainingDistance < enemyData.attackRange)
            {
                StopMovement();
                currentState = EnemyState.Attack;
            }
            else
            {
                navAgent.destination = moveTarget.position;
            }
        }

        public void Return()
        {
            if(navAgent.remainingDistance < 0.1f)
            {
                currentState = EnemyState.Idle;
                StopMovement();
                moveTarget = null;
            }
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealthPoints -= damageAmount;

            Debug.Log("Enemy Hit");

            if(currentHealthPoints <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void StopMovement()
        {
            navAgent.destination = transform.position;
            navAgent.velocity = Vector3.zero;
        }
    }
}