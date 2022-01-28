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
    bool _movesManually;
    int _currentTravelPointIndex=0;
    float _positionReachedDelay = 0f;
    bool _isAgentOnWaypoint = false;
    float _currentReachedDelay = 0f;
    float _remainingDistanceOffset = 1.0f;
    private bool _isGoingToCheckout = false;
    private string _dataFileName;

    //Agreeableness movement speed increase
    [SerializeField]
    float _movementSpeedIncrease;

    //Animator
    Animator _animController;

    //Timer
    private float _totalTraverseTimer = 0f;

    Traits _emotionalTraits;

    private List<Transform> _orderOfTravelPoints = new List<Transform>();
    private List<Transform> _registerTravelPoints = new List<Transform>();
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

    public bool IsGoingToCheckout { get { return _isGoingToCheckout; } }
    public bool IsAgentOnWaypoint { get { return _isAgentOnWaypoint; } }
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

        //trigger walking animation
        _animController.SetBool("IsMoving", true);

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
    public void SwitchAgentActive(int switchKey) //switch the agent from a NavmeshAgent to a Navmesh obstacle
    {
        switch(switchKey)
        {
            case 0: //key 0 is agent is pauzed
                _agent.isStopped = true;
                break;
            case 1: //agent continues
                _agent.isStopped = false;
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
                    _isAgentOnWaypoint = false;
                    
                    _emotionalTraits.TriggerOpenessCalculation(_currentTravelPointIndex); //trigger openess 
                    
                    SetAgentDestination(_orderOfTravelPoints[_currentTravelPointIndex].position);
                }
            }
            else
            {
                if (_isAgentOnWaypoint == false)
                    _isAgentOnWaypoint = true;
                
                _currentReachedDelay -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other) //use OnTriggerEnter for now to detect neighbours for Agreeableness calculation, this is not good for performance !
    {
        //if (other.transform.tag == "Agent")
        //{
        //    if(other.GetComponent<Agent>()._emotionalTraits.Agreeableness <= _emotionalTraits.Agreeableness) //other agent doesn't care so tries to move faster to surpass the other agent
        //    {
        //        other.GetComponent<NavMeshAgent>().speed += _movementSpeedIncrease / 2;
        //        transform.GetComponent<NavMeshAgent>().speed -= _movementSpeedIncrease / 4;
        //    }
        //    else //this agent cares less than the agent he crossed
        //    {
        //        transform.GetComponent<NavMeshAgent>().speed += _movementSpeedIncrease / 4;
        //        other.GetComponent<NavMeshAgent>().speed -= _movementSpeedIncrease / 2;
        //    }
        //}
    }
    void WriteDataToFile(string fileName)
    { 
        string path = Application.dataPath + "/Data/" + fileName + ".csv";

        TextWriter tw = new StreamWriter(path, true);
        tw.WriteLine(_emotionalTraits.Extraversion.ToString() + ";" + _emotionalTraits.Openness + ";"
        + _emotionalTraits.Conscientiousness + ";" + _emotionalTraits.Agreeableness + ";" + _emotionalTraits.Neuroticism + ";" + _totalTraverseTimer.ToString("0.00") + ";" + (_orderOfTravelPoints.Count-1));
        tw.Close();

    }
}
