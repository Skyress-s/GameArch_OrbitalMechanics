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


using Script.UI;
using UnityEngine;

namespace Script.NonECSScripts
{
    public enum ArrowMode
    {
        Force,  
        Velocity,
        Disabled
    }

    public class CelestialBody : MonoBehaviour, IPlanetaryInfo
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
        
        public SolarSystem parentSystem { get; set; }

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
        
        public float AxialTilt { get; set; }
        public float RotationalSpeed { get; set; }

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

            gameObject.transform.localRotation = Quaternion.AngleAxis(AxialTilt, Vector3.right);
        }

        private void FixedUpdate()
        {
            var rotDelta = RotationalSpeed * Time.fixedDeltaTime;
            transform.Rotate(0, rotDelta, 0, Space.Self);
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

            // set value to visualize based on enum:
            var arr = ArrowType switch
            {
                ArrowMode.Force => CurrentForce,
                ArrowMode.Velocity => Velocity,
                ArrowMode.Disabled => Vector3.zero,
                _ => Vector3.zero
            };

            // set min-max values based on enum:
            var minMax = ArrowType switch
            {
                ArrowMode.Force => parentSystem.minMaxForce,
                ArrowMode.Velocity => parentSystem.minMaxVelocity,
                ArrowMode.Disabled => new Vector2(0, 1),
                _ => new Vector2(0, 1)
            };
            
            var position = transform.position;
            // var scaleFactor = position.magnitude;
            // scaleFactor *= scaleFactor;
            //
            // // minMax *= scaleFactor;
            
            var unitArr = arr.normalized;
            
            var r = MapToUnitInterval(arr.magnitude, minMax.x, minMax.y);
            r = ArrowType == ArrowMode.Force ? Mathf.Sqrt(r) : r;

            _arrowMat.SetColor(ArrowColor, new Color(r, 0, 1-r));

            _arrow.position = position + unitArr * 0.1f;
            var scale = _arrow.localScale;
            scale.z = (2f*r + 0.5f)*10f;
            _arrow.localScale = scale;
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

        private static float MapToUnitInterval(float x, float min, float max)
        {
            return (x - min) / (max - min);
        }


        public float KineticEnergy()
        {
            return 0.5f * mass * Velocity.sqrMagnitude;
        }

        public string GetInfo() {
            return $"--{name}--\nMass : {Mass}\nPosition : {transform.position}\nVelocity : {Velocity}\nKinetic Energy : {KineticEnergy()}";
        }
    }
}