using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.NonECSScripts.Player {
    public class PlanetPlayerController : PlayerController {
        public PlanetPlayer planetPlayer;
        
        public Vector3 ControlRotation = Vector3.zero;
        public float Zoom = 0f;
        [SerializeField] private float _mouseSensetivity = 1f;
        [SerializeField] private float _scrollSensetivity = 1f;


        private Vector2 _mousePosLastFrame;

        private void Awake() {
            if (Instance != null) {
                Destroy(this);       
            }
            else {
                Instance = this;
            }
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                var ray = planetPlayer.GetCamera().ScreenPointToRay(Input.mousePosition);
                
                // if (Physics.Raycast(ray,out RaycastHit hit, 100000)) {
                if (Physics.SphereCast(ray, 1f, out RaycastHit hit, 100000)) {
                    Debug.DrawRay(ray.origin, ray.direction.normalized*100, Color.red, 4f);
                    CelestialBody celestialBody = hit.transform.GetComponent<CelestialBody>();
                    if (celestialBody != null) {
                        planetPlayer.SetTargetCelestianBody(celestialBody);
                    }
                }
            }

            if (Input.GetKey(KeyCode.Mouse2)) {
                Vector2 mousePositionDelta = (Vector2)Input.mousePosition - _mousePosLastFrame;

                ControlRotation.x -= mousePositionDelta.y * _mouseSensetivity;
                ControlRotation.y += mousePositionDelta.x * _mouseSensetivity;

                // planetPlayer.transform.localEulerAngles = ControlRotation;
            }
            
            planetPlayer.SetCameraLength(Input.mouseScrollDelta[1] * _scrollSensetivity);
            planetPlayer.AddToCameraLength(Input.mouseScrollDelta[1] * _scrollSensetivity);
            
            _mousePosLastFrame = Input.mousePosition;
        }
    }
}