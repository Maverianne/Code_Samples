using System;
using System.Collections.Generic;
using Support;

namespace GameControl.SessionData
{
    public class SessionObject
    {
        [Serializable]
        public struct StateObject
        {
            public int id;
            public int state;

            public string GetString()
            {
                var dataString = id + Constants.Data.DifferentStringDelimiter.ToString() + state;
                return dataString;
            }
        }
        
        [Serializable]
        public struct NpcObject
        {
            public int id;
            public string bioString;
            public string stateString;
            public string positionString;
            public string velocityString;
            public string rotationString;
            public string animString;
            public string animParamValueString;
            public string hierarchyString;
            
            public string GetString()
            {
                var dataString = id + Constants.Data.VectorDelimiter  + bioString + Constants.Data.SameObjectDelimiter + stateString + Constants.Data.SameObjectDelimiter  + positionString + Constants.Data.SameObjectDelimiter  + velocityString + Constants.Data.SameObjectDelimiter  + rotationString + Constants.Data.SameObjectDelimiter  + animString + Constants.Data.SameObjectDelimiter  +
                                animParamValueString + Constants.Data.SameObjectDelimiter  + hierarchyString;
                return dataString;
            }
        }

        [Serializable]
        public struct SwitchObject
        {
            public int id;
            public int position;
            public int locked;
            
            public string GetString()
            {
                var dataString = id + Constants.Data.DifferentStringDelimiter.ToString()  + position + Constants.Data.DifferentStringDelimiter.ToString() + locked;
                return dataString;
            }
        }
        
        [Serializable]
        public struct GateObject
        {
            public int id;
            public int state;
            public int isLockedInPos;
            public int gateClosedAtLeastOnce;
            public int hasBeenControlled;
            public int openFromLeft;
            
            public string GetString()
            {
                var dataString = id + Constants.Data.VectorDelimiter + state + Constants.Data.VectorDelimiter + isLockedInPos
                                 + Constants.Data.VectorDelimiter + gateClosedAtLeastOnce  + Constants.Data.VectorDelimiter + hasBeenControlled
                                 + Constants.Data.VectorDelimiter + openFromLeft;
                return dataString;
            }
        }

        [Serializable]
        public struct OrbObject
        {
            public int id;
            public int hostType;
            public int hostId;
            public int orbHasKaboomed;
            
            public string GetString()
            {
                var dataString = id +Constants.Data.DifferentStringDelimiter.ToString() + hostType + Constants.Data.DifferentStringDelimiter.ToString() + hostId + Constants.Data.DifferentStringDelimiter.ToString() + orbHasKaboomed;
                return dataString;
            }
        }


        public List<StateObject> PowerUps { get; private set; }
        public List<StateObject> Elevators { get; private set; }
        public List<StateObject> KnockOverObjects { get; private set; }
        public List<StateObject> DestructibleObjects { get; private set; }
        public List<StateObject> SpeedBalls { get; private set; }
        public List<NpcObject> EnemyObjects { get; private set; }
        public List<NpcObject> NpcObjects { get; private set; }
        public List<SwitchObject> SwitchObjects { get; private set; }
        public List<GateObject> Gates { get; private set; }
        public List<OrbObject> Orbs { get; private set; }

        public void Initialize()
        {
            PowerUps = new List<StateObject>();
            Elevators = new List<StateObject>();
            KnockOverObjects = new List<StateObject>();
            DestructibleObjects = new List<StateObject>();
            SpeedBalls = new List<StateObject>();
            EnemyObjects = new List<NpcObject>();
            NpcObjects = new List<NpcObject>();
            SwitchObjects = new List<SwitchObject>();
            Gates = new List<GateObject>();
            Orbs = new List<OrbObject>();
            
        }

        public void ClearLists()
        {
            PowerUps.Clear();
            Elevators.Clear();
            KnockOverObjects.Clear();
            DestructibleObjects.Clear();
            SpeedBalls.Clear();
            EnemyObjects.Clear();
            NpcObjects.Clear();
            SwitchObjects.Clear();
            Gates.Clear();
            Orbs.Clear();
        }

