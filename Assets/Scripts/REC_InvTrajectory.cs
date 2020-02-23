using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If i have to Choose one, I'll Definitely go with this one
//because it's little Memory footprint <3
public class REC_InvTrajectory : Unit
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
    float iTime;
    [SerializeField] Vector3 lVel, aVel;
    [SerializeField]
    List<TimeEvent> events = new List<TimeEvent>();
    public override void Set_Velocity(Vector3 vel)
    {
        lVel = vel;
        aVel = Random.insideUnitSphere * 1E2F; // looks better
        iTime = Time.time;
    }

    static bool record = false;
    public override void Record_Start()
    {
        events.Clear();
        events.Add(new TimeEvent(transform));
    }
    public override int Record_Stop()
    {
        record = false;
        events.Add(new TimeEvent(transform));
        return events.Count;
    }
    public override bool Record_Capture()
    {
        return true;
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
        float time = (Time.time - iTime);
        float maxTime = TimeStone.inst.interval;
        while (true)
        {
            var simSpeed = TimeStone.inst.speed;
            switch (TimeStone.inst.state)
            {
                case TimeStone.TimeState.None:
                    corID = null;
                    yield break;
                case TimeStone.TimeState.Freeze:
                    yield return null;
                    break;
                case TimeStone.TimeState.Flow:
                    time += Time.deltaTime * simSpeed;
                    if (time < maxTime)
                        Simulate(events[0], events[1], time);
                    else
                        time = maxTime;
                    yield return null;
                    break;
                case TimeStone.TimeState.Rewind:
                    time -= Time.deltaTime * simSpeed;
                    if (time > 0)
                        Simulate(events[0], events[1], time);
                    else
                        time = 0;
                    yield return null;
                    break;
            }
        }
    }
    void Simulate(TimeEvent frm, TimeEvent to, float t)
    {
        Vector3 nPos = frm.iPos + lVel * t;
        nPos.y += (0.5f * (-9.81f) * t * t);
        transform.position = nPos;

        Quaternion nRot = Quaternion.Euler(frm.iRot.eulerAngles + aVel * t);
        transform.rotation = nRot;
    }
}
