using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapNode
{
    int x { get; set; }
    int y { get; set; }
}

public interface IPathfindingAlgorithm
{
    List<IMapNode> CalculatePath(IMapNode startingPoint, IMapNode targetPoint);
    void Clear();
}
