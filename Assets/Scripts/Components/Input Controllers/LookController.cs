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
        GameEvent selectTile = GameEventPool.Get(GameEventId.SelectTile)
            .With(EventParameters.Entity, Self.ID)
            .With(EventParameters.Target, null)
            .With(EventParameters.TilePosition, WorldUtility.GetEntityPosition(Self));
        FireEvent(World.Services.Self, selectTile);

        m_TileSelection = (Point)selectTile.Paramters[EventParameters.TilePosition];
        m_PopupInstance = GameObject.Instantiate(m_Popup);
        m_PopupInstance.transform.SetParent(GameObject.Find("Canvas").transform);
        m_PopupInstance.GetComponent<InfoMono>().SourcePos = Camera.main.WorldToScreenPoint(WorldUtility.GetGameObject(WorldUtility.GetEntityAtPosition(m_TileSelection)).transform.position);
        GameEvent showTileInfo = GameEventPool.Get(GameEventId.ShowTileInfo)
            .With(EventParameters.TilePosition, m_TileSelection)
            .With(EventParameters.Info, "");
        FireEvent(World.Services.Self, showTileInfo);

        m_PopupInstance.GetComponent<InfoMono>().SetData(WorldUtility.GetEntityAtPosition(m_TileSelection).Name, showTileInfo.GetValue<string>(EventParameters.Info));

        UIManager.Push(null);

        selectTile.Release();
        showTileInfo.Release();
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = GameEventPool.Get(GameEventId.SelectNewTileInDirection)
                    .With(EventParameters.InputDirection, desiredDirection)
                    .With(EventParameters.TilePosition, m_TileSelection);
                FireEvent(World.Services.Self, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];
                moveSelection.Release();

                GameEvent showTileInfo = GameEventPool.Get(GameEventId.ShowTileInfo).With(EventParameters.TilePosition, m_TileSelection)
                                                                                    .With(EventParameters.Info, "");
                FireEvent(World.Services.Self, showTileInfo);
                m_PopupInstance.GetComponent<InfoMono>().SourcePos = Camera.main.WorldToScreenPoint(WorldUtility.GetGameObject(WorldUtility.GetEntityAtPosition(m_TileSelection)).transform.position);
                m_PopupInstance.GetComponent<InfoMono>().SetData(WorldUtility.GetEntityAtPosition(m_TileSelection).Name, showTileInfo.GetValue<string>(EventParameters.Info));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                showTileInfo.Release();
            }

            else if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log(WorldUtility.GetEntityAtPosition(m_TileSelection, false).Serialize());
            }

            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(InputBinder.GetKeyCodeForAction(RequestedAction.Look)))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());
                FireEvent(World.Services.Self, GameEventPool.Get(GameEventId.EndSelection)
                    .With(EventParameters.TilePosition, m_TileSelection)).Release();
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                GameObject.Destroy(m_PopupInstance);
                UIManager.ForcePop();
                //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
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