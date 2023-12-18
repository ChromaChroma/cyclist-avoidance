using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class cyclingBehaviour : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public Transform circle;

    public Vector2 direction = new Vector2(0,1);
    public float accelerationFactor = 38f;
    public float turnFactor = 3.5f;

    // public int Spawnbike = 0;
    public bool Spawn = false;
    public Button yourButton;


    // Start is called before the first frame update
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick(){
		Debug.Log ("You have clicked the button!");
        Spawn = true;
	}

    // Update is called once per frame
    void Update()
    {
        //myRigidBody.velocity = direction * 10;
    }

    void FixedUpdate()
    {
        // Spawnbike = !Spawnbike;
        if(Spawn){
            ApplyDriveForce();
            if(Vector2.Distance(myRigidBody.position, circle.position) <= 5)
            {
                myRigidBody.AddForce(new Vector2(0,1));
            }
        }
    }

    void ApplyDriveForce()
    {
        float rotationAngle = 1f;
        myRigidBody.MoveRotation(rotationAngle);
        myRigidBody.AddForce(new Vector2(1,0));
    }

    
}
