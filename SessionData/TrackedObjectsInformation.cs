using System;
using System.Collections.Generic;
using System.Globalization;
using Support;
using UnityEngine;

namespace GameControl.SessionData
{
    public class TrackedObjectsInformation : MonoBehaviour
    {
        [Serializable]
        public struct VisitedRoomData
        {
            public int id;
            public string roomName;
            public bool discovered;
            public bool hideOnMap;
           
            
            public string GetVisitedRoomSaveString()
            {
                if (hideOnMap) return string.Empty;
                if (!discovered) return string.Empty;
                var dataString = "";
                dataString += Constants.Data.GeneralObjectData.Id + Constants.Data.Delimiter + id + Constants.Data.SameObjectDelimiter;
                dataString += Constants.Data.RoomData.RoomName + Constants.Data.Delimiter + roomName + Constants.Data.SameObjectDelimiter;
                dataString += Constants.Data.RoomData.Discovered + Constants.Data.Delimiter + (discovered ? "1" : "0") + Constants.Data.SameObjectDelimiter;
                
                return dataString;
            }
        }
        
        [Serializable]
        public struct FountainData
        {
            public int fountainId;
            public int roomId;
            public string roomName;
            public string epigraphText;
            public Vector3 position;

            public string GetFountainSaveString()
            {
                var dataString = "";
                dataString += Constants.Data.GeneralObjectData.Id + Constants.Data.Delimiter + fountainId + Constants.Data.SameObjectDelimiter;
                dataString += Constants.Data.FountainData.RoomId + Constants.Data.Delimiter + roomId + Constants.Data.SameObjectDelimiter;
                dataString += Constants.Data.FountainData.RoomName + Constants.Data.Delimiter + roomName + Constants.Data.SameObjectDelimiter;
                dataString += Constants.Data.FountainData.Epigraph + Constants.Data.Delimiter + epigraphText + Constants.Data.SameObjectDelimiter;
                dataString += ParseVectorValueString(Constants.Data.GeneralObjectData.Position, position); 
                return dataString;
            }
        }
        
        [Serializable]
        public struct DiscoveredGatesData
        {
            public int gateId;
            public Constants.SecurityLevel securityLevel;
            public Vector3 position;
            public Vector3 boundarySize;
            public Vector3 centerPosition;
            
            public bool hideOnMap;
            public string GetGateSaveString()
            {
                if (hideOnMap) return string.Empty;
                var dataString = "";
                dataString += Constants.Data.GeneralObjectData.Id + Constants.Data.Delimiter + gateId + Constants.Data.SameObjectDelimiter;
                dataString += Constants.Data.GateData.Security + Constants.Data.Delimiter + (int)securityLevel + Constants.Data.SameObjectDelimiter;
                dataString += ParseVectorValueString(Constants.Data.GeneralObjectData.Position, position);
                dataString += ParseVectorValueString(Constants.Data.GateData.BoundarySize, boundarySize);
                dataString += ParseVectorValueString(Constants.Data.GateData.CenterPosition, centerPosition);
                return dataString;
            }
        }
        [Serializable]
        public struct NpcData
        {
            public string character;
            public Vector3 position;
            public bool isEnemy;
            public int health;
            public string GetNpcSaveString()
            {
                var dataString = "";
                dataString += Constants.Data.GeneralObjectData.Id + Constants.Data.Delimiter + character + Constants.Data.SameObjectDelimiter;
                dataString += ParseVectorValueString(Constants.Data.GeneralObjectData.Position, position);
                dataString += Constants.Data.NpcData.IsEnemy + Constants.Data.Delimiter + (isEnemy ? "1" : "0") + Constants.Data.SameObjectDelimiter;
                dataString += Constants.Data.NpcData.Health + Constants.Data.Delimiter + health + Constants.Data.SameObjectDelimiter;
                return dataString;
            }
        }
        
        public List<NpcData> TalkedNpcs { get; private set; }
        public List<DiscoveredGatesData> DiscoveredGates { get; private set; }
        public List<FountainData> SavedFountains { get; private set; }
        public List<VisitedRoomData> VisitedRooms { get; private set; }

