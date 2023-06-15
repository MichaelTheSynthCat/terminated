using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IntegrityMask script controls the device's integrity bar
public class IntegrityMask : MonoBehaviour
{
    private Transform integrity_bar; // transform of the green bar, which shows remaining integrity points
    public float zero_x = -0.3091f; // x position when the green bar is not visible
    void Awake()
    {
        foreach(Transform child in transform)
        {
            if(child.name == "Integrity Bar" || child.name == "Firewall Bar")
            {
                integrity_bar = child;
            }
        }
    }

    public void Show(float part) // update integrity bar
    {
        Vector3 vector3 = integrity_bar.localPosition;
        vector3.x = (1 - part) * zero_x;
        integrity_bar.localPosition = vector3;
    }
}
