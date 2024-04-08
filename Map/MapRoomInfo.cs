   using System;
using System.Collections.Generic;
using _9YoS.Scripts.Europa.Neutral.Central;
   using _9YoS.Scripts.Managers;
using LDtkUnity;
using UnityEngine;

namespace _9YoS.Scripts.UI.Map {
   public class MapRoomInfo : MonoBehaviour {
      
      [SerializeField] private LDtkComponentLevel componentLevel;
      [SerializeField] private ConstantsManager.Map.Areas myArea;
      [SerializeField] private MapCellSpriteLibrary.MapCellColorSwatch _areaColor;
      [Header("Custom Room")]
      [SerializeField] private bool isSecretRoom;
      [SerializeField] private bool roomIsAlreadyDiscovered;
      
      private MapGeneratorUI.MapCellInformation[] _cellInfo;
      private CompositeCollider2D _collider2D;

      private Vector2[] _realWorldPositions;
      private Vector2 _firstCoordinate;
      private Vector2 _offset;
      private Vector2 _colliderSize;
      
      private int _cellNumber;
      private int _horizontalRooms;
      private int _verticalRooms;

      private float _incrementXSize;
      private float _incrementYSize;

      private string _roomId;
      private ConstantsManager.Map.Areas _areaIdentifier;
      
      private ConstantsManager.Gameplay.CardinalDirection _rayDirection;

      private RaycastHit2D[] _hit1;
      private RaycastHit2D[] _hit2;
      private RaycastHit2D[] _hit3;
      

      public LDtkComponentLevel ComponentLevel => componentLevel;
      public Vector2 Offset => _offset;

      private void Awake() {
         _collider2D = GetComponent<CompositeCollider2D>();
         componentLevel = GetComponentInParent<LDtkComponentLevel>();
      }

      public void GetCellsAndPositions()
      {
         UpdateMyOffset();
         var bounds = _collider2D.bounds;
         _colliderSize = bounds.size;
         _horizontalRooms = Mathf.RoundToInt(_colliderSize.x / ConstantsManager.Gameplay.StandardRoomWidthUnit );
         _verticalRooms = Mathf.RoundToInt(_colliderSize.y / ConstantsManager.Gameplay.StandardRoomHeightUnit);

         _cellNumber = _horizontalRooms * _verticalRooms;
         _cellInfo = new MapGeneratorUI.MapCellInformation[_cellNumber];
         _realWorldPositions = new Vector2[_cellNumber];

         _firstCoordinate = new Vector2((bounds.center.x - bounds.extents.x), (bounds.center.y - bounds.extents.y));
         
         _incrementXSize = ConstantsManager.Gameplay.StandardRoomWidthUnit  / 2;
         _incrementYSize = ConstantsManager.Gameplay.StandardRoomHeightUnit / 2;

        
         var cell = 0;
         for (var h = 0; h < _horizontalRooms; h++) {
            for (var v = 0; v < _verticalRooms; v++) {
               var integerCoordX = Mathf.FloorToInt((_firstCoordinate.x + _incrementXSize + _offset.x) / ConstantsManager.Gameplay.StandardRoomWidthUnit);
               var integerCoordY = Mathf.FloorToInt((_firstCoordinate.y + _incrementYSize + _offset.y) / ConstantsManager.Gameplay.StandardRoomHeightUnit);
               
               _cellInfo[cell].position.x = integerCoordX;
               _cellInfo[cell].position.y = integerCoordY;
               
               _realWorldPositions[cell] = new Vector2(Mathf.RoundToInt(_firstCoordinate.x + _incrementXSize), Mathf.RoundToInt(_firstCoordinate.y + _incrementYSize));
               _incrementYSize += ConstantsManager.Gameplay.StandardRoomHeightUnit;

               cell++;
            }

            _incrementYSize = ConstantsManager.Gameplay.StandardRoomHeightUnit / 2;
            _incrementXSize += ConstantsManager.Gameplay.StandardRoomWidthUnit;
         }
         GetCellInfo();
         AssignIdentifierInfo();
         CheckWalls();
      }
      
