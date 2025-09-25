using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.SceneLoader
{
    public class SceneLoader : MonoBehaviour, ISceneLoader, ILoadingData
    {
#if UNITY_EDITOR
        [SerializeField] private SceneRef exclude;
#endif

        private struct SceneLoadingInfo : IEquatable<SceneLoadingInfo>
        {
            public SceneRef SceneRef;
            public AsyncOperation Operation;

            public bool Equals(SceneLoadingInfo other)
            {
                return Equals(SceneRef, other.SceneRef) && Equals(Operation, other.Operation);
            }

            public override bool Equals(object obj)
            {
                return obj is SceneLoadingInfo other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(SceneRef, Operation);
            }
        }

        private readonly List<Scene> _activeScenes = new();
        private readonly List<SceneLoadingInfo> _loadingScenes = new();

#if UNITY_EDITOR
        private void Awake()
        {
            ServiceProvider.SetService<ILoadingData>(this);
            CheckAndAddActiveScenesInEditor();
        }
#endif


        private async Task StartLoading(SceneRef sceneRef, LoadSceneMode mode)
        {
            var loadingInfo = new SceneLoadingInfo();

            try
            {
                loadingInfo.SceneRef = sceneRef;
                loadingInfo.Operation = SceneManager.LoadSceneAsync(sceneRef.Index, mode);

                _loadingScenes.Add(loadingInfo);

                await loadingInfo.Operation;

                var loadedScene = SceneManager.GetSceneByBuildIndex(sceneRef.Index);

                if (loadedScene.IsValid() && loadedScene.isLoaded)
                    _activeScenes.Add(loadedScene);
                else
                    throw new Exception($"The Scene with name {sceneRef.Name} didn't load correctly.");
            }
            catch (Exception e)
            {
                if (_loadingScenes.Remove(loadingInfo))
                    Debug.LogError($"{sceneRef.Name} {{{sceneRef.Index}}}: is not on the loading list");

                Debug.LogException(e);
            }
        }

        private async Task StartUnloading(Scene activeScenes)
        {
            await SceneManager.UnloadSceneAsync(activeScenes);
        }

        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneRef sceneRef, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            if (sceneRef == null)
            {
                Debug.LogWarning("No SceneRef assigned");
                return;
            }

            if (sceneRef.Index < 0)
            {
                Debug.LogWarning($"Not Valid SceneRef. Cause SceneIndex: {sceneRef.Name} is < 0");
                return;
                
            }

            if (IsSceneLoaded(sceneRef))
            {
                Debug.LogWarning($"Scene: {sceneRef.Name} is already loaded.");
                return;
                
            }

            if (IsSceneLoading(sceneRef))
            {
                Debug.LogWarning($"Scene: {sceneRef.Name} is already loading.");
                return;
            }

            await StartLoading(sceneRef, mode);
        }

        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneRef[] sceneRef)
        {
            List<Task> activeScenesTask = new();

            foreach (var t in sceneRef)
                activeScenesTask.Add(LoadSceneAsync(t));

            await Task.WhenAll(activeScenesTask);
            
            _loadingScenes.Clear();
        }

        public float GetCurrentLoadingProgress()
        {
            var progress = _loadingScenes.Sum(t => t.Operation.progress);

            return _loadingScenes.Count > 0 ? progress / _loadingScenes.Count : -1f;
        }

        /// <inheritdoc/>
        public async Task UnloadAll()
        {
            for (var i = _activeScenes.Count - 1; i >= 0; i--)
            {
                var scene = _activeScenes[i];

                _activeScenes.RemoveAt(i);
                await StartUnloading(scene);
            }
        }

        /// <inheritdoc/>
        public async Task UnloadAll(SceneRef exception)
        {
            for (var i = _activeScenes.Count - 1; i >= 0; i--)
            {
                if (_activeScenes[i].buildIndex == exception.Index)
                    continue;

                await StartUnloading(_activeScenes[i]);
                _activeScenes.RemoveAt(i);
            }
        }

        public async Task UnloadAll(SceneRef[] exeptions)
        {
            for (var i = _activeScenes.Count - 1; i >= 0; i--)
            {
                if (IsSceneInExceptionArray(_activeScenes[i].buildIndex, exeptions))
                    continue;

                await StartUnloading(_activeScenes[i]);
                _activeScenes.RemoveAt(i);
            }
        }

        private bool IsSceneInExceptionArray(int index, SceneRef[] sceneRefs) => sceneRefs.Any(t => index == t.Index);

        public bool IsSceneLoaded(SceneRef sceneRef) => _activeScenes.Any(t => t.buildIndex == sceneRef.Index);

        private bool IsSceneLoading(SceneRef sceneRef) => _loadingScenes.Any(t => t.SceneRef.Index == sceneRef.Index);

#if UNITY_EDITOR
        private void CheckAndAddActiveScenesInEditor()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                if (scene.buildIndex > exclude.Index && !IsSceneLoaded(scene) && !IsSceneLoading(scene))
                {
                    _activeScenes.Add(scene);
                }
            }
        }

        private bool IsSceneLoaded(Scene scene) => _activeScenes.Any(t => t.buildIndex == scene.buildIndex);

        private bool IsSceneLoading(Scene scene) => _loadingScenes.Any(t => t.SceneRef.Index == scene.buildIndex);
#endif
    }
}