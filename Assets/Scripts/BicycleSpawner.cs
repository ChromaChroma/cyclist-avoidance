using UnityEngine;

public class BicycleSpawner : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 10.0f)] [Tooltip("Time between spawning cyclists")]
    public float spawningSpeed = 1f;

    [SerializeField] [Tooltip("GameObject (Cyclist) to spawn")]
    public GameObject spawnObject;
    
    [SerializeField] [Tooltip("GameObject (Goal) to navigate the bicycles to")]
    public GameObject goalObject;

    private float _time;

    public bool RadiiActive;

    void Update()
    {
        _time += Time.deltaTime;
        if (_time >= spawningSpeed)
        {
            _time -= spawningSpeed;

            // Spawn Cyclist
            var spawnedCyclist = Instantiate(spawnObject);
            spawnObject.GetComponent<SimpleNavMeshAi>().ShowRadii = RadiiActive;
            Debug.Log($"Active?: {RadiiActive}");
            Cyclists.cyclistList.Add(spawnedCyclist);
            spawnedCyclist.transform.position = transform.position;
            spawnedCyclist.transform.rotation = transform.rotation;
            
            var cyclistAi = spawnedCyclist.GetComponent<SimpleNavMeshAi>();
            cyclistAi.goal = goalObject.transform;

            // Add cyclist to collective list of agents (for collision checks and nearest neighbours)

        }
    }
}