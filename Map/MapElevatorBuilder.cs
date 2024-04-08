using System;
using _9YoS.Scripts.Gameplay.BaseObject;
using _9YoS.Scripts.Managers;
using UnityEngine;

namespace _9YoS.Scripts.UI.Map
{
    public class MapElevatorBuilder : MonoBehaviour
    {
        [SerializeField] private ConstantsManager.Map.Elevators elevator;
        private string _elevatorName;
        private Vector2 _topElevator;
        private Vector2 _bottomElevator;
        private Vector2 _topElevatorAreaOffset;
        private Vector2 _bottomElevatorAreaOffset;
        private int _isBuild;
        
        private void OnEnable()
        {
            GetElevatorInfo();
            _isBuild = GetElevatorData();
            
        }

        public void BuildElevator()
        {
            if(_isBuild > 0) return;
            var topPositionWithOffset = new Vector2(_topElevator.x + _topElevatorAreaOffset.x, _topElevator.y + _topElevatorAreaOffset.y);
            var bottomPositionWithOffset = new Vector2(_bottomElevator.x + _bottomElevatorAreaOffset.x, _bottomElevator.y + _bottomElevatorAreaOffset.y);

            var positionExtends = Vector2.Distance(topPositionWithOffset, bottomPositionWithOffset);    
            var cellNumber = Mathf.RoundToInt(positionExtends / ConstantsManager.Gameplay.StandardRoomHeightUnit);

            var cellInfo = new MapGeneratorUI.MapCellInformation[cellNumber];
            
            
            var incrementYSize =  ConstantsManager.Gameplay.StandardRoomHeightUnit / 2;

            var firstCoordinate = new Vector2(bottomPositionWithOffset.x , bottomPositionWithOffset.y + incrementYSize);

            for (int i = 0; i < cellInfo.Length - 1; i++)
            {
                cellInfo[i].position.x = Mathf.FloorToInt(firstCoordinate.x / ConstantsManager.Gameplay.StandardRoomWidthUnit);
                cellInfo[i].position.y = Mathf.FloorToInt( (firstCoordinate.y + incrementYSize)/ConstantsManager.Gameplay.StandardRoomHeightUnit);
                cellInfo[i].isElevatorConnectionCell = true;
                cellInfo[i].cellBorderSwatch = MapCellSpriteLibrary.MapCellColorSwatch.TransitionElevator;
                cellInfo[i].cellFillSwatch =  MapCellSpriteLibrary.MapCellColorSwatch.TransitionElevator;
                cellInfo[i].roomStatus = MapCellSpriteLibrary.RoomStatus.Explored;
                GameplayManager.Instance.SessionManager.CurrentSession.AttemptToAddMapCellInformation(cellInfo[i]);
                incrementYSize +=  ConstantsManager.Gameplay.StandardRoomHeightUnit;;
            }

            _isBuild++;
            SetElevatorData(_isBuild);

        }
        
        private void SetElevatorData( int status) {
            if (string.IsNullOrEmpty(_elevatorName)) return;
            var trackingInfo = new TrackedMonoBehaviour.TrackingData {id = _elevatorName, objectType = TrackedMonoBehaviour.TrackingType.Interactable};
            GameplayManager.Instance.SessionManager.CurrentSession.SetTrackedDataString(trackingInfo, status.ToString());
        }

        private int GetElevatorData() {
            if (string.IsNullOrEmpty(_elevatorName)) return ConstantsManager.Quest.ErrorNoQuestName;

            var trackingInfo = new TrackedMonoBehaviour.TrackingData {id = _elevatorName, objectType = TrackedMonoBehaviour.TrackingType.Interactable};
            var dataString = GameplayManager.Instance.SessionManager.CurrentSession.GetTrackedDataString(trackingInfo);
            
            return ReferenceEquals(dataString, null) ? 0 : int.Parse(dataString);
        }

