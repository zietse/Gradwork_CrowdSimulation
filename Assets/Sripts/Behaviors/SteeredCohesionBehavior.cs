using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behavior/SteeredCohesion")]
public class SteeredCohesionBehavior : FlockBehavior
{
    Vector2 _currentVelocity;
    [SerializeField]
    private float agentSmoothTime = 0.5f;

    public override Vector2 CalculateMove(Agent agent, List<Transform> neighbours, Flock flock)
    {
        if (neighbours.Count == 0) //no changes in the agents context
            return Vector2.zero;

        //add all points together
        Vector2 cohesionMove = Vector2.zero;
        foreach (Transform neighbour in neighbours)
        {
            cohesionMove += (Vector2)neighbour.position;
        }

        cohesionMove /= neighbours.Count;

        //take offset from agents position
        cohesionMove -= (Vector2)agent.transform.position;
        cohesionMove = Vector2.SmoothDamp(agent.transform.forward, cohesionMove, ref _currentVelocity, agentSmoothTime);


        return cohesionMove;
    }

}

