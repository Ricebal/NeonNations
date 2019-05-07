using System.Collections.Generic;

public interface IDStarLiteEnvironment
{
    void MoveTo(Coordinates s);
    LinkedList<Coordinates> GetObstaclesInVision();
    Coordinates GetPosition();
}