      private void CheckWalls() {
         var bounds = _collider2D.bounds;
         for (var i = 0; i < _cellInfo.Length; i++) {
            _cellInfo[i].borderTypeRight = Math.Abs(_realWorldPositions[i].x + 3.5 - (bounds.center.x + bounds.extents.x)) < 1 ? MapCellSpriteLibrary.MapCellBorder.Solid : MapCellSpriteLibrary.MapCellBorder.None;
            _cellInfo[i].borderTypeLeft = Math.Abs(_realWorldPositions[i].x - 3.5 - (bounds.center.x - bounds.extents.x)) < 1? MapCellSpriteLibrary.MapCellBorder.Solid : MapCellSpriteLibrary.MapCellBorder.None;
            _cellInfo[i].borderTypeTop = Math.Abs(_realWorldPositions[i].y + 2.2 - (bounds.center.y + bounds.extents.y)) < 1 ? MapCellSpriteLibrary.MapCellBorder.Solid : MapCellSpriteLibrary.MapCellBorder.None;
            _cellInfo[i].borderTypeBottom = Math.Abs(_realWorldPositions[i].y - 2.2 - (bounds.center.y - bounds.extents.y)) < 1 ? MapCellSpriteLibrary.MapCellBorder.Solid : MapCellSpriteLibrary.MapCellBorder.None;
            
            if (_cellInfo[i].borderTypeRight == MapCellSpriteLibrary.MapCellBorder.None) _cellInfo[i].borderTypeRight = DoubleCheckEmptyWalls(ConstantsManager.Gameplay.CardinalDirection.Right,i);
            if (_cellInfo[i].borderTypeLeft == MapCellSpriteLibrary.MapCellBorder.None) _cellInfo[i].borderTypeLeft = DoubleCheckEmptyWalls(ConstantsManager.Gameplay.CardinalDirection.Left, i);
            if (_cellInfo[i].borderTypeTop == MapCellSpriteLibrary.MapCellBorder.None) _cellInfo[i].borderTypeTop = DoubleCheckEmptyWalls(ConstantsManager.Gameplay.CardinalDirection.Up, i);
            if (_cellInfo[i].borderTypeBottom == MapCellSpriteLibrary.MapCellBorder.None) _cellInfo[i].borderTypeBottom = DoubleCheckEmptyWalls(ConstantsManager.Gameplay.CardinalDirection.Down, i);
            
            if (_cellInfo[i].borderTypeRight == MapCellSpriteLibrary.MapCellBorder.Solid) _cellInfo[i].borderTypeRight = ScanWalls(ConstantsManager.Gameplay.CardinalDirection.Right,i);
            if (_cellInfo[i].borderTypeLeft == MapCellSpriteLibrary.MapCellBorder.Solid) _cellInfo[i].borderTypeLeft = ScanWalls(ConstantsManager.Gameplay.CardinalDirection.Left, i);
            if (_cellInfo[i].borderTypeTop == MapCellSpriteLibrary.MapCellBorder.Solid) _cellInfo[i].borderTypeTop = ScanWalls(ConstantsManager.Gameplay.CardinalDirection.Up, i);
            if (_cellInfo[i].borderTypeBottom == MapCellSpriteLibrary.MapCellBorder.Solid) _cellInfo[i].borderTypeBottom = ScanWalls(ConstantsManager.Gameplay.CardinalDirection.Down, i);

            _cellInfo[i].isSpecialRoom = CheckIfSpecialRoom(i);
         }  
         SetColor();
         if(roomIsAlreadyDiscovered) IsExplored();
      }

