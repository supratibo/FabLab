using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;

public class PlaceSensor : MonoBehaviour {

	public GameObject confirmDialogBox;

	public Text dialogText;

	public GameObject sensorRepresatingObject;
	
	private Vector3 clickedPosition = Vector3.zero;

	private int currentNumberOfSensors = 0;
	private int maxNumberOfSensors = 3;

	private List<GameObject> sensorList;

	public GameObject resetButton;
	public GameObject animButton;

	// Use this for initialization
	void Start () {
		sensorList = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (currentNumberOfSensors == maxNumberOfSensors) {
			resetButton.SetActive (true);
			animButton.SetActive(true);
		} else {
			resetButton.SetActive (false);
			animButton.SetActive(false);
		}

		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
			RaycastHit hit;

			bool HitRayCast = Physics.Raycast (ray, out hit, 1000); //casts a ray and fill the hit information, if nothing is hit returns false

			if (HitRayCast && !confirmDialogBox.activeSelf && hit.collider.gameObject.tag != "Sensor") //if something that isn't a sensor was hit and the dialog box isn't activated
			{
				clickedPosition = hit.point;

				if(currentNumberOfSensors < maxNumberOfSensors) //If there's still sensors to be positioned
				{
					dialogText.text = "Position sensor #" + (currentNumberOfSensors + 1) + " here?"; 
					OpenDialogBox();
				} 
			}
		}

	}

	public void ConfirmDialogBox () {

		GameObject createdSensor = GameObject.Instantiate (sensorRepresatingObject, clickedPosition, Quaternion.identity) as GameObject;
		//instantiate an object at the clicked position to represent the sensor in the scene 
		sensorList.Add (createdSensor);

		currentNumberOfSensors++;

		StartCoroutine (sendSensorData (currentNumberOfSensors));
		//start coroutine to send sensor's position to openhab

		CloseDialogBox ();
	}

	public void CancelDialogBox () {
		CloseDialogBox ();
	}

	private void CloseDialogBox() {
		confirmDialogBox.SetActive (false);
	}


	private void OpenDialogBox() {
		confirmDialogBox.SetActive (true);
	}

	IEnumerator sendSensorData(int sensorNumber) {


		string url = "http://localhost:8080/rest/items/Sensor" + sensorNumber + "/state";
		int dataLength = clickedPosition.ToString ().Length;
		string data = clickedPosition.ToString().Remove(dataLength - 1, 1).Remove(0, 1);


		HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
		request.Method = "PUT";
		request.ContentType = "text/plain";

		using (StreamWriter writer = new StreamWriter(request.GetRequestStream( ))) {
			writer.WriteLine (data);
		}

		WebResponse response = request.GetResponse ();

		using (StreamReader reader = new StreamReader(response.GetResponseStream( ))) {
			while (reader.Peek( ) != -1) {
					Debug.Log (reader.ReadLine ());
			}
		}

		yield return new WaitForSeconds(0f);
	}

	public void resetSensorData() {

		foreach (GameObject sensor in sensorList)
			GameObject.Destroy (sensor);

		clickedPosition = Vector3.zero;

		for(int i = 1; i <= maxNumberOfSensors; i++)
			StartCoroutine(sendSensorData(i));

		currentNumberOfSensors = 0;

	}



	
}
