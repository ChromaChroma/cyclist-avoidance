using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNavMeshAi : MonoBehaviour
{
    public Transform goal;

    private NavMeshAgent _agent;
    private MeshRenderer _meshRenderer;
    private float _goalLocationOffset;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        if (goal is not null) // Not sure this always runs After adding goal when Instantiating
        {
            _agent.destination = goal.position;
            var bounds = goal.GetComponent<MeshRenderer>().bounds;
            _goalLocationOffset = Math.Min(bounds.size.x,bounds.size.z);
        }
    }

    private void Update()
    {
        var goalPos = goal.transform.position;
        if (Vector3.Distance(goalPos, transform.position) <= _goalLocationOffset )
        {
            Destroy(gameObject);
        }
    }
}