using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AIAction
{
    public int Priority;
    public Func<MoveDirection> ActionToTake;
}

public class AIActionPriorityComparer : IComparer<AIAction>
{
    public int Compare(AIAction x, AIAction y)
    {
        if (x.Priority < y.Priority)
            return -1;
        if (x.Priority == y.Priority)
            return 0;
        return 1;
    }
}

public class AIController : InputControllerBase
{
    Energy m_Energy;
    Aggression m_Aggression;
    EquipmentBehavior m_Equipment;

    protected override void Start()
    {
        base.Start();
        m_Energy = GetComponent<Energy>();
        m_Aggression = GetComponent<Aggression>();
        m_Equipment = GetComponentInChildren<EquipmentBehavior>();
    }

    //temp
    bool canAttack = true;
    void Update()
    {
        if (!IsOwner)
            return;
        if (m_Equipment.MainHand == null)
            return;

        var weaponRange = m_Equipment.MainHand.GetComponent<MeleeWeapon>().component.Range;
        var target = m_Aggression.GetTarget();

        if (target != null)
        {
            if (Vector2.Distance(transform.position, target.transform.position) <= weaponRange && canAttack)
            {
                MoveServerRpc(MoveDirection.None, 0f, 0f);   
                m_Equipment.Attack();
                StartCoroutine(RegenAttack());
            }
            else
            { 
                var direction = (target.transform.position - transform.position).normalized;
                m_Equipment.UpdatePositionServerRpc(target.transform.position, transform.position);
                MoveServerRpc(MoveDirection.None, direction.x, direction.y);   
            }
        }

        //MoveDirection desiredDirection = MoveDirection.None;

        //GameEvent getActionEventBuilder = GameEventPool.Get(GameEventId.GetActionToTake)
        //                                                .With(EventParameter.AIActionList, new PriorityQueue<AIAction>(new AIActionPriorityComparer()));

        //PriorityQueue<AIAction> actions = gameObject.FireEvent(getActionEventBuilder).GetValue<PriorityQueue<AIAction>>(EventParameter.AIActionList);
        //if (actions.Count > 0)
        //    desiredDirection = actions[0].ActionToTake();

        //getActionEventBuilder.Release();

        //if(desiredDirection != MoveDirection.None && m_Energy.component.CanTakeATurn)
        //{
        //    Vector2 direction = Services.Map.GetVector(desiredDirection);
        //    MoveServerRpc(desiredDirection, direction.x, direction.y);
        //    m_Energy.component.TakeTurn();
        //}
    }

    IEnumerator RegenAttack()
    {
        canAttack = false;
        yield return new WaitForSeconds(1f);
        canAttack = true;
    }
}