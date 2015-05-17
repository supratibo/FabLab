using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System;
using AssemblyCSharpfirstpass;

public class MainAddSensor : MonoBehaviour {
	private float posCam = 4.03f;

	public static bool[] isDisplayed = new bool[0];
	public static GameObject[] objModel = new GameObject[0];
	public LinkedList<GameObject> noVisible = new LinkedList<GameObject>();
	public LinkedList<ParseXML> AllSensorXML = new LinkedList<ParseXML>();


	/* displayGUI Level
	 * -2	Time
	 * -1	New Sensor
	 * 0	None
	 * 1 	Main Menu
	 * 2
	 * 3
	 * 4
	 * 5
	 * 
	 * 
	 * 
	 * 
	 * 
	 */

	public static int displayGUI = 0;







	private string AddNewFunction = "";
	private string AddNewID = "";

	private int CurrentID = -1;
	private int Count = 0;
	private int current = 0;

	private ObjSettingItem CurrentItem;

	private bool isInList = false;
	private bool showSensorDetails = false;
	public bool showAllGeo = true;
	private bool showAllSensor = true;
	private string debug = "";

	public bool GUIGetListFromUrl = false;

	//public GameObject plane;

	public string NameContains = "";


	private string[] _SensorNames;

	public bool _move = false;
	private int _Maxid = 0;
	private Sensor _Current;

	private Hashtable hashtable = new Hashtable();
	private LinkedList<ParseXML> ListAllSensor = new LinkedList<ParseXML>();


	void Awake ()
	{

	}

	// Read cfg file and set 2nd floor meshs inactive
	void Start ()
	{
		hashtable.Add("Content-Type", "text/plain");


		if(StaticMemory.LoadSensorPath != ""){
			StaticMemory.SensorList = Sensor.Import(System.IO.File.ReadAllText(StaticMemory.LoadSensorPath));
		}
		foreach(Sensor s in StaticMemory.SensorList){
			s.SensorObject.GetComponent<Renderer>().material.color = Color.red;
			s.SensorObject.name = "Device";
			s.SensorObject.GetComponent<Renderer>().enabled = false;

			CreateNewSensor(s);
			//s.SensorScript = (SensorScript) s.SensorObject.AddComponent(typeof(SensorScript));

			//s.SensorScript.Sensor = s;
			//s.SensorScript._SensorUpdator = (SensorUpdator) s.SensorObject.AddComponent(typeof(SensorUpdator));;

			StaticMemory.SensorCount++;
			s.SensorScript.Count = StaticMemory.SensorCount;

			s.SensorScript.SensorInit = true;

		}
	}


	private float doubleClickStart = -1.0f;
	private bool disableClicks  = false;
	IEnumerator lockClicks()
	{
		disableClicks = true;
		yield return new WaitForSeconds(0.4f);
		disableClicks = false;
	}

	IEnumerator WaitForRequest(WWW www)
	{

		yield return www;
		if (www.error == null){
			ParseXML p = new ParseXML(www.data);
			if(p.Name !="")
				debug = "\tName:\t" + p.Name + "\n\tType:\t" + p.Type + "\n\tState:\t" + p.State;
			else
				debug = "\tSent";
		}
		else{
			debug = "Not Found";
		}

	}

	int InitMod = -1;



	IEnumerator WaitForRequestAll(WWW www)
	{
		yield return www;
		if (www.error == null){
			if(Record){
				DateTime d = DateTime.Now;
				if(InitMod == -1){
					BeginDateTime = d;
					InitMod = RecordCount % RecordStep;
					System.IO.File.WriteAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/Save/"+ RecordName,"");

				}
				if(RecordCount % RecordStep == InitMod){
					System.IO.File.AppendAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/Save/"+ RecordName,"" + ((int)((d - BeginDateTime).TotalSeconds)) + "\t" + String.Format("{0:s}",d) + "\t" + www.data + "\n");
				}
			}
			AllSensorXML = ParseXML.MakeListData(www.data, true);
			foreach(Sensor s in StaticMemory.SensorList){
				s.UpdateValue(AllSensorXML);
			}
		}
		else{
			AllSensorXML.Clear();
		}
		www = null;
		getWWW = false;
	}

	bool getWWW = false;
	DateTime currentTime = DateTime.Now;

