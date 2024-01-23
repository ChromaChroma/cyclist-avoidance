using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using CollisionAvoidance;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
            // _agent.updatePosition = false;
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

            var xScaled = Mathf.Cos((float)currentRadian);
            var zScaled = (float)Math.Sin(currentRadian);

            var x = xScaled * radius + pos.x;
            var z = zScaled * radius + pos.z;

            Vector3 currentPos = new Vector3(x, 2, z);
            lineRenderer.SetPosition(currentStep, currentPos);
        }
    }

    private void Update()
    {
        var curPos = transform.position;
        if (Vector3.Distance(goal.transform.position, curPos) <= _goalLocationOffset)
        {
            // Is at goal destination
            Cyclists.cyclistList.Remove(gameObject);
            Destroy(gameObject);
        }
        else
        {
            // Run Collision Avoidance using desired velocity
            var movementVector = _avoidanceAlgorithm.AvoidCollisions(gameObject, _agent.desiredVelocity);
            var m = movementVector;
            // While on navmesh prefer to push to the right
            if (_agent.isOnNavMesh && _agent.FindClosestEdge(out var h))
            {
                // Check if closest edge is on right side of agent, based on angle
                var onRight = 0 < Vector3.SignedAngle(movementVector, h.position - transform.position, Vector3.up);
                var angle = onRight ? 30f : Math.Min(30f, 90f * h.distance);
                movementVector = Quaternion.AngleAxis(angle, Vector3.up) * movementVector;
            }
            
            // Move based on movement vector and CA
            _agent.Move(movementVector * (Time.deltaTime * 0.1f));
            
            // Rotate Cyclist roughly to face next move's position
            Vector3 direction = (_agent.nextPosition - curPos).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);
        }
        
        if (ShowRadii)
        {
            DrawCircle(breakCircleRenderer, _avoidanceAlgorithm.BrakeRangeRadius);
            DrawCircle(steerCircleRenderer, _avoidanceAlgorithm.SteerRangeRadius);
        }
    }
}