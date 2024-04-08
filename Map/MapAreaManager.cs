using System;
using _9YoS.Scripts.Europa.Neutral.Central;
using _9YoS.Scripts.Managers;
using UnityEngine;

namespace _9YoS.Scripts.UI.Map { 
    [Serializable]
    public struct RoomInfo {
        public MapRoomInfo mapRoomInfo;
        public string roomName;
    } 
    
    public class MapAreaManager : MonoBehaviour {
        private MapCellSpriteLibrary.MapCellColorSwatch _colorSwatch;
        public ConstantsManager.Map.Areas area;
        private Vector2 _offset;
        private MapRoomInfo[] _mapRoomInfos;
        private RoomInfo[] _roomInfo;
        private bool _isMapped;
        
        protected virtual void Awake() {
          
            _mapRoomInfos = FindObjectsOfType<MapRoomInfo>();
            _roomInfo = new RoomInfo[_mapRoomInfos.Length];
        }
        
        public void AreaIsMapped() {
            if(_isMapped) return;
            _isMapped = true;
            foreach (var mapRoom in _mapRoomInfos)
            {
                mapRoom.IsMapped();
            }
            
            var mapPiece = PlayerController.Instance.Europa.ProgressionData.NumberOfMapPiecesFound;
            mapPiece += 1;
            PlayerController.Instance.Europa.ProgressionData.NumberOfMapPiecesFound = mapPiece;
            CheckGrantAchievement();
        }
        
        public void RoomIsExplored(string nameRoom) {
            for (var i = 0; i < _roomInfo.Length; i++) {
                if (_roomInfo[i].roomName != nameRoom) continue;
                _roomInfo[i].mapRoomInfo.IsExplored();
            }  
        }

        public virtual void GetAreaInfo() {
            switch (area) {
                case ConstantsManager.Map.Areas.Default:
                    break;
                case ConstantsManager.Map.Areas.AreaA:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaA;
                    _offset.x = ConstantsManager.Map.AreaOffSet.AOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.AOffsetY;
                    break;
                case ConstantsManager.Map.Areas.AreaB:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaB;
                    _offset.x = ConstantsManager.Map.AreaOffSet.BOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.BOffsetY;
                    break;
                case ConstantsManager.Map.Areas.AreaC:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaC;
                    _offset.x = ConstantsManager.Map.AreaOffSet.COffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.COffsetY;
                    break;
                case ConstantsManager.Map.Areas.AreaD:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaD;
                    _offset.x = ConstantsManager.Map.AreaOffSet.DOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.DOffsetY;
                    break;
                case ConstantsManager.Map.Areas.AreaE:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaE;
                    _offset.x = ConstantsManager.Map.AreaOffSet.EOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.EOffsetY;
                    break;
                case ConstantsManager.Map.Areas.AreaF:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaF;
                    _offset.x = ConstantsManager.Map.AreaOffSet.FOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.FOffsetY;
                    break;
                case ConstantsManager.Map.Areas.AreaGa:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaG;
                    _offset.x = ConstantsManager.Map.AreaOffSet.GaOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.GaOffsetY;
                    break;
                case ConstantsManager.Map.Areas.AreaGb:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaG;
                    _offset.x = ConstantsManager.Map.AreaOffSet.GbOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.GbOffsetY;
                    break;
                case ConstantsManager.Map.Areas.AreaGc:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaG;
                    _offset.x = ConstantsManager.Map.AreaOffSet.GcOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.GcOffsetY;
                    break;
                case ConstantsManager.Map.Areas.PaintingB:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaB;
                    _offset.x = ConstantsManager.Map.AreaOffSet.PaintingBOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.PaintingBOffsetY;
                    break;
                case ConstantsManager.Map.Areas.PaintingC:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaC;
                    _offset.x = ConstantsManager.Map.AreaOffSet.PaintingCOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.PaintingCOffsetY;
                    break;
                case ConstantsManager.Map.Areas.PaintingD:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaD;
                    _offset.x = ConstantsManager.Map.AreaOffSet.PaintingDOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.PaintingDOffsetY;
                    break; 
                case ConstantsManager.Map.Areas.AreaH:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaH;
                    break;
                case ConstantsManager.Map.Areas.AreaHTop:
                    break;
                case ConstantsManager.Map.Areas.DarkApino:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaA;
                    _offset.x = ConstantsManager.Map.AreaOffSet.DarkApinoOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.DarkApinoOffsetY;
                    break;
                case ConstantsManager.Map.Areas.SecondChase:
                    _colorSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaE;
                    _offset.x = ConstantsManager.Map.AreaOffSet.SecondChaseOffsetX;
                    _offset.y = ConstantsManager.Map.AreaOffSet.SecondChaseOffsetY;
                    break;
            }

            _offset = new Vector2(_offset.x + ConstantsManager.Map.AreaOffSet.GeneralOffsetX, _offset.y + ConstantsManager.Map.AreaOffSet.GeneralOffsetY);
            if(area == ConstantsManager.Map.Areas.AreaH) return;
            GameplayManager.Instance.UI.Pause.mapUI.CurrentAreaOffset = _offset;
        }
        
        public void GetRoomsIndividualInfo() {
            for (var i = 0; i < _roomInfo.Length; i++) {
                _roomInfo[i].roomName = _mapRoomInfos[i].ComponentLevel.Identifier;
                _roomInfo[i].mapRoomInfo = _mapRoomInfos[i];
                _roomInfo[i].mapRoomInfo.AreaManagerGiveColor(_colorSwatch);
                _roomInfo[i].mapRoomInfo.AreaManagerOffSet(_offset.x, _offset.y);
                _roomInfo[i].mapRoomInfo.AreaManagerGiveIdentifierInfo(_mapRoomInfos[i].ComponentLevel.Identifier, area);
                _roomInfo[i].mapRoomInfo.GetCellsAndPositions();
            }
        }
        private void CheckGrantAchievement() {
            var mapAreas = PlayerController.Instance.Europa.ProgressionData.NumberOfMapPiecesFound;
            if(mapAreas < ConstantsManager.Achievement.NumberOfMapPieces) return;
            AchievementManager.AttemptToGrantAchievement(ConstantsManager.Achievement.CompletedMapBasic);
        }
    }
}
