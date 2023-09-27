using System;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace Script {
    public class CelestialVelocityAuthoring : MonoBehaviour {
        [SerializeField] private Vector3 startVelocity;

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + startVelocity);
        }

        private class CelestialVelocityAuthoringBaker : Baker<CelestialVelocityAuthoring> {
            public override void Bake(CelestialVelocityAuthoring authoring) {
                
            var ent = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(ent, new CelestialVelocityComponent() {
                celestialVelocity = authoring.startVelocity,
            });
            }
        }
    }
}