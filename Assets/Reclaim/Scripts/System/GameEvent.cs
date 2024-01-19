using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Netcode;
using UnityEngine;

public static class SoundKey
{
    public const string AttackHit = nameof(AttackHit);
    public const string AttackMiss = nameof(AttackMiss);
    public const string Heal = nameof(Heal);
    public const string TakeDamage = nameof(TakeDamage);
    public const string RangedAttack = nameof(RangedAttack);
    public const string Cast = nameof(Cast);
    public const string Quaff = nameof(Quaff);
    public const string Activate = nameof(Activate);
    public const string Died = nameof(Died);
}

public enum GameEventId
{
    None = 0,

    CalculateTileFlags,

    //AI
    GetActionToTake,
    GetCombatRating,
    GetPackInformation,
    BreakRank,


    FOVRecalculated,
    BeforeFOVRecalculated,
    SetVisibility,
    SetHasBeenVisited,
    VisibilityUpdated,
    CheckVisibility,
    BlocksMovement,
    BlocksVision,
    InitFOV,
    GetVisibleTiles,
    EntityVisibilityState,
    IsInFOV,

    GetEntityType,

    GetFaction,
    SetFaction,

    OpenInventory,
    CloseInventory,
    AddToInventory,
    RemoveFromInventory,
    EmptyBag,
    ItemEquipped,
    ItemUnequipped,
    OnBeforeEquip,
    OnAfterEquip,
    GetMultiBodyPartUse,
    Equip,
    TryEquip,
    Unequip,
    GetWeapon,
    GetIcon,
    GetCurrentInventory,


    Pickup,
    Drop,
    GetValue,
    AddItem,
    RemoveItem,
    AddItems,
    GetRarity,
    GetItems,
    DropItemsOnMap,
    AddItemsToInventory,
    GetValidAppendage,
    GetBodyPartType,
    PromptToGiveItem,


    RevealAllTiles,

    //World
    StartWorld,
    UpdateWorldView,
    Spawn,
    Despawn,
    SelectTile,
    SelectNewTileInDirection,
    GetEntityOnTile,
    ShowInfo,
    AddComponentToTile,
    ProgressTime,
    ProgressTimeUntilIdHasTakenTurn,
    PauseTime,
    UnPauseTime,
    RegisterEntity,
    DestroyEntity,
    GetEntity,
    CalculatePath,
    IsValidDungeonTile,
    GetClosestEnemy,
    GetActivePlayerId,
    GetCurrentLevel,
    GameWin,
    GameFailure,
    CleanFoVData,
    GameObject,
    Tag,


    GetRandomValidPoint,
    GetRandomValidPointInRange,
    AddValidPoints,
    MoveUp,
    MoveDown,

    Mouse1,
    Mouse2,

    //Moving
    EntityOvertaking,
    BeforeMoving,
    ExecuteMove,
    AfterMoving,
    MoveEntity,
    SetEntityPosition,
    SetPoint,
    GetPoint,

    Interact,
    ForceInteract,
    InteractInDirection,
    InteractWithTarget,
    HostileInteraction,
    PrimaryInteraction,

    TakeTurn,

    StopMovement,

    //Time progression
    StartTurn,
    CharacterRotated,
    UpdateEntity,
    ActivateObject,
    DeactivateObject,
    EndTurn,
    RegisterWithTimeSystem,
    RegisterPlayableCharacter,
    UnRegisterPlayer,


    UpdateRenderer,
    GetRenderSprite,
    GetSprite,
    SetSprite,
    AlterSprite,

    UpdateTile,
    EndSelection,
    ShowTileInfo,
    DestroyObject,
    CleanTile,
    PathfindingData,
    IsValidTile,
    GetValueOnTile,
    SaveLevel,
    SaveVisibilityData,
    LoadLevel,
    SerializeTile,
    GetVisibilityData,


    SetCameraPosition,
    UpdateCamera,

    RotateActiveCharacter,
    IsPlayableCharacter,
    SetActiveCharacter,
    ConvertToPlayableCharacter,

