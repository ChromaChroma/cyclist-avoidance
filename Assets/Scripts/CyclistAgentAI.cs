using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclistAgentAI : MonoBehaviour
{
    [Range(0.0f, 10.0f)] public float MaxSpeed = 1;
    [Range(0.0f, 60.0f)] public float MaxTurningRadius = 45;

    private float _currentSpeed; // [0, MaxSpeed]
    private float _currentTurnRadius; // [-MaxTurningRadius, MaxTurningRadius] 

    private float _forwardAmount = 0.1f;
    private float _turnAmount = 0.1f;

    private float speed;
    private float speedMax = 30f;
    private float speedMin = -50f;
    private float acceleration = 30f;
    private float brakeSpeed = 100f;
    private float reverseSpeed = 30f;

    private float idleSlowdown = 10f;
    
    private float turnSpeed;
    private float turnSpeedMax = 300f;
    private float turnSpeedAcceleration = 300f;

    private float turnIdleSlowdown = 500f;

    [SerializeField] private Transform targetPositionTransform;
    public Vector3 target;
    
    private void Update()
    {
        target = targetPositionTransform.position;

        
        Vector3 dirToMovePosition = (target - transform.position).normalized;
        float dot = Vector3.Dot(transform.up, dirToMovePosition);
        _forwardAmount = dot > 0 ? 1 : -1;

        float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.right);
        Debug.Log("AngleToDir: " + angleToDir);
        _turnAmount = angleToDir > 0 ? 1f : -1f;
        
        Move();

    }

    private void Move()
    {
        switch (_forwardAmount)
        {
            case > 0:
                // Accelerating
                speed += _forwardAmount * acceleration * Time.deltaTime;
                break;
            case < 0 when speed > 0:
                // Braking
                speed += _forwardAmount * brakeSpeed * Time.deltaTime;
                break;
            case < 0:
                // Reversing
                speed += _forwardAmount * reverseSpeed * Time.deltaTime;
                break;
            
            // Not accelerating or braking
            case 0 when speed > 0:
                speed -= idleSlowdown * Time.deltaTime;
                break;
            case 0 when speed < 0:
                speed += idleSlowdown * Time.deltaTime;
                break;
        }

        speed = Mathf.Clamp(speed, speedMin, speedMax);

        transform.position += transform.up * (Time.deltaTime * speed);
        // Debug.Log(speed);

        if (speed < 0)
        {
            // Going backwards, invert wheels
            _turnAmount *= -1f;
        }
        
        if (_turnAmount > 0 || _turnAmount < 0)
        {
            // Turning
            if ((turnSpeed > 0 && _turnAmount < 0) || (turnSpeed < 0 && _turnAmount > 0))
            {
                // Changing turn direction
                float minTurnAmount = 20f;
                turnSpeed = _turnAmount * minTurnAmount;
            }
        
            turnSpeed += _turnAmount * turnSpeedAcceleration * Time.deltaTime;
        }
        else
        {
            // Not turning
            if (turnSpeed > 0)
            {
                turnSpeed -= turnIdleSlowdown * Time.deltaTime;
            }
        
            if (turnSpeed < 0)
            {
                turnSpeed += turnIdleSlowdown * Time.deltaTime;
            }
        
            if (turnSpeed > -1f && turnSpeed < +1f)
            {
                // Stop rotating
                turnSpeed = 0f;
            }
        }
        
        Debug.Log("Turn Speed(1): " + turnSpeed);
        
        float speedNormalized = speed / speedMax;
        float invertSpeedNormalized = Mathf.Clamp(1 - speedNormalized, .75f, 1f);

        turnSpeed = Mathf.Clamp(turnSpeed, -turnSpeedMax, turnSpeedMax) * -1; // times -1 because z is inverted 
        Debug.Log("turn speed: " + turnSpeed);
        
        var turnRadius = turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad;
        transform.Rotate(0, 0, turnRadius);

        // carRigidbody.angularVelocity = new Vector3(0, turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad, 0);

        if (transform.eulerAngles.x > 2 || transform.eulerAngles.x < -2 || transform.eulerAngles.y > 2 ||
            transform.eulerAngles.y < -2)
        {
            transform.eulerAngles = new Vector3(0,0,  transform.eulerAngles.y);
        }
    }
}