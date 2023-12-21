using UnityEngine;

public class SpawnSpeedScript : MonoBehaviour
{
   public void Init(SliderWithLabel comp)
   {
      foreach (var spawner in GameObject.FindGameObjectsWithTag("Spawner"))
      {
          spawner.GetComponent<BicycleSpawner>().spawningSpeed = comp.CurrentValue();
      }
   }

   public void UpdateSpawnSpeed(float speed)
   {
       foreach (var spawner in GameObject.FindGameObjectsWithTag("Spawner"))
       {
           spawner.GetComponent<BicycleSpawner>().spawningSpeed = speed;
       }
   }
}