// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: SolarSystem.cs
// //FileType: Visual C# Source file
// //Author : Anders P. Åsbø
// //Created On : 9/11/2023
// //Last Modified On : 10/12/2023
// //Copy Rights : Anders P. Åsbø
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
// ReSharper disable LoopCanBeConvertedToQuery

namespace Script.NonECSScripts
{
    public class SolarSystem : MonoBehaviour
    {
        [Header("Defaults")]
        [SerializeField] private TextAsset initialConditions;

        [SerializeField] private CelestialBody defaultBody;
        [SerializeField] private bool forceOrbits;
        
        [Header("Unit Scaling")]
        [SerializeField] private float secondsPerSimulatedYear = 12f;
        [SerializeField] private float lengthUnitsPerAU = 3f;
        [SerializeField] private float massUnitsPerSolarMass = 100f;
        
        private Vector3 _barycenterPos = Vector3.zero;
        private Vector3 _barycenterVel = Vector3.zero;
        private List<CelestialBody> _celestialBodies;
        private float _kineticEnergy;
        private float _potentialEnergy;
        private CelestialBody _sun;
        private float _totalEnergy;
        private float _totalMass;
        private Vector2 _minMaxForce = new Vector2(float.MaxValue, float.MinValue);
        private Vector2 _minMaxVelocity = new Vector2(float.MaxValue, float.MinValue);

        // ReSharper disable once InconsistentNaming
        private float G { get; set; }
        public float systemMass => _totalMass;

        public Vector2 minMaxForce => _minMaxForce;

        public Vector2 minMaxVelocity => _minMaxVelocity;

