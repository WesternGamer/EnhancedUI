using VRageMath;

namespace EnhancedUI.Utils
{
    public static class LinearAlgebraExtensions
    {
        public static int[] ToArray(this Vector3I v)
        {
            return new[] { v.X, v.Y, v.Z };
        }
    }
}