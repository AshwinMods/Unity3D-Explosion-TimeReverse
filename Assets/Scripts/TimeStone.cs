using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TimeStone : MonoBehaviour
{
	public static TimeStone inst; //Per scene Singleton
	private void Awake()
	{
		if (inst != null)
			Destroy(this);
		inst = this;
	}

	// because i've set Scene according to technique
	public void Change_Technique(int tech)
	{
		if (tech != (int)technique)
			UnityEngine.SceneManagement.SceneManager.LoadScene(tech);
	}

	public enum RecTech
	{
		FixedInterval, Manual
	}
	public enum TimeState
	{
		None, Flow, Freeze, Rewind
	}
	[Header("Controll")]
	public RecTech technique;
	public TimeState state;
	public float speed = 1;
	public float SPEED { set { speed = value; } }
	public float interval = 0.5f;
	public AnimationCurve playCurve;// A lovely timeReplayFX Curve maybe

	[Header("Reference")]
	public Unit[] units;
	public Transform unitParent;

	[Header("Events")]
	public UnityEvent onRecordStart = new UnityEvent();
	public UnityEvent onRecordStop = new UnityEvent();
	public UnityEvent onRecordPlay = new UnityEvent();
	public UnityEvent onRecordEnd = new UnityEvent();

	bool recording = false;
	bool chkMovement = false;
	float timer = 0;
	private void OnEnable()
	{
		Physics.autoSimulation = (technique == RecTech.FixedInterval);
	}
	private void LateUpdate()
	{
		if (recording)
		{
			timer += Time.deltaTime;
			if (timer >= interval)
			{
				if (technique == RecTech.FixedInterval)
				{
					timer -= interval;
					chkMovement = false;
					foreach (var u in units)
						chkMovement |= u.Record_Capture();
					if (!chkMovement)
					{
						Time_Record_Stop();
						Time_Record_Play();
					}
				}
				else if (technique == RecTech.Manual)
				{
					Time_Record_Stop();
					Time_Record_Play();
				}
			}
		}
	}
	public void Time_Record_Start()
	{
		if (recording)
			return;

		if (unitParent.childCount != units.Length)
			units = unitParent.GetComponentsInChildren<Unit>();

		state = TimeState.None;
		recording = true;
		Physics.autoSimulation = (technique == RecTech.FixedInterval);
		foreach (var u in units)
			u.Record_Start();
		timer = 0;
		onRecordStart.Invoke();
	}
	public void Time_Record_Stop()
	{
		if (!recording)
			return;

		int evCount = 0;
		foreach (var u in units)
			evCount += u.Record_Stop();
		Debug.Log("Time Stoped, Total Events: " + evCount);
		recording = false;
		onRecordStop.Invoke();
	}
	public void Time_Record_Play()
	{
		Physics.autoSimulation = false;
		if (state == TimeState.None)
		{
			state = TimeState.Freeze;
			foreach (var u in units)
				u.Record_Play();
			onRecordPlay.Invoke();
		}
		state = TimeState.Freeze;
	}
	public void Time_Direction(float dir)
	{
		if (dir > 0)
			state = TimeState.Flow;
		else if (dir < 0)
			state = TimeState.Rewind;
		else
			state = TimeState.Freeze;
	}
	public void Time_Break()
	{
		state = TimeState.None;

		foreach (var u in units)
			u.Reset();
		Physics.autoSimulation = (technique == RecTech.FixedInterval);
		onRecordEnd.Invoke();
	}



	//When you have too much time :D
	[Header("Weekend Stuff")]
	[SerializeField] Color[] colors;
	[System.Serializable]
	public class ColorEvent : UnityEvent<Color> { }
	public ColorEvent onColorChange = new ColorEvent();
	int lCol = 0;
	Coroutine corColor;
	public void Set_Color(int indx)
	{
		if (indx < 0 || indx >= colors.Length || indx == lCol)
			return;
		if (corColor != null)
			StopCoroutine(corColor);
		corColor = StartCoroutine(Tween_Color(colors[lCol], colors[indx]));
		lCol = indx;
	}

	IEnumerator Tween_Color(Color frm, Color to)
	{
		var timer = 0f;
		while (timer < 1)
		{
			timer += Time.deltaTime;
			onColorChange.Invoke(to * timer + frm * (1 - timer));
			yield return null;
		}
	}
}
