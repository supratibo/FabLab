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


namespace AssemblyCSharpfirstpass
{
	public class ParseBoolExp{
		public static string[] op = 
		{
			"Not b",
			"Or b b",
			"And b b",
			"If b b b",
			"EQ s s",
			"NEQ s s",
			"== f f",
			"!= f f",
			"<= f f",
			"< f f",
			">= f f",
			"> f f"
		};

		public string _op = "";
		public string _count = "";
		public string _value = "";

		public LinkedList<ParseBoolExp> _childB = new LinkedList<ParseBoolExp> ();
		public LinkedList<ParseStringExp> _childS = new LinkedList<ParseStringExp> ();
		public LinkedList<ParseFloatExp> _childF = new LinkedList<ParseFloatExp> ();

		public ParseBoolExp(){
		}

		public static ParseBoolExp Make (string s){
			ParseBoolExp item = new ParseBoolExp ();

			while (s.StartsWith(" ")){
				s = ParseString.SplitFirst(s, ' ')[1];
			}
			
			string[] strv = ParseString.SplitFirst(s, ',');
			string[] strp = ParseString.SplitFirst(s, ')');
			string str = "";
			if(strv[0].Length < strp[0].Length){
				item._count = "," + strv[1];
				str = strv[0];
			}
			else{
				item._count = ")" + strp[1];
				str = strp[0];
			}
			string argsAndCount;

			foreach(string opLine in op){
				if (s.StartsWith(opLine.Split(' ')[0])){
					switch(opLine.Split(' ')[0]){
						
					// Cas ou le 1er mot est un operateur.
					/*case "True":
					case "False":
						item._op = opLine.Split(' ')[0];
						break;
*/

					case "Not":
					case "And":
					case "Or":
					case "If":
						item._op = opLine.Split(' ')[0];
						argsAndCount = ParseString.SplitFirst(s, '(')[1];
						while (!argsAndCount.StartsWith(")") ){
							ParseBoolExp child = ParseBoolExp.Make(argsAndCount);
							argsAndCount = child._count;
							item._childB.AddLast(child);



							while (argsAndCount.StartsWith(",")){
								argsAndCount = ParseString.SplitFirst(argsAndCount, ',')[1];
							}
							while (argsAndCount.StartsWith(" ")){
								argsAndCount = ParseString.SplitFirst(argsAndCount, ' ')[1];
							}
		
						}
						item._count = ParseString.SplitFirst(argsAndCount, ')')[1];

						break;


					case "EQ":
					case "NEQ":
						item._op = opLine.Split(' ')[0];
						argsAndCount = ParseString.SplitFirst(s, '(')[1];
						while (!argsAndCount.StartsWith(")") ){
							//				Debug.Log("#" + argsAndCount);
							ParseStringExp child = ParseStringExp.Make(argsAndCount);
							argsAndCount = child._count;
							//				Debug.Log("##" + argsAndCount);
							item._childS.AddLast(child);

							
							while (argsAndCount.StartsWith(",")){
								argsAndCount = ParseString.SplitFirst(argsAndCount, ',')[1];
							}
							while (argsAndCount.StartsWith(" ")){
								argsAndCount = ParseString.SplitFirst(argsAndCount, ' ')[1];
							}
						}
						item._count = ParseString.SplitFirst(argsAndCount, ')')[1];
						
						break;
						
					case "==":
					case "!=":
					case "<=":
					case "<":
					case ">=":
					case ">":
						item._op = opLine.Split(' ')[0];
						argsAndCount = ParseString.SplitFirst(s, '(')[1];
						while (!argsAndCount.StartsWith(")") ){
							ParseFloatExp child = ParseFloatExp.Make(argsAndCount);
							argsAndCount = child._count;
							item._childF.AddLast(child);
							
							
							
							while (argsAndCount.StartsWith(",")){
								argsAndCount = ParseString.SplitFirst(argsAndCount, ',')[1];
							}
							while (argsAndCount.StartsWith(" ")){
								argsAndCount = ParseString.SplitFirst(argsAndCount, ' ')[1];
							}

						}
						item._count = ParseString.SplitFirst(argsAndCount, ')')[1];
						
						break;
					}
				}
				else{
					item._value = str;


				}


			}
			return item;
		}


