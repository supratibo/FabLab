using UnityEngine;
using System.Collections;
using AssemblyCSharpfirstpass;

public class LaunchLoadObj : MonoBehaviour {
	public GameObject _this;

	private Vector2 Scroll1 = Vector2.zero;
	private Vector2 Scroll2 = Vector2.zero;



	// Use this for initialization
	void Awake () {
		MainAddSensor.isDisplayed = new bool[StaticMemory.ArrayofOBJ.Length];
		if(StaticMemory.ArrayofOBJ.Length> 0){
			MainAddSensor.isDisplayed[0] = true;
		}
		MainAddSensor.objModel = new GameObject[StaticMemory.ArrayofOBJ.Length];

		bool isactive = true;

		for (int i = 0; i < StaticMemory.ArrayofOBJ.Length; i++){
			GameObject g = new GameObject();
			g.name = StaticMemory.ArrayofOBJ[i];
			MainAddSensor.objModel[i] = g;

			if (!StaticMemory.ArrayofOBJBool[i]){
				continue;
			}
			g.transform.parent = _this.transform;
			g.transform.localScale = Vector3.one;
			g.transform.localPosition = Vector3.zero;
			g.transform.localRotation = Quaternion.identity;



			OBJ obj = (OBJ) g.AddComponent(typeof(OBJ));
			obj.objPath = StaticMemory.CurrentPreSelectedBuildingPath + "/" + StaticMemory.ArrayofOBJ[i];
			obj.DoRun();

			ObjSettings os = (ObjSettings) g.AddComponent(typeof(ObjSettings));
			//os.root.GO = g;
			os.DoRun();

			if(isactive){
				isactive = false;
			}
			else {
				g.SetActive(false); 
			}
			try{
				string[] s = System.IO.File.ReadAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/" +  StaticMemory.ArrayofOBJ[i].Split('.')[0] + ".oSet").Split(';');
				g.transform.localPosition = new Vector3 (float.Parse(s[0]),float.Parse(s[1]),float.Parse(s[2]));
				g.transform.localRotation = new Quaternion(float.Parse(s[3]),float.Parse(s[4]),float.Parse(s[5]),float.Parse(s[6]));
				g.transform.localScale = new Vector3 (float.Parse(s[7]),float.Parse(s[8]),float.Parse(s[9]));
				
			}
			catch{
			}
		}
	
	}
	
	// Update is called once per frame
	void Start () {
		for (int i = 0; i < StaticMemory.ArrayofOBJ.Length; i++){
			try{
				string[] s = System.IO.File.ReadAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/" +  StaticMemory.ArrayofOBJ[i].Split('.')[0] + ".oSet").Split(';');
				MainAddSensor.objModel[i].transform.localPosition = new Vector3 (float.Parse(s[0]),float.Parse(s[1]),float.Parse(s[2]));
				MainAddSensor.objModel[i].transform.localRotation = new Quaternion(float.Parse(s[3]),float.Parse(s[4]),float.Parse(s[5]),float.Parse(s[6]));
				MainAddSensor.objModel[i].transform.localScale = new Vector3 (float.Parse(s[7]),float.Parse(s[8]),float.Parse(s[9]));
			}
			catch{
				
			}
		}
	}

	public bool debug = false;
	void Update(){
		if(debug){
			foreach (Transform t in MainAddSensor.objModel[0].transform){
				Debug.Log(t.name);
			}
			debug = false;
		}
	}

	private int currentGUI = -1;
	private string SweetHomeHeight = "250";
	private string floor= "";

	void OnGUI(){
		if(MainAddSensor.displayGUI == 2){
			int currentY;
			int largeur;
			int size = 0;

			
			GUI.BeginGroup(new Rect (0,0,2* StaticMemory._Margin + StaticMemory._SizeX, 3 * StaticMemory._Margin + StaticMemory._SizeX));
			
			
			/////////////////////////////////////AFFICHAGE
			// Groupe : Caption
			GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin, StaticMemory._SizeX, StaticMemory._Caption));
			GUI.DrawTexture(new Rect(StaticMemory._SizeX - 3*StaticMemory._Caption,0, StaticMemory._Caption*3, StaticMemory._Caption), StaticMemory.A4HTexture, ScaleMode.StretchToFill, true, 0);
			GUI.Box(new Rect(0,0, StaticMemory._SizeX, StaticMemory._Caption), "");
			currentY = 0;
			largeur  = StaticMemory._SizeX - 2 * StaticMemory._Margin;
			
