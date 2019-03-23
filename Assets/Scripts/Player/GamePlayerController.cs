using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

namespace Player
{
    public class GamePlayerController : MonoBehaviour
    {
        [Header("Controller Dependencies"), SerializeField]
        private Rigidbody _playerRb;

        [SerializeField]
        private Camera _playerCamera;

        [SerializeField]
        private BaseWeaponBehaviour _weapon;

        [Header("Player Config"), SerializeField]
        private float _walkingSpeed = 5f;

        [SerializeField]
        private float _jumpPower = 5f;

        [SerializeField]
        private float _jumpSnapDistance = 0.5f;

        [FormerlySerializedAs("_gravityAcceleration"),SerializeField]
        private float _gravityValue = 9.81f;

        [SerializeField]
        private float _maxGravityDescentSpeed = 53;

        [SerializeField]
        private float _sprintMultiplier = 1.5f;

        [SerializeField]
        private float _cameraVerticalAngleLimit = 70f;

        [Header("Control Settings"), SerializeField]
        private KeyCode _forwardKey = KeyCode.W;

        [SerializeField]
        private KeyCode _backwardKey = KeyCode.S;

        [SerializeField]
        private KeyCode _leftKey = KeyCode.A;

        [SerializeField]
        private KeyCode _rightKey = KeyCode.D;

        [SerializeField]
        private KeyCode _crouchKey = KeyCode.C;

        [SerializeField]
        private KeyCode _sprintKey = KeyCode.LeftShift;

        [SerializeField]
        private KeyCode _aimButton = KeyCode.Mouse1;

        [SerializeField]
        private KeyCode _fireButton = KeyCode.Mouse0;

        [SerializeField]
        private KeyCode _cursorUnlockKey = KeyCode.Escape;

        [SerializeField]
        private KeyCode _cursorLockKey = KeyCode.Mouse0;

        [SerializeField]
        private float _mouseSensitivity = 1;

        [SerializeField]
        private float _cameraCrouchOffsetDistance = 0.5f;

        [SerializeField]
        private bool _invertMouseY;

        private PlayerControllerMovementState _controllerMovementState;
        private Vector3 _inputDirection;
        private bool _isShooting;
        private bool _isAiming;
        private Vector3 _startingCameraPos;
        private Vector3 _directionVector;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _startingCameraPos = _playerCamera.transform.localPosition;
            _playerRb.useGravity = false;
        }

        private void FixedUpdate()
        {
            _weapon.ShouldAim = _isAiming;

            if (_isShooting)
            {
                _weapon.Fire();
            }

            float finalWalkingSpeed = _walkingSpeed;

            switch (_controllerMovementState)
            {
                case PlayerControllerMovementState.SprintJumpApplied:
                case PlayerControllerMovementState.OnGroundSprinting:
                    finalWalkingSpeed *= _sprintMultiplier;
                    break;
                case PlayerControllerMovementState.Jumping:
                    _controllerMovementState = PlayerControllerMovementState.JumpApplied;
                    ApplyJump();
                    break;
                case PlayerControllerMovementState.SprintJumping:
                    _controllerMovementState = PlayerControllerMovementState.SprintJumpApplied;
                    ApplyJump();
                    break;
            }

            bool isInGroundState = _controllerMovementState == PlayerControllerMovementState.OnGround ||
                                   _controllerMovementState == PlayerControllerMovementState.OnGroundSprinting;


            if (_inputDirection == Vector3.zero) return;

            if (isInGroundState)
            {
                _directionVector = (_playerRb.transform.right * _inputDirection.x) +
                                   (_playerRb.transform.forward * _inputDirection.z);
            }

            Debug.Log(_directionVector);

            _playerRb.MovePosition(_playerRb.position + (_directionVector * finalWalkingSpeed) * Time.deltaTime);
        }

        private void ApplyJump()
        {
            StartCoroutine(HandleJumpArc());
        }

