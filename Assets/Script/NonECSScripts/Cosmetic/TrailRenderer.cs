using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script.NonECSScripts.Cosmetic {
    public class TrailRenderer : MonoBehaviour {
        [SerializeField] private LineRenderer _lr;

        private static readonly float k_tickRate = 0.8f;
        [SerializeField] private float TrailLengthAU = 3f;
        private Queue<Vector3> points = new();


        private void Start() {
            var lineRenderGameObject = Addressables.InstantiateAsync("PlanetaryTrail", transform, false).WaitForCompletion();
            lineRenderGameObject.transform.localPosition = Vector3.zero;
            _lr = lineRenderGameObject.GetComponent<LineRenderer>();
            
            InvokeRepeating("UpdateTrail", 0 , k_tickRate);
        }

        void UpdateTrail() {
            points.Enqueue(transform.position);

            if (points.Count > 100) {
                points.Dequeue();
            }
            
            _lr.SetPositions(points.ToArray());
        }

    }
}