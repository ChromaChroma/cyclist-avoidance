using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPauseScript : MonoBehaviour
{
    private Button _button;
    private Image _buttonImage;
    private bool _active;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _buttonImage = _button.GetComponent<Image>();
        _button.onClick.AddListener(PlayPause);

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

    private void PausedButton() => _buttonImage.color = Color.grey;
    
    private void PlayingButton() => _buttonImage.color = Color.white;
}