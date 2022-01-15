using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crowd : MonoBehaviour
{
    [SerializeField]
    Transform[] _spawnPoints;
    [SerializeField]
    GameObject _agentPrefab;
    [SerializeField]
    int _amountOfAgentsOnPoint;


    //Timers
    [Range(0.0f, 10.0f)]
    public float _newAgentSpawnInterval;
    [SerializeField]
    public float _minWaitInterval;
    public float _maxWaitInterval;
    private float _currentSpawnInterval;

    //Emotional Variables
    [SerializeField]
    private float _minIntrovertnessFactor;
    [SerializeField]
    private float _maxIntrovertnessFactor;

 
    private List<Transform> _availableWaypoints = new List<Transform>();
    private List<Transform> _availableRegisters = new List<Transform>();
    private List<GameObject> _crowd = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //set all timer variablese
        _currentSpawnInterval = _newAgentSpawnInterval;

        //first scan for all waypoints in the scene
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        //scan for all available registers
        GameObject[] registers = GameObject.FindGameObjectsWithTag("RegisterWaypoint");

        foreach (GameObject waypoint in waypoints) //fill it with all the waypoints found in the scene
            _availableWaypoints.Add(waypoint.transform);

        foreach (GameObject register in registers) //fill list with all available registers
            _availableRegisters.Add(register.transform);

    }
    void SpawnAgent()
    {
        GameObject tempAgent = Instantiate(_agentPrefab, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
        tempAgent.GetComponent<Agent>().OrderOfTravelPoints = GetRandomTraverseOrder();
        tempAgent.GetComponent<Agent>().RegisterTravelPoints = _availableRegisters;
        tempAgent.GetComponent<Agent>().PositionReachedDelay = Random.Range(_minWaitInterval, _maxWaitInterval); //randomize how long agent needs to wait after reaching a waypoint
        
        Agent.EmotionalSpecifiers tempSpecifiers;
        tempSpecifiers.Introvertness = Random.Range(_minIntrovertnessFactor, _maxIntrovertnessFactor);
        tempSpecifiers.Hurrieness = Random.Range(_minIntrovertnessFactor, _maxIntrovertnessFactor);
        tempAgent.GetComponent<Agent>().EmotionalVariables = tempSpecifiers; //randomize the introvertness factor for now
        
        _crowd.Add(tempAgent);
    }
    List<Transform> GetRandomTraverseOrder()
    {
        //Randomize the entire traverse list
        List<Transform> returnList = _availableWaypoints;
        for(int i=0;i<_availableWaypoints.Count*10;i++)
        {
            int rand1 = Random.Range(0, _availableWaypoints.Count);
            int rand2 = Random.Range(0, _availableWaypoints.Count);
            Transform tempTransform = returnList[rand1];
            returnList[rand1] = returnList[rand2];
            returnList[rand2] = tempTransform;
        }

        return returnList;
    }
    // Update is called once per frame
    void Update()
    {
        _currentSpawnInterval -= Time.deltaTime;
        if(_currentSpawnInterval <= 0f)
        {
            SpawnAgent();
            _currentSpawnInterval = _newAgentSpawnInterval;
        }
    }
}
