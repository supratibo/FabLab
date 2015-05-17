using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using AssemblyCSharpfirstpass;

public class SensorScript : MonoBehaviour {
	public static float scale = 0.1f;
	//private int Count = 0;
	private Vector3 SizeValue = new Vector3 (0.4f, 0.4f, 0.4f);
	private Vector3 SizeMeta  = new Vector3 (1.2f, 0.4f, 0.4f);

	public Sensor Sensor;
	public SensorUpdator _SensorUpdator;
	public int Count = 0;

	public GameObject SensorDisplay;

	public bool SensorReady = false;
	public bool SensorInit = false;

	public string[] StringNamesID = {};
	public string[] StringValue = {};
	public string[] StringMeta = {};
	public Color ColorBG = Color.cyan;
	public Color ColorText = Color.white;

	public bool DisplayValue = true;
	public bool DoMove = false;


	public bool[] Display = {true, true};




	// Use this for initialization
	void Start () {
	}


	// Update is called once per frame
	void Update () {
		if(!SensorReady & SensorInit){
			float angle = 180 - Vector3.Angle(Vector3.forward,new Vector3(Sensor.SensorNormal.x,0,Sensor.SensorNormal.z));
			if(Vector3.Dot(Vector3.left,new Vector3(Sensor.SensorNormal.x,0,Sensor.SensorNormal.z)) < 0f)
				angle = - angle;
			try{	
				SensorDisplay = (_SensorUpdator.Import(System.IO.File.ReadAllLines(StaticMemory.CurrentPath + "/Sensor/" + Sensor.SensorDefault.PathVisualSensor))).GObject;
			}
			catch{
				SensorDisplay = (_SensorUpdator.Import(System.IO.File.ReadAllLines(StaticMemory.CurrentPath + "/Sensor/Default"))).GObject;
				}
			_SensorUpdator.KeyValuesRootList.AddLast(new SensorUpdator.KeyValue("[#Normale]","" + angle));
			SensorReady = true;
		}
		if(transform.lossyScale.x != scale){

			float S = transform.lossyScale.x;
			float s = transform.localScale.x;

			transform.localScale = scale * s / S * Vector3.one;
		}
	}



	public void UpdateValue (LinkedList<ParseXML> allSensorXML)	{
		_SensorUpdator.SensorValues.Clear();
		foreach(string s in Sensor.NameID){
			foreach(ParseXML xml in allSensorXML){
				if(s == xml.Name){
					_SensorUpdator.SensorValues.AddLast(xml);
				}
			}
		}
	}



	public bool GetDisplay(int index){
		return Display[index];
	}


	// Set active or Inactive function of Visu selection of PLace/Functions
	public void UpdateDisplay(int index, bool value){
		Display[index] = value;
		bool b = true;
		foreach(bool val in Display){
			b = b && val;
		}
		Sensor.SensorObject.SetActive(b);
	}



}
