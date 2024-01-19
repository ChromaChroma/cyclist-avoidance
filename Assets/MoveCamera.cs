using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    float inputX, inputZ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        if(inputX != 0) {
            rotate();
        }
        if(inputZ != 0) {
            move();
        }
    if ( Input.GetKeyDown(KeyCode.Alpha1) == true ){
        transform.position = new Vector3(0,50,3);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
    if ( Input.GetKeyDown(KeyCode.Alpha2) == true ){
        transform.position = new Vector3(25,40,-5);
        transform.rotation = Quaternion.Euler(60f, 290f, 0f);
    }
    if ( Input.GetKeyDown(KeyCode.Alpha3) == true ){
        transform.position = new Vector3(14,2,-3);
        transform.rotation = Quaternion.Euler(0f, 300f, 0f);
    }


    }

    private void rotate()
    {
        transform.Rotate(new Vector3(0f, inputX * Time.deltaTime * 2, 0f));
    }

    private void move()
    {
        transform.position += transform.forward * inputZ * Time.deltaTime * 2;
    }


}
