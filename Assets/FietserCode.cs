using System;
using UnityEngine;
using UnityEngine.AI;

public class FietserController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public NavMeshAgent other;
    public Vector3 startLoc;
    public Vector3 endLoc;

    void Start()
    {
        startLoc = agent.transform.position;
        endLoc = startLoc;


        //the following only works for bikers starting on either x=0 or z=0:
        endLoc.x = -endLoc.x;
        endLoc.z = -endLoc.z;


        //de case waar ze allebei naar rechts gaan:
        //float xCoord = endLoc.x;
        //endLoc.x = endLoc.z;
        //endLoc.z = xCoord;
    }
    // Update is called once per frame
    void Update()
    {
        if (agent.transform.position == startLoc)
            agent.SetDestination(endLoc);
        if (agent.transform.position == endLoc)
            agent.SetDestination(startLoc);

        //dom idee:
        //if ((agent.transform.position - other.transform.position).magnitude <= 20)
        //{
        //    var rotation = Quaternion.AngleAxis(1, Vector3.up);

        //    Vector3 fiets1V = agent.velocity;
        //    Vector3 fiets2V = other.velocity;

        //    agent.velocity = Vector3.RotateTowards(fiets1V,fiets2V,(float)Math.PI/360,10);//rotation * fiets1V;
        //    other.velocity = Vector3.RotateTowards(fiets2V,fiets1V,(float)Math.PI/360,10);//rotation * fiets2V;
        //}
    }


}
