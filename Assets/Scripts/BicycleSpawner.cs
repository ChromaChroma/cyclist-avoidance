using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class BicycleSpawner : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 10.0f)] [Tooltip("Time between spawning cyclists")]
    private float spawningSpeed = 1f;

    [SerializeField] [Tooltip("GameObject (Cyclist) to spawn")]
    public GameObject spawnObject;

    private float _time;

    void Update()
    {
        _time += Time.deltaTime;
        if (_time >= spawningSpeed)
        {
            _time -= spawningSpeed;

            // Spawn Cyclist
            var spawnedCyclist = Instantiate(spawnObject);
            spawnedCyclist.transform.position = transform.position;
            
            var cyclistAi = spawnedCyclist.GetComponent<SimpleNavMeshAi>();
            cyclistAi.goal = GameObject.Find("Goal").transform;

            // Add cyclist to collective list of agents (for collision checks and nearest neighbours)

        }
    }
}