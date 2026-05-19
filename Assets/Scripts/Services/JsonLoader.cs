using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MatrixMatcher.Services
{
    public static class JsonLoader
    {
        [System.Serializable]
        private class JsonMatrix
        {
            public float m00, m10, m20, m30;
            public float m01, m11, m21, m31;
            public float m02, m12, m22, m32;
            public float m03, m13, m23, m33;

            public Matrix4x4 ToMatrix4x4()
            {
                return new Matrix4x4
                       {
                           m00 = m00,
                           m10 = m10,
                           m20 = m20,
                           m30 = m30,
                           m01 = m01,
                           m11 = m11,
                           m21 = m21,
                           m31 = m31,
                           m02 = m02,
                           m12 = m12,
                           m22 = m22,
                           m32 = m32,
                           m03 = m03,
                           m13 = m13,
                           m23 = m23,
                           m33 = m33
                       };
            }
        }

        [System.Serializable]
        private class JsonMatrixArray
        {
            public JsonMatrix[] matrices;
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
                var list = new List<Matrix4x4>();

                foreach (var jm in wrapper.matrices)
                {
                    list.Add(jm.ToMatrix4x4());
                }

                return list;
            }

            var single = JsonUtility.FromJson<JsonMatrix>(json);

            return single != null
                       ? new List<Matrix4x4> { single.ToMatrix4x4() }
                       : new List<Matrix4x4>();
        }
    }
}