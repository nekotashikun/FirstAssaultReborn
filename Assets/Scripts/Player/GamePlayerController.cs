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
        private KeyCode _sprintKey = KeyCode.LeftShift;

        [SerializeField]
        private float _mouseSensitivity = 1;

        [SerializeField]
        private bool _invertMouseY;

        [SerializeField]
        private KeyCode _aimButton = KeyCode.Mouse1;

        [SerializeField]
        private KeyCode _fireButton = KeyCode.Mouse0;


        private PlayerControllerMovementState _controllerMovementState;
        private Vector3 _inputDirection;
        private Quaternion _velocityDirection;
        private bool _isShooting;
        private bool _isAiming;


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


            if (_inputDirection == Vector3.zero)
            {
                _playerRb.velocity = new Vector3(0, _playerRb.velocity.y, 0);
                return;
            }

            float resultDirectionSpeed =
                (1 / (Mathf.Abs(_inputDirection.x) + Mathf.Abs(_inputDirection.z))) * finalWalkingSpeed;

            _playerRb.velocity = _velocityDirection * new Vector3(_inputDirection.x * resultDirectionSpeed,
                                     _playerRb.velocity.y, _inputDirection.z * resultDirectionSpeed);
        }

        private void ApplyJump()
        {
            _playerRb.AddRelativeForce(0, _jumpPower, 0, ForceMode.VelocityChange);
        }

        // Update is called once per frame
        private void Update()
        {
            float mouseInputX = Input.GetAxis("Mouse X");
            float mouseInputY = !_invertMouseY ? -Input.GetAxis("Mouse Y") : Input.GetAxis("Mouse Y");

            _playerCamera.transform.Rotate(mouseInputY * _mouseSensitivity, 0, 0);

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
            if (other.gameObject.CompareTag("Floor"))
                _controllerMovementState = _controllerMovementState == PlayerControllerMovementState.SprintJumpApplied
                    ? PlayerControllerMovementState.OnGroundSprinting
                    : PlayerControllerMovementState.OnGround;
        }
    }
}