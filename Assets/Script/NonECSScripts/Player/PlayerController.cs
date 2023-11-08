using System;
using UnityEngine;

namespace Script.NonECSScripts.Player {
    public abstract class PlayerController : MonoBehaviour {
        public static PlayerController Instance { get; protected set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        void Initialize() {
            Instance = null;
        }
    }
}