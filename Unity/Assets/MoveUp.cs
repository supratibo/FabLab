using UnityEngine;
using System.Collections;

public class MoveUp : MonoBehaviour {

	public float speed;
	public float waitTime;
	public bool move = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(move)
			gameObject.rigidbody.MovePosition(new Vector3(transform.position.x, transform.position.y + speed, transform.position.z));
		if (transform.position.y > -6.5f)
						move = false;
	}

	public void startAnimation() {
		StartCoroutine (startMoving ());
	}

	IEnumerator startMoving() {
		yield return new WaitForSeconds(waitTime);
		move = true;
	}
}
