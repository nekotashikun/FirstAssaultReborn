using System.Collections;
using System.Collections.Generic;

using Scripts.Player.Balin.Input;
using UnityEngine;

namespace Scripts.Player.Balin.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class TestMovement : MonoBehaviour, IControllable
    {
        public const float standingSize = 1.8f;
        public const float crouchingSize = 0.8f;

        public const float lookVerticalUpperLimit = -89;
        public const float lookVerticalLowerLimit = 89;

        [SerializeField]
        public CharacterController characterController;

        public string currentState = "";

        public float moveSpeed = 5;
        public float sprintSpeedModifier = 2f;
        public float walkSpeedModifier = 0.75f;
        public float crouchSpeedModifier = 0.5f;
        public float rotateSpeed = 90;
        public float jumpSpeed = 5;
        public float airControlModifier = 0.2f;
        public AnimationCurve inertiaCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.1f, 0.9f, Mathf.Tan(Mathf.PI / 4), Mathf.Tan(Mathf.PI / 4)), new Keyframe(1f, 1f));


        public float lookAngle = 0;
        public Vector3 speed = Vector3.zero;
        public float verticalSpeed = 0;

        private InputState _inputState;
        public CharacterBaseState characterState = CharacterStateMachine.Standing;

        // Start is called before the first frame update
        void Start()
        {
            characterController = gameObject.GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            characterState.OnFixedUpdate(this, _inputState);

            if(characterState == CharacterStateMachine.Standing)
            {
                currentState = "Standing";
            }
            else if (characterState == CharacterStateMachine.Crouching)
            {
                currentState = "Crouching";
            }
            else if (characterState == CharacterStateMachine.Running)
            {
                currentState = "Running";
            }
            else if (characterState == CharacterStateMachine.CrouchWalking)
            {
                currentState = "CrouchWalking";
            }
            else if (characterState == CharacterStateMachine.Walking)
            {
                currentState = "Walking";
            }
            else if (characterState == CharacterStateMachine.Sprinting)
            {
                currentState = "Sprinting";
            }
            else if (characterState == CharacterStateMachine.Jumping)
            {
                currentState = "Jumping";
            }
            else if(characterState == CharacterStateMachine.Falling)
            {
                currentState = "Falling";
            }
        }

        void Update()
        {
            Debug.DrawLine(transform.position, transform.position + characterController.velocity, Color.black);
        }

        public static Vector2 GetPositionDeltaComponents(float angle)
        {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        public void HandleInput(InputState tickState)
        {
            _inputState = tickState;
        }
    }

}
