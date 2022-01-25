using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Traits))]
public class Agent : MonoBehaviour
{
   

    Collider _agentCollider;
    NavMeshAgent _agent;
    NavMeshObstacle _agentObstacle;
    bool _movesManually;
    int _currentTravelPointIndex=0;
    float _positionReachedDelay = 0f;
    float _currentReachedDelay = 0f;
    float _remainingDistanceOffset = 1.0f;
    bool _isGoingToCheckout = false;
    private string _dataFileName;

    //Animator
    Animator _animController;

    //Timer
    private float _totalTraverseTimer = 0f;

    Traits _emotionalTraits;

    private List<Transform> _orderOfTravelPoints = new List<Transform>();
    private List<Transform> _registerTravelPoints = new List<Transform>();
    private List<GameObject> _neighbors = new List<GameObject>();
    public Collider AgentCollider { get { return _agentCollider; } }
    public string DataFileName { get { return _dataFileName; } set { _dataFileName = value; } }
    public float PositionReachedDelay { get { return _positionReachedDelay; } set { _positionReachedDelay = value; } }
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
        _animController = GetComponent<Animator>();
        _emotionalTraits = GetComponent<Traits>();
        _movesManually = true;
        _currentReachedDelay = _positionReachedDelay;

        //Set all the direct personal trait impacting factors for the agents
        GetComponent<NavMeshAgent>().radius = 1f * _emotionalTraits.Extraversion;
        GetComponent<NavMeshAgent>().speed = 5f * _emotionalTraits.Neuroticism; //adjust the speed here : map around normal speed of 5
        
        
        GetComponent<NavMeshAgent>().obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
        _agentObstacle = GetComponent<NavMeshObstacle>();

        //trigger walking animation
        _animController.SetBool("IsMoving", true);

        _agentObstacle.enabled = false;
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
    public void SwitchAgentType(int switchKey) //switch the agent from a NavmeshAgent to a Navmesh obstacle
    {
        switch(switchKey)
        {
            case 0: //key 0 is from agent to obstacle
                _agent.isStopped = true;
                _agent.enabled = false;
                _agentObstacle.enabled = true;
                break;
            case 1:
                _agent.isStopped = false;
                _agent.enabled = true;
                _agentObstacle.enabled = false;
                break;
        }
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

            //Print Agent data to file
            WriteDataToFile(_dataFileName);

            Destroy(this.gameObject);
            return;
        }

        if (_agent.velocity.magnitude > 0f) //agent is moving
            _animController.SetBool("IsMoving", true);
        else
            _animController.SetBool("IsMoving", false);

        if (_agent.remainingDistance <= _remainingDistanceOffset)
        {
            if (_currentReachedDelay <= 0f)
            {

                if (_currentTravelPointIndex+1 == _orderOfTravelPoints.Count-1) //customer reached the end of the list needs to go to checkout
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
            {
                _currentReachedDelay -= Time.deltaTime;
            }
        }
    }

    void WriteDataToFile(string fileName)
    { 
        string path = Application.dataPath + "/Data/" + fileName + ".csv";

        TextWriter tw = new StreamWriter(path, true);
        tw.WriteLine(_emotionalTraits.Extraversion.ToString() + ";" + _emotionalTraits.Openess + ";"
        + _emotionalTraits.Conscientiosness + ";" + _emotionalTraits.Agreeableness + ";" + _emotionalTraits.Neuroticism + ";" + _totalTraverseTimer + ";" + (_orderOfTravelPoints.Count-1));
        tw.Close();

    }
}
