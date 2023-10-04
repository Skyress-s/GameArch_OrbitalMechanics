// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: CelestialBody.cs
// //FileType: Visual C# Source file
// //Author : Anders P. Åsbø
// //Created On : 04/10/2023
// //Last Modified On : 04/10/2023
// //Copy Rights : Anders P. Åsbø
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////

using System;
using UnityEngine;

namespace Script.NonECSScripts
{
    public class CelestialBody : MonoBehaviour
    {
        [SerializeField] [Min(1e-7f)] [Tooltip("Unit: Solar masses.")]
        private float mass = 1;

        [SerializeField] private Vector3 initialVelocity = Vector3.zero;

        public float Mass => mass;

        public void Awake()
        {
            Velocity = initialVelocity;
        }

        public Vector3 Velocity { get; set; }

        public float KineticEnergy()
        {
            return 0.5f * mass * Velocity.sqrMagnitude;
        }
    }
}