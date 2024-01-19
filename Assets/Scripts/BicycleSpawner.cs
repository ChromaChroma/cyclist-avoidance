using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class BicycleSpawner : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 10.0f)] [Tooltip("Time between spawning cyclists")]
    public float spawningSpeed = 1f;

    [SerializeField] [Tooltip("GameObject (Cyclist) to spawn")]
    public GameObject spawnObject;
    
    [SerializeField] [Tooltip("GameObject (Goal) to navigate the bicycles to")]
    public GameObject goalObject;

    private float _time;

    void Update()
    {
        _time += Time.deltaTime;
        if (_time >= spawningSpeed)
        {
            _time -= spawningSpeed;

            // Spawn Cyclist
            var spawnedCyclist = Instantiate(spawnObject);
            Cyclists.cyclistList.Add(spawnedCyclist);
            spawnedCyclist.transform.position = transform.position;
            spawnedCyclist.transform.rotation = transform.rotation;
            
            var cyclistAi = spawnedCyclist.GetComponent<SimpleNavMeshAi>();
            cyclistAi.goal = goalObject.transform;

            // Add cyclist to collective list of agents (for collision checks and nearest neighbours)

        }
    }
}