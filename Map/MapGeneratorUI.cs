using System;
using System.Collections;
using System.Collections.Generic;
using _9YoS.Scripts.Europa.Neutral.Central;
using _9YoS.Scripts.Gameplay.BaseObject;
using _9YoS.Scripts.Managers;
using _9YoS.Scripts.Managers.Save;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace _9YoS.Scripts.UI.Map {
    public class MapGeneratorUI : MonoBehaviour {
        [Serializable]
        public struct IntegerVector2 {
            public int x;
            public int y;

            public IntegerVector2(int newX, int newY) {
                x = newX;
                y = newY;
            }
        }
        [Serializable]
        public struct Marker
        {
            public IntegerVector2 position;
            public string trackedDataId;
        
            public string GetMarkerSaveDataString()
            {
                var dataString = "";
                dataString += ConstantsManager.SaveData.MapMarker.TrackedDataId + ConstantsManager.SaveData.MapMarker.Delimiter + trackedDataId + ConstantsManager.SaveData.SameObjectDelimiter;
               dataString += ConstantsManager.SaveData.MapMarker.PositionX + ConstantsManager.SaveData.MapMarker.Delimiter + position.x + ConstantsManager.SaveData.SameObjectDelimiter;
                dataString += ConstantsManager.SaveData.MapMarker.PositionY + ConstantsManager.SaveData.MapMarker.Delimiter + position.y + ConstantsManager.SaveData.SameObjectDelimiter;
                return dataString;
            }
        }
        [Serializable]
        public struct MapCellInformation
        {
            public MapCellSpriteLibrary.RoomStatus roomStatus;
            public bool roomIsSpecial;
            public IntegerVector2 position;

            public MapCellSpriteLibrary.MapCellBorder borderTypeTop;
            public MapCellSpriteLibrary.MapCellBorder borderTypeRight;
            public MapCellSpriteLibrary.MapCellBorder borderTypeBottom;
            public MapCellSpriteLibrary.MapCellBorder borderTypeLeft;
            
            public MapCellSpriteLibrary.MapCellColorSwatch cellFillSwatch;
            public MapCellSpriteLibrary.MapCellColorSwatch cellBorderSwatch;
            public bool isElevatorConnectionCell;
            public bool isSpecialRoom;
            public ConstantsManager.Map.Areas worldArea;
            public string roomId;
            
            public string GetCellSaveDataString() {
                //For compression purposes we are doing our own JsonUtil style of data parsing
                var dataString = "";
                dataString += ConstantsManager.SaveData.MapCell.RoomStatus + ConstantsManager.SaveData.MapCell.Delimiter + (int)roomStatus + ConstantsManager.SaveData.SameObjectDelimiter;
                
                dataString += ConstantsManager.SaveData.MapCell.RoomIsSpecial + ConstantsManager.SaveData.MapCell.Delimiter +
                              (roomIsSpecial ? SessionManager.IntegerTrue : SessionManager.IntegerFalse) + ConstantsManager.SaveData.SameObjectDelimiter;
                
                dataString += ConstantsManager.SaveData.MapCell.IsElevatorConnectionCell + ConstantsManager.SaveData.MapCell.Delimiter +
                              (isElevatorConnectionCell ? SessionManager.IntegerTrue : SessionManager.IntegerFalse) + ConstantsManager.SaveData.SameObjectDelimiter;
                
                dataString += ConstantsManager.SaveData.MapCell.PositionX + ConstantsManager.SaveData.MapCell.Delimiter + position.x + ConstantsManager.SaveData.SameObjectDelimiter;
                dataString += ConstantsManager.SaveData.MapCell.PositionY + ConstantsManager.SaveData.MapCell.Delimiter + position.y + ConstantsManager.SaveData.SameObjectDelimiter;

                dataString += ConstantsManager.SaveData.MapCell.BorderTypeTop + ConstantsManager.SaveData.MapCell.Delimiter + (int)borderTypeTop + ConstantsManager.SaveData.SameObjectDelimiter;
                dataString += ConstantsManager.SaveData.MapCell.BorderTypeRight + ConstantsManager.SaveData.MapCell.Delimiter + (int)borderTypeRight + ConstantsManager.SaveData.SameObjectDelimiter;
                dataString += ConstantsManager.SaveData.MapCell.BorderTypeBottom + ConstantsManager.SaveData.MapCell.Delimiter + (int)borderTypeBottom + ConstantsManager.SaveData.SameObjectDelimiter;
                dataString += ConstantsManager.SaveData.MapCell.BorderTypeLeft + ConstantsManager.SaveData.MapCell.Delimiter + (int)borderTypeLeft + ConstantsManager.SaveData.SameObjectDelimiter;
                
                dataString += ConstantsManager.SaveData.MapCell.CellBorderSwatch + ConstantsManager.SaveData.MapCell.Delimiter + (int)cellBorderSwatch + ConstantsManager.SaveData.SameObjectDelimiter;
                dataString += ConstantsManager.SaveData.MapCell.CellFillSwatch + ConstantsManager.SaveData.MapCell.Delimiter + (int)cellFillSwatch + ConstantsManager.SaveData.SameObjectDelimiter;

                dataString += ConstantsManager.SaveData.MapCell.CellArea + ConstantsManager.SaveData.MapCell.Delimiter + (int)worldArea + ConstantsManager.SaveData.SameObjectDelimiter;
                dataString += ConstantsManager.SaveData.MapCell.RoomId + ConstantsManager.SaveData.MapCell.Delimiter + roomId + ConstantsManager.SaveData.SameObjectDelimiter;

                return dataString;
            }
        }
        
        [SerializeField] private MapCellSpriteLibrary mapCellSpriteLibrary;
        [SerializeField] private Transform mapCellParentTransform;
        [Space]
        [SerializeField] private Vector2 gridSpacing;
        [SerializeField] private Vector2 gridMargin;
        [Space]
        [SerializeField] private GameObject mapCellPrefab;
        [SerializeField] private GameObject mapMarkerPrefab;
        [Space]
        [SerializeField] private Image overlay;
        [SerializeField] private Color overlayShowColor;
        [SerializeField] private MapData cellCorrectionData;
        [SerializeField] private CanvasGroup mapCanvasGroup;
        [SerializeField] private RectTransform mapMarkerRT;
        private bool _lockMarkerPosition;

        private int _horizontalRooms;
        private int _verticalRooms;
        private Vector2 _markerPosition;
        private Dictionary<string, PooledMapRoomCell> _pooledCellRegistry;
        private Dictionary<string, MapCellInformation> _cellInformationRegistry;
        private List<PooledMapMarker> _mapMarkerPool;
        private MapAreaName _areaName;

        private Vector2 _currentAreaOffset;
        public RectTransform MapMarkerRT => mapMarkerRT;
        public float AreaMultiplier => ConstantsManager.Map.GeneralMultiplier;
        public Vector2 GridOffset => gridSpacing + gridMargin;
        public Vector2 CurrentAreaOffset {
            get => _currentAreaOffset;
            set => _currentAreaOffset = value;
        }

        public bool LockMarkerPosition
        {
            get => _lockMarkerPosition;
            set => _lockMarkerPosition = value;
        }
        
        public Vector2 LockPosition { get; set; }

        private void Awake() 
        {
            _pooledCellRegistry = new Dictionary<string, PooledMapRoomCell>();
            _mapMarkerPool = new List<PooledMapMarker>();
            _cellInformationRegistry = new Dictionary<string, MapCellInformation>();
            _areaName = GetComponent<MapAreaName>();
        }
        

        public void ShowMap() {
            StopAllCoroutines();
            var playerPos = PlayerController.Instance.transform.position;
            playerPos.x = Mathf.FloorToInt((playerPos.x  + _currentAreaOffset.x) / ConstantsManager.Gameplay.StandardRoomWidthUnit);
            playerPos.y =  Mathf.FloorToInt((playerPos.y  + _currentAreaOffset.y) / ConstantsManager.Gameplay.StandardRoomHeightUnit);
            
            
            mapMarkerRT.anchoredPosition = new Vector2(playerPos.x * GridOffset.x, playerPos.y * GridOffset.y);
            if (_lockMarkerPosition) mapMarkerRT.anchoredPosition = LockPosition;
            overlay.color = overlayShowColor;
            GenerateMap();
            StartCoroutine(PerformShowMap());
        }

        private IEnumerator PerformShowMap(float overlayFadeDuration = 0.25f) {
            //Assumes the map is already generated
            var timer = 0f;
            var overlayStartColor = overlay.color;

            while (timer < overlayFadeDuration) {
                timer += Time.deltaTime;
                var progress = Mathf.Clamp01(timer / overlayFadeDuration);

                overlay.color = Color.Lerp(overlayStartColor, Color.clear, Easing.InOutQuad(progress));

                yield return null;
            }
        }

        private void GenerateMap() 
        {
            CorrectMapData();
            LoadNewCells(GetNewCellsLoaded());
            UpdateCells(GetCellsUpdated());
            LoadMapMarkers();
            OrganizeGameObjects();
            _areaName.RevealActiveNames();
        }

        public void PreLoadMap()
        {
            LoadNewCells(GetNewCellsLoaded());
        }

        private List<MapCellInformation> GetNewCellsLoaded()
        {
            var newCells = new List<MapCellInformation>();
            foreach (var cellInfo in GameplayManager.Instance.SessionManager.CurrentSession.MapCellInformationRecords)
            {
                if (cellInfo.roomStatus == MapCellSpriteLibrary.RoomStatus.Undiscovered) continue;
                var cellKey = cellInfo.roomId + cellInfo.worldArea + cellInfo.position.x + cellInfo.position.y;
                if (_cellInformationRegistry.ContainsKey(cellKey)) continue;
                newCells.Add(cellInfo);
                _cellInformationRegistry.Add(cellKey, cellInfo);
            } 
            return newCells;
        }

        private Dictionary<string, MapCellInformation> GetCellsUpdated()
        {
            var updatedCells = new Dictionary<string, MapCellInformation>();
            foreach (var cellInfo in GameplayManager.Instance.SessionManager.CurrentSession.MapCellInformationRecords)
            {
                if (cellInfo.roomStatus != MapCellSpriteLibrary.RoomStatus.Mapped && cellInfo.roomStatus != MapCellSpriteLibrary.RoomStatus.Explored) continue;
                var cellKey = cellInfo.roomId + cellInfo.worldArea + cellInfo.position.x + cellInfo.position.y;
                if (!_pooledCellRegistry.ContainsKey(cellKey)) continue;
                if (updatedCells.ContainsKey(cellKey)) continue;
                updatedCells.Add(cellKey, cellInfo);
            }
            return updatedCells;
        }

        private void LoadMapMarkers()
        {
            var mapMarkerCounter = 0;
            foreach (var marker in GameplayManager.Instance.SessionManager.CurrentSession.MapMarkerRecords)
            {
                var trackingData = new TrackedMonoBehaviour.TrackingData { id = marker.trackedDataId, objectType = TrackedMonoBehaviour.TrackingType.Switch };
                var dataString = GameplayManager.Instance.SessionManager.CurrentSession.GetTrackedDataString(trackingData);
                if (string.IsNullOrEmpty(dataString))
                {
                    var tmb = TrackedMonoBehaviour.GetTrackedMonoBehaviourFromTrackingData(trackingData);
                    if(ReferenceEquals(tmb, null)) continue;
                    dataString = tmb.GetSaveData();
                }
                if(dataString == SessionManager.StringTrue) continue;
                if (mapMarkerCounter >= _mapMarkerPool.Count)
                {
                    var newMarkerGo = Instantiate(mapMarkerPrefab, mapCellParentTransform);
                    var pooledMarker = newMarkerGo.GetComponent<PooledMapMarker>();
                    _mapMarkerPool.Add(pooledMarker);
                }

                var currentMarker = _mapMarkerPool[mapMarkerCounter];
                currentMarker.CellRectTransform.anchoredPosition = new Vector2(marker.position.x, marker.position.y);
                currentMarker.CellRectTransform.transform.SetAsLastSibling();
                currentMarker.gameObject.SetActive(true);
                mapMarkerCounter++;
            }
        }

        private void LoadNewCells(List<MapCellInformation> cellInformation)
        {
            foreach (var cellInfo in cellInformation)
            {
                var cellKey = cellInfo.roomId + cellInfo.worldArea + cellInfo.position.x + cellInfo.position.y;
                if (_pooledCellRegistry.ContainsKey(cellKey)) continue;
                
                var newMapCellGo = Instantiate(mapCellPrefab, mapCellParentTransform);
                var mapCell = newMapCellGo.GetComponent<PooledMapRoomCell>();
                _pooledCellRegistry.Add(cellKey, mapCell);
                
                var currentCell = _pooledCellRegistry[cellKey];
                var borderSprite = cellInfo.isElevatorConnectionCell ? mapCellSpriteLibrary.ElevatorCellSprite : mapCellSpriteLibrary.GetNormalCellBorder(cellInfo);
                var borderColor = mapCellSpriteLibrary.GetCellBorderColor(cellInfo.cellBorderSwatch);
                var fillColor = mapCellSpriteLibrary.GetCellFillColor(cellInfo.cellFillSwatch);
                currentCell.SetMapCellAppearance(borderSprite, fillColor, borderColor, cellInfo.roomStatus);
                currentCell.CellRectTransform.anchoredPosition = new Vector2(cellInfo.position.x * GridOffset.x, cellInfo.position.y * GridOffset.y);
                currentCell.CellRectTransform.sizeDelta = gridSpacing;

                _pooledCellRegistry[cellKey] = mapCell;
            }
        }
        
        private void UpdateCells(Dictionary<string, MapCellInformation> cellInformation)
        {
            foreach (var cellInfo in cellInformation)
            {
               UpdatePooledCell(cellInfo.Key, cellInfo.Value);
            }
        }

        private void UpdatePooledCell(string cellKey, MapCellInformation cellInformation)
        {
            if(!_pooledCellRegistry.ContainsKey(cellKey)) return;
            var updatedCell = _pooledCellRegistry[cellKey];
            updatedCell.UpdateAppearance(cellInformation.roomStatus);
            _pooledCellRegistry[cellKey] = updatedCell;
        }

        private void OrganizeGameObjects()
        {
            foreach (var pooledCell in _pooledCellRegistry)
            {
                switch (pooledCell.Value.RoomStatus)
                {
                    case MapCellSpriteLibrary.RoomStatus.Mapped:
                        pooledCell.Value.CellRectTransform.transform.SetAsFirstSibling();
                        break;
                    case MapCellSpriteLibrary.RoomStatus.Explored:
                        pooledCell.Value.CellRectTransform.transform.SetAsLastSibling();
                        break;
                }
            }
            mapMarkerRT.transform.SetAsLastSibling();
            mapCanvasGroup.alpha = 1;
        }

        private void CorrectMapData()
        {
            foreach (var cell in cellCorrectionData.CellData)
            {
                var xPos = cell.position.x / GridOffset.x;
                var yPos = cell.position.y / GridOffset.y;
                var newPos = new IntegerVector2((int)xPos, (int)yPos);
                
                var correctedCell = new MapCellInformation
                {
                    position = newPos,
                    cellBorderSwatch = cell.borderSwatch,
                    cellFillSwatch = cell.fillSwatch,
                    borderTypeTop = cell.borderTop,
                    borderTypeRight = cell.borderRight,
                    borderTypeBottom = cell.borderBottom,
                    borderTypeLeft = cell.borderLeft,
                };
                GameplayManager.Instance.SessionManager.CurrentSession.AttemptCorrectMapData(correctedCell, cell.overwriteRoomStatus, cell.roomStatus);
            }
        }
        public void AttemptAddMapMarkerData(MapMarkerData data)
        {
            var newMarkerData = new Marker();
            newMarkerData.position.x = data.MapCoordinateX;
            newMarkerData.position.y = data.MapCoordinateY;
            newMarkerData.trackedDataId = data.GeneratorTrackingId;
            GameplayManager.Instance.SessionManager.CurrentSession.AttemptToAddMapMarkerData(newMarkerData);
        }
        public void HideAllMapCells()
        {
            mapCanvasGroup.alpha = 0;
        }

    }
}