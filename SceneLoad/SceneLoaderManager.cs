using System;
using System.Collections.Generic;
using System.Collections;
using Data.Storage;
using Player;
using Support;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameControl
{
    public class SceneLoaderManager : MonoBehaviour
    {
        public List<Constants.Biome> _loadedBiomes = new List<Constants.Biome>();
        public void LoadNewBiome()
        {
            if(_loadedBiomes.Contains(ReinaDirector.Instance.Status.CurrentRoom.Biome)) return;
            _loadedBiomes.Add(ReinaDirector.Instance.Status.CurrentRoom.Biome);
            StartCoroutine(PerformLoadBiome(ReinaDirector.Instance.Status.CurrentRoom.Biome));
        }

        private Dictionary<int, string> GetBiomeScenes(Constants.Biome biome)
        {
            Dictionary<int, string> scenes = new Dictionary<int, string>();
            foreach (var registry in GameplayManager.Instance.RoomSceneRegistry.RoomList)
            {
                if(registry.RoomBiome != biome) continue;
                if (GameplayManager.Instance.ActiveObjectManager.GameObjectIsAccessible( Constants.TrackedObjectType.Room,registry.RoomId)) continue;
                if(scenes.ContainsKey(registry.RoomId)) continue;
                scenes.Add(registry.RoomId, registry.SceneNameString);
            }

            if (biome == Constants.Biome.FaustOffice)
            {
                foreach (var registry in GameplayManager.Instance.RoomSceneRegistry.RoomList)
                {
                    if(registry.RoomBiome != Constants.Biome.InBetween && registry.RoomBiome != Constants.Biome.Home) continue;
                    if (GameplayManager.Instance.ActiveObjectManager.GameObjectIsAccessible( Constants.TrackedObjectType.Room,registry.RoomId)) continue;
                    if(scenes.ContainsKey(registry.RoomId)) continue;
                    scenes.Add(registry.RoomId, registry.SceneNameString);
                }
            }
            return scenes;
        }

        private IEnumerator PerformLoadBiome(Constants.Biome biome)
        {
            var scenesToLoad = GetBiomeScenes(biome);
            var scenesAsync = new List<AsyncOperation>();

            var startTime = Time.time;
            if (scenesToLoad.Count == 0) yield break;

            EventManager.TriggerEvent(Constants.Events.Data.InitLoadGame);
            EventManager.TriggerEvent(Constants.Events.Data.InitBiomeLoad);
            GameplayManager.Instance.AdjustTimeFlowSources(Constants.TimeStopSources.PauseMenuOpen, true);
            foreach (var scene in scenesToLoad)
            {
                var async = SceneManager.LoadSceneAsync(scene.Value, LoadSceneMode.Additive);
                if (GameplayManager.Instance.ActiveObjectManager.GameObjectIsAccessible( Constants.TrackedObjectType.Room,scene.Key)) continue;
                if(scenesAsync.Contains(async)) continue;
                scenesAsync.Add(async);
            }

            foreach (var scene in scenesAsync)
            {
                while (!scene.isDone) yield return null;
            }
            var loadTime = Time.time - startTime;
            yield return new WaitForSecondsRealtime(Mathf.Min(0.25f, loadTime * 0.1f));
            EventManager.TriggerEvent(Constants.Events.Data.BiomeLoaded);
            GameplayManager.Instance.AdjustTimeFlowSources(Constants.TimeStopSources.PauseMenuOpen, false);
        }
        private Constants.Biome GetBiome()
        {
            var saveLog = SaveManager.GetInt(0, Constants.PlayerPrefs.Keys.CurrentSaveLog, Constants.PlayerPrefs.DefaultValues.CurrentSaveLog);
            var roomId = SaveManager.GetInt(saveLog, Constants.Data.Keys.Reina.CurrentRoomId);
            foreach (var registry in GameplayManager.Instance.RoomSceneRegistry.RoomList)
            {
                if(registry.RoomId != roomId) continue;
    
                return registry.RoomBiome == Constants.Biome.Other ? Constants.Biome.Entrance : registry.RoomBiome;
            }

            return Constants.Biome.Entrance;
        }

        private Constants.Biome GetTrackedObjectBiome(int trackedId)
        {
            foreach (var registry in GameplayManager.Instance.RoomSceneRegistry.RoomList)
            {
                if(registry.RoomId != trackedId) continue;
    
                return registry.RoomBiome == Constants.Biome.Other ? Constants.Biome.Entrance : registry.RoomBiome;
            }

            return Constants.Biome.Entrance;
        }
        
        public IEnumerator PerformLoadTrackedObjectBiome( Constants.TrackedObjectType type, int id, int roomId, Action roomEvent)
        {
            if (GameplayManager.Instance.ActiveObjectManager.GameObjectIsAccessible(type, id))
            {
                roomEvent?.Invoke();
                yield break;
            }
            yield return PerformLoadBiome(GetTrackedObjectBiome(roomId));
            roomEvent?.Invoke();
        }
        
        
        private Dictionary<int, string> GetBiomeScenes()
        {
            Dictionary<int, string> scenes = new Dictionary<int, string>();
            foreach (var registry in GameplayManager.Instance.RoomSceneRegistry.RoomList)
            {
				
                if(registry.RoomBiome != GetBiome()) continue;
                if (GameplayManager.Instance.ActiveObjectManager.GameObjectIsAccessible( Constants.TrackedObjectType.Room,registry.RoomId)) continue;
                if(scenes.ContainsKey(registry.RoomId)) continue;
                scenes.Add(registry.RoomId, registry.SceneNameString);
            }

            return scenes;
        }

        public IEnumerator PerformLoadBiomeStartUp()
        {
            EventManager.TriggerEvent(Constants.Events.Data.InitLoadGame);
            if(GameplayManager.Instance.ForcedGameMode == Constants.GameMode.Development) yield break;
            var scenesToLoad = GetBiomeScenes();
            var extraScenes = GetEssentialScenes();
            var scenesAsync = new List<AsyncOperation>();
            
            if (extraScenes.Count != 0)
            {
                foreach (var scene in extraScenes)
                {
                    if(scenesToLoad.ContainsKey(scene.Key)) continue;
                    scenesToLoad.Add(scene.Key, scene.Value);
                }
            }
            
            var startTime = Time.time;
            foreach (var scene in scenesToLoad)
            {
                var async = SceneManager.LoadSceneAsync(scene.Value, LoadSceneMode.Additive);
                if (GameplayManager.Instance.ActiveObjectManager.GameObjectIsAccessible( Constants.TrackedObjectType.Room,scene.Key)) continue;
                if(scenesAsync.Contains(async)) continue;
                scenesAsync.Add(async);
            }

            foreach (var scene in scenesAsync)
            {
                while (!scene.isDone) yield return null;
            }
            var loadTime = Time.time - startTime;
            yield return new WaitForSecondsRealtime(Mathf.Min(0.25f, loadTime * 0.1f));
        }
        
        private  Dictionary<int, string> GetEssentialScenes()
        {
            Dictionary<int, string> scenes = new Dictionary<int, string>();
            foreach (var roomReference in GameplayManager.Instance.RoomSceneRegistry.RoomList) {
                var essential = false;

                switch (GameplayManager.Instance.CurrentGameSession.GameMode) {
                    case Constants.GameMode.ReinaAndJericho:
                        essential = roomReference.EssentialInStoryMode;
                        break;
                    case Constants.GameMode.SpeedMode:
                        essential = roomReference.EssentialInSpeedMode;
                        break;
                    case Constants.GameMode.Randomizer:
                        essential = roomReference.EssentialInRandomizerMode;
                        break;
                    case Constants.GameMode.None:
                    case Constants.GameMode.Development:
                    default:
                        break;
                }
                
                if (!essential) continue;
                if (GameplayManager.Instance.ActiveObjectManager.GameObjectIsAccessible( Constants.TrackedObjectType.Room, roomReference.RoomId)) continue;
                scenes.Add(roomReference.RoomId, roomReference.SceneNameString);
            }
            return scenes;
        }

    }
}