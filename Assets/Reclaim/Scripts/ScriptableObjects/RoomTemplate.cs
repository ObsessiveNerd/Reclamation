using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnFrequency
{
    One,
    Few,
    Many
}

[Serializable]
public class SpawnFrequencyData
{
    public string BlueprintName;
    public SpawnFrequency Frequency;
}

[CreateAssetMenu(fileName = "RoomTemplete.asset", menuName = "Reclamation/Room Templete")]
public class RoomTemplate : ScriptableObject
{
    public List<SpawnFrequencyData> BlueprintNames = new List<SpawnFrequencyData>();

    public List<string> GetBlueprintsToSpawn(int maxItems)
    {
        List<string> retValue = new List<string>();
        int fewAmount = (int)(maxItems * 0.1f);
        int manyAmount = (int)(maxItems * 0.3f);

        foreach(var sfd in BlueprintNames)
        {
            if (retValue.Count > maxItems)
                break;

            int amountAdded = 0;
            retValue.Add(sfd.BlueprintName);
            if (sfd.Frequency == SpawnFrequency.One)
                continue;
            amountAdded++;
            if(sfd.Frequency == SpawnFrequency.Few)
            {
                while (amountAdded < fewAmount)
                { 
                    retValue.Add(sfd.BlueprintName);
                    amountAdded++;
                }
                continue;
            }
            if(sfd.Frequency == SpawnFrequency.Many)
            {
                while (amountAdded < manyAmount)
                { 
                    retValue.Add(sfd.BlueprintName);
                    amountAdded++;
                }
                continue;
            }
        }
        return retValue;
    }
}
