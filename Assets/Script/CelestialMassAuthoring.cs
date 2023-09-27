using Script;
using Unity.Entities;
using UnityEngine;

public class CelestialMassAuthoring : MonoBehaviour {
    [SerializeField] private float celestialMass = 1f;
    private class CelestialMassAuthoringBaker : Baker<CelestialMassAuthoring> {
        public override void Bake(CelestialMassAuthoring authoring) {
            var ent = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(ent, new CelestialMassComponent() {
                celestialMass = authoring.celestialMass,
            });
        }
    }
}