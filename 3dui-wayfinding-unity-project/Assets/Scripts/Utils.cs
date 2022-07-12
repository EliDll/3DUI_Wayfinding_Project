using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }
    }
    
    // Vector 3 Comparison class. Used for using Vector3 as key in Dictionaries.
    public class Vector3Comparer : IEqualityComparer<Vector3>
    {
        private readonly float _tolerance;

        public Vector3Comparer(float tolerance)
        {
            _tolerance = tolerance;
        }

        public bool Equals(Vector3 vec1, Vector3 vec2) {
            return Math.Abs(vec1.x - vec2.x) < _tolerance && Math.Abs(vec1.y - vec2.y) < _tolerance && Math.Abs(vec1.z - vec2.z) < _tolerance;
        }
 
        public int GetHashCode (Vector3 vec){
            return Mathf.FloorToInt (vec.x) ^ Mathf.FloorToInt (vec.y) << 2 ^ Mathf.FloorToInt (vec.z) >> 2;
        }
    }

    
}