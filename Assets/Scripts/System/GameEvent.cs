using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventId
{
    //AI
    public const string GetActionToTake = nameof(GetActionToTake);
    public const string GetCombatRating = nameof(GetCombatRating);
    public const string GetPackInformation = nameof(GetPackInformation);
    public const string BreakRank = nameof(BreakRank);

    //FOV
    public const string FOVRecalculated = nameof(FOVRecalculated);
    public const string BeforeFOVRecalculated = nameof(BeforeFOVRecalculated);
    public const string SetVisibility = nameof(SetVisibility);
    public const string SetHasBeenVisited = nameof(SetHasBeenVisited);
    public const string VisibilityUpdated = nameof(VisibilityUpdated);
    public const string CheckVisibility = nameof(CheckVisibility);
    public const string IsTileBlocking = nameof(IsTileBlocking);
    public const string InitFOV = nameof(InitFOV);
    public const string GetVisibleTiles = nameof(GetVisibleTiles);

    //Entity Info
    public const string GetEntityType = nameof(GetEntityType);

    //Factions
    public const string GetFaction = nameof(GetFaction);

    //Inventory
    public const string OpenInventory = nameof(OpenInventory);
    public const string CloseInventory = nameof(CloseInventory);
    public const string AddToInventory = nameof(AddToInventory);
    public const string RemoveFromInventory = nameof(RemoveFromInventory);
    public const string EmptyBag = nameof(EmptyBag);
    public const string ItemEquipped = nameof(ItemEquipped);
    public const string ItemUnequipped = nameof(ItemUnequipped);
    public const string Equip = nameof(Equip);
    public const string Unequip = nameof(Unequip);
    public const string GetRangedWeapon = nameof(GetRangedWeapon);
    public const string GetIcon = nameof(GetIcon);
    public const string GetCurrentInventory = nameof(GetCurrentInventory);

    //Item
    public const string Pickup = nameof(Pickup);
    public const string Drop = nameof(Drop);
    public const string GetValue = nameof(GetValue);
    public const string AddItem = nameof(AddItem);
    public const string RemoveItem = nameof(RemoveItem);
    public const string AddItems = nameof(AddItems);
    public const string GetRarity = nameof(GetRarity);
    public const string GetItems = nameof(GetItems);
    public const string DropItemsOnMap = nameof(DropItemsOnMap);
    public const string AddItemsToInventory = nameof(AddItemsToInventory);
    public const string GetBodyPartType = nameof(GetBodyPartType);

    //Debug
    public const string RevealAllTiles = nameof(RevealAllTiles);

    //World
    public const string StartWorld = nameof(StartWorld);
    public const string UpdateWorldView = nameof(UpdateWorldView);
    public const string Spawn = nameof(Spawn);
    public const string Despawn = nameof(Despawn);
    public const string SelectTile = nameof(SelectTile);
    public const string SelectNewTileInDirection = nameof(SelectNewTileInDirection);
    public const string GetEntityOnTile = nameof(GetEntityOnTile);
    public const string ShowInfo = nameof(ShowInfo);
    public const string AddComponentToTile = nameof(AddComponentToTile);
    public const string ProgressTime = nameof(ProgressTime);
    public const string ProgressTimeUntilIdHasTakenTurn = nameof(ProgressTimeUntilIdHasTakenTurn);
    public const string PauseTime = nameof(PauseTime);
    public const string UnPauseTime = nameof(UnPauseTime);
    public const string RegisterEntity = nameof(RegisterEntity);
    public const string DestroyEntity = nameof(DestroyEntity);
    public const string GetEntity = nameof(GetEntity);
    public const string CalculatePath = nameof(CalculatePath);
    public const string IsValidDungeonTile = nameof(IsValidDungeonTile);
    public const string GetClosestEnemy = nameof(GetClosestEnemy);
    public const string GetActivePlayerId = nameof(GetActivePlayerId);
    public const string GetCurrentLevel = nameof(GetCurrentLevel);
    public const string GameWin = nameof(GameWin);
    public const string GameFailure = nameof(GameFailure);
    public const string CleanFoVData = nameof(CleanFoVData);

    //Dungeon
    public const string GetRandomValidPoint = nameof(GetRandomValidPoint);
    public const string GetRandomValidPointInRange = nameof(GetRandomValidPointInRange);
    public const string AddValidPoints = nameof(AddValidPoints);
    public const string MoveUp = nameof(MoveUp);
    public const string MoveDown = nameof(MoveDown);

    //Moving
    public const string EntityOvertaking = nameof(EntityOvertaking);
    public const string BeforeMoving = nameof(BeforeMoving);
    public const string ExecuteMove = nameof(ExecuteMove);
    public const string AfterMoving = nameof(AfterMoving);
    public const string MoveEntity = nameof(MoveEntity);
    public const string SetEntityPosition = nameof(SetEntityPosition);
    public const string SetPoint = nameof(SetPoint);
    public const string GetPoint = nameof(GetPoint);
    public const string Interact = nameof(Interact);
    public const string ForceInteract = nameof(ForceInteract);
    public const string InteractInDirection = nameof(InteractInDirection);
    public const string InteractWithTarget = nameof(InteractWithTarget);
    public const string StopMovement = nameof(StopMovement);

    //Time progression
    public const string StartTurn = nameof(StartTurn);
    public const string CharacterRotated = nameof(CharacterRotated);
    public const string UpdateEntity = nameof(UpdateEntity);
    public const string ActivateObject = nameof(ActivateObject);
    public const string DeactivateObject = nameof(DeactivateObject);
    public const string EndTurn = nameof(EndTurn);
    public const string RegisterWithTimeSystem = nameof(RegisterWithTimeSystem);
    public const string RegisterPlayableCharacter = nameof(RegisterPlayableCharacter);
    public const string UnRegisterPlayer = nameof(UnRegisterPlayer);

    //Rendering
    public const string UpdateRenderer = nameof(UpdateRenderer);
    public const string GetRenderSprite = nameof(GetRenderSprite);
    public const string GetSprite = nameof(GetSprite);
    public const string AlterSprite = nameof(AlterSprite);

    //Tiles
    public const string UpdateTile = nameof(UpdateTile);
    public const string EndSelection = nameof(EndSelection);
    public const string ShowTileInfo = nameof(ShowTileInfo);
    public const string DestroyObject = nameof(DestroyObject);
    public const string CleanTile = nameof(CleanTile);
    public const string PathfindingData = nameof(PathfindingData);
    public const string IsValidTile = nameof(IsValidTile);
    public const string GetValueOnTile = nameof(GetValueOnTile);
    public const string SaveLevel = nameof(SaveLevel);
    public const string SaveVisibilityData = nameof(SaveVisibilityData);
    public const string LoadLevel = nameof(LoadLevel);
    public const string SerializeTile = nameof(SerializeTile);
    public const string GetVisibilityData = nameof(GetVisibilityData);

    //Camera
    public const string SetCameraPosition = nameof(SetCameraPosition);
    public const string UpdateCamera = nameof(UpdateCamera);

    //Character Selection
    public const string RotateActiveCharacter = nameof(RotateActiveCharacter);
    public const string SetActiveCharacter = nameof(SetActiveCharacter);
    public const string ConvertToPlayableCharacter = nameof(ConvertToPlayableCharacter);

    //Input
    public const string MoveKeyPressed = nameof(MoveKeyPressed);
    public const string PromptForInput = nameof(PromptForInput);

    //Energy
    public const string HasEnoughEnergyToTakeATurn = nameof(HasEnoughEnergyToTakeATurn);
    public const string UseEnergy = nameof(UseEnergy);
    public const string AlterEnergy = nameof(AlterEnergy);
    public const string GetMinimumEnergyForAction = nameof(GetMinimumEnergyForAction);
    public const string SkipTurn = nameof(SkipTurn);
    public const string GetEnergy = nameof(GetEnergy);

    //Combat
    public const string AmAttacking = nameof(AmAttacking);
    public const string PerformAttack = nameof(PerformAttack);
    public const string RollToHit = nameof(RollToHit);
    public const string TakeDamage = nameof(TakeDamage);
    public const string Died = nameof(Died);
    public const string RestoreHealth = nameof(RestoreHealth);
    public const string GetWeaponType = nameof(GetWeaponType);
    public const string GetEquipment = nameof(GetEquipment);
    public const string AddArmorValue = nameof(AddArmorValue);
    public const string Sharpness = nameof(Sharpness);
    public const string SeverBodyPart = nameof(SeverBodyPart);
    public const string GetHealth = nameof(GetHealth);

    //Stats
    public const string GetPrimaryStatType = nameof(GetPrimaryStatType);
    public const string GetStat = nameof(GetStat);

    //Body
    public const string GrowBodyPart = nameof(GrowBodyPart);
    public const string CheckEquipment = nameof(CheckEquipment);
    public const string GetCurrentEquipment = nameof(GetCurrentEquipment);
    public const string CheckItemEquiped = nameof(CheckItemEquiped);

    //Data Request
    public const string GetEntities = nameof(GetEntities);
    public const string GetEntityLocation = nameof(GetEntityLocation);
    public const string GetInteractableObjects = nameof(GetInteractableObjects);
    public const string GetObject = nameof(GetObject);
    public const string GetName = nameof(GetName);

    //UI
    public const string UIInput = nameof(UIInput);
    public const string CloseUI = nameof(CloseUI);
    public const string OpenInventoryUI = nameof(OpenInventoryUI);
    public const string OpenChestUI = nameof(OpenChestUI);
    public const string RegisterUI = nameof(RegisterUI);
    public const string UnRegisterUI = nameof(UnRegisterUI);
    public const string OpenSpellUI = nameof(OpenSpellUI);
    public const string GetPlayableCharacters = nameof(GetPlayableCharacters);
    public const string SetInfo = nameof(SetInfo);
    public const string GetInfo = nameof(GetInfo);
    public const string UpdateUI = nameof(UpdateUI);
    public const string GetContextMenuActions = nameof(GetContextMenuActions);

    //Spellcasting
    public const string GetSpells = nameof(GetSpells);
    public const string SpellSelected = nameof(SpellSelected);
    public const string OpenSpellSelector = nameof(OpenSpellSelector);
}

