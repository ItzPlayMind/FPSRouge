using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public delegate void OnChangeValue<T>(ref T value);
    public static float AngleBetween(Vector3 a, Vector3 b)
    {
        return Mathf.Acos(Vector3.Dot(a, b) / (Vector3.Magnitude(a) * Vector3.Magnitude(b))) * Mathf.Rad2Deg;
    }


}
