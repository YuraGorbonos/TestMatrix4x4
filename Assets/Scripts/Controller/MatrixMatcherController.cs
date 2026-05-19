using System.Collections.Generic;
using UnityEngine;

namespace MatrixMatcher.Controller
{
    public class MatrixMatcherController : MonoBehaviour
    {
        public struct MatrixMatch
        {
            public int ModelIndex { get; set; }

            public int SpaceIndex { get; set; }

            public Matrix4x4 Offset { get; set; }
        }

        public IReadOnlyList<MatrixMatch> Matches => _matches;

        private Model.MatrixData _data;
        private readonly List<MatrixMatch> _matches = new();

        public void Initialize(Model.MatrixData data)
        {
            _data = data;
        }
    }
}
