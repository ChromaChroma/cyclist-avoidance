using System;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ToolDropdownScript : MonoBehaviour
{
    private TMP_Dropdown _dropdown;
    [SerializeField] [CanBeNull] private GameObject _obstaclesView;
    private SpawnOnClick3D _spawnComponent;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.onValueChanged.AddListener(UpdateToolMode);
        _spawnComponent = GetComponent<SpawnOnClick3D>();
        _spawnComponent._requiredToolMode = ToolMode.Spawner;
        
        LoadItems();
    }

    private void UpdateToolMode(int newIndex)
    {
        var toolMode = _dropdown.options[newIndex].text;
        if (Enum.TryParse<ToolMode>(toolMode, out var mode))
        {
            ActiveMode.Mode = mode;

            switch (mode)
            {
                case ToolMode.None:
                case ToolMode.Spawner:
                    _obstaclesView?.SetActive(false);
                    
                    break;
                case ToolMode.Obstacles:
                    _obstaclesView?.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    

    private void LoadItems()
    {
        _dropdown.ClearOptions();

        foreach (ToolMode toolMode in Enum.GetValues(typeof(ToolMode)))
        {
            var option = new TMP_Dropdown.OptionData(toolMode.ToString());
            _dropdown.options.Add(option);
        }

        _dropdown.value = 0;
    }
}