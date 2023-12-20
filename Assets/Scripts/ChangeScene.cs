using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    private Button _button;
    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
        _dropdown = GetComponentInChildren<TMP_Dropdown>();

        _button.onClick.AddListener(ChangeSceneFunc);
        LoadItems();
    }

    private void LoadItems()
    {
        _dropdown.ClearOptions();
        
        var currentSceneName = SceneManager.GetActiveScene().name;
        int currentSceneIndex = 0;
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            
            if (currentSceneName == sceneName) 
                currentSceneIndex = i;
            
            var option = new TMP_Dropdown.OptionData(sceneName);
            _dropdown.options.Add(option);
        }
        
        _dropdown.value = currentSceneIndex; 
    }

    private void ChangeSceneFunc()
    {
        var selectedScene = _dropdown.options[_dropdown.value].text;
        SceneManager.LoadScene(sceneName: selectedScene);
    }
}