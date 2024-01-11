using System.Collections.Generic;

public interface IPathfindingAlgorithm
{
    List<Point> CalculatePath(Point startingPoint, Point targetPoint);
}
