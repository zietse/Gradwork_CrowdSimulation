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
        public float Extraversion; //low value is people that don't like to be near other people, high people is vey social people
        public float Openness; //low is people that don't like change, don't enjoy new things, high is creative and focussed on tackling new challenges
        public float Conscientiosness; //low is people that dislike structure, make messes, fails to complete tasks, high is people that finish important tasks right away and spend time preparing.
        public float Agreeableness; //low is people that are more competitive and sometimes manipulative, high in this is people that are sometimes manipulative but mostly care about others and feel empathy
        public float Neuroticism; //low is don't worry much, deal well with stress, high is experience a lot of stress, get upset easily,...
    }

    Collider _agentCollider;
    NavMeshAgent _agent;
    bool _movesManually;
    int _currentTravelPointIndex=0;
    float _positionReachedDelay = 0f;
    float _currentReachedDelay = 0f;
    float _remainingDistanceOffset = 1.0f;
    bool _isGoingToCheckout = false;

    //Timer
    private float _totalTraverseTimer = 0f;

    //emotional traits specifiers
    private EmotionalSpecifiers _emotionalVariables;

    private List<Transform> _orderOfTravelPoints = new List<Transform>();
    private List<Transform> _registerTravelPoints = new List<Transform>();
    private List<GameObject> _neighbors = new List<GameObject>();
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
    public List<Transform> RegisterTravelPoints { get { return _registerTravelPoints; } set { _registerTravelPoints = value; } }
    public List<GameObject> Neighbors { get { return _neighbors; } set { _neighbors = value; } }

    private void Start()
    {
        _agentCollider = GetComponent<Collider>();
        _movesManually = true;
        _currentReachedDelay = _positionReachedDelay;
        GetComponent<NavMeshAgent>().radius =  _emotionalVariables.Extraversion;
        GetComponent<NavMeshAgent>().obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
        //GetComponent<NavMeshAgent>().speed = 30; //adjust the speed here
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

        _totalTraverseTimer += Time.deltaTime;

        if (_agent.remainingDistance <= _remainingDistanceOffset && _isGoingToCheckout) //agent reached register point so delete itself
        {
            //print total time to console - later print this maybe to a file while doing the actual sim
            Debug.LogWarning("Total Time for agent to complete tour: " + _totalTraverseTimer.ToString("0.00") + " s");
            Destroy(this.gameObject);
        }

        if (_agent.remainingDistance <= _remainingDistanceOffset)
        {
            if (_currentReachedDelay <= 0f)
            {
                if(_currentTravelPointIndex+1 == _orderOfTravelPoints.Count-1) //customer reached the end of the list needs to go to checkout
                {
                    Debug.LogWarning("Customer has finished shoping");
                    SetAgentDestination(_registerTravelPoints[Random.Range(0, _registerTravelPoints.Count)].position); //set the agent checkout to a random position
                    _isGoingToCheckout = true;
                }
                else
                {
                    //Update agent path to travel
                    Debug.LogWarning("Reached Waypoint");
                    _currentTravelPointIndex++;
                    _currentReachedDelay = _positionReachedDelay; //reset delay timer
                    SetAgentDestination(_orderOfTravelPoints[_currentTravelPointIndex].position);
                }
            }
            else
                _currentReachedDelay -= Time.deltaTime;
        }
       
    }
}
