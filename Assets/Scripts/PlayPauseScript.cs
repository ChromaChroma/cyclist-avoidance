using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPauseScript : MonoBehaviour
{
    public Button button;
    private bool _active;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayPause);

        _active = Time.timeScale != 0;
        if (_active)
        {
            PlayingButton();
        }
        else
        {
            PausedButton();
        }
    }

    private void PlayPause()
    {
        if (_active)
        {
            Time.timeScale = 0;
            _active = !_active;
            PausedButton();
        }
        else
        {
            Time.timeScale = 1;
            _active = !_active;
            PlayingButton();
        }
        
    }

    private void PausedButton()
    {
        button.GetComponentInChildren<TMP_Text>().text = "Paused";
        button.GetComponent<Image>().color = Color.grey;
    }

    private void PlayingButton()
    {
        button.GetComponentInChildren<TMP_Text>().text = "Playing";
        button.GetComponent<Image>().color = Color.white;
    }
}
