using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: OrbitRenderer.cs
// //FileType: Visual C# Source file
// //Author : Mathias Mohn Mørch
// //Created On : 12/12/2023
// //Last Modified On : 12/12/2023
// //Copy Rights : Mathias Mohn Mørch
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
namespace Script.NonECSScripts.Cosmetic {
    public class OrbitRenderer : MonoBehaviour {
        private TrailRenderer _lr;
        [SerializeField] public Color _color = Color.white;
        private IEnumerator Start() {
            yield return new WaitForSeconds(1f);
            var lineRenderGameObject = Addressables.InstantiateAsync("PlanetaryTrail", transform, false).WaitForCompletion();
            lineRenderGameObject.transform.localPosition = Vector3.zero;
            _lr = lineRenderGameObject.GetComponent<TrailRenderer>();
            _lr.SetPositions(new []{-GetComponent<CelestialBody>().Velocity.normalized, transform.position});
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new(_color, 0.0f), new(_color, 1.0f) },
                new GradientAlphaKey[] { new(1f, 0.0f), new(0.1f, 1.0f) }
            );
            _lr.colorGradient = gradient;
        }

        void Update() {
            if (_lr == null) {
                return;
            }

            if (_lr.positionCount == 0) {
                return;
                // Debug.LogWarning($"Gameobject {gameObject.name} has no positions in its line renderer.");
            }
            
            Vector3 lastTrailPos = _lr.GetPosition(0);
            
            
            if (Vector3.Distance(lastTrailPos, transform.position) < 0.4f && _lr.positionCount >= 2) { // last point close enough
                Vector3 secondLastTrailPos = _lr.GetPosition(1);
                if (Vector3.Distance(lastTrailPos, transform.position) < Vector3.Distance(secondLastTrailPos, transform.position)) {
                    _lr.time -= Time.deltaTime;
                }
            }
            else {
                _lr.time += Time.deltaTime * 0.9f;
            }
            // points.Enqueue(transform.position);
            //
            // if (points.Count > 100) {
            //     points.Dequeue();
            // }
            //
            // _lr.positionCount = points.Count;
            //
            // _lr.SetPositions(points.ToArray());
        }

    }
}