using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class EnergyData : ComponentData
{
    public float EnergyReginerationDelay;
    public bool CanTakeATurn = true;
}

public class Energy : EntityComponent
{
    public EnergyData Data = new EnergyData();

    public override void WakeUp(IComponentData data = null)
    {
        if(data != null)
            Data = data as EnergyData;
    }

    public override IComponentData GetData()
    {
        return Data;
    }

    public void TakeTurn()
    {
        if (!Data.CanTakeATurn)
            return;

        Data.CanTakeATurn = false;
        gameObject.FireEvent(GameEventPool.Get(GameEventId.TakeTurn)).Release();

        Services.Coroutine.InvokeCoroutine(RegerateEnergyDelay());
        //GetComponent<NetworkObject>().StartCoroutine(RegerateEnergyDelay());
        //StartCoroutine(RegerateEnergyDelay());
    }

    IEnumerator RegerateEnergyDelay()
    {
        yield return new WaitForSeconds(Data.EnergyReginerationDelay);
        Data.CanTakeATurn = true;
    }
    
}
