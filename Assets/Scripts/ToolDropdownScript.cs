using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolDropdownScript : MonoBehaviour
{
    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.onValueChanged.AddListener(UpdateToolMode);
        LoadItems();
    } 
    
    private void UpdateToolMode(int newIndex)
    {
        var toolMode = _dropdown.options[newIndex].text;
        if (Enum.TryParse<ToolMode>(toolMode, out var mode))
        {
            ActiveMode.Mode = mode;
        }
    }

    private void LoadItems()
    {
        _dropdown.ClearOptions();

        foreach (ToolMode toolMode in Enum.GetValues(typeof(ToolMode)))
        {
            Debug.Log(toolMode.ToString());
            var option = new TMP_Dropdown.OptionData(toolMode.ToString());
            _dropdown.options.Add(option);
        }

        _dropdown.value = 0;
    }
}