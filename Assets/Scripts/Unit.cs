using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    //Using Abstraction, because i'm looking for more solution as well.
    //Event based Recording, Trajectory Back Tracing, etc
    public Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public virtual void Set_Velocity(Vector3 vel)
    {
        if(rb != null)
            rb.velocity = vel;
    }
    public virtual void Record_Start()
    {
    }
    public virtual bool Record_Capture()
    {
        return false;
    }
    public virtual int Record_Stop()
    {
        return 0;
    }
    public virtual void Record_Play()
    {
    }
    public virtual void Record_End()
    {

    }
    public virtual void Reset()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
	}
}
