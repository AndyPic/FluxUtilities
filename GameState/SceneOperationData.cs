using UnityEngine;
using UnityEngine.SceneManagement;

namespace Flux.State
{
    [CreateAssetMenu(fileName = "SceneOperation", menuName = "Flux/Game State/Scene Operation")]
    public class SceneOperationData : A_StateOperationData
    {
        [field: Tooltip("The 'main' / 'active' scene to load (Note: Leave empty to not change main scene)")]
        [field: SerializeField] public string MainScene { get; private set; }
        [field: Tooltip("Additional scenes to additively load.")]
        [field: SerializeField] public string[] AdditionalScenes { get; private set; }

        public override A_StateOperation CreateInstance()
        {
            return new SceneOperation(this);
        }
    }

    public class SceneOperation : A_StateOperation
    {
        private readonly SceneOperationData data;

        private readonly AsyncOperation[] operations;

        public SceneOperation(SceneOperationData data)
        {
            this.data = data;

            int length = data.AdditionalScenes == null ? 0 : data.AdditionalScenes.Length;
            operations = new AsyncOperation[length + 1];
        }

        public override void Start()
        {
            for (int i = 0; i < operations.Length; i++)
            {
                operations[i] = null;
            }

            if (data.MainScene != null && data.MainScene != string.Empty)
            {
                // if we have a main scene, wait for it to load before moving to
                // additional scenes
                operations[0] = SceneManager.LoadSceneAsync(data.MainScene, LoadSceneMode.Single);
                operations[0].completed += _ =>
                {
                    for (int i = 0; i < data.AdditionalScenes.Length; i++)
                    {
                        string sceneName = data.AdditionalScenes[i];
                        operations[i + 1] = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    }
                };
            }
            else
            {
                // if no main scene, just load the additional scenes
                for (int i = 0; i < data.AdditionalScenes.Length; i++)
                {
                    string sceneName = data.AdditionalScenes[i];
                    operations[i + 1] = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                }
            }
        }

        public override void Update() { }

        public override float GetProgress()
        {
            float progressSum = 0;
            for (int i = 0; i < operations.Length; i++)
            {
                if (operations[i] == null)
                    continue;

                progressSum += operations[i].progress;
            }

            return progressSum / operations.Length;
        }
    }
}