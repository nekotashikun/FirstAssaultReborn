using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GamePlayerController : MonoBehaviour
{
    [SerializeField] 
    private Rigidbody _playerRb;

    [SerializeField] 
    private float _walkingSpeed;
    
    private Vector3 _inputDirection;
    private Vector3 _previousInputDirection;

    private void FixedUpdate()
    {
        Debug.Log(_inputDirection);
        if (_inputDirection == Vector3.zero)
        {
            _previousInputDirection = _inputDirection;
            return;
        }

        float resultDirectionSpeed =
            (1 / (Mathf.Abs(_inputDirection.x) + Mathf.Abs(_inputDirection.z))) * _walkingSpeed;
        
        _playerRb.AddRelativeForce(_inputDirection * resultDirectionSpeed, ForceMode.VelocityChange);
        
        _previousInputDirection = _inputDirection;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _inputDirection.z = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _inputDirection.z = -1f;
        }
        else
        {
            _inputDirection.z = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _inputDirection.x = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _inputDirection.x = -1f;
        }
        else
        {
            _inputDirection.x = 0;
        }
    }
}
