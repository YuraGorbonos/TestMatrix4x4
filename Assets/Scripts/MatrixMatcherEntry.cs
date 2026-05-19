using MatrixMatcher.Controller;
using MatrixMatcher.Model;
using MatrixMatcher.Services;
using MatrixMatcher.View;
using UnityEngine;

namespace MatrixMatcher.Entry
{
    public class MatrixMatcherEntry : MonoBehaviour
    {
        [SerializeField]
        private string _modelsFileName = "model.json";

        [SerializeField]
        private string _spacesFileName = "space.json";

        [SerializeField]
        private MatrixVisualizer _visualizer;

        private void Start()
        {
            var mainCamera = Camera.main;

            if (mainCamera != null && mainCamera.GetComponent<FreeCameraController>() == null)
            {
                mainCamera.gameObject.AddComponent<FreeCameraController>();
            }

            var models = JsonLoader.LoadFromFile(_modelsFileName);
            var spaces = JsonLoader.LoadFromFile(_spacesFileName);

            var data = gameObject.AddComponent<MatrixData>();
            data.SetModels(models);
            data.SetSpaces(spaces);

            var controller = gameObject.AddComponent<MatrixMatcherController>();
            controller.Initialize(data);

            _visualizer.Initialize(data, controller);
        }
    }
}