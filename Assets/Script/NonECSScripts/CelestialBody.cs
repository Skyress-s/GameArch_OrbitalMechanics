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
        [SerializeField] private bool isSun;
        
        public bool IsSun
        {
            get => isSun;
            set => isSun = value;
        }

        public float Mass
        {
            get => mass;
            set => mass = value;
        }

        public Vector3 CurrentForce { private get; set; }
        
        public void Awake()
        {
            Velocity = initialVelocity;
        }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            var forcePos = position + CurrentForce.normalized * 0.16f;
            var velPos = position + Velocity.normalized*0.16f;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(forcePos, forcePos + CurrentForce);

            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawLine(velPos, velPos + Velocity);
        }

        public Vector3 Velocity { get; set; }

        public float KineticEnergy()
        {
            return 0.5f * mass * Velocity.sqrMagnitude;
        }
    }
}