//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;


namespace AssemblyCSharpfirstpass
{
	public class SensorUnityItem{
		public LinkedList<SensorUnityItem> ListChild = new LinkedList<SensorUnityItem>();

		public LinkedList<SensorUnityFunction> ListFunction = new LinkedList<SensorUnityFunction>();
		public SensorUnityFunction CurrentFunction;

		public SensorUnityItem Parent = null;

		public string ObjectComponent = "GameObject";
		public GameObject GObject = null;

		public string Name = "GameObject";

		public SensorUnityItem (){
		}

		public SensorUnityItem (SensorUnityItem Root){
				foreach(SensorUnityFunction s in Root.ListFunction){
					ListFunction.AddFirst(new SensorUnityFunction(s.Name, s.Protected));
				}
		}

		public static string[] PrimitiveList = {"GameObject","Text","Sphere","Cube","Plane","Capsule","Cylindre","Spot Light"};

		public string ToString(string tab){
			string Res =
					tab +
					PString.WriteArg("Type","ITEM") +
					PString.WriteArg("Name",Name) + 
					PString.WriteArg("ObjectComponent",ObjectComponent) + 
					"\n";
			foreach(SensorUnityFunction s in ListFunction){
				Res += tab + "\t" + PString.WriteArg("Type","FUNCTION") + s.ToString() + "\n";
			}
			foreach(SensorUnityItem c in ListChild){
				Res += c.ToString(tab + "\t");
			}
			Res += tab + PString.WriteArg("Type","END") + "\n";
			return Res;
		}


		public SensorUnityItem(string s){
		}

		public static SensorUnityItem CreateRoot (string[] lines){
			SensorUnityItem Root = new SensorUnityItem();
			Root.Import(lines, 0);
			Root.Name = "ROOT";
			Root.ObjectComponent = "Sphere";
			Root.GObject.transform.localRotation = Quaternion.identity;
			Root.GObject.transform.localPosition = Vector3.zero;
			Root.UpdateSetting("All");
			return Root;
			}

		public int Import(string[] lines, int start){
			for (int k = start ;  k < lines.Length ; k++){
				switch (PString.ReadArg("Type",lines[k])){
				case "END":
					CurrentFunction = ListFunction.First.Value;
					return k;
					break;
				case "FUNCTION":
					SensorUnityFunction f = new SensorUnityFunction(lines[k]);
					f.NameItem = Name;
					ListFunction.AddLast(f);
					break;
				case "ITEM" :
					if(k!= start){
						SensorUnityItem Child = new SensorUnityItem();
						Child.Parent = this;
						k = Child.Import(lines, k);
						this.ListChild.AddLast(Child);
						try{
							Child.GObject.transform.parent = this.GObject.transform;
						}
						catch{}
					}
					else{
						Name = PString.ReadArg("Name",lines[k]);
						ObjectComponent = PString.ReadArg("ObjectComponent",lines[k]);
							CreateComponent();
					}
					break;
				
				default:
					start++;
					break;
				}
			}
			return lines.Length;
		}






		public void CreateComponent(){
			switch(ObjectComponent){
			case "Sphere":
				GObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				break;
			case "Cube":
				GObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
				break;
			case "Plane":
				GObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
				break;
			case "Capsule":
				GObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
				break;
			case "Cylindre":
				GObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				break;
			case "GameObject":
				GObject = new GameObject();
				break;
			case "Text":
				GObject = (GameObject) GameObject.Instantiate(GameObject.Find("Text"));
				break;
			case "Spot Light":
				GObject = new GameObject();
				GObject.AddComponent<Light>().type = LightType.Point;
				break;
			}
			GObject.name = Name;
			try{
				GObject.transform.parent = Parent.GObject.transform;
			}
			catch{}
		}

		public void UpdateCurrentFunction(string Name){
			foreach(SensorUnityFunction s in ListFunction){
				if(s.Name == Name){
					CurrentFunction = s;
				}
			}
			//CurrentFunction.Update("All",GObject);
			foreach(SensorUnityItem s in ListChild){
				s.UpdateCurrentFunction(Name);
			}
		}





