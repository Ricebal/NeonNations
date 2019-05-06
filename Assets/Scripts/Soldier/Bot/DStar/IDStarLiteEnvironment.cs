using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDStarLiteEnvironment
{
    void MoveTo(Coordinates s);
    LinkedList<Coordinates> GetObstaclesInVision();
    Coordinates GetPosition();
}