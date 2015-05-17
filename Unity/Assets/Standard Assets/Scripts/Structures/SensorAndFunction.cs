using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharpfirstpass;

public class SensorAndFunction{
	public static string[] ListFunctions = new string[0];

	public string Type = "";
	public string SensorName = "";
	public LinkedList<string> Functions = new LinkedList<string>();

	// Non traité pour l'instant
	public LinkedList<Data> Datas = new LinkedList<Data>();
	public string Constructor = "";
	public string Comments = "";

	public string PathVisualSensor = "Default";



	//other feilds?

	// Delarations
	public SensorAndFunction(){
	}

	public SensorAndFunction(SensorAndFunction s){
		Type = s.Type;
		SensorName = s.SensorName;
		PathVisualSensor = s.PathVisualSensor;
		Functions = new LinkedList<string> (s.Functions);
		Datas = new  LinkedList<Data>();
		foreach(Data d in s.Datas){
			Datas.AddLast( new Data(d));
		}
		Constructor = s.Constructor;
		Comments = s.Comments;
	}
	
	public SensorAndFunction(string[] lines, int start){
		for (int i = start; i< lines.Length; i++){


		}

	}

	public static int Import(LinkedList<SensorAndFunction> list,string[] lines){
		SensorAndFunction Current = new SensorAndFunction();
		for (int i = 0; i< lines.Length; i++){
			switch(PString.ReadArg("Type",lines[i])){
			case "Group":
				foreach(string s in PString.ReadArgs("Group",lines[i])){
					AddFunctionAndDoUpdate(new LinkedList<SensorAndFunction>(),s);
				}
				 


				break;
			case "BeginSensorAndFunction":
				Current = new SensorAndFunction();
				Current.Type = PString.ReadArg("SensorType",lines[i]);
				Current.SensorName = PString.ReadArg("SensorName",lines[i]);
				Current.Constructor = PString.ReadArg("Constructor",lines[i]);
				Current.Comments = PString.ReadArg("Comments",lines[i]); 
				Current.PathVisualSensor = PString.ReadArg("PathVisualSensor",lines[i]); 
				
				break;
			case "Function":
				Current.Functions.AddLast(PString.ReadArg("Function",lines[i])); 
				break;
			case "Data":
				Current.Datas.AddLast(new Data(lines[i])); 
				break;
			case "EndSensorAndFunction":
				list.AddLast(Current);
				break;
			default:
				Debug.Log("Default");
				return i;
			}			
		}
		return lines.Length;
	}


	// Overload
	public string ToString(){
		string res = 
				PString.WriteArg("Type","BeginSensorAndFunction") + 
				PString.WriteArg("SensorType",Type) + 
				PString.WriteArg("SensorName",SensorName) + 
				PString.WriteArg("Constructor",Constructor) + 
				PString.WriteArg("PathVisualSensor",PathVisualSensor)+ 
				PString.WriteArg("Comments",Comments)+ 
				"\n";

		foreach(string s in Functions){
			res += "\t" + PString.WriteArg("Type","Function") + PString.WriteArg("Function",s) + "\n";
		}
		foreach(Data d in Datas){
			res += "\t" + PString.WriteArg("Type","Data") + d.ToString() + "\n";
		}
		res += PString.WriteArg("Type","EndSensorAndFunction") + "\n";
		return res;
	}

	public static string ExportListFunction(){
		string res = "";
		foreach(string s in SensorAndFunction.ListFunctions){
			res += PString.WriteArg("Group",s);
		}
		return res;
	}

	public static LinkedList<string> ExportCurrentList(LinkedList<string> res, SensorAndFunction sensor){
		foreach(string s in sensor.Functions){
			if(!res.Contains(s)){
				res.AddLast(s);
			}
		}
		return res;
	}


	public static void ImportListFunction(string s){
		foreach(string v in s.Split('[')[1].Split(']')[0].Split(',')){
			if(v != ""){
				AddFunctionAndDoUpdate(new LinkedList<SensorAndFunction>(),v);
			}
		}

	}

	// Use it for export the whole structure : ListFunctions and the giving list
	public static string Export(LinkedList<SensorAndFunction> list){
		Debug.Log("##");
		string res = PString.WriteArg("Type","Group") + ExportListFunction() + "\n";
		foreach(SensorAndFunction s in list){
			res = res + s.ToString();
		}
		return res;
	}

