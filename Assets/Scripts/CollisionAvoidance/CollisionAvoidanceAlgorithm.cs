﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

namespace CollisionAvoidance
{
    public class CollisionAvoidanceAlgorithm
    {
        const int MaxFOVAngle = 135;

        // Configurable properties
        public bool AutoUpdateRadii = true;

        // Accessible properties
        public float BrakeRangeRadius { get; private set; }
        public float SteerRangeRadius { get; private set; }

        // Internal properties 
        private float _maxRadius;
        private readonly NavMeshAgent _agent;

        public bool Collides;
        private readonly List<GameObject> _colCyclists = new();

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
            // Brake radius function
            var brakeRangeCurveFunction = (float)(2 + Math.Pow(v, 1.3));
            BrakeRangeRadius = brakeRangeCurveFunction;

            // Steer radius function
            var steerRangeCurveFunction = (float)(2 + v * 0.6f + Math.Pow(v / 13, 2));
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

            var brakeVectors = new List<Vector3>();
            var steeringForce = Vector3.zero;

            // Find all cyclists within radius max (brake, steering)
            foreach (var c in _cyclists)
            {
                if (c == currentCyclist) continue;

                // Check Euclidean distance between cyclists
                var distance = Vector3.Distance(currentCyclist.transform.position, c.transform.position);
                if (distance < 0.7)
                {
                    this.Collides = true;
                    _colCyclists.Add(c);
                }
                else if (_colCyclists.Contains(c))
                {
                    _colCyclists.Remove(c);

                    if (_colCyclists.Count == 0) this.Collides = false;
                }

                if (distance > _maxRadius) continue; // Ignore, outside of reaction range

                // Check if agent is in FOV
                var angleToOther = RelativeAngleToCyclist(currentCyclist, c); //Relative angle [-180,180]

                if (angleToOther is > MaxFOVAngle or < -MaxFOVAngle) continue; // Ignore, outside of FOV

                // var willCollide = WillCollide(currentCyclist, c);
                var (willCollide, _) = ApproximateCollision(currentCyclist, c);
                if (distance < BrakeRangeRadius && willCollide)
                {
                    // Braking logic
                    brakeVectors.Add(-preferredVelocity * BrakingForce(distance, angleToOther));
                }

                if (willCollide && distance < SteerRangeRadius)
                {
                    // Do steer logic
                    var sf = Vector3.Cross(preferredVelocity, Vector3.up).normalized * SteerForce(distance, angleToOther);
                    if (sf.magnitude > steeringForce.magnitude)
                    {
                        steeringForce = sf;
                    }
                }
            }

            // Apply brake vector to velocity vector
            var length = brakeVectors.Count;
            if (length != 0)
            {
                // var accumulativeVector = brakeVectors.Aggregate(Vector3.zero, (v, acc) => acc + v) * (1f / length);
                var accumulativeVector = brakeVectors.OrderByDescending(v => v.magnitude).First();
                preferredVelocity += accumulativeVector;
            }

            return preferredVelocity + steeringForce;
        }

        private float RelativeAngleToCyclist(GameObject currentCyclist, GameObject other)
        {
            var deltaPos = other.transform.position - currentCyclist.transform.position;
            return Vector3.SignedAngle(_agent.velocity, deltaPos, Vector3.up);
        }

        private bool WillCollide(GameObject currentCyclist, GameObject otherCyclist) //rudimentary approach
        {
            var cPos = currentCyclist.transform.position;
            var oPos = otherCyclist.transform.position;

            float dist = Vector3.Distance(cPos, oPos);
            float futureDistCur = Vector3.Distance(cPos + _agent.velocity.normalized * .1f, oPos);
            float futureDistOth = Vector3.Distance(cPos,
                oPos + otherCyclist.GetComponent<NavMeshAgent>().velocity.normalized * .1f);

            return dist < 0.1f || (futureDistCur < dist && futureDistOth < dist);
        }

        // Based on comfort zone paper, 3.2 page 4, "lateral clearance from the model was 0.6-0.9 m"
        // We take the average value, 0.75m as our constant
        private const float BrakeComfortClearance = 0.75f;

        private float BrakingForce(float distance, float angleToOther) => distance switch
        {
            // Close range 'reactive' behaviour
            <= BrakeComfortClearance when angleToOther < -80 => -0.1f,
            <= BrakeComfortClearance when angleToOther < -45 => -0.2f,
            <= BrakeComfortClearance when angleToOther < 80 => 0.99f,
            <= BrakeComfortClearance when angleToOther >= 80 => -0.2f,

            // Predictive range behaviour
            > BrakeComfortClearance when angleToOther < -30 => -0.1f,
            > BrakeComfortClearance when angleToOther < -20 => -0.05f,
            > BrakeComfortClearance when angleToOther < 0 => 0f,
            > BrakeComfortClearance when angleToOther < 25 => 0.4f,
            > BrakeComfortClearance when angleToOther < 70 => 0.95f,
            > BrakeComfortClearance when angleToOther > 0 => 0f,
            _ => throw new ArgumentOutOfRangeException(nameof(distance), distance,
                "BrakingForce value was not covered in patterns")
        };

        // TODO: verder werken aan metric: nu heb ik alleen aantal fietsers dat aankomt gegeven een bepaalde tijd, kan time signature meegeven met het tellen om de flow per (x aantal seconden) in kaart te brengen

        private float SteerForce(float distance, float angleToOther)
        {
            var sf = angleToOther switch
            {
                < -35 => -1f, //slight to no r
                < -10 => -2f, //r
                < 10 => -5f, //r
                < 45 => -2f, //r
                >= 45 => -1f, // no or left to avoid colliding
                _ => 0
            };

            return sf; //Vector3.Slerp(Vector3.zero, sf, Time.deltaTime * 25f);
        }

        // Roughly calculates if and when two agents would collide
        private (bool, float) ApproximateCollision(GameObject cur, GameObject other)
        {
            // Implement an collision approximation/prediction algorithm
            // Collision calculate algorithm for two object trajectories
            // Based on: https://math.stackexchange.com/questions/4713617/how-to-calculate-if-2-objects-following-2-different-arbitrary-trajectories-will

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

            var vDelta = deltaVelX + deltaVelY + deltaVelZ;
            var r0Delta = deltaX + deltaY + deltaZ;

            var vrDelta = deltaX * deltaVelX + deltaY * deltaVelY + deltaZ * deltaVelZ;
            var cyclistRadius = Math.Max(2, cVel.magnitude);
            var rP2 = Math.Pow(cyclistRadius + cyclistRadius, 2);
            var vDeltaP2 = Math.Pow(vDelta, 2);
            var r0DeltaP2 = Math.Pow(r0Delta, 2);
            var combinedP2 = Math.Pow(vrDelta, 2);

            var t2 = Convert.ToSingle((-vrDelta - Math.Sqrt(combinedP2 - vDeltaP2 * (r0DeltaP2 - rP2))) / vDeltaP2) +
                     0.5f;

            return t2 is float.NaN or < 0
                ? (false, -1) // t < 0 is not collision after NOW, return false
                : (true, t2);
        }
    }
}