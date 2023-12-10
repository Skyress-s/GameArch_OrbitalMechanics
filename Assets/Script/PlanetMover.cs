using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Script {
    public class PlanetMover : MonoBehaviour {
        
        [SerializeField] private Collider colliderX;
        [SerializeField] private Collider colliderY;
        [SerializeField] private Collider colliderZ;

        public Transform target;
        
        public static PlanetMover Instance { get; private set; }

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public static void Enable(Transform target) {
            Instance.target = target;
            Instance.gameObject.SetActive(true);
        }

        public static void Disable() {
            Instance.gameObject.SetActive(false);
        }

        private void Update() {
            transform.position = target.position;
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                StartDrag();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0)) {
                EndDrag();
                dragging = false;
            }


            if (!dragging) 
                return;
            
            Vector3 endPos = Input.mousePosition;

            Vector3 v1 = endPos - startPos;

            float distance = Vector3.Dot(v1, screenSpaceForward);
            Debug.LogWarning($"ScreenSpaceForward : {screenSpaceForward} | v1 : {v1} | distance : {distance}");
            target.position += direction * distance/100f;
            startPos = endPos;
        }

        private Vector3 direction = Vector3.forward;
        
        private bool dragging = false;
        private Vector3 startPos;
        private Vector3 screenSpaceForward;
        private void StartDrag() {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10000, LayerMask.GetMask("InGameUI"))) {
                
                // }
                // if (Physics.SphereCast(ray, 0.1f, out RaycastHit hit, 100000)) {
                dragging = true;

                if (hit.collider == colliderX) {
                    direction = Vector3.right;
                }
                else if (hit.collider == colliderY) {
                    direction = Vector3.up;
                }
                else if (hit.collider == colliderZ) {
                    direction = Vector3.forward;
                }
                
                
                Vector3 center = Camera.main.WorldToScreenPoint(transform.position);
                Vector3 forward = Camera.main.WorldToScreenPoint(transform.position + direction);
                startPos = Input.mousePosition;
                Debug.LogWarning(startPos);
                screenSpaceForward = (forward - center).normalized;
            }

            
        }
        
        

        private void EndDrag() {
            Vector3 endPos = Input.mousePosition;

            Vector3 v1 = endPos - startPos;

            float distance = Vector3.Dot(v1, screenSpaceForward);
            Debug.LogWarning($"ScreenSpaceForward : {screenSpaceForward} | v1 : {v1} | distance : {distance}");
            target.position += direction * distance/100f;

        }

    }
}