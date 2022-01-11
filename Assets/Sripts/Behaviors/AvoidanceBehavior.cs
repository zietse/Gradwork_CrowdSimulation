using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]
public class AvoidanceBehavior : FlockBehavior
{
    public override Vector2 CalculateMove(Agent agent, List<Transform> neighbours, Flock flock)
    {
        if (neighbours.Count == 0) //no changes in the agents context
            return Vector2.zero;

        //add all points together
        Vector2 avoidanceMove = Vector2.zero;

        int nAvoid = 0;
        foreach (Transform neighbour in neighbours)
        {
            if(Vector2.SqrMagnitude(neighbour.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                avoidanceMove.x += (agent.transform.position.x - neighbour.position.x);
                avoidanceMove.y += (agent.transform.position.z - neighbour.position.z);
            }
        }

        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        return avoidanceMove;
    }
}
