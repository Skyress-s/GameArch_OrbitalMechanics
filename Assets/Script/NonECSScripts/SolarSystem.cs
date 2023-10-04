using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.NonECSScripts
{
    public class SolarSystem : MonoBehaviour
    {
        [SerializeField] private uint objectToFreeze = 0;
        [SerializeField] private bool freezeObject;
        [SerializeField] private List<CelestialBody> celestialBodies;
        
        public readonly float G = Mathf.PI * Mathf.PI * 4f;

        private void Start()
        {
            celestialBodies = new List<CelestialBody>(GetComponentsInChildren<CelestialBody>());
        }

        private Vector3 Gravity(CelestialBody A, CelestialBody B)
        {
            var rAB = B.transform.position - A.transform.position;
            return G * B.Mass * rAB.normalized / rAB.sqrMagnitude;
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < celestialBodies.Count; i++)
            {
                for (int j = 0; j < celestialBodies.Count; j++)
                {
                    var A = celestialBodies[i];
                    var B = celestialBodies[j];
                    
                    if (A == B) continue;
                    if (freezeObject && i == objectToFreeze) continue;
                    
                    var aCurrent = Gravity(A, B);
                    
                    A.transform.position += A.Velocity*Time.fixedDeltaTime + aCurrent * (0.5f * Time.fixedDeltaTime * Time.fixedDeltaTime);
                    
                    var aNext = Gravity(A, B);
                    
                    A.Velocity += (aCurrent + aNext) * (0.5f * Time.fixedDeltaTime);
                }
            }
        }
    }
}
