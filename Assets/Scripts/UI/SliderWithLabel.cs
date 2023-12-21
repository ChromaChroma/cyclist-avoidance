using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class SliderWithLabel : MonoBehaviour
{
    // Slider value fields for inspector
    [SerializeField] private string _baseText;
    [SerializeField] private float _min;
    [SerializeField] private float _max;
    [SerializeField] private float _default;
    
    // Event fields for slider in inspector
    [SerializeField] private UnityEvent _initialize = new();
    [SerializeField] private Slider.SliderEvent _onValueChanged = new();
    
    // Child components in the SliderWithLabel
    private Slider _slider;
    private TMP_Text _labelText;
    
    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
        _labelText = GetComponentInChildren<TMP_Text>();
        
        _onValueChanged.AddListener(UpdateSliderLabel);
        _slider.onValueChanged = _onValueChanged;

        _slider.minValue = _min;
        _slider.maxValue = _max;
        _slider.value = _default;

        UpdateSliderLabel(_slider.value);
        
        _initialize.Invoke();
    }

    private void UpdateSliderLabel(float currentSpeed) => _labelText.text = $"{_baseText} [{currentSpeed:F1}]";
    
    public float CurrentValue() => _slider.value;
}