	void Update () {

		if(Input.GetKey(KeyCode.F12)){
			System.IO.File.WriteAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/Autosave.sen", Sensor.Export(StaticMemory.SensorList));
			Application.LoadLevel(0);
		}
		// Set the Yposition of the camera
		if (transform.localPosition.y != posCam)
			transform.localPosition = new Vector3 (transform.localPosition.x, posCam, transform.localPosition.z);
		
		if(MainAddSensor.displayGUI == 0 || MainAddSensor.displayGUI == 1 || MainAddSensor.displayGUI == 1.5 ){
			// Shift floor
			if (Input.GetKeyDown (KeyCode.Keypad8)) {
				posCam += 2.5f ;
			}
			
			if (Input.GetKeyDown (KeyCode.Keypad2)) {
				posCam -= 2.5f ;
			}
			
			if (Input.GetKeyDown (KeyCode.Keypad5)) {
				posCam = 4.03f ;
			}
			
			if(Input.GetKeyDown(KeyCode.M)){
				MainAddSensor.displayGUI = 1;
			}

			if(Input.GetKeyDown(KeyCode.Escape)){
				MainAddSensor.displayGUI = 0;
			}
			
			GetComponent<CharacterController> ().enabled = true;
		}
		else{
			
			if(Input.GetKeyDown(KeyCode.Escape) && MainAddSensor.displayGUI > 0){
				MainAddSensor.displayGUI = 1;
			}
			
			GetComponent<CharacterController> ().enabled = MainAddSensor.displayGUI == 1;
		}

		if(Replay){
			DoRecord = false;
			Record = false;
		}

		if(DoRecord){
			DateTime n = DateTime.Now;
			if(BeginDateTime <= n && n <= EndDateTime){
				GC.Collect();
				if(!Record){
					DoBeginRecord(BeginDateTime);
				}

				Record = true;

			}
			else{
				if(RecordCount < 0){
					DoEndRecord(DateTime.Now);
				}

			}
		}


		if(Replay){
			if(UseReplaySpeed){
				// prossess current ligne
				ReplayCurrentTime += RealReplaySpeed * Time.deltaTime;
				
				ReplayCurrentLine = ReplayData.Length-1;
				for(int i = 0; i < ReplayData.Length; i++){
					float t = float.Parse(ReplayData[i].Split('\t')[0]);
					if(ReplayCurrentTime <= t){
						ReplayCurrentLine = i;
						break;
					}
				}
			}
			
			AllSensorXML = ParseXML.MakeListData(ReplayData[ReplayCurrentLine].Split('\t')[2], false);
			foreach(Sensor s in StaticMemory.SensorList){
				s.UpdateValue(AllSensorXML);
			}
			
			
		}
		else{

			string urlget = StaticMemory.URL + "/rest/items/" + StaticMemory.RootNameItem;
			if(true || !getWWW){
				DateTime d = DateTime.Now;
				TimeSpan t = d - currentTime;
				if(t. TotalHours*3600000 > StaticMemory.Step * 1000f){
				
					WWW wwwget = new WWW(urlget);
					currentTime = d; 
					StartCoroutine(WaitForRequestAll(wwwget));
					getWWW = true;

					if(Record){
						if(RecordCount <= 0){
							Record = false;
						}
						RecordCount --;
					}
				}
			
			}
		}

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		bool HitRayCast = Physics.Raycast (ray, out hit, 1000);
		Debug.DrawLine (transform.position, hit.point, Color.red);
		GameObject hitGameObject;
		if (HitRayCast) {

			if(Input.GetMouseButtonUp(2)){
				if(CurrentItem != null){
					CurrentItem.AutoColor = false;

				}

				CurrentItem = (ObjSettingItem) hit.transform.gameObject.GetComponent<ObjSettingItem>();
				if (CurrentItem != null) {
					ColorPicker.SelectedColor = CurrentItem.Color;
					ColorPicker.TempColor = CurrentItem.Color;
					ColorPicker.OpenCondition = true;
					CurrentItem.AutoColor = true;
				}
				else{
					ColorPicker.OpenCondition = false;
				}

			}
				
				
				
				
				
				bool ItemHit = hit.transform.name.Contains("Hit");
			bool ItemSend = hit.transform.name.Contains("Send") && Input.GetMouseButtonDown(0);

			if(ItemHit || ItemSend){
				string sHit = hit.transform.name.Split('\\')[2].Split('#')[1];
				string sSend= PString.RemplaceSpecialChar(hit.transform.name.Split('\\')[hit.transform.name.Split('\\').Length-2].Split('#')[1],false);
				GameObject g = hit.transform.gameObject;
				while(g.name != "Device"){
					try{
						g = g.transform.parent.gameObject;
					}
					catch{
						break;
					}
				}
				try{
					SensorUpdator sU = (SensorUpdator) g.GetComponent(typeof(SensorUpdator));
					if(ItemHit){
						sU.Root.UpdateCurrentFunction(sHit);
						sU.CountHover = 15;
					}
					if(ItemSend){
						Debug.Log(ItemHit + "<>" + ItemSend);
						foreach (string s in sSend.Split('\n')){
							int i = int.Parse(s.Split('>')[0]);
							string m = s.Split('>')[1];
							m = m.Replace("[#Name]",hit.transform.name.Split('\\')[0].Replace(" ",""));
							foreach(ParseXML xml in sU.SensorValues){
								if( i == 0){
									string url = StaticMemory.URL + "/rest/items/" + s;
									WWW www = new WWW(xml.Link, System.Text.Encoding.UTF8.GetBytes (m), hashtable);
									StartCoroutine(WaitForRequest(www));
								}
								i--;
							}

						}
					}
				}
				catch{}
			}

		}



		// Get Double clic
		bool doubleclic = false;
		if ((MainAddSensor.displayGUI == 0 || MainAddSensor.displayGUI == 1) && Input.GetMouseButtonUp(0))
		{
			if (disableClicks)
				return;
			
			if (doubleClickStart > 0 && (Time.time - doubleClickStart) < 0.4)
			{
				doubleclic = true;
				doubleClickStart = -1;
				lockClicks();
			}
			else
			{
				doubleClickStart = Time.time;
			}
		}

		if((Input.GetMouseButton(0) & Input.GetMouseButtonDown(1)) || (Input.GetMouseButton(1) & Input.GetMouseButtonDown(0))){
			if (HitRayCast) {
				hit.transform.gameObject.SetActive(false);
				noVisible.AddLast(hit.transform.gameObject);
			}

		}

		if(Input.GetKeyDown(KeyCode.F8)){
			noVisible.Last.Value.SetActive(true);
			noVisible.RemoveLast();
		}



		if (doubleclic) {

			///TODO STUFF
			debug = "";
			if (HitRayCast) {
				MainAddSensor.displayGUI = -1;

				// An other device found

				if(hit.transform.name.Contains("Item")){
					Transform t = hit.transform;
					while(t.name != "Device"){
						try{
							t = t.transform.parent;
						}
						catch{
							break;
						}
					}
					try{
						_Current = ((SensorScript) t.gameObject.GetComponent(typeof(SensorScript))).Sensor;
						t.transform.GetComponent<Renderer>().enabled = true;
						AddNewSensorLevel = 1;
						GUILevel = 3;
					}
					catch{}
					

				}
				else{
					CurrentItem = (ObjSettingItem) hit.transform.gameObject.GetComponent<ObjSettingItem>();
					if (item != null) {
						GUILevel = 0;
						AddNewSensorLevel = 1;
						
						
						_Current = new Sensor();
						_Current.SensorObject.name = "Device";
						_Current.ID = _Maxid;
						_Current.SensorObject.transform.parent = CurrentItem.Root.gameObject.transform;
						_Current.OBJ = CurrentItem.Root.gameObject.name;
						_Current.ObjParentItem = CurrentItem;
						_Maxid++;
						
						isInList = false;
						_Current.SensorObject.GetComponent<Renderer>().material.color = Color.red;
						_Current.SensorObject.transform.position = hit.point;
						_Current.SensorNormal = hit.normal;
						
						AddNewFunction = "";
                		AddNewID = "";
						CurrentID = -1;

					}
				}
			}
		}
	}


	// Display GUI
	void OnGUI ()
	{
		int currentY;
		int largeur;
		int size = 0;
	//	GUI.Label(new Rect(10,10,200,30), debug);
		
		if (MainAddSensor.displayGUI == 0) {
			largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
			if(GUI.Button(new Rect (StaticMemory._Margin, Screen.height -StaticMemory._hauteurChamp * 2 -  StaticMemory._Margin, largeur, StaticMemory._hauteurChamp * 2), "Menu")){
				MainAddSensor.displayGUI = 1;
			}
			largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
			if(GUI.Button(new Rect (StaticMemory._Margin, Screen.height -StaticMemory._hauteurChamp * 5 -  StaticMemory._Margin, largeur, StaticMemory._hauteurChamp * 2), "Time")){
				MainAddSensor.displayGUI = -2;
			}
		}
		//GUI.BeginGroup(new Rect (0,0,2* StaticMemory._Margin + StaticMemory._SizeX, 3 * StaticMemory._Margin + StaticMemory._SizeX));

		if (MainAddSensor.displayGUI == -2) {
			GUIDisplayTime();
		}
			
		if (MainAddSensor.displayGUI == 1) {
			GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX , StaticMemory._SizeY));
			
		




