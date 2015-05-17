using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharpfirstpass;

public class Geoloc {
	private char sep = '=';
	public string Building = "";
	public string Floor = "";
	public string Place ="";
	public string Comments = "";
	public string Surface = "0";

	public string Image = "";

	public Geoloc(){
	}

	public Geoloc(string s){
		
		Building = PString.ReadArg("Building",s);
		Floor = PString.ReadArg("Floor",s);
		Place= PString.ReadArg("Place",s);
		Image= PString.ReadArg("Image",s);
		Comments= PString.ReadArg("Comments",s);
		Surface= PString.ReadArg("Surface",s);

	}

	public Geoloc (Geoloc g){
		Building = g.Building;
		Floor = g.Floor;
		Place = g.Place;
		Comments = g.Comments;
		Surface = g.Surface;
		Image = g.Image;

	}


	public string ToString(){
		return 
			PString.WriteArg("Type","SemanticGeoloc") + 
			PString.WriteArg("Building",Building) +
			PString.WriteArg("Floor",Floor) + 
			PString.WriteArg("Place",Place) + 
			PString.WriteArg("Image",Image) + 
			PString.WriteArg("Comments",Comments) +
			PString.WriteArg("Surface",Surface);

	}


	public static string[] GetAllBuildings(LinkedList<Geoloc> list){
		LinkedList<string> temp = new LinkedList<string>();
		int count = 0;
		foreach(Geoloc s in list){
			if(s.Building == ""){
				continue;
			}
			bool isIn = false;
			foreach(string sIn in temp){
				if (sIn == s.Building){
					isIn = true;
					break;
				}
			}
			if(!isIn){
				temp.AddLast(s.Building);
				count ++;
			}
		}
		string[] res = new string[count];
		temp.CopyTo(res, 0);
		return res;
	}

	public static string[] GetAllFloors(LinkedList<Geoloc> list, string Build){
		LinkedList<string> temp = new LinkedList<string>();
		int count = 0;
		foreach(Geoloc s in list){
			if(s.Building != Build || s.Floor == ""){
				continue;
			}
			bool isIn = false;
			foreach(string sIn in temp){
				if (sIn == s.Floor){
					isIn = true;
					break;
				}
			}
			if(!isIn){
				temp.AddLast(s.Floor);
				count ++;
			}
		}
		string[] res = new string[count];
		temp.CopyTo(res, 0);
		return res;
	}


	public static string[] GetAllRooms(LinkedList<Geoloc> list, string Build, string Floor){
		LinkedList<string> temp = new LinkedList<string>();
		int count = 0;
		foreach(Geoloc s in list){
			if(s.Building != Build || s.Floor != Floor || s.Place == ""){
				continue;
			}
			bool isIn = false;
			foreach(string sIn in temp){
				if (sIn == s.Place){
					isIn = true;
					break;
				}
			}
			if(!isIn){
				temp.AddLast(s.Place);
				count ++;
			}
		}
		string[] res = new string[count];
		temp.CopyTo(res, 0);
		return res;
	}


	public static Geoloc GetRoom(LinkedList<Geoloc> l, string b, string f, string r){
		foreach (Geoloc g in l){
			if(g.Building == b && g.Floor == f && g.Place == r)
				return new Geoloc (g);
		}
		return new Geoloc();
	}

	public static LinkedList<Geoloc> Delete (LinkedList<Geoloc> l, string b, string f, string r){
		LinkedList<Geoloc> res = new LinkedList<Geoloc>();
		foreach (Geoloc g in l){
			if(g.Building == b && g.Floor == f && g.Place == r){
				continue;
			}
			else{
			res.AddLast(g);
			}
		}
		return res;
	}

	public static LinkedList<Geoloc> Replace (LinkedList<Geoloc> l, Geoloc geo, string b, string f, string r){
		LinkedList<Geoloc> res = new LinkedList<Geoloc>();
		foreach (Geoloc g in l){
			if(g.Building == b && g.Floor == f && g.Place == r){
				res.AddLast(geo);
			}
			else{
				res.AddLast(g);
			}
		}
		return res;
	}


	public static string Export(LinkedList<Geoloc> l){
		string res = "";
		foreach(Geoloc g in l){
			res = res + g.ToString().Replace('\n', '#') + "\n";
		}
		return res;
	}

	public static LinkedList<Geoloc> Import(string s){
		LinkedList<Geoloc> l = new LinkedList<Geoloc>();
		foreach (string sl in s.Split('\n')){
			if(sl != "")
				l.AddLast(new Geoloc(sl.Replace('#', '\n')));
		}
		return l;
	}

}
