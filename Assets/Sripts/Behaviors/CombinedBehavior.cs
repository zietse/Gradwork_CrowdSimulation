using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behavior/Combined")]
public class CombinedBehavior : FlockBehavior
{
    public FlockBehavior[] _behaviors;
    public float[] _behaviorWeights; 
    public override Vector2 CalculateMove(Agent agent, List<Transform> neighbours, Flock flock)
    {

        //check for weight mismatch
        if (_behaviors.Length != _behaviors.Length)
        {
            Debug.LogError("Data is not synced in: " + name, this);
            return Vector2.zero;
        }

        //initalize Move
        Vector2 move = Vector2.zero;
        
        //go through behaviors
        for(int i=0; i<_behaviors.Length;i++)
        {
            Vector2 partialMove = _behaviors[i].CalculateMove(agent, neighbours, flock) * _behaviorWeights[i];

            if(partialMove != Vector2.zero)
            {
                if (partialMove.sqrMagnitude > _behaviorWeights[i] * _behaviorWeights[i])
                {
                    partialMove.Normalize();
                    partialMove *= _behaviorWeights[i];
                }

                move += partialMove;
            }
        }
        return move;
    }
}
