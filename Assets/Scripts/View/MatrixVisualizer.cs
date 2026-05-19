using System.Collections.Generic;
using UnityEngine;

namespace MatrixMatcher.View
{
    public class MatrixVisualizer : MonoBehaviour
    {
        [SerializeField]
        private PrimitiveType _typeVisual;

        [SerializeField]
        private float _pointScale = 0.5f;

        [SerializeField]
        private Color _modelColor = Color.blue;

        [SerializeField]
        private Color _spaceColor = Color.red;

        [SerializeField]
        private Color _matchedColor = Color.green;

        private Model.MatrixData _data;
        private Controller.MatrixMatcherController _controller;
        private readonly List<GameObject> _modelObjects = new();
        private readonly List<GameObject> _spaceObjects = new();

        public void Initialize(Model.MatrixData data, Controller.MatrixMatcherController controller)
        {
            _data = data;
            _controller = controller;
            Visualize();
        }

        private void Visualize()
        {
            ClearVisuals();
            CreateVisuals(_data.Models, "Model", _modelObjects, _modelColor);
            CreateVisuals(_data.Spaces, "Space", _spaceObjects, _spaceColor);
            ApplyMatchedColors();
        }

        private void ClearVisuals()
        {
            foreach (var obj in _modelObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }

            foreach (var obj in _spaceObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }

            _modelObjects.Clear();
            _spaceObjects.Clear();
        }

        private void CreateVisuals(IReadOnlyList<Matrix4x4> matrices, string prefix, List<GameObject> targetList, Color color)
        {
            for (int i = 0; i < matrices.Count; i++)
            {
                GameObject obj = GameObject.CreatePrimitive(_typeVisual);

                var collider = obj.GetComponent<Collider>();

                if (collider != null)
                {
                    Destroy(collider);
                }

                obj.transform.position = GetPosition(matrices[i]);
                obj.transform.rotation = matrices[i].rotation;
                obj.name = $"{prefix}_{i}";
                obj.transform.localScale = Vector3.one * _pointScale;

                var renderer = obj.GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material.color = color;
                }

                targetList.Add(obj);
            }
        }

        private void ApplyMatchedColors()
        {
            var matchedModelIndices = new HashSet<int>();
            var matchedSpaceIndices = new HashSet<int>();

            foreach (var vo in _controller.ValidOffsets)
            {
                foreach (var m in vo.Matches)
                {
                    matchedModelIndices.Add(m.ModelIndex);
                    matchedSpaceIndices.Add(m.SpaceIndex);
                }
            }

            for (int i = 0; i < _modelObjects.Count; i++)
            {
                if (matchedModelIndices.Contains(i))
                {
                    var r = _modelObjects[i].GetComponent<Renderer>();

                    if (r != null)
                    {
                        r.material.color = _matchedColor;
                    }
                }
            }

            for (int i = 0; i < _spaceObjects.Count; i++)
            {
                if (matchedSpaceIndices.Contains(i))
                {
                    var r = _spaceObjects[i].GetComponent<Renderer>();

                    if (r != null)
                    {
                        r.material.color = _matchedColor;
                    }
                }
            }
        }

        private static Vector3 GetPosition(Matrix4x4 matrix)
        {
            return new Vector3(matrix.m03, matrix.m13, matrix.m23);
        }
    }
}