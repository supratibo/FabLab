using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class GUIFunction : MonoBehaviour {
	
	private GUIStyle gui = new GUIStyle();


	public int Focus = 0;

	public int _type = -1;
	public int _sensor = -1;
	
	
	private Vector2 scrollPositionType = Vector2.zero;
	private Vector2 scrollPositionSensor = Vector2.zero;
	private Vector2 scrollPositionFunction = Vector2.zero;
	
	private string NewType = "";
	private string NewSensor = "";
	private string NewFunction = "";
	
	private string Path = "";
	
	private SensorAndFunction CurrentSensor = new SensorAndFunction();
	private Data NewData = new Data();

	
	// Use this for initialization
	void Start () {
		
	}

	public string[] lt;
	public string[] ls;

		void OnGUI (){
		if(GUIMain._display == 1){

		int currentY;
		int largeur;
		int size = 0;

		if(Input.GetMouseButtonDown(0) || Input.anyKeyDown)
			Focus = 0;


			if(Input.GetKey(KeyCode.Escape)){
				GUIMain._display = 0;
			}


		/////////////////////////////////////TRAITEMENT
		lt = SensorAndFunction.GetAllTypes(StaticMemory.SensorCfgList);
		ls = new string[0];

		int curT = _type;
		int curS = _sensor;

		try{
			if (_type >= 0){
					ls = SensorAndFunction.GetAllNames(StaticMemory.SensorCfgList, lt[_type]);
			}
		}
		catch{
			_type = -1;
			_sensor = -1;
		}
		
		GUI.BeginGroup(new Rect (0,0,2* StaticMemory._Margin + StaticMemory._SizeX, 3 * StaticMemory._Margin + StaticMemory._SizeX));
		
		/////////////////////////////////////AFFICHAGE
		// Groupe : Caption
			GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin, StaticMemory._SizeX, StaticMemory._Caption));
			GUI.DrawTexture(new Rect(StaticMemory._SizeX - 3*StaticMemory._Caption,0, StaticMemory._Caption*3, StaticMemory._Caption), StaticMemory.A4HTexture, ScaleMode.StretchToFill, true, 0);
		GUI.Box(new Rect(0,0, StaticMemory._SizeX, StaticMemory._Caption), "");
		currentY = 0;
		largeur  = StaticMemory._SizeX - 2 * StaticMemory._Margin;

			if(GUI.Button(new Rect(StaticMemory._Margin, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "<")){
				GUIMain._display = 0;
			}

			GUI.Box(new Rect(StaticMemory._Margin + StaticMemory._SizeX/4, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2, largeur/2, StaticMemory._hauteurChamp),"Sensor Management");

		GUI.EndGroup();
		
		// Groupe : Corps de la GUI
		GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX , StaticMemory._SizeY));
		
		// Groupe : Type
		GUI.BeginGroup (new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY/2 ));
		GUI.Box(new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY/2 ), "");
		currentY = 0;
		largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
		
		size = (StaticMemory._SizeY/2 - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
		
		GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Types");
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

		scrollPositionType = GUI.BeginScrollView(
			new Rect(0,currentY, StaticMemory._SizeX / 4, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(size -2) + StaticMemory._hauteurChamp),
			scrollPositionType, 
			new Rect(0,currentY, StaticMemory._SizeX / 4 - 17, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(lt.Length - 1) + StaticMemory._hauteurChamp),
			false, false
			);
		
		for (int i = 0; i< lt.Length; i++){
			if (GUI.Toggle(new Rect(StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), _type == i, "   " + lt[i]) & _type != i)
				_type = i;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		}
		GUI.EndScrollView();
		currentY = StaticMemory._SizeY/2  - StaticMemory._hauteurEntre -  StaticMemory._hauteurChamp;

		NewType = GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur - StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), NewType);
		
		if (GUI.Button(new Rect(StaticMemory._Margin + largeur - StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "+") || NewType.Contains("\n")){
			if(NewType != ""){
				SensorAndFunction g = new SensorAndFunction();
				g.Type = NewType.Split('\n')[0];
					StaticMemory.SensorCfgList.AddLast(g);
				NewType = "";
				if(_type == -1){
					_type = lt.Length;
				}
			}
		}

		GUI.EndGroup();


		// groupe : Sensor
		GUI.BeginGroup (new Rect(0,StaticMemory._SizeY/2, StaticMemory._SizeX / 4, StaticMemory._SizeY/2 ));
		currentY = 0;
		GUI.Box(new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY/2 ), "");
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Sensors");
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
	
		scrollPositionSensor = GUI.BeginScrollView(
			new Rect(0,currentY, StaticMemory._SizeX / 4, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(size -2) + StaticMemory._hauteurChamp),
			scrollPositionSensor, 
			new Rect(0,currentY, StaticMemory._SizeX / 4 - 17, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(ls.Length - 1) + StaticMemory._hauteurChamp),
			false, false
			);
		
		for (int i = 0; i< ls.Length; i++){
			if (GUI.Toggle(new Rect(StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), _sensor == i, "   " + ls[i]) & _sensor != i){
				_sensor = i;
					CurrentSensor = SensorAndFunction.GetSensor(StaticMemory.SensorCfgList, lt[_type], ls[_sensor]); 
			}
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		}
		GUI.EndScrollView();
		currentY = StaticMemory._SizeY/2  - StaticMemory._hauteurEntre -  StaticMemory._hauteurChamp;
		
		NewSensor = GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur - StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), NewSensor);
		
		if (GUI.Button(new Rect(StaticMemory._Margin + largeur - StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "+") || NewSensor.Contains("\n")){
			if(NewSensor != "" && _type >= 0){
				SensorAndFunction g = new SensorAndFunction();
				g.Type = lt[_type];
				g.SensorName = NewSensor.Split('\n')[0];
					StaticMemory.SensorCfgList.AddLast(g);
				if(_sensor == -1){
					_sensor = ls.Length;
					CurrentSensor = new SensorAndFunction(g);
				}
				NewSensor = "";

			}
		}

		if(Focus < 0){
			GUI.FocusControl(Focus + "");
		}


		GUI.EndGroup();
		
	
			
		
		// 2eme collone
		GUI.BeginGroup (new Rect(StaticMemory._SizeX / 4, 0, StaticMemory._SizeX / 2, StaticMemory._SizeY));
		GUI.Box (new Rect(0,0, StaticMemory._SizeX / 2, StaticMemory._SizeY), "");
		currentY = StaticMemory._Caption + StaticMemory._Margin;
		largeur  = StaticMemory._SizeX / 2 - 2 * StaticMemory._Margin;
		currentY = 0;

			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Current Sensor information");
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

		if(_type >= 0 && _sensor >= 0){
				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Type");

			CurrentSensor.Type = GUI.TextField(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp), CurrentSensor.Type);

			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Name");
			CurrentSensor.SensorName = GUI.TextField(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp), CurrentSensor.SensorName);

			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Constructor");

			CurrentSensor.Constructor = GUI.TextField(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp), CurrentSensor.Constructor);

			currentY += StaticMemory._hauteurChamp + 2* StaticMemory._hauteurEntre;
				GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Virtual Sensor");
				if(GUI.Button(	new Rect(StaticMemory._Margin + 2 * largeur/4, currentY, largeur/4, StaticMemory._hauteurChamp), "New")){
					StaticMemory.LoadVirtualSensor = StaticMemory.CurrentPath + "/Sensor/Void";
					StaticMemory.SaveVirtualSensor = StaticMemory.CurrentPath + "/Sensor/" + CurrentSensor.PathVisualSensor;
					Application.LoadLevel(2);


				}
				if(GUI.Button(	new Rect(StaticMemory._Margin + 3 * largeur/4, currentY, largeur/4, StaticMemory._hauteurChamp), "Edit")){
					StaticMemory.LoadVirtualSensor = StaticMemory.CurrentPath + "/Sensor/" + CurrentSensor.PathVisualSensor;
					StaticMemory.SaveVirtualSensor = StaticMemory.CurrentPath + "/Sensor/" + CurrentSensor.PathVisualSensor;
					Application.LoadLevel(2);

				}
			currentY += StaticMemory._hauteurChamp;
				CurrentSensor.PathVisualSensor = GUI.TextField(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), CurrentSensor.PathVisualSensor);

			
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			GUI.Label(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), "Comments");
			currentY += StaticMemory._hauteurChamp;
			CurrentSensor.Comments = GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 7), CurrentSensor.Comments);
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


			
			GUI.Label(new Rect(StaticMemory._Margin + posX[0], currentY, SizeX[0], StaticMemory._hauteurChamp), "Name");
			GUI.Label(new Rect(StaticMemory._Margin + posX[1], currentY, SizeX[1], StaticMemory._hauteurChamp), "Unit");
			GUI.Label(new Rect(StaticMemory._Margin + posX[2], currentY, SizeX[2], StaticMemory._hauteurChamp), "Min");
			GUI.Label(new Rect(StaticMemory._Margin + posX[3], currentY, SizeX[3], StaticMemory._hauteurChamp), "Max");
			GUI.Label(new Rect(StaticMemory._Margin + posX[4], currentY, SizeX[4], StaticMemory._hauteurChamp), "Prec.");
			currentY += StaticMemory._hauteurChamp;

			LinkedList<Data> dList = new LinkedList<Data> ();
			foreach(Data d in CurrentSensor.Datas){
				Data temp = new Data();
					temp.Name = GUI.TextField(new Rect(StaticMemory._Margin + posX[0], currentY, SizeX[0], StaticMemory._hauteurChamp), d.Name);
					temp.Unit = GUI.TextField(new Rect(StaticMemory._Margin + posX[1], currentY, SizeX[1], StaticMemory._hauteurChamp), d.Unit);
					temp.UseMin = GUI.TextField(new Rect(StaticMemory._Margin + posX[2], currentY, SizeX[2], StaticMemory._hauteurChamp), d.UseMin);
					temp.UseMax = GUI.TextField(new Rect(StaticMemory._Margin + posX[3], currentY, SizeX[3], StaticMemory._hauteurChamp), d.UseMax);
					temp.Precision = GUI.TextField(new Rect(StaticMemory._Margin + posX[4], currentY, SizeX[4], StaticMemory._hauteurChamp), d.Precision);
				


				if (GUI.Button(new Rect(StaticMemory._Margin + 5 * largeur/6 + 2  * StaticMemory._hauteurEntre, currentY, largeur/6 - 2 * StaticMemory._hauteurEntre, StaticMemory._hauteurChamp), "Del")){
				}
				else{
						dList.AddLast(temp);
					}
				currentY += StaticMemory._hauteurChamp;


			}
				CurrentSensor.Datas = dList;
			currentY += StaticMemory._hauteurEntre;

			if(CurrentSensor.Datas.Count < 6){
				GUI.SetNextControlName("1");
				NewData.Name      = GUI.TextArea(new Rect(StaticMemory._Margin + posX[0], currentY, SizeX[0], StaticMemory._hauteurChamp), NewData.Name);
				GUI.SetNextControlName("2");
				NewData.Unit      = GUI.TextArea(new Rect(StaticMemory._Margin + posX[1], currentY, SizeX[1], StaticMemory._hauteurChamp), NewData.Unit);
				GUI.SetNextControlName("3");
				NewData.UseMin    = GUI.TextArea(new Rect(StaticMemory._Margin + posX[2], currentY, SizeX[2], StaticMemory._hauteurChamp), NewData.UseMin);
				GUI.SetNextControlName("4");
				NewData.UseMax    = GUI.TextArea(new Rect(StaticMemory._Margin + posX[3], currentY, SizeX[3], StaticMemory._hauteurChamp), NewData.UseMax);
				GUI.SetNextControlName("5");
				NewData.Precision = GUI.TextArea(new Rect(StaticMemory._Margin + posX[4], currentY, SizeX[4], StaticMemory._hauteurChamp), NewData.Precision);

				if(NewData.Name.Contains("\n")){
					NewData.Name = NewData.Name.Split('\n')[0] + NewData.Name.Split('\n')[1];
					Focus = 2;
				}

				if(NewData.Unit.Contains("\n")){
					NewData.Unit = NewData.Unit.Split('\n')[0] + NewData.Unit.Split('\n')[1];
					Focus = 3;
				}

				if(NewData.UseMin.Contains("\n")){
					NewData.UseMin = NewData.UseMin.Split('\n')[0] + NewData.UseMin.Split('\n')[1];
					Focus = 4;
				}
				if(NewData.UseMax.Contains("\n")){
					NewData.UseMax = NewData.UseMax.Split('\n')[0] + NewData.UseMax.Split('\n')[1];
					Focus = 5;
				}

				if (GUI.Button(new Rect(StaticMemory._Margin + 5 * largeur/6 + 2  * StaticMemory._hauteurEntre, currentY, largeur/6 - 2 * StaticMemory._hauteurEntre, StaticMemory._hauteurChamp), "Add") || NewData.Precision.Contains("\n")){
					if(NewData.Precision.Contains("\n")){
						NewData.Precision = NewData.Precision.Split('\n')[0] + NewData.Precision.Split('\n')[1];
						Focus = 1;
					}
					CurrentSensor.Datas.AddLast(NewData);
					NewData = new Data ();
				}



				if(Focus > 0){
					if(Input.GetKey(KeyCode.Tab)){
						Focus += 1;
						if(Focus > 5){
							Focus = 1;
						}
					}
					GUI.FocusControl(Focus + "");
				}

			}




			currentY = StaticMemory._SizeY  - 1*(StaticMemory._hauteurEntre +  StaticMemory._hauteurChamp);
			if(GUI.Button(new Rect (StaticMemory._Margin + 2 * largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp), "Apply")){
					StaticMemory.SensorCfgList = SensorAndFunction.Replace(StaticMemory.SensorCfgList, CurrentSensor, lt[_type], ls[_sensor]);
					CurrentSensor = SensorAndFunction.GetSensor (StaticMemory.SensorCfgList, lt[_type], ls[_sensor]);
			}
			if(GUI.Button(new Rect (StaticMemory._Margin + 1 * largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp), "Revert")){
					CurrentSensor = SensorAndFunction.GetSensor (StaticMemory.SensorCfgList, lt[_type], ls[_sensor]);
			}
			
			if(GUI.Button(new Rect (StaticMemory._Margin + 0 * largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp), "Delete")){
					StaticMemory.SensorCfgList = SensorAndFunction.Delete(StaticMemory.SensorCfgList, lt[_type], ls[_sensor]);
				_sensor = -1;
				
			}
		}
		
		
		GUI.EndGroup();




		// 3eme collone
		GUI.BeginGroup (new Rect(3 * StaticMemory._SizeX / 4, 0, StaticMemory._SizeX / 4, StaticMemory._SizeY));
		GUI.Box (new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY), "");
		currentY = StaticMemory._Caption + StaticMemory._Margin;
		largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
		currentY = 0;
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Semantic grouping");
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

		size = (StaticMemory._SizeY - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
		scrollPositionFunction = GUI.BeginScrollView(
			new Rect(0,currentY, StaticMemory._SizeX / 4, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(size -3) + StaticMemory._hauteurChamp),
			scrollPositionFunction, 
			new Rect(0,currentY, StaticMemory._SizeX / 4 - 17, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(SensorAndFunction.ListFunctions.Length - 1) +  StaticMemory._hauteurEntre +  StaticMemory._hauteurChamp),
			false, false
			);
		if (_type < 0 | _sensor < 0){
			currentY += StaticMemory._hauteurEntre;

		for (int i = 0; i< SensorAndFunction.ListFunctions.Length; i++){
				if (GUI.Toggle(new Rect(StaticMemory._Margin , currentY, largeur - StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), false, "   " + SensorAndFunction.ListFunctions[i])){
			}
			if(GUI.Button(new Rect(StaticMemory._Margin + largeur - StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "X")){
						StaticMemory.SensorCfgList = SensorAndFunction.DelFunctionAndDoUpdate(StaticMemory.SensorCfgList, SensorAndFunction.ListFunctions[i]);
			}
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		}
		}
		else{
			foreach (string s in CurrentSensor.Functions){
				bool b = GUI.Toggle(new Rect(StaticMemory._Margin , currentY, largeur - StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), true, "   " + s);
				if (!b){
					CurrentSensor.Functions.Remove(s);
				}
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			}
			currentY += StaticMemory._hauteurEntre;

				foreach (string s in SensorAndFunction.ListFunctions){
				if(!CurrentSensor.Functions.Contains(s)){
					if (GUI.Toggle(new Rect(StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), false, "   " + s)){
						CurrentSensor.Functions.AddLast (s);
					}
					currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				}
			}

		}

		GUI.EndScrollView();


		currentY = StaticMemory._SizeY  - StaticMemory._hauteurEntre -  StaticMemory._hauteurChamp;

		NewFunction = GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur - StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), NewFunction);
		if (GUI.Button(new Rect(StaticMemory._Margin + largeur - StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "+") || NewFunction.Contains("\n")){
			if( NewFunction != "" && NewFunction != "\n")
					StaticMemory.SensorCfgList = SensorAndFunction.AddFunctionAndDoUpdate(StaticMemory.SensorCfgList, NewFunction.Split('\n')[0]);
			NewFunction = "";
		}


		GUI.EndGroup();


		GUI.EndGroup();

		if(curT != _type){
			_sensor = -1;
		}

		if(_type >= 0 && _sensor >= 0){
			try{
				if(lt[_type] == CurrentSensor.Type && ls[_sensor] == CurrentSensor.SensorName){
						StaticMemory.SensorCfgList = SensorAndFunction.Replace(StaticMemory.SensorCfgList, CurrentSensor, lt[_type], ls[_sensor]);
						CurrentSensor = SensorAndFunction.GetSensor (StaticMemory.SensorCfgList, lt[_type], ls[_sensor]);
				}
			}
			catch {
			}
		}
			GUI.EndGroup();
		}
	}

}
