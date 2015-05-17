using UnityEngine;
using System.Collections;

public class MouseRayGizmos : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Draw a line in the editor to see mouse's raycast if it hits any object
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		bool HitRayCast = Physics.Raycast (ray, out hit, 1000);

		if(HitRayCast)
			Debug.DrawLine(transform.position, hit.point, Color.red);
	}
}
