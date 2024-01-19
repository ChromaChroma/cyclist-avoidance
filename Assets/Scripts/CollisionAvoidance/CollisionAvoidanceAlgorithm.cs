using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

namespace CollisionAvoidance
{
    public class CollisionAvoidanceAlgorithm
    {
        // Temp Fields
        private long _brakeRangeRadius;
        private long _steerRangeRadius; // Where steer rardius >= brake radius, for now
        private readonly List<GameObject> _cyclists = Cyclists.cyclistList;

        public Vector3 AvoidCollisions(GameObject currentCyclist, Vector3 preferredVelocity)
        {
            
            Debug.Log(_cyclists.Count);
            // List for accumulating avoidance vectors
            List<Vector3> collisionAvoidanceVectors = new List<Vector3>();

            // Find all cyclists within radius max (brake, steering)
            foreach (var c in _cyclists)
            {
                // Check either Euclidean distance or A* distance between cyclists
                float distance = Vector3.Distance(currentCyclist.transform.position, c.transform.position);

                if (distance > _brakeRangeRadius) continue; // Ignore, outside of range

                var (isCollisionImminent, tCol) = ApproximateCollision(currentCyclist, c);

                var avoidanceVector = preferredVelocity;

                if (isCollisionImminent && distance < _brakeRangeRadius)
                {
                    // // Do braking logic
                    //
                    // const int maxBrakeTimeDistance = 3; // Temp value
                    // var breakTColNormalized = Math.Min(tCol, maxBrakeTimeDistance) / maxBrakeTimeDistance;
                    // avoidanceVector *= breakTColNormalized;
                }

                if (isCollisionImminent && distance < _steerRangeRadius)
                {
                    // // Do steer logic
                    // const int maxSteerTimeDistance = 1; // Temp value
                    // var breakTColNormalized = Math.Min(tCol, maxSteerTimeDistance) / maxSteerTimeDistance;
                    //
                    // // Compute angles between cyclists and directions. then choose force
                    //
                    //
                    // avoidanceVector *= breakTColNormalized;
                }

                collisionAvoidanceVectors.Add(avoidanceVector);
            }

            var vectorsSize = collisionAvoidanceVectors.Count;
            Debug.Log($"Amount of cyclists near: {vectorsSize}");
            var accumulativeVector =
                collisionAvoidanceVectors.Aggregate(Vector3.zero, (next, acc) => acc + next) * (1f / vectorsSize);
            
            // Debug.Log($"Preff. vector:: {preferredVelocity}, Acc vector:: {accumulativeVector}");
            // Debug.Log($"Avoidance vector:: {preferredVelocity + accumulativeVector}");
            
            return (preferredVelocity + accumulativeVector) * 0.5f;
        }

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

            Debug.Log($"time to collision::{t}");
            return t is float.NaN or < 0
                ? (false, -1) // t < 0 is not collision at later point, return false
                : (true, t);
        }
    }
}