    MoveKeyPressed,
    PromptForInput,

    HasEnoughEnergyToTakeATurn,
    UseEnergy,
    AlterEnergy,
    GetMinimumEnergyForAction,
    SkipTurn,
    GetEnergy,

    //Combat
    PerformAttack,
    PerformMeleeAttack,
    PerformRangedAttack,
    ActiveWeapon1,
    ActiveWeapon2,
    RollToHit,
    ApplyDamage,
    DamageTaken,
    DealtDamage,
    Died,
    RestoreHealth,
    RegenHealth,
    Rest,
    AddMaxHealth,
    RemoveMaxHealth,
    AddMaxMana,
    RemoveMaxMana,
    RestoreMana,
    DepleteMana,
    ApplyEffectToTarget,
    GetWeaponType,
    GetSpellType,
    GetWeaponTypes,
    GetEquipment,
    AddArmorValue,
    Sharpness,
    SeverBodyPart,
    GetHealth,
    GetMana,
    ManaCost,
    Quaff,
    FireProjectile,
    SavingThrow,
    SaveFailed,
    CastSpellEffect,
    GetAmmo,
    GetResistances,
    GetImmunity,
    GetArmor,

    //Stats
    GetPrimaryStatType,
    GetStat,
    GetStatRaw,
    GainExperience,
    GetExperience,
    LevelUp,
    GetLevel,
    BoostStat,
    StatBoosted,
    GetAttributePoints,

    //Body
    GrowBodyPart,
    CheckEquipment,
    GetCurrentEquipment,
    CheckItemEquiped,

    //Data Request
    GetEntities,
    GetEntityLocation,
    GetInteractableObjects,
    GetObject,
    GetName,
    GetSpawnRestrictions,

    //UI
    UIInput,
    CloseUI,
    OpenInventoryUI,
    OpenUI,
    OpenChestUI,
    RegisterUI,
    UnRegisterUI,
    OpenSpellUI,
    GetPlayableCharacters,
    SetInfo,
    SetName,
    GetInfo,
    GetPortrait,
    UpdateUI,
    GetContextMenuActions,
    EntityTookDamage,
    EntityHealedDamage,
    EntityUsedMana,
    EntityRegainedMana,
    OpenSpellExaminationUI,

    //Spellcasting
    GetSpells,
    SpellSelected,
    OpenSpellSelector,
    AffectArea,
    GetSpellSaveDC,
    GetActiveAbilities,
    AddToActiveAbilities,
    RemoveFromActiveAbilities,

    //Enchanment
    EnchantItem,
    GetEnchantments,

    //Party
    LookingForGroup,
    SetLeader,
    MakePartyLeader,
    RemoveFromParty,

    //Sound
    Playsound,

}

[Serializable]
public enum EventParameter
{
    InputDirection,
    Flag,
    Faction,
    TakeTurn,
    CanMove,
    UpdateWorldView,
    CleanupComponents,
    RemainingEnergy,
    RequiredEnergy,
    EnergyRegen,
    ActionTaken,
    Abilities,
    RenderSprite,
    Restrictions,
    Resistances,
    Immunity,
    Renderer,
    Point,
    Entity,
    Character,
    Item,
    Items,
    Target,
    EntityType,
    DesiredBodyPartTypes,
    Creature,
    Value,
    MaxValue,
    TilePosition,
    Attack,
    Color,
    Damage,
    DamageAmount,
    SoundSource,
    Source,
    DamageSource,
    DamageList,
    Healing,
    Mana,
    Melee,
    DamageType,
    RollToHit,
    Crit,
    Cost,
    WeaponType,
    SpellType,
    WeaponTypeList,
    Weapon,
    GameEvent,
    Enemy,
    Equipment,
    Seed,
    GameObject,
    TileInSight,
    VisibleTiles,
    HasBeenVisited,
    FOVRange,
    BlocksMovementFlags,
    BlocksVision,
    Weight,
    StartPos,
    EndPos,
    Path,
    Range,
    AIActionList,
    IsPartyLeader,
    StatType,
    SpellList,
    Spell,
    SpellContinues,
    Name,
    Head,
    Torso,
    LeftArm,
    RightArm,
    Armor,
    Ammo,

