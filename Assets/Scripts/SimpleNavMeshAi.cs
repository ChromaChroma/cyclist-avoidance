using System;
using System.Collections;
using System.Collections.Generic;
using CollisionAvoidance;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNavMeshAi : MonoBehaviour
{
    public Transform goal;

    private NavMeshAgent _agent;
    private float _goalLocationOffset;
    private CollisionAvoidanceAlgorithm _avoidanceAlgorithm;

    private void Awake()
    {
        _avoidanceAlgorithm = new();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (goal is not null) // Not sure this always runs After adding goal when Instantiating
        {
            _agent.destination = goal.position;
            var bounds = goal.GetComponent<MeshRenderer>().bounds;
            _goalLocationOffset = Math.Min(bounds.size.x,bounds.size.z);
            // _agent.updatePosition = false;
        }
    }

    private void Update()
    {
        if (Vector3.Distance(goal.transform.position, transform.position) <= _goalLocationOffset )
        {
            Cyclists.cyclistList.Remove(gameObject);
            Destroy(gameObject);
        }   
        
        // var movementVector = _avoidanceAlgorithm.AvoidCollisions(gameObject, _agent.velocity);
        // Debug.Log($"Old: {_agent.velocity}, new: {movementVector}");
        _agent.velocity = _agent.velocity;
        
        // gameObject.transform.position += movementVector;
        // _agent.nextPosition = gameObject.transform.position += movementVector;
        Debug.Log($"oldPos: {gameObject.transform.position}, newPos: {gameObject.transform.position + _avoidanceAlgorithm.AvoidCollisions(gameObject, _agent.velocity)}");
    }
}