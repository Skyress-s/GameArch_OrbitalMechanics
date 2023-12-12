using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: UISingleton.cs
// //FileType: Visual C# Source file
// //Author : Mathias Mohn Mørch
// //Created On : 12/12/2023
// //Last Modified On : 12/12/2023
// //Copy Rights : Mathias Mohn Mørch
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
public class UISingleton : MonoBehaviour
{
    public static UISingleton Instance { get; private set; }
    
    [SerializeField] private Canvas canvas;
    [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
    
    
    private Transform targetTransform;
    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }


    private void Update() {
        if (targetTransform == null) {
            return;
        }

        transform.position = targetTransform.position;
        Vector3 screenSpace = Camera.main.WorldToScreenPoint(targetTransform.position);
        
        
        
        // transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        _verticalLayoutGroup.transform.position = screenSpace;
    }

    public void Empty() {
        for (int i = _verticalLayoutGroup.transform.childCount - 1; i >= 0; i--) {
            Destroy(_verticalLayoutGroup.transform.GetChild(i).gameObject);
        }
    }
    
    public RectTransform RequestAt(Transform targetTransform, Vector3 offset) {
        // while (_verticalLayoutGroup.transform.childCount > 0) {
        //     Destroy(_verticalLayoutGroup.transform.GetChild(0).gameObject);
        // }
        Empty();

        
        
        this.targetTransform = targetTransform;
        return _verticalLayoutGroup.GetComponent<RectTransform>();

    }
    
}