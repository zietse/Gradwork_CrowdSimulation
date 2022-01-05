using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    private Transform _targetPosition;
    public Transform TargetPosition
    {
        get { return _targetPosition; }
        set { _targetPosition = value; }
    }
    NavMeshAgent agent;


    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(_targetPosition.position);
    }
}
