using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharpfirstpass;

public class GUILocSelection : MonoBehaviour {
	public static LinkedList<LabelAndBoolean> DisplaySensorByLoc = new LinkedList<LabelAndBoolean>();
	public static LinkedList<string> DisplaySensorByFunction = new LinkedList<string>();
	public static char sep = '#';

	public bool AutoFoldingBuilding = false;
	public bool AutoFoldingFloor = false;
	public bool AutoSortFunction = true;

	private string str = "";
	
	private Vector2 Scroll1 = Vector2.zero;
	private Vector2 Scroll2 = Vector2.zero;
	private int sizeScroll = 0;

	// Use this for initialization
	void Start () {
		str = "" + SensorScript.scale;

		// Create the struct
		DisplaySensorByLoc.Clear();
		foreach(string Building in Geoloc.GetAllBuildings(StaticMemory.RoomList)){
			GUILocSelection.DisplaySensorByLoc.AddLast(new LabelAndBoolean(Building + sep + "" + sep + "", true));
			foreach(string Floor in Geoloc.GetAllFloors(StaticMemory.RoomList, Building)){
				GUILocSelection.DisplaySensorByLoc.AddLast(new LabelAndBoolean(Building + sep + Floor + sep + "", true));
				foreach(string Place in Geoloc.GetAllRooms(StaticMemory.RoomList, Building, Floor)){
					GUILocSelection.DisplaySensorByLoc.AddLast(new LabelAndBoolean(Building + sep + Floor + sep + Place, true));
				}
			}
		}


		DisplaySensorByFunction.Clear();
		foreach(string s in SensorAndFunction.ListFunctions){
			DisplaySensorByFunction.AddLast(s);
		}
	}
	
	void OnGUI(){
		if(MainAddSensor.displayGUI == 3){
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

			if(GUI.Button(new Rect(StaticMemory._Margin, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2,  StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), " < ") ||Input.GetKey(KeyCode.Escape)){
				MainAddSensor.displayGUI = 1;
			}

			GUI.Box(new Rect(StaticMemory._Margin + StaticMemory._SizeX/4,StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2, largeur/2, StaticMemory._hauteurChamp),"Sensor display settings");
			GUI.EndGroup();
			
			// Groupe : Corps de la GUI
			GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX , StaticMemory._SizeY));
			
