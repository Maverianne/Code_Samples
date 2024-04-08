using System.Collections.Generic;
using Support;
using UnityEngine;

namespace GameControl.SessionData
{
    public class ActiveObjectManager : MonoBehaviour
    {
        private Dictionary<TrackedMonoBehaviour.TrackedIdentifier, GameObject> _activeGameObjectList;

        private void Awake()
        {
            _activeGameObjectList = new Dictionary<TrackedMonoBehaviour.TrackedIdentifier, GameObject>();
        }

        public GameObject GetActiveGameObject(Constants.TrackedObjectType type, int id)
        {
            foreach (var go in _activeGameObjectList)
            {
                if(go.Key.Type != type) continue;
                if(go.Key.Id != id) continue;
                return go.Value;
            }
            return null;
        }

        public bool GameObjectIsAccessible(Constants.TrackedObjectType type, int id)
        {
            foreach (var go in _activeGameObjectList)
            {
                if(go.Key.Type != type) continue;
                if(go.Key.Id != id) continue;
                return true;
            }
            return false;
        }
        public void SubscribeToActiveList(TrackedMonoBehaviour.TrackedIdentifier identifier, GameObject go)
        {
            if(_activeGameObjectList.ContainsKey(identifier)) return;
            _activeGameObjectList.Add(identifier, go);
        }
    }
}