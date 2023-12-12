using System;
using System.Collections.Generic;
using Script.NonECSScripts;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: AsteroidBelt.cs
// //FileType: Visual C# Source file
// //Author : Mathias Mohn Mørch
// //Created On : 12/12/2023
// //Last Modified On : 12/12/2023
// //Copy Rights : Mathias Mohn Mørch
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
public class AsteroidBelt : MonoBehaviour {
    
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject systemObject;
    
    [SerializeField] private int asteroidCount = 100;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float beltRotationSpeed = 1f;
    [SerializeField] private float innerSize = 2f;
    [SerializeField] private float outerSize = 5f;

    private SolarSystem _system;
    private bool _hasSystem;

    private List<Transform> asteroids = new List<Transform>();

    private void Start()
    {

        _hasSystem = systemObject.TryGetComponent<SolarSystem>(out _system);
        _hasSystem = _system != null;
        
        float stepAngles = 360f / (float)asteroidCount;
        for (int i = 0; i < asteroidCount; i++) {
            Vector3 position = transform.position + Quaternion.Euler(0, stepAngles * i, 0) * Vector3.forward * UnityEngine.Random.Range(innerSize, outerSize);
            Transform trans = Instantiate(asteroidPrefab, position, Random.rotation, transform).transform;
            trans.localScale *= Random.Range(0.04f, 0.2f);
            asteroids.Add(trans);
        }

        if (!_hasSystem) return;
        beltRotationSpeed = -Mathf.Sqrt(_system.G * _system.sun.Mass * 10f / innerSize);
    }


    private void Update() {
        Vector3 rot = transform.localEulerAngles;
        rot.y += beltRotationSpeed * Time.deltaTime;
        transform.localEulerAngles = rot;
        
        foreach (var asteroid in asteroids) {
            asteroid.rotation *= Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0) ;
        }
    }

    private void OnDrawGizmosSelected() {
        Handles.DrawWireArc(transform.position, transform.up, transform.right, 360, innerSize, 3f);
        Handles.DrawWireArc(transform.position, transform.up, transform.right, 360, outerSize, 3f);
    }

    private void OnValidate() {
        if (innerSize < 0) {
            innerSize = 0f;
        }
        if (outerSize < innerSize) {
            outerSize = innerSize;
        }
    }
}