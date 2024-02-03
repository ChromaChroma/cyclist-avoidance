using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnOnClick3D : MonoBehaviour
{
    [SerializeField] public GameObject _gameObject;
    [SerializeField] public ToolMode _requiredToolMode = ToolMode.Spawner;
    [SerializeField] [CanBeNull] public Action _onSpawnClick;
    [SerializeField] [CanBeNull] public Action _onDespawnClick;
    
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
            var gameObject = Instantiate(_gameObject);
            gameObject.transform.position = hit.point;
            
            // Manual Hack for if gameobject is spawner
            var comp = gameObject.GetComponent<BicycleSpawner>();
            if (comp is not null)
            {
                comp.goalObject = GameObject.Find("Goal");
            }
            
            // Lastly, run action
            _onSpawnClick?.Invoke();
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
            
            // Lastly, run action
            _onDespawnClick?.Invoke();
        }
    }
}