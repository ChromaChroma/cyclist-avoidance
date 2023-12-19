using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNavMeshAi : MonoBehaviour
{
    public Transform goal;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (goal is not null) // Not sure this always runs After adding goal when Instantiating
        {
            _agent.destination = goal.position;
        }
    }
}