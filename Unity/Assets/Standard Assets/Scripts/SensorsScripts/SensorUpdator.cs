using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssemblyCSharpfirstpass{
	public class SensorUpdator : MonoBehaviour {

		public struct KeyValue {
			public string Key;
			public string Value;
			public Vector2 Scale;
			public string Id;

			public KeyValue(string k, string v){
				Key = k;
				Value = v;
				Scale =  Vector2.zero;
				Id = "0";
				if(k == ""){
					Key = "#";
				}
			}
		}

		public void DebugL(string s){
			if(debug)
				Debug.Log(s);
		}

		public bool debug = false;
				// Values of Sensors
		public LinkedList<ParseXML> SensorValues = new LinkedList<ParseXML>();

				// Values of Links
		public LinkedList<KeyValue> KeyValuesMakeList = new LinkedList<KeyValue>();
		public LinkedList<KeyValue> KeyValuesRootList = new LinkedList<KeyValue>();

		public int rand = 0;

		//private SensorUnityFunction CurrentFunction;

		public int CountHover = 0;

		public SensorUnityItem Root;
		public ObjSettingItem ObjParentItem;

		public bool EditMode = false;

		public DateTime StartTime = new DateTime();

		// Use this for initialization
		void Start () {
			StartTime = System.DateTime.Now;
			rand = (int) UnityEngine.Random.Range(0,10);
		}
		
		// Update is called once per frame
		void Update () {
			if(Root == null){
				return;
			}

			// Get Current Settings
			LinkedList<KeyValue> EvalCurrent = SetListSettings();
			if(Root.CurrentFunction.SclX.value == 0){
				Root.MakeCurrentFunction("All",KeyValuesMakeList);
			}
			// Eval All Functions elements and Update Current Function
			if(!EditMode){
				if(CountHover > 0){
					CountHover--;


				}
				else {
					SensorUnityFunction old = Root.CurrentFunction;
					foreach(SensorUnityFunction fun in Root.ListFunction){
						fun.Evaluate("Condition", EvalCurrent);
						if(fun.Condition.value){
							// Do change function.
							Root.CurrentFunction = fun;

						}
					}
					if(debug)
						Debug.Log(Root.CurrentFunction.Name);

					if(old != Root.CurrentFunction){
						Root.MakeCurrentFunction("All",KeyValuesMakeList);
						Root.UpdateCurrentFunction(Root.CurrentFunction.Name);
						StartTime = System.DateTime.Now;
						

					}
					else{
					
					}
					//Root.EvaluateCurrentFunction("All",EvalCurrent);

				}


				//Root.MakeCurrentFunction("All",KeyValuesMakeList);
				Root.EvaluateCurrentFunction("All",EvalCurrent, true,ObjParentItem);
				
			}
			else{
				Root.UpdateCurrentFunction(Root.CurrentFunction.Name);
				Root.MakeCurrentFunction("All",KeyValuesMakeList);
				
				// Evaluate All;
				Root.EvaluateCurrentFunction("All",EvalCurrent, true,ObjParentItem);

			}

			rand ++;

		}





		public LinkedList<KeyValue> SetListSettings(){
			LinkedList<KeyValue> EvalCurrent = new LinkedList<KeyValue>();
			if(debug)
				Debug.Log("###0 " + EvalCurrent.Count);
			

			foreach(KeyValue k in KeyValuesRootList){
				EvalCurrent.AddLast(k);
			}

			if(debug)
				Debug.Log("###1 " + EvalCurrent.Count);

			
			float f = (DateTime.Now.Subtract(StartTime).Milliseconds/1000f) + (DateTime.Now.Subtract(StartTime).Seconds);
			EvalCurrent.AddFirst(new KeyValue("[#Count]",""+f));

			int i = 0;
			foreach(ParseXML xml in SensorValues){
				EvalCurrent.AddFirst(new KeyValue("[#"+i+"]",xml.State));
				EvalCurrent.AddFirst(new KeyValue("[#Name"+i+"]",xml.Name));
				EvalCurrent.AddFirst(new KeyValue("[#URL"+i+"]",xml.Link));
				EvalCurrent.AddFirst(new KeyValue("[#Type"+i+"]",xml.Type));
				i++;
			}
			if(debug)
				Debug.Log("#SensorValues# " + SensorValues.Count);

			if(debug)
				Debug.Log("###2 " + EvalCurrent.Count);


			
			foreach(SensorUnityFunction suf in Root.ListFunction){
				
				suf.Evaluate("Condition",SensorUpdator.AddKeysCurrentElement(EvalCurrent,Root));
				
			}
			return EvalCurrent;
		}

		public static LinkedList<KeyValue> AddKeysCurrentElement(LinkedList<KeyValue> list, SensorUnityItem item){
			LinkedList<KeyValue> res = new LinkedList<KeyValue>();
			res.AddFirst(new KeyValue ("[#Name]",item.Name));
			try{
				res.AddFirst(new KeyValue ("[#Parent]",item.Parent.Name));
			}
			catch{}
			res.AddFirst(new KeyValue ("[#Type]",item.ObjectComponent));
			foreach(KeyValue k in list){
				if(k.Key.Contains("##")){
					res.AddLast(k);
				}
				else{
					res.AddFirst(k);
				}
			}


			return res;

		}



		public SensorUnityItem Import(string[] p){
			foreach(string s in p){
				if(PString.ReadArg("Type",s) == "ROOT"){
					string Name = PString.ReadArg("Name",s);
					try{
						int.Parse(Name.Replace("[#","").Replace("]",""));
					}
					catch{
						if(Name.Contains("##")){
							KeyValuesRootList.AddLast(new KeyValue(Name,PString.ReadArg("Value",s)));
						}
						else{
							KeyValuesMakeList.AddLast(new KeyValue(Name,PString.ReadArg("Value",s)));
						}
					}
				}
			}
			Root = SensorUnityItem.CreateRoot(p);
			Root.GObject.transform.parent = this.transform;
			Root.UpdateCurrentFunction(Root.CurrentFunction.Name);
			return Root;
		}

		public string Export(){
			string res = "";
			foreach(KeyValue k in KeyValuesMakeList){
				res += PString.WriteArg("Type","ROOT") + PString.WriteArg("Name",k.Key) + PString.WriteArg("Value",k.Value) + "\n\n";
			}
			foreach(KeyValue k in KeyValuesRootList){
				res += PString.WriteArg("Type","ROOT") + PString.WriteArg("Name",k.Key) + PString.WriteArg("Value",k.Value) + "\n\n";
			}
				return res + Root.ToString("");;
		}



	}
}
