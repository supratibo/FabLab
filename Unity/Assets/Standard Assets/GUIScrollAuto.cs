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


namespace AssemblyCSharpfirstpass
{
	public class GUIScrollAuto {
		private Vector2 Scroll = Vector2.zero;
		private int end = 0;

		public GUIScrollAuto (){
		}

		public void Begin(int y, int width, int height){

			Scroll = GUI.BeginScrollView(
				new Rect(0,y, width,height - y),
				Scroll, 
				new Rect(0,y, width - 17,end - y),
				false, false
				);
		}

		public void End(int _end){
			end = _end;
			GUI.EndScrollView();

		}
	}
}
