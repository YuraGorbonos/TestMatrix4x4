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

        private Model.MatrixData _data;
        private readonly List<GameObject> _modelObjects = new();
        private readonly List<GameObject> _spaceObjects = new();

        public void Initialize(Model.MatrixData data, Controller.MatrixMatcherController controller)
        {
            _data = data;
            Visualize();
        }

        private void Visualize()
        {
            ClearVisuals();
            CreateVisuals(_data.Models, "Model", _modelObjects, _modelColor);
            CreateVisuals(_data.Spaces, "Space", _spaceObjects, _spaceColor);
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
                Vector3 pos = GetPosition(matrices[i]);

                GameObject obj = GameObject.CreatePrimitive(_typeVisual);
                obj.transform.position = pos;
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

        private static Vector3 GetPosition(Matrix4x4 matrix)
        {
            return new Vector3(matrix.m03, matrix.m13, matrix.m23);
        }
    }
}