			if(GUI.Button(new Rect(StaticMemory._Margin, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2,  StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), " < ")){
				MainAddSensor.displayGUI = 1;
			}

			GUI.Box(new Rect(StaticMemory._Margin + StaticMemory._SizeX/4, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2, largeur/2, StaticMemory._hauteurChamp),"Model Management");
			GUI.EndGroup();
			
			// Groupe : Corps de la GUI
			GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX , StaticMemory._SizeY));
			
			// Groupe : 1re colonne
			GUI.BeginGroup (new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY ));
			GUI.Box(new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY ), "");
			currentY = 0;
			largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
			size = (StaticMemory._SizeY/2 - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
			
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "List of OBJ files");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			Scroll1 = GUI.BeginScrollView(
				new Rect(0,currentY, StaticMemory._SizeX / 4, StaticMemory._SizeY - currentY),
				Scroll1, 
				new Rect(0,currentY, StaticMemory._SizeX / 4 - 17, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(StaticMemory.ArrayofOBJ.Length) + StaticMemory._hauteurChamp),
				false, false
				);

			for(int i = 0; i < StaticMemory.ArrayofOBJ.Length; i++){
				if(GUI.Button(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), StaticMemory.ArrayofOBJ[i].Split('.')[0])){
					currentGUI = i;
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			}
			GUI.EndScrollView();
			GUI.EndGroup();
			

			// Groupe : 2e colonne
			GUI.BeginGroup (new Rect(StaticMemory._SizeX / 4,0, StaticMemory._SizeX / 4, StaticMemory._SizeY));
			GUI.Box (new Rect(0 ,0, StaticMemory._SizeX / 4, StaticMemory._SizeY), "");
			currentY = 0;
			largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
			size = (StaticMemory._SizeY - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Display settings");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			if(GUI.Button(new Rect(StaticMemory._Margin, currentY, largeur/3, StaticMemory._hauteurChamp), "All")){
				for(int i = 0; i< MainAddSensor.isDisplayed.Length; i++){
					MainAddSensor.isDisplayed[i] = true;
				}
				CheckVisible();
			}
			if(GUI.Button(new Rect(StaticMemory._Margin + 1 * largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp), "None")){
				for(int i = 0; i< MainAddSensor.isDisplayed.Length; i++){
					MainAddSensor.isDisplayed[i] = false;
				}
				CheckVisible();
			}

			if(GUI.Button(new Rect(StaticMemory._Margin + 2 * largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp), "Reverse")){
				for(int i = 0; i< MainAddSensor.isDisplayed.Length; i++){
					MainAddSensor.isDisplayed[i] = !MainAddSensor.isDisplayed[i];
				}
				CheckVisible();
			}
			currentY += StaticMemory._hauteurChamp + 2 * StaticMemory._hauteurEntre;

			if(currentGUI >= 0){
				Scroll1 = GUI.BeginScrollView(
					new Rect(0,currentY, StaticMemory._SizeX / 4, StaticMemory._SizeY - currentY),
					Scroll1, 
					new Rect(0,currentY, StaticMemory._SizeX / 4 - 17, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(StaticMemory.ArrayofOBJ.Length) + StaticMemory._hauteurChamp),
					false, false
					);

				bool b = GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur , StaticMemory._hauteurChamp),  MainAddSensor.isDisplayed[currentGUI], MainAddSensor.objModel[currentGUI].name.Split('.')[0]);
				if(MainAddSensor.isDisplayed[currentGUI] != b){
					MainAddSensor.isDisplayed[currentGUI] = b;
					CheckVisible();
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				if(GUI.Button(new Rect(StaticMemory._Margin + 2 * largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp), "one")){
					for(int i = 0; i< MainAddSensor.isDisplayed.Length; i++){
						MainAddSensor.isDisplayed[i] = i == currentGUI;
					}
					CheckVisible();
				}
				currentY += StaticMemory._hauteurChamp + 2* StaticMemory._hauteurEntre;
			}
			else{
				Scroll1 = GUI.BeginScrollView(
					new Rect(0,currentY, StaticMemory._SizeX / 4, StaticMemory._SizeY - currentY),
					Scroll1, 
					new Rect(0,currentY, StaticMemory._SizeX / 4 - 17, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(StaticMemory.ArrayofOBJ.Length + 1) + StaticMemory._hauteurChamp),
					false, false
					);
			}

			for(int i = 0; i< MainAddSensor.isDisplayed.Length; i++){
			if(i == currentGUI){
					continue;
				}
				bool b = GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur , StaticMemory._hauteurChamp),  MainAddSensor.isDisplayed[i], MainAddSensor.objModel[i].name.Split('.')[0]);
				if(MainAddSensor.isDisplayed[i] != b){
					MainAddSensor.isDisplayed[i] = b;
					CheckVisible();
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			}
			GUI.EndScrollView();
			GUI.EndGroup();

			
			
			// 3eme collone
			GUI.BeginGroup (new Rect(StaticMemory._SizeX / 2, 0, StaticMemory._SizeX / 2, StaticMemory._SizeY));
			GUI.Box (new Rect(0,0, StaticMemory._SizeX / 2, StaticMemory._SizeY), "");
			currentY = StaticMemory._Caption + StaticMemory._Margin;
			largeur  = StaticMemory._SizeX / 2 - 2 * StaticMemory._Margin;
			currentY = 0;
			if(currentGUI == -1){
				GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Location settings:");
			}
			if(currentGUI >= 0){
				GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Location settings: " + MainAddSensor.objModel[currentGUI].name.Split('.')[0]);
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

				Transform t = MainAddSensor.objModel[currentGUI].transform;
				// Set Floor
				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur / 2, StaticMemory._hauteurChamp), "Set floor");
				floor = GUI.TextArea(new Rect(StaticMemory._Margin + largeur / 2, currentY, largeur/4, StaticMemory._hauteurChamp), floor);
				if(GUI.Button(new Rect(StaticMemory._Margin + 3 * largeur / 4, currentY, largeur/4, StaticMemory._hauteurChamp), "Set") || floor.Contains("\n")){
					int f = int.Parse(floor.Split('\n')[0]);
					int h = int.Parse(SweetHomeHeight);
					Vector3 u = t.localPosition;
					t.localPosition = new Vector3 (u.x, f*h, u.z);
					floor = "";

				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur / 2, StaticMemory._hauteurChamp), "Set Height (in cm)");
				SweetHomeHeight = GUI.TextField(new Rect(StaticMemory._Margin + largeur / 2, currentY, largeur/4, StaticMemory._hauteurChamp), SweetHomeHeight);
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

				int[] posX =  {
					StaticMemory._Margin + largeur / 6,
					StaticMemory._Margin + largeur / 3,
					StaticMemory._Margin + 3 * largeur / 4,
					StaticMemory._Margin + 7 * largeur / 8
				};

				int[] sizeX = {
					largeur / 6,
					largeur / 3,
					largeur / 8,
					largeur / 8,
				};



				// Set Position
				GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur , StaticMemory._hauteurChamp), "Set Position (Y up)");
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				
				GUI.Label(new Rect(posX[0], currentY, sizeX[0], StaticMemory._hauteurChamp), "x:");
				float tx = float.Parse(GUI.TextArea(new Rect(posX[1], currentY, sizeX[1], StaticMemory._hauteurChamp), "" + t.localPosition.x));

				if(GUI.Button(new Rect(posX[2], currentY, sizeX[2], StaticMemory._hauteurChamp), "-")){
					tx-= int.Parse(SweetHomeHeight) / 10;
				}
				if(GUI.Button(new Rect(posX[3], currentY, sizeX[3], StaticMemory._hauteurChamp), "+")){
					tx+= int.Parse(SweetHomeHeight) / 10;
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

				GUI.Label(new Rect(posX[0], currentY, sizeX[0], StaticMemory._hauteurChamp), "y:");
				float ty = float.Parse(GUI.TextArea(new Rect(posX[1], currentY, sizeX[1], StaticMemory._hauteurChamp), "" + t.localPosition.y));
				
				if(GUI.Button(new Rect(posX[2], currentY, sizeX[2], StaticMemory._hauteurChamp), "-")){
					ty -= int.Parse(SweetHomeHeight) / 10;
				}
				if(GUI.Button(new Rect(posX[3], currentY, sizeX[3], StaticMemory._hauteurChamp), "+")){
					ty+= int.Parse(SweetHomeHeight) / 10;
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

				GUI.Label(new Rect(posX[0], currentY, sizeX[0], StaticMemory._hauteurChamp), "z:");
				float tz = float.Parse(GUI.TextArea(new Rect(posX[1], currentY, sizeX[1], StaticMemory._hauteurChamp), "" + t.localPosition.z));
				
				if(GUI.Button(new Rect(posX[2], currentY, sizeX[2], StaticMemory._hauteurChamp), "-")){
					tz-= int.Parse(SweetHomeHeight) / 10;
				}
				if(GUI.Button(new Rect(posX[3], currentY, sizeX[3], StaticMemory._hauteurChamp), "+")){
					tz+=int.Parse(SweetHomeHeight) / 10; 
				}
				t.localPosition = new Vector3(tx,ty,tz);
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;



				// Set Rotation
				GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur , StaticMemory._hauteurChamp), "Set Rotation");
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				
				GUI.Label(new Rect(posX[0], currentY, sizeX[0], StaticMemory._hauteurChamp), "x:");
				float rx = float.Parse(GUI.TextArea(new Rect(posX[1], currentY, sizeX[1], StaticMemory._hauteurChamp), "" + t.localEulerAngles.x));
				
				if(GUI.Button(new Rect(posX[2], currentY, sizeX[2], StaticMemory._hauteurChamp), "-")){
					rx-= 45;
				}
				if(GUI.Button(new Rect(posX[3], currentY, sizeX[3], StaticMemory._hauteurChamp), "+")){
					rx+= 45;
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				
				GUI.Label(new Rect(posX[0], currentY, sizeX[0], StaticMemory._hauteurChamp), "y:");
				float ry = float.Parse(GUI.TextArea(new Rect(posX[1], currentY, sizeX[1], StaticMemory._hauteurChamp), "" + t.localEulerAngles.y));
				
				if(GUI.Button(new Rect(posX[2], currentY, sizeX[2], StaticMemory._hauteurChamp), "-")){
					ry -= 45;
				}
				if(GUI.Button(new Rect(posX[3], currentY, sizeX[3], StaticMemory._hauteurChamp), "+")){
					ry+= 45;
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				
				GUI.Label(new Rect(posX[0], currentY, sizeX[0], StaticMemory._hauteurChamp), "z:");
				float rz = float.Parse(GUI.TextArea(new Rect(posX[1], currentY, sizeX[1], StaticMemory._hauteurChamp), "" + t.localEulerAngles.z));
				
				if(GUI.Button(new Rect(posX[2], currentY, sizeX[2], StaticMemory._hauteurChamp), "-")){
					rz-= 45;
				}
				if(GUI.Button(new Rect(posX[3], currentY, sizeX[3], StaticMemory._hauteurChamp), "+")){
					rz+=45; 
				}
				t.localEulerAngles = new Vector3(rx,ry,rz);
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

				// Set Scale
				GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur , StaticMemory._hauteurChamp), "Set Scale");
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				
				GUI.Label(new Rect(posX[0], currentY, sizeX[0], StaticMemory._hauteurChamp), "x:");
				float sx = float.Parse(GUI.TextArea(new Rect(posX[1], currentY, sizeX[1], StaticMemory._hauteurChamp), "" + t.localScale.x));
				
				if(GUI.Button(new Rect(posX[2], currentY, sizeX[2], StaticMemory._hauteurChamp), "-")){
					sx *= 0.9f;
				}
				if(GUI.Button(new Rect(posX[3], currentY, sizeX[3], StaticMemory._hauteurChamp), "+")){
					sx *= 1.1f;
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				
				GUI.Label(new Rect(posX[0], currentY, sizeX[0], StaticMemory._hauteurChamp), "y:");
				float sy = float.Parse(GUI.TextArea(new Rect(posX[1], currentY, sizeX[1], StaticMemory._hauteurChamp), "" + t.localScale.y));

				if(GUI.Button(new Rect(posX[2], currentY, sizeX[2], StaticMemory._hauteurChamp), "-")){
					sy *= 0.9f;
				}
				if(GUI.Button(new Rect(posX[3], currentY, sizeX[3], StaticMemory._hauteurChamp), "+")){
					sy *= 1.1f;
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				
				GUI.Label(new Rect(posX[0], currentY, sizeX[0], StaticMemory._hauteurChamp), "z:");
				float sz = float.Parse(GUI.TextArea(new Rect(posX[1], currentY, sizeX[1], StaticMemory._hauteurChamp), "" + t.localScale.z));

				if(GUI.Button(new Rect(posX[2], currentY, sizeX[2], StaticMemory._hauteurChamp), "-")){
					sz *= 0.9f;
				}
				if(GUI.Button(new Rect(posX[3], currentY, sizeX[3], StaticMemory._hauteurChamp), "+")){
					sz *= 1.1f;
				}

				Vector3 v = new Vector3(sx,sy,sz);
				if(!v.Equals(t.localScale)){
					t.localScale = v;
				}
				
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;


				// SaveCurrent settings
				if(GUI.Button(new Rect(StaticMemory._Margin, currentY, largeur /4, StaticMemory._hauteurChamp), "Save Current")){
				System.IO.File.WriteAllText(
						StaticMemory.CurrentPreSelectedBuildingPath + "/" +  StaticMemory.ArrayofOBJ[currentGUI].Split('.')[0] + ".oSet",
						t.localPosition.x + ";" + t.localPosition.y + ";" + t.localPosition.z + ";" +
						t.localRotation.x + ";" + t.localRotation.y + ";" + t.localRotation.z + ";" + t.localRotation.w + ";" + 
						t.localScale.x + ";" + t.localScale.x + ";" + t.localScale.x);
				}

				if(GUI.Button(new Rect(StaticMemory._Margin + largeur /4 , currentY, largeur/4 , StaticMemory._hauteurChamp), "Reload")){
					try{
						string[] s = System.IO.File.ReadAllText(StaticMemory.CurrentPreSelectedBuildingPath + "/" +  StaticMemory.ArrayofOBJ[currentGUI].Split('.')[0] + ".oSet").Split(';');
						t.localPosition = new Vector3 (float.Parse(s[0]),float.Parse(s[1]),float.Parse(s[2]));
						t.localRotation = new Quaternion(float.Parse(s[3]),float.Parse(s[4]),float.Parse(s[5]),float.Parse(s[6]));
						t.localScale = new Vector3 (float.Parse(s[7]),float.Parse(s[8]),float.Parse(s[9]));

					}
					catch{
					}
				}
				if(GUI.Button(new Rect(StaticMemory._Margin + 2*largeur /4, currentY, largeur /4, StaticMemory._hauteurChamp), "Clear Current")){
					t.localScale = Vector3.one;
					t.localPosition = Vector3.zero;
					t.localRotation = Quaternion.identity;

				}
				if(GUI.Button(new Rect(StaticMemory._Margin + 3* largeur /4, currentY, largeur /4, StaticMemory._hauteurChamp), "Erase setting")){
					System.IO.File.Delete( StaticMemory.CurrentPreSelectedBuildingPath + "/" +  StaticMemory.ArrayofOBJ[currentGUI].Split('.')[0] + ".oSet");
					t.localScale = Vector3.one;
					t.localPosition = Vector3.zero;
					t.localRotation = Quaternion.identity;
				}


			}
			

			GUI.EndGroup();
			GUI.EndGroup();
			GUI.EndGroup();








		}
	}

	void CheckVisible(){
		for(int i = 0 ; i < MainAddSensor.isDisplayed.Length; i++){
			MainAddSensor.objModel[i].SetActive(MainAddSensor.isDisplayed[i]);
		}
	}

}
