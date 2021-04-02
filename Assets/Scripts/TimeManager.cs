using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  handle time in game for pause and slow motion
/// </summary>
public static class TimeManager 
{

    /// <summary>
    ///  slow down time by slowDownFactor and update fixedDeltaTime. 
    /// </summary>
    public static void SlowMotion(float slowDownFactor){
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = Time.timeScale * Time.fixedUnscaledDeltaTime;
    }

    /// <summary>
    ///  restore the time and update time. 
    /// </summary>
    public static void RestoreTime(){
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.fixedUnscaledDeltaTime;
    }

}
