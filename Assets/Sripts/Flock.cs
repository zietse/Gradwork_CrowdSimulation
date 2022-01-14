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
    public int startingCount = 250;

    [SerializeField]
    private float flockDensity = 0.08f;

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
            Vector2 randomPos = Random.insideUnitCircle * startingCount * flockDensity;

            Agent newAgent = Instantiate(_agentPrefab,new Vector3(randomPos.x,1.5f,randomPos.y),Quaternion.Euler(Vector3.up * Random.Range(0f,360f)),transform);
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

        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, _neighborRadius); //for calculating neighbors just use distance
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
