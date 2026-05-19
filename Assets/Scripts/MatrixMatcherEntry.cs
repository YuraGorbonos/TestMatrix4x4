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
        private string _resultFileName = "result.json";

        [SerializeField]
        private MatrixVisualizer _visualizer;

        private void Start()
        {
            var models = JsonLoader.LoadFromFile(_modelsFileName);
            var spaces = JsonLoader.LoadFromFile(_spacesFileName);

            var data = new MatrixData();
            data.SetModels(models);
            data.SetSpaces(spaces);

            var controller = new MatrixMatcherController();
            controller.Initialize(data);
            controller.SaveResults(_resultFileName);

            _visualizer.Initialize(data, controller);
        }
    }
}