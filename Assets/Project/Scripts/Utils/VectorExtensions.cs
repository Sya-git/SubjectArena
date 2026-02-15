using UnityEngine;

namespace SubjectArena.Utils
{
    public static class VectorExtensions
    {
        /// Converts a Vector3 to a Vector2 by retaining the x and z components of the Vector3
        /// while ignoring the y component.
        /// <param name="v3">The Vector3 instance to convert.</param>
        /// <returns>A Vector2 containing the x and z components of the input Vector3.</returns>
        public static Vector2 ToVector2XZ(this Vector3 v3) => new Vector2(v3.x, v3.z);

        /// Converts a Vector2 to a Vector3 by setting the x and z components of the Vector3
        /// to match the x and y components of the Vector2, while setting the y component of the
        /// Vector3 to zero.
        /// <param name="v2">The Vector2 instance to convert.</param>
        /// <returns>A Vector3 with the x component equal to the x of the Vector2,
        /// the y component set to zero, and the z component equal to the y of the Vector2.</returns>
        public static Vector3 ToVector3X0Z(this Vector2 v2) => new Vector3(v2.x, 0, v2.y);

        public static float Damp(this float current, float target, float dampTime, float deltaTime)
        {
            if (dampTime < 0.0001f) return target;

            float k = Mathf.Log(2f) / dampTime;
            float step = 1f - Mathf.Exp(-k * deltaTime);

            return Mathf.Lerp(current, target, step);
        }
        
    }
}