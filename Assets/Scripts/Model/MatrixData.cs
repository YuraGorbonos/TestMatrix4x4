using System.Collections.Generic;
using UnityEngine;

namespace MatrixMatcher.Model
{
    public class MatrixData : MonoBehaviour
    {
        public List<Matrix4x4> Models { get; private set; } = new();

        public List<Matrix4x4> Spaces { get; private set; } = new();

        public void SetModels(List<Matrix4x4> models)
        {
            Models = models;
        }

        public void SetSpaces(List<Matrix4x4> spaces)
        {
            Spaces = spaces;
        }
    }
}