using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public GameObject obstacle;

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            AddObstacle();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RemoveObstacle();
        }
        
    }

    void MoveObstacle()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit)
            && raycastHit.collider.tag.Equals("Obstacle"))
        {
            var point = raycastHit.point;
            var obj = raycastHit.collider.gameObject;
            point.y -= obj.transform.position.y / 2;
            obj.transform.position = point;
        }
    }

    void AddObstacle()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit)
            && !raycastHit.collider.tag.Equals("Obstacle"))
        {
            var point = raycastHit.point;
            point.y += obstacle.transform.localScale.y / 2;
            Instantiate(obstacle, point, Quaternion.identity);
        }
    }

    void RemoveObstacle()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit raycastHit)
            && raycastHit.collider.tag.Equals("Obstacle"))
        {
            Destroy(raycastHit.collider.gameObject);
        }
    }
}