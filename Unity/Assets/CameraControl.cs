using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float movementSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		

	
	}

	void FixedUpdate() {
		if (Input.GetKey (KeyCode.LeftArrow))
			GetComponent<CharacterController>().Move(-transform.right*movementSpeed);
		
		if(Input.GetKey(KeyCode.RightArrow))
			GetComponent<CharacterController>().Move(transform.right*movementSpeed);
		
		if(Input.GetKey(KeyCode.UpArrow))
			GetComponent<CharacterController>().Move(transform.forward*movementSpeed);
		
		if(Input.GetKey(KeyCode.DownArrow))
			GetComponent<CharacterController>().Move(-transform.forward*movementSpeed);
	}
}
