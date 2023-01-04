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
        private Transform target;
        private Vector3 startPosition;
        #endregion

        #region Variables: General
        private float currentHealthPoints;
        private float attackDelay;
        #endregion

        #region Variables: Animation
        [SerializeField] private Animator animationController;
        private int animationRunningParameterHash;
        private int animationAttackingParameterHash;
        #endregion

        private NavMeshAgent navAgent;

        public void Awake()
        {
            GetComponent<SphereCollider>().radius = enemyData.fovRadius;
            navAgent = GetComponent<NavMeshAgent>();
            currentHealthPoints = enemyData.healthPoints;

            startPosition = transform.position;

            currentState = EnemyState.Idle;

            animationRunningParameterHash = Animator.StringToHash("Running");
            animationAttackingParameterHash = Animator.StringToHash("Attacking");
        }

        public void Update()
        {
            //Debug.Log(currentState.ToString());

            if (currentState == EnemyState.Move)
            {
                Move();
            }
            else if (currentState == EnemyState.Return)
            {
                Return();
            }
            else if (currentState == EnemyState.Attack)
            {
                Attack();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = other.transform;
                navAgent.destination = target.position;
                transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);

                currentState = EnemyState.Move;

                animationController.SetBool(animationRunningParameterHash, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = null;
                navAgent.destination = startPosition;
                navAgent.velocity = Vector3.zero;
                transform.rotation = Quaternion.LookRotation(startPosition - transform.position, Vector3.up);

                currentState = EnemyState.Return;

                animationController.SetBool(animationRunningParameterHash, true);
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
                animationController.SetBool(animationRunningParameterHash, true);
                navAgent.destination = target.position;
            }
        }

        public void Return()
        {
            if(navAgent.remainingDistance < 0.1f)
            {
                currentState = EnemyState.Idle;
                StopMovement();
                target = null;
            }
        }

        private void Attack()
        {
            if ((target.position - transform.position).magnitude > enemyData.attackRange)
            {
                currentState = EnemyState.Move;
                animationController.ResetTrigger(animationAttackingParameterHash);
                navAgent.destination = target.position;
                return;
            }

            transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);

            if(attackDelay >= enemyData.attackRate)
            {
                //Do the attack
                animationController.SetTrigger(animationAttackingParameterHash);

                attackDelay = 0;
            }
            else
            {
                attackDelay += Time.deltaTime;
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
            animationController.SetBool(animationRunningParameterHash, false);
        }
    }
}