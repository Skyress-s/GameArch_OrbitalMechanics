using OM.SolarPanel;
using UnityEngine;

public class SolarPanel : MonoBehaviour {
    [SerializeField] private Sun _sun;
    private bool x, y, z;
    private void Update() {
        Vector3 toSun = (_sun.transform.position - transform.position).normalized;
    }
}
