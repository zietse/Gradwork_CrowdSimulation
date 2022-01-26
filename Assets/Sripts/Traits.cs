using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Extraversion = low value is people that don't like to be near other people, high value is vey social people
//Openness = low is people that don't like change, don't enjoy new things, high is creative and focussed on tackling new challenges
// Conscientiosness = low is people that dislike structure, make messes, fails to complete tasks, high is people that finish important tasks right away and spend time preparing.
// Agreeableness = low is people that are more competitive and sometimes manipulative, high in this is people that are sometimes manipulative but mostly care about others and feel empathy
// Neuroticism = low is don't worry much, deal well with stress, high is experience a lot of stress, get upset easily,...

public class Traits : MonoBehaviour
{
    //emotional traits specifiers
    /*Done*/ [Range(0.1f,1f)] public float Extraversion; //Linked with evade radius, how higher the extraversion is thus the lower the radius is and people go closer to each other.
    /*Done*/[Range(0f,1f)] public float Openess; //random switch order of traversable points
    /*Done*/[Range(0.1f,1f)] public float Conscientiosness; //Mapped to a functionality that people randomly just stand still inside the store when this is low, when high people have a direct plan and don't just randomly stop
    [Range(0.1f,1f)] public float Agreeableness; //create extra movement speed when coming across people with also a high agreeableness
    /*Done*/ [Range(0.1f,1f)] public float Neuroticism; //linked with movement speed - low people move slower and are less in a hurry and high people almost run.

    [SerializeField]
    float _minWaitInterval;
    [SerializeField]
    float _maxWaitInterval;

    //Timers
    private float _pauzePathTimer; //how long does the pause last ?
    private float _pauzeInterval; //how often do the pauzes occur ?
    private float _currentPauzePathTimer;
    private float _currentPauzeInterval;

    private bool _canSwitch;
    // Start is called before the first frame update
    void Start()
    {
        _pauzePathTimer = Conscientiosness <= 0.5f ? Random.Range(5f, 10f) : Random.Range(0f, 3f);

        _pauzePathTimer = Random.Range(_minWaitInterval, _maxWaitInterval); // amount of time agent stands still
        _pauzeInterval = Random.Range((Conscientiosness * 50), Conscientiosness * 80f); //how lower the conscientiosness value thus the lower the interval between pauzes
        _currentPauzeInterval = _pauzeInterval;
        _currentPauzePathTimer = _pauzePathTimer;

        //how long does an agent need to wait after reaching a waypoint
        GetComponentInParent<Agent>().PositionReachedDelay = Random.Range(_minWaitInterval / (Conscientiosness * 10), _maxWaitInterval / (Conscientiosness * 10)); //also mapped to the conscientiosness value
        //example min=5 conscientiosness = 0.2 (*10) = we get 5 / 2 = 2.5 seconds 
    }



    // Update is called once per frame
    void Update()
    {
        if(!GetComponentInParent<Agent>().IsGoingToCheckout && !GetComponentInParent<Agent>().IsAgentOnWaypoint) //only update this as long as the agent is still shopping and not going to checkout and not standing still at a waypoint
            UpdateWaitBehavior();
    }
    public void TriggerOpenessCalculation(int currentWaypointIndex) //this function is triggered when an agent has reached waypoint and swaps some waypoints due to his openess factor
    {
        Agent tempAgent = GetComponentInParent<Agent>();

        for (int i = 0; i < Openess * 10; i++) //Openess tells the amount of shuffles
        {
            int rand1 = Random.Range(currentWaypointIndex, tempAgent.OrderOfTravelPoints.Count);
            int rand2 = Random.Range(currentWaypointIndex, tempAgent.OrderOfTravelPoints.Count);
            Transform tempTransform = tempAgent.OrderOfTravelPoints[rand1];
            tempAgent.OrderOfTravelPoints[rand1] = tempAgent.OrderOfTravelPoints[rand2];
            tempAgent.OrderOfTravelPoints[rand2] = tempTransform;
        }
    }
    private void UpdateWaitBehavior()
    {
        _currentPauzeInterval -= Time.deltaTime;
        if (_currentPauzeInterval < 0f)
        {
            _currentPauzePathTimer -= Time.deltaTime;
            if (_canSwitch)
            {
                GetComponentInParent<Agent>().SwitchAgentActive(0);
                _canSwitch = false;
            }
            if (_currentPauzePathTimer < 0f) //we reset everything we pauzed and switch back moving
            {
                _canSwitch = true;
                GetComponentInParent<Agent>().SwitchAgentActive(1);
                _currentPauzeInterval = _pauzeInterval;
                _currentPauzePathTimer = _pauzePathTimer;
            }
        }
    }
    
}
