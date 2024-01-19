using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemScript1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      //GameObject mijnGameObject = new GameObject("NieuwObject", typeof(SpriteRenderer));
      Vector3 startVector = new Vector3(-50,0,0);
      transform.position = startVector;
    }

    // Update is called once per frame
    void Update()
    {
      Vector3 startVector = new Vector3(-50,0,0);
      if (transform.position.x >= 50)
      {
        transform.position = startVector;
      }
      else
      {
        {
            transform.position = transform.position + new Vector3(0.3f, 0, 0);
        }
      }

    }
}
