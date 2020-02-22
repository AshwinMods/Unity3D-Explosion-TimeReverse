using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xplosion : MonoBehaviour
{	
	[Header("Placement")]
	[SerializeField] float radius = 10;
	[SerializeField] float angleStep = 10;

	[Header("Xplosion")]
	[SerializeField] float xMagnitude = 20; //I'm not being technical here...

	[Header("Reference")]
	public Unit[] units;
	public Transform unitParent;
	private void OnEnable()
	{
		Reset();
	}

	public void Reset()
	{
		if (unitParent.childCount != units.Length)
			units = unitParent.GetComponentsInChildren<Unit>();
		float t = 0;
		var pos = Vector3.zero;
		for (int i = 0; i < units.Length; i++)
		{
			pos.x = radius * Mathf.Cos(t * Mathf.Deg2Rad);
			pos.z = radius * Mathf.Sin(t * Mathf.Deg2Rad);
			pos.y = ((int)t / (360));
			units[i].transform.position = pos;
			units[i].rb.velocity = Vector3.zero;
			units[i].rb.angularVelocity = Vector3.zero;
			units[i].transform.forward = pos + (Vector3.down * pos.y); //Sphere to Cylinder :P
			t += angleStep;
		}
	}

	public void Detonate()
	{
		//For Noise
		var xPoint = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 0f), Random.Range(-2f, 2f)); //Who doesn't love Flying ?
		var xSpeed = xMagnitude + Random.Range(-2f, 2f);
		Vector3 xVel;
		Vector3 dir;
		foreach (var u in units)
		{
			//u.rb.AddExplosionForce(xForce, xPoint, 1E4F, 0f);
			dir = u.transform.position - xPoint;
			xVel = dir.normalized * Mathf.Lerp(xSpeed, 0f, (dir.magnitude/100f)); //Drop to 0 With Distance of 100, Will it look good? IDK
			u.Set_Velocity(xVel);
		}
	}
}
