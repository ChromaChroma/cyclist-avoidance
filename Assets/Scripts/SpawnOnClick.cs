using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class SpawnOnClick : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private ToolMode _requiredToolMode = ToolMode.Spawner;
    
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0) && ActiveMode.Mode == _requiredToolMode)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        var screenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100, 1<<6)) // Check hit on backdrop only
        {
            var spawner = Instantiate(_gameObject);
            var p = hit.point;
            p.z = _gameObject.transform.position.z; // Set depth to same as original
            spawner.transform.position = p;
        }
    }
}
