using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class SpawnOnClick3D : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private ToolMode _requiredToolMode = ToolMode.Spawner;

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() || ActiveMode.Mode != _requiredToolMode)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Spawn();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Despawn();
        }
    }

    private void Spawn()
    {
        var screenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit)) // Check hit on backdrop only
        {
            var p = hit.point;
            p.y += _gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2; // Set height to be flush on the ground

            var spawner = Instantiate(_gameObject); 
            spawner.GetComponent<BicycleSpawner>().goalObject = GameObject.Find("Goal");
            spawner.transform.position = p;
        }
    }

    private void Despawn()
    {
        var screenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100)
            && hit.collider.gameObject.name
                .StartsWith(_gameObject.name)) // Check hit on same type as GameObject spawned
        {
            Destroy(hit.collider.gameObject);
        }
    }
}