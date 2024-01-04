//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LookController : InputControllerBase
//{
//    GameObject m_Popup;
//    GameObject m_PopupInstance;

//    Point m_TileSelection;

//    public void Start()
//    {
//        //m_Popup = Resources.Load<GameObject>("Prefabs/UI/ItemPopup");

//        //Point p = WorldUtility.GetEntityPosition(Self);
//        //Services.TileSelectionService.SelectTile(p);

//        //m_TileSelection = p;
//        //m_PopupInstance = GameObject.Instantiate(m_Popup);
//        //m_PopupInstance.transform.SetParent(GameObject.Find("Canvas").transform);
//        //m_PopupInstance.GetComponent<InfoMono>().SourcePos = Camera.main.WorldToScreenPoint(WorldUtility.GetGameObject(WorldUtility.GetEntityAtPosition(m_TileSelection)).transform.position);

//        //string info = Services.TileInteractionService.ShowTileInfo(m_TileSelection);
//        //m_PopupInstance.GetComponent<InfoMono>().SetData(WorldUtility.GetEntityAtPosition(m_TileSelection).Name, info);

//        //UIManager.Push(null);
//    }

//    //public override void HandleEvent(GameEvent gameEvent)
//    //{
//    //    if (gameEvent.ID == GameEventId.UpdateEntity)
//    //    {
//    //        MoveDirection desiredDirection = InputUtility.GetMoveDirection();

//    //        if (desiredDirection != MoveDirection.None)
//    //        {
//    //            m_TileSelection = Services.TileSelectionService.SelectTileInNewDirection(m_TileSelection, desiredDirection);

//    //            string info = Services.TileInteractionService.ShowTileInfo(m_TileSelection);
//    //            string name = WorldUtility.GetEntityAtPosition(m_TileSelection).Name;
//    //            if (info == "Multiple Items")
//    //            {
//    //                name = info;
//    //                info = "";
//    //            }
//    //            m_PopupInstance.GetComponent<InfoMono>().SetData(name, info);

//    //            var entity = WorldUtility.GetEntityAtPosition(m_TileSelection);
//    //            var go = WorldUtility.GetGameObject(entity);
//    //            m_PopupInstance.GetComponent<InfoMono>().SourcePos = Camera.main.WorldToScreenPoint(go.transform.position);
//    //            Services.WorldUpdateService.UpdateWorldView();
//    //        }

//    //        else if (Input.GetKeyDown(KeyCode.Return))
//    //        {
//    //            Debug.Log(WorldUtility.GetEntityAtPosition(m_TileSelection, false).Serialize());
//    //        }

//    //        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(InputBinder.GetKeyCodeForAction(RequestedAction.Look)))
//    //        {
//    //            Self.RemoveComponent(this);
//    //            Self.AddComponent(new PlayerInputController());

//    //            Services.TileSelectionService.EndTileSelection(m_TileSelection);
//    //            Services.WorldUpdateService.UpdateWorldView();
               
//    //            GameObject.Destroy(m_PopupInstance);
//    //            UIManager.ForcePop();
//    //        }
//    //    }
//    //}
//}