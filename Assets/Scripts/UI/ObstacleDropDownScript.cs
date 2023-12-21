using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleDropDownScript : MonoBehaviour
{
    private TMP_Dropdown _dropdown;
    private SpawnOnClick3D _spawnComponent;
    private GameObject[] _obstacles;
    private NavMeshSurface _navMeshSurface;
    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        _navMeshSurface = FindObjectOfType<NavMeshSurface>();
        _spawnComponent = GetComponent<SpawnOnClick3D>();
            
        _spawnComponent._onSpawnClick = () =>_navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
        _spawnComponent._onDespawnClick = () =>_navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
        
        _dropdown.onValueChanged.AddListener(UpdateObstacle);
        
        _obstacles = Resources.LoadAll("Obstacles", typeof(GameObject))
            .Select(o => (GameObject)o)
            .ToArray();
        
        LoadOptions();
    }
    
    private void LoadOptions()
    {
        _dropdown.ClearOptions();
        
        foreach (var obstacle in _obstacles)
        {
            var option = new TMP_Dropdown.OptionData(obstacle.name);
            _dropdown.options.Add(option);
        }

        _dropdown.value = 0;
        _spawnComponent._gameObject = _obstacles[0];
    }


    private void UpdateObstacle(int index)
    {
        _spawnComponent._gameObject = _obstacles[index];
    }
}
