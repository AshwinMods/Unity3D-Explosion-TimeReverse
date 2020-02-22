using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class CamController : MonoBehaviour
{
	[SerializeField] float sensivity = 0.1f;
	public void On_Drag(BaseEventData e)
	{
		var p = (PointerEventData)e;
		var rot = new Vector3(-p.delta.y, p.delta.x, 0) * sensivity;
		transform.eulerAngles += rot;
	}
}
