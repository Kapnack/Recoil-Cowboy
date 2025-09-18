using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.SceneLoader
{
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
#if UNITY_EDITOR
        [SerializeField] private SceneRef exclude;
#endif

        private readonly List<Scene> activeScenes = new();

#if UNITY_EDITOR
        private IEnumerator Start()
        {
            if (SceneManager.sceneCount == 0)
                yield return null;

            CheckAndAddActiveScenesInEditor();
        }
#endif

        private IEnumerator LoadSceneCoroutine(SceneRef sceneRef, LoadSceneMode mode)
        {
            var asyncOp = SceneManager.LoadSceneAsync(sceneRef.Index, mode);

            while (!asyncOp.isDone)
                yield return null;
        
            var loadedScene = SceneManager.GetSceneByBuildIndex(sceneRef.Index);

            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                activeScenes.Add(loadedScene);
            }
            else
            {
                Debug.LogWarning($"The Scene with name {sceneRef.Name} didn't load correctly.");
            }
        }

        /// <inheritdoc/>
        private static IEnumerator UnLoadSceneCoroutine(Scene activeScenes)
        {
            var asyncOp = SceneManager.UnloadSceneAsync(activeScenes);

            while (asyncOp != null && !asyncOp.isDone)
                yield return null;
        }

        /// <inheritdoc/>
        public void LoadScene(SceneRef sceneRef, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            if (sceneRef == null)
            {
                Debug.LogWarning("No SceneRef assigned");
                return;
            }

            if (IsSceneLoaded(sceneRef))
            {
                Debug.LogWarning($"Scene is already loaded.");
                return;
            }

            if (sceneRef.Index < 0)
            {
                Debug.LogWarning($"Not Valid SceneRef. Cause SceneIndex: {sceneRef.Index} is < 0");
                return;
            }

            StartCoroutine(LoadSceneCoroutine(sceneRef, mode));
        }

        /// <inheritdoc/>
        public void LoadSceneAsync(SceneRef[] sceneRef, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            foreach (var t in sceneRef)
                LoadScene(t, mode);
        }

        /// <inheritdoc/>
        public void UnloadAll()
        {
            for (var i = activeScenes.Count - 1; i >= 0; i--)
            {
                var scene = activeScenes[i];
            
                StartCoroutine(UnLoadSceneCoroutine(scene));
                activeScenes.RemoveAt(i);
            }
        }

        /// <inheritdoc/>
        public void UnloadAll(SceneRef exception)
        {
            for (var i = activeScenes.Count - 1; i >= 0; i--)
            {
                if (activeScenes[i].buildIndex == exception.Index)
                    continue;

                StartCoroutine(UnLoadSceneCoroutine(activeScenes[i]));
                activeScenes.RemoveAt(i);
            }
        }

        /// <inheritdoc/>
        public void UnloadAll(SceneRef[] exeptions)
        {
            for (var i = activeScenes.Count - 1; i >= 0; i--)
            {
                if (!IsSceneInRefArray(activeScenes[i].buildIndex, exeptions))
                    continue;

                StartCoroutine(UnLoadSceneCoroutine(activeScenes[i]));
                activeScenes.RemoveAt(i);
            }
        }

        private bool IsSceneInRefArray(int index, SceneRef[] sceneRefs)
        {
            return sceneRefs.Any(t => index == t.Index);
        }

        public bool IsSceneLoaded(SceneRef sceneRef)
        {
            return activeScenes.Any(t => t.buildIndex == sceneRef.Index);
        }

#if UNITY_EDITOR
        private void CheckAndAddActiveScenesInEditor()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                if (scene.buildIndex > exclude.Index)
                {
                    activeScenes.Add(scene);
                }
            }
        }
#endif
    }
}