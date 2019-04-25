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

        [Header("Optional Dependencies"), SerializeField]
        private Animator _animator;

        [Header("Player Config"), SerializeField]
        private float _walkingSpeed = 5f;

        [SerializeField]
        private string _walkAnimParameter = string.Empty;

        [SerializeField]
        private string _sprintAnimParameter = string.Empty;

        [SerializeField]
        private string _crouchAnimParameter = string.Empty;

        [SerializeField]
        private string _crouchWalkAnimParameter = string.Empty;

        [SerializeField]
        private string _jumpAnimParameter = string.Empty;

        [SerializeField]
        private string _midAirParameter = string.Empty;

        [SerializeField]
        private string _landAnimParameter = string.Empty;

        [SerializeField]
        private float _jumpPower = 5f;

        [SerializeField]
        private float _jumpSnapDistance = 0.5f;

        [FormerlySerializedAs("_gravityAcceleration"), SerializeField]
        private float _gravityValue = 9.81f;

        [SerializeField]
        private float _maxGravityDescentSpeed = 53;

        [SerializeField]
        private float _sprintMultiplier = 1.5f;

        [SerializeField]
        private float _cameraVerticalAngleLimit = 70f;

        [SerializeField]
        private float _aimCrouchMovementSpeedDivider = 2;

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
        private KeyCode _reloadButton = KeyCode.R;

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
        private List<GameObject> _touchedFloorObjects;
        private bool _isCrouching;
        private bool _isReloading;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _startingCameraPos = _playerCamera.transform.localPosition;
            _playerRb.useGravity = false;
            _touchedFloorObjects = new List<GameObject>();
            _playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void FixedUpdate()
        {
            _playerRb.velocity = Vector3.zero;
            _playerRb.angularVelocity = Vector3.zero;
            _weapon.ShouldAim = _isAiming;

            //TODO: Clean this up jeezus
            if (_isShooting && _controllerMovementState != PlayerControllerMovementState.Sprinting)
            {
                //Debug.Log("[Matt] Fire Input Received [TRUE]");
                _animator.SetBool(_weapon.FireAnimParameter, _weapon.Fire());
            }
            else
            {
                //Debug.Log("[Matt] Fire Input Received [FALSE]");
                _animator.SetBool(_weapon.FireAnimParameter, false);
            }

            if (_isReloading && (_controllerMovementState == PlayerControllerMovementState.OnGround || _controllerMovementState == PlayerControllerMovementState.Walking))
            {
                //Debug.Log("[Matt] Reload Input Received [TRUE]");
                _animator.SetBool(_weapon.ReloadAnimParameter, _weapon.Reload());
            }
            else
            {
                //Debug.Log("[Matt] Reload Input Received [FALSE]");
                _animator.SetBool(_weapon.ReloadAnimParameter, false);
            }

            float finalWalkingSpeed;

            if (_isAiming || _isCrouching)
            {
                finalWalkingSpeed = _walkingSpeed / _aimCrouchMovementSpeedDivider;
            }
            else
            {
                finalWalkingSpeed = _walkingSpeed;
            }
            
            float finalWalkingSpeed;

            if (_isAiming || _isCrouching)
            {
                finalWalkingSpeed = _walkingSpeed / _aimCrouchMovementSpeedDivider;
            }
            else
            {
                finalWalkingSpeed = _walkingSpeed;
            }
            
            bool shouldIgnoreSprintMultiplierPrevention = false;

            switch (_controllerMovementState)
            {
                case PlayerControllerMovementState.OnGround:
                    if (_animator == null) break;
                    _animator.SetBool(_walkAnimParameter, false);
                    _animator.SetBool(_sprintAnimParameter, false);
                    _animator.SetBool(_jumpAnimParameter, false);
                    _animator.SetBool(_midAirParameter, false);
                    _animator.SetBool(_landAnimParameter, false);
                    _animator.SetBool(_crouchWalkAnimParameter, false);
                    _animator.SetBool(_crouchAnimParameter, !_isAiming && !_isShooting && !_isReloading && _isCrouching);
                    break;
                case PlayerControllerMovementState.Walking:
                    if (_animator == null) break;
                    _animator.SetBool(_walkAnimParameter, false);
                    _animator.SetBool(_sprintAnimParameter, false);
                    _animator.SetBool(_jumpAnimParameter, false);
                    _animator.SetBool(_midAirParameter, false);
                    _animator.SetBool(_landAnimParameter, false);
                    _animator.SetBool(_crouchWalkAnimParameter, false);
                    _animator.SetBool(_isCrouching ? _crouchWalkAnimParameter : _walkAnimParameter,
                        !_isAiming && !_isShooting);
                    break;
                case PlayerControllerMovementState.SprintJumpApplied:
                    if (_animator == null) goto case PlayerControllerMovementState.Sprinting;
                    _animator.SetBool(_walkAnimParameter, false);
                    _animator.SetBool(_sprintAnimParameter, false);
                    _animator.SetBool(_jumpAnimParameter, false);
                    _animator.SetBool(_midAirParameter, false);
                    _animator.SetBool(_landAnimParameter, false);
                    _animator.SetBool(_crouchWalkAnimParameter, false);
                    _animator.SetBool(_midAirParameter, !_isAiming && !_isShooting && !_isReloading);
                    goto case PlayerControllerMovementState.Sprinting;
                case PlayerControllerMovementState.Sprinting:
                    if (_animator != null)
                    {
                        _animator.SetBool(_walkAnimParameter, false);
                        _animator.SetBool(_sprintAnimParameter, false);
                        _animator.SetBool(_jumpAnimParameter, false);
                        if (_touchedFloorObjects.Count != 0)
                        {
                            _animator.SetBool(_midAirParameter, false);
                        }
                        _animator.SetBool(_landAnimParameter, false);
                        _animator.SetBool(_crouchWalkAnimParameter, false);
                        _animator.SetBool(_sprintAnimParameter, !_isAiming && !_isShooting && !_isReloading);
                    }

                    if (_isAiming || _isShooting) break;
                    finalWalkingSpeed *= _sprintMultiplier;
                    break;
                case PlayerControllerMovementState.Jumping:
                    _controllerMovementState = PlayerControllerMovementState.JumpApplied;
                    HandleMandatoryJumpingLogic();
                    break;
                case PlayerControllerMovementState.SprintJumping:
                    _controllerMovementState = PlayerControllerMovementState.SprintJumpApplied;
                    HandleMandatoryJumpingLogic();
                    break;
                case PlayerControllerMovementState.JumpApplied:
                    if (_animator == null) break;

                    _animator.SetBool(_walkAnimParameter, false);
                    _animator.SetBool(_sprintAnimParameter, false);
                    _animator.SetBool(_jumpAnimParameter, false);
                    _animator.SetBool(_midAirParameter, false);
                    _animator.SetBool(_landAnimParameter, false);
                    _animator.SetBool(_crouchWalkAnimParameter, false);
                    _animator.SetBool(_midAirParameter, !_isAiming && !_isShooting && !_isReloading);
                    break;
            }

            bool isInGroundState = _controllerMovementState == PlayerControllerMovementState.OnGround ||
                                   _controllerMovementState == PlayerControllerMovementState.Sprinting ||
                                   _controllerMovementState == PlayerControllerMovementState.Walking;


            if (_inputDirection == Vector3.zero) return;

            if (isInGroundState)
            {
                _directionVector = (_playerRb.transform.right * _inputDirection.x) +
                                   (_playerRb.transform.forward * _inputDirection.z);
            }


            _playerRb.MovePosition(_playerRb.position + (_directionVector * finalWalkingSpeed) * Time.deltaTime);
        }

        private void HandleMandatoryJumpingLogic()
        {
            ApplyJump();
            if (_animator == null) return;
            _animator.SetBool(_walkAnimParameter, false);
            _animator.SetBool(_sprintAnimParameter, false);
            _animator.SetBool(_jumpAnimParameter, false);
            _animator.SetBool(_midAirParameter, false);
            _animator.SetBool(_landAnimParameter, false);
            _animator.SetBool(_crouchWalkAnimParameter, false);
            _animator.SetBool(_jumpAnimParameter, !_isAiming && !_isShooting);
        }

        private void ApplyJump()
        {
            StartCoroutine(HandleJumpArc(false));
        }

        private IEnumerator HandleJumpArc(bool shouldJustFall)
        {
            float fallingVelocityAbsolute = 0;
            float jumpVelocityAbsolute = shouldJustFall ? 0 : _jumpPower;
            while (_controllerMovementState == PlayerControllerMovementState.JumpApplied ||
                   _controllerMovementState == PlayerControllerMovementState.SprintJumpApplied)
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

                    _playerRb.MovePosition(new Vector3(_playerRb.position.x,
                        _playerRb.position.y + (jumpVelocityAbsolute * Time.fixedDeltaTime), _playerRb.position.z));
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

                    _playerRb.MovePosition(new Vector3(_playerRb.position.x,
                        _playerRb.position.y - fallingVelocityAbsolute * Time.fixedDeltaTime, _playerRb.position.z));
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private void Update()
        {
            HandleCursorLockState();

            HandleCameraInput();

            HandleGunInput();

            if (_controllerMovementState != PlayerControllerMovementState.OnGround &&
                _controllerMovementState != PlayerControllerMovementState.Sprinting &&
                _controllerMovementState != PlayerControllerMovementState.Walking) return;
            
            HandleCrouchInput();

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

            _controllerMovementState = _controllerMovementState == PlayerControllerMovementState.Sprinting
                ? PlayerControllerMovementState.SprintJumping
                : PlayerControllerMovementState.Jumping;
        }

        private void HandleSprintingInput()
        {
            if (Input.GetKey(_sprintKey) && _controllerMovementState == PlayerControllerMovementState.Walking && !_isCrouching)
            {
                _controllerMovementState = PlayerControllerMovementState.Sprinting;
            }
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

            if (_inputDirection == Vector3.zero)
            {
                _controllerMovementState = PlayerControllerMovementState.OnGround;
                return;
            }

            _controllerMovementState = PlayerControllerMovementState.Walking;
        }

        private void HandleGunInput()
        {
            _isAiming = Input.GetKey(_aimButton) && !_isReloading;
            _isShooting = Input.GetKey(_fireButton) && !_isReloading;
            _isReloading = Input.GetKeyDown(_reloadButton) && !_isShooting && !_isAiming;
        }

        private void HandleCameraInput()
        {
            float mouseInputX = Input.GetAxis("Mouse X");
            float mouseInputY = !_invertMouseY ? -Input.GetAxis("Mouse Y") : Input.GetAxis("Mouse Y");

            _playerRb.MoveRotation(_playerRb.rotation * Quaternion.Euler(0, mouseInputX * _mouseSensitivity, 0));

            var angles = _playerCamera.transform.localRotation.eulerAngles;

            float target = _playerCamera.transform.eulerAngles.x;

            if (target > 180)
            {
                target -= 360;
            }
            
            target += mouseInputY * _mouseSensitivity;

            angles.x = Mathf.Clamp(target, -_cameraVerticalAngleLimit, _cameraVerticalAngleLimit);

            _playerCamera.transform.localRotation = Quaternion.Euler(angles);
        }

        private void HandleCrouchInput()
        {
            _isCrouching = Input.GetKey(_crouchKey);
            if (_isCrouching)
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
            int originalCount = _touchedFloorObjects.Count;

            _touchedFloorObjects.Add(other.gameObject);

            if (originalCount != 0) return;

            HandleTouchingGround(other);
        }

        private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag("Floor")) return;

            _touchedFloorObjects.Remove(other.gameObject);

            if (_touchedFloorObjects.Count > 0) return;

            HandleLeavingGround(other);
        }

        private void HandleTouchingGround(Collision other)
        {
            _controllerMovementState = _controllerMovementState == PlayerControllerMovementState.SprintJumpApplied
                ? PlayerControllerMovementState.Sprinting
                : PlayerControllerMovementState.OnGround;
        }

        private void HandleLeavingGround(Collision other)
        {
            if (_controllerMovementState != PlayerControllerMovementState.OnGround &&
                _controllerMovementState != PlayerControllerMovementState.Sprinting &&
                _controllerMovementState != PlayerControllerMovementState.Walking) return;

            _controllerMovementState = _controllerMovementState == PlayerControllerMovementState.Sprinting
                ? PlayerControllerMovementState.SprintJumpApplied
                : PlayerControllerMovementState.JumpApplied;
            StartCoroutine(HandleJumpArc(true));
        }
    }
}