		public string toString(string tab){
			string res =  tab + _op;

			if(_op != "True" && _op != "False")
				res += "(\n";
			int i;

			i = _childB.Count;
			bool diff1 = false;
			bool diff2 = false;
			foreach( ParseBoolExp b in _childB){

				res += b.toString(tab + "\t");
				i--;
				if( i > 0)
					res += ",\n ";
				diff1 = true;
			}

			i = _childS.Count;
			foreach( ParseStringExp b in _childS){
				if(diff1){
					res+=",\n";
					diff1 = false;
				}
				res += b.toString(tab + "\t");
				i--;
				if( i > 0)
					res += ",\n ";
				diff2 = true;
			}
			
			i = _childF.Count;
			foreach( ParseFloatExp b in _childF){
				if(diff1 || diff2){
					res+=",\n";
					diff1 = false;
					diff2 = false;
				}

				res += b.toString(tab + "\t");
				i--;
				if( i > 0)
					res += ",\n ";
			}


			if(_op != "True" && _op != "False")
				res += ")";

			return res;
		}


		public bool Evaluate(LinkedList<SensorUpdator.KeyValue> LinkKeyValues){
			bool res;
			switch (_op){
			case "True":
				return true;

			case "False":
				return false;

			case "Or":
				res = false;
				foreach(ParseBoolExp p in _childB){
					res |= p.Evaluate(LinkKeyValues);
				}
				return res;

			case "And":
				res = true;
				foreach(ParseBoolExp p in _childB){
					res &= p.Evaluate(LinkKeyValues);
				}
				return res;

			case "Not":
				return !_childB.First.Value.Evaluate(LinkKeyValues);

			case "If":
				if(_childB.First.Value.Evaluate(LinkKeyValues)){
					return _childB.First.Next.Value.Evaluate(LinkKeyValues);
				}
				else{
					return _childB.Last.Value.Evaluate(LinkKeyValues);
				}
			case "EQ":
				foreach(SensorUpdator.KeyValue k in LinkKeyValues){
					//Debug.Log(k.Key);
				}
				return _childS.First.Value.Evaluate(LinkKeyValues) == _childS.Last.Value.Evaluate(LinkKeyValues);
				
			case "NEQ":
				return !(_childS.First.Value.Evaluate(LinkKeyValues) == _childS.Last.Value.Evaluate(LinkKeyValues));
				
			case "==":
				return _childF.First.Value.Evaluate(LinkKeyValues) == _childF.Last.Value.Evaluate(LinkKeyValues);
				
			case "=!":
				return !(_childF.First.Value.Evaluate(LinkKeyValues) == _childF.Last.Value.Evaluate(LinkKeyValues));
				
			case "<=":
				return _childF.First.Value.Evaluate(LinkKeyValues) <= _childF.Last.Value.Evaluate(LinkKeyValues);
				
			case "<":
				return _childF.First.Value.Evaluate(LinkKeyValues) < _childF.Last.Value.Evaluate(LinkKeyValues);
				
			case ">=":
				return _childF.First.Value.Evaluate(LinkKeyValues) >= _childF.Last.Value.Evaluate(LinkKeyValues);
				
			case ">":
				return _childF.First.Value.Evaluate(LinkKeyValues) > _childF.Last.Value.Evaluate(LinkKeyValues);

			default:
				if(_value.Contains("#")){
					try{
						return bool.Parse(PString.RemplaceSpecialChar(_value,LinkKeyValues));
					}
					catch{
						return false;
					}
				}
				else{
					try{
							return bool.Parse(_value);
					}
					catch{
						return true;
					}
				}

			}

		}
	}
}
