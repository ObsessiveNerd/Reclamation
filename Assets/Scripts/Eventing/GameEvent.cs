using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventId
{
    //Inventory
    public const string OpenInventory = nameof(OpenInventory);
    public const string CloseInventory = nameof(CloseInventory);
    public const string AddToInventory = nameof(AddToInventory);
    public const string RemoveFromInventory = nameof(RemoveFromInventory);
    public const string EmptyBag = nameof(EmptyBag);

    //Item
    public const string Pickup = nameof(Pickup);
    public const string Drop = nameof(Drop);

    //World
    public const string StartWorld = nameof(StartWorld);
    public const string UpdateWorldView = nameof(UpdateWorldView);
    public const string Spawn = nameof(Spawn);
    public const string Despawn = nameof(Despawn);
    public const string SelectTile = nameof(SelectTile);
    public const string SelectNewTileInDirection = nameof(SelectNewTileInDirection);
    public const string GetActivePlayer = nameof(GetActivePlayer);

    //Moving
    public const string BeforeMoving = nameof(BeforeMoving);
    public const string ExecuteMove = nameof(ExecuteMove);
    public const string AfterMoving = nameof(AfterMoving);
    public const string MoveEntity = nameof(MoveEntity);
    public const string Interact = nameof(Interact);

    //Time progression
    public const string StartTurn = nameof(StartTurn);
    public const string UpdateEntity = nameof(UpdateEntity);
    public const string EndTurn = nameof(EndTurn);
    public const string RegisterWithTimeSystem = nameof(RegisterWithTimeSystem);

    //Rendering
    public const string UpdateRenderer = nameof(UpdateRenderer);
    public const string GetRenderSprite = nameof(GetRenderSprite);
    public const string GetSprite = nameof(GetSprite);
    public const string AlterSprite = nameof(AlterSprite);

    //Tiles
    public const string UpdateTile = nameof(UpdateTile);
    public const string EndSelection = nameof(EndSelection);

    //Character Selection
    public const string RotateActiveCharacter = nameof(RotateActiveCharacter);

    //Input
    public const string MoveKeyPressed = nameof(MoveKeyPressed);
    public const string HasInputController = nameof(HasInputController);
    public const string PromptForInput = nameof(PromptForInput);

    //Energy
    public const string HasEnoughEnergyToTakeATurn = nameof(HasEnoughEnergyToTakeATurn);
    public const string UseEnergy = nameof(UseEnergy);
    public const string AlterEnergy = nameof(AlterEnergy);
    public const string GetMinimumEnergyForAction = nameof(GetMinimumEnergyForAction);
    public const string SkipTurn = nameof(SkipTurn);
}

public static class EventParameters
{
    public const string InputDirection = nameof(InputDirection);
    public const string TakeTurn = nameof(TakeTurn);
    public const string UpdateWorld = nameof(UpdateWorld);
    public const string CleanupComponents = nameof(CleanupComponents);
    public const string RemainingEnergy = nameof(RemainingEnergy);
    public const string RequiredEnergy = nameof(RequiredEnergy);
    public const string EnergyRegen = nameof(EnergyRegen);
    public const string ActionTaken = nameof(ActionTaken);
    public const string RenderSprite = nameof(RenderSprite);
    public const string Renderer = nameof(Renderer);
    public const string Point = nameof(Point);
    public const string Entity = nameof(Entity);
    public const string EntityType = nameof(EntityType);
    public const string Creature = nameof(Creature);
    public const string Value = nameof(Value);
    public const string TilePosition = nameof(TilePosition);
}

public class GameEvent
{
    public string ID { get { return m_ID; } }
    string m_ID { get; }

    public Dictionary<string, object> Paramters { get { return m_Parameters; } }
    Dictionary<string, object> m_Parameters;

    public GameEvent(string id, params KeyValuePair<string, object>[] parameters)
    {
        m_ID = id;
        m_Parameters = new Dictionary<string, object>();
        foreach (var param in parameters)
            m_Parameters[param.Key] = param.Value;
    }

    public GameEvent(string id, Dictionary<string, object> parameters)
    {
        m_ID = id;
        m_Parameters = parameters;
    }
}
