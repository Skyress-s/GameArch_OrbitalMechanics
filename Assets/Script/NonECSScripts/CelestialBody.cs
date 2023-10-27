// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: CelestialBody.cs
// //FileType: Visual C# Source file
// //Author : Anders P. Åsbø
// //Created On : 27/10/2023
// //Last Modified On : 27/10/2023
// //Copy Rights : Anders P. Åsbø
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.NonECSScripts
{
    public enum ArrowMode
    {
        Force,
        Velocity,
        Disabled
    }

    public class CelestialBody : MonoBehaviour
    {
        [SerializeField] [Min(1e-7f)] [Tooltip("Unit: Solar masses.")]
        private float mass = 1;

        [Header("Orbital parameters")]
        [SerializeField] private Vector3 initialVelocity = Vector3.zero;
        [SerializeField] private bool isSun;
        
        [Header("Visualizations")]
        [SerializeField] private ArrowMode arrowMode;
        [SerializeField] private Material material;

        private Transform _arrow;
        private bool _hasArrow;
        private bool _arrowEnabled = false;

        private Material _arrowMat;
        private static readonly int ArrowColor = Shader.PropertyToID("_ArrowColor");

        public bool IsSun
        {
            get => isSun;
            set => isSun = value;
        }

        public float Mass
        {
            get => mass;
            set => mass = value;
        }

        public Vector3 Velocity { get; set; }

        public ArrowMode ArrowType
        {
            get => arrowMode;
            set => arrowMode = value;
        }

        public Vector3 CurrentForce { private get; set; }

        public void Awake()
        {
            Velocity = initialVelocity;
        }

        private void Start()
        {
            _arrow = transform.GetChild(0);
            _hasArrow = _arrow != null;
            if(!_hasArrow) return;

            _arrowMat = _arrow.GetComponent<MeshRenderer>().material;
            _hasArrow = _arrowMat != null;
            
            _arrowMat.SetColor(ArrowColor, Color.clear);
        }

        private void LateUpdate()
        {
            if (!_hasArrow) return;

            if (arrowMode == ArrowMode.Disabled)
            {
                if (!_arrowEnabled) return;
                _arrowMat.SetColor(ArrowColor, Color.clear);
                
                _arrowEnabled = false;
                return;
            }
            if (!_arrowEnabled)
            {
                _arrowEnabled = true;
            }

            var arr = ArrowType switch
            {
                ArrowMode.Force => CurrentForce,
                ArrowMode.Velocity => Velocity,
                ArrowMode.Disabled => Vector3.zero,
                _ => Vector3.zero
            };
            
            var unitArr = arr.normalized;
            var r = MapToUnitInterval(arr.magnitude, 3f, 1f);
            Debug.Log($"{r} | {arr.magnitude}");
            _arrowMat.SetColor(ArrowColor, new Color(r, 1-r, 0));
            
            _arrow.position = transform.position + unitArr * 0.16f;
            _arrow.forward = arr;
        }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            var forcePos = position + CurrentForce.normalized * 0.16f;
            var velPos = position + Velocity.normalized * 0.16f;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(forcePos, forcePos + CurrentForce);

            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawLine(velPos, velPos + Velocity);
        }

        private static float MapToUnitInterval(float x, float strength, float offset)
        {
            return Mathf.Approximately(strength*x, 0) ? 0: 1 / (1 + Mathf.Exp(-strength*x + offset));
        }


        public float KineticEnergy()
        {
            return 0.5f * mass * Velocity.sqrMagnitude;
        }
    }
}