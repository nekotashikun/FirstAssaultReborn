﻿using UnityEngine;

namespace Player
{
    public class GamePlayerController : MonoBehaviour
    {
        [Header("Controller Dependencies"), SerializeField]
        private Rigidbody _playerRb;

        [SerializeField]
        private Camera _playerCamera;

        [Header("Speed settings"), SerializeField]
        private float _walkingSpeed = 5f;

        [SerializeField]
        private float _jumpPower = 5f;

        [SerializeField]
        private float _sprintMultiplier = 1.5f;

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


        private PlayerControllerState _controllerState;
        private Vector3 _inputDirection;
        private Vector3 _previousVelocity;
        private Quaternion _velocityDirection;

        private void FixedUpdate()
        {
            float finalWalkingSpeed = _walkingSpeed;
            switch (_controllerState)
            {
                case PlayerControllerState.SprintJumpApplied:
                case PlayerControllerState.OnGroundSprinting:
                    finalWalkingSpeed *= _sprintMultiplier;
                    break;

                case PlayerControllerState.Jumping:
                case PlayerControllerState.SprintJumping:

                    _controllerState = _controllerState == PlayerControllerState.SprintJumping
                        ? PlayerControllerState.SprintJumpApplied
                        : PlayerControllerState.JumpApplied;
                    _playerRb.AddRelativeForce(0, _jumpPower, 0, ForceMode.VelocityChange);
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
            _previousVelocity = _playerRb.velocity;
        }

        // Update is called once per frame
        private void Update()
        {
            float mouseInputX = Input.GetAxis("Mouse X");
            float mouseInputY = !_invertMouseY ? -Input.GetAxis("Mouse Y") : Input.GetAxis("Mouse Y");

            _playerCamera.transform.Rotate(mouseInputY * _mouseSensitivity, 0, 0);
            transform.Rotate(0, mouseInputX * _mouseSensitivity, 0);


            if (_controllerState != PlayerControllerState.OnGround && _controllerState != PlayerControllerState.OnGroundSprinting) return;

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

            _controllerState = Input.GetKey(_sprintKey)
                ? PlayerControllerState.OnGroundSprinting
                : PlayerControllerState.OnGround;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _controllerState = _controllerState == PlayerControllerState.OnGroundSprinting
                    ? PlayerControllerState.SprintJumping
                    : PlayerControllerState.Jumping;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Floor"))
                _controllerState = _controllerState == PlayerControllerState.SprintJumpApplied
                    ? PlayerControllerState.OnGroundSprinting
                    : PlayerControllerState.OnGround;
        }
    }
}