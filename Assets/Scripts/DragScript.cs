using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragScript : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    private bool isFollowing = false;

    private void Update()
    {
        if (isFollowing)
        {
            targetTransform.position = GetMousePosition();
        }

        if (Input.GetMouseButtonDown(0))
        {
            isFollowing = !isFollowing;
        }
    }

    private Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
        {
            Debug.Log(raycastHit.point);
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