			// Groupe : 1re colonne
			GUI.BeginGroup (new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY ));
			GUI.Box(new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY ), "");
			currentY = 0;
			largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
			size = (StaticMemory._SizeY/2 - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
			
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Options");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur , StaticMemory._hauteurChamp), "Scale of sensor");
			currentY += StaticMemory._hauteurChamp;
			str = GUI.TextArea(new Rect(StaticMemory._Margin , currentY,  largeur - 5*StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), str);
			if(GUI.Button(new Rect(StaticMemory._Margin + largeur - 5*StaticMemory._hauteurChamp, currentY, 2*StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "Ok") || str.Contains("\n")){
				try{
					SensorScript.scale = float.Parse(str.Split('\n')[0]);
				}
				catch {}
			}

			if(GUI.Button(new Rect(StaticMemory._Margin + largeur - 2*StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "-")){
				try{
					SensorScript.scale = float.Parse(str) * 0.9f;
				}
				catch {
					SensorScript.scale *= 0.9f;
				}
				str = "" + SensorScript.scale;
			}
			if(GUI.Button(new Rect(StaticMemory._Margin + largeur - StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "+")){
				try{
					SensorScript.scale = float.Parse(str) * 1.1f;
				}
				catch {
					SensorScript.scale *= 1.1f;
				}
				str = "" + SensorScript.scale;
			}
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			/*
			GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur , StaticMemory._hauteurChamp), "?");
			currentY += StaticMemory._hauteurChamp;
			AutoFoldingBuilding = GUI.Toggle(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp),AutoFoldingBuilding,"\tAuto Folding Building");
			currentY += StaticMemory._hauteurChamp;
			AutoFoldingFloor = GUI.Toggle(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp),AutoFoldingFloor,"\tAuto Folding Floor");

			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			
			GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), "?");
			currentY += StaticMemory._hauteurChamp;
			AutoSortFunction = GUI.Toggle(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp),AutoSortFunction,"\tAuto Sort Function");
			currentY += StaticMemory._hauteurChamp;
			*/

			                        // Corps
			
			
			GUI.EndGroup();

			// Groupe : 2e colonne
			GUI.BeginGroup (new Rect(StaticMemory._SizeX / 4,0, 3 * StaticMemory._SizeX / 8, StaticMemory._SizeY ));
			GUI.Box(new Rect(0,0, 3 * StaticMemory._SizeX / 8, StaticMemory._SizeY ), "");
			currentY = 0;
			largeur  = 3 * StaticMemory._SizeX / 8 - 2 * StaticMemory._Margin;
			size = (StaticMemory._SizeY/2 - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
			
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Localisation settings");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			// Corps
			string currentBuilding = "";
			string currentFloor = "";
			
			bool currentBoolBuilding = false;
			bool currentBoolFloor = false;


			Scroll1 = GUI.BeginScrollView(
				new Rect(0,currentY, 3 * StaticMemory._SizeX / 8, StaticMemory._SizeY - currentY),
				Scroll1, 
				new Rect(0,currentY, 3 * StaticMemory._SizeX / 8 - 17, sizeScroll - currentY),
				false, false
				);


			foreach(LabelAndBoolean item in GUILocSelection.DisplaySensorByLoc){
				string b = item.Label.Split(sep)[0];
				string f = item.Label.Split(sep)[1];
				string p = item.Label.Split(sep)[2];

				// Test new building
				if(currentBuilding != b){
					currentY += 2*StaticMemory._hauteurEntre;
					currentBoolBuilding = GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur - 6 * StaticMemory._hauteurChamp , StaticMemory._hauteurChamp),item.Value,"\t" + b);
					GUI.enabled = currentBoolBuilding;

					if(GUI.Button(new Rect(StaticMemory._Margin + largeur - 6 * StaticMemory._hauteurChamp, currentY,  2*StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "All")){
						foreach(Sensor s in StaticMemory.SensorList){
							if(s.Building == b){
								s.SensorScript.UpdateDisplay(0,true);
							}
						}
						foreach(LabelAndBoolean i in GUILocSelection.DisplaySensorByLoc){
							if(i.Label.StartsWith(b)){
								i.Value = true;
							}
						}
					}
					if(GUI.Button(new Rect(StaticMemory._Margin + largeur - 4 * StaticMemory._hauteurChamp, currentY,  2*StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "None")){
						foreach(Sensor s in StaticMemory.SensorList){
							if(s.Building == b){
								s.SensorScript.UpdateDisplay(0,false);
							}
						}
						foreach(LabelAndBoolean i in GUILocSelection.DisplaySensorByLoc){
							if(i.Label.StartsWith(b)){
								i.Value = false;
							}
						}

					}
					if(GUI.Button(new Rect(StaticMemory._Margin + largeur - 2 * StaticMemory._hauteurChamp, currentY,  2*StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "= !")){
						foreach(Sensor s in StaticMemory.SensorList){
							if(s.Building == b){
								s.SensorScript.UpdateDisplay(0,!s.SensorScript.GetDisplay(0));
							}
						}
						foreach(LabelAndBoolean i in GUILocSelection.DisplaySensorByLoc){
							if(i.Label.Split(sep)[2] != "" && i.Label.StartsWith(b)){
								i.Value = !i.Value;
							}
						}
					}
					currentY += StaticMemory._hauteurChamp;

					if(currentBoolBuilding != item.Value){
						// Do treatement
						item.Value = currentBoolBuilding;

						foreach(Sensor s in StaticMemory.SensorList){
							if(s.Building == b){
								s.SensorScript.UpdateDisplay(0,currentBoolBuilding && LabelAndBoolean.GetValue(GUILocSelection.DisplaySensorByLoc, s.Building + sep + s.Floor + sep + s.Place));
							}
						}

					}
					currentBuilding = b;
					currentFloor = "";
					continue;
				}
				if(!AutoFoldingBuilding || currentBoolBuilding){
					// Test new Floor
					if(currentFloor != f){
						currentY += 2*StaticMemory._hauteurEntre;
						GUI.enabled = currentBoolBuilding;
						currentBoolFloor = GUI.Toggle(new Rect(StaticMemory._Margin + StaticMemory._hauteurChamp, currentY, largeur - 4 * StaticMemory._hauteurChamp, StaticMemory._hauteurChamp),currentBoolBuilding && item.Value,"\t" + f);
					
						GUI.enabled = currentBoolBuilding && currentBoolFloor;
						if(GUI.Button(new Rect(StaticMemory._Margin + largeur - 3 * StaticMemory._hauteurChamp, currentY,  StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "A")){
							foreach(Sensor s in StaticMemory.SensorList){
								if(s.Building == b && s.Floor == f){
									s.SensorScript.UpdateDisplay(0,true);
								}
							}
							foreach(LabelAndBoolean i in GUILocSelection.DisplaySensorByLoc){
								if(i.Label.Split(sep)[2] != "" && i.Label.StartsWith(b + sep + f)){
									i.Value = true;
								}
							}
						}
						if(GUI.Button(new Rect(StaticMemory._Margin + largeur - 2 *StaticMemory._hauteurChamp, currentY,  StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "N")){
							foreach(Sensor s in StaticMemory.SensorList){
								if(s.Building == b && s.Floor == f){
									s.SensorScript.UpdateDisplay(0,false);
								}
							}
							foreach(LabelAndBoolean i in GUILocSelection.DisplaySensorByLoc){
								if(i.Label.Split(sep)[2] != "" && i.Label.StartsWith(b + sep + f)){
									i.Value = false;
								}
							}
							
						}
						if(GUI.Button(new Rect(StaticMemory._Margin + largeur - 1 * StaticMemory._hauteurChamp, currentY,  StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "R")){
							foreach(Sensor s in StaticMemory.SensorList){
								if(s.Building == b && s.Floor == f){
									s.SensorScript.UpdateDisplay(0,!s.SensorScript.GetDisplay(0));
								}
							}
							foreach(LabelAndBoolean i in GUILocSelection.DisplaySensorByLoc){
								if(i.Label.Split(sep)[2] != "" && i.Label.StartsWith(b + sep + f)){
									i.Value = !i.Value;
								}
							}
						}
						currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
						if(currentBoolBuilding && currentBoolFloor != item.Value){
							// Do treatement
							item.Value = currentBoolFloor;
					
							foreach(Sensor s in StaticMemory.SensorList){
								if(s.Building == b && s.Floor == f){
									s.SensorScript.UpdateDisplay(0,currentBoolFloor && LabelAndBoolean.GetValue(GUILocSelection.DisplaySensorByLoc, s.Building + sep + s.Floor + sep + s.Place));
								}
							}
					
							
						}
						currentFloor = f;
						continue;
					}
					// Add current Place
					if(!AutoFoldingFloor || currentBoolFloor){
						GUI.enabled = currentBoolBuilding && currentBoolFloor;
						bool current = GUI.Toggle(new Rect(StaticMemory._Margin + 2*StaticMemory._hauteurChamp, currentY, largeur - 2*StaticMemory._hauteurChamp, StaticMemory._hauteurChamp),currentBoolBuilding && currentBoolFloor && item.Value,"\t" + p);
						currentY += StaticMemory._hauteurChamp;
						if(currentBoolBuilding && currentBoolFloor && current != item.Value){
							// Do treatement
							item.Value = current;
						
							foreach(Sensor s in StaticMemory.SensorList){
								if(s.Building == b && s.Floor == f && s.Place == p){
									s.SensorScript.UpdateDisplay(0,current);
								}
							}
						}
					}
				}
				
				
			}
			GUI.EndScrollView();
			sizeScroll = currentY;

			GUI.enabled = true;
			GUI.EndGroup();

			// Groupe : 1re colonne
			GUI.BeginGroup (new Rect(5 * StaticMemory._SizeX / 8,0, 3 * StaticMemory._SizeX / 8, StaticMemory._SizeY ));
			GUI.Box(new Rect(0,0, 3 * StaticMemory._SizeX / 8, StaticMemory._SizeY ), "");
			currentY = 0;
			largeur  = 3 * StaticMemory._SizeX / 8 - 2 * StaticMemory._Margin;
			size = (StaticMemory._SizeY/2 - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
			
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Semantic grouping");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			// Corps

			Scroll2 = GUI.BeginScrollView(
				new Rect(0,currentY, 3 * StaticMemory._SizeX / 8, StaticMemory._SizeY - currentY),
				Scroll2, 
				new Rect(0,currentY, 3 * StaticMemory._SizeX / 8 - 17, SensorAndFunction.ListFunctions.Length * (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre) + StaticMemory._hauteurEntre),
				false, false
				);

			if(AutoSortFunction){
				foreach(string saf in SensorAndFunction.ListFunctions){
					if(GUILocSelection.DisplaySensorByFunction.Contains(saf)){
						if(!GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp),true,"\t" + saf)){
						GUILocSelection.DisplaySensorByFunction.Remove(saf);
						}
						currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
						foreach(Sensor s in StaticMemory.SensorList){
							s.SensorScript.UpdateDisplay(1,SensorAndFunction.DoIntersection(s.Functions));
						}
					}
				}
				currentY += StaticMemory._hauteurEntre;
				
				foreach(string saf in SensorAndFunction.ListFunctions){
					if(!GUILocSelection.DisplaySensorByFunction.Contains(saf)){
						if(GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp),false,"\t" + saf)){
							GUILocSelection.DisplaySensorByFunction.AddLast(saf);
						}
						currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
					}
				}
			}
			else{
				foreach(string saf in SensorAndFunction.ListFunctions){
					if(GUILocSelection.DisplaySensorByFunction.Contains(saf)){
						if(!GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp),true,"\t" + saf)){
							GUILocSelection.DisplaySensorByFunction.Remove(saf);
						}
					}
					else {
						if(GUI.Toggle(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp),false,"\t" + saf)){
							GUILocSelection.DisplaySensorByFunction.AddLast(saf);
						}
					}
					currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				}
				currentY += StaticMemory._hauteurEntre;
				
				foreach(string saf in SensorAndFunction.ListFunctions){

				}
			}
			GUI.EndScrollView();
			GUI.EndGroup();
			
			GUI.EndGroup();
			GUI.EndGroup();


		}

	}
}
