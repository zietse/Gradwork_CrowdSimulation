using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Extraversion = low value is people that don't like to be near other people, high value is vey social people
//Openness = low is people that don't like change, don't enjoy new things, high is creative and focussed on tackling new challenges
// Conscientiosness = low is people that dislike structure, make messes, fails to complete tasks, high is people that finish important tasks right away and spend time preparing.
// Agreeableness = low is people that are more competitive and sometimes manipulative, high in this is people that are sometimes manipulative but mostly care about others and feel empathy
// Neuroticism = low is don't worry much, deal well with stress, high is experience a lot of stress, get upset easily,...


//Extraversion for now has effect on the how close people are to each other
//Neuroticism has effect on how fast people go through the store
public class Traits : MonoBehaviour
{
    //emotional traits specifiers
    [Range(0.5f,1f)] public float Extraversion;
    [Range(0f,1f)] public float Openess;
    [Range(0f,1f)] public float Conscientiosness;
    [Range(0f,1f)] public float Agreeableness;
    [Range(0f,1f)] public float Neuroticism;


    //Timers
    private float _pauzePathTimer;
    private float _pauzeInterval;
    private float _currentPauzePathTimer;
    private float _currentPauzeInterval;

    private bool _canSwitch;
    // Start is called before the first frame update
    void Start()
    {
        _pauzePathTimer = 10.0f;
        _pauzeInterval = Random.Range(10f, 25f);
        _currentPauzeInterval = _pauzeInterval;
        _currentPauzePathTimer = _pauzePathTimer;
    }


    // Update is called once per frame
    void Update()
    {
        //UpdateNavmeshWaitBehavior();
    }
    private void UpdateNavmeshWaitBehavior()
    {
        _currentPauzeInterval -= Time.deltaTime;
        if (_currentPauzeInterval < 0f)
        {
            _currentPauzePathTimer -= Time.deltaTime;
            if (_canSwitch)
            {
                GetComponentInParent<Agent>().SwitchAgentType(0);
                _canSwitch = false;
            }
            if (_currentPauzePathTimer < 0f) //we reset everything we pauzed and switch back to normal agent
            {
                _canSwitch = true;
                GetComponentInParent<Agent>().SwitchAgentType(1);
                _currentPauzeInterval = _pauzeInterval;
                _currentPauzePathTimer = _pauzePathTimer;
            }
        }
    }
}
