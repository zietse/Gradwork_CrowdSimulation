using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;
public class Crowd : MonoBehaviour
{
    [SerializeField]
    Transform[] _spawnPoints;
    [SerializeField]
    GameObject[] _agentPrefabs;
    [SerializeField]
    int _amountOfAgentsOnPoint;
    [SerializeField]
    private bool _useWaypointShifting;


    //Timers
    [Range(0.0f, 10.0f)]
    public float _newAgentSpawnInterval;


    private float _currentSpawnInterval;


    [SerializeField]
    private int _traversePointCap;

    [SerializeField]
    private float _neighbourDetectionRadius;

    [SerializeField]
    private string[] _dataFileNames;

 
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

        //data gets written in order of each agent to compare the data - if I want everything in the same file I type the same filename twice
        //write title of CSVs
        for(int i=0;i<_dataFileNames.Length;i++)
        {
            string path = Application.dataPath + "/Data/" + _dataFileNames[i] + ".csv";

            TextWriter tw = new StreamWriter(path, false);
            tw.WriteLine("Extraversion; Openness; Conscientiousness; Agreeableness; Neuroticism; Traverse Time; #waypoints");
            tw.Close();
        }

        //assign random order to the waypoints at the beginning of the simulation
        _availableWaypoints = GetRandomTraverseOrder();
    }
    void SpawnAgent()
    {
        int randAgentPrefabIndex = Random.Range(0, _agentPrefabs.Length);

        GameObject tempAgent = Instantiate(_agentPrefabs[randAgentPrefabIndex], _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);

        if(_useWaypointShifting)
            tempAgent.GetComponent<Agent>().OrderOfTravelPoints = GetShiftedTraverseList();  //shift _available waypoint list before assigning
        else
            tempAgent.GetComponent<Agent>().OrderOfTravelPoints = GetRandomTraverseOrder(); //this is fully randomized to create a chaotic simulation but for statistics I need better consistent data
        
        tempAgent.GetComponent<Agent>().RegisterTravelPoints = _availableRegisters;
        tempAgent.GetComponent<Agent>().DataFileName = _dataFileNames[randAgentPrefabIndex];

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

        if(returnList.Count > _traversePointCap) //cap the amount of waypoints the agent needs to traverse so we can control the test data
        {
            returnList = returnList.GetRange(0,_traversePointCap+1);
        }

        return returnList;
    }
    List<Transform> GetShiftedTraverseList()
    {
        List<Transform> tempList = _availableWaypoints;
        //for every agent spawned shift the traverse list by 1 to the left so the results stay as close as possible to each other
        //every second waypoint becomes the first in the next agent's list
        for(int i =_availableWaypoints.Count; i<0 ;--i)
        {
            if (i == 0) //we are at the end swap first with last
            {
                tempList[0] = _availableWaypoints[_availableWaypoints.Count - 1];
                tempList[tempList.Count - 1] = _availableWaypoints[0];
            }
            else
            {
                tempList[i] = _availableWaypoints[i - 1];
                tempList[i - 1] = _availableWaypoints[i];
            }
        }

        return tempList;
    }
    // Update is called once per frame
    void Update()
    {
        CleanupCrowd();

        _currentSpawnInterval -= Time.deltaTime;
        if(_currentSpawnInterval <= 0f)
        {
            SpawnAgent();
            _currentSpawnInterval = _newAgentSpawnInterval;
        }

        //UpdateAgentsNeighbors();
    }

    void UpdateAgentsNeighbors()
    {
        //loop over all agents to check for neighbours
        for (int i = 0; i < _crowd.Count; i++)
        {
            for (int j = 0; j < _crowd.Count; j++)
            {
                if (Vector2.Distance(_crowd[i].transform.position, _crowd[j].transform.position) < _neighbourDetectionRadius) //agent is in range add to neighbors
                {
                    //Debug.LogWarning("Added neighbor for agent - " + i);
                }
                else
                {
                    if(_crowd.Contains(_crowd[j])) //this part f the code might not be that performant ??
                    {
                        //Debug.LogWarning("Removed neighbor for agent - " + i);
                    }
                }
            }

        }
    }

    void CleanupCrowd()
    {
        for(int i=0;i<_crowd.Count;i++)
        {
            if (_crowd[i] == null)
                _crowd.RemoveAt(i);
        }
    }

}
