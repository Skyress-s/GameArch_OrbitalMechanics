using Unity.Entities;
using Unity.Mathematics;

namespace Script {
    public struct CelestialVelocity : IComponentData {
        public float3 celestialVelocity;
    }
}