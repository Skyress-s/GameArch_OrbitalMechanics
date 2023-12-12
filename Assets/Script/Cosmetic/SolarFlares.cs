// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: SolarFlares.cs
// //FileType: Visual C# Source file
// //Author : Anders P. Åsbø
// //Created On : 12/12/2023
// //Last Modified On : 12/12/2023
// //Copy Rights : Anders P. Åsbø
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////

using UnityEngine;

public class SolarFlares : MonoBehaviour
{
    [SerializeField] private Transform sun;
    private bool _hasSun;

    private void Start()
    {
        _hasSun = sun != null;
    }

    private void LateUpdate()
    {
        if (_hasSun) transform.position = sun.position;
    }
}