        private void Awake()
        {
            VisitedRooms = new List<VisitedRoomData>();
            SavedFountains = new List<FountainData>();
            DiscoveredGates = new List<DiscoveredGatesData>();
            TalkedNpcs = new List<NpcData>();
        }

        private static string ParseValue(float value)
        {
            var single = value.ToString().Replace(',', '.');
            return single;
        }

        private static string ParseVectorValueString(string saveKey,Vector3 vectorValues)
        {
            var dataString = saveKey + Constants.Data.Delimiter + ParseValue(vectorValues.x) + 
                             Constants.Data.VectorDelimiter + ParseValue(vectorValues.y) + 
                             Constants.Data.VectorDelimiter + ParseValue(vectorValues.z) + Constants.Data.SameObjectDelimiter;
            return dataString;
        }
        
        #region VisitedRoom
        public void AddVisitedRoomData(VisitedRoomData visitedRoom)
        {
            for (int i = 0; i < VisitedRooms.Count; i++)
            {
                if(VisitedRooms[i].id != visitedRoom.id) continue;
                VisitedRooms[i] = visitedRoom;
                return;
            }
            VisitedRooms.Add(visitedRoom);
        }

        public VisitedRoomData GetVisitedRoomData(int id, VisitedRoomData roomData)
        {
            foreach (var room in VisitedRooms)
            {
                if (room.id != id) continue;
                roomData = room;
                return roomData;
            }
            return roomData;
        }

        public bool VisitedRoomExist(int id)
        {
            foreach (var room in VisitedRooms)
            {
                if (room.id != id) continue;
                return true;
            }
            return false;
        }
        
        #endregion
        
        #region Fountain
        public void AddFountainData(FountainData fountain)
        {
            for (int i = 0; i < SavedFountains.Count; i++)
            {
                if(SavedFountains[i].roomName != fountain.roomName) continue;
                SavedFountains[i] = fountain;
                return;
            }
            SavedFountains.Add(fountain);
        }

        public FountainData GetFountainData(string roomName, FountainData fountain)
        {
            foreach (var data in SavedFountains)
            {
                if (data.roomName != roomName) continue;
                fountain = data;
                return fountain;
            }
            return fountain;
        }
        
        public FountainData GetFountainData(int fountainId, FountainData fountain)
        {
            foreach (var data in SavedFountains)
            {
                if (data.fountainId != fountainId) continue;
                fountain = data;
                return fountain;
            }
            return fountain;
        }
        public bool FountainExist(string roomName)
        {
            foreach (var fountainData in SavedFountains)
            {
                if (fountainData.roomName != roomName) continue;
                return true;
            }
            return false;
        }
        
        #endregion
        
        #region Gates
        public void AddGateData(DiscoveredGatesData gate)
        {
            for (int i = 0; i < DiscoveredGates.Count; i++)
            {
                if(DiscoveredGates[i].gateId != gate.gateId) continue;
                DiscoveredGates[i] = gate;
                return;
            }
            DiscoveredGates.Add(gate);
        }

        public DiscoveredGatesData GetGateData(int id, DiscoveredGatesData gateData)
        {
            foreach (var gate in DiscoveredGates)
            {
                if (gate.gateId != id) continue;
                gateData = gate;
                return gateData;
            }
            return gateData;
        }

        public bool GateExist(int id)
        {
            foreach (var gate in DiscoveredGates)
            {
                if (gate.gateId != id) continue;
                return true;
            }
            return false;
        }
        
        #endregion
        
        #region Npc
        public void AddNpcData(NpcData npcData)
        {
            for (int i = 0; i < TalkedNpcs.Count; i++)
            {
                if(TalkedNpcs[i].character != npcData.character) continue;
                TalkedNpcs[i] = npcData;
                return;
            }
            TalkedNpcs.Add(npcData);
        }

        public NpcData GetNpcData(string character, NpcData npcData)
        {
            foreach (var npc in TalkedNpcs)
            {
                if (npc.character != character) continue;
                npcData = npc;
                return npcData;
            }
            return npcData;
        }

        public bool NpcExist(string character)
        {
            foreach (var npcData in TalkedNpcs)
            {
                if (npcData.character != character) continue;
                return true;
            }
            return false;
        }
        
        #endregion
    }
}