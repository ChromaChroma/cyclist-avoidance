using UnityEngine;
using UnityEngine.UI;

public class AgentRadiiToggle : MonoBehaviour
{
    private Button _button;
    private Image _buttonImage;
    private bool _active;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _buttonImage = _button.GetComponent<Image>();
        _button.onClick.AddListener(Toggle);
        ToggledOff();
    }


    private void Toggle()
    {
        if (_active)
        {
            SetAgentsRadii(false);
            ToggledOff();
        }
        else
        {
            SetAgentsRadii(true);
            ToggledOn();
        }
        _active = !_active;
    }

    private void SetAgentsRadii(bool active)
    {
        foreach (var cyclist in Cyclists.cyclistList)
        {
            cyclist.GetComponent<SimpleNavMeshAi>().ShowRadii = active;
        }

        foreach (var spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            spawner.GetComponent<BicycleSpawner>().radiiActive = active;
        }
    }

    private void ToggledOff() => _buttonImage.color = Color.grey;
    
    private void ToggledOn() => _buttonImage.color = Color.white;
}