        private void GetElevatorInfo()
        {
            switch (elevator)
            {
                case ConstantsManager.Map.Elevators.Default:
                    break;
                case ConstantsManager.Map.Elevators.ElevatorAToC:
                    SetUpInfo(
                        ConstantsManager.Map.ElevatorName.ElevatorAToC,
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorAToCTop,  
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorAToCBottom,
                        ConstantsManager.Map.AreaOffSet.COffsetX,
                        ConstantsManager.Map.AreaOffSet.COffsetY,
                        ConstantsManager.Map.AreaOffSet.AOffsetX,
                        ConstantsManager.Map.AreaOffSet.AOffsetY
                    );
                    break;
                case ConstantsManager.Map.Elevators.ElevatorAToD:
                    SetUpInfo(
                        ConstantsManager.Map.ElevatorName.ElevatorAToD,
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorAToDTop,  
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorAToDBottom,
                        ConstantsManager.Map.AreaOffSet.DOffsetX,
                        ConstantsManager.Map.AreaOffSet.DOffsetY,
                        ConstantsManager.Map.AreaOffSet.AOffsetX,
                        ConstantsManager.Map.AreaOffSet.AOffsetY
                    );
                    break;
                case ConstantsManager.Map.Elevators.ElevatorBToC:
                    SetUpInfo(
                        ConstantsManager.Map.ElevatorName.ElevatorBToC,
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorBToCTop,  
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorBToCBottom,
                        ConstantsManager.Map.AreaOffSet.COffsetX,
                        ConstantsManager.Map.AreaOffSet.COffsetY,
                        ConstantsManager.Map.AreaOffSet.BOffsetX,
                        ConstantsManager.Map.AreaOffSet.BOffsetY
                    );
                    break;
                case ConstantsManager.Map.Elevators.ElevatorCToG:
                    SetUpInfo(
                        ConstantsManager.Map.ElevatorName.ElevatorCToG,
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorCToGTop,  
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorCToGBottom,
                        ConstantsManager.Map.AreaOffSet.GaOffsetX,
                        ConstantsManager.Map.AreaOffSet.GaOffsetY,
                        ConstantsManager.Map.AreaOffSet.COffsetX,
                        ConstantsManager.Map.AreaOffSet.COffsetY
                    );
                    break;
                case ConstantsManager.Map.Elevators.ElevatorDToF:
                    SetUpInfo(
                        ConstantsManager.Map.ElevatorName.ElevatorDToF,
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorDToFTop,  
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorDToFBottom,
                        ConstantsManager.Map.AreaOffSet.DOffsetX,
                        ConstantsManager.Map.AreaOffSet.DOffsetY,
                        ConstantsManager.Map.AreaOffSet.FOffsetX,
                        ConstantsManager.Map.AreaOffSet.FOffsetY
                    );
                    break;
                case ConstantsManager.Map.Elevators.ElevatorDToG:
                    SetUpInfo(
                        ConstantsManager.Map.ElevatorName.ElevatorDToG,
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorDToGTop,  
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorDToGBottom,
                        ConstantsManager.Map.AreaOffSet.GbOffsetX,
                        ConstantsManager.Map.AreaOffSet.GbOffsetY,
                        ConstantsManager.Map.AreaOffSet.DOffsetX,
                        ConstantsManager.Map.AreaOffSet.DOffsetY
                    );
                    break;
                case ConstantsManager.Map.Elevators.ElevatorF1:
                    SetUpInfo(
                        ConstantsManager.Map.ElevatorName.ElevatorF1,
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorF1Top,  
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorF1Bottom,
                        ConstantsManager.Map.AreaOffSet.FOffsetX,
                        ConstantsManager.Map.AreaOffSet.FOffsetY,
                        ConstantsManager.Map.AreaOffSet.FOffsetX,
                        ConstantsManager.Map.AreaOffSet.FOffsetY
                    );
                    break;
                case ConstantsManager.Map.Elevators.ElevatorF2:
                    SetUpInfo(
                        ConstantsManager.Map.ElevatorName.ElevatorF2,
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorF2Top,  
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorF2Bottom,
                        ConstantsManager.Map.AreaOffSet.FOffsetX,
                        ConstantsManager.Map.AreaOffSet.FOffsetY,
                        ConstantsManager.Map.AreaOffSet.FOffsetX,
                        ConstantsManager.Map.AreaOffSet.FOffsetY
                    );
                    break;
                case ConstantsManager.Map.Elevators.ElevatorF3:
                    SetUpInfo(
                        ConstantsManager.Map.ElevatorName.ElevatorF3,
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorF3Top,  
                        ConstantsManager.Map.ElevatorSpawnPointName.ElevatorF3Bottom,
                        ConstantsManager.Map.AreaOffSet.FOffsetX,
                        ConstantsManager.Map.AreaOffSet.FOffsetY,
                        ConstantsManager.Map.AreaOffSet.FOffsetX,
                        ConstantsManager.Map.AreaOffSet.FOffsetY
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetUpInfo(
            string elevatorID, 
            string topElevator, 
            string bottomElevator,
            float topPosOffsetX, 
            float topPosOffsetY,
            float bottomPosOffsetX, 
            float bottomPosOffsetY )
        {
            _elevatorName = elevatorID;
            var topSpawn =  GameplayManager.Instance.DefaultSpawnPointList.GetSpawnLocationFromName(topElevator);
            _topElevator = topSpawn.SpawnCoordinate;
            
            var bottomSpawn =  GameplayManager.Instance.DefaultSpawnPointList.GetSpawnLocationFromName(bottomElevator);
            _bottomElevator = bottomSpawn.SpawnCoordinate;
            
            _topElevatorAreaOffset = new Vector2(topPosOffsetX + ConstantsManager.Map.AreaOffSet.GeneralOffsetX, topPosOffsetY + ConstantsManager.Map.AreaOffSet.GeneralOffsetY);
            _bottomElevatorAreaOffset = new Vector2(bottomPosOffsetX +  ConstantsManager.Map.AreaOffSet.GeneralOffsetX, bottomPosOffsetY +  ConstantsManager.Map.AreaOffSet.GeneralOffsetY);
        }
    }
}
