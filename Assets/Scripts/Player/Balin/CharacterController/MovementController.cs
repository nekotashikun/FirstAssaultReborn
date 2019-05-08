using System;
using System.Collections;
using System.Collections.Generic;

using Scripts.Player.Balin.Input;
using UnityEngine;

namespace Scripts.Player.Balin.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour, IControllable, ILocalCharacterView
    {
        private delegate IEnumerator CharacterStateDelegate();

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

        private InputState _inputState;
		public MovementStateEnum MovementState { get; private set; } = MovementStateEnum.Standing;

        public Vector3 Position => transform.position;

        public Vector3 LookRotation => new Vector3(lookAngle, transform.eulerAngles.y);

        private MovementStateEnum _nextState;

        private Dictionary<MovementStateEnum, CharacterStateDelegate> _movementStateDelegates;

        // Start is called before the first frame update
        void Start()
        {
            characterController = gameObject.GetComponent<CharacterController>();

            _movementStateDelegates = new Dictionary<MovementStateEnum, CharacterStateDelegate>()
            {
                { MovementStateEnum.Standing, StandingState },
                { MovementStateEnum.Running, RunningState },
                { MovementStateEnum.Sprinting, SprintingState },
                { MovementStateEnum.Walking, WalkingState },
                { MovementStateEnum.Crouching, CrouchingState },
                { MovementStateEnum.CrouchWalking, CrouchWalkingState },
                { MovementStateEnum.Jumping, JumpingState },
                { MovementStateEnum.Falling, FallingState }
            };

            StartCoroutine(StandingState());
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.Rotate(Vector3.up, rotateSpeed * _inputState.MouseHorizontal * Time.fixedDeltaTime, Space.Self);

            lookAngle += rotateSpeed * _inputState.MouseVertical * Time.fixedDeltaTime;
            lookAngle = Mathf.Clamp(lookAngle, lookVerticalUpperLimit, lookVerticalLowerLimit);

            currentState = MovementState.ToString();
        }

        void Update()
        {
            Debug.DrawLine(transform.position, transform.position + characterController.velocity, Color.black);
        }

        public void HandleInput(InputState tickState)
        {
            _inputState = tickState;
        }

        private IEnumerator StandingState()
        {
            //State Start
            MovementState = MovementStateEnum.Standing;

            //State Update
            while(true)
            {
                if(!characterController.isGrounded)
                {
                    _nextState = MovementStateEnum.Falling;
                    break;
                }
                
                if(_inputState.Crouch)
                {
                    _nextState = MovementStateEnum.Crouching;
                    break;
                }
                
                if(!_inputState.Walk && _inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.Running;
                    break;
                }
                
                if (_inputState.Walk && _inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.Walking;
                    break;
                }

                if (_inputState.Jump)
                {
                    _nextState = MovementStateEnum.Jumping;
                    break;
                }
                
                speed = Vector3.zero;

                Vector3 movementDelta = Physics.gravity;
                characterController.Move(movementDelta * Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }

            //State End

            //State Transition
            StartCoroutine(_movementStateDelegates[_nextState]());
        }

        private IEnumerator RunningState()
        {
            MovementState = MovementStateEnum.Running;

            while (true)
            {
                if (!characterController.isGrounded)
                {
                    _nextState = MovementStateEnum.Falling;
                    break;
                }

                if (!_inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.Standing;
                    break;
                }

                if (_inputState.Crouch && _inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.CrouchWalking;
                    break;
                }

                if (_inputState.Walk && _inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.Walking;
                    break;
                }

                if (_inputState.Jump)
                {
                    _nextState = MovementStateEnum.Jumping;
                    break;
                }

                if (_inputState.Sprint && _inputState.MoveForward && !_inputState.MoveBackward)
                {
                    _nextState = MovementStateEnum.Sprinting;
                    break;
                }

                float movementHorizontalAxis = (_inputState.MoveRight ? 1 : 0) + (_inputState.MoveLeft ? -1 : 0);
                float movementVerticalAxis = (_inputState.MoveForward ? 1 : 0) + (_inputState.MoveBackward ? -1 : 0);

                Vector2 movementDirection = new Vector2(movementHorizontalAxis, movementVerticalAxis);
                if (movementDirection.sqrMagnitude > 1)
                {
                    movementDirection = movementDirection.normalized;
                }

                if (movementDirection == Vector2.zero)
                {
                    _nextState = MovementStateEnum.Standing;
                    break;
                }

                speed =
                transform.forward * moveSpeed * movementDirection.y +
                transform.right * moveSpeed * movementDirection.x
                ;

                if (Physics.Raycast(transform.position, -transform.up, MovementController.standingSize * 0.75f))
                {
                    speed.y = Physics.gravity.y;
                }

                characterController.Move(speed * Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }

            //State Transition
            StartCoroutine(_movementStateDelegates[_nextState]());
        }

        private IEnumerator CrouchingState()
        {
            MovementState = MovementStateEnum.Crouching;

            characterController.height = crouchingSize;
            characterController.Move(new Vector3(0, -(standingSize - crouchingSize) / 2, 0));

            while (true)
            {
                if (!characterController.isGrounded)
                {
                    _nextState = MovementStateEnum.Falling;
                    break;
                }

                if (
                !_inputState.Crouch &&
                !Physics.SphereCast(new Ray(transform.position, Vector3.up), characterController.radius, 2f)
                )
                {
                    _nextState = MovementStateEnum.Standing;
                    break;
                }

                if (_inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.CrouchWalking;
                    break;
                }

                speed = Vector3.zero;

                Vector3 movementDelta = Physics.gravity;
                characterController.Move(movementDelta * Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }

            characterController.height = standingSize;
            characterController.Move(new Vector3(0, +(standingSize - crouchingSize) / 2.1f, 0));

            //State Transition
            StartCoroutine(_movementStateDelegates[_nextState]());
        }

        private IEnumerator SprintingState()
        {
            MovementState = MovementStateEnum.Sprinting;

            while (true)
            {
                if (!characterController.isGrounded)
                {
                    _nextState = MovementStateEnum.Falling;
                    break;
                }

                if (
                !_inputState.IsMoving || 
                !_inputState.Sprint || 
                !_inputState.MoveForward || 
                _inputState.MoveBackward
                )
                {
                    _nextState = MovementStateEnum.Running;
                    break;
                }

                if (_inputState.Crouch)
                {
                    _nextState = MovementStateEnum.CrouchWalking;
                    break;
                }

                if (_inputState.Jump)
                {
                    _nextState = MovementStateEnum.Jumping;
                    break;
                }

                float movementHorizontalAxis = (_inputState.MoveRight ? 1 : 0) + (_inputState.MoveLeft ? -1 : 0);
                float movementVerticalAxis = (_inputState.MoveForward ? 1 : 0) + (_inputState.MoveBackward ? -1 : 0);

                Vector2 movementDirection = new Vector2(movementHorizontalAxis, movementVerticalAxis);
                if (movementDirection.sqrMagnitude > 1)
                {
                    movementDirection = movementDirection.normalized;
                }

                speed =
                transform.forward * moveSpeed * sprintSpeedModifier * movementDirection.y +
                transform.right * moveSpeed * sprintSpeedModifier * movementDirection.x
                ;

                if (Physics.Raycast(transform.position, -transform.up, MovementController.standingSize * 0.75f))
                {
                    speed.y = Physics.gravity.y;
                }

                characterController.Move(speed * Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }

            //State Transition
            StartCoroutine(_movementStateDelegates[_nextState]());
        }

        private IEnumerator WalkingState()
        {
            MovementState = MovementStateEnum.Walking;

            while (true)
            {
                if (!characterController.isGrounded)
                {
                    _nextState = MovementStateEnum.Falling;
                    break;
                }

                if (!_inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.Standing;
                    break;
                }

                if (_inputState.Crouch && _inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.CrouchWalking;
                    break;
                }

                if (!_inputState.Walk && _inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.Running;
                    break;
                }

                if (_inputState.Jump)
                {
                    _nextState = MovementStateEnum.Jumping;
                    break;
                }

                float movementHorizontalAxis = (_inputState.MoveRight ? 1 : 0) + (_inputState.MoveLeft ? -1 : 0);
                float movementVerticalAxis = (_inputState.MoveForward ? 1 : 0) + (_inputState.MoveBackward ? -1 : 0);

                Vector2 movementDirection = new Vector2(movementHorizontalAxis, movementVerticalAxis);
                if (movementDirection.sqrMagnitude > 1)
                {
                    movementDirection = movementDirection.normalized;
                }

                if (movementDirection == Vector2.zero)
                {
                    _nextState = MovementStateEnum.Standing;
                    break;
                }

                speed =
                transform.forward * moveSpeed * walkSpeedModifier * movementDirection.y +
                transform.right * moveSpeed * walkSpeedModifier * movementDirection.x
                ;

                if (Physics.Raycast(transform.position, -transform.up, MovementController.standingSize * 0.75f))
                {
                    speed.y = Physics.gravity.y;
                }

                characterController.Move(speed * Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }

            //State Transition
            StartCoroutine(_movementStateDelegates[_nextState]());
        }

        private IEnumerator CrouchWalkingState()
        {
            MovementState = MovementStateEnum.CrouchWalking;

            characterController.height = crouchingSize;
            characterController.Move(new Vector3(0, -(standingSize - crouchingSize) / 2, 0));

            while (true)
            {
                if (!characterController.isGrounded)
                {
                    _nextState = MovementStateEnum.Falling;
                    break;
                }

                if (!_inputState.IsMoving)
                {
                    _nextState = MovementStateEnum.Crouching;
                    break;
                }

                if (
                !_inputState.Crouch &&
                _inputState.IsMoving &&
                !Physics.SphereCast(new Ray(transform.position, Vector3.up), characterController.radius, 2f)
                )
                {
                    _nextState = MovementStateEnum.Running;
                    break;
                }

                float movementHorizontalAxis = (_inputState.MoveRight ? 1 : 0) + (_inputState.MoveLeft ? -1 : 0);
                float movementVerticalAxis = (_inputState.MoveForward ? 1 : 0) + (_inputState.MoveBackward ? -1 : 0);

                Vector2 movementDirection = new Vector2(movementHorizontalAxis, movementVerticalAxis);
                if (movementDirection.sqrMagnitude > 1)
                {
                    movementDirection = movementDirection.normalized;
                }

                if (movementDirection == Vector2.zero)
                {
                    _nextState = MovementStateEnum.Crouching;
                    break;
                }

                speed =
                transform.forward * moveSpeed * crouchSpeedModifier * movementDirection.y +
                transform.right * moveSpeed * crouchSpeedModifier * movementDirection.x
                ;

                if (Physics.Raycast(transform.position, -transform.up, MovementController.standingSize * 0.75f))
                {
                    speed.y = Physics.gravity.y;
                }

                characterController.Move(speed * Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }

            characterController.height = standingSize;
            characterController.Move(new Vector3(0, +(standingSize - crouchingSize) / 2.1f, 0));

            //State Transition
            StartCoroutine(_movementStateDelegates[_nextState]());
        }

        private IEnumerator JumpingState()
        {
            MovementState = MovementStateEnum.Jumping;

            speed.y = jumpSpeed;
            characterController.Move(new Vector3(0, speed.y * Time.fixedDeltaTime, 0));

            while (true)
            {
                if (speed.y <= 0)
                {
                    _nextState = MovementStateEnum.Falling;
                    break;
                }

                speed.y += Physics.gravity.y * Time.fixedDeltaTime;

                CollisionFlags flags = characterController.Move(speed * Time.fixedDeltaTime);

                if ((flags & CollisionFlags.Above) != 0)
                {
                    speed.y = 0;
                }

                yield return new WaitForFixedUpdate();
            }

            //State Transition
            StartCoroutine(_movementStateDelegates[_nextState]());
        }

        private IEnumerator FallingState()
        {
            MovementState = MovementStateEnum.Falling;

            speed.y = 0;

            while (true)
            {
                if (characterController.isGrounded)
                {
                    _nextState = MovementStateEnum.Standing;
                    break;
                }

                speed.y += Physics.gravity.y * Time.fixedDeltaTime;

                float movementHorizontalAxis = (_inputState.MoveRight ? 1 : 0) + (_inputState.MoveLeft ? -1 : 0);
                float movementVerticalAxis = (_inputState.MoveForward ? 1 : 0) + (_inputState.MoveBackward ? -1 : 0);

                Vector2 movementDirection = new Vector2(movementHorizontalAxis, movementVerticalAxis);
                if (movementDirection.sqrMagnitude > 1)
                {
                    movementDirection = movementDirection.normalized;
                }

                characterController.Move(speed * Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }

            speed.y = 0;

            //State Transition
            StartCoroutine(_movementStateDelegates[_nextState]());
        }
    }

}
