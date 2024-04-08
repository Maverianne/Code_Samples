using _9YoS.Scripts.Gameplay.BaseObject;
using _9YoS.Scripts.Managers;
using UnityEngine;

namespace _9YoS.Scripts.UI.Map
{
    public class HallwayH : MonoBehaviour
    {
        [SerializeField] private string hallwayID;
        [SerializeField] private Vector2 hallwayTop;
        [SerializeField] private Vector2 hallwayBottom;
        private int _hallwayVisited;
        
        private void BuildHallway()
        {
            var offset = new Vector2(ConstantsManager.Map.AreaOffSet.HOffsetX, ConstantsManager.Map.AreaOffSet.HOffsetY);
            var topPositionWithOffset = new Vector2(hallwayTop.x + offset.x, hallwayTop.y + offset.y);
            var bottomPositionWithOffset = new Vector2(hallwayBottom.x + offset.x, hallwayBottom.y + offset.y);

            var verticalRooms = Vector2.Distance(topPositionWithOffset, bottomPositionWithOffset);    
            const int horizontalRooms = 2;
            verticalRooms = Mathf.RoundToInt(verticalRooms / ConstantsManager.Gameplay.StandardRoomHeightUnit);
            var cellNumber = Mathf.RoundToInt(verticalRooms * horizontalRooms);
            
            var cellInfo = new MapGeneratorUI.MapCellInformation[cellNumber];

            var incrementXSize = ConstantsManager.Gameplay.StandardRoomWidthUnit  / 2;
            var incrementYSize =  ConstantsManager.Gameplay.StandardRoomHeightUnit / 2;

            var firstCoordinate = new Vector2(bottomPositionWithOffset.x - incrementXSize , bottomPositionWithOffset.y + incrementYSize);
       
            
             var cell = 0;
             for (var h = 0; h < horizontalRooms; h++) 
             {
                for (var v = 0; v < verticalRooms; v++) {
                   var integerCoordX = Mathf.FloorToInt((firstCoordinate.x + incrementXSize ) / ConstantsManager.Gameplay.StandardRoomWidthUnit);
                   var integerCoordY = Mathf.FloorToInt((firstCoordinate.y + incrementYSize ) / ConstantsManager.Gameplay.StandardRoomHeightUnit);
                         
                   cellInfo[cell].position.x = integerCoordX;
                   cellInfo[cell].position.y = integerCoordY;
                   cellInfo[cell].cellBorderSwatch = MapCellSpriteLibrary.MapCellColorSwatch.AreaH;
                   cellInfo[cell].cellFillSwatch =  MapCellSpriteLibrary.MapCellColorSwatch.TransitionElevator;
                   switch (h)
                   {
                       case 0:
                           cellInfo[cell].borderTypeLeft = MapCellSpriteLibrary.MapCellBorder.Partial;
                           cellInfo[cell].borderTypeBottom = MapCellSpriteLibrary.MapCellBorder.None;
                           cellInfo[cell].borderTypeTop = MapCellSpriteLibrary.MapCellBorder.None;
                           cellInfo[cell].borderTypeRight = MapCellSpriteLibrary.MapCellBorder.None;
                           break;
                       case 1:
                           cellInfo[cell].borderTypeLeft = MapCellSpriteLibrary.MapCellBorder.None;
                           cellInfo[cell].borderTypeBottom = MapCellSpriteLibrary.MapCellBorder.None;
                           cellInfo[cell].borderTypeTop = MapCellSpriteLibrary.MapCellBorder.None;
                           cellInfo[cell].borderTypeRight = MapCellSpriteLibrary.MapCellBorder.Partial;
                           break;
                   }
                   
                   cellInfo[cell].roomStatus = MapCellSpriteLibrary.RoomStatus.Mapped;
                   // _realWorldPositions[cell] = new Vector2(Mathf.RoundToInt(_firstCoordinate.x + _incrementXSize), Mathf.RoundToInt(_firstCoordinate.y + _incrementYSize));
                   GameplayManager.Instance.SessionManager.CurrentSession.AttemptToAddMapCellInformation(cellInfo[cell]);
                   incrementYSize += ConstantsManager.Gameplay.StandardRoomHeightUnit;
                   cell++;
                }
                incrementYSize = ConstantsManager.Gameplay.StandardRoomHeightUnit / 2;
                incrementXSize += ConstantsManager.Gameplay.StandardRoomWidthUnit;
             }
             _hallwayVisited++;
            SetHallwayData(_hallwayVisited);
        }
        private void SetHallwayData( int status) {
            if (string.IsNullOrEmpty(hallwayID)) return;
            var trackingInfo = new TrackedMonoBehaviour.TrackingData {id = hallwayID, objectType = TrackedMonoBehaviour.TrackingType.Interactable};
            GameplayManager.Instance.SessionManager.CurrentSession.SetTrackedDataString(trackingInfo, status.ToString());
        }

        private int GetHallwayData() {
            if (string.IsNullOrEmpty(hallwayID)) return ConstantsManager.Quest.ErrorNoQuestName;

            var trackingInfo = new TrackedMonoBehaviour.TrackingData {id = hallwayID, objectType = TrackedMonoBehaviour.TrackingType.Interactable};
            var dataString = GameplayManager.Instance.SessionManager.CurrentSession.GetTrackedDataString(trackingInfo);
            
            return ReferenceEquals(dataString, null) ? 0 : int.Parse(dataString);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag(ConstantsManager.Tags.PlayerHit)) return;
            GameplayManager.Instance.UI.Pause.mapUI.LockMarkerPosition = true;
            GameplayManager.Instance.UI.Pause.mapUI.LockPosition = new Vector2(ConstantsManager.Map.EuropaDefaultPosHallwayX, ConstantsManager.Map.EuropaDefaultPosHallwayY);
            _hallwayVisited = GetHallwayData();
            if(_hallwayVisited > 0) return;
            BuildHallway();
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            if (!col.CompareTag(ConstantsManager.Tags.PlayerHit)) return;
            GameplayManager.Instance.UI.Pause.mapUI.LockMarkerPosition = false;
        }
        
    }
}