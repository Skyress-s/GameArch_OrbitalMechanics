using Unity.Entities;
using Unity.Mathematics;

namespace Script {
    public struct CelestialVelocityComponent : IComponentData {
        public float3 celestialVelocity;
    }
}