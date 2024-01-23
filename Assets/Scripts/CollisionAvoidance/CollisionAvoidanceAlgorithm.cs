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
            List<Vector3> collisionAvoidanceVectors = new List<Vector3>();

            // Find all cyclists within radius max (brake, steering)
            foreach (var c in _cyclists)
            {
                // Check Euclidean distance between cyclists
                var distance = Vector3.Distance(currentCyclist.transform.position, c.transform.position);
                if (distance > _maxRadius) continue; // Ignore, outside of reaction range
                
                // Check if agent is in FOV
                var angleToOther = RelativeAngleToCyclist(currentCyclist, c); //Relative angle [-180,180]
                const int maxFOVAngle = 135; // Temp, Angle (+ and -) angle of FOV. 
                if (angleToOther is > maxFOVAngle or < -maxFOVAngle) continue; // Ignore, outside of FOV
                
                var avoidanceVector = preferredVelocity;

                var (isCollisionImminent, tCol) = ApproximateCollision(currentCyclist, c);
                if (isCollisionImminent && distance < BrakeRangeRadius)
                {
                    // Do braking logic

                    if (angleToOther < 0) //And is not traveling same direction (within angle)
                    {
                        // Other cyclist on the left

                        //>> Prefer maintaining speed or speeding up

                        var spedUpVector = avoidanceVector * 1.2f;
                        const float maxSpeed = 3.5f;
                        if (spedUpVector.magnitude > maxSpeed)
                        {
                            avoidanceVector = spedUpVector * (maxSpeed / spedUpVector.magnitude);
                        }
                        else
                        {
                            avoidanceVector = spedUpVector;
                        }

                        Debug.Log(
                            $"Mag:{spedUpVector.magnitude}, pref:{preferredVelocity.magnitude}, av:{avoidanceVector.magnitude}");
                        Debug.Log($"original:{preferredVelocity}, spedup:{spedUpVector}, eventual:{avoidanceVector}");
                    }
                    else if (angleToOther >= 0) //And is not traveling same direction (within angle)
                    {
                        // Other cyclist on the right

                        //>> Prefer braking and slowing down

                        // const float minColTime = 3;
                        // Debug.Log($"SlowDownBEFORE:{avoidanceVector}");
                        // if (avoidanceVector.magnitude <= 0.5)
                        // {
                        //     //keep avoidance vector as is
                        // } else if (tCol <= minColTime) // Strong breaking based on time until collision
                        // {
                        //     avoidanceVector *= 0.9f * (minColTime / tCol);
                        // }
                        // else //within break range, outside strong break range
                        // {
                        //     avoidanceVector *= 0.9f;
                        // }
                        // Debug.Log($"Preff:{preferredVelocity}");
                        // Debug.Log($"SlowDown:{avoidanceVector}");
                    }
                }

                if (isCollisionImminent && distance < SteerRangeRadius)
                {
                    // // Do steer logic
                }

                collisionAvoidanceVectors.Add(avoidanceVector);
            }

            var vectorsSize = collisionAvoidanceVectors.Count;
            if (vectorsSize != 0)
            {
                var accumulativeVector =
                    collisionAvoidanceVectors.Aggregate(Vector3.zero, (next, acc) => acc + next) * (1f / vectorsSize);
                return (preferredVelocity + accumulativeVector) * 0.5f;
            }

            return preferredVelocity;
        }

        private float RelativeAngleToCyclist(GameObject currentCyclist, GameObject gameObject)
        {
            return 45; //temp
            
            //Calculate the degrees/radians [-180,180] the other cyclist is 
            throw new NotImplementedException();
        }

        // Roughly calculates if and when two agents would collide
        private (bool, float) ApproximateCollision(GameObject cur, GameObject other)
        {
            // Implement an collision approximation/prediction algorithm

            var cPos = cur.transform.position;
            var oPos = other.transform.position;

            var cAgent = cur.GetComponent<NavMeshAgent>();
            var oAgent = other.GetComponent<NavMeshAgent>();

            var cVel = cAgent.velocity;
            var oVel = oAgent.velocity;


            var deltaX = cPos.x - oPos.x;
            var deltaY = cPos.y - oPos.y;
            var deltaZ = cPos.z - oPos.z;

            var deltaVelX = cVel.x - oVel.x;
            var deltaVelY = cVel.y - oVel.y;
            var deltaVelZ = cVel.z - oVel.z;

            var t = Convert.ToSingle((deltaX * deltaVelX + deltaY * deltaVelY + deltaZ * deltaVelZ)
                                     / (Math.Pow(deltaVelX, 2) + Math.Pow(deltaVelY, 2) + Math.Pow(deltaVelZ, 2)));

            // Debug.Log($"time to collision::{t}");
            return t is float.NaN or < 0
                ? (false, -1) // t < 0 is not collision after NOW, return false
                : (true, t);
        }
    }
}