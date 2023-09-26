using Unity.Entities;
using UnityEngine;

namespace Script {
    public struct CelestialMassComponent : IComponentData {
        [Tooltip("Unit - Solar Mass SM 2 * 10^30")] 
        public float celestialMass;
    }
}