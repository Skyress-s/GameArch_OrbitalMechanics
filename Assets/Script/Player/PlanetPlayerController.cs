using System.Collections.Generic;
using Script.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.NonECSScripts.Player {
    public class PlanetPlayerController : PlayerController {
        public PlanetPlayer planetPlayer;
        
        public Vector3 ControlRotation = Vector3.zero;
        public float Zoom = 0f;
        [SerializeField] private float _mouseSensetivity = 1f;
        [SerializeField] private float _scrollSensetivity = 1f;
    
        
        private bool bWaitingForNewPlanetClick = false;

        private Vector2 _mousePosLastFrame;
        
        [SerializeField] private Collider XZCollider;
        
        [Header("System")] [SerializeField] private SolarSystem system;

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
                if (bWaitingForNewPlanetClick) {
                    PlaceNewPlanet();
                }
                else {
                    
                    HandleOnClickPrimary();
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

        private void HandleOnClickPrimary() {

            if (IsPointerOverUIElement()) {
                return;
            }
            
            var ray = planetPlayer.GetCamera().ScreenPointToRay(Input.mousePosition);
            if (Physics.SphereCast(ray, 1f, out RaycastHit hit, 100000, LayerMask.GetMask("Default"))) {
                CelestialBody celestialBody = hit.transform.GetComponent<CelestialBody>();
                
                if (celestialBody != null && planetPlayer.GetTargetCelestianBody() != celestialBody) {
                    planetPlayer.SetTargetCelestianBody(celestialBody);
                    UISingletonBuilder builder = new UISingletonBuilder();

                    if (celestialBody.TryGetComponent(out IPlanetaryInfo planetaryInfo)) {
                        
                        builder.AddText(planetaryInfo.GetInfo());
                        builder.AddButton("Move", OnStartMovePlanet);
                        builder.AddArrowVisualizer(celestialBody);
                        builder.AddMassSlider(celestialBody);
                        builder.Build(celestialBody.transform, Vector3.zero);
                        
                    }
                    
                    PlanetMover.Disable();
                    
                }
            }
        }
        
        //https://forum.unity.com/threads/how-to-detect-if-mouse-is-over-ui.1025533/
        //Returns 'true' if we touched or hovering on Unity UI element.
        public bool IsPointerOverUIElement()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }
 
 
        //Returns 'true' if we touched or hovering on Unity UI element.
        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                    return true;
            }
            return false;
        }

        public void OnClickPlaceNewPlanet() {
            bWaitingForNewPlanetClick = true;
            XZCollider.enabled = true;
        }

        private void PlaceNewPlanet() {
            var ray = planetPlayer.GetCamera().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100000, LayerMask.GetMask("Water"))) {
                SpawnNewPlanet(hit.point);
                
            }

            XZCollider.enabled = false;
            bWaitingForNewPlanetClick = false;
        }


        private void SpawnNewPlanet(Vector3 position) {
            if (system)
            {
                // Adds a planet with 10x jupiter's mass and correct orbital velocity at position
                system.AddPlanet(position, true, 0.0095f);
            }
            Debug.DrawRay(position, Vector3.up * 1000f, Color.red, 100f);
        }
 
 
        //Gets all event system raycast results of current mouse or touch position.
        static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }

        private void OnStartMovePlanet() {
            PlanetMover.Enable(planetPlayer.GetTargetCelestianBody().transform);
            
        }
    }
}