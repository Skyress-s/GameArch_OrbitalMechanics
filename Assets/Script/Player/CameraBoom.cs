using System;
using UnityEngine;

// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: CameraBoom.cs
// //FileType: Visual C# Source file
// //Author : Mathias Mohn Mørch
// //Created On : 12/12/2023
// //Last Modified On : 12/12/2023
// //Copy Rights : Mathias Mohn Mørch
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
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