        #region StateObject

        public void AddStateObject(Constants.TrackedObjectType type, StateObject stateObject)
        {
            switch (type)
            {
                case Constants.TrackedObjectType.PowerUp:
                    AddToList(PowerUps, stateObject);
                    break;
                case Constants.TrackedObjectType.Elevator:
                    AddToList(Elevators, stateObject);
                    break;
                case Constants.TrackedObjectType.KnockOverObject:
                    AddToList(KnockOverObjects, stateObject);
                    break;
                case Constants.TrackedObjectType.DestructibleObject:
                    AddToList(DestructibleObjects, stateObject);
                    break;
                case Constants.TrackedObjectType.SpeedBall:
                    AddToList(SpeedBalls, stateObject);
                    break;
            }
        }
        
        public bool StateObjectInformationExist(Constants.TrackedObjectType type, int objId)
        {
            switch (type)
            {
                case Constants.TrackedObjectType.PowerUp:
                    return CheckStateObject(PowerUps, objId);
                case Constants.TrackedObjectType.Elevator:
                    return CheckStateObject(Elevators, objId);
                case Constants.TrackedObjectType.KnockOverObject:
                    return CheckStateObject(KnockOverObjects, objId);
                case Constants.TrackedObjectType.DestructibleObject:
                    return CheckStateObject(DestructibleObjects, objId);
                case Constants.TrackedObjectType.SpeedBall:
                    return CheckStateObject(SpeedBalls, objId);
            }
            return false;
        }

        public StateObject GetStateObjectInfo(Constants.TrackedObjectType type, StateObject stateObject)
        {
            switch (type)
            {
                case Constants.TrackedObjectType.PowerUp:
                   return PassStateObject(PowerUps, stateObject);
                case Constants.TrackedObjectType.Elevator:
                    return PassStateObject(Elevators, stateObject);
                case Constants.TrackedObjectType.KnockOverObject:
                    return PassStateObject(KnockOverObjects, stateObject);
                case Constants.TrackedObjectType.DestructibleObject:
                    return PassStateObject(DestructibleObjects, stateObject);
                case Constants.TrackedObjectType.SpeedBall:
                    return PassStateObject(SpeedBalls, stateObject);
            }
            return stateObject;
        }
        
        private bool CheckStateObject(List<StateObject> stateList, int objId)
        {
            for (var i = 0; i < stateList.Count; i++)
            {
                if(stateList[i].id != objId) continue;
                return true;
            }
            return false;
        }
        
        private StateObject PassStateObject(List<StateObject> stateList, StateObject stateObject)
        {
            for (var i = 0; i < stateList.Count; i++)
            {
                if(stateList[i].id != stateObject.id) continue;
                return stateList[i];
            }
            return stateObject;
        }

        private void AddToList(List<StateObject> stateList, StateObject info)
        {
            for (var i = 0; i < stateList.Count; i++)
            {
                if(stateList[i].id != info.id) continue;
                stateList[i] = info;
                return;
            }
            stateList.Add(info);
            SortList(stateList);
        }

        #endregion
        
        public void AddEnemyObject(NpcObject enemyObject)
        {
            for (var i = 0; i < EnemyObjects.Count; i++)
            {
                if(EnemyObjects[i].id != enemyObject.id) continue;
                EnemyObjects[i] = enemyObject;
                return;
            }
            EnemyObjects.Add(enemyObject);
            EnemyObjects.Sort((v1, v2) =>
            {
                return v1.id - v2.id;
            });
            
        }
        
        public bool EnemyObjectExist(int objId)
        {
            for (var i = 0; i < EnemyObjects.Count; i++)
            {
                if(EnemyObjects[i].id != objId) continue;
                return true;
            }
            return false;
        }
        
