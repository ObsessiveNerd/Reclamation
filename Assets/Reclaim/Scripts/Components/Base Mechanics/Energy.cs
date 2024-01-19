using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class EnergyData : EntityComponent
{
    public float EnergyReginerationDelay;
    public bool CanTakeATurn = true;

    Type MonobehaviorType = typeof(Energy);
    public override void WakeUp()
    {
        
    }

    public void TakeTurn()
    {
        if (!CanTakeATurn)
            return;

        CanTakeATurn = false;
        Entity.FireEvent(GameEventPool.Get(GameEventId.TakeTurn)).Release();

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

public class Energy : ComponentBehavior<EnergyData>
{

}