    Legs,
    Arms,
    Back,
    Neck,
    Finger,
    Objects,
    InventoryContextActions,
    ChestContextActions,
    IncludeSelf,
    Level,
    NewGame,
    Rarity,
    Info,
    Exp,
    StatBoostAmount,
    Stats,
    AttributePoints,
    Key,
    Owner,
    BodyParts,
    BodyPart,
    Effect,
    Enchantments,
}

[Serializable]
public struct GameEventSerializable
{
    [SerializeField]
    public GameEventId Id;
    [SerializeField]
    public List<EventParameter> ParameterKeys;
    [SerializeField]
    public List<string> ParameterValues;

    public GameEventSerializable(/*string targetId, */GameEvent ge)
    {
        //TargetEntityId = targetId;
        Id = ge.ID;
        ParameterKeys = new List<EventParameter>();
        ParameterValues = new List<string>();
        foreach (var key in ge.Paramters.Keys)
        {
            ParameterKeys.Add(key);
            ParameterValues.Add(ge.Paramters[key].ToString());
        }
    }

    public GameEvent CreateGameEvent()
    {
        Dictionary<EventParameter, object> parameters = new Dictionary<EventParameter, object>();
        for (int i = 0; i < ParameterKeys.Count; i++)
            parameters.Add(ParameterKeys[i], ParameterValues[i]);
        GameEvent ge = GameEventPool.Get(Id)
                        .With(parameters);
        return ge;
    }
}

public class GameEvent
{
    public bool Locked = false;
    //public bool ContinueProcessing;
    public GameEventId ID { get { return m_ID; } }
    GameEventId m_ID { get; set; }

    public Dictionary<EventParameter, object> Paramters { get { return m_Parameters; } }
    Dictionary<EventParameter, object> m_Parameters = new Dictionary<EventParameter, object>();

    GameEventSerializable m_GameEventSerializable = default;
    public GameEvent(GameEventId id)
    {
        m_ID = id;
        //ContinueProcessing = true;
    }

    public GameEvent SetId(GameEventId id)
    {
        if (m_ID != GameEventId.None)
            Debug.LogError($"GameEvent still has id: {m_ID} but you're attempting to change it into {id}");
        m_ID = id;
        return this;
    }

    public GameEvent With(EventParameter id, object value)
    {
        if (value is Enum)
            value = (int)value;

        if (m_Parameters.ContainsKey(id))
            m_Parameters[id] = value;
        else
            m_Parameters.Add(id, value);
        return this;
    }

    public GameEvent With(Dictionary<EventParameter, object> values)
    {
        foreach (var key in values.Keys)
            With(key, values[key]);
        return this;
    }

    public bool HasParameter(EventParameter parameterId)
    {
        return m_Parameters.ContainsKey(parameterId);
    }

    public T GetValue<T>(EventParameter parameterId)
    {
        if (m_Parameters.TryGetValue(parameterId, out object value))
        {
            if (value == null)
                return default(T);
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), value.ToString());
            if (value is string && typeof(T) != typeof(string))
                return JsonUtility.FromJson<T>(value as string);
            return (T)value;
        }
        return default(T);
    }

    public void SetValue<T>(EventParameter parameterId, T newValue)
    {
        if (m_Parameters.TryGetValue(parameterId, out object value))
            m_Parameters[parameterId] = newValue;
    }

    public void Release()
    {
        if (Locked)
            throw new Exception("Cannot release a locked event.");
        else
            //Debug.Log($"Game event {m_ID} released");
            GameEventPool.Release(this);
    }

    public void Clean()
    {
        //ContinueProcessing = true;
        m_ID = GameEventId.None;
        m_Parameters.Clear();
        m_GameEventSerializable = default;
    }
}