		public void MakeCurrentFunction (string str,LinkedList<SensorUpdator.KeyValue> KeyValuesRootList){
			CurrentFunction.Make(str, KeyValuesRootList);
			foreach(SensorUnityItem s in ListChild){
				s.MakeCurrentFunction(str, KeyValuesRootList);
			}
		}



		public void EvaluateCurrentFunction (string str,LinkedList<SensorUpdator.KeyValue> KeyValuesList, bool Root, ObjSettingItem RootItem){
			CurrentFunction.Evaluate(str,SensorUpdator.AddKeysCurrentElement(KeyValuesList,this));

			GObject.transform.localPosition = new Vector3(CurrentFunction.PosX.value,CurrentFunction.PosY.value,CurrentFunction.PosZ.value);
			GObject.transform.localRotation = Quaternion.Euler(new Vector3(CurrentFunction.RotX.value,CurrentFunction.RotY.value,CurrentFunction.RotZ.value));


			Vector3 v = new Vector3(CurrentFunction.SclX.value,CurrentFunction.SclY.value,CurrentFunction.SclZ.value);
			if(v != GObject.transform.localScale){
				GObject.transform.localScale = v;
			}

			GObject.SetActive(CurrentFunction.Active.value);


			try{
				TextMesh tm = (TextMesh) GObject.GetComponent(typeof(TextMesh));
				tm.text = CurrentFunction.Text.value;
			}
			catch{}

			try{
				MeshRenderer m = GObject.GetComponent<MeshRenderer>();
				m.enabled = CurrentFunction.DisplayComponent.value;
			}
			catch{}
			Color c = new Color (
				Mathf.Max(0, Mathf.Min(CurrentFunction.colorR.value / 255f, 1)),
				Mathf.Max(0, Mathf.Min(CurrentFunction.colorG.value / 255f, 1)),
				Mathf.Max(0, Mathf.Min(CurrentFunction.colorB.value / 255f, 1))
				);

			try{
				GObject.GetComponent<Renderer>().material.color = c;

			}
			catch{}

			try{
				GObject.GetComponent<Light>().color = c;
         	    GObject.GetComponent<Light>().range = CurrentFunction.LightRange.value * GObject.transform.lossyScale.x;
				GObject.GetComponent<Light>().intensity = CurrentFunction.LightIntensity.value;
				GObject.GetComponent<Light>().spotAngle = CurrentFunction.LightAngle.value;
			}
			catch{}
			///
			string st = Name;
			st += " \\Item\\";
			if(CurrentFunction.MetaHit.value){
				st += "Hit#" + CurrentFunction.MetaHitName + "\\";
			}
			if(CurrentFunction.MetaPerform.value){
				st += "Send#" + PString.RemplaceSpecialChar(CurrentFunction.MetaPerformText, true) + "\\";
			}
			GObject.name = st;


			if(Root){
				try{
					RootItem.GetComponent<Renderer>().enabled = CurrentFunction.MetaRootRenderer.value;
					if( CurrentFunction.MetaRootSetColor.value){
						RootItem.GetComponent<Renderer>().material.color = c;
					}
				}
				catch{}
			}
			
			
			foreach(SensorUnityItem s in ListChild){
				s.EvaluateCurrentFunction(str,KeyValuesList, false,RootItem);
			}
		}

		private void UpdateSetting (string str){
			CurrentFunction.Update(str, GObject);
			foreach(SensorUnityItem s in ListChild){
				s.UpdateSetting(str);
			}


		}


		public void Duplicate(SensorUnityItem Current, bool b){
			ObjectComponent = Current.ObjectComponent;
			if(b){
				Name = Current.Name;
			}
			foreach(SensorUnityFunction suf in Current.ListFunction){
				SensorUnityFunction s = suf.Duplicate();
				ListFunction.AddLast(s);
				if(suf == Current.CurrentFunction){
					CurrentFunction = s;
				}
			}
			CreateComponent();
			foreach(SensorUnityItem sui in Current.ListChild){
				SensorUnityItem child = new SensorUnityItem ();
				child.Parent = this;
				child.Duplicate(sui, true);
				ListChild.AddLast(child);
			}


		}


	}
}

