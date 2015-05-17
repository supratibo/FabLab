using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System;


public class InitialScript : MonoBehaviour {
	private string _output = "sensor.log";
	private string _SensorCfg = "SensorNames.cfg";
	
	private string[] _SensorNames;
	public bool _firstfloor = true;
	public bool _2floor = false;
	public bool _move = false;
	private int _Maxid = 0;
	private LinkedList<Sensor> _sensorList = new LinkedList<Sensor> ();
	private Sensor _Current = new Sensor ();

	// Use this for initialization
	void Awake ()
	{
		
	//	((OBJ)GameObject.FindGameObjectWithTag ("FirstFloor").GetComponentInChildren (typeof(OBJ))).objPath = GUIScriptMainMenu._f;
	//	((OBJ)GameObject.FindGameObjectWithTag ("SecondFloor").GetComponentInChildren (typeof(OBJ))).objPath = GUIScriptMainMenu._s;
	//	_2floor = !(GUIScriptMainMenu._s == "");
	//	_output = GUIScriptMainMenu._o;
	//	_SensorCfg = GUIScriptMainMenu._n;
		
		if (!_2floor)
			GameObject.FindGameObjectWithTag ("SecondFloor").SetActive (false);
	}
	
	
	// Read cfg file and set 2nd floor meshs inactive
	void Start ()
	{
		_SensorNames = File.ReadAllLines (_SensorCfg);
		//Cursor.visible = false;
		if (_2floor) {
			GameObject g2 = GameObject.FindGameObjectWithTag ("SecondFloor");
			g2.SetActiveRecursively (false);
			g2.SetActive (true);
		}
	}
}
