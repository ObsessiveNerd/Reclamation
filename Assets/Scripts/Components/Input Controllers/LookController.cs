using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookController : InputControllerBase
{
    GameObject m_Popup;
    GameObject m_PopupInstance;

    Point m_TileSelection;

    public override void Init(IEntity self)
    {
        m_Popup = Resources.Load<GameObject>("UI/ItemPopup");

        base.Init(self);

        Point p = WorldUtility.GetEntityPosition(Self);
        Services.TileSelectionService.SelectTile(p);

        m_TileSelection = p;
        m_PopupInstance = GameObject.Instantiate(m_Popup);
        m_PopupInstance.transform.SetParent(GameObject.Find("Canvas").transform);
        m_PopupInstance.GetComponent<InfoMono>().SourcePos = Camera.main.WorldToScreenPoint(WorldUtility.GetGameObject(WorldUtility.GetEntityAtPosition(m_TileSelection)).transform.position);

        string info = Services.TileInteractionService.ShowTileInfo(m_TileSelection);
        m_PopupInstance.GetComponent<InfoMono>().SetData(WorldUtility.GetEntityAtPosition(m_TileSelection).Name, info);

        UIManager.Push(null);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                m_TileSelection = Services.TileSelectionService.SelectTileInNewDirection(m_TileSelection, desiredDirection);

                string info = Services.TileInteractionService.ShowTileInfo(m_TileSelection);
                m_PopupInstance.GetComponent<InfoMono>().SetData(WorldUtility.GetEntityAtPosition(m_TileSelection).Name, info);
                m_PopupInstance.GetComponent<InfoMono>().SourcePos = Camera.main.WorldToScreenPoint(WorldUtility.GetGameObject(WorldUtility.GetEntityAtPosition(m_TileSelection)).transform.position);
                Services.WorldUpdateService.UpdateWorldView();
            }

            else if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log(WorldUtility.GetEntityAtPosition(m_TileSelection, false).Serialize());
            }

            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(InputBinder.GetKeyCodeForAction(RequestedAction.Look)))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());

                Services.TileSelectionService.EndTileSelection(m_TileSelection);
                Services.WorldUpdateService.UpdateWorldView();
               
                GameObject.Destroy(m_PopupInstance);
                UIManager.ForcePop();
            }
        }
    }
}

public class DTO_LookController : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new LookController();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(PlayerInputController);
    }
}