public static class EventParameters
{
    public const string InputDirection = nameof(InputDirection);
    public const string TakeTurn = nameof(TakeTurn);
    public const string UpdateWorldView = nameof(UpdateWorldView);
    public const string CleanupComponents = nameof(CleanupComponents);
    public const string RemainingEnergy = nameof(RemainingEnergy);
    public const string RequiredEnergy = nameof(RequiredEnergy);
    public const string EnergyRegen = nameof(EnergyRegen);
    public const string ActionTaken = nameof(ActionTaken);
    public const string RenderSprite = nameof(RenderSprite);
    public const string Renderer = nameof(Renderer);
    public const string Point = nameof(Point);
    public const string Entity = nameof(Entity);
    public const string Character = nameof(Character);
    public const string Item = nameof(Item);
    public const string Items = nameof(Items);
    public const string Target = nameof(Target);
    public const string EntityType = nameof(EntityType);
    public const string Creature = nameof(Creature);
    public const string Value = nameof(Value);
    public const string MaxValue = nameof(MaxValue);
    public const string TilePosition = nameof(TilePosition);
    public const string Attack = nameof(Attack);
    public const string Color = nameof(Color);
    public const string Damage = nameof(Damage);
    public const string DamageList = nameof(DamageList);
    public const string Healing = nameof(Healing);
    public const string DamageType = nameof(DamageType);
    public const string RollToHit = nameof(RollToHit);
    public const string WeaponType = nameof(WeaponType);
    public const string GameEvent = nameof(GameEvent);
    public const string Enemy = nameof(Enemy);
    public const string Equipment = nameof(Equipment);
    public const string Seed = nameof(Seed);
    public const string GameObject = nameof(GameObject);
    public const string TileInSight = nameof(TileInSight);
    public const string VisibleTiles = nameof(VisibleTiles);
    public const string HasBeenVisited = nameof(HasBeenVisited);
    public const string FOVRange = nameof(FOVRange);
    public const string BlocksMovement = nameof(BlocksMovement);
    public const string Weight = nameof(Weight);
    public const string StartPos = nameof(StartPos);
    public const string EndPos = nameof(EndPos);
    public const string Path = nameof(Path);
    public const string Range = nameof(Range);
    public const string AIActionList = nameof(AIActionList);
    public const string IsPartyLeader = nameof(IsPartyLeader);
    public const string StatType = nameof(StatType);
    public const string SpellList = nameof(SpellList);
    public const string Spell = nameof(Spell);
    public const string Name = nameof(Name);
    public const string Head = nameof(Head);
    public const string Torso = nameof(Torso);
    public const string LeftArm = nameof(LeftArm);
    public const string RightArm = nameof(RightArm);
    public const string Legs = nameof(Legs);
    public const string Objects = nameof(Objects);
    public const string InventoryContextActions = nameof(InventoryContextActions);
    public const string ChestContextActions = nameof(ChestContextActions);
    public const string IncludeSelf = nameof(IncludeSelf);
    public const string Level = nameof(Level);
    public const string NewGame = nameof(NewGame);
    public const string Rarity = nameof(Rarity);
}

[Serializable]
public class GameEventSerializable
{
    [SerializeField]
    public string TargetEntityId;
    [SerializeField]
    public string Id;
    [SerializeField]
    public List<string> ParameterKeys;
    [SerializeField]
    public List<string> ParameterValues;

    public GameEventSerializable(string targetId, GameEvent ge)
    {
        TargetEntityId = targetId;
        Id = ge.ID;
        ParameterKeys = new List<string>();
        ParameterValues = new List<string>();
        foreach(var key in ge.Paramters.Keys)
        {
            ParameterKeys.Add(key);
            ParameterValues.Add(ge.Paramters[key].ToString());
        }
    }

    public GameEvent CreateGameEvent()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        for(int i = 0; i < ParameterKeys.Count; i++)
            parameters.Add(ParameterKeys[i], ParameterValues[i]);
        GameEvent ge = new GameEvent(Id, parameters);
        return ge;
    }
}

public class GameEvent
{
    public bool ContinueProcessing = true;
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

    public T GetValue<T>(string parameterId)
    {
        if (Paramters.TryGetValue(parameterId, out object value))
        {
            if (value == null)
                return default(T);
            return (T)value;
        }
        return default(T);
    }
}
