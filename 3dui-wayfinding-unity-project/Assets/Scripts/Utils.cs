using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        // on xz plane
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 forward)
        {
            float forwardAngle = Mathf.Atan2(-forward.z, forward.x) * Mathf.Rad2Deg;
            float globalAngle = Mathf.Atan2(v1.z - v2.z, v1.x - v2.x) * Mathf.Rad2Deg;

            // base to 0-360 -> modulo by 360 -> rebase to -180 to 180
            return mod((int)forwardAngle + globalAngle + 180, 360) - 180;
        }
        public static float mod(float x, float m) {
            return (x%m + m)%m;
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