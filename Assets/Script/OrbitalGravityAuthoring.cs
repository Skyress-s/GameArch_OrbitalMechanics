using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class OrbitalGravityAuthoring : MonoBehaviour {
    private class OrbitalGravityAuthoringBaker : Baker<OrbitalGravityAuthoring> {
        public override void Bake(OrbitalGravityAuthoring authoring) {
            var ent = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(ent, new OrbitalGravityComponent {
                scale = 1f
            });
        }
    }
}