	/*public static LinkedList<SensorAndFunction> Import(string text){
		LinkedList<SensorAndFunction> res = new LinkedList<SensorAndFunction>();
		foreach(string l in text.Split('\n')){
			if(l.StartsWith("[")){
				SensorAndFunction.ListFunctions = new string[0];
				res = new LinkedList<SensorAndFunction>();

				ImportListFunction(l);
			}
			if(l.StartsWith("{")){
				res.AddLast(new SensorAndFunction (l));
			}
		}
		return res;
	}*/
	
	public static string[] GetAllTypes(LinkedList<SensorAndFunction> list){
		LinkedList<string> temp = new LinkedList<string>();
		int count = 0;
		foreach(SensorAndFunction s in list){
			if(s.Type == ""){
				continue;
			}
			bool isIn = false;
			foreach(string sIn in temp){
				if (sIn == s.Type){
					isIn = true;
					break;
				}
			}
			if(!isIn){
				temp.AddLast(s.Type);
				count ++;
			}
		}
		string[] res = new string[count];
		temp.CopyTo(res, 0);
		return res;
	}

	public static string[] GetAllNames(LinkedList<SensorAndFunction> list, string Type){
		LinkedList<string> temp = new LinkedList<string>();
		int count = 0;
		foreach(SensorAndFunction s in list){
			if(s.Type != Type || s.SensorName == ""){
				continue;
			}
			bool isIn = false;
			foreach(string sIn in temp){
				if (sIn == s.SensorName){
					isIn = true;
					break;
				}
			}
			if(!isIn){
				temp.AddLast(s.SensorName);
				count ++;
			}
		}
		string[] res = new string[count];
		temp.CopyTo(res, 0);
		return res;
	}

	public static SensorAndFunction GetSensor(LinkedList<SensorAndFunction> list, string type, string name){
		foreach(SensorAndFunction s in list){
			if(s.Type == type  && s.SensorName == name ){
				return new SensorAndFunction(s);
			}
		}
		return new SensorAndFunction();
	}
	
	public static LinkedList<SensorAndFunction> AddFunctionAndDoUpdate(LinkedList<SensorAndFunction> list, string label){
		string[] temp = new string[SensorAndFunction.ListFunctions.Length + 1];
		bool inf = true;
		int offset = 0;


		for (int i = 0; i < SensorAndFunction.ListFunctions.Length ; i++){
			int res = label.CompareTo(SensorAndFunction.ListFunctions[i]);
			if(res == 0){
				return list;
			}
			if(inf && res <= 0){
				inf = false;
				offset = 1;
				temp[i] = label;
			}
			temp[i + offset] = SensorAndFunction.ListFunctions[i];
		}
		if (inf){
			temp[temp.Length -1] = label;
		}

		SensorAndFunction.ListFunctions = temp;
		return list;
	}

	public static LinkedList<SensorAndFunction> DelFunctionAndDoUpdate(LinkedList<SensorAndFunction> list, string label){
		string[] temp = new string[SensorAndFunction.ListFunctions.Length - 1];
		bool test = false;
		int offset = 0;
		int index = temp.Length -1;
		
		
		for (int i = 0; i < SensorAndFunction.ListFunctions.Length ; i++){
			int res = label.CompareTo(SensorAndFunction.ListFunctions[i]);
			if(res == 0){
				test = true;
				offset = -1;
				index = i;
				continue;
			}
			if((i + offset) < temp.Length)
				temp[i + offset] = SensorAndFunction.ListFunctions[i];
		}
		if (test){
			SensorAndFunction.ListFunctions = temp;
			foreach(SensorAndFunction s in list){
				s.Functions.Remove(label);
			}

		} 
		return list;

	}

	public static LinkedList<SensorAndFunction> Replace (LinkedList<SensorAndFunction> list, SensorAndFunction g, string type, string name){
		LinkedList<SensorAndFunction> res = new LinkedList<SensorAndFunction>();
		foreach(SensorAndFunction s in list){
			if (type == s.Type && name == s.SensorName){
				res.AddLast(g);
			}
			else{
				res.AddLast(s);
			}
		}
		return res;

	}

	public static LinkedList<SensorAndFunction> Delete (LinkedList<SensorAndFunction> list, string type, string name){
		LinkedList<SensorAndFunction> res = new LinkedList<SensorAndFunction>();
		foreach(SensorAndFunction s in list){
			if (type == s.Type && name == s.SensorName){
			}
			else{
				res.AddLast(s);
			}
		}
		return res;
		
	}


	public static bool DoIntersection(LinkedList<string> list){
		foreach(string saf in GUILocSelection.DisplaySensorByFunction){
			if(list.Contains(saf)){
				return true;
			}
		}
		return false;
	}
}