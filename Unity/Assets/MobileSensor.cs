using UnityEngine;
using System.Collections;

public class MobileSensor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (positionUpdater ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator positionUpdater()
	{
		yield return new WaitForSeconds (0.1f);
		StartCoroutine (getPosition());
		StartCoroutine (positionUpdater ());
	}
	IEnumerator getPosition()
	{
		Vector3 newPosition = Vector3.zero;
		WWW www = new WWW("http://localhost:8080/rest/items/MobileSensor/state");

		yield return www;
		
		string positionString = www.text;
		string[] splitPosition = positionString.Split(',');

		if (splitPosition.Length == 3) {
			newPosition.x = float.Parse (splitPosition [0]);
			newPosition.y = float.Parse (splitPosition [1]);
			newPosition.z = float.Parse (splitPosition [2]);
		}

		rigidbody.MovePosition (newPosition);

	}
}
