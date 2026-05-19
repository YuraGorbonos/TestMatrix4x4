using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MatrixMatcher.Services
{
    public static class JsonLoader
    {
        [System.Serializable]
        private class JsonMatrixArray
        {
            public Model.JsonMatrix[] matrices;
        }

        public static List<Matrix4x4> LoadFromFile(string fileName)
        {
            string path = BuildPath(fileName);

            if (!File.Exists(path))
            {
                Debug.LogError($"File not found: {path}");
                return new List<Matrix4x4>();
            }

            string json = File.ReadAllText(path).Trim();
            return ParseMatrices(json);
        }

        private static string BuildPath(string fileName)
        {
            string streamingPath = Path.Combine(Application.streamingAssetsPath, fileName);

            if (File.Exists(streamingPath))
            {
                return streamingPath;
            }

            return Path.Combine(Application.dataPath, "StreamingAssets", fileName);
        }

        private static List<Matrix4x4> ParseMatrices(string json)
        {
            if (json.StartsWith("["))
            {
                json = $"{{\"matrices\":{json}}}";
            }

            var wrapper = JsonUtility.FromJson<JsonMatrixArray>(json);

            if (wrapper?.matrices is { Length: > 0 })
            {
                var list = new List<Matrix4x4>(wrapper.matrices.Length);

                foreach (var jm in wrapper.matrices)
                {
                    list.Add(ToMatrix4x4(jm));
                }

                return list;
            }

            var single = JsonUtility.FromJson<Model.JsonMatrix>(json);

            return single != null
                       ? new List<Matrix4x4> { ToMatrix4x4(single) }
                       : new List<Matrix4x4>();
        }

        private static Matrix4x4 ToMatrix4x4(Model.JsonMatrix jm)
        {
            return new Matrix4x4
                   {
                       m00 = jm.m00, m10 = jm.m10, m20 = jm.m20, m30 = jm.m30,
                       m01 = jm.m01, m11 = jm.m11, m21 = jm.m21, m31 = jm.m31,
                       m02 = jm.m02, m12 = jm.m12, m22 = jm.m22, m32 = jm.m32,
                       m03 = jm.m03, m13 = jm.m13, m23 = jm.m23, m33 = jm.m33
                   };
        }
    }
}