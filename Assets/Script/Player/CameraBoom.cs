using System;
using UnityEngine;

namespace Script.NonECSScripts.Player {
    public class CameraBoom : MonoBehaviour {
        public float targetLength = 2f;
        [SerializeField] private float _sharpness = 5f;
        private void Update() {
            Vector3 currentLocalPos = transform.localPosition;
            float currentLength = transform.localPosition.z;
        }
    }
}