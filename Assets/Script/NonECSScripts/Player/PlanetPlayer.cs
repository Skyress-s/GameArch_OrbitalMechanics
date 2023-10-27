using System;
using UnityEngine;

namespace Script.NonECSScripts.Player {
    public class PlanetPlayer : MonoBehaviour {
        private CelestialBody targetCelestianBody = null;
        private Camera camera;

        public Camera GetCamera() {
            return camera;
        }

        public void AddToCameraLength(float add) {
            var pos = camera.transform.localPosition;
            pos.z += add;
            if (pos.z > 0)
                pos.z = 0;
            
            camera.transform.localPosition = pos;
            
        }


        public void SetTargetCelestianBody(CelestialBody celestialBody) {
            targetCelestianBody = celestialBody;
        }
        private void Start() {
            camera = GetComponentInChildren<Camera>();
        }

        private void Update() {
            if (targetCelestianBody) {
                transform.position = targetCelestianBody.transform.position;
            }
        }
    }
}