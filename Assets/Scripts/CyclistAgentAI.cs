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

    public float Speed => _currentSpeed;

    [SerializeField] private Transform targetPositionTransform;
    public Vector3 target;

    public void SetInputs(float forwardAmount, float turnAmount)
    {
    }


    private void Awake()
    {
        // cardriver = GetComponent<BikeAgent>;
    }

    private void Update()
    {
        target = targetPositionTransform.position;

        Move();
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

        switch (_turnAmount)
        {
            case > 0 or < 0 when (turnSpeed > 0 && _turnAmount < 0) || (turnSpeed < 0 && _turnAmount > 0):
                // Changing turn direction
                float min_turnAmount = 20f;
                turnSpeed = _turnAmount * min_turnAmount;
                break;

            case { } when turnSpeed > 0:
                turnSpeed -= turnIdleSlowdown * Time.deltaTime;
                break;

            case { } when turnSpeed < 0:
                turnSpeed += turnIdleSlowdown * Time.deltaTime;
                break;

            case { } when turnSpeed is > -1f and < +1f:
                // Stop rotating
                turnSpeed = 0f;
                break;
        }
        
        float speedNormalized = speed / speedMax;
        float invertSpeedNormalized = Mathf.Clamp(1 - speedNormalized, .75f, 1f);

        turnSpeed = Mathf.Clamp(turnSpeed, -turnSpeedMax, turnSpeedMax);

        transform.Rotate(new Vector3(turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad, 0, 0));
        Debug.Log(turnSpeed);
        Debug.Log(transform.rotation);

        // carRigidbody.angularVelocity = new Vector3(0, turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad, 0);

        // if (transform.eulerAngles.x > 2 || transform.eulerAngles.x < -2 || transform.eulerAngles.z > 2 ||
        //     transform.eulerAngles.z < -2)
        // {
        //     transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        // }
    }

    private void Move()
    {
        Vector3 dirToMovePosition = (target - transform.position).normalized;
        float dot = Vector3.Dot(transform.up, dirToMovePosition);
        // Debug.Log(dot);

        _forwardAmount = dot > 0 ? 1 : -1;

        float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);

        _turnAmount = angleToDir > 0 ? 1f : -1f;
        Debug.Log(angleToDir);

        SetInputs(_forwardAmount, _turnAmount);
    }


    // #region Fields
    private float speed;
    private float speedMax = 70f;
    private float speedMin = -50f;
    private float acceleration = 30f;
    private float brakeSpeed = 100f;
    private float reverseSpeed = 30f;

    private float idleSlowdown = 10f;

    //
    private float turnSpeed;
    private float turnSpeedMax = 300f;
    private float turnSpeedAcceleration = 300f;

    private float turnIdleSlowdown = 500f;
    //
    // private float forwardAmount;
    // private float turnAmount;
    //
    // private Rigidbody carRigidbody;
    // #endregion

    // private void Update()
    // {
    //     if (forwardAmount > 0)
    //     {
    //         // Accelerating
    //         speed += forwardAmount * acceleration * Time.deltaTime;
    //     }
    //
    //     if (forwardAmount < 0)
    //     {
    //         if (speed > 0)
    //         {
    //             // Braking
    //             speed += forwardAmount * brakeSpeed * Time.deltaTime;
    //         }
    //         else
    //         {
    //             // Reversing
    //             speed += forwardAmount * reverseSpeed * Time.deltaTime;
    //         }
    //     }
    //
    //     if (forwardAmount == 0)
    //     {
    //         // Not accelerating or braking
    //         if (speed > 0)
    //         {
    //             speed -= idleSlowdown * Time.deltaTime;
    //         }
    //
    //         if (speed < 0)
    //         {
    //             speed += idleSlowdown * Time.deltaTime;
    //         }
    //     }
    //
    //     speed = Mathf.Clamp(speed, speedMin, speedMax);
    //
    //
    //     carRigidbody.velocity = transform.forward * speed;
    //
    //     if (speed < 0)
    //     {
    //         // Going backwards, invert wheels
    //         turnAmount = turnAmount * -1f;
    //     }
    //
    //     if (turnAmount > 0 || turnAmount < 0)
    //     {
    //         // Turning
    //         if ((turnSpeed > 0 && turnAmount < 0) || (turnSpeed < 0 && turnAmount > 0))
    //         {
    //             // Changing turn direction
    //             float minTurnAmount = 20f;
    //             turnSpeed = turnAmount * minTurnAmount;
    //         }
    //
    //         turnSpeed += turnAmount * turnSpeedAcceleration * Time.deltaTime;
    //     }
    //     else
    //     {
    //         // Not turning
    //         if (turnSpeed > 0)
    //         {
    //             turnSpeed -= turnIdleSlowdown * Time.deltaTime;
    //         }
    //
    //         if (turnSpeed < 0)
    //         {
    //             turnSpeed += turnIdleSlowdown * Time.deltaTime;
    //         }
    //
    //         if (turnSpeed > -1f && turnSpeed < +1f)
    //         {
    //             // Stop rotating
    //             turnSpeed = 0f;
    //         }
    //     }
    //
    //     float speedNormalized = speed / speedMax;
    //     float invertSpeedNormalized = Mathf.Clamp(1 - speedNormalized, .75f, 1f);
    //
    //     turnSpeed = Mathf.Clamp(turnSpeed, -turnSpeedMax, turnSpeedMax);
    //
    //     carRigidbody.angularVelocity = new Vector3(0, turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad, 0);
    //
    //     if (transform.eulerAngles.x > 2 || transform.eulerAngles.x < -2 || transform.eulerAngles.z > 2 ||
    //         transform.eulerAngles.z < -2)
    //     {
    //         transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    //     }
    // }
    //
    // public void StopCompletely()
    // {
    //     speed = 0f;
    //     turnSpeed = 0f;
    // }
}