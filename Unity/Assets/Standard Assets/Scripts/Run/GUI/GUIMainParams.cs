using UnityEngine;
using System.Collections;

public class GUIMainParams : MonoBehaviour {
	public string str = "1:2;3:4";
	private string NewParam = "";

	private string Name = "PARAMS";


	private string SeparatorLine = ";";
	private string SeparatorFeild = ":";
	
	private bool showAdvencedSettings = false;
	private bool autoUpdate = false;

	private Vector2 Scroll1 = Vector2.zero;
	private int sizeScroll = 0;



	private string Find = "";

	// Use this for initialization
	void Start () {
		NewParam = "" + SeparatorFeild;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		if(MainAddSensor.displayGUI == 4){

			int currentY;
			int largeur;
			int size = 0;
			
			
			GUI.BeginGroup(new Rect (0,0,2* StaticMemory._Margin + StaticMemory._SizeX, 3 * StaticMemory._Margin + StaticMemory._SizeX));
			
			
			/////////////////////////////////////AFFICHAGE
			// Groupe : Caption
			GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin, StaticMemory._SizeX, StaticMemory._Caption));
			GUI.DrawTexture(new Rect(StaticMemory._SizeX - 3*StaticMemory._Caption,0, StaticMemory._Caption*3, StaticMemory._Caption), StaticMemory.A4HTexture, ScaleMode.StretchToFill, true, 0);
			GUI.Box(new Rect(0,0, StaticMemory._SizeX, StaticMemory._Caption), "");
			currentY = 0;
			largeur  = StaticMemory._SizeX - 2 * StaticMemory._Margin;
			
			if(GUI.Button(new Rect(StaticMemory._Margin, StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2,  StaticMemory._hauteurChamp, StaticMemory._hauteurChamp), " < ") ||Input.GetKey(KeyCode.Escape)){
				MainAddSensor.displayGUI = 1;
			}
			
			GUI.Box(new Rect(StaticMemory._Margin + StaticMemory._SizeX/4,StaticMemory._Caption/2 - StaticMemory._hauteurChamp/2, largeur/2, StaticMemory._hauteurChamp),"OpenHab Script's parametres");
			GUI.EndGroup();
			
			// Groupe : Corps de la GUI
			GUI.BeginGroup (new Rect(StaticMemory._Margin,StaticMemory._Margin*2 + StaticMemory._Caption, StaticMemory._SizeX , StaticMemory._SizeY));
			
