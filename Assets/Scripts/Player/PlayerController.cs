using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private CharacterController characterController;

        #region Variables: Movement
        private const float MOVE_SPEED = 5f;
        private Vector3 playerMovement;
        #endregion

        private float hortizontalMovement;
        private float verticalMovement;
        

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            hortizontalMovement = Input.GetAxis("Horizontal");
            verticalMovement = Input.GetAxis("Vertical");
            
            playerMovement = new Vector3(hortizontalMovement, 0, verticalMovement).normalized;

            characterController.Move(playerMovement * MOVE_SPEED * Time.deltaTime);
        }
    }
}