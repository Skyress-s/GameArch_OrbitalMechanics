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
            foreach (var orbitalGravity in SystemAPI.Query<RefRW<LocalTransform>>()) {
                orbitalGravity.ValueRW.Position += math.up() * SystemAPI.Time.DeltaTime;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}