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

    private void FixedUpdate()
    {
        if (_inputDirection == Vector3.zero)
        {
            _playerRb.velocity = new Vector3(0, _playerRb.velocity.y, 0);
            return;
        }

        float resultDirectionSpeed = (1 / (Mathf.Abs(_inputDirection.x) + Mathf.Abs(_inputDirection.z))) * _walkingSpeed;
        
        _playerRb.velocity = new Vector3(_inputDirection.x * resultDirectionSpeed, _playerRb.velocity.y, _inputDirection.z * resultDirectionSpeed);
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
