using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GUIHomeInformation : MonoBehaviour {

	private GUIStyle gui = new GUIStyle();


	private int _building = -1;
	private int _floor = -1;
	private int _room = -1;

	
	private Vector2 scrollPositionBuilding = Vector2.zero;
	private Vector2 scrollPositionFloor = Vector2.zero;
	private Vector2 scrollPositionPlace = Vector2.zero;

	private string NewBuilding = "";
	private string NewFloor = "";
	private string NewPlace = "";

	private string Path = "";

	private Geoloc CurrentPlace = new Geoloc();

	// Use this for initialization
	void Start () {

	}


	void OnGUI (){
		if(GUIMain._display == 2){

			if(Input.GetKey(KeyCode.Escape)){
				GUIMain._display = 0;
			}

		int currentY;
		int largeur;
		int size = 0;

		/////////////////////////////////////TRAITEMENT
		string[] lb = Geoloc.GetAllBuildings(StaticMemory.RoomList);
		string[] lf = new string[0];
		string[] lr = new string[0];

		int curB = _building;
		int curF = _floor;
		int curP = _room;

		try{
			if (_building >= 0){
				lf = Geoloc.GetAllFloors(StaticMemory.RoomList, lb[Math.Max(lb.Length -1,_building)]);
			
				if (_floor >= 0){
					lr = Geoloc.GetAllRooms(StaticMemory.RoomList, lb[_building], lf[_floor]);
//					Debug.Log("&2 " + lb[_building] + " && " + lf[_room]);
				}
				else{
//					Debug.Log("&1 " + lb[_building]);
				}
			}
		}
		catch{
			_building = -1;
			_floor = -1;
			_room = -1;
		}

			GUI.BeginGroup(new Rect (0,0,2* StaticMemory._Margin + StaticMemory._SizeX, 3 * StaticMemory._Margin + StaticMemory._SizeX));


		/////////////////////////////////////AFFICHAGE
		// Groupe : Caption
			GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin, StaticMemory._SizeX, StaticMemory._Caption));
			GUI.DrawTexture(new Rect(StaticMemory._SizeX - 3*StaticMemory._Caption,0, StaticMemory._Caption*3, StaticMemory._Caption), StaticMemory.A4HTexture, ScaleMode.StretchToFill, true, 0);
		GUI.Box(new Rect(0,0, StaticMemory._SizeX, StaticMemory._Caption), "");
		currentY = 0;
		largeur  = StaticMemory._SizeX - 2 * StaticMemory._Margin;

			if(GUI.Button(new Rect(StaticMemory._Margin, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2,  StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), " < ")){
				GUIMain._display = 0;
			}
			if(GUI.Button(new Rect(StaticMemory._Margin + StaticMemory._hauteurChamp, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2, 3 * StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), " Save ")){
				StaticMemory.SaveRoomList = true;
				GUIMain._display = 0;
			}

			


			GUI.Box(new Rect(StaticMemory._Margin + StaticMemory._SizeX/4, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2, largeur/2, StaticMemory._hauteurChamp),"Place Management");

		GUI.EndGroup();

		// Groupe : Corps de la GUI
		GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX , StaticMemory._SizeY));

			// Groupe : Buildings
			GUI.BeginGroup (new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY/2 ));
				GUI.Box(new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY/2 ), "");
				currentY = 0;
				largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
			
				size = (StaticMemory._SizeY/2 - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	

				GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Buildings");
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

				scrollPositionBuilding = GUI.BeginScrollView(
					new Rect(0,currentY, StaticMemory._SizeX / 4, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(size -2) + StaticMemory._hauteurChamp),
					scrollPositionBuilding, 
					new Rect(0,currentY, StaticMemory._SizeX / 4 - 17, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(lb.Length) + StaticMemory._hauteurChamp),
					false, false
				);

					for (int i = 0; i< lb.Length; i++){
			if (GUI.Toggle(new Rect(StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), _building == i, "   " + lb[i]) & _building != i)
							_building = i;
						currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
					}
					GUI.EndScrollView();
				currentY = StaticMemory._SizeY/2  - StaticMemory._hauteurEntre -  StaticMemory._hauteurChamp;

				GUI.SetNextControlName("Buildings");
				NewBuilding = GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur - StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), NewBuilding);

				if (GUI.Button(new Rect(StaticMemory._Margin + largeur - StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "+") || NewBuilding.Contains("\n")){
					if(NewBuilding != ""){
						Geoloc g = new Geoloc();
						g.Building = NewBuilding.Split('\n')[0];
						StaticMemory.RoomList.AddLast(g);
						NewBuilding = "";
						if(_building == -1){
							_building = lb.Length;
							GUI.FocusControl("Floor");
						}
					}
				}
			GUI.EndGroup();

			// groupe : Floor
			GUI.BeginGroup (new Rect(0,StaticMemory._SizeY/2, StaticMemory._SizeX / 4, StaticMemory._SizeY/2 ));
				currentY = 0;
				GUI.Box(new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY/2 ), "");
				GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Floors");
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

		scrollPositionFloor = GUI.BeginScrollView(
			new Rect(0,currentY, StaticMemory._SizeX / 4, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(size -3) + StaticMemory._hauteurChamp),
			scrollPositionFloor, 
			new Rect(0,currentY, StaticMemory._SizeX / 4 - 17, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(lf.Length) + StaticMemory._hauteurChamp),
			false, false
			);
		
		for (int i = 0; i< lf.Length; i++){
			if (GUI.Toggle(new Rect(StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), _floor == i, "   " + lf[i]))
				_floor = i;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		}
		GUI.EndScrollView();
		currentY = StaticMemory._SizeY/2  - StaticMemory._hauteurEntre -  StaticMemory._hauteurChamp;
		
		GUI.SetNextControlName("Floor");
		NewFloor = GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur - StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), NewFloor);
		if (GUI.Button(new Rect(StaticMemory._Margin + largeur - StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "+") || NewFloor.Contains("\n")){
			if(_building < 0 || NewFloor == ""){
			}
			else{
				Geoloc g = new Geoloc();
				g.Building = lb[_building];
				g.Floor = NewFloor.Split('\n')[0];
				StaticMemory.RoomList.AddLast(g);
				NewFloor = "";

				if(_floor == -1){
					_floor = lf.Length;
					GUI.FocusControl("Place");
				}
			}
		}
				
		GUI.EndGroup();


			// Groupe : 
			GUI.BeginGroup (new Rect(StaticMemory._SizeX / 4,0, StaticMemory._SizeX / 4, StaticMemory._SizeY));
				GUI.Box (new Rect(0 ,0, StaticMemory._SizeX / 4, StaticMemory._SizeY), "");
				currentY = 0;
				largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
				size = (StaticMemory._SizeY - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
				GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Places");
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
				

				scrollPositionPlace = GUI.BeginScrollView(
					new Rect(0,currentY, StaticMemory._SizeX / 4, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(size -3) + StaticMemory._hauteurChamp),
					scrollPositionPlace, 
					new Rect(0,currentY, StaticMemory._SizeX / 4 - 17, (StaticMemory._hauteurChamp+StaticMemory._hauteurEntre)*(lr.Length) + StaticMemory._hauteurChamp),
					false, false
				);
		
					for (int i = 0; i< lr.Length; i++){
					if (GUI.Toggle(new Rect(StaticMemory._Margin , currentY, largeur, StaticMemory._hauteurChamp), _room == i,"   " + lr[i])){
							if(_room != i){
								_room = i;
					CurrentPlace = Geoloc.GetRoom (StaticMemory.RoomList, lb[_building], lf[_floor], lr[_room]);
							}
			}
						currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
						}
				GUI.EndScrollView();

		currentY = StaticMemory._SizeY - StaticMemory._hauteurEntre -  StaticMemory._hauteurChamp;
		
		GUI.SetNextControlName("Place");
		NewPlace = GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur - StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), NewPlace);
		if (GUI.Button(new Rect(StaticMemory._Margin + largeur - StaticMemory._hauteurChamp, currentY, StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), "+") || NewPlace.Contains("\n")){
			if(_building < 0 || _floor < 0 || NewPlace == ""){
			}
			else{
				Geoloc g = new Geoloc();
				g.Building = lb[_building];
				g.Floor = lf[_floor];
				g.Place = NewPlace.Split('\n')[0];
				StaticMemory.RoomList.AddLast(g);
				NewPlace = "";

				if(_room == -1)
					_room = lr.Length;
				CurrentPlace = new Geoloc(g);


			}
		}

			GUI.EndGroup();




		// 3eme collone
			GUI.BeginGroup (new Rect(StaticMemory._SizeX / 2, 0, StaticMemory._SizeX / 2, StaticMemory._SizeY));
				GUI.Box (new Rect(0,0, StaticMemory._SizeX / 2, StaticMemory._SizeY), "");
				currentY = StaticMemory._Caption + StaticMemory._Margin;
				largeur  = StaticMemory._SizeX / 2 - 2 * StaticMemory._Margin;
				currentY = 0;
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Some information on the current Room");
			
			/*
			public string Building = "";
			public string Floor = "";
			public string Place ="";
			public string Comments = "";
			*/
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		if(_room >= 0){
		GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Building");
			CurrentPlace.Building = GUI.TextField(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp), CurrentPlace.Building);
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Floor");
		CurrentPlace.Floor = GUI.TextField(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp), CurrentPlace.Floor);
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Place");
		CurrentPlace.Place = GUI.TextField(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp), CurrentPlace.Place);
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur/4, StaticMemory._hauteurChamp), "Surface");
		CurrentPlace.Surface = GUI.TextField(new Rect(StaticMemory._Margin + largeur/4, currentY, 3*largeur/4, StaticMemory._hauteurChamp), CurrentPlace.Surface + "");
		

		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre * 3;
		GUI.Box(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp), "Comments");
		currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
		CurrentPlace.Comments = GUI.TextArea(new Rect(StaticMemory._Margin, currentY, largeur, StaticMemory._hauteurChamp * 10), CurrentPlace.Comments);
		currentY += StaticMemory._hauteurChamp * 10 + StaticMemory._hauteurEntre;


		if(GUI.Button(new Rect (StaticMemory._Margin + 0 * largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp), "Apply")){
				StaticMemory.RoomList = Geoloc.Replace(StaticMemory.RoomList, CurrentPlace, lb[_building], lf[_floor], lr[_room]);
				CurrentPlace =  Geoloc.GetRoom (StaticMemory.RoomList, lb[_building], lf[_floor], lr[_room]);
		}
		if(GUI.Button(new Rect (StaticMemory._Margin + 1 * largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp), "Revert")){
				CurrentPlace =  Geoloc.GetRoom (StaticMemory.RoomList, lb[_building], lf[_floor], lr[_room]);
		}
		
		if(GUI.Button(new Rect (StaticMemory._Margin + 2 * largeur/3, currentY, largeur/3, StaticMemory._hauteurChamp), "Delete")){
				StaticMemory.RoomList = Geoloc.Delete (StaticMemory.RoomList, lb[_building], lf[_floor], lr[_room]);
				_room = -1;

			}
		}
		currentY = StaticMemory._SizeY -  (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre) ;


		GUI.EndGroup();
		GUI.EndGroup();
			GUI.EndGroup();




		////////////
		/// 
		if(curF != _floor){
			_room = -1;
		}
		if(curB != _building){
			_floor = -1;
			_room = -1;
		}
		}

	}


}
