using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class Agent : MonoBehaviour
{
    Collider _agentCollider;
    public Collider AgentCollider { get { return _agentCollider; } }

    private void Start()
    {
        _agentCollider = GetComponent<Collider>();
    }
    
    public void Move(Vector2 velocity)
    {
        transform.forward = velocity;

        //this still needs to be replaced with the implementation of the navmeshAgent !!
        transform.rotation = Quaternion.identity;
        transform.Translate(new Vector3(velocity.x,0,velocity.y) * Time.deltaTime);
    }

}
