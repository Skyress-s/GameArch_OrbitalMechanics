using UnityEngine;

// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: PlayerController.cs
// //FileType: Visual C# Source file
// //Author : Mathias Mohn Mørch
// //Created On : 12/12/2023
// //Last Modified On : 12/12/2023
// //Copy Rights : Mathias Mohn Mørch
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
namespace Script.NonECSScripts.Player {
    public abstract class PlayerController : MonoBehaviour {
        public static PlayerController Instance { get; protected set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        void Initialize() {
            Instance = null;
        }
    }
}