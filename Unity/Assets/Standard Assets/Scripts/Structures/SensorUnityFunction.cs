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
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharpfirstpass{
	public class SensorUnityFunction{
		public static string[] LinkKeyValues = {"[#1]","2","[#2]","1","[#3]","3","[#4]","2","[##Tab1]","{1,2,3,1;2,3,1,2;3,1,2,3;1,2,3,1}"};

		public struct StringItem{
			public string str;
			public ParseStringExp item;
			public string value;

			public StringItem(string s){
				str = s;
				item = new ParseStringExp ();
				value = "";
			}

			public void Make(LinkedList<SensorUpdator.KeyValue> KeyValuesRootList){
				item = ParseStringExp.Make(PString.RemplaceSpecialChar(str,KeyValuesRootList));
			}
			
			public void Evaluate(LinkedList<SensorUpdator.KeyValue> KeyValuesList){
				value = item.Evaluate(KeyValuesList);
			}
		};

		public struct BoolItem{
			public string str;
			public ParseBoolExp item;
			public bool value;
			
			public BoolItem(string s){
				str = s;
				item = new ParseBoolExp ();	
				value = true;
			}

			public void Make(LinkedList<SensorUpdator.KeyValue> KeyValuesRootList){
				item = ParseBoolExp.Make(PString.RemplaceSpecialChar(str,KeyValuesRootList));
			}
			
			public void Evaluate(LinkedList<SensorUpdator.KeyValue> KeyValuesList){
				value = item.Evaluate(KeyValuesList);
			}
		};

		public struct FloatItem{
			public string str;
			public ParseFloatExp item;
			public float value;
			
			public FloatItem(string s){
				str = s;
				item = new ParseFloatExp ();
				value = 0;
			}

			public void Make(LinkedList<SensorUpdator.KeyValue> KeyValuesRootList){
				item = ParseFloatExp.Make(PString.RemplaceSpecialChar(str,KeyValuesRootList));
			}
			
			public void Evaluate(LinkedList<SensorUpdator.KeyValue> KeyValuesList){
				value = item.Evaluate(KeyValuesList);

			}
		};

		public bool Init = false;

		public string Name = "";
		public string NameItem = "";
		public bool Protected = false;

		//public Color color = Color.grey;
		public StringItem Text = new StringItem ("Text");

		public FloatItem PosX = new FloatItem ("0");
		public FloatItem PosY = new FloatItem ("0");
		public FloatItem PosZ = new FloatItem ("0");
		public FloatItem RotX = new FloatItem ("0");
		public FloatItem RotY = new FloatItem ("0");
		public FloatItem RotZ = new FloatItem ("0");
		public FloatItem SclX = new FloatItem ("1");
		public FloatItem SclY = new FloatItem ("1");
		public FloatItem SclZ = new FloatItem ("1");

		public FloatItem colorR = new FloatItem ("1");
		public FloatItem colorG = new FloatItem ("1");
		public FloatItem colorB = new FloatItem ("1");

		public FloatItem LightAngle 	= new FloatItem ("30");
		public FloatItem LightIntensity = new FloatItem ("1");
		public FloatItem LightRange 	= new FloatItem ("10");

		public BoolItem Active 			 = new BoolItem ("True");
		public BoolItem DisplayComponent = new BoolItem ("True");
		public BoolItem Condition = new BoolItem ("False");

		public BoolItem MetaHit = new BoolItem("True");
		public string MetaHitName = "Hover";
		public BoolItem MetaPerform = new BoolItem("False");
		public string MetaPerformText = "";

		public BoolItem MetaRootSetColor = new BoolItem("False");
		public BoolItem MetaRootRenderer = new BoolItem("True");




		public SensorUnityFunction (string s, bool b){
			Protected = b;
			Name = s;
		}

		public SensorUnityFunction(string s){
			Name = PString.ReadArg("Name",s);
			Protected = bool.Parse(PString.ReadArg("Protected",s));
			
			Text = 				new StringItem (PString.ReadArg("Text",s));

			colorR = 			new FloatItem (PString.ReadArg("colorR",s));
			colorG = 			new FloatItem (PString.ReadArg("colorG",s));
			colorB = 			new FloatItem (PString.ReadArg("colorB",s));
			
			PosX = 				new FloatItem (PString.ReadArg("PosX",s));
			PosY = 				new FloatItem (PString.ReadArg("PosY",s));
			PosZ = 				new FloatItem (PString.ReadArg("PosZ",s));
				   				
			RotX = 				new FloatItem (PString.ReadArg("RotX",s));
			RotY = 				new FloatItem (PString.ReadArg("RotY",s));
			RotZ = 				new FloatItem (PString.ReadArg("RotZ",s));

			SclX = 				new FloatItem (PString.ReadArg("SclX",s));
			SclY = 				new FloatItem (PString.ReadArg("SclY",s));
			SclZ = 				new FloatItem (PString.ReadArg("SclZ",s));

			LightAngle = 		new FloatItem (PString.ReadArg("LightAngle",s));
			LightIntensity = 	new FloatItem (PString.ReadArg("LightIntensity",s));
			LightRange = 		new FloatItem (PString.ReadArg("LightRange",s));
			Condition = 		new BoolItem (PString.ReadArg("Condition",s));
			
			Active = 			new BoolItem (PString.ReadArg("Active",s));
			DisplayComponent = 	new BoolItem (PString.ReadArg("DisplayComponent",s));

			
			MetaHit = 			new BoolItem (PString.ReadArg("MetaHit",s));
			MetaHitName = 		PString.ReadArg("MetaHitName",s);
			if(MetaHitName == ""){
				MetaHitName = "Hover";
			}

			MetaPerform = 		new BoolItem (PString.ReadArg("MetaPerform",s));
			
			MetaPerformText = 	PString.ReadArg("MetaPerformText",s);

			
			MetaRootSetColor = 		new BoolItem (PString.ReadArg("MetaRootSetColor",s));
			MetaRootRenderer = 		new BoolItem (PString.ReadArg("MetaRootRenderer",s));


			Active.str = PString.ReadArg("Active",s);
			DisplayComponent.str = PString.ReadArg("DisplayComponent",s);
		}
		
		
		
		
		public string ToString(){
			return
				PString.WriteArg("Name",Name) + 
				PString.WriteArg("Protected",Protected.ToString()) + 
				PString.WriteArg("colorR",colorR.str) + 
				PString.WriteArg("colorG",colorG.str) + 
				PString.WriteArg("colorB",colorB.str) + 
				PString.WriteArg("Text",Text.str) + 
				PString.WriteArg("PosX",PosX.str) + 
				PString.WriteArg("PosY",PosY.str) + 
				PString.WriteArg("PosZ",PosZ.str) + 
				PString.WriteArg("RotX",RotX.str) + 
				PString.WriteArg("RotY",RotY.str) + 
				PString.WriteArg("RotZ",RotZ.str) + 
				PString.WriteArg("SclX",SclX.str) + 
				PString.WriteArg("SclY",SclY.str) + 
				PString.WriteArg("SclZ",SclZ.str) + 
				PString.WriteArg("LightAngle",LightAngle.str) + 
				PString.WriteArg("LightIntensity",LightIntensity.str) + 
				PString.WriteArg("LightRange",LightRange.str) + 
				PString.WriteArg("Condition",Condition.str) + 
				PString.WriteArg("Active",Active.str) + 
				PString.WriteArg("DisplayComponent",DisplayComponent.str) +
					
				PString.WriteArg("MetaHit",MetaHit.str) + 	
				PString.WriteArg("MetaHitName",MetaHitName) + 	

				PString.WriteArg("MetaPerform",MetaPerform.str) + 
				PString.WriteArg("MetaPerformText",MetaPerformText) + 
					
				PString.WriteArg("MetaRootSetColor",MetaRootSetColor.str) + 
				PString.WriteArg("MetaRootRenderer",MetaRootRenderer.str);



			
		}

		/*List des mots clefs : 
			 *  - All : tous
			 *  - Geo : Pos + Rot + Scl
			 *  - Light : Color + Angle, intensity and range
		*/
		public void Make(string Key, LinkedList<SensorUpdator.KeyValue> KeyValuesRootList){
			if(Key == "All"){
				Make ("Text" ,KeyValuesRootList);
				Make ("Geo",KeyValuesRootList);
				Make ("Color",KeyValuesRootList);
				Make ("Light",KeyValuesRootList);
				Make ("Bool",KeyValuesRootList);
				Make ("Condition",KeyValuesRootList);
				Make ("Meta",KeyValuesRootList);
			}
			
			if(Key == "Geo"){
				Make ("Pos",KeyValuesRootList);
				Make ("Rot",KeyValuesRootList);
				Make ("Scl",KeyValuesRootList);
			}

			if(Key == "Bool"){
				Make ("Active",KeyValuesRootList);
				Make ("Display",KeyValuesRootList);
				Make ("Condition",KeyValuesRootList);
			}


			switch (Key){
			case "Text":
				Text.Make(KeyValuesRootList);
				break;
			case "Pos":
				PosX.Make(KeyValuesRootList);
				PosY.Make(KeyValuesRootList);
				PosZ.Make(KeyValuesRootList);
				break;
			case "Rot":
				RotX.Make(KeyValuesRootList);
				RotY.Make(KeyValuesRootList);
				RotZ.Make(KeyValuesRootList);
				break;
			case "Scl":
				SclX.Make(KeyValuesRootList);
				SclY.Make(KeyValuesRootList);
				SclZ.Make(KeyValuesRootList);
				break;
			case "Color":
				colorR.Make(KeyValuesRootList);
				colorG.Make(KeyValuesRootList);
				colorB.Make(KeyValuesRootList);
				break;

			case "Light":
				LightAngle.Make(KeyValuesRootList);
				LightIntensity.Make(KeyValuesRootList);
				LightRange.Make(KeyValuesRootList);
				break;

			case "Active":
				Active.Make(KeyValuesRootList);
				break;

			case "Display":
				DisplayComponent.Make(KeyValuesRootList);
				break;

			case "Condition":
				Condition.Make(KeyValuesRootList);
				break;

			case "Meta":
				MetaHit.Make(KeyValuesRootList);
				MetaPerform.Make(KeyValuesRootList);
				MetaRootSetColor.Make(KeyValuesRootList);
				MetaRootRenderer.Make(KeyValuesRootList);
				break;
			}
		}

		public void Evaluate(string Key, LinkedList<SensorUpdator.KeyValue> LinkKeyValues){
			if(Key == "All"){
				Evaluate ("Text",LinkKeyValues);
				Evaluate ("Geo",LinkKeyValues);
				Evaluate ("Color",LinkKeyValues);
				Evaluate ("Light",LinkKeyValues);
				Evaluate ("Bool",LinkKeyValues);
				Evaluate ("Condition",LinkKeyValues);
				Evaluate ("Meta",LinkKeyValues);
			}
			
			if(Key == "Geo"){
				Evaluate ("Pos",LinkKeyValues);
				Evaluate ("Rot",LinkKeyValues);
				Evaluate ("Scl",LinkKeyValues);
			}
			
			if(Key == "Bool"){
				Evaluate ("Active",LinkKeyValues);
				Evaluate ("Display",LinkKeyValues);
				Evaluate ("Condition",LinkKeyValues);
			}
			switch (Key){
			case "Text":
				Text.Evaluate(LinkKeyValues);
				break;
			case "Pos":
				PosX.Evaluate(LinkKeyValues);
				PosY.Evaluate(LinkKeyValues);
				PosZ.Evaluate(LinkKeyValues);
				break;
			case "Rot":
				RotX.Evaluate(LinkKeyValues);
				RotY.Evaluate(LinkKeyValues);
				RotZ.Evaluate(LinkKeyValues);
				break;
			case "Scl":
				SclX.Evaluate(LinkKeyValues);
				SclY.Evaluate(LinkKeyValues);
				SclZ.Evaluate(LinkKeyValues);
				break;

			case "Color":
				colorR.Evaluate(LinkKeyValues);
				colorG.Evaluate(LinkKeyValues);
				colorB.Evaluate(LinkKeyValues);
				break;

			case "Light":
				LightAngle.Evaluate(LinkKeyValues);
				LightIntensity.Evaluate(LinkKeyValues);
				LightRange.Evaluate(LinkKeyValues);
				break;

			case "Active":
				Active.Evaluate(LinkKeyValues);
				break;

			case "Display":
				DisplayComponent.Evaluate(LinkKeyValues);
				break;

			case "Condition":
				Condition.Evaluate(LinkKeyValues);
				break;
			
			case "Meta":
				MetaHit.Evaluate(LinkKeyValues);
				MetaPerform.Evaluate(LinkKeyValues);
				MetaRootSetColor.Evaluate(LinkKeyValues);
				MetaRootRenderer.Evaluate(LinkKeyValues);
				break;

			}
		}

		
		public SensorUnityFunction Duplicate(){
			return new SensorUnityFunction(this.ToString());
		}



		public void Update(string Key, GameObject g){
			if(Key == "All"){
				Update ("Text",g);
				Update ("Geo",g);
				Update ("Color",g);
				Update ("Light",g);
				Update ("Bool",g);
				Update ("Condition",g);
				Update ("Meta",g);
			}
			
			if(Key == "Geo"){
				Update ("Pos",g);
				Update ("Rot",g);
				Update ("Scl",g);
			}
			
			if(Key == "Bool"){
				Update ("Active",g);
				Update ("Display",g);
				Update ("Condition",g);
			}
			switch (Key){
			case "Text":
				try{
					((TextMesh) g.GetComponent(typeof(TextMesh))).text = Text.value.Replace("\\n","\n");
				}
				catch {}
				break;
			case "Pos":
				g.transform.localPosition = new Vector3 (PosX.value, PosY.value, PosZ.value);
				break;
			case "Rot":
				g.transform.eulerAngles = new Vector3 (RotX.value, RotY.value, RotZ.value);
				break;
			case "Scl":
				Vector3 v = new Vector3 (SclX.value, SclY.value, SclZ.value);
				if(v != g.transform.localScale){
					g.transform.localScale  = v;
				}
				break;
				
			case "Color":
				try{
					g.GetComponent<Renderer>().material.color = new Color(colorR.value, colorG.value, colorB.value);
				}
				catch {}
				try{
					g.GetComponent<Light>().color = new Color(colorR.value, colorG.value, colorB.value);
				}
				catch {}
				break;
				
			case "Light":
				try{
					g.GetComponent<Light>().color = new Color(colorR.value, colorG.value, colorB.value);
					g.GetComponent<Light>().spotAngle = LightAngle.value;
					g.GetComponent<Light>().intensity = LightIntensity.value;
					g.GetComponent<Light>().range = LightRange.value;
				}
				catch {}
				break;
				
			case "Active":
				g.SetActive(Active.value);
				break;
				
			case "Display":
				            try{
					g.GetComponent<Renderer>().enabled = DisplayComponent.value;
				}
				catch {}
				break;
				
			case "Condition":
				//Condition.Evaluate();
				break;

			case "Meta":


				break;


			}
		}



	}
}

