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
        const int maxFOVAngle = 135;

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

            // Brake radius function: 3+ 0.15 + (v/10)^2
            var brakeRangeCurveFunction = (float)(3 + v * 0.15f + Math.Pow(v / 10, 2));
            BrakeRangeRadius = brakeRangeCurveFunction;

            // Steer radius function: 4 + 0.2x + (v/13)^2
            var steerRangeCurveFunction = (float)(4 + v * 0.2f + Math.Pow(v / 13, 2));
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

            var largestBrakeVector = Vector3.zero;

            // Find all cyclists within radius max (brake, steering)
            foreach (var c in _cyclists)
            {
                if (c == currentCyclist) continue; // Ignore self

                // Check Euclidean distance between cyclists
                var distance = Vector3.Distance(currentCyclist.transform.position, c.transform.position);
                if (distance > _maxRadius) continue; // Ignore outside of reaction range

                // Check if agent is in FOV
                var angleToOther = RelativeAngleToCyclist(currentCyclist, c);
                if (angleToOther is > maxFOVAngle or < -maxFOVAngle) continue; // Ignore outside of FOV

                if (distance < BrakeRangeRadius && WillCollide(currentCyclist, c))
                {
                    // Braking logic
                    var brakeVector = -preferredVelocity * BrakingForce(distance, angleToOther);
                    if (brakeVector.magnitude > largestBrakeVector.magnitude)
                    {
                        largestBrakeVector = brakeVector;
                    }
                }

                var (isCollisionImminent, tCol) = ApproximateCollision(currentCyclist, c);
                if (isCollisionImminent && distance < SteerRangeRadius)
                {
                    // Do steer logic
                }
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

        private bool WillCollide(GameObject currentCyclist, GameObject otherCyclist) //rudimentary approach
        {
            var cPos = currentCyclist.transform.position;
            var oPos = otherCyclist.transform.position;

            float dist = Vector3.Distance(cPos, oPos);
            float futureDistCur = Vector3.Distance(cPos + currentCyclist.GetComponent<NavMeshAgent>().velocity.normalized * .1f, oPos);
            float futureDistOth = Vector3.Distance(cPos, oPos + otherCyclist.GetComponent<NavMeshAgent>().velocity.normalized * .1f);

            return dist < .1 && futureDistCur < dist && futureDistOth < dist;
        }

        private float BrakingForce(float distance, float angleToOther) => distance switch
        {
            <= 1 when angleToOther < 0 => 1.5f,
            <= 1 => 3f,
            > 1 when angleToOther < 0 => .7f,
            > 1 => 9f,
            _ => throw new ArgumentOutOfRangeException(nameof(distance), distance, null)
        }; // TODO: verder werken aan metric: nu heb ik alleen aantal fietsers dat aankomt gegeven een bepaalde tijd, kan time signature meegeven met het tellen om de flow per (x aantal seconden) in kaart te brengen
    }
}