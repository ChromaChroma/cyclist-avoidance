using UnityEngine;
using UnityEngine.UI;

public class SpawnSpeedScript : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    public void Initialize()
    {
        var currentValue = _slider.value;
        foreach (var spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            spawner.GetComponent<BicycleSpawner>().spawningSpeed = currentValue;
        }
    }

    public void UpdateSpawnSpeed()
    {
        foreach (var spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            spawner.GetComponent<BicycleSpawner>().spawningSpeed = _slider.value;
        }
    }
}