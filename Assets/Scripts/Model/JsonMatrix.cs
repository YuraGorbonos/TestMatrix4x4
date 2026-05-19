using System;

namespace MatrixMatcher.Model
{
    [Serializable]
    public class JsonMatrix
    {
        public float m00, m10, m20, m30;
        public float m01, m11, m21, m31;
        public float m02, m12, m22, m32;
        public float m03, m13, m23, m33;
    }
}