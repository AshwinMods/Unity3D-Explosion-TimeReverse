using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class REC_FixedInterval : Unit
{
    [System.Serializable]
    public struct TimeEvent
    {
        public TimeEvent(Transform trans)
        {
            iPos = trans.position;
            iRot = trans.rotation;
        }
        public Vector3 iPos;
        public Quaternion iRot;
    }
    [SerializeField]
    List<TimeEvent> events = new List<TimeEvent>();
    public override void Set_Velocity(Vector3 vel)
    {
        rb.velocity = vel;
    }

    static bool record = false;
    public override void Record_Start()
    {
        events.Clear();
        Record_Capture();
    }
    public override int Record_Stop()
    {
        Record_Capture();
        record = false;
        return events.Count;
    }
    public override bool Record_Capture()
    {
        events.Add(new TimeEvent(transform));
        return !rb.IsSleeping();
    }

    Coroutine corID;
    public override void Record_Play()
    {
        if (corID != null)
            StopCoroutine(corID);
        if (events.Count > 0)
            corID = StartCoroutine(ProcessRecord());
    }
    public override void Record_End()
    {
        if (corID != null)
            StopCoroutine(corID);
    }

    IEnumerator ProcessRecord()
    {
        int itr = events.Count - 1;
        while (true) // The Endless bargin with TimeLoop
        {
            transform.position = events[itr].iPos;
            transform.rotation = events[itr].iRot;
            switch (TimeStone.inst.state)
            {
                case TimeStone.TimeState.None:
                    corID = null;
                    yield break;
                case TimeStone.TimeState.Freeze:
                    yield return null;
                    break;
                case TimeStone.TimeState.Flow:
                    if (itr + 1 < events.Count)
                        yield return Simulate(events[itr], events[++itr]);
                    else
                        yield return null;
                    break;
                case TimeStone.TimeState.Rewind:
                    if(itr > 0)
                        yield return Simulate(events[itr], events[--itr]);
                    else
                        yield return null;
                    break;
            }
        }
    }
    IEnumerator Simulate(TimeEvent frm, TimeEvent to)
    {
        float t = 0f;
        var simSpeed = TimeStone.inst.speed / TimeStone.inst.interval;
        while (t < 1)
        {
            t += Time.deltaTime * simSpeed;
            transform.position = Vector3.Lerp(frm.iPos, to.iPos, t);
            transform.rotation = Quaternion.Lerp(frm.iRot, to.iRot, t);
            yield return null;
        }
        transform.position = to.iPos;
        transform.rotation = to.iRot;
    }
}
