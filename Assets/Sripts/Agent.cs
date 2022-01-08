using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NavMeshAgent))]
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
        var navmeshAgent = GetComponent<NavMeshAgent>();
        //navmeshAgent.velocity = velocity;
        navmeshAgent.velocity = velocity;
    }

}
