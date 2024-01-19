using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclistAi : MonoBehaviour
{
    
    [SerializeField] private Transform targetPositionTransform;
    private Vector3 target;

    private float _speed;
    private float _turnAmount;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpeed();

       
        Move();
    }

    private void UpdateSpeed()
    {
        Vector3 dirToMovePosition = (target - transform.position).normalized;
        float dot = Vector3.Dot(transform.up, dirToMovePosition);
        _speed = dot > 0 ? 1 : -1;
        
        float angleToDir = Vector3.SignedAngle(transform.up, dirToMovePosition, Vector3.up);
        Debug.Log("AngleToDir: " + angleToDir);
        if (angleToDir > 0)
        {
            
        }
        _turnAmount = angleToDir > 0 ? 1f : -1f; // Due to left being - on the y
        
        
        
    }

    private void Move()
    {
        transform.position += transform.up * (Time.deltaTime * _speed);

        float turnSpeedAcceleration = 300f;
        float turnIdleSlowdown = 500f;
        var speedMax = 30f;
        var speed = _speed;
        var turnSpeed = 30f;
        var turnSpeedMax = 300f;
        
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
        float speedNormalized = speed / speedMax;
        float invertSpeedNormalized = Mathf.Clamp(1 - speedNormalized, .75f, 1f);
        //
        // Debug.Log("turn speed: " + invertSpeedNormalized);
        // Debug.Log("turn speed: " + _turnAmount);
        //
        turnSpeed = Mathf.Clamp(_turnAmount, -turnSpeedMax, turnSpeedMax); 
        // Debug.Log("turn speed: " + turnSpeed);
        
        var turnRadius = turnSpeed * (invertSpeedNormalized * 1f) ;
        transform.Rotate(0, 0, turnRadius );

        // carRigidbody.angularVelocity = new Vector3(0, turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad, 0);

        // if (transform.eulerAngles.x > 2 || transform.eulerAngles.x < -2 || transform.eulerAngles.y > 2 ||
        //     transform.eulerAngles.y < -2)
        // {
        //     transform.eulerAngles = new Vector3(0,0,  transform.eulerAngles.y);
        // }
    }
}
