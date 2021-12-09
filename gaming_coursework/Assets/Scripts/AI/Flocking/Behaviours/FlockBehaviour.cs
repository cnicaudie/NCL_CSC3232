using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehaviour : ScriptableObject
{
    public abstract Vector3 CalculateMove(FlockAgent agent, List<Transform> neighbours, Flock flock);
}