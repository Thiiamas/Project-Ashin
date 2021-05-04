using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{

    public static List<Collider2D> GetCollidersInCollider(Collider2D col, LayerMask layer)
    {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(layer);
        filter.useTriggers = true;
        Physics2D.OverlapCollider(col, filter, colliders);
        return colliders;
    }

    public static void StartParticleSystem(ParticleSystem ps, float psDuration) 
    {        
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.duration = psDuration;
        ps.Play();
    }
}
