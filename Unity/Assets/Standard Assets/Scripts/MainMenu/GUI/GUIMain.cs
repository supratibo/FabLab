using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class GUIMain : MonoBehaviour {
	public Texture A4HTexture;
	private bool UsePrefab = false;
	private string CurrentPreSelectedBuilding = "None Selected";

	private string SensorCfgFile = "";
	private string SensorFile = "";
	private string CurrentPreSelectedBuildingGeo = "";
	private string CurrentPreSelectedBuildingSen = "";
	private bool BuildingCheck = false;
	private bool GeoCheck = false;
	private bool SenCheck = false;
	private string[] ArrayofGeo = new string[0];
	private int ArrayofGeoCount = -1;
	private string[] ArrayofSen = new string[0];
	private int ArrayofSenCount = -1;

	private char chk = (char)10003;
	private Browser CurrentPreSelectedBuildingUI;

	private bool autorun = false;


	// Use this for initialization
	void Awake () {
		if(StaticMemory.RunFirstTime){
			StaticMemory.RunFirstTime = false;
			string[] args = System.Environment.CommandLine.Split(' ');
			for(int i = 0; i< args.Length; i++){
				if ( args[i] == "--path" || args[i] == "-p"){
					StaticMemory.CurrentPath = args[i+1];
					StaticMemory.CurrentPreSelectedBuildingPath = args[i+1];
					i++;
				}
				if ( args[i] == "--current" || args[i] == "-c"){
					StaticMemory.CurrentPreSelectedBuildingPath = args[i+1];
					i++;
				}
				if ( args[i] == "--url" || args[i] == "-u"){
					StaticMemory.URL = args[i+1];
					i++;
				}
				if ( args[i] == "--auto" || args[i] == "-a"){
					autorun = true;
				}

				if ( args[i] == "--time" || args[i] == "-t"){
					StaticMemory.Step = float.Parse(args[i+1]);
				}

			}
		}
		StaticMemory.A4HTexture = A4HTexture;
	}

	void Start () {
		//_tcp = new TCPServer();
		CurrentPreSelectedBuildingUI = gameObject.AddComponent<Browser>() as Browser;
		CurrentPreSelectedBuildingUI.name = "CurrentPreSelectedBuildingUI";


		if(autorun){
			doCheck();
		}
	}


	void Update () {
		
		StaticMemory._SizeX = Screen.width  - 2 * StaticMemory._Margin;
		StaticMemory._SizeY = Screen.height - 3 * StaticMemory._Margin - StaticMemory._Caption;

		if(StaticMemory.SaveRoomList){
			System.IO.File.WriteAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/House.geo" ,  Geoloc.Export(StaticMemory.RoomList));
			StaticMemory.SaveRoomList = false;
			doCheck();
		}
		if(StaticMemory.LoadRoomList){
			StaticMemory.RoomList = Geoloc.Import(System.IO.File.ReadAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/House.geo"));
			StaticMemory.LoadRoomList = false;
		}
		if(StaticMemory.LoadSensorCfgList){
			StaticMemory.SensorCfgList = new LinkedList<SensorAndFunction>();
			SensorAndFunction.Import(StaticMemory.SensorCfgList, System.IO.File.ReadAllLines(StaticMemory.CurrentPath + "/Sensor/Sensor.sensorCfg"));
			StaticMemory.LoadSensorCfgList = false;
		}
		if(StaticMemory.LoadSensorList){
			Sensor.Active = false;
			StaticMemory.LoadSensorList = false;
			Sensor.Active = true;

		}

		if(Input.GetKey(KeyCode.KeypadEnter)){
			doCheck();
		}


	}

	private string _ErrorsLogs = "";
	private int id = -1;
	private bool isCorrect = false;

	public static int _display = 0;

	void DoMyWindow(int windowID) {
		//if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World"))
			print("Got a click in window " + windowID);
		
		GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}

	void OnGUI(){
		/*
		Rect windowRect0 = new Rect(20, 20, 120, 50);
		Rect windowRect1 = new Rect(20, 100, 120, 50);
		GUI.color = Color.red;
		windowRect0 = GUI.Window(0, windowRect0, DoMyWindow, " ");
		GUI.color = Color.black;
		*/
		if(_display == 0){
		int currentY;
		int largeur;
		/////////////////////////////////////AFFICHAGE
			// Groupe : Caption
			GUI.BeginGroup(new Rect (0,0,2* StaticMemory._Margin + StaticMemory._SizeX, 3 * StaticMemory._Margin + StaticMemory._SizeX));
			GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin, StaticMemory._SizeX, StaticMemory._Caption));
			GUI.DrawTexture(new Rect(StaticMemory._SizeX - 3*StaticMemory._Caption,0, StaticMemory._Caption*3, StaticMemory._Caption), StaticMemory.A4HTexture, ScaleMode.StretchToFill, true, 0);
			GUI.Box(new Rect(0,0, StaticMemory._SizeX, StaticMemory._Caption), "");
		currentY = 0;
		largeur  = StaticMemory._SizeX - 2 * StaticMemory._Margin;
			
	

			GUI.Box(new Rect(StaticMemory._Margin + StaticMemory._SizeX/4, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2, largeur/2, StaticMemory._hauteurChamp),"Welcome");

		GUI.EndGroup();
		
		// Groupe : Corps de la GUI
		GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX , StaticMemory._SizeY));
		
		// 1ere collone
		GUI.BeginGroup (new Rect(0, 0, StaticMemory._SizeX / 4, StaticMemory._SizeY));
		GUI.Box (new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY), "");
		currentY = StaticMemory._Caption + StaticMemory._Margin;
		largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
		currentY = 0;
		GUI.Box(new Rect(0, 0, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Run Panel");
		currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 2;
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 2;
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 2;

		

			if(GeoCheck && SenCheck){
				if(autorun){
					autorun = false;
					Application.LoadLevel(1);
				}

				if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Run")){
					Application.LoadLevel(1);
				}
			}
		currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 2;

		
		currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 2;
		currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 2;

		
			if(BuildingCheck)
			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Manage Home Settings")){
			_display = 2;
		}
		currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 2;
		if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Manage Sensors Settings")){
			_display = 1;
		}

			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 2;
			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Quit")){
				Application.Quit();
			}
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 2;

		GUI.EndGroup();
		
		// 2eme collone
		GUI.BeginGroup (new Rect(StaticMemory._SizeX / 2, 0, StaticMemory._SizeX / 2, StaticMemory._SizeY));
		GUI.Box (new Rect(0,0, StaticMemory._SizeX / 2, StaticMemory._SizeY), "");
		currentY = StaticMemory._Caption + StaticMemory._Margin;
		largeur  = StaticMemory._SizeX / 2 - 2 * StaticMemory._Margin;
		currentY = 0;
		GUI.Box(new Rect(0, 0, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Settings");
			currentY += StaticMemory._hauteurChamp  + StaticMemory._Margin;

		//Browser __b =  GetComponent<Browser>();
		
		GUI.Box (new Rect (StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), " Building model");
		currentY += StaticMemory._hauteurChamp  + StaticMemory._hauteurEntre;

		GUI.Label (new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp ), CurrentPreSelectedBuilding);
		currentY += StaticMemory._hauteurChamp  + StaticMemory._hauteurEntre;

			StaticMemory.CurrentPreSelectedBuildingPath = GUI.TextField (new Rect (StaticMemory._Margin, currentY, largeur - 2 * StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), StaticMemory.CurrentPreSelectedBuildingPath);
		if (GUI.Button (new Rect (StaticMemory._Margin + largeur - 2 * StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), ">")){
				CurrentPreSelectedBuildingUI.OpenFile(StaticMemory.CurrentPath,"","l");
		}
			if (GUI.Button (new Rect (StaticMemory._Margin + largeur - 1 * StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "" + chk)){
			doCheck();
		}

			if(StaticMemory.ArrayofOBJ.Length > 0){
			currentY += StaticMemory._hauteurChamp;
				for (int i = 0 ; i< StaticMemory.ArrayofOBJ.Length ; i ++){
					GUI.Label(new Rect (StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), StaticMemory.ArrayofOBJ[i]);
				currentY += StaticMemory._hauteurChamp;

			}
		}
		else{
			currentY += StaticMemory._hauteurChamp;
		}


		if(CurrentPreSelectedBuildingUI.GetResponse){
				StaticMemory.CurrentPreSelectedBuildingPath = CurrentPreSelectedBuildingUI.responce;
			CurrentPreSelectedBuildingUI.GetResponse = false;
		}

		currentY += StaticMemory._hauteurEntre;


		if(BuildingCheck){
			GUI.Label(new Rect (StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), CurrentPreSelectedBuildingGeo);

			currentY += StaticMemory._hauteurChamp;
			if(ArrayofGeo.Length > 0){
				for (int i = 0 ; i< ArrayofGeo.Length ; i ++){
					GUI.Label(new Rect (StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), ArrayofGeo[i]);
					currentY += StaticMemory._hauteurChamp;
					if(ArrayofGeoCount >= 0){
							CurrentPreSelectedBuildingGeo = "" + chk;
						GeoCheck = true;
					}
				}
			}


			if(!GeoCheck && ArrayofGeo.Length == 0){
				if(GUI.Button(new Rect (StaticMemory._Margin + largeur/4 , currentY, largeur /2 , StaticMemory._hauteurChamp ), "Manage Home Settings")){
					_display = 2;
				}
				currentY += StaticMemory._hauteurChamp  + StaticMemory._hauteurEntre;
			}



			
		/*************************************************************************************************/
			if(GeoCheck){

					currentY = StaticMemory._SizeY / 2;
					if(!SenCheck && GeoCheck && ArrayofSen.Length == 0){
						GUI.Box (new Rect (StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), "Sensor settings");
						currentY += StaticMemory._hauteurChamp  + StaticMemory._hauteurEntre;
						if(GUI.Button(new Rect (StaticMemory._Margin + largeur/4 , currentY, largeur /2 , StaticMemory._hauteurChamp ), "Set Sensor's Location")){
							Application.LoadLevel(1);
						}
						currentY += StaticMemory._hauteurChamp  + StaticMemory._hauteurEntre;
					}

						if(ArrayofSen.Length > 0){
							for (int i = 0 ; i< ArrayofGeo.Length ; i ++){
							if(GUI.Toggle(new Rect (StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp),ArrayofSenCount == i, ArrayofSen[i])){
									ArrayofSenCount = i;
								}
								currentY += StaticMemory._hauteurChamp;
								if(ArrayofSenCount >= 0){
									CurrentPreSelectedBuildingGeo = "" + chk;
									StaticMemory.LoadSensorList = true;
									StaticMemory.LoadSensorPath = StaticMemory.CurrentPreSelectedBuildingPath + "/" + ArrayofSen[i];
									SenCheck = true;
								}
							}
						}

					currentY += StaticMemory._hauteurChamp  + StaticMemory._hauteurEntre;
				}
			}
			currentY = StaticMemory._SizeY - 4 * StaticMemory._hauteurChamp  - 2 * StaticMemory._hauteurEntre;
			GUI.Box (new Rect(StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), "Database URL");
			currentY += StaticMemory._hauteurChamp;
			StaticMemory.URL = GUI.TextField (new Rect(StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), StaticMemory.URL);
			currentY += StaticMemory._hauteurChamp  + StaticMemory._hauteurEntre;

			GUI.Box (new Rect(StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), "Sensor Configuration");
			currentY += StaticMemory._hauteurChamp;
			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur/2, StaticMemory._hauteurChamp), "Reload")){
				if (SensorCfgFile != ""){
					SensorAndFunction.Import(StaticMemory.SensorCfgList, System.IO.File.ReadAllLines(StaticMemory.CurrentPath + "/Sensor/Sensor.sensorCfg"));
				}
			}
			if(GUI.Button(new Rect (StaticMemory._Margin + largeur/2, currentY, largeur/2, StaticMemory._hauteurChamp), "Save")){
				string s = SensorAndFunction.Export(StaticMemory.SensorCfgList);
				System.IO.File.WriteAllText(StaticMemory.CurrentPath + "/Sensor/Sensor.sensorCfg", s);

			}
				

		GUI.EndGroup();
		GUI.EndGroup();
		GUI.EndGroup();
	}
	}


	private void doCheck (){
		BuildingCheck = false;
		GeoCheck = false;
		SenCheck = false;
		try{
			
			string[] res = Directory.GetFiles(StaticMemory.CurrentPreSelectedBuildingPath + "/", "*.obj");
			StaticMemory.ArrayofOBJ = new string[res.Length];
			StaticMemory.ArrayofOBJBool = new bool[res.Length];
			for(int i = 0 ; i<res.Length ;i++){
				string[] file = res[i].Split('/');
				StaticMemory.ArrayofOBJ[i] = file[file.Length -1];
				StaticMemory.ArrayofOBJBool[i] = true;
			}

			if(res.Length == 0){
				CurrentPreSelectedBuilding = "No *.obj file found!";
			}
			if(res.Length == 1){
				string[] file = res[0].Split('/');
				CurrentPreSelectedBuilding = "Load: " + file[file.Length -1];
				BuildingCheck = true;
			}
			if(res.Length > 1){
				CurrentPreSelectedBuilding = "Load multiples files (see bellow)";
				BuildingCheck = true;
			}

			
			
			ArrayofGeo = new string[0];
			ArrayofGeoCount = -1;
			res = Directory.GetFiles(StaticMemory.CurrentPreSelectedBuildingPath + "/", "*.geo");
			if(res.Length == 0){
				CurrentPreSelectedBuildingGeo = "No information on the current building, please create it";
			}
			if(res.Length == 1){
				StaticMemory.LoadRoomList = true;
				CurrentPreSelectedBuildingGeo = "" + chk;
				GeoCheck = true;
			}
			if(res.Length > 1){
				CurrentPreSelectedBuildingGeo = "Chose one :";
				ArrayofGeo = new string[res.Length];
				
				for(int i = 0 ; i<res.Length ;i++){
					string[] file = res[i].Split('/');
					ArrayofGeo[i] = file[file.Length -1];
				}
			}

			ArrayofSen = new string[0];
			ArrayofSenCount = -1;
			res = Directory.GetFiles(StaticMemory.CurrentPreSelectedBuildingPath + "/", "*.sen");
			if(res.Length == 0){
				CurrentPreSelectedBuildingSen = "No existing set of Sensor";
			}
			if(res.Length == 1){
				CurrentPreSelectedBuildingSen = "" + chk;
				StaticMemory.LoadSensorList = true;
				StaticMemory.LoadSensorPath = res[0];
				SenCheck = true;
			}
			if(res.Length > 1){
				CurrentPreSelectedBuildingSen = "Chose one :";
				ArrayofSen = new string[res.Length];
				
				for(int i = 0 ; i<res.Length ;i++){
					string[] file = res[i].Split('/');
					ArrayofSen[i] = file[file.Length -1];
				}
			}



		}
		catch (IOException e){
			Debug.Log( e);
			CurrentPreSelectedBuilding = "Error: Directory Not Found" ;
			
		}
	}

	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		
		// check for errors
		if (www.error == null)
		{
			Debug.Log(www.data);

		} else {
			Debug.Log(www.error);
		}
	}
	
}