			// Groupe : 1re colonne
			GUI.BeginGroup (new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY ));
			GUI.Box(new Rect(0,0, StaticMemory._SizeX / 4, StaticMemory._SizeY ), "");
			currentY = 0;
			largeur  = StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
			size = (StaticMemory._SizeY/2 - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
			
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Settings");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			GUI.Label(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp), "Name");
			currentY += StaticMemory._hauteurChamp;
			Name = GUI.TextField(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp), Name);
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			GUI.Label(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp), "Find");
			currentY += StaticMemory._hauteurChamp;
			Find = GUI.TextField(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp), Find).Replace(SeparatorFeild,"").Replace(SeparatorLine,"");;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;



			if(GUI.Button(new Rect(StaticMemory._Margin , currentY,  largeur, (int) (1.5f * StaticMemory._hauteurChamp)), "Get")){

			}
			currentY += (int) (1.5f * StaticMemory._hauteurChamp) + StaticMemory._hauteurEntre;
			autoUpdate = GUI.Toggle(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp),autoUpdate, "Auto Update");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;


			if(GUI.Button(new Rect(StaticMemory._Margin , currentY,  largeur, 3 * StaticMemory._hauteurChamp), "Set")){
				
			}

			currentY += 3 * StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;


			showAdvencedSettings = GUI.Toggle(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp),showAdvencedSettings, "More");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			if(showAdvencedSettings){
				GUI.Label(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp), "Line seperator");
				currentY += StaticMemory._hauteurChamp;
				SeparatorLine = GUI.TextField(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp), SeparatorLine);
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

				GUI.Label(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp), "Element separator");
				currentY += StaticMemory._hauteurChamp;
				SeparatorFeild = GUI.TextField(new Rect(StaticMemory._Margin , currentY,  largeur, StaticMemory._hauteurChamp), SeparatorFeild);
				currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;


			}


			GUI.EndGroup();


			// Groupe : 2e colonne
			GUI.BeginGroup (new Rect(StaticMemory._SizeX / 4,0, 3 * StaticMemory._SizeX / 4, StaticMemory._SizeY ));
			GUI.Box(new Rect(0,0, 3 * StaticMemory._SizeX / 4, StaticMemory._SizeY ), "");
			currentY = 0;
			largeur  = 3 * StaticMemory._SizeX / 4 - 2 * StaticMemory._Margin;
			size = (StaticMemory._SizeY/2 - StaticMemory._hauteurChamp) / (StaticMemory._hauteurChamp + StaticMemory._hauteurEntre);	
			
			GUI.Box(new Rect(0, currentY, largeur + 2 * StaticMemory._Margin, StaticMemory._hauteurChamp), "Edition");
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;
			currentY += StaticMemory._hauteurChamp + StaticMemory._hauteurEntre;

			Scroll1 = GUI.BeginScrollView(
				new Rect(0,currentY, 3 * StaticMemory._SizeX / 4, StaticMemory._SizeY - currentY),
				Scroll1, 
				new Rect(0,currentY, 3 * StaticMemory._SizeX / 4 - 17, sizeScroll - currentY),
				false, false
				);

			float[] Size = {.25f,.65f};
			string res = "";
			string[] lines = str.Replace(SeparatorLine,"§").Split('§');
			int currX;
			string[] el;
			for (int j = 0; j < lines.Length ; j++){
				if (lines[j].Contains(Find) && lines[j] != ""){
					if(GUI.Button(new Rect(StaticMemory._Margin + largeur * .9f,currentY, largeur * .1f, StaticMemory._hauteurChamp),"x")){
						continue;
					}
					el = lines[j].Replace(SeparatorFeild,"§").Split('§');
					currX = StaticMemory._Margin;
					for (int i = 0; i< Size.Length; i++){
						string s = GUI.TextField(new Rect(currX,currentY, Size[i] * largeur, StaticMemory._hauteurChamp), el[i]);
						s = s.Replace(SeparatorFeild,"").Replace(SeparatorLine,"");
						if(i == Size.Length -1){
							res += s;
						}
						else{
								res += s + SeparatorFeild;
						}
						currX += (int) (Size[i] * largeur);
	
					}
					currentY += StaticMemory._hauteurChamp;
					if(j != lines.Length -1){
						res += SeparatorLine;
					}
				}
				else{
					if(j == lines.Length -1){
						res += lines[j];
					}
					else{
						res += lines[j] + SeparatorLine;
					}
				}
			}

			currentY += StaticMemory._hauteurEntre;

			currX = StaticMemory._Margin;
			el = NewParam.Replace(SeparatorFeild,"§").Split('§');
			NewParam = "";
			GUI.SetNextControlName("1");
			for (int i = 0; i< Size.Length; i++){
				string s = GUI.TextField(new Rect(currX,currentY, Size[i] * largeur, StaticMemory._hauteurChamp), el[i]);
				s = s.Replace(SeparatorFeild,"").Replace(SeparatorLine,"");
				if(i == Size.Length -1){
					NewParam += s;
				}
				else{
					NewParam += s + SeparatorFeild;
				}
				currX += (int) (Size[i] * largeur);
				
			}
			currentY += StaticMemory._hauteurChamp;
			if(GUI.Button(new Rect(StaticMemory._Margin,currentY, largeur * .1f, StaticMemory._hauteurChamp),"Add")){
				if(NewParam.Replace(SeparatorFeild,"#").Split('#')[0] != ""){
					res += SeparatorLine + NewParam;
					NewParam = "" + SeparatorFeild;
					GUI.FocusControl("1");
				}
			}
			currentY += StaticMemory._hauteurChamp;

			//res.Replace(SeparatorFeild + SeparatorLine, SeparatorLine);
			//res.Replace(SeparatorLine + SeparatorLine, SeparatorLine);
			//str = res.Remove(res.LastIndexOf(SeparatorLine), Find.Length);
			str = res;
			
			sizeScroll = currentY;
			GUI.EndScrollView();


			GUI.EndGroup();
			
			GUI.EndGroup();
			GUI.EndGroup();
		}



	}

}
