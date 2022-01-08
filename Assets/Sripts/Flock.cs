using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField]
    private Agent _agentPrefab;
    List<Agent> _agents = new List<Agent>();

    [SerializeField]
    private FlockBehavior _behavior;

    [Range(10, 200)]
    private int startingCount = 250;

    [SerializeField]
    List<Transform> _flockSpawnPoints = new List<Transform>();

    [Range(1f, 100f)]
    public float _impactFactor = 10f;
    [Range(1f, 100f)]
    public float _maxSpeed = 5f;
    [Range(0.1f, 10f)]
    public float _neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float _avoidanceRadiusMultiplier = 0.5f;

    float _squareMaxSpeed;
    float _squareNeighborRadius;
    float _squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return _squareAvoidanceRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        _squareMaxSpeed = Mathf.Sqrt(_maxSpeed);
        _squareNeighborRadius = Mathf.Sqrt(_neighborRadius);
        _squareAvoidanceRadius = _squareNeighborRadius * Mathf.Sqrt(_avoidanceRadiusMultiplier);

        for(int i=0;i<startingCount;i++)
        {
            Vector3 tempPos = _flockSpawnPoints[Random.Range(0, _flockSpawnPoints.Count)].position;

            tempPos.x  += Random.Range(-0.25f * i, 0.25f * i);
            tempPos.z += Random.Range(-0.25f * i, 0.25f * i);

            Agent newAgent = Instantiate(_agentPrefab, tempPos,Quaternion.identity);
            newAgent.name = "Agent" + i;
            _agents.Add(newAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Agent agent in _agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            Vector2 move = _behavior.CalculateMove(agent, context, this);
            move *= _impactFactor;
            if(move.sqrMagnitude > _squareMaxSpeed)
            {
                move = move.normalized * _maxSpeed;
            }

            agent.Move(move);
        }
    }
    private List<Transform> GetNearbyObjects(Agent agent) //get all nearby objects
    {
        List<Transform> context = new List<Transform>();

        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, _neighborRadius);
        foreach(Collider c in contextColliders)
        {
            if(c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }

        return context;

    }
}
