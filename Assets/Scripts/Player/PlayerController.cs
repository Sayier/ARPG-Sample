using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private CharacterController characterController;
        [SerializeField]
        private Animator animationController;
        private Transform animationTransform;

        #region Variables: Movement
        private const float MOVE_SPEED = 6f;
        private Vector2 playerMovement;
        private bool isRunning;
        #endregion

        #region Variables: Inputs
        private DefaultInputActions inputActions;
        private InputAction moveAction;
        #endregion

        #region Variables: Animation
        private int animationRunningParameterHash;
        #endregion

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputActions = new DefaultInputActions();

            isRunning = false;
            animationRunningParameterHash = Animator.StringToHash("Running");   //Hash of the running variable

            animationTransform = animationController.transform;
        }

        private void OnEnable()
        {
            moveAction = inputActions.Player.Move;
            moveAction.Enable();
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

                characterController.Move(playerMovementToVector3 * MOVE_SPEED * Time.deltaTime);
            }
            else if(isRunning != false) //Disable Running state if needed
            {
                isRunning = false;
                animationController.SetBool(animationRunningParameterHash, false);
            }

            
        }
    }
}