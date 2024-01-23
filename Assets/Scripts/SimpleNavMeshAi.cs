using System;
using CollisionAvoidance;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class SimpleNavMeshAi : MonoBehaviour
{
    public Transform goal;
    private float _goalLocationOffset;
    private NavMeshAgent _agent;
    // CA
    private CollisionAvoidanceAlgorithm _avoidanceAlgorithm;
    
    // Show Radii fields
    private bool _showRadii;

    public bool ShowRadii
    {
        get => _showRadii;
        set 
        {
            breakCircleRenderer.enabled = value;
            steerCircleRenderer.enabled = value;
            _showRadii = value;
        }
    }
    [SerializeField] private LineRenderer breakCircleRenderer;
    [SerializeField] private LineRenderer steerCircleRenderer;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _avoidanceAlgorithm = new CollisionAvoidanceAlgorithm(_agent)
        {
            AutoUpdateRadii = true
        };
    }

    private void Start()
    {
        if (goal is not null)
        {
            _agent.destination = goal.position;
            var bounds = goal.GetComponent<MeshRenderer>().bounds;
            _goalLocationOffset = Math.Min(bounds.size.x, bounds.size.z);
            _agent.updatePosition = false;
            _agent.autoBraking = false; //Simulation does not really require single goal agents to stop slowly.
        }
    }

    private const int Steps = 100;
    private void DrawCircle(LineRenderer lineRenderer, float radius)
    {
        var pos = gameObject.transform.position;
        lineRenderer.positionCount = Steps;

        for (int currentStep = 0; currentStep < Steps; currentStep++)
        {
            var circumferenceProgress = (float)currentStep / Steps;

            var currentRadian = circumferenceProgress * 2 * Math.PI;

            var xScaled = Mathf.Cos((float) currentRadian);
            var zScaled = (float)Math.Sin(currentRadian);
            
            var x = xScaled * radius + pos.x;
            var z = zScaled * radius + pos.z;

            Vector3 currentPos = new Vector3(x, 2, z);
            lineRenderer.SetPosition(currentStep, currentPos);
        }
    }

    private void Update()
    {
        Debug.Log($"Active: {breakCircleRenderer.enabled}Ac2: {steerCircleRenderer.enabled}, s: {_showRadii}");
        if (Vector3.Distance(goal.transform.position, transform.position) <= _goalLocationOffset)
        {
            // Is at goal destination
            Cyclists.cyclistList.Remove(gameObject);
            Destroy(gameObject);
        }
        else
        {
            var movementVector = _avoidanceAlgorithm.AvoidCollisions(gameObject, _agent.desiredVelocity);
            var trans = gameObject.transform;
            var newPos = trans.position + movementVector * Time.deltaTime;
            _agent.nextPosition = newPos;
            trans.position = newPos;
            Debug.Log($"m:{ShowRadii}");
            if (ShowRadii)
            {
                Debug.Log($"brake: {_avoidanceAlgorithm.BrakeRangeRadius}, ster:{_avoidanceAlgorithm.SteerRangeRadius}");
                DrawCircle(breakCircleRenderer, _avoidanceAlgorithm.BrakeRangeRadius);
                DrawCircle(steerCircleRenderer, _avoidanceAlgorithm.SteerRangeRadius);
            }
        }
    }
}