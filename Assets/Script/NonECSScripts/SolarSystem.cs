// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: SolarSystem.cs
// //FileType: Visual C# Source file
// //Author : Anders P. Åsbø
// //Created On : 04/10/2023
// //Last Modified On : 04/10/2023
// //Copy Rights : Anders P. Åsbø
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.NonECSScripts
{
    public class SolarSystem : MonoBehaviour
    {
        [SerializeField] private TextAsset initialConditions;
        [SerializeField] private bool forceOrbits;
        [SerializeField] private float secondsPerSimulatedYear = 12f;
        [SerializeField] private float lengthUnitsPerAU = 3f;
        [SerializeField] private float massUnitsPerSolarMass = 100f;
        private CelestialBody[] _celestialBodies;

        private float G;
        private Vector3 _barycenterPos = Vector3.zero;
        private Vector3 _barycenterVel = Vector3.zero;
        private CelestialBody _sun;
        private float _totalMass;
        private float _potentialEnergy;
        private float _kineticEnergy;
        private float _totalEnergy;

        private void Start()
        {
            // Calculates correct gravitational constant from our unit scale:
            G = Mathf.PI * Mathf.PI * 4f * lengthUnitsPerAU * lengthUnitsPerAU *
                lengthUnitsPerAU / (massUnitsPerSolarMass * secondsPerSimulatedYear * secondsPerSimulatedYear);

            // collecting celestial bodies added in scene:
            _celestialBodies = new CelestialBody[transform.childCount];
            
            for (int i = 0; i < transform.childCount; i++)
            {
                _celestialBodies[i] = transform.GetChild(i).GetComponent<CelestialBody>();
            }

            if (initialConditions != null)
            {
                // initializes orbital data:
                InitSystemFromFile();
            }
            
            foreach (var body in _celestialBodies.Where(body => body.IsSun))
            {
                _sun = body;
                break;
            }

            if (_sun == null) _sun = _celestialBodies[0];

            if (forceOrbits)
                foreach (var body in _celestialBodies)
                {
                    if (_sun == body) continue;
                    var r = Vector3.Distance(body.transform.position, _sun.transform.position);
                    body.Velocity = new Vector3(Mathf.Sqrt(G * _sun.Mass / r), 0f, 0f);
                }

            CenterBarycenter();
            Debug.Log("After centering");
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            foreach (var body in _celestialBodies)
            {
                if (body == _sun) continue;
                var pos = body.transform.position;
                Gizmos.DrawLine(pos, new Vector3(pos.x, _sun.transform.position.y, pos.z));
            }

            DrawWireDisk(_barycenterPos, lengthUnitsPerAU * 40f, Color.green);
        }

        private static void DrawWireDisk(Vector3 position, float radius, Color color)
        {
            Color oldColor = Gizmos.color;
            color.a = 0.125f;
            Gizmos.color = color;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 1e-4f, 1));
            Gizmos.DrawSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        private void CenterBarycenter()
        {
            foreach (var body in _celestialBodies)
            {
                _barycenterVel += body.Velocity * body.Mass;
                _barycenterPos += body.transform.position * body.Mass;
                _totalMass += body.Mass;
            }

            Debug.Log($"Net mass: {_totalMass}");

            _barycenterVel /= _totalMass > 0 ? _totalMass : 1f;
            _barycenterPos /= _totalMass > 0 ? _totalMass : 1f;

            foreach (var body in _celestialBodies)
            {
                body.Velocity -= _barycenterVel;
                body.transform.position -= _barycenterPos;
            }
        }

        private void FixedUpdate()
        {
            foreach (var currentBody in _celestialBodies)
            {
                // temp variables for acceleration
                var aCurrent = Vector3.zero;
                var aNext = Vector3.zero;

                // derive acceleration at current position from interaction potential:
                foreach (var otherBody in _celestialBodies) aCurrent += Gravity(currentBody, otherBody);

                currentBody.CurrentForce = aCurrent;

                // calculate new position:
                currentBody.transform.position += currentBody.Velocity * Time.fixedDeltaTime +
                                                  aCurrent * (0.5f * Time.fixedDeltaTime * Time.fixedDeltaTime);

                // derive new acceleration at updated position using interaction potential:
                foreach (var otherBody in _celestialBodies) aNext += Gravity(currentBody, otherBody);

                // calculate new velocity using mean acceleration between old and new position:
                currentBody.Velocity += (aCurrent + aNext) * (0.5f * Time.fixedDeltaTime);
            }
        }

        private Vector3 Gravity(Component A, CelestialBody B)
        {
            if (A == B) return Vector3.zero;
            var rAB = B.transform.position - A.transform.position;
            return G * B.Mass * rAB.normalized / rAB.sqrMagnitude;
        }

        private void InitSystemFromFile()
        {
            // defines which characters to split file into lines on:
            var fileDelimiters = new[] { "\r\n", "\r", "\n" };

            // split file into array of non-empty lines:
            var lines = initialConditions.text.Split(fileDelimiters, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 1)
            {
                Debug.LogWarning($"{initialConditions.name} was empty!");
                return;
            }

            var positions = new Vector3[lines.Length];
            var velocities = new Vector3[lines.Length];
            var masses = new float[lines.Length];

            for (var i = 0; i < lines.Length; i++)
            {
                // split line and read coordinates:
                var elements = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (elements.Length < 7)
                {
                    Debug.LogWarning($"{initialConditions.name} is missing data on line {i}");
                    continue;
                }

                positions[i] = new Vector3(
                    float.Parse(elements[0], CultureInfo.InvariantCulture),
                    float.Parse(elements[2], CultureInfo.InvariantCulture),
                    float.Parse(elements[1], CultureInfo.InvariantCulture)
                );
                
                velocities[i] = new Vector3(
                    float.Parse(elements[3], CultureInfo.InvariantCulture),
                    float.Parse(elements[5], CultureInfo.InvariantCulture),
                    float.Parse(elements[4], CultureInfo.InvariantCulture)
                );

                masses[i] = float.Parse(elements[6], CultureInfo.InvariantCulture);
            }

            var numInits = positions.Length < _celestialBodies.Length ? positions.Length : _celestialBodies.Length;

            for (int i = 0; i < numInits; i++)
            {
                _celestialBodies[i].IsSun = i < 1;
                _celestialBodies[i].transform.position = positions[i] * lengthUnitsPerAU;
                _celestialBodies[i].Velocity = velocities[i] * (lengthUnitsPerAU/secondsPerSimulatedYear);
                _celestialBodies[i].Mass = masses[i] * massUnitsPerSolarMass;
            }
        }
    }
}