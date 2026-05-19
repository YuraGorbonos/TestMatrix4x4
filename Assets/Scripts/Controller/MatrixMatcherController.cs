using System.Collections.Generic;
using System.IO;
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

        public struct ValidOffset
        {
            public Matrix4x4 Offset;
            public List<MatrixMatch> Matches;
        }

        private class SpaceLookup
        {
            private readonly Dictionary<int, List<int>> _buckets = new();
            private readonly List<Matrix4x4> _spaces;

            public SpaceLookup(List<Matrix4x4> spaces)
            {
                _spaces = spaces;

                for (int i = 0; i < spaces.Count; i++)
                {
                    var hash = ComputeHash(spaces[i]);

                    if (!_buckets.ContainsKey(hash))
                    {
                        _buckets[hash] = new List<int>();
                    }

                    _buckets[hash].Add(i);
                }
            }

            public bool TryFind(Matrix4x4 target, out int index)
            {
                var hash = ComputeHash(target);

                if (_buckets.TryGetValue(hash, out var candidates))
                {
                    foreach (var idx in candidates)
                    {
                        if (AreEqual(target, _spaces[idx]))
                        {
                            index = idx;
                            return true;
                        }
                    }
                }

                index = -1;
                return false;
            }

            private static int ComputeHash(Matrix4x4 m)
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + Quant(m.m00);
                    hash = hash * 31 + Quant(m.m01);
                    hash = hash * 31 + Quant(m.m02);
                    hash = hash * 31 + Quant(m.m03);
                    hash = hash * 31 + Quant(m.m10);
                    hash = hash * 31 + Quant(m.m11);
                    hash = hash * 31 + Quant(m.m12);
                    hash = hash * 31 + Quant(m.m13);
                    hash = hash * 31 + Quant(m.m20);
                    hash = hash * 31 + Quant(m.m21);
                    hash = hash * 31 + Quant(m.m22);
                    hash = hash * 31 + Quant(m.m23);
                    hash = hash * 31 + Quant(m.m30);
                    hash = hash * 31 + Quant(m.m31);
                    hash = hash * 31 + Quant(m.m32);
                    hash = hash * 31 + Quant(m.m33);
                    return hash;
                }
            }

            private static int Quant(float v)
            {
                return Mathf.RoundToInt(v * 10000f);
            }

            private static bool AreEqual(Matrix4x4 a, Matrix4x4 b, float tol = 1e-5f)
            {
                for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    if (Mathf.Abs(a[i, j] - b[i, j]) > tol)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [System.Serializable]
        private class JsonResult
        {
            public List<JsonValidOffset> ValidOffsets;
        }

        [System.Serializable]
        private class JsonValidOffset
        {
            public JsonMatrix Offset;
            public List<JsonMatch> Matches;
        }

        [System.Serializable]
        private class JsonMatch
        {
            public int ModelIndex;
            public int SpaceIndex;
        }

        [System.Serializable]
        private class JsonMatrix
        {
            public float m00, m10, m20, m30;
            public float m01, m11, m21, m31;
            public float m02, m12, m22, m32;
            public float m03, m13, m23, m33;
        }

        public IReadOnlyList<ValidOffset> ValidOffsets => _validOffsets;

        private Model.MatrixData _data;
        private readonly List<ValidOffset> _validOffsets = new();

        public void Initialize(Model.MatrixData data)
        {
            _data = data;
            FindAllValidOffsets();
            SaveResultsToJson("result.json");
        }

        private void FindAllValidOffsets()
        {
            _validOffsets.Clear();

            if (_data.Models.Count == 0 || _data.Spaces.Count == 0)
            {
                return;
            }

            var lookup = new SpaceLookup(_data.Spaces);
            var seenOffsets = new List<Matrix4x4>();

            for (int j = 0; j < _data.Spaces.Count; j++)
            {
                for (int i = 0; i < _data.Models.Count; i++)
                {
                    var model = _data.Models[i];

                    if (Mathf.Approximately(model.determinant, 0f))
                    {
                        continue;
                    }

                    var offset = _data.Spaces[j] * model.inverse;

                    bool alreadySeen = false;

                    foreach (var existing in seenOffsets)
                    {
                        if (AreEqual(offset, existing))
                        {
                            alreadySeen = true;
                            break;
                        }
                    }

                    if (alreadySeen)
                    {
                        continue;
                    }

                    bool allMatch = true;

                    foreach (var m in _data.Models)
                    {
                        if (!lookup.TryFind(offset * m, out _))
                        {
                            allMatch = false;
                            break;
                        }
                    }

                    if (allMatch)
                    {
                        seenOffsets.Add(offset);
                        var vo = new ValidOffset { Offset = offset, Matches = new List<MatrixMatch>() };

                        for (int mi = 0; mi < _data.Models.Count; mi++)
                        {
                            var transformed = offset * _data.Models[mi];

                            if (lookup.TryFind(transformed, out int si))
                            {
                                vo.Matches.Add(new MatrixMatch { ModelIndex = mi, SpaceIndex = si, Offset = offset });
                            }
                        }

                        _validOffsets.Add(vo);
                    }
                }
            }

            Debug.Log("=== Результат поиска ===");
            Debug.Log($"Найдено валидных смещений: {_validOffsets.Count}");

            int totalMatches = 0;

            for (int v = 0; v < _validOffsets.Count; v++)
            {
                Debug.Log($"--- Смещение #{v + 1} ---");
                Debug.Log($"Offset:\n{_validOffsets[v].Offset}");

                foreach (var m in _validOffsets[v].Matches)
                {
                    Debug.Log($"  Модель[{m.ModelIndex}] → Пространство[{m.SpaceIndex}]");
                    totalMatches++;
                }
            }

            Debug.Log($"Всего совпадений (пар модель-пространство): {totalMatches}");
        }

        private void SaveResultsToJson(string fileName)
        {
            var result = new JsonResult
                         {
                             ValidOffsets = new List<JsonValidOffset>()
                         };

            foreach (var vo in _validOffsets)
            {
                var jsonVo = new JsonValidOffset
                             {
                                 Offset = MatrixToJson(vo.Offset),
                                 Matches = new List<JsonMatch>()
                             };

                foreach (var m in vo.Matches)
                {
                    jsonVo.Matches.Add(new JsonMatch
                                       {
                                           ModelIndex = m.ModelIndex,
                                           SpaceIndex = m.SpaceIndex
                                       });
                }

                result.ValidOffsets.Add(jsonVo);
            }

            string json = JsonUtility.ToJson(result, true);
            string path = Path.Combine(Application.streamingAssetsPath, fileName);

            try
            {
                File.WriteAllText(path, json);
                Debug.Log($"Результаты сохранены в: {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка сохранения файла: {e.Message}");
            }
        }

        private static JsonMatrix MatrixToJson(Matrix4x4 matrix)
        {
            return new JsonMatrix
                   {
                       m00 = matrix.m00, m10 = matrix.m10, m20 = matrix.m20, m30 = matrix.m30,
                       m01 = matrix.m01, m11 = matrix.m11, m21 = matrix.m21, m31 = matrix.m31,
                       m02 = matrix.m02, m12 = matrix.m12, m22 = matrix.m22, m32 = matrix.m32,
                       m03 = matrix.m03, m13 = matrix.m13, m23 = matrix.m23, m33 = matrix.m33
                   };
        }

        private static bool AreEqual(Matrix4x4 a, Matrix4x4 b, float tol = 1e-5f)
        {
            for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
            {
                if (Mathf.Abs(a[i, j] - b[i, j]) > tol)
                {
                    return false;
                }
            }

            return true;
        }
    }
}