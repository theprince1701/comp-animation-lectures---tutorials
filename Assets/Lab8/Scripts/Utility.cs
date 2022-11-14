using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{

    public static float Attenuate(Vector3 target, Vector3 current, float length)
    {
        return Mathf.Clamp01((target - current).magnitude / length);
    }
}