			// 1ere collone
			GUI.BeginGroup (new Rect(0, 0, StaticMemory._SizeX / 4, StaticMemory._SizeY));
			GUI.Box (new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY), "");
			largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
			currentY = 0;
			
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Main Menu");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			
			GUI.DrawTexture(new Rect(0,currentY, largeur + 2 * StaticMemory._Margin, (largeur + 2 * StaticMemory._Margin)/3), StaticMemory.A4HTexture, ScaleMode.StretchToFill, true, 0);
			currentY += (largeur + 2 * StaticMemory._Margin)/3  + StaticMemory._hauteurEntre ;

			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Time")){
				MainAddSensor.displayGUI = -2;
			}
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 3;

			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Model(s) Parameters")){
				MainAddSensor.displayGUI = 2;
			}
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre;

			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Display Setting")){
				MainAddSensor.displayGUI = 3;
			}
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre;
			
			GUI.enabled = false;
			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "OpenHab parameters settings")){
				MainAddSensor.displayGUI = 4;
			}
			GUI.enabled = true;
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 3;
			
			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Save Sensor Configuration")){
				System.IO.File.WriteAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/Autosave.sen", Sensor.Export(StaticMemory.SensorList));
			}
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre;
			
			
			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Save Model(s) Colors")){
				for (int i = 0; i < StaticMemory.ArrayofOBJ.Length; i++){
					ObjSettings os = (ObjSettings) MainAddSensor.objModel[i].GetComponent(typeof(ObjSettings));
					System.IO.File.WriteAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/" +  os.name.Split('.')[0] + ".obSet",os.root.Export(""));

				}
			}
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre * 3;
			
			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Back to home page")){
				Application.LoadLevel(0);
			}
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre;

			if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Quit")){
				Application.Quit();
			}
			currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre;

		
			GUI.EndGroup();
			GUI.EndGroup();
		
		}

		if (MainAddSensor.displayGUI == -1) {
			/////////////////////////////////////AFFICHAGE
			GUIDoubleClic();

		}
	}


	public bool DoRecord = false;
	public bool Record = false;
	
	public bool ReplayInit = false;
	public bool Replay = false;

	private bool GUIRecord = false;
	private bool GUIReplay = false;
	
	private LinkedList<string> RecordData = new LinkedList<string> ();
	private string[] ReplayData;
	private string RecordName = "AutoSave";
	private string ReplayName = "AutoSave";
	private string ReplayInitName = "";

	private int RecordCount = 20;
	private string StepTime = "1";
	private int RecordStep = 1;
		private string BeginTime = "-1";
		private DateTime BeginDateTime = DateTime.Now;
	private DateTime EndDateTime = DateTime.Now;
	private string EndTime = "-1";

	private bool RecordReplay = true;

	private string ReplayStepTime = "1";
	private int ReplayStep = 1;
	private string ReplayBeginTime = "-1";
	private DateTime ReplayBeginDateTime = DateTime.Now;
	private DateTime ReplayEndDateTime = DateTime.Now;
	private string ReplayEndTime = "-1";
	
	private string ReplaySpeed = "1";
	private bool UseReplaySpeed = true;
	private float RealReplaySpeed = 1;
	private float ReplayCurrentTime = 0;
	private int ReplayCurrentLine = 1;
	
	//private bool RecordReplay = true;



	private void GUIDisplayTime(){
		GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX , StaticMemory._SizeY));

		// 1ere collone
		GUI.BeginGroup (new Rect(0, 0, StaticMemory._SizeX / 3, StaticMemory._SizeY));
		GUI.Box (new Rect(0,0, StaticMemory._SizeX / 3, StaticMemory._SizeY), "");
		int largeur  = StaticMemory._SizeX / 3 - 2 * StaticMemory._Margin;
		int currentY = 0;
		
		GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), String.Format("{0:u}",DateTime.Now).Replace("Z",""));
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		
		GUI.DrawTexture(new Rect(largeur/4,currentY, largeur/2 + 2 * StaticMemory._Margin, (largeur/2 + 2 * StaticMemory._Margin)/3), StaticMemory.A4HTexture, ScaleMode.StretchToFill, true, 0);
		currentY += (largeur/2 + 2 * StaticMemory._Margin)/3 ;

		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

		if(GUIRecord){
			if(GUI.Button(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Record")){
				GUIRecord = false;
			}
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			if(BeginTime == "-1"){
				BeginTime = String.Format("{0:u}",DateTime.Now).Replace("Z","");
				EndTime = String.Format("{0:u}",DateTime.Now).Replace("Z","");
			}
			if(BeginTime == ""){
				BeginTime = String.Format("{0:u}",DateTime.Now).Replace("Z","");
			}
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/3, StaticMemory._hauteurChamp)," Name:");
			if(GUI.Button(new Rect (StaticMemory._Margin + largeur - 2 * StaticMemory._hauteurChamp, currentY, 2 * StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "Now")){
				RecordName = String.Format("{0:u}",DateTime.Now).Replace("Z","").Replace(":","");
			}
			if(DoRecord){
				GUI.Label(new Rect(StaticMemory._Margin + largeur/3, currentY, 2 * largeur/3- 2 * StaticMemory._hauteurChamp, StaticMemory._hauteurChamp),RecordName);
			}
			else{
				RecordName = GUI.TextField(new Rect(StaticMemory._Margin + largeur/3, currentY, 2 * largeur/3- 2 * StaticMemory._hauteurChamp, StaticMemory._hauteurChamp),RecordName);
			}
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			
			
			
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/3, StaticMemory._hauteurChamp)," Begin:");
			if(DoRecord){
				GUI.Label(new Rect(StaticMemory._Margin + largeur/3, currentY, 2 * largeur/3, StaticMemory._hauteurChamp),BeginTime);
			}
			else{
				BeginTime = GUI.TextField(new Rect(StaticMemory._Margin + largeur/3, currentY, 2 * largeur/3, StaticMemory._hauteurChamp),BeginTime);
			}
			currentY += StaticMemory._hauteurChamp;
			
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/3, StaticMemory._hauteurChamp)," End:");
			if(DoRecord){
				GUI.Label(new Rect(StaticMemory._Margin + largeur/3, currentY, 2 * largeur/3, StaticMemory._hauteurChamp),EndTime);
			}
			else{
				EndTime = GUI.TextField(new Rect(StaticMemory._Margin + largeur/3, currentY, 2 * largeur/3, StaticMemory._hauteurChamp),EndTime);
			}
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/3, StaticMemory._hauteurChamp)," Step (s):");
			if(DoRecord){
				GUI.Label(new Rect(StaticMemory._Margin + largeur/3, currentY, 2 * largeur/3, StaticMemory._hauteurChamp),StepTime);
			}
			else{
				StepTime = GUI.TextField(new Rect(StaticMemory._Margin + largeur/3, currentY, 2 * largeur/3, StaticMemory._hauteurChamp),StepTime);
			}
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			
			if(!DoRecord){
				if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), "Record")){
					try{
						RecordStep = int.Parse(StepTime);
						if(RecordStep > 0){
							BeginDateTime = DateTime.Parse(BeginTime);
							EndDateTime = DateTime.Parse(EndTime);
			
							if (BeginDateTime < DateTime.Now){
								BeginTime = "";
								BeginDateTime = DateTime.Now;
							}
			
							TimeSpan span = (DateTime.Parse(EndTime) - DateTime.Parse(BeginTime));
							RecordCount = (int) span.TotalSeconds + RecordStep;
							Debug.Log(RecordCount);
							DoRecord = RecordCount > RecordStep;
						
			
						}
						else{
							StepTime = "1";
						}
					}
					catch{
						StepTime = "1";
					}
				}
			}
			else{
				if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), "Stop")){
					DoRecord = false;
					if(Record){
						DoEndRecord(DateTime.Now);
					}
				}
			}
			currentY += StaticMemory._hauteurChamp + 2*StaticMemory._hauteurEntre;
		}
		else{
			if(GUI.Button(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Record")){
				GUIRecord = true;
				GUIReplay = false;
			}
			currentY += StaticMemory._hauteurChamp + 2*StaticMemory._hauteurEntre;


		}

		if(GUIReplay){
			if(GUI.Button(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Replay")){
				GUIReplay = false;
			}
		
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/3, StaticMemory._hauteurChamp)," Name:");

			ReplayName = GUI.TextArea(new Rect(StaticMemory._Margin + largeur/3, currentY, 2 * largeur/3- 2 * StaticMemory._hauteurChamp, StaticMemory._hauteurChamp),ReplayName);

			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp)," Name:");
			if(GUI.Button(new Rect (StaticMemory._Margin + largeur - 2 * StaticMemory._hauteurChamp, currentY, 2 * StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), ">") || RecordName.Contains("\n")){
			try{
					RecordName = RecordName.Split('\n')[0];
					ReplayData = System.IO.File.ReadAllLines(StaticMemory.CurrentPreSelectedBuildingPath + "/Save/"+ ReplayName);
					ReplayBeginTime = ReplayData[0].Split('\t')[1];
					ReplayBeginDateTime = DateTime.Parse(ReplayBeginTime);
					ReplayEndTime = ReplayData[ReplayData.Length -1].Split('\t')[1];
					ReplayEndDateTime = DateTime.Parse(ReplayEndTime);
					ReplayInitName = ReplayName;

					minValue = 0;
					maxValue = ReplayData.Length -1;
					ReplayCurrentLine = 0;

					ReplayInit = true;
					Replay = true;
					UseReplaySpeed = false;
					RealReplaySpeed = 0;
				}
				catch{
					ReplayInit = false;
				}

			}
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;


			if(ReplayInit){
				int tempLine = ReplayCurrentLine;
				int i = int.Parse(ReplayData[2].Split('\t')[0]) - int.Parse(ReplayData[1].Split('\t')[0]);
				string s = " Name:  " + ReplayInitName + "\n" + 
						"\tBegin:  \t" + ReplayBeginTime.Replace("T","  ") + "\n" +
						"\tEnd:    \t" + ReplayEndTime.Replace("T","  ") + "\n" +
						"\tCount:  \t" + ReplayData.Length + "\t\t" +
						"\tStep:   \t" + i;

				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp*3),s);
				currentY += StaticMemory._hauteurChamp*3 + StaticMemory._hauteurEntre;

				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre/2;


				GUILayout.BeginArea(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp));
				MyGUI.MinMaxSlider(ref minValue, ref maxValue, -5, ReplayData.Length+5);
				GUILayout.EndArea();
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

				minValue = Math.Max (0,(int) minValue);
				maxValue = Math.Min (ReplayData.Length -1,(int) maxValue);

				GUI.Label(new Rect(10 + 0*largeur/4, currentY, largeur/4, StaticMemory._hauteurChamp*2),ReplayData[(int) minValue].Split('\t')[1].Replace("T","\n  "));
				GUI.Label(new Rect(StaticMemory._Margin*2 -10 + 3*largeur/4, currentY, largeur/4, StaticMemory._hauteurChamp*2),ReplayData[(int) maxValue].Split('\t')[1].Replace("T","\n  "));
				GUI.Box(new Rect(StaticMemory._Margin + largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp*7/4),ReplayData[ReplayCurrentLine].Split('\t')[1].Replace("T","\n"));
				currentY += StaticMemory._hauteurChamp*2;
				ReplayCurrentLine = (int) GUI.HorizontalScrollbar(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), ReplayCurrentLine, 1F, minValue, maxValue + 1);
				currentY += StaticMemory._hauteurChamp;

				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp)," Speed: ");
				currentY += StaticMemory._hauteurChamp;
				ReplaySpeed = GUI.TextField(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp),ReplaySpeed);
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;



				// Line off Buttom : <<, <, ||, > & >> 
				if(GUI.Button(new Rect(StaticMemory._Margin + 3*largeur/15, currentY, largeur/15, StaticMemory._hauteurChamp),"<<")){
					ReplayCurrentLine = (int) minValue;
				}
				if(GUI.Button(new Rect(StaticMemory._Margin + 5*largeur/15, currentY, largeur/15, StaticMemory._hauteurChamp),"-")){
					RealReplaySpeed -= 1;
				}
				if(!UseReplaySpeed){
					if(GUI.Button(new Rect(StaticMemory._Margin + 6*largeur/15, currentY, 3*largeur/15, StaticMemory._hauteurChamp), "Play")){
						UseReplaySpeed = true;
						RealReplaySpeed = int.Parse(ReplaySpeed);
					}
				}
				else{
					ReplaySpeed = "" + RealReplaySpeed;
					if(GUI.Button(new Rect(StaticMemory._Margin + 6*largeur/15, currentY, 3*largeur/15, StaticMemory._hauteurChamp),"||")){
						UseReplaySpeed = false;
						RealReplaySpeed = 0;
					}
				}
				if(GUI.Button(new Rect(StaticMemory._Margin + 9*largeur/15, currentY, largeur/15, StaticMemory._hauteurChamp),"+")){
					RealReplaySpeed += 1;
				}
				if(GUI.Button(new Rect(StaticMemory._Margin + 11*largeur/15, currentY, largeur/15, StaticMemory._hauteurChamp),">>")){
					ReplayCurrentLine = (int) maxValue;
				}

				ReplayCurrentLine = Math.Max (0, ReplayCurrentLine);
				ReplayCurrentLine = Math.Min (ReplayData.Length -1, ReplayCurrentLine);


				if(ReplayCurrentLine != tempLine){
					ReplayCurrentTime = float.Parse(ReplayData[ReplayCurrentLine].Split('\t')[0]);



				}


			}
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		}
		else{
			if(GUI.Button(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Replay")){
				GUIRecord = false;
				GUIReplay = true;
			}
			currentY += StaticMemory._hauteurChamp + 2*StaticMemory._hauteurEntre;
			
			
		}
		
		currentY += StaticMemory._hauteurChamp  + StaticMemory._hauteurEntre;
		if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 1), "Menu")){
			MainAddSensor.displayGUI = 1;
		}
		currentY += StaticMemory._hauteurChamp * 1  + StaticMemory._hauteurEntre * 1;
		
		if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 1), "Back") || Input.GetKey(KeyCode.Escape)){
			MainAddSensor.displayGUI = 0;
		}
		currentY += StaticMemory._hauteurChamp * 1  + StaticMemory._hauteurEntre * 1;
		
		
		GUI.EndGroup();
		GUI.EndGroup();
		
	}

	public float minValue = 1;
	public float maxValue = 2;


	private void DoBeginRecord(DateTime n){
		Debug.Log("##Begin!");
		try{
			System.IO.Directory.CreateDirectory(StaticMemory.CurrentPreSelectedBuildingPath + "/Save/");
			currentTime = DateTime.Today;
			Debug.Log("##Begin ");

		}
		catch{}

		RecordData.Clear();
		
	}
	private void DoEndRecord(DateTime n){
		Debug.Log("##End!");
		Record = false;
		DoRecord = false;

		string s = "";
		foreach(string str in RecordData){
			s += str + "\n";
		}
		//System.IO.File.WriteAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/Save/"+ RecordName ,s);	
	}



	private void CreateNewSensor(Sensor s){
		s.SensorDefault = SensorAndFunction.GetSensor(StaticMemory.SensorCfgList, s.SensorType, s.SensorName);
		s.Functions = SensorAndFunction.ExportCurrentList(new LinkedList<string>(), s.SensorDefault);

		try{
			GameObject.DestroyImmediate(s.SensorScript._SensorUpdator.Root.GObject);
			GameObject.DestroyImmediate(s.SensorScript._SensorUpdator);
			GameObject.DestroyImmediate(s.SensorScript);
			foreach(Transform child in s.SensorObject.transform) {
				Destroy(child);
			}

		}
		catch{}

		s.SensorScript = (SensorScript) s.SensorObject.AddComponent(typeof(SensorScript));
		s.SensorScript.Sensor = s;
		s.SensorScript._SensorUpdator = (SensorUpdator) s.SensorObject.AddComponent(typeof(SensorUpdator));;
		s.SensorScript._SensorUpdator.ObjParentItem = s.ObjParentItem;
		s.SensorScript.SensorInit = true;
		if(!StaticMemory.SensorList.Contains(s)){
			StaticMemory.SensorList.AddLast(s);
		}
	}

	private void DestroyCurrentSensor(Sensor s){
		s.IsActive = false;
		try{
			GameObject.DestroyImmediate(s.SensorScript._SensorUpdator.Root.GObject);
			GameObject.DestroyImmediate(s.SensorScript._SensorUpdator);
			GameObject.DestroyImmediate(s.SensorScript);
			foreach(Transform child in s.SensorObject.transform) {
				Destroy(child);
			}
			
		}
		catch{}

		GameObject.DestroyImmediate(s.SensorObject);
		StaticMemory.SensorList.Remove(s);
	}


	private int GUILevel = 0;
	private GUIScrollAuto scrollGeo = new GUIScrollAuto ();
	private GUIScrollAuto scrollSensor = new GUIScrollAuto ();
	private GUIScrollAuto scrollSensorList = new GUIScrollAuto ();
	private GUIScrollAuto scrollSensorSettings = new GUIScrollAuto ();
	private GUIScrollAuto scrollSensorDefaultSettings = new GUIScrollAuto ();
	private GUIScrollAuto scrollSensorVirtualSettings = new GUIScrollAuto ();



	private int[] item = {1,0};
	public int AddNewSensorLevel = 0; 

	private void GUIDoubleClic(){
		switch (AddNewSensorLevel){
		case 0:
			// Default : Menu
			GUIDoubleClicMenu();
			break;
		case 1:
			// Add New Sensor
			GUINewSensor();
			break;
		case 2:
			// Change Color;
			break;
		}
	}

	private void GUIDoubleClicMenu(){

		int currentY = 0;
		int largeur;
		
		largeur  = StaticMemory._SizeX/4 - 2 * StaticMemory._Margin;

		GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX/4 , StaticMemory._SizeY));
		GUI.Box (new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY), "");

		GUI.DrawTexture(new Rect(0,currentY, largeur + 2 * StaticMemory._Margin, (largeur + 2 * StaticMemory._Margin)/3), StaticMemory.A4HTexture, ScaleMode.StretchToFill, true, 0);
		currentY += (largeur + 2 * StaticMemory._Margin)/3  + StaticMemory._hauteurEntre;
	
		if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Add New Sensor")){
			AddNewSensorLevel = 1;
		}
		currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre;

		if(GUI.Button(new Rect (StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 2), "Edit current Object")){
			AddNewSensorLevel = 2;
		}
		currentY += StaticMemory._hauteurChamp * 2  + StaticMemory._hauteurEntre;
		GUI.EndGroup();

		
	}


	private void GUINewSensor(){

		int currentY = 0;
		int largeur;

		GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin, StaticMemory._SizeX, StaticMemory._Caption));
		GUI.DrawTexture(new Rect(StaticMemory._SizeX - 3*StaticMemory._Caption,0, StaticMemory._Caption*3, StaticMemory._Caption), StaticMemory.A4HTexture, ScaleMode.StretchToFill, true, 0);
		
		GUI.Box(new Rect(0,0, StaticMemory._SizeX, StaticMemory._Caption), "");
		currentY = 0;
		largeur  = StaticMemory._SizeX - 2 * StaticMemory._Margin;
		
		if(GUI.Button(new Rect(StaticMemory._Margin, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2,  StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), " < ") || Input.GetKey(KeyCode.Escape)){
			DestroyCurrentSensor(_Current);
			MainAddSensor.displayGUI = 0;
		}
		
		
		GUI.Box(new Rect(StaticMemory._Margin + StaticMemory._SizeX/4, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2, largeur/2, StaticMemory._hauteurChamp),"Sensor");
		
		
		GUI.EndGroup();
		
		GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX , StaticMemory._SizeY));


		switch (GUILevel){
		case 0:
			// Display Spatial Localisation
			largeur  = StaticMemory._SizeX / 2;
			GUI.BeginGroup (new Rect(0, 0, largeur, StaticMemory._SizeY));
			GUI.Box (new Rect(0,0, StaticMemory._SizeX / 2, StaticMemory._SizeY), "");
			largeur  -= 2 * StaticMemory._Margin;


			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Localisation");
			if(GUI.Button(new Rect(largeur - 3 * StaticMemory._Margin, currentY, 4 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Toggle")){
				showAllGeo = ! showAllGeo;
			}
			if(GUI.Button(new Rect(2*StaticMemory._Margin, currentY, StaticMemory._Margin, StaticMemory._hauteurChamp), ">") && _Current.Place != ""){
				GUILevel ++;
			}

			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			scrollGeo.Begin(currentY, StaticMemory._SizeX / 2, StaticMemory._SizeY - StaticMemory._hauteurEntre);
			if(showAllGeo){
				currentY = GUIGeoAll(largeur, currentY);
			}else{
				currentY = GUIGeoCurr(largeur, currentY);
			}
			scrollGeo.End(currentY);
			GUI.EndGroup();
			break;
		case 1:
			// Display Sensor sefault settings
			largeur  = StaticMemory._SizeX / 2;
			GUI.BeginGroup (new Rect(0, 0, largeur, StaticMemory._SizeY));
			GUI.Box (new Rect(0,0, StaticMemory._SizeX / 2, StaticMemory._SizeY), "");
			largeur  -= 2 * StaticMemory._Margin;
			
			
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Sensor Type");
			if(GUI.Button(new Rect(largeur - 3 * StaticMemory._Margin, currentY, 4 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Toggle")){
				showAllSensor = ! showAllSensor;
			}
			if(GUI.Button(new Rect(StaticMemory._Margin, currentY, StaticMemory._Margin, StaticMemory._hauteurChamp), "<")){
				GUILevel --;
			}
			if(GUI.Button(new Rect(2*StaticMemory._Margin, currentY, StaticMemory._Margin, StaticMemory._hauteurChamp), ">") && _Current.SensorName != ""){
				CreateNewSensor(_Current);
				GUILevel ++;
			}

			
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			
			scrollSensor.Begin(currentY, StaticMemory._SizeX / 2, StaticMemory._SizeY - StaticMemory._hauteurEntre);
			if(showAllSensor){
				currentY = GUISenAll(largeur, currentY);
			}else{
				currentY = GUISenCurr(largeur, currentY);
			}
			scrollSensor.End(currentY);
			GUI.EndGroup();

			largeur  = StaticMemory._SizeX / 2;
			GUI.BeginGroup (new Rect(StaticMemory._SizeX / 2, 0, largeur, StaticMemory._SizeY));
			GUI.Box (new Rect(0,0, StaticMemory._SizeX / 2, StaticMemory._SizeY), "");
			largeur  -= 2 * StaticMemory._Margin;
			currentY = 0;
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Sensor List");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			scrollSensorList.Begin(currentY, StaticMemory._SizeX / 2, StaticMemory._SizeY - StaticMemory._hauteurEntre);


			currentY = GUISenList(largeur, currentY);
			scrollSensorList.End(currentY);
			GUI.EndGroup();

			break;

		case 2:
			// Treatement :  Create new sensor


			GUILevel = 3;
			break;

		case 3:
			// Display : 
			largeur  = StaticMemory._SizeX - 2*StaticMemory._Margin;
			GUI.Box(new Rect(0, 0, StaticMemory._SizeX, StaticMemory._hauteurChamp), "");
			GUI.Box(new Rect(StaticMemory._Margin, 0, largeur, StaticMemory._hauteurChamp), "Sensor Settings");
			if(GUI.Button(new Rect(StaticMemory._Margin, currentY, StaticMemory._Margin, StaticMemory._hauteurChamp), "<")){
				GUILevel -= 2;
			}

			if (GUI.Button (new Rect(largeur - 14*StaticMemory._Margin, currentY, 3*StaticMemory._Margin, StaticMemory._hauteurChamp), "ESC") || Input.GetKey(KeyCode.Escape)) {
				MainAddSensor.displayGUI = 0;
				GUILevel = 0;
			}
			
			if (GUI.Button (new Rect(largeur - 11*StaticMemory._Margin, currentY, 3*StaticMemory._Margin, StaticMemory._hauteurChamp), "DEL")) {
				GameObject.DestroyImmediate(_Current.SensorObject);
				_Current.IsActive = false;
				MainAddSensor.displayGUI = 0;
			}
			

			if (GUI.Button (new Rect(largeur - 8*StaticMemory._Margin, currentY, 3*StaticMemory._Margin, StaticMemory._hauteurChamp), "Move")) {
				_Current.SensorScript.DoMove = true;
				//_Current.SensorScript.SensorHit.SetActive(false);
				MainAddSensor.displayGUI = 0;

			}

			
			if (GUI.Button (new Rect(largeur - 5*StaticMemory._Margin, currentY, 3*StaticMemory._Margin, StaticMemory._hauteurChamp), ">")) {
				_Current.SensorObject.GetComponent<Renderer>().enabled = false;
				GUILevel = 0;
				if(!isInList){
				}
				MainAddSensor.displayGUI = 0;
			}
			if (GUI.Button (new Rect(largeur - 2*StaticMemory._Margin, currentY, 3*StaticMemory._Margin, StaticMemory._hauteurChamp), "Save")) {
				_Current.SensorObject.GetComponent<Renderer>().enabled = false;
				System.IO.File.WriteAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/Autosave.sen", Sensor.Export(StaticMemory.SensorList));

			}




			for (int i = 0; i < item.Length; i++){

				largeur  = StaticMemory._SizeX / item.Length;
				GUI.BeginGroup (new Rect(i * StaticMemory._SizeX / item.Length, StaticMemory._hauteurChamp + StaticMemory._hauteurEntre, largeur, StaticMemory._SizeY - StaticMemory._hauteurEntre - StaticMemory._hauteurChamp));
				string[] Labels = {"List","Edit","Type","Virtual","None"};
				int max = Labels.Length;
				if(item[i] != max - 1){
					GUI.Box (new Rect(0,0, StaticMemory._SizeX / item.Length, StaticMemory._SizeY - StaticMemory._hauteurEntre - StaticMemory._hauteurChamp), "");
				}
				largeur  -= 2 * StaticMemory._Margin;
				currentY = 0;


				for(int j = 0; j< max; j++){

					if(item[i] == j){
						GUI.Box(new Rect(j*(largeur + 2 * StaticMemory._Margin)/max, currentY, (largeur + 2 * StaticMemory._Margin)/max, StaticMemory._hauteurChamp), "");
						GUI.Box(new Rect(j*(largeur + 2 * StaticMemory._Margin)/max, currentY, (largeur + 2 * StaticMemory._Margin)/max, StaticMemory._hauteurChamp), Labels[j]);
					
					}else{
							if(GUI.Button(new Rect(j*(largeur + 2 * StaticMemory._Margin)/max, currentY, (largeur + 2 * StaticMemory._Margin)/max, StaticMemory._hauteurChamp), Labels[j])){
							item[i] = j;
						}
					}
				}

				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;



				switch(item[i]){
				case 0:
					scrollSensorList.Begin(currentY, StaticMemory._SizeX / item.Length, StaticMemory._SizeY - StaticMemory._hauteurChamp - StaticMemory._hauteurEntre);
					currentY = GUISenList(largeur, currentY);
					scrollSensorList.End(currentY);
					break;
				case 1:
					scrollSensorSettings.Begin(currentY, StaticMemory._SizeX / item.Length, StaticMemory._SizeY - StaticMemory._hauteurChamp - StaticMemory._hauteurEntre);
					currentY = GUIFunction(largeur, currentY);
					scrollSensorSettings.End(currentY);
					break;
				case 2:
					scrollSensorDefaultSettings.Begin(currentY, StaticMemory._SizeX / item.Length, StaticMemory._SizeY - StaticMemory._hauteurChamp - StaticMemory._hauteurEntre);
					currentY = GUIDefaultSensor(largeur, currentY);
					scrollSensorDefaultSettings.End(currentY);
					break;

				case 3:
					scrollSensorVirtualSettings.Begin(currentY, StaticMemory._SizeX / item.Length, StaticMemory._SizeY - StaticMemory._hauteurChamp - StaticMemory._hauteurEntre);
					currentY = GUIVirtualSensor(largeur, currentY);
					scrollSensorVirtualSettings.End(currentY);
					break;

				}
				GUI.EndGroup();
			}

			break;

		}
		
		GUI.EndGroup();
	}

	private int GUIGeoAll(int largeur, int currentY){
		String[] buidList = Geoloc.GetAllBuildings(StaticMemory.RoomList);
		foreach(string sB in buidList){
			if(sB == ""){
				continue;
			}
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), sB);
			currentY += StaticMemory._hauteurChamp;

			String[] FloorList = Geoloc.GetAllFloors(StaticMemory.RoomList,sB);
			foreach(string sF in FloorList){
				if(sF == ""){
					continue;
				}

				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), "\t" + sF);
				currentY += StaticMemory._hauteurChamp ;

				String[] PlaceList = Geoloc.GetAllRooms(StaticMemory.RoomList,sB,sF);
				foreach(string sP in PlaceList){
					bool id = false;
					try{
						id = sB == _Current.Building && sF == _Current.Floor && sP == _Current.Place;
					}
					catch{}
					bool b = GUI.Toggle(new Rect(StaticMemory._Margin * 2, currentY, largeur, StaticMemory._hauteurChamp), id,"\t" + sP);
					if(b && !id){
						_Current.Building = sB;
						_Current.Floor = sF;
						_Current.Place = sP;
						GUILevel ++;
					}
					if(!b && id){
						_Current.Building = "";
						_Current.Floor = "";
						_Current.Place = "";
						
					}
					currentY += StaticMemory._hauteurChamp;
				}
				currentY += StaticMemory._hauteurEntre;

			}
			currentY += StaticMemory._hauteurEntre;
		}
		return currentY;

	}


	private int GUIGeoCurr(int largeur, int currentY){
		bool doFirst = false;
		if(_Current.Building == ""){
			String[] buidList = Geoloc.GetAllBuildings(StaticMemory.RoomList);
			if (buidList.Length == 1){
				doFirst = true;
			}
			foreach(string s in buidList){
				if(GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), false || doFirst,"\t" + s)){
					_Current.Building = s;
				}
				doFirst = false;
				currentY += StaticMemory._hauteurChamp;
			}
		}
		else{
			if(!GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), true,"\t" + _Current.Building)){
				_Current.Building = "";
			}

			currentY += StaticMemory._hauteurChamp + 2 * StaticMemory._hauteurEntre;
			
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp),"Floor:");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			if(_Current.Floor == ""){
				
				String[] FloorList = Geoloc.GetAllFloors(StaticMemory.RoomList,_Current.Building);
				if (FloorList.Length == 1){
					doFirst = true;
				}

				foreach(string s in FloorList){
					if(GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), false || doFirst,"\t" + s)){
						_Current.Floor = s;
					}
					doFirst = false;
					currentY += StaticMemory._hauteurChamp ;
				}
			}
			else{
				
				if(!GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), true,"\t" + _Current.Floor)){
					_Current.Floor = "";
				}
				currentY += StaticMemory._hauteurChamp + 2 * StaticMemory._hauteurEntre;
				
				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp),"Room:");
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				
				String[] PlaceList = Geoloc.GetAllRooms(StaticMemory.RoomList,_Current.Building,_Current.Floor);
				if (PlaceList.Length == 1){
					doFirst = true;
				}
				foreach(string s in PlaceList){
					bool b = GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), (s == _Current.Place) || doFirst,"\t" + s);
					if(b && s != _Current.Place){
						_Current.Place = s;
						GUILevel++;
					}
					doFirst = false;
					currentY += StaticMemory._hauteurChamp;
				}
			}
		}

		return currentY;
	}

	private int GUISenList(int largeur, int currentY){
		NameContains = GUI.TextField(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp),NameContains);
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		
		foreach(string s in _Current.NameID){
			foreach(ParseXML item in AllSensorXML){
				if(s == item.Name){
					if(!GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), true, "\t[" + item.Type.Replace("Item","") + "]   " + item.Name)){
						_Current.NameID.Remove(s);
					}
					currentY += StaticMemory._hauteurChamp;
				}
			}
		}
		currentY += StaticMemory._hauteurEntre;

		foreach(ParseXML item in AllSensorXML){
			if(item.Name.Contains(NameContains) || item.Type.Contains(NameContains)){
				if(!(_Current.NameID.Contains(item.Name))){
					if(GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), false,"\t[" + item.Type.Replace("Item","") + "]   " + item.Name)){
						_Current.NameID.AddLast(item.Name);
					}
					currentY += StaticMemory._hauteurChamp;
				}
					
			}
		}	
		return currentY;
	}


	private int GUISenCurr(int largeur, int currentY){
		bool doFirst = false;

		if(_Current.SensorType == ""){
			String[] TypeList = SensorAndFunction.GetAllTypes(StaticMemory.SensorCfgList);
			if (TypeList.Length == 1){
				doFirst = true;
			}
			foreach(string s in TypeList){
				if(GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), false || doFirst,"\t" + s)){
					_Current.SensorType = s;
				}
				doFirst = false;
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			}

		}
		else{
			if(!GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), true,"\t" + _Current.SensorType)){
				_Current.SensorType = "";
			}
			currentY += StaticMemory._hauteurChamp + 2 * StaticMemory._hauteurEntre;
			
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp),"Sensor Name:");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			
			String[] NameList = SensorAndFunction.GetAllNames(StaticMemory.SensorCfgList,_Current.SensorType);
			if (NameList.Length == 1){
				doFirst = true;
			}
			foreach(string s in NameList){
				bool b = GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), (s == _Current.SensorName) || doFirst,"\t" + s);
				if(b && s != _Current.SensorName){
					_Current.SensorName = s;
					GUILevel++;
				}
				doFirst = false;
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			}
		}
		return currentY;
	}

	private int GUISenAll(int largeur, int currentY){
		String[] TypeList = SensorAndFunction.GetAllTypes(StaticMemory.SensorCfgList);
		foreach(string sT in TypeList){
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), sT);
			currentY += StaticMemory._hauteurChamp;

			String[] NameList = SensorAndFunction.GetAllNames(StaticMemory.SensorCfgList,sT);
			foreach(string sN in NameList){
				bool id = _Current.SensorType == sT && _Current.SensorName == sN;
				bool b = GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), id,"\t" + sN);
				if(b && !id){
					_Current.SensorType = sT;
					_Current.SensorName = sN;
					CreateNewSensor(_Current);
					GUILevel++;
				}
				if(!b && id){
					_Current.SensorType = "";
					_Current.SensorName = "";
				}
				currentY += StaticMemory._hauteurChamp;
				
			}
		}
		return currentY;
	}

	private int GUIFunction(int largeur, int currentY){
		GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/8, StaticMemory._hauteurChamp),"Name");
		_Current.Name = GUI.TextField(new Rect(StaticMemory._Margin + largeur/8, currentY, 7*largeur/8, StaticMemory._hauteurChamp),_Current.Name);
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/8, StaticMemory._hauteurChamp),"ID");
		AddNewID = GUI.TextArea(new Rect(StaticMemory._Margin + largeur/8, currentY, 6*largeur/8, StaticMemory._hauteurChamp),AddNewID);
		if(GUI.Button(new Rect(StaticMemory._Margin + 7*largeur/8, currentY, largeur/8, StaticMemory._hauteurChamp),"Add") || AddNewID.Contains("\n")){
			_Current.NameID.AddLast(AddNewID.Split('\n')[0]);
			AddNewID = "";
		}
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;


		int cur = 0;
		foreach(string s in _Current.NameID){
			if(GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur - 2* StaticMemory._hauteurChamp, StaticMemory._hauteurChamp),CurrentID == cur, s)){
				CurrentID = cur;
			}
			if(GUI.Button(new Rect(StaticMemory._Margin + largeur - 2* StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp),"x")){
				_Current.NameID.Remove(s);
				cur = -1;
				currentY += StaticMemory._hauteurChamp;
				break;
			}
			currentY += StaticMemory._hauteurChamp;
			cur++;
		}

		currentY += StaticMemory._hauteurEntre;
		cur = 0;
		foreach(string s in _Current.NameID){
			if(CurrentID == cur){
				if(GUI.Button(new Rect(StaticMemory._Margin, currentY, largeur/2, StaticMemory._hauteurChamp),"Check")){
					string urlget = StaticMemory.URL + "/rest/items/" + s;
					WWW wwwget = new WWW(urlget);
					StartCoroutine(WaitForRequest(wwwget));
				}
				_Current.Command = GUI.TextArea(new Rect(StaticMemory._Margin + largeur/2, currentY, largeur/4, StaticMemory._hauteurChamp), _Current.Command);
				
				if(GUI.Button(new Rect(StaticMemory._Margin + 3 * largeur/4 , currentY, 1 * largeur/4, StaticMemory._hauteurChamp),"Send") || _Current.Command.Contains("\n")){
					string url = StaticMemory.URL + "/rest/items/" + s;
					WWW www = new WWW(url, System.Text.Encoding.UTF8.GetBytes (_Current.Command.Split('\n')[0]), hashtable);
					_Current.Command = "";
					StartCoroutine(WaitForRequest(www));
				}

				currentY += StaticMemory._hauteurChamp;
				
				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp*4), debug);
				currentY += StaticMemory._hauteurChamp*2 + StaticMemory._hauteurEntre *2;
				
				

			}
			cur++;
			
			
		}
		
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		GUI.Box (new Rect(StaticMemory._Margin,currentY, largeur, current), "");
		GUI.Box (new Rect(StaticMemory._Margin,currentY, largeur, StaticMemory._hauteurChamp), "Semantic grouping");
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		
		int curY1 = currentY;
		int curY2 = currentY;
		
		foreach(string s in SensorAndFunction.ListFunctions){
			if( _Current.Functions.Contains(s)){
				bool b = GUI.Toggle(new Rect(StaticMemory._Margin, curY1, largeur/2, StaticMemory._hauteurChamp), true, s);
				if(!b){
					_Current.Functions.Remove(s);
					Debug.Log("Remove");
				}
				curY1 +=StaticMemory._hauteurChamp;
			}
		}

		foreach(string s in SensorAndFunction.ListFunctions){
			if(! _Current.Functions.Contains(s)){
				if(GUI.Toggle(new Rect(StaticMemory._Margin + largeur/2, curY2, largeur/2, StaticMemory._hauteurChamp), false, s)){
					_Current.Functions.AddLast(s);	
					Debug.Log("Add");
					
				}
				curY2 +=StaticMemory._hauteurChamp;
			}
		}

		currentY = Math.Max(curY1,curY2) + StaticMemory._hauteurEntre;
		current = currentY;
		AddNewFunction = GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur/2, StaticMemory._hauteurChamp),AddNewFunction);
		if(GUI.Button(new Rect (StaticMemory._Margin + largeur/2, currentY, largeur/6, StaticMemory._hauteurChamp), "Add") || AddNewFunction.Contains("\n")){
			AddNewFunction = AddNewFunction.Split('\n')[0];
			SensorAndFunction.AddFunctionAndDoUpdate(new LinkedList<SensorAndFunction>(),AddNewFunction);
			_Current.Functions.AddLast(AddNewFunction);
			AddNewFunction = "";
			
		}
		if(GUI.Button(new Rect (StaticMemory._Margin + 4 * largeur/6, currentY, largeur/6, StaticMemory._hauteurChamp), "Create")){
			SensorAndFunction.AddFunctionAndDoUpdate(new LinkedList<SensorAndFunction>(),AddNewFunction);
			AddNewFunction = "";
		}
		if(GUI.Button(new Rect (StaticMemory._Margin + 5 * largeur/6, currentY, largeur/6, StaticMemory._hauteurChamp), "Delete")){
			SensorAndFunction.DelFunctionAndDoUpdate(new LinkedList<SensorAndFunction>(),AddNewFunction);
			AddNewFunction = "";
		}
		
		currentY += StaticMemory._hauteurChamp;
		return currentY;
	}

	private int GUIDefaultSensor(int largeur, int currentY){

		GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Type");
		GUI.Label(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp), "\t" + _Current.SensorDefault.Type);
		
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Name");
		GUI.Label(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp), "\t" + _Current.SensorDefault.SensorName);
		
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Constructor");
		
		GUI.Label(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp),"\t" +  _Current.SensorDefault.Constructor);
		
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		
		
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), "Comments");
		currentY += StaticMemory._hauteurChamp;
		GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 7), _Current.SensorDefault.Comments);
		currentY += StaticMemory._hauteurChamp * 7 + StaticMemory._hauteurEntre;
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		
		int[] posX = {
			0,
			1 * largeur/3 - StaticMemory._hauteurEntre,
			largeur/2 + 0 * largeur/9 ,
			largeur/2 + 1 * largeur/9 ,
			largeur/2 + 2 * largeur/9 + StaticMemory._hauteurEntre
		};
		int[] SizeX = {
			largeur/3 - 2 * StaticMemory._hauteurEntre,
			largeur/6,
			largeur/9,
			largeur/9,
			largeur/9
		}; 
		
		
		
		GUI.Box(new Rect(StaticMemory._Margin + posX[0], currentY, SizeX[0], StaticMemory._hauteurChamp), "Name");
		GUI.Box(new Rect(StaticMemory._Margin + posX[1], currentY, SizeX[1], StaticMemory._hauteurChamp), "Unit");
		GUI.Box(new Rect(StaticMemory._Margin + posX[2], currentY, SizeX[2], StaticMemory._hauteurChamp), "Min");
		GUI.Box(new Rect(StaticMemory._Margin + posX[3], currentY, SizeX[3], StaticMemory._hauteurChamp), "Max");
		GUI.Box(new Rect(StaticMemory._Margin + posX[4], currentY, SizeX[4], StaticMemory._hauteurChamp), "Prec.");
		currentY += StaticMemory._hauteurChamp;
		
		LinkedList<Data> dList = new LinkedList<Data> ();
		foreach(Data d in _Current.SensorDefault.Datas){
			Data temp = new Data();
			temp.Name = GUI.TextField(new Rect(StaticMemory._Margin + posX[0], currentY, SizeX[0], StaticMemory._hauteurChamp), d.Name);
			temp.Unit = GUI.TextField(new Rect(StaticMemory._Margin + posX[1], currentY, SizeX[1], StaticMemory._hauteurChamp), d.Unit);
			temp.UseMin = GUI.TextField(new Rect(StaticMemory._Margin + posX[2], currentY, SizeX[2], StaticMemory._hauteurChamp), d.UseMin);
			temp.UseMax = GUI.TextField(new Rect(StaticMemory._Margin + posX[3], currentY, SizeX[3], StaticMemory._hauteurChamp), d.UseMax);
			temp.Precision = GUI.TextField(new Rect(StaticMemory._Margin + posX[4], currentY, SizeX[4], StaticMemory._hauteurChamp), d.Precision);
			
			
			
			dList.AddLast(temp);
			currentY += StaticMemory._hauteurChamp;
			
			
		}
		_Current.SensorDefault.Datas = dList;
		
		currentY = StaticMemory._SizeY -  2 * (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre) ;
		return currentY;

	}
	//Vector2 Scroll1 = Vector2.zero;

	private int GUIVirtualSensor(int largeur, int currentY){
		LinkedList<SensorUpdator.KeyValue> TempList = new LinkedList<SensorUpdator.KeyValue> ();
		foreach(SensorUpdator.KeyValue k in _Current.SensorScript._SensorUpdator.KeyValuesMakeList){
			SensorUpdator.KeyValue k2 = new SensorUpdator.KeyValue(k.Key,k.Value); 
			if(k2.Key == "DELETED"){
				continue;
			}
			k2.Key = k.Key;
			GUI.Label(new Rect(StaticMemory._Margin + 0*largeur/4, currentY, 1*largeur/4 - 0*StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), k2.Key.Replace("[","").Replace("]",""));
			k2.Value = GUI.TextField(new Rect(StaticMemory._Margin + 1*largeur/4, currentY, 3*largeur/4 - 1*StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), k2.Value);

			currentY += StaticMemory._hauteurChamp;
			
			TempList.AddLast(k2);
		}
		_Current.SensorScript._SensorUpdator.KeyValuesMakeList.Clear();
		foreach(SensorUpdator.KeyValue k in TempList){
			_Current.SensorScript._SensorUpdator.KeyValuesMakeList.AddLast(k);
		}
		
		currentY += StaticMemory._hauteurEntre;
		currentY += StaticMemory._hauteurEntre;

		
		LinkedList<SensorUpdator.KeyValue> TempListRoot = new LinkedList<SensorUpdator.KeyValue> ();
		foreach(SensorUpdator.KeyValue k in _Current.SensorScript._SensorUpdator.KeyValuesRootList){
			if(!k.Key.Contains("##")){
				TempListRoot.AddLast(k);
				Debug.Log(k.Key);
				continue;
			}
			SensorUpdator.KeyValue k2 = new SensorUpdator.KeyValue(k.Key,k.Value); 
			if(k2.Key == "DELETED"){
				continue;
			}
			string[] val = k2.Value.Split('{')[1].Split('}')[0].Split(';');
			
			k2.Key = k.Key;
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur - StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), k2.Key.Replace("[","").Replace("]",""));

			currentY += StaticMemory._hauteurChamp;
			currentY += StaticMemory._hauteurEntre;
			//k2.Id = GUI.TextField(new Rect(StaticMemory._Margin + 2*largeur/5, currentY, largeur/5, StaticMemory._hauteurChamp),k.Id);
			
			
			//currentY += StaticMemory._hauteurChamp;
			k2.Value = "{";
			
			int max = 0;
			for(int i = 0; i<val.Length; i++){
				max = Math.Max(max,val[i].Split(',').Length);
			}
			string newline = "";
			for (int t = 0; t< max -1; t++){
				newline += ",";
			}
			
			int l = Math.Min(max, 8);
			
			
			k2.Scale = GUI.BeginScrollView(
				new Rect(0,currentY, largeur + StaticMemory._Margin*2,StaticMemory._hauteurChamp*(val.Length+2)),
				k.Scale, 
				new Rect(0,currentY, 30 + (max)* (largeur- 30) / l + StaticMemory._Margin*2 -17,StaticMemory._hauteurChamp*(val.Length+2-17)),
				false, false
				);
			
			
			for (int i = 0; i<max;i++){
				GUI.Label(new Rect(StaticMemory._Margin  + 30 + (i)* (largeur- 30) / l,
				                   currentY,
				                   (largeur- 30) / l,
				                   StaticMemory._hauteurChamp), "   " + i);
				
				
			}
			currentY += StaticMemory._hauteurChamp;
			
			
			for(int i = 0; i<val.Length; i++){
				string[] subVal = val[i].Split(',');
				
				
				
				if( i != 0){
					k2.Value += ";";
				}
				GUI.Label(new Rect(StaticMemory._Margin ,
				                   currentY,
				                   30,
				                   StaticMemory._hauteurChamp),"" + i);
				for(int j = 0;  j<subVal.Length; j++){
					

					subVal[j] = GUI.TextField(new Rect(StaticMemory._Margin  + 30 + (j)* (largeur- 30) / l,
					                                   currentY,
					                                   (largeur- 30) / l,
					                                   StaticMemory._hauteurChamp), subVal[j]).Replace(";","").Replace(",","").Replace("}","").Replace("{","");
					

					
					if(j != 0){
						k2.Value += "," + subVal[j];
					}
					else{
						k2.Value += subVal[j];
					}
					
				}

				currentY += StaticMemory._hauteurChamp;
				
			}
			GUI.EndScrollView();
			currentY += StaticMemory._hauteurChamp;
			
			k2.Value += "}";
			TempListRoot.AddLast(k2);
			
			currentY += StaticMemory._hauteurEntre;
			currentY += StaticMemory._hauteurEntre;
			
			
		}
		_Current.SensorScript._SensorUpdator.KeyValuesRootList.Clear();
		foreach(SensorUpdator.KeyValue k in TempListRoot){
			_Current.SensorScript._SensorUpdator.KeyValuesRootList.AddLast(k);
		}

		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;


		return currentY;

	}

}
