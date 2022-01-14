using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    public struct EmotionalSpecifiers
    {
        public float Introvertness;
        public float Hurrieness;
    }

    Collider _agentCollider;
    NavMeshAgent _agent;
    bool _movesManually;
    int _currentTravelPointIndex=0;
    float _positionReachedDelay = 0f;
    float _currentReachedDelay = 0f;

    private EmotionalSpecifiers _emotionalVariables;

    private List<Transform> _orderOfTravelPoints = new List<Transform>();
    public Collider AgentCollider { get { return _agentCollider; } }
    public float PositionReachedDelay { get { return _positionReachedDelay; } set { _positionReachedDelay = value; } }
    public EmotionalSpecifiers EmotionalVariables { get { return _emotionalVariables; } set { _emotionalVariables = value; } }
    public List<Transform> OrderOfTravelPoints { get { return _orderOfTravelPoints; } 
        set 
        { 
            _orderOfTravelPoints = value;
            SetAgentDestination(_orderOfTravelPoints[_currentTravelPointIndex].position);
        } 
    }


    private void Start()
    {
        _agentCollider = GetComponent<Collider>();
        _movesManually = true;
        _currentReachedDelay = _positionReachedDelay;
        GetComponent<NavMeshAgent>().radius = _emotionalVariables.Introvertness;
    }
    public void MoveToPoint(Vector3 destination)
    {
        _agent.destination = destination;
        _agent.isStopped = false;
        _agent.enabled = true;
    }
    public void SetAgentDestination(Vector3 destination)
    {
        GetComponent<NavMeshAgent>().SetDestination(destination);
        _movesManually = false;
    }
    public void Move(Vector2 move)
    {
        if (!_movesManually)
            return;

        _agent.Move(move * Time.deltaTime);
    }
    private void Update()
    {
        if(!_agent)
            _agent = GetComponent<NavMeshAgent>();

        if(_agent.remainingDistance <= 2f)
        {
            if (_currentReachedDelay <= 0f)
            {
                //Update agent path to travel
                Debug.LogWarning("Reached Waypoint");
                _currentTravelPointIndex++;
                _currentReachedDelay = _positionReachedDelay; //reset delay timer
                SetAgentDestination(_orderOfTravelPoints[_currentTravelPointIndex].position);
            }
            else
                _currentReachedDelay -= Time.deltaTime;
        }
    }
}