      private MapCellSpriteLibrary.MapCellBorder ScanWalls(ConstantsManager.Gameplay.CardinalDirection rayDirection, int cell) {
      
         GetRayDirection(rayDirection, cell, ConstantsManager.LayerNames.Ground);
         
         var solidCenter = false;
         foreach (var hit in _hit1) {
            if (hit.collider == null) continue;
            if (hit.collider.CompareTag(ConstantsManager.Tags.PlatformGround)) continue;
            if (hit.collider.CompareTag(ConstantsManager.Tags.Untagged)) continue;
            solidCenter = true;
         }    
         var solidUpper = false;
         foreach (var hit in _hit2) {
            if (hit.collider == null) continue;
            if (hit.collider.CompareTag(ConstantsManager.Tags.PlatformGround)) continue;
            if (hit.collider.CompareTag(ConstantsManager.Tags.Untagged)) continue;
            solidUpper = true;
         }   
         
         var solidLower = false;
         foreach (var hit in _hit3) {
            if (hit.collider == null) continue;
            if (hit.collider.CompareTag(ConstantsManager.Tags.PlatformGround)) continue;
            if (hit.collider.CompareTag(ConstantsManager.Tags.Untagged)) continue;
            solidLower = true;
         }
         //DoubleCheck
         GetRayDirection(rayDirection, cell, ConstantsManager.LayerNames.Default);
         
         if (!solidLower)
         {
            foreach (var hit in _hit3)
            {
               if(!hit.collider.CompareTag(ConstantsManager.Tags.RoomBlock)) continue;
               solidLower = true;
            }
         }
         if (!solidUpper)
         {
            foreach (var hit in _hit2)
            {
               if(!hit.collider.CompareTag(ConstantsManager.Tags.RoomBlock)) continue;
               solidUpper = true;
            }
         }
         if (!solidCenter)
         {
            foreach (var hit in _hit1)
            {
               if(!hit.collider.CompareTag(ConstantsManager.Tags.RoomBlock)) continue;
               solidCenter = true;
            }
         }  
            

         if (solidLower)
         {
            foreach (var hit in _hit3)
            {
               if(!hit.collider.CompareTag(ConstantsManager.Tags.RoomTransition)) continue;
               solidLower = false;
            }
         }
         if (solidUpper)
         {
            foreach (var hit in _hit2)
            {
               if(!hit.collider.CompareTag(ConstantsManager.Tags.RoomTransition)) continue;
               solidUpper = false;
            }
         }
         if (solidCenter)
         {
            foreach (var hit in _hit1)
            {
               if(!hit.collider.CompareTag(ConstantsManager.Tags.RoomTransition)) continue;
               solidCenter = false;
            }
         }

         if(solidCenter && solidLower && solidUpper) return MapCellSpriteLibrary.MapCellBorder.Solid;

         return MapCellSpriteLibrary.MapCellBorder.Partial;
      }

      private MapCellSpriteLibrary.MapCellBorder DoubleCheckEmptyWalls(ConstantsManager.Gameplay.CardinalDirection rayDirection, int cell)
      {
          GetRayDirection(rayDirection, cell, ConstantsManager.LayerNames.Ground);
         
         var solidCenter = false;
         var solidUpper = false;
         var solidLower = false;
 
         //DoubleCheck
         GetRayDirection(rayDirection, cell, ConstantsManager.LayerNames.Default);
         foreach (var hit in _hit3)
         {
            if(!hit.collider.CompareTag(ConstantsManager.Tags.RoomBlock)) continue; 
            solidLower = true;
         }
         foreach (var hit in _hit2)
         {
            if(!hit.collider.CompareTag(ConstantsManager.Tags.RoomBlock)) continue; 
            solidUpper = true;
         }
         foreach (var hit in _hit1)
         {
            if(!hit.collider.CompareTag(ConstantsManager.Tags.RoomBlock)) continue; 
            solidCenter = true;
         }
         
         if(solidCenter && solidLower && solidUpper) return MapCellSpriteLibrary.MapCellBorder.Solid;

         return MapCellSpriteLibrary.MapCellBorder.None;
      }

      private MapCellSpriteLibrary.MapCellColorSwatch ScanForElementalRoom(int cell) {
         var boxCenter = new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y);
         var boxSize = new Vector2(.5f, .5f);
         
         var colliders = new List<Collider2D>();
         var contactFilter2D = new ContactFilter2D { layerMask = LayerMask.GetMask(ConstantsManager.LayerNames.DetectOnlyPlayer), useTriggers = true };

         Physics2D.OverlapBox(boxCenter, boxSize, 0f, contactFilter2D, colliders);
         
         foreach (var colliderHit in colliders) {
            if (colliderHit.CompareTag(ConstantsManager.Tags.WaterArea)) return MapCellSpriteLibrary.MapCellColorSwatch.WaterCell;
            if (colliderHit.CompareTag(ConstantsManager.Tags.HeatArea)) return MapCellSpriteLibrary.MapCellColorSwatch.HeatCell;
            if (colliderHit.CompareTag(ConstantsManager.Tags.PoisonArea)) return MapCellSpriteLibrary.MapCellColorSwatch.PoisonCell;
            if (colliderHit.CompareTag(ConstantsManager.Tags.TunnelArea)) return MapCellSpriteLibrary.MapCellColorSwatch.TunnelCell;
         }
         
         return _areaColor;
      }
      
      private MapCellSpriteLibrary.MapCellColorSwatch ScanForSpecialRoom(int cell) {
         var boxCenter = new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y);
         var boxSize = new Vector2(.5f, .5f);
         
