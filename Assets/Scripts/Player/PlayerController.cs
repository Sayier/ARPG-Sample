using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;

        private CharacterController characterController;
        

        #region Variables: Movement
        private Vector2 playerMovement;
        private bool isRunning;
        #endregion

        #region Variables: Attack
        private const float MIN_COMBO_DELAY_TIME = 0.1f;
        private const int MAX_COMBO_STEPS = 2;
        private int currentComboStep;
        private Coroutine comboAttackResetCoroutine;
        private bool isAttacking;
        #endregion

        #region Variables: Inputs
        private DefaultInputActions inputActions;
        private InputAction moveAction;
        private InputAction attackAction;
        #endregion

        #region Variables: Animation
        private int animationRunningParameterHash;
        private int animationAttackComboParameterHash;
        [SerializeField] private Animator animationController;
        private Transform animationTransform;
        #endregion

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputActions = new DefaultInputActions();

            isRunning = false;
            animationRunningParameterHash = Animator.StringToHash("Running");   //Hash of the running variable

            isAttacking = false;
            currentComboStep = 0;
            comboAttackResetCoroutine = null;
            animationAttackComboParameterHash = Animator.StringToHash("AttackComboStep");

            animationTransform = animationController.transform;
        }

        private void OnEnable()
        {
            moveAction = inputActions.Player.Move;
            moveAction.Enable();

            attackAction = inputActions.Player.Attack;
            attackAction.performed += OnAttackAction;
            attackAction.Enable();
        }


        private void OnDisable()
        {
            moveAction.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            if (isAttacking)
            {
                return;
            }
            playerMovement = moveAction.ReadValue<Vector2>();   //Read Input from InputMapping
            //Debug.Log("X: " + playerMovement.x.ToString() + ", Y: " + playerMovement.y.ToString());

            if (playerMovement.sqrMagnitude > 0.01f)    //Checks for movement
            {
                if(isRunning != true)   //Enable Running state if needed
                {
                    isRunning = true;
                    animationController.SetBool(animationRunningParameterHash, true);
                }

                Vector3 playerMovementToVector3 = new Vector3(playerMovement.x, 0f, playerMovement.y);  

                animationTransform.rotation = Quaternion.LookRotation(-playerMovementToVector3, Vector3.up); //Forward is reversed because of how the animation was built

                characterController.Move(playerMovementToVector3 * playerData.moveSpeed * Time.deltaTime);
            }
            else if(isRunning != false) //Disable Running state if needed
            {
                isRunning = false;
                animationController.SetBool(animationRunningParameterHash, false);
            }

            
        }

        private void OnAttackAction(InputAction.CallbackContext obj)
        {
            isAttacking = true;

            if (currentComboStep == MAX_COMBO_STEPS)
            {
                return;
            }

            float animationTime = animationController.GetCurrentAnimatorStateInfo(0).normalizedTime;

            //Debug.Log("AnimationTime is " + animationTime);
            //Debug.Log("Current Combo Step is " + currentComboStep);

            if ( currentComboStep == 0 || (animationTime >= MIN_COMBO_DELAY_TIME && animationTime <= 0.9f))
            {
                if(comboAttackResetCoroutine != null)
                {
                    StopCoroutine(comboAttackResetCoroutine);
                }

                currentComboStep++;
                animationController.SetBool(animationRunningParameterHash, false);
                animationController.SetInteger(animationAttackComboParameterHash, currentComboStep);

                comboAttackResetCoroutine = StartCoroutine(ResettingAttackCombo());
            }
        }

        private IEnumerator ResettingAttackCombo()
        {

            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(animationController.GetAnimatorTransitionInfo(0).duration);
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => animationController.GetAnimatorTransitionInfo(0).normalizedTime >= 0.9f);

            currentComboStep = 0;
            //Debug.Log("Combo set to 0");

            animationController.SetInteger(animationAttackComboParameterHash, currentComboStep);
            
            playerMovement = moveAction.ReadValue<Vector2>();
            if (playerMovement.sqrMagnitude > 0.01f && isRunning)
            {
                animationController.SetBool(animationRunningParameterHash, true);
            }
            
            isAttacking = false;
        }
    }
}