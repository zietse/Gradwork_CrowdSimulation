using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Crowd : MonoBehaviour
{
    [SerializeField]
    Transform[] _spawnPoints;
    [SerializeField]
    GameObject _agentPrefab;
    [SerializeField]
    int _amountOfAgentsOnPoint;
    [SerializeField]
    Transform[] _targetPositions;

    private List<GameObject> _crowd = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform spawnpoint in _spawnPoints)
        {
            for(int i=0;i<_amountOfAgentsOnPoint;i++)
            { 
               
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
   
    }
}
