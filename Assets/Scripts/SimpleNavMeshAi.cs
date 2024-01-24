using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNavMeshAi : MonoBehaviour
{
    public Transform goal;

    private NavMeshAgent _agent;
    private float _goalLocationOffset;
    private List<Vector3> _destinations = new List<Vector3>();

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (goal is not null) // Not sure this always runs After adding goal when Instantiating
        {
            var distZroad = Math.Abs(_agent.transform.position.x - 0.29); //distance from Z-road
            var distXroad = Math.Abs(_agent.transform.position.z - 1.25); //distance from X-road

            if (distXroad < distZroad)
            {
                _destinations.Add(new Vector3(goal.transform.position.x, goal.transform.position.y, _agent.transform.position.z));
            }
            else
            {
                _destinations.Add(new Vector3(_agent.transform.position.x, goal.transform.position.y, goal.transform.position.z));
            }

            _destinations.Add(goal.transform.position);


            //_agent.agentTypeID = 1;
            _agent.radius = 0.5f;
            _agent.destination = _destinations[0];
            var bounds = goal.GetComponent<MeshRenderer>().bounds;
            _goalLocationOffset = Math.Min(bounds.size.x,bounds.size.z);
        }
    }

    private void Update()
    {
        if (Vector3.Distance(_agent.destination, _agent.transform.position) < _goalLocationOffset )
        {
            if (_destinations.Count == 1) Destroy(gameObject);
            else
            {
                _destinations.RemoveAt(0);
                _agent.SetDestination(_destinations[0]);
            }
        }
    }
}