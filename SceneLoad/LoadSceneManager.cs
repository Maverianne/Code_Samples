using System.Collections;
using Objects.Pooled;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LoadSceneManager : MonoBehaviour
    {
        private bool _loadingLevel;
        private bool _fadeOverlayOn;

        private LoadFadeUI _loadUI;
        private float _fadeInTime;
        private const string MainSceneName = "Main";

        public string CurrentLevel { get; set; }
        private string PreviousLevel { get; set; }

        public void LoadLevel(string level, Vector2 playePos, float fadeInTime = 0.2f)
        {
            StartCoroutine(PerformLoadLevel(level, playePos, fadeInTime));
        }

        public void LoadCredits(float fadeInTime = 0.2f)
        {
            if (_loadingLevel) return;
            _loadingLevel = true;
            _fadeInTime = fadeInTime;
            StartCoroutine( PerformLoadOneScene(Constants.Scenes.SceneNames.CreditScene, Constants.Events.GameStatus.LaunchedCredits));
        }

        public void LoadTitleScreen(float fadeInTime = 0.2f) {
            if (_loadingLevel) return;
            _loadingLevel = true;
            _fadeInTime = fadeInTime;
            PreviousLevel = string.Empty;
            CurrentLevel = string.Empty;
            StartCoroutine( PerformLoadOneScene(Constants.Scenes.SceneNames.LaunchScreen, Constants.Events.GameStatus.ReturnTitleScreen));
            SessionManager.TrackedGameData.Clear();
            SessionManager.TrackedGameObjects.Clear();
        }
        
        public IEnumerator PerformLoadLevel(string level, Vector2 playerPos, float fadeInTime = 0.2f, bool usePosition = true)
        {
            if (_loadingLevel) yield break;
            _loadingLevel = true;
            _fadeInTime = fadeInTime;
            StartCoroutine(PerformFadeOverlay(fadeInTime: _fadeInTime));
            yield return PerformPauseAndLoad(level, playerPos, usePosition);
            yield return null;
        }

        private IEnumerator PerformPauseAndLoad(string levelName, Vector2 playerPosition, bool usePosition = true)
        {
            Time.timeScale = 0;
            PreviousLevel = CurrentLevel;
            CurrentLevel = levelName;
            var fadeGo = MainManager.Instance.ObjectPool.SpawnFromPool(Constants.ObjectPool.UI.FadeScreen, MainManager.Instance.transform.position);
            var fadeUI = fadeGo.GetComponent<PooledUI>();
            yield return new WaitForSecondsRealtime(_fadeInTime);
            
            MainManager.Instance.WindowManager.CloseAllWindows();
            if (usePosition) GameplayManager.Instance.PlayerCharacter.transform.position = playerPosition;
            
            yield return PerformUnloadCurrentLevel();
            
            yield return PerformLoadNextLevel();
            if(_fadeOverlayOn) _loadUI.Terminate();
            fadeUI.Terminate();
            GameplayManager.Instance.CameraManager.FollowNewTarget(GameplayManager.Instance.PlayerCharacter.transform);
            Time.timeScale = 1;
            _loadingLevel = false;
        }

        private IEnumerator PerformLoadNextLevel()
        {
            var currentNumberOfLoadedScenes = SceneManager.sceneCount;
            var loadedScenes = new Scene[currentNumberOfLoadedScenes];
            
            for (var i = 0; i < currentNumberOfLoadedScenes; i++) loadedScenes[i] = SceneManager.GetSceneAt(i);
     
            foreach (var loadedScene in loadedScenes) {
                if (loadedScene.name != MainSceneName) continue;
                break;
            }
            
            var asyncLoadScene = SceneManager.LoadSceneAsync(CurrentLevel, LoadSceneMode.Additive);
            yield return asyncLoadScene;
        }

        private IEnumerator PerformUnloadCurrentLevel()
        {
            if(string.IsNullOrEmpty(PreviousLevel)) yield break;
            var currentNumberOfLoadedScenes = SceneManager.sceneCount;
            var loadedScenes = new Scene[currentNumberOfLoadedScenes];

            for (var i = 0; i < currentNumberOfLoadedScenes; i++) loadedScenes[i] = SceneManager.GetSceneAt(i);
            var asyncUnloadScene = SceneManager.UnloadSceneAsync(PreviousLevel);
            yield return asyncUnloadScene;
            yield return null;
        }

        private IEnumerator PerformLoadOneScene(string scene, string eventTrigger)
        {
            Time.timeScale = 0;
            var fadeGo = MainManager.Instance.ObjectPool.SpawnFromPool(Constants.ObjectPool.UI.FadeScreen, MainManager.Instance.transform.position);
            var fadeUI = fadeGo.GetComponent<PooledUI>();
            yield return new WaitForSecondsRealtime(_fadeInTime);
            
            MainManager.Instance.WindowManager.CloseAllWindows();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
            while (!asyncLoad.isDone) yield return null;
            
            if(_fadeOverlayOn)  _loadUI.Terminate();
            fadeUI.Terminate();
            EventManager.TriggerEvent(eventTrigger);
            Time.timeScale = 1;
            _loadingLevel = false;
        }
        private IEnumerator PerformFadeOverlay(float defaultDelay = 0.5f, float fadeInTime = 0.2f)
        {
            yield return new WaitForSecondsRealtime(defaultDelay);
            if (!_loadingLevel) yield break;
            _fadeOverlayOn = true;
            var loadGo = MainManager.Instance.ObjectPool.SpawnFromPool(Constants.ObjectPool.UI.LoadScreen, MainManager.Instance.transform.position);
            _loadUI = loadGo.GetComponent<LoadFadeUI>();
        }
    }
}