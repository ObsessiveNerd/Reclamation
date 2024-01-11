using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Energy : EntityComponent
{
    public float EnergyReginerationDelay;
    public bool CanTakeATurn = true;

    public void Start()
    {
        //RegisteredEvents.Add(GameEventId.TakeTurn, TakeTurn);
    }

    public void TakeTurn()
    {
        if (!CanTakeATurn)
            return;

        CanTakeATurn = false;
        gameObject.FireEvent(GameEventPool.Get(GameEventId.TakeTurn)).Release();

        Services.Coroutine.InvokeCoroutine(RegerateEnergyDelay());
        //GetComponent<NetworkObject>().StartCoroutine(RegerateEnergyDelay());
        //StartCoroutine(RegerateEnergyDelay());
    }

    IEnumerator RegerateEnergyDelay()
    {
        yield return new WaitForSeconds(EnergyReginerationDelay);
        CanTakeATurn = true;
    }
    
}
