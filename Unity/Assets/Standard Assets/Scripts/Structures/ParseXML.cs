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
using System.Linq;
using UnityEngine; 

public class ParseXML{
	public string Type = "";
	public string Name = "";
	public string State = "";
	public string Link = "";

	public ParseXML (string s){
		string[] ts = s.Split('>');
		for (int i = 0; i< ts.Length ; i++){
			switch(ts[i]){
			case "<type":
				Type = ts[i+1].Split('<')[0];
				i++;
				break;
			case "<name":
				Name = ts[i+1].Split('<')[0];
				i++;
				break;
			case "<state":
				State = ts[i+1].Split('<')[0].Replace("000000","");
				i++;
				break;
			case "<link":
				Link = ts[i+1].Split('<')[0];
				i++;
				break;
			}
		}
	}
	public static LinkedList<ParseXML> MakeListData(string s, bool group){
		LinkedList<ParseXML> res = new LinkedList<ParseXML> ();

		if(s.Contains("<members>")){
			s = s.Replace("item>","items>").Replace("members>","item>");
		}
		foreach(string str in s.Replace("<item>","\n").Split('\n')){
			ParseXML item = new ParseXML(str);
			if( item.Type != "" && (group || item.Type != "GroupItem")){
				res.AddLast(item);
			}
		}

		LinkedList<ParseXML> toReturn = new LinkedList<ParseXML>();
		foreach (ParseXML p in res.OrderBy(go=>go.Name).ToList().OrderBy(go=>go.Type).ToList()){
			toReturn.AddLast(p);
		}
		return toReturn;
	}

	public static string GetValue(LinkedList<ParseXML> List, string Name){
		foreach(ParseXML p in List){
			if (p.Name == Name){
				return p.State;
			}
		}
		return "NotFound";
	}



}
