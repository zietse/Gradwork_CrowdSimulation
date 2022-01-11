using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Flock/Behavior/Cohesion")]
public class CohesionBehavior : FlockBehavior
{
    public override Vector2 CalculateMove(Agent agent, List<Transform> neighbours, Flock flock)
    {
        if (neighbours.Count == 0) //no changes in the agents context
            return Vector2.zero;

        //add all points together
        Vector2 cohesionMove = Vector2.zero;
        foreach(Transform neighbour in neighbours)
        {
            cohesionMove.x += neighbour.position.x;
            cohesionMove.y += neighbour.position.z;
        }

        cohesionMove /= neighbours.Count;

        //take offset from agents position
        cohesionMove.x -= agent.transform.position.x;
        cohesionMove.y -= agent.transform.position.z;

        return cohesionMove;
    }

}
