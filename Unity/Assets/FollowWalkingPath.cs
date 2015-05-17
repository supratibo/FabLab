using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;

public class FollowWalkingPath : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (sendSensorData ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LaunchAnimation() {
		iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("MobileSensorPath"), "time", 20, "easytype", iTween.EaseType.easeInOutElastic));
	}

	IEnumerator sendSensorData() {
		
		
		string url = "http://localhost:8080/rest/items/MobileSensor/state";
		int dataLength = gameObject.transform.position.ToString().Length;
		string data = gameObject.transform.position.ToString().Remove(dataLength - 1, 1).Remove(0, 1);
		
		
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
		
		yield return new WaitForSeconds(0.1f);

		StartCoroutine (sendSensorData ());
	}
}
