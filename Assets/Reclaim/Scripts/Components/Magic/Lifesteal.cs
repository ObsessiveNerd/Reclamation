using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifesteal : EntityComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.DealtDamage);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.DealtDamage)
        {
            IEntity damageSource = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.DamageSource));
            Damage damage = gameEvent.GetValue<Damage>(EventParameters.Damage);

            GameEvent healForDamage = GameEventPool.Get(GameEventId.RestoreHealth)
                                        .With(EventParameters.Healing, damage.DamageAmount);

            damageSource.FireEvent(healForDamage).Release();
        }
    }
}

public class DTO_Lifesteal : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Lifesteal();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Lifesteal);
    }
}
