using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    float maxTime, timeLeft;
    bool isOn = false;
    public bool IsOn {
        get => isOn;
    }

    public Timer(float _maxTime) {
        maxTime = _maxTime;
        isOn = false;
    }

    public void Decrease(float timeToDecrease) 
    {
        timeLeft -= timeToDecrease;
        if( timeLeft <= 0 && isOn ) {
            Reset();
        }        
    }

    public void Start() 
    {
        isOn = true;
        timeLeft = maxTime;
    }

    public void Reset() 
    {
        isOn = false;
        timeLeft = 0;
    }



}
