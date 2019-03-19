using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        private float _mouseSensitivity = 1;

        [SerializeField]
        private float _cameraCrouchOffsetDistance = 0.5f;

        [SerializeField]
        private bool _invertMouseY;

        private PlayerControllerMovementState _controllerMovementState;
        private Vector3 _inputDirection;
        private Quaternion _velocityDirection;
        private bool _isShooting;
        private bool _isAiming;
        private Vector3 _startingCameraPos;
        private Vector3 _directionVector;

        private void Start()
        {
            _startingCameraPos = _playerCamera.transform.localPosition;
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
                    ApplyJump();
                    _controllerMovementState = PlayerControllerMovementState.JumpApplied;
                    break;
                case PlayerControllerMovementState.SprintJumping:
                    ApplyJump();
                    _controllerMovementState = PlayerControllerMovementState.SprintJumpApplied;
                    break;
            }

            bool isInGroundState = _controllerMovementState == PlayerControllerMovementState.OnGround ||
                                   _controllerMovementState == PlayerControllerMovementState.OnGroundSprinting;


            if (_inputDirection == Vector3.zero)
            {
                if (isInGroundState && _controllerMovementState != PlayerControllerMovementState.JumpApplied && _controllerMovementState != PlayerControllerMovementState.SprintJumpApplied)
                {
                    _playerRb.isKinematic = true;
                }
                return;
            }

            VerifyKinematicState();

//            float resultDirectionSpeed =
//                (1 / (Mathf.Abs(_inputDirection.x) + Mathf.Abs(_inputDirection.z))) * finalWalkingSpeed;

            if (isInGroundState)
            {
                _directionVector = (_playerRb.transform.right * _inputDirection.x) +
                                   (_playerRb.transform.forward * _inputDirection.z);
            }

            Debug.Log(_directionVector);

            _playerRb.MovePosition(_playerRb.position + (_directionVector * finalWalkingSpeed) * Time.deltaTime);


//            _playerRb.MovePosition((_playerRb.position + (_playerRb.transform.forward * new Vector3(_inputDirection.x * resultDirectionSpeed,
//                                     _playerRb.velocity.y, _inputDirection.z * resultDirectionSpeed) * Time.fixedDeltaTime)));
        }

        private void VerifyKinematicState()
        {
            if (_playerRb.isKinematic)
            {
                _playerRb.isKinematic = false;
            }
        }

        private void ApplyJump()
        {
            VerifyKinematicState();
            _playerRb.AddRelativeForce(0, _jumpPower, 0, ForceMode.VelocityChange);
        }

        // Update is called once per frame
        private void Update()
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

            _isAiming = Input.GetKey(_aimButton);
            _isShooting = Input.GetKey(_fireButton);

            if (_controllerMovementState != PlayerControllerMovementState.OnGround &&
                _controllerMovementState != PlayerControllerMovementState.OnGroundSprinting) return;

            _velocityDirection = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

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

            _controllerMovementState = Input.GetKey(_sprintKey)
                ? PlayerControllerMovementState.OnGroundSprinting
                : PlayerControllerMovementState.OnGround;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _controllerMovementState = _controllerMovementState == PlayerControllerMovementState.OnGroundSprinting
                    ? PlayerControllerMovementState.SprintJumping
                    : PlayerControllerMovementState.Jumping;
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