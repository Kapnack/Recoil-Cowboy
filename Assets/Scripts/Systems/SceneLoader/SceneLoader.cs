using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.SceneLoader
{
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
#if UNITY_EDITOR
        [SerializeField] private SceneRef exclude;
#endif

        private readonly List<Scene> _activeScenes = new();
        private readonly List<SceneRef> _loadingScenes = new();

#if UNITY_EDITOR
        private void Awake()
        {
            CheckAndAddActiveScenesInEditor();
        }
#endif


        private async Task StartLoading(SceneRef sceneRef, LoadSceneMode mode)
        {
            try
            {
                _loadingScenes.Add(sceneRef);

                await SceneManager.LoadSceneAsync(sceneRef.Index, mode);

                _loadingScenes.Remove(sceneRef);

                var loadedScene = SceneManager.GetSceneByBuildIndex(sceneRef.Index);

                if (loadedScene.IsValid() && loadedScene.isLoaded)
                    _activeScenes.Add(loadedScene);
                else
                    throw new Exception($"The Scene with name {sceneRef.Name} didn't load correctly.");
            }
            catch (Exception e)
            {
                if (_loadingScenes.Remove(sceneRef))
                    Debug.LogError($"{sceneRef.Name} {{{sceneRef.Index}}}: is not on the loading list");

                Debug.LogException(e);
            }
        }

        /// <inheritdoc/>
        private async Task StartUnloading(Scene activeScenes)
        {
            await SceneManager.UnloadSceneAsync(activeScenes);
        }

        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneRef sceneRef, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            if (sceneRef == null)
                throw new Exception("No SceneRef assigned");

            if (IsSceneLoaded(sceneRef))
                throw new Exception($"Scene: {sceneRef.Name} is already loaded.");

            if (IsSceneLoading(sceneRef))
                throw new Exception($"Scene: {sceneRef.Name} is already loading.");

            if (sceneRef.Index < 0)
                throw new Exception($"Not Valid SceneRef. Cause SceneIndex: {sceneRef.Name} is < 0");

            await StartLoading(sceneRef, mode);
        }

        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneRef[] sceneRef)
        {
            foreach (var t in sceneRef)
                await LoadSceneAsync(t);
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

        /// <inheritdoc/>
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

        private bool IsSceneLoading(SceneRef sceneRef) => _loadingScenes.Any(t => t.Index == sceneRef.Index);

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
        
        private bool IsSceneLoading(Scene scene) => _loadingScenes.Any(t => t.Index == scene.buildIndex);
#endif
    }
}