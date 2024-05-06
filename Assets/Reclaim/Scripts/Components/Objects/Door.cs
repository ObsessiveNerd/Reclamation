using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorData : EntityComponent
{
    public bool Locked;
    public bool Open;

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.PrimaryInteraction, PrimaryInteraction);
        RegisteredEvents.Add(GameEventId.SecondaryInteraction, SecondaryInteraction);
    }

    void PrimaryInteraction(GameEvent gameEvent)
    {
        gameEvent.GetValue<List<Action>>(EventParameter.ActionList).Add(() => Open = !Open);
    }

    void SecondaryInteraction(GameEvent gameEvent)
    {

    }
}

public class Door : ComponentBehavior<DoorData>
{
    public Sprite OpenTexture;
    public Sprite ClosedTexture;

    BoxCollider2D m_BC;
    SpriteRenderer m_SR;
    DoorData m_Data;
    bool m_Open;

    // Start is called before the first frame update
    void Start()
    {
        m_BC = GetComponent<BoxCollider2D>();
        m_SR = GetComponent<SpriteRenderer>();
        m_Data = GetComponent() as DoorData;
        CheckDoorChange();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDoorChange();
    }

    void CheckDoorChange()
    {
        if (m_Data.Open != m_Open)
        {
            m_Open = m_Data.Open;
            if (m_Data.Open)
                m_SR.sprite = OpenTexture;
            else
                m_SR.sprite = ClosedTexture;

            m_BC.isTrigger = m_Data.Open;
        }
    }
}