        private IEnumerator HandleJumpArc()
        {
            float fallingVelocityAbsolute = 0;
            float jumpVelocityAbsolute = _jumpPower;
            while (_controllerMovementState == PlayerControllerMovementState.JumpApplied || _controllerMovementState == PlayerControllerMovementState.SprintJumpApplied)
            {

                if (jumpVelocityAbsolute > 0)
                {
                    if (jumpVelocityAbsolute < _jumpSnapDistance)
                    {
                        jumpVelocityAbsolute = 0;
                    }
                    else
                    {
                        jumpVelocityAbsolute -= _gravityValue * Time.fixedDeltaTime;
                    }
                    _playerRb.MovePosition(new Vector3(_playerRb.position.x, _playerRb.position.y + (jumpVelocityAbsolute * Time.fixedDeltaTime), _playerRb.position.z));
                }
                else
                {
                    if (fallingVelocityAbsolute < _maxGravityDescentSpeed)
                    {
                        fallingVelocityAbsolute += _gravityValue * Time.fixedDeltaTime;
                    }
                    else if (fallingVelocityAbsolute > _maxGravityDescentSpeed)
                    {
                        fallingVelocityAbsolute = _maxGravityDescentSpeed;
                    }
                    _playerRb.MovePosition(new Vector3(_playerRb.position.x, _playerRb.position.y - fallingVelocityAbsolute * Time.fixedDeltaTime, _playerRb.position.z));
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void Update()
        {
            HandleCursorLockState();
            
            HandleCrouchInput();

            HandleCameraInput();

            HandleGunInput();

            if (_controllerMovementState != PlayerControllerMovementState.OnGround &&
                _controllerMovementState != PlayerControllerMovementState.OnGroundSprinting) return;

            HandleMovementInput();

            HandleSprintingInput();

            HandleJumpingInput();
        }

        private void HandleCursorLockState()
        {
            if (Input.GetKeyDown(_cursorLockKey))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Input.GetKeyDown(_cursorUnlockKey))
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

        private void HandleJumpingInput()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            
            _controllerMovementState = _controllerMovementState == PlayerControllerMovementState.OnGroundSprinting
                ? PlayerControllerMovementState.SprintJumping
                : PlayerControllerMovementState.Jumping;
        }

        private void HandleSprintingInput()
        {
            _controllerMovementState = Input.GetKey(_sprintKey)
                ? PlayerControllerMovementState.OnGroundSprinting
                : PlayerControllerMovementState.OnGround;
        }

        private void HandleMovementInput()
        {
            if (Input.GetKey(_forwardKey) && !Input.GetKey(_backwardKey))
            {
                _inputDirection.z = 1f;
            }
            else if (Input.GetKey(_backwardKey) && !Input.GetKey(_forwardKey))
            {
                _inputDirection.z = -1f;
            }
            else
            {
                _inputDirection.z = 0;
            }

            if (Input.GetKey(_rightKey) && !Input.GetKey(_leftKey))
            {
                _inputDirection.x = 1f;
            }
            else if (Input.GetKey(_leftKey) && !Input.GetKey(_rightKey))
            {
                _inputDirection.x = -1f;
            }
            else
            {
                _inputDirection.x = 0;
            }
        }

        private void HandleGunInput()
        {
            _isAiming = Input.GetKey(_aimButton);
            _isShooting = Input.GetKey(_fireButton);
        }

        private void HandleCameraInput()
        {
            float mouseInputX = Input.GetAxis("Mouse X");
            float mouseInputY = !_invertMouseY ? -Input.GetAxis("Mouse Y") : Input.GetAxis("Mouse Y");

            _playerRb.MoveRotation(_playerRb.rotation * Quaternion.Euler(0, mouseInputY * _mouseSensitivity, 0));

            var angles = _playerCamera.transform.localRotation.eulerAngles;

            float target = _playerCamera.transform.eulerAngles.x;

            if (target > 180)
            {
                target -= 360;
            }

            angles.x = Mathf.Clamp(target, -_cameraVerticalAngleLimit, _cameraVerticalAngleLimit);

            _playerCamera.transform.localRotation = Quaternion.Euler(angles);

            transform.Rotate(0, mouseInputX * _mouseSensitivity, 0);
        }

        private void HandleCrouchInput()
        {
            if (Input.GetKey(_crouchKey))
            {
                _playerCamera.transform.localPosition = new Vector3(_playerCamera.transform.localPosition.x,
                    _startingCameraPos.y - _cameraCrouchOffsetDistance, _playerCamera.transform.localPosition.z);
            }
            else
            {
                _playerCamera.transform.localPosition = _startingCameraPos;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Floor")) return;

            _controllerMovementState = _controllerMovementState == PlayerControllerMovementState.SprintJumpApplied
                ? PlayerControllerMovementState.OnGroundSprinting
                : PlayerControllerMovementState.OnGround;
        }
    }
}