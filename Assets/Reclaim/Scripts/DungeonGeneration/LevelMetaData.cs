using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelMetaData
{
    public static string MetadataPath
    {
        get
        {
            return $"LevelData";
        }
    }

    public const string StairsUp = nameof(StairsUp);
    public const string StairsDown = nameof(StairsDown);
    public const string ContainsBoss = nameof(ContainsBoss);
    public const string AverageMosterCR = nameof(AverageMosterCR);
    public const string MonsterTypes = nameof(MonsterTypes);
    public const string SpecialItems = nameof(SpecialItems);
    public const string TileType = nameof(TileType);
}
