using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

namespace CollisionAvoidance
{
    public class CollisionAvoidanceAlgorithm
    {
        // Configurable properties
        public bool AutoUpdateRadii = true;
        
        // Accessable properties
        public float BrakeRangeRadius { get; private set; }
        public float SteerRangeRadius { get; private set; }
        
        // Internal properties 
        private float _maxRadius;
        private readonly NavMeshAgent _agent; 
        public CollisionAvoidanceAlgorithm(NavMeshAgent agent)
        {
            _agent = agent;
            BrakeRangeRadius = 0;
            SteerRangeRadius = 0;
            _maxRadius = 0;
        }

        private readonly List<GameObject> _cyclists = Cyclists.cyclistList;

        public void UpdateRadii()
        {
            // Update radii of agent based on current velocity
            var v = _agent.velocity.magnitude;
            
            // The following formulas are based on rough visual copy of the comfort zone paper figure 5
            
            // Brake radius
            // min_brake_radius + v * linear constant + v^2
            // 3+ 0.15 + (v/10)^2
            var brakeRangeCurveFunction = (float)(3 + v * 0.15f + Math.Pow(v/10, 2));
            BrakeRangeRadius = brakeRangeCurveFunction;
            
            // Steer radius
            // min_steer_radius + v * linear constant + v^2
            // 4 + 0.2x + (v/13)^2
            var steerRangeCurveFunction = (float)(4 + v * 0.2f + Math.Pow(v/13, 2));
            SteerRangeRadius = steerRangeCurveFunction;
            
            
            // Update max Radius to largest radius
            _maxRadius = Math.Max(BrakeRangeRadius, SteerRangeRadius);
        }

        public Vector3 AvoidCollisions(GameObject currentCyclist, Vector3 preferredVelocity)
        {
            if (AutoUpdateRadii)
            {
                UpdateRadii();
            }
      
            // List for accumulating avoidance vectors
            // List<Vector3> collisionAvoidanceVectors = new List<Vector3>();
            List<Vector3> brakeVectors = new List<Vector3>();

            // Find all cyclists within radius max (brake, steering)
            foreach (var c in _cyclists)
            {
                if (c != currentCyclist)
                {
                    // Check Euclidean distance between cyclists
                    var distance = Vector3.Distance(currentCyclist.transform.position, c.transform.position);
                    if (distance > _maxRadius) continue; // Ignore, outside of reaction range

                    // Check if agent is in FOV
                    var angleToOther = RelativeAngleToCyclist(currentCyclist, c); //Relative angle [-180,180]
                    const int maxFOVAngle = 135; // Temp, Angle (+ and -) angle of FOV. 
                    if (angleToOther is > maxFOVAngle or < -maxFOVAngle) continue; // Ignore, outside of FOV

                    var (isCollisionImminent, tCol) = ApproximateCollision(currentCyclist, c);
                    if (((distance < BrakeRangeRadius) && willCollide(currentCyclist, c)))
                    {
                        // Braking logic
                        brakeVectors.Add(-preferredVelocity * brakingForce(distance, angleToOther));
                    }

                    if (isCollisionImminent && distance < SteerRangeRadius)
                    {
                        // Do steer logic
                    }
                }
            }
            
            // Apply brake vector to velocity vector
            var length = brakeVectors.Count;
            if (length != 0)
            {
                var accumulativeVector = largestVec(brakeVectors); //for braking, the closest other cyclist counts
                //var accumulativeVector = brakeVectors.Aggregate(Vector3.zero, (v, acc) => acc + v) * (1f / length);
                //Debug.Log($"Acc Brake: {accumulativeVector}, pref:{preferredVelocity}, comb ={preferredVelocity + accumulativeVector}");
                preferredVelocity += accumulativeVector;
            }
            
            return preferredVelocity;
        }

        private float RelativeAngleToCyclist(GameObject currentCyclist, GameObject other)
        {
            var v1 = currentCyclist.GetComponent<NavMeshAgent>().velocity;
            var d = other.transform.position - currentCyclist.transform.position;
            return Vector3.SignedAngle(v1, d, Vector3.up);
        }

        // Roughly calculates if and when two agents would collide
        private (bool, float) ApproximateCollision(GameObject cur, GameObject other)
        {
            // Collision calculate algorithm for two object trajectories
            // Based on: https://math.stackexchange.com/questions/4713617/how-to-calculate-if-2-objects-following-2-different-arbitrary-trajectories-will

            var cPos = cur.transform.position;
            var oPos = other.transform.position;

            var cVel = cur.GetComponent<NavMeshAgent>().velocity;
            var oVel = other.GetComponent<NavMeshAgent>().velocity;


            var deltaX = cPos.x - oPos.x;
            var deltaY = cPos.y - oPos.y;
            var deltaZ = cPos.z - oPos.z;

            var deltaVelX = cVel.x - oVel.x;
            var deltaVelY = cVel.y - oVel.y;
            var deltaVelZ = cVel.z - oVel.z;

            var t = Convert.ToSingle((deltaX * deltaVelX + deltaY * deltaVelY + deltaZ * deltaVelZ)
                                     / (Math.Pow(deltaVelX, 2) + Math.Pow(deltaVelY, 2) + Math.Pow(deltaVelZ, 2)));

            return t is float.NaN or < 0
                ? (false, -1) // t < 0 is not collision after NOW, return false
                : (true, t);
        }

        private bool willCollide(GameObject currentCyclist, GameObject otherCyclist) //rudimentary approach
        {
            float dist = Vector3.Distance(currentCyclist.transform.position, otherCyclist.transform.position);
            float futureDistCur = Vector3.Distance(currentCyclist.transform.position + currentCyclist.GetComponent<NavMeshAgent>().velocity.normalized * .1f, otherCyclist.transform.position);
            float futureDistOth = Vector3.Distance(currentCyclist.transform.position, otherCyclist.transform.position + otherCyclist.GetComponent<NavMeshAgent>().velocity.normalized * .1f);
            if (dist < .1) return true;
            if ((futureDistCur < dist) && (futureDistOth < dist)) return true; else return false;
        }


        private Vector3 largestVec(List<Vector3> l)
        {
            Vector3 res = Vector3.zero;
            foreach (Vector3 v in l)
            {
                if (v.magnitude > res.magnitude) res = v;
            }
            return res;
        }

        private float brakingForce(float distance, float angleToOther)
        {
            if (distance <= 1)
            {
                if (angleToOther < 0)
                {
                    return 1.5f;
                }
                else
                {
                    return 3f;
                } //in case of problems, put back to 1
            }
            else
            {
                if (angleToOther < 0)
                {
                    return .7f;
                }
                else
                {
                    return .9f;
                }
                
            }
        } // TODO: verder werken aan metric: nu heb ik alleen aantal fietsers dat aankomt gegeven een bepaalde tijd, kan time signature meegeven met het tellen om de flow per (x aantal seconden) in kaart te brengen
    }
}