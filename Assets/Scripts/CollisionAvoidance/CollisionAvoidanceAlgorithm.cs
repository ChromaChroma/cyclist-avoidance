using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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

        // Collision checking
        public bool collides = false;
        private List<GameObject> colCyclists = new List<GameObject>();

        // Grouping behaviour
        public GameObject leader = null;
        public bool hasleader() { leader != null; }

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
            // var brakeRangeCurveFunction = (float)(2 + v * 0.6f + Math.Pow(v / 10, 2));
            var brakeRangeCurveFunction = (float)(2 + Math.Pow(v, 1.3));
            BrakeRangeRadius = brakeRangeCurveFunction;

            // Steer radius function: 3+ 0.15 + (v/10)^2 :: 4 + 0.2x + (v/13)^2
            var steerRangeCurveFunction = (float)(2  + v * 0.6f + Math.Pow(v / 13, 2));
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

            // Find all cyclists within radius max (brake, steering)
            foreach (var c in _cyclists)
            {
                if (c == currentCyclist) continue;

                // Check Euclidean distance between cyclists
                var distance = Vector3.Distance(currentCyclist.transform.position, c.transform.position);
                if (distance < 0.7)
                {
                    this.collides = true;
                    colCyclists.Add(c);
                }
                else if (colCyclists.Contains(c))
                {
                    colCyclists.Remove(c);
                    
                    if (colCyclists.Count == 0) this.collides = false;
                }
                

                if (distance > _maxRadius) continue; // Ignore, outside of reaction range

                // Check if agent is in FOV
                var angleToOther = RelativeAngleToCyclist(currentCyclist, c); //Relative angle [-180,180]

                if (angleToOther is > maxFOVAngle or < -maxFOVAngle) continue; // Ignore, outside of FOV
                
                var willCollide = WillCollide(currentCyclist, c);
                if (distance < BrakeRangeRadius && willCollide)
                {   // Braking logic
                    brakeVectors.Add(-preferredVelocity * BrakingForce(distance, angleToOther));
                }
                
                if (willCollide && distance < SteerRangeRadius)
                {   // Do steer logic
                }

                // Add cyclist ahead to leader
                if ((distance < BrakeRangeRadius) && (AngleBetweenVelocities(c) < 5))
                {
                    leader = c;
                }
                // And remove if distance is too far again
                if ((c == leader) && (distance > BrakeRangeRadius))
                {
                    leader = null;
                }
            }

            // Apply brake vector to velocity vector
            var length = brakeVectors.Count;
            if (length != 0)
            {
                // var accumulativeVector = brakeVectors.Aggregate(Vector3.zero, (v, acc) => acc + v) * (1f / length);
                var accumulativeVector = brakeVectors.OrderByDescending(v => v.magnitude).First();
                // Debug.Log($"Pref: {preferredVelocity.magnitude}, brakeV:{accumulativeVector.magnitude}, comb: {(preferredVelocity + accumulativeVector).magnitude}");
                preferredVelocity += accumulativeVector;
            }

            return preferredVelocity;
        }

        private float RelativeAngleToCyclist(GameObject currentCyclist, GameObject other)
        {
            var deltaPos = other.transform.position - currentCyclist.transform.position;
            return Vector3.SignedAngle(_agent.velocity, deltaPos, Vector3.up);
        }

        private float AngleBetweenVelocities(GameObject other)
        {
            return Vector3.Angle(_agent.velocity, other.transform.velocity, Vector3.up);
        }

        private bool WillCollide(GameObject currentCyclist, GameObject otherCyclist) //rudimentary approach
        {
            var cPos = currentCyclist.transform.position;
            var oPos = otherCyclist.transform.position;

            float dist = Vector3.Distance(cPos, oPos);
            float futureDistCur = Vector3.Distance(cPos + _agent.velocity.normalized * .1f, oPos);
            float futureDistOth = Vector3.Distance(cPos, oPos + otherCyclist.GetComponent<NavMeshAgent>().velocity.normalized * .1f);

            return dist < 0.5f || (futureDistCur < dist && futureDistOth < dist);
        }
        
        // Based on comfort zone paper, 3.2 page 4, "lateral clearance from the model was 0.6-0.9 m"
        // We take the average value, 0.75m as our constant
        private const float BrakeComfortClearance = 0.75f;
        private float BrakingForce(float distance, float angleToOther) => distance switch
        {
            // Close range 'reactive' behaviour
            <= BrakeComfortClearance when angleToOther < -80 => -0.1f,
            <= BrakeComfortClearance when angleToOther < -45 => -0.2f,
            <= BrakeComfortClearance when angleToOther <  80 =>  0.99f,
            <= BrakeComfortClearance when angleToOther >= 80 => -0.2f,
            
            // Predictive range behaviour
            > BrakeComfortClearance  when angleToOther < -45 => -0.1f,
            > BrakeComfortClearance  when angleToOther < -20 => -0.05f,
            > BrakeComfortClearance  when angleToOther <  0  =>  0f,
            > BrakeComfortClearance  when angleToOther <  25 =>  0.4f,
            > BrakeComfortClearance  when angleToOther <  70 =>  0.95f,
            > BrakeComfortClearance  when angleToOther >  0  =>  0f,
            _ => throw new ArgumentOutOfRangeException(nameof(distance), distance, "BrakingForce value was not covered in patterns")
        };
        
        // TODO: verder werken aan metric: nu heb ik alleen aantal fietsers dat aankomt gegeven een bepaalde tijd, kan time signature meegeven met het tellen om de flow per (x aantal seconden) in kaart te brengen
    }
}