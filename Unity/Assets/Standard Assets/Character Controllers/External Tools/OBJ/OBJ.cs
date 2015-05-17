using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using AssemblyCSharpfirstpass;

public class OBJ : MonoBehaviour {
	
	public string objPath = "";
	
	/* OBJ file tags */
	private const string O 	= "o";
	private const string G 	= "g";
	private const string V 	= "v";
	private const string S 	= "s";
	private const string VT = "vt";
	private const string VN = "vn";
	private const string F 	= "f";
	private const string MTL = "mtllib";
	private const string UML = "usemtl";

	/* MTL file tags */
	private const string NML = "newmtl";
	private const string NS = "Ns"; // Shininess
	private const string KA = "Ka"; // Ambient component (not supported)
	private const string KD = "Kd"; // Diffuse component
	private const string KS = "Ks"; // Specular component
	private const string D = "d"; 	// Transparency (not supported)
	private const string TR = "Tr";	// Same as 'd'
	private const string ILLUM = "illum"; // Illumination model. 1 - diffuse, 2 - specular
	private const string MAP_KD = "map_Kd"; // Diffuse texture (other textures are not supported)
	
	private string basepath;
	private string mtllib;
	private GeometryBuffer buffer;

	void SetPath(string s){
				objPath = s;
		}

	void Start ()
	{
	}


	public void DoRun(){
		buffer = new GeometryBuffer ();
		StartCoroutine (Load (objPath));
	}

	public IEnumerator Load(string path) {
		basepath = (path.IndexOf("/") == -1) ? "" : path.Substring(0, path.LastIndexOf("/") + 1);
		
//		WWW loader = new WWW(path);
//		yield return loader;
//		SetGeometryData(loader.text);
		SetGeometryData(System.IO.File.ReadAllText (path));

		if(hasMaterials) {
//			loader = new WWW(basepath + mtllib);
//			yield return loader;
//			SetMaterialData(loader.text);
			SetMaterialData(System.IO.File.ReadAllText (basepath + mtllib));

			foreach(MaterialData m in materialData) {
				if(m.diffuseTexPath != null) {
					WWW texloader = new WWW(basepath + m.diffuseTexPath);
					yield return texloader;
					m.diffuseTex = texloader.texture;
				}
			}
		}
		Build();

	}



	private void SetGeometryData(string data) {
		data = data.Replace(" \r\n","\n");
		string[] lines = data.Split("\n".ToCharArray());

		buffer.PushUV(new Vector2(0, 0));

		for(int i = 0; i < lines.Length; i++) {
			string l = lines[i];
		
			if(l.IndexOf("#") != -1) l = l.Substring(0, l.IndexOf("#"));

			if(l == "" || l == " "){
				continue;
			}


			string[] p = l.Split(" ".ToCharArray());
			int y = 0;
			for(int x = 0; x < p.Length;x++){
				if(p[x] == ""){
					y++;
				}
			}
			
			switch(p[0]) {
			case O:
				buffer.PushObject(p[1].Trim());
				buffer.PushGroup(p[1].Trim());
					break;
			case G:
			//case S:
				buffer.PushObject(p[1].Trim());
				buffer.PushGroup(p[1].Trim());
					break;
				case V:
					buffer.PushVertex( new Vector3( cf(p[1]), cf(p[2]), -cf(p[3]) ) );
					break;
				case VT:
					buffer.PushUV(new Vector2( cf(p[1]), cf(p[2]) ));
					break;
				case VN:
					buffer.PushNormal(new Vector3( cf(p[1]), cf(p[2]), -cf(p[3]) ));
					break;
				case F:
				for (int k = 2 ; k < p.Length - 1; k++){

					int j;
					/// 1ft point :
					j = 1;
					string[] c0 = p[p.Length - j].Trim().Split("/".ToCharArray());
					FaceIndices fi0 = new FaceIndices();
					fi0.vi = int.Parse(c0[0])-1;										// Set Point
					if(c0.Length > 1 && c0[1] != "") fi0.vu = 0; 				// ci(c[1])-1;	// Set Texture
					if(c0.Length > 2 && c0[2] != "") fi0.vn = int.Parse(c0[2])-1;		// Set Normal

					/// 2nd Point :
					j = k;
					string[] c1 = p[p.Length - j].Trim().Split("/".ToCharArray());
					FaceIndices fi1 = new FaceIndices();
					fi1.vi = int.Parse(c1[0])-1;										// Set Point
					if(c1.Length > 1 && c1[1] != "") fi1.vu = 0; 				// ci(c[1])-1;	// Set Texture
					if(c1.Length > 2 && c1[2] != "") fi1.vn = int.Parse(c1[2])-1;		// Set Normal

					/// 3rd Point : 
					j = k + 1;
					string[] c2 = p[p.Length - j].Trim().Split("/".ToCharArray());
					FaceIndices fi2 = new FaceIndices();
					fi2.vi = int.Parse(c2[0])-1;										// Set Point
					if(c2.Length > 1 && c2[1] != "") fi2.vu = 0; 				// ci(c[1])-1;	// Set Texture
					if(c2.Length > 2 && c2[2] != "") fi2.vn = int.Parse(c2[2])-1;		// Set Normal

					/// Triangle
					buffer.PushFace(fi0);
					buffer.PushFace(fi1);
					buffer.PushFace(fi2);

					/// Triangle (reverse)
					// buffer.PushFace(fi0);
					// buffer.PushFace(fi2);
					// buffer.PushFace(fi1);


				}
				
				break;
				case MTL:
			//		mtllib = p[1].Trim();
					break;
				case UML:
					buffer.PushMaterialName(p[1].Trim());
					break;
			}
		}
		
		// buffer.Trace();
	}
	
	private float cf(string v) {
		if (v == "")
			return 0;
		else
			return float.Parse(v);
	}

	private bool hasMaterials {
		get {
			return mtllib != null;
		}
	}
	
	/* ############## MATERIALS */
	private List<MaterialData> materialData;
	private class MaterialData {
		public string name;
		public Color ambient;
   		public Color diffuse;
   		public Color specular;
   		public float shininess;
   		public float alpha;
   		public int illumType;
   		public string diffuseTexPath;
   		public Texture2D diffuseTex;
	}
	
	private void SetMaterialData(string data) {
		string[] lines = data.Split("\n".ToCharArray());
		
		materialData = new List<MaterialData>();
		MaterialData current = new MaterialData();
		
		for(int i = 0; i < lines.Length; i++) {
			string l = lines[i];
			
			if(l.IndexOf("#") != -1) l = l.Substring(0, l.IndexOf("#"));
			string[] p = l.Split(" ".ToCharArray());
			
			switch(p[0]) {
				case NML:
					current = new MaterialData();
					current.name = p[1].Trim();
					materialData.Add(current);
				break;
				case KA:
					current.ambient = gc(p);
					break;
				case KD:
					current.diffuse = gc(p);
					break;
				case KS:
					current.specular = gc(p);
					break;
				case NS:
					current.shininess = cf(p[1]) / 1000;
					break;
				case D:
				case TR:
					current.alpha = cf(p[1]);
					break;
				case MAP_KD:
					current.diffuseTexPath = p[1].Trim();
					break;
				case ILLUM:
					current.illumType = int.Parse(p[1]);
					break;
					
			}
		}	
	}
	
	private Material GetMaterial(MaterialData md) {

		Material m;
		
		if(md.illumType == 2) {
			m =  new Material(Shader.Find("Specular"));
			m.SetColor("_SpecColor", md.specular);
			m.SetFloat("_Shininess", md.shininess);
		} else {
			m =  new Material(Shader.Find("Diffuse"));
		}

		m.SetColor("_Color", md.diffuse);
		
		if(md.diffuseTex != null) m.SetTexture("_MainTex", md.diffuseTex);
		
		return m;
	}
	
	private Color gc(string[] p) {
		return new Color( cf(p[1]), cf(p[2]), cf(p[3]) );
	}

	int inti = 0;

	private void Build() {

		Dictionary<string, Material> materials = new Dictionary<string, Material>();
		materials.Add("default", new Material(Shader.Find("VertexLit")));

//		if (hasMaterials) {
//			foreach(MaterialData md in materialData) {
//				materials.Add("default", new Material(Shader.Find("VertexLit")));
//				//materials.Add(md.name, GetMaterial(md));
//		
//			}
//		} else {
//			materials.Add("default", new Material(Shader.Find("VertexLit")));
//		
//		}

		GameObject[] ms = new GameObject[buffer.numObjects];

		for(int i = 0; i < buffer.numObjects; i++) {
			GameObject go = new GameObject();
				go.transform.parent = gameObject.transform;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent(typeof(MeshRenderer));
				go.AddComponent(typeof(ObjSettingItem));
				ms[i] = go;
		}

		buffer.PopulateMeshes(ms, materials);
		for(int i = 0; i < buffer.numObjects; i++) {
			ms[i].transform.localScale = Vector3.one;
			ms[i].AddComponent(typeof(MeshCollider));


		}

	}
}