        public NpcObject GetEnemyObjectInfo(NpcObject npcObject)
        {
            for (var i = 0; i < EnemyObjects.Count; i++)
            {
                if(EnemyObjects[i].id != npcObject.id) continue;
                return EnemyObjects[i];
            }
            return npcObject;
        }

        
        public void AddNpcObject(NpcObject enemyObject)
        {
            for (var i = 0; i < NpcObjects.Count; i++)
            {
                if(NpcObjects[i].id != enemyObject.id) continue;
                NpcObjects[i] = enemyObject;
                return;
            }
            NpcObjects.Add(enemyObject);
            NpcObjects.Sort((v1, v2) =>
            {
                return v1.id - v2.id;
            });
        }
        
        public bool NpcObjectExist(int objId)
        {
            for (var i = 0; i < NpcObjects.Count; i++)
            {
                if(NpcObjects[i].id != objId) continue;
                return true;
            }
            return false;
        }
        
        public NpcObject GetNpcObjectInfo(NpcObject npcObject)
        {
            for (var i = 0; i < NpcObjects.Count; i++)
            {
                if(NpcObjects[i].id != npcObject.id) continue;
                return NpcObjects[i];
            }
            return npcObject;
        }


        public void AddSwitchObject(SwitchObject switchObject)
        {
            for (var i = 0; i < SwitchObjects.Count; i++)
            {
                if(SwitchObjects[i].id != switchObject.id) continue;
                SwitchObjects[i] = switchObject;
                return;
            }
            SwitchObjects.Add(switchObject);
            SwitchObjects.Sort((v1, v2) =>
            {
                return v1.id - v2.id;
            });
        }
        
        public bool SwitchObjectExist(int objId)
        {
            for (var i = 0; i < SwitchObjects.Count; i++)
            {
                if(SwitchObjects[i].id != objId) continue;
                return true;
            }
            return false;
        }
        public SwitchObject GetSwitchObjectInfo(SwitchObject switchObject)
        {
            for (var i = 0; i < SwitchObjects.Count; i++)
            {
                if(SwitchObjects[i].id != switchObject.id) continue;
                return SwitchObjects[i];
            }
            return switchObject;
        }
        
        public void AddGate(GateObject gateObject)
        {
            for (var i = 0; i < Gates.Count; i++)
            {
                if(Gates[i].id != gateObject.id) continue;
                Gates[i] = gateObject;
                return;
            }
            Gates.Add(gateObject);
            Gates.Sort((v1, v2) =>
            {
                return v1.id - v2.id;
            });
        }
        public bool GateObjectExist(int objId)
        {
            for (var i = 0; i < Gates.Count; i++)
            {
                if(Gates[i].id != objId) continue;
                return true;
            }
            return false;
        }
        public GateObject GetGateObjectInfo(GateObject gateObject)
        {
            for (var i = 0; i < Gates.Count; i++)
            {
                if(Gates[i].id != gateObject.id) continue;
                return Gates[i];
            }
            return gateObject;
        }
        
        public void AddOrb(OrbObject orbObject)
        {
            for (var i = 0; i < Orbs.Count; i++)
            {
                if(Orbs[i].id != orbObject.id) continue;
                Orbs[i] = orbObject;
                return ;
            }
            Orbs.Add(orbObject);
            Orbs.Sort((v1, v2) =>
            {
                return v1.id - v2.id;
            });
        }
        public bool OrbExist(int objId)
        {
            for (var i = 0; i < Orbs.Count; i++)
            {
                if(Orbs[i].id != objId) continue;
                return true;
            }
            return false;
        }
        public OrbObject GetOrbInfo(OrbObject orbObject)
        {
            for (var i = 0; i < Orbs.Count; i++)
            {
                if(Orbs[i].id != orbObject.id) continue;
                return Orbs[i];
            }
            return orbObject;
        }

        private void SortList(List<StateObject> listToSort)
        {
            listToSort.Sort((v1, v2) =>
            {
                return v1.id - v2.id;
            });
        }
        
    }
}