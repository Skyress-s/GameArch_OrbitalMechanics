using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Script {
    public partial struct OribtalGravitySystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            
        }

        public void OnUpdate(ref SystemState state) {
            // Apply Newtons formula for gravity for all planets
            // Basic Approach for moving an object! Remember to mark the object as Dynamic when baking! See class OrbitalGravityAuthoring
            foreach (var og1 in SystemAPI.Query<RefRW<LocalTransform>, RefRW<CelestialVelocityComponent>, RefRW<CelestialMassComponent>>()) {
                foreach (var og2 in SystemAPI.Query<RefRW<LocalTransform>, RefRW<CelestialVelocityComponent>, RefRW<CelestialMassComponent>>()) {
                    
                    // orbitalGravity.ValueRW.Position += math.up() * SystemAPI.Time.DeltaTime;
                    float gravConst = 4f * math.PI * math.PI;
                    float3 twoToOne = (og2.Item1.ValueRO.Position - og1.Item1.ValueRO.Position);
                    float underLine = math.length(twoToOne);
                    underLine *= underLine * underLine;
                    
                    float3 lol = -gravConst * (og1.Item3.ValueRO.celestialMass * og2.Item3.ValueRO.celestialMass) /
                        underLine * twoToOne;

                    og1.Item2.ValueRW.celestialVelocity += (lol / og1.Item3.ValueRO.celestialMass) * SystemAPI.Time.DeltaTime;
                    // velocity turns into float3(NaN, NaN, NaN) runtime
                    og1.Item1.ValueRW.Position += og1.Item2.ValueRO.celestialVelocity * SystemAPI.Time.DeltaTime;
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}