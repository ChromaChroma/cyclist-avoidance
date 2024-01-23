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

        //idee:
        if ((agent.transform.position - other.transform.position).magnitude <= 20)
        {

            Vector3 fiets1V = agent.velocity;
            Vector3 fiets2V = other.velocity;

            Vector3 rotateRight = Vector3.Cross(fiets1V,Vector3.up);
            Vector3 sForce; //steering force
            Vector3 bForce; //braking force


            if (Vector3.Dot(fiets2V-fiets1V, rotateRight) > 0) //if the distance vector between the two cyclists is in the same direction as the velocity turned 90 degrees to the right
            {
                bForce = -fiets1V * .1f;
            }
            else
            {
                bForce = Vector3.zero;
            }
            

            agent.velocity = agent.velocity + bForce;//rotation * fiets1V;

            rotateRight = Vector3.Cross(fiets2V, Vector3.up);

            if (Vector3.Dot(fiets1V - fiets2V, rotateRight) > 0) //if the distance vector between the two cyclists is in the same direction as the velocity turned 90 degrees to the right
            {
                bForce = -fiets2V * .1f;
            }
            else
            {
                bForce = Vector3.zero;
            }

            other.velocity = other.velocity + bForce;//rotation * fiets2V;
        }
    }


}
