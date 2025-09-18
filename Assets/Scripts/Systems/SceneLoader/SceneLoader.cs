using System.Collections;
using System.Threading.Tasks;
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

        private async Task LoadSceneCoroutine(SceneRef sceneRef, LoadSceneMode mode)
        {
            await SceneManager.LoadSceneAsync(sceneRef.Index, mode);

            var loadedScene = SceneManager.GetSceneByBuildIndex(sceneRef.Index);

            if (loadedScene.IsValid() && loadedScene.isLoaded)
                activeScenes.Add(loadedScene);
            else
                Debug.LogError($"The Scene with name {sceneRef.Name} didn't load correctly.");
        }

        /// <inheritdoc/>
        private static IEnumerator UnLoadSceneCoroutine(Scene activeScenes)
        {
            var asyncOp = SceneManager.UnloadSceneAsync(activeScenes);

            while (asyncOp != null && !asyncOp.isDone)
                yield return null;
        }

        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneRef sceneRef, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            if (IsSceneLoaded(sceneRef))
            {
                Debug.LogWarning($"Scene is already loaded.");
                return;
            }

            if (sceneRef.Index < 0)
            {
                Debug.LogError($"Not Valid SceneRef. Cause SceneIndex of {sceneRef.Name} is < 0");
                return;
            }

            await LoadSceneCoroutine(sceneRef, mode);
        }

        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneRef[] sceneRef)
        {
            foreach (var t in sceneRef)
                await LoadSceneAsync(t);
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