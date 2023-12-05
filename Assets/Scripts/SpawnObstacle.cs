using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnObstacle : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    
    public GameObject obstacle;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                var point = raycastHit.point;
                point.y += obstacle.transform.localScale.y / 2;
                Instantiate(obstacle, point, Quaternion.identity);
            }
        }
    }
}