        private void Start()
        {
            // Kepler's 3rd law of planetary motion: a^3/T^2 = G*M/(4*pi^2),
            //  - a: semi-major axis of planet's orbit (set to mean distance between Earth and Sun: 1 astronomical unit).
            //  - T: orbital period of planet (set to Earth's: 1 year).
            //  - G: Newton's gravitational constant (needs to be calculated).
            //  - M: Mass of system's star (set to the Sun's: 1 Solar Mass).

            // Calculates correct gravitational constant G = 4*(pi^2)*(a^3)/(M*T^2) from our unit scale:
            G = Mathf.PI * Mathf.PI * 4f * lengthUnitsPerAU * lengthUnitsPerAU * lengthUnitsPerAU / (massUnitsPerSolarMass * secondsPerSimulatedYear * secondsPerSimulatedYear);
            Debug.Log($"G = {G}");
            
            // collecting celestial bodies added in scene:
            _celestialBodies = new List<CelestialBody>();

            var unitScales = new Dictionary<string, float>
            {
                {"length", lengthUnitsPerAU},
                {"time", secondsPerSimulatedYear},
                {"mass", massUnitsPerSolarMass},
            };
            for (var i = 0; i < transform.childCount; i++)
            {
                _celestialBodies.Add(transform.GetChild(i).GetComponent<CelestialBody>());
                _celestialBodies[^1].parentSystem = this;
                _celestialBodies[^1].ArrowType = ArrowMode.Disabled;
                _celestialBodies[^1].unitScales = unitScales;
            }

            if (initialConditions != null)
                // initializes orbital data from file:
                InitSystemFromFile();

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
                
                // find min and max force for visualization
                var forceAnalog = aCurrent.magnitude;
                _minMaxForce.x = Mathf.Min(forceAnalog, minMaxForce.x);
                _minMaxForce.y = Mathf.Max(forceAnalog, minMaxForce.y);

                // calculate new position using equation of motion r_n+1 = r_n + v_n * dt + 0.5*a_n*dt^2:
                currentBody.transform.position += currentBody.Velocity * Time.fixedDeltaTime +
                                                  aCurrent * (0.5f * Time.fixedDeltaTime * Time.fixedDeltaTime);

                // derive new acceleration at updated position using interaction potential:
                foreach (var otherBody in _celestialBodies) aNext += Gravity(currentBody, otherBody);

                // calculate new velocity using v_n+1 = v_n + 0.5*(a_n+1 + a_n)*dt:
                currentBody.Velocity += (aCurrent + aNext) * (0.5f * Time.fixedDeltaTime);

                // find min and max velocity for visualization
                var vel = currentBody.Velocity.magnitude;
                _minMaxVelocity.x = Mathf.Min(forceAnalog, minMaxForce.x);
                _minMaxVelocity.y = Mathf.Max(forceAnalog, minMaxForce.y);
            }
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
            var oldColor = Gizmos.color;
            color.a = 0.125f;
            Gizmos.color = color;
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 1e-4f, 1));
            Gizmos.DrawSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        private void CenterBarycenter()
        {
            // Calculate center of mass' position and velocity:
            foreach (var body in _celestialBodies)
            {
                _barycenterVel += body.Velocity * body.Mass;
                _barycenterPos += body.transform.position * body.Mass;
                _totalMass += body.Mass;
            }

            Debug.Log($"Net mass: {_totalMass}");

            _barycenterVel /= _totalMass > 0 ? _totalMass : 1f;
            _barycenterPos /= _totalMass > 0 ? _totalMass : 1f;

            // make center of mass stationary at coordinates (0,0,0):
            foreach (var body in _celestialBodies)
            {
                body.Velocity -= _barycenterVel;
                body.transform.position -= _barycenterPos;
            }
        }

        private Vector3 Gravity(Component A, CelestialBody B)
        {
            if (A == B) return Vector3.zero; // no interaction with self

            // calculate acceleration due to gravity from body B on A:
            var rAB = B.transform.position - A.transform.position;
            return G * B.Mass * rAB.normalized / rAB.sqrMagnitude;
        }

        /// <summary>
        /// Adds planet to system
        /// </summary>
        /// <param name="pos">position to add planet at</param>
        /// <param name="forceOrbit">if true - force orbit around sun</param>
        /// <param name="mass">mass of new planet [solar masses]</param>
        /// <param name="axialTilt">axialTilt of planet [deg]</param>
        /// <param name="rotRate">rotation rate of planet [deg/day]</param>
        /// <returns></returns>
        public CelestialBody AddPlanet(Vector3 pos, bool forceOrbit = true, float mass = 3e-6f, float axialTilt = 23.44f, float rotRate = 366.2422f)
        {
            var newBody = Instantiate<CelestialBody>(defaultBody, pos, Quaternion.identity, transform);
            newBody.parentSystem = this;
            newBody.ArrowType = ArrowMode.Disabled;
            newBody.Mass = mass * massUnitsPerSolarMass;
            newBody.AxialTilt = axialTilt;
            newBody.RotationalSpeed = rotRate * secondsPerSimulatedYear * 366.2422f;
            newBody.unitScales = new Dictionary<string, float>
            {
                {"length", lengthUnitsPerAU},
                {"time", secondsPerSimulatedYear},
                {"mass", massUnitsPerSolarMass},
            };

            if (forceOrbit)
            {
                var rVec = pos - _sun.transform.position;
                newBody.Velocity = Mathf.Sqrt(G * _sun.Mass / rVec.magnitude) * Vector3.Cross(rVec,Vector3.up).normalized;
            }

            _celestialBodies.Add(newBody);
            return _celestialBodies[^1];
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
            var axialTilts = new float[lines.Length];
            var rotationalSpeeds = new float[lines.Length];

            for (var i = 0; i < lines.Length; i++)
            {
                // split line and read coordinates:
                var elements = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (elements.Length < 9)
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
                axialTilts[i] = float.Parse(elements[7], CultureInfo.InvariantCulture);
                rotationalSpeeds[i] = float.Parse(elements[8], CultureInfo.InvariantCulture);
            }

            var numInits = positions.Length < _celestialBodies.Count ? positions.Length : _celestialBodies.Count;

            var daysToSecPerYear = secondsPerSimulatedYear * 366.2422f;
            // scale system data to chosen units:
            for (var i = 0; i < numInits; i++)
            {
                _celestialBodies[i].IsSun = i < 1;
                _celestialBodies[i].transform.position = positions[i] * lengthUnitsPerAU;
                _celestialBodies[i].Velocity = velocities[i] / secondsPerSimulatedYear * lengthUnitsPerAU;
                _celestialBodies[i].Mass = masses[i] * massUnitsPerSolarMass;
                _celestialBodies[i].AxialTilt = axialTilts[i];
                _celestialBodies[i].RotationalSpeed = rotationalSpeeds[i] * daysToSecPerYear;
            }
        }
    }
}