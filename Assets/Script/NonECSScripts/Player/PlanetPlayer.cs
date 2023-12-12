using UnityEngine;

namespace Script.NonECSScripts.Player {
    /// <summary>
    /// Mainly developed by: Mathias Mohn Mørch
    /// </summary>
    public class PlanetPlayer : MonoBehaviour {
        [SerializeField]private CelestialBody targetCelestianBody = null;
        private Camera camera;
        private PlanetPlayerController _planetPlayerController;

        private void Start() {
            camera = GetComponentInChildren<Camera>();
            _planetPlayerController = PlanetPlayerController.Instance as PlanetPlayerController;
        }

        private void Update() {
            if (targetCelestianBody) {
                transform.position = Vector3.Lerp(transform.position, targetCelestianBody.transform.position,
                    Time.deltaTime * 8f);
            }
            
            // Sets target celestial body
            Quaternion targetRot = Quaternion.Euler(_planetPlayerController.ControlRotation);
            
            // Set target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 7f);
            
            // SetZoom
            
        }

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

        public void SetCameraLength(float newLength) {
            var pos = camera.transform.localPosition;
            pos.z = Mathf.Max(newLength, 0f);
            
        }


        public void SetTargetCelestianBody(CelestialBody celestialBody) {
            targetCelestianBody = celestialBody;
        }
        
        public CelestialBody GetTargetCelestianBody() {
            return targetCelestianBody;
        }
    }
}