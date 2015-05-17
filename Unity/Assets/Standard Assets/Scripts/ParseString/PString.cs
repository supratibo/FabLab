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


namespace AssemblyCSharpfirstpass
{
	public class PString{
		private static char sep = ':';
	//	private static char _OP = '§';
		private static char sepArg = ',';
		private static string[] whiteSpace = {"\t", " "};


		private static string[] SpecialChar ={
			"\\", "_BACKSLASH",
			"{",  "_BEGINCURLYBRACKET",
			"}",  "_ENDCURLYBRACKET",
			"[",  "_BEGINBRACKET",
			"]",  "_ENDBRACKET",
			":",  "_COLON",
			",",  "_COMMA",
			"\"", "_QUOTATIONMARK",
			"#", "_SHARP"
		};

		public static string RemplaceSpecialChar (string line, bool sens){
			if(sens){
				for (int i = 0; i < SpecialChar.Length; i += 2){
					line = line.Replace(SpecialChar[i], SpecialChar[i+1]);
				}
				return line;
			}
			else{
				for (int i = 0; i < SpecialChar.Length; i += 2){
					line = line.Replace(SpecialChar[i+1], SpecialChar[i]);
				}
				return line;
			}
		}

		public static string RemplaceSpecialChar (string line, LinkedList<SensorUpdator.KeyValue> Links){
			foreach(AssemblyCSharpfirstpass.SensorUpdator.KeyValue k in Links){
				if(k.Key.Contains("##")){
					if(line.Contains(k.Key)){
					//	Debug.Log(Links[i] + "  " +  line);
						string[] args = line.Replace(k.Key,"$").Split('$');
						line = args[0];
						for (int j = 1; j < args.Length;j++){
							if(args[j].StartsWith("[")){
								int idx = 0;
								idx = int.Parse(args[j].Split('[')[1].Split(':')[0]);

								int idy = 0;
								try{
									idy = int.Parse(args[j].Split(']')[0].Split(':')[1]);
								}
								catch{}
								try{
									line += k.Value.Split('{')[1].Split('}')[0].Split(';')[idy].Split(',')[idx] + args[j].Replace(args[j].Split(']')[0] + "]", "");
								}
								catch{
										line += "?" + k.Key + args[j];
									}

							}
							else{
								line += k.Value + args[j];
							}
						}


					}
				}
				else{
					line = line.Replace(k.Key, k.Value);
				}
			}
			return line;

		}

		public static string RemoveWhiteSpaces(string arg){
			foreach(string s in whiteSpace){
				if(arg.StartsWith(s)){
					return PString.RemoveWhiteSpaces(arg.Substring(s.Length));
				}
			}
			return arg;
		}

		public static string WriteArg(string ArgName, string Value){
			return ArgName + ":" + PString.RemplaceSpecialChar(Value, true) + sepArg + whiteSpace[0];
		}



		public static string ReadArg (string ArgName, string Line){
			foreach( string arg in Line.Split(sepArg)){
				string s = PString.RemoveWhiteSpaces(arg);
				if (s.Split(sep)[0] == ArgName){
					return PString.RemplaceSpecialChar(s.Split(sep)[1], false) ;
				}
			}
			return "";
		}

		public static LinkedList<string> ReadArgs (string ArgName, string Line){
			LinkedList<string> res = new LinkedList<string>();
			foreach( string arg in Line.Split(sepArg)){
				string s = PString.RemoveWhiteSpaces(arg);
				if (s.Split(sep)[0] == ArgName){
					res.AddLast(PString.RemplaceSpecialChar(s.Split(sep)[1], false));
				}
			}
			return res;
		}


	}
}

