using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]
public class AlignmentBehavior : FlockBehavior
{
    public override Vector2 CalculateMove(Agent agent, List<Transform> neighbours, Flock flock)
    {
        if (neighbours.Count == 0) //no changes in the agents context maintain current alignment
            return agent.transform.forward;

        //add all points together
        Vector2 alignmentMove = Vector2.zero;
        foreach (Transform neighbour in neighbours)
        {
            alignmentMove += (Vector2)neighbour.transform.forward;
        }

        alignmentMove /= neighbours.Count;

        //take offset from agents position

        return alignmentMove;
    }
}
