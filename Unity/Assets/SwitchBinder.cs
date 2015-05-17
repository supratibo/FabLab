using UnityEngine;
using System.Collections;

public class SwitchBinder : MonoBehaviour {

	//Script that binds the active state of an object to a switch state of OpenHAB

	public string stateURL;
	public string stateTest;
	// Use this for initialization
	void Start () {
		StartCoroutine (stateUpdater ());
	}
	
	// Update is called once per frame
	void Update () {


	}
	IEnumerator stateUpdater()
	{
				yield return new WaitForSeconds (0.1f);
				StartCoroutine (getState());
				StartCoroutine (stateUpdater());
		}
	IEnumerator getState()
	{
		WWW www = new WWW(stateURL);
		yield return www;
		
		string state = www.text;

		stateTest = state;

		if (state.Equals ("ON")) 
		{
			foreach(Transform child in transform)
				child.gameObject.SetActive(true);
		}
		else
		{
			foreach(Transform child in transform)
				child.gameObject.SetActive(false);
		}
		
	}
}
