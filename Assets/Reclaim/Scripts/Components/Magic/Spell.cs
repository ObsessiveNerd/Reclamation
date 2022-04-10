using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellFailedSaveEffect
{
    None,
    DamageHalved,
    SpellStops
}

public class Spell : EntityComponent
{
    public int ManaCost;
    public SpellFailedSaveEffect SpellFailed;
    public SpellType SpellType;

    public Spell(int cost, SpellFailedSaveEffect spellFailed, SpellType spellType)
    {
        ManaCost = cost;
        SpellFailed = spellFailed;
        SpellType = spellType;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetSpells);
        RegisteredEvents.Add(GameEventId.ManaCost);
        RegisteredEvents.Add(GameEventId.GetInfo);
        RegisteredEvents.Add(GameEventId.SaveFailed);
        RegisteredEvents.Add(GameEventId.GetSpellType);
        RegisteredEvents.Add(GameEventId.CastSpellEffect);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions);
    }

    public virtual void CastSpellEffect(GameEvent ge) { }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.GetSpells)
            gameEvent.GetValue<HashSet<string>>(EventParameters.SpellList).Add(Self.ID);

        else if (gameEvent.ID == GameEventId.GetSpellType)
            gameEvent.Paramters[EventParameters.SpellType] = SpellType;

        else if (gameEvent.ID == GameEventId.ManaCost)
            gameEvent.Paramters[EventParameters.Value] = ManaCost;

        else if (gameEvent.ID == GameEventId.CastSpellEffect)
            CastSpellEffect(gameEvent);

        else if (gameEvent.ID == GameEventId.GetInfo)
        {
            Dictionary<string, string> info = gameEvent.GetValue<Dictionary<string, string>>(EventParameters.Info);
            info.Add($"{nameof(Spell)}{Guid.NewGuid()}", $"Mana cost: {ManaCost}");
        }

        else if (gameEvent.ID == GameEventId.SaveFailed)
        {
            switch (SpellFailed)
            {
                case SpellFailedSaveEffect.DamageHalved:
                    foreach (var damage in gameEvent.GetValue<List<Damage>>(EventParameters.DamageList))
                        damage.DamageAmount /= 2;
                    break;
                case SpellFailedSaveEffect.SpellStops:

                    break;
            }
        }
        //else if(gameEvent.ID == GameEventId.GetContextMenuActions)
        //{
        //    IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
        //    ContextMenuButton dropButton = new ContextMenuButton("Add to ActionBar", () =>
        //    {
        //        GameEvent addToActiveAbilities = GameEventPool.Get(GameEventId.AddToActiveAbilities)
        //                                .With(EventParameters.Entity, Self.ID);

        //        FireEvent(source, addToActiveAbilities, true).Release();
        //    });
        //    gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(dropButton);
        //}
    }
}

public class DTO_Spell : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        int manaCost = 0;
        SpellFailedSaveEffect failed = SpellFailedSaveEffect.None;
        SpellType st = SpellType.None;

        foreach (string kvp in data.Split(','))
        {
            string key = kvp.Split('=')[0];
            string value = kvp.Split('=')[1];

            if (key == "ManaCost")
            {
                manaCost = int.Parse(value);
            }
            else if (key == "SpellFailed")
            {
                failed = (SpellFailedSaveEffect)Enum.Parse(typeof(SpellFailedSaveEffect), value);
            }
            else if (key == "SpellType")
            {
                st = (SpellType)Enum.Parse(typeof(SpellType), value);
            }
        }
        Component = new Spell(manaCost, failed, st);
    }

    public string CreateSerializableData(IComponent component)
    {
        Spell s = (Spell)component;
        return $"{nameof(Spell)}: {nameof(s.ManaCost)}={s.ManaCost}, {nameof(s.SpellFailed)}={s.SpellFailed}, {nameof(s.SpellType)}={s.SpellType}";
    }
}
