﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFovAlgorithm
{
    List<Point> GetVisibleTiles(IEntity source, int range);
}