         var colliders = new List<Collider2D>();
         var contactFilter2D = new ContactFilter2D { layerMask = LayerMask.GetMask(ConstantsManager.LayerNames.Default), useTriggers = true };

         Physics2D.OverlapBox(boxCenter, boxSize, 0f, contactFilter2D, colliders);
         
         foreach (var colliderHit in colliders) {
            if (colliderHit.CompareTag(ConstantsManager.Tags.SaveRoom)) return MapCellSpriteLibrary.MapCellColorSwatch.Save;
            if (colliderHit.CompareTag(ConstantsManager.Tags.HibinoRoom)) return MapCellSpriteLibrary.MapCellColorSwatch.Hibino;
            if (colliderHit.CompareTag(ConstantsManager.Tags.TheatreRoom)) return MapCellSpriteLibrary.MapCellColorSwatch.Theatre;
            if (colliderHit.CompareTag(ConstantsManager.Tags.TransitionRoom)) return MapCellSpriteLibrary.MapCellColorSwatch.TransitionRoom;
         }
         return _areaColor;
      }

      private void GetRayDirection(ConstantsManager.Gameplay.CardinalDirection rayDirection, int cell, string layerMask)
      {
         switch (rayDirection)
         {
            case ConstantsManager.Gameplay.CardinalDirection.Up:
               _hit1 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y), Vector2.up, 3f, LayerMask.GetMask(layerMask));
               _hit2 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x + 1.55f, _realWorldPositions[cell].y), Vector2.up, 3f, LayerMask.GetMask(layerMask)); 
               _hit3 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x - 1.55f, _realWorldPositions[cell].y), Vector2.up, 3f, LayerMask.GetMask(layerMask));
               break ;
            case ConstantsManager.Gameplay.CardinalDirection.Right:
               _hit1 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y), Vector2.right,  3.5f, LayerMask.GetMask(layerMask));
               _hit2 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y + 0.96f), Vector2.right, 3.5f, LayerMask.GetMask(layerMask));
               _hit3 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y - 0.96f), Vector2.right, 3.5f, LayerMask.GetMask(layerMask));
               break ;
            case ConstantsManager.Gameplay.CardinalDirection.Down:
               _hit1 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y),  Vector2.down, 3f, LayerMask.GetMask(layerMask));
               _hit2 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x + 1.55f, _realWorldPositions[cell].y),  Vector2.down, 3f, LayerMask.GetMask(layerMask)); 
               _hit3 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x - 1.55f, _realWorldPositions[cell].y),  Vector2.down, 3f, LayerMask.GetMask(layerMask));
               break ;
            case ConstantsManager.Gameplay.CardinalDirection.Left:
               _hit1 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y), Vector2.left,  3.5f, LayerMask.GetMask(layerMask));
               _hit2 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y + 0.96f), Vector2.left, 3.5f, LayerMask.GetMask(layerMask));
               _hit3 = Physics2D.RaycastAll(new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y - 0.96f), Vector2.left, 3.5f, LayerMask.GetMask(layerMask));
               break ;
         }
      }
      
      private bool CheckIfSpecialRoom(int cell) {
         var boxCenter = new Vector2(_realWorldPositions[cell].x, _realWorldPositions[cell].y);
         var boxSize = new Vector2(0.5f, 0.5f);
         
         var colliders = new List<Collider2D>();
         var contactFilter2D = new ContactFilter2D { layerMask = LayerMask.GetMask(ConstantsManager.LayerNames.Default), useTriggers = true };

         Physics2D.OverlapBox(boxCenter, boxSize, 0f, contactFilter2D, colliders);
         
         foreach (var colliderHit in colliders)
         {
            if (colliderHit.CompareTag(ConstantsManager.Tags.SaveRoom)) return true;
            if (colliderHit.CompareTag(ConstantsManager.Tags.HibinoRoom)) return true;
            if (colliderHit.CompareTag(ConstantsManager.Tags.TheatreRoom)) return true;
            if (colliderHit.CompareTag(ConstantsManager.Tags.TransitionRoom)) return true;
         }
         return false;
      }

      public void IsExplored() {
         
         GameplayManager.Instance.UI.Pause.mapUI.CurrentAreaOffset = _offset;
         for (var i = 0; i < _cellInfo.Length; i++)
         {
            if(_cellInfo[i].roomStatus == MapCellSpriteLibrary.RoomStatus.Explored) continue;
            _cellInfo[i].roomStatus = MapCellSpriteLibrary.RoomStatus.Explored;
         }
         
         if (isSecretRoom) {
            var currentSecretRoomVisited = PlayerController.Instance.Europa.ProgressionData.NumberOfSecretRoomsFound;
            currentSecretRoomVisited += 1;
            PlayerController.Instance.Europa.ProgressionData.NumberOfSecretRoomsFound = currentSecretRoomVisited;
            CheckGrantAchievement();
         }
         UpdateInfo();
      }
      
      public void IsMapped() {
         for (var i = 0; i < _cellInfo.Length; i++) {
            if (isSecretRoom) return;
            switch (_cellInfo[i].roomStatus)
            {
               case MapCellSpriteLibrary.RoomStatus.Mapped:
               case MapCellSpriteLibrary.RoomStatus.Explored:
                  continue;
               case MapCellSpriteLibrary.RoomStatus.Undiscovered:
                  _cellInfo[i].roomStatus = MapCellSpriteLibrary.RoomStatus.Mapped;
                  break;
            }
         }
         UpdateInfo();
      }

      private void UpdateInfo() {
         foreach (var cell in _cellInfo) {
            GameplayManager.Instance.SessionManager.CurrentSession.AttemptToAddMapCellInformation(cell);
         }
      }

      private void SetColor() {
         for (var i = 0; i < _cellInfo.Length; i++) {
            if (_cellInfo[i].isSpecialRoom) {
               _cellInfo[i].cellFillSwatch = ScanForSpecialRoom(i);
               _cellInfo[i].cellBorderSwatch = ScanForSpecialRoom(i);
            } else {
               _cellInfo[i].cellFillSwatch = ScanForElementalRoom(i);
               _cellInfo[i].cellBorderSwatch = _areaColor;
            }
         }
      }
      public void AreaManagerOffSet(float xOffSet, float yOffset) {
         _offset.x = xOffSet;
         _offset.y = yOffset;
       
      }

      private void UpdateMyOffset()
      {
         if(myArea == ConstantsManager.Map.Areas.Default) return;
         switch (myArea)
         {
            case ConstantsManager.Map.Areas.AreaHTop:
               _offset.x = ConstantsManager.Map.AreaOffSet.TopHOffsetX;
               _offset.y =  ConstantsManager.Map.AreaOffSet.TopHOffsetY;
               break;
            case ConstantsManager.Map.Areas.AreaH:
               _offset.x = ConstantsManager.Map.AreaOffSet.HOffsetX;
               _offset.y =  ConstantsManager.Map.AreaOffSet.HOffsetY;
               break;
         }
      }
      
      public void AreaManagerGiveColor(MapCellSpriteLibrary.MapCellColorSwatch colorSwatch) {
         _areaColor = colorSwatch;
      }
      
      private void CheckGrantAchievement() {
         var mapAreas = PlayerController.Instance.Europa.ProgressionData.NumberOfSecretRoomsFound;
         
         if(mapAreas < ConstantsManager.Achievement.NumberOfSecretRooms) return;
         AchievementManager.AttemptToGrantAchievement(ConstantsManager.Achievement.FoundSecretRooms);
      }

      private void GetCellInfo()
      {
         for (var i = 0; i < _cellInfo.Length; i++) {
            _cellInfo[i] = GameplayManager.Instance.SessionManager.CurrentSession.GetMapCellData(_cellInfo[i]);
         }
         UpdateMapCell();
      }

      private void UpdateMapCell()
      {
         for (var i = 0; i < _cellInfo.Length; i++)
         {
            if(_cellInfo[i].roomStatus == MapCellSpriteLibrary.RoomStatus.Undiscovered) continue;
            SetColor();
            CheckWalls();
            UpdateInfo();
         }
      }

      public void AreaManagerGiveIdentifierInfo(string componentLevelIdentifier, ConstantsManager.Map.Areas area)
      {
         _roomId = componentLevelIdentifier;
         _areaIdentifier = area;
      }

      private void  AssignIdentifierInfo()
      {
         for (var i = 0; i < _cellInfo.Length; i++) {
            _cellInfo[i].roomId = _roomId;
            _cellInfo[i].worldArea = _areaIdentifier;
         }
         UpdateInfo();
      }
   }
}
