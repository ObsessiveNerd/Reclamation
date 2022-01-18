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
        GameEvent selectTile = new GameEvent(GameEventId.SelectTile, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID),
                                                                               new KeyValuePair<string, object>(EventParameters.Target, null),
                                                                               new KeyValuePair<string, object>(EventParameters.TilePosition, WorldUtility.GetEntityPosition(Self)));
        FireEvent(World.Instance.Self, selectTile);

        m_TileSelection = (Point)selectTile.Paramters[EventParameters.TilePosition];
        m_PopupInstance = GameObject.Instantiate(m_Popup);
        m_PopupInstance.transform.SetParent(GameObject.Find("Canvas").transform);
        m_PopupInstance.GetComponent<InfoMono>().SourcePos = Camera.main.WorldToScreenPoint(WorldUtility.GetGameObject(WorldUtility.GetEntityAtPosition(m_TileSelection)).transform.position);
        GameEvent showTileInfo = new GameEvent(GameEventId.ShowTileInfo, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection),
                                                                            new KeyValuePair<string, object>(EventParameters.Info, ""));
        FireEvent(World.Instance.Self, showTileInfo);

        m_PopupInstance.GetComponent<InfoMono>().SetData(WorldUtility.GetEntityAtPosition(m_TileSelection).Name, showTileInfo.GetValue<string>(EventParameters.Info));

        UIManager.Push(null);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = new GameEvent(GameEventId.SelectNewTileInDirection, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection),
                                                                                                    new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
                FireEvent(World.Instance.Self, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];

                GameEvent showTileInfo = new GameEvent(GameEventId.ShowTileInfo, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection),
                                                                                    new KeyValuePair<string, object>(EventParameters.Info, ""));
                FireEvent(World.Instance.Self, showTileInfo);
                m_PopupInstance.GetComponent<InfoMono>().SourcePos = Camera.main.WorldToScreenPoint(WorldUtility.GetGameObject(WorldUtility.GetEntityAtPosition(m_TileSelection)).transform.position);
                m_PopupInstance.GetComponent<InfoMono>().SetData(WorldUtility.GetEntityAtPosition(m_TileSelection).Name, showTileInfo.GetValue<string>(EventParameters.Info));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
            }

            else if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log(WorldUtility.GetEntityAtPosition(m_TileSelection, false).Serialize());
            }

            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(InputBinder.GetKeyCodeForAction(RequestedAction.Look)))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController());
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.EndSelection, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection)));
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