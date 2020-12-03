using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LightingMapDataInfo{
	
	[HideInInspector]public int type;
	public Renderer meshrender = null;
	public Terrain terrain = null;

	public int lightindex;
	public Vector4 lightoffsetscale;

	public void setvalue(){
		switch(this.type){
			case 1:
				this.meshrender.lightmapIndex = this.lightindex;
				this.meshrender.lightmapScaleOffset =this. lightoffsetscale;
			break;
			case 2:
				this.terrain.lightmapIndex = this.lightindex;;
				this.terrain.lightmapScaleOffset = this. lightoffsetscale;;
			break;	
			}
	}
	
}

public class Loadlightmapdata : MonoBehaviour {

	[SerializeField]
	private List<LightingMapDataInfo> _renderinfo = new List<LightingMapDataInfo>();


	[SerializeField]
	private List<Texture2D> _renderDatainfo = new List<Texture2D>() ;
	void Awake(){
		this.setLightMapData(this._renderDatainfo);
	}
	
	void Start() {
		inneStaticBatchingCombine(this.gameObject);
		StaticBatchingUtility.Combine(this.gameObject);
	}

	private void inneStaticBatchingCombine(GameObject root)
	{
		List<GameObject> go_list = new List<GameObject>();
		foreach (Transform childTransform in root.transform)
		{
			inneStaticBatchingCombine(childTransform.gameObject);
			go_list.Add(childTransform.gameObject);
		}
		if (go_list.Count > 0) {
			GameObject[] gos = new GameObject[go_list.Count];
			for (int i = 0; i < go_list.Count; i++)
			{
				gos[i] = go_list[i];
			}
			StaticBatchingUtility.Combine(gos, root);
			Debug.Log("合并批次 数量"+gos.Length);
		}
	}

	public List<Texture2D> get_renderDatainfo(){
		return this._renderDatainfo;
	}
	/// <summary>
	/// 对贴图数组赋值
	/// </summary>
	public void set_renderDatainfo( List<Texture2D> intexturelist){
		this._renderDatainfo = new List<Texture2D>(intexturelist.ToArray());
	}
	/// <summary>
	/// 对贴图数组赋值
	/// </summary>
	public void set_renderDatainfo( Texture2D[] intexturelist){
		this._renderDatainfo = new List<Texture2D>(intexturelist);
	}
	public LightingMapDataInfo[] get_renderinfo(){
		return this._renderinfo.ToArray();
	}
	public void Init(){
		this._renderinfo.Clear();
		this._renderDatainfo.Clear();
		int texcount = LightmapSettings.lightmaps.Length;

		MeshRenderer[] r = this.gameObject.GetComponentsInChildren<MeshRenderer>();

		foreach (MeshRenderer i in r){
			i.gameObject.name = i.gameObject.name.Replace("_dite2","");
			MeshFilter mf = i.gameObject.GetComponent<MeshFilter>() as MeshFilter;

			Debug.Log(mf.sharedMesh);
			if (mf.sharedMesh == null){
				i.gameObject.SetActive(false);
				i.gameObject.name = i.gameObject.name  + "_dite";
				continue;
			}
			if (i.lightmapIndex == -1){
				i.gameObject.SetActive(false);
				i.gameObject.name = i.gameObject.name  + "_dite";
				continue;
			}
			Texture2D tdata = LightmapSettings.lightmaps[i.lightmapIndex].lightmapColor as Texture2D;

			int index = this._renderDatainfo.IndexOf(tdata);
			if (index == -1){
				this._renderDatainfo.Add(tdata);
				index = this._renderDatainfo.Count - 1;
			}
			LightingMapDataInfo rrr = new LightingMapDataInfo();
			rrr.type = 1;
			rrr.meshrender= i;
			rrr.lightoffsetscale = i.lightmapScaleOffset;	
			rrr.lightindex = index;
			this._renderinfo.Add(rrr);
		}
		Terrain[] tr = this.gameObject.GetComponentsInChildren<Terrain>();
		foreach (Terrain i in tr){
			if (i.lightmapIndex == -1){
				i.gameObject.SetActive(false);
				i.gameObject.name = i.gameObject.name  + "_dite";
				continue;
			}
			Texture2D tdata = LightmapSettings.lightmaps[i.lightmapIndex].lightmapColor as Texture2D;

			int index = this._renderDatainfo.IndexOf(tdata);
			if (index == -1){
				this._renderDatainfo.Add(tdata);
				index = this._renderDatainfo.Count - 1;
			}
			LightingMapDataInfo rrr = new LightingMapDataInfo();
			rrr.type = 2;
			rrr.terrain= i;	
			rrr.lightoffsetscale = i.lightmapScaleOffset;	
			rrr.lightindex = index;
			this._renderinfo.Add(rrr);
		}

		
	}

	/// <summary>
	/// 对场景进行烘培灯光性息的赋值\n
	/// _renderDatainfo: LIST类型的贴图组
	/// </summary>
	public void setLightMapData (List<Texture2D> _renderDatainfo)
	{
		if (_renderinfo == null || _renderinfo.Count == 0)
			return;			
		// //缔特
		// //整理灯光贴图
		// //创建临时贴图list，并复制导入数据进入
		//	LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
		// List<Texture2D>  Templightmaps = new List<Texture2D>(this._renderDatainfo.ToArray());
		// // this._renderDatainfo.ForEach(i => Templightmaps.Add(i));
		// // Templightmaps = this._renderDatainfo; //List<Texture2D> 
		// //遍历当前场景数据，如果临时list里有就删除list里的数据，并按当前id插入进去，得到最终贴图的顺序
		// for (int i = 0;i<LightmapSettings.lightmaps.Length;i++){
		// 	Texture2D nowTex = LightmapSettings.lightmaps[i].lightmapColor;
		// 	int ia = Templightmaps.IndexOf(nowTex);
		// 	if (ia != -1)
		// 	{
		// 		Templightmaps.RemoveAt(ia);
		// 	}
		// 	Templightmaps.Insert(i,LightmapSettings.lightmaps[i].lightmapColor);
		// }
		// //把贴图赋值给当前场景
		// List<LightmapData> aa = new List<LightmapData>();		
		// Templightmaps.ForEach(i => aa.Add(this.setNewLightmapdata(i)));
		// LightmapSettings.lightmaps = aa.ToArray();
		// //赋值模型性息
		// for (int i=0;i<_renderinfo.Count;i++)
		// {	
		//  _renderinfo[i];.setvalue();	

		// 	// var info = _renderinfo[i];	
		//	// info.renderer.lightmapIndex = xuhao;
		//	// info.renderer.lightmapScaleOffset = info.lightoffsetscale;
		// }

		
		//如果不考虑贴图问题的话，可以直接使用这个。
		//把对象中记录的所有贴图都导入场景（无视原有的贴图，是强制性的替换）
		LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
		List<LightmapData> aa0 = new List<LightmapData>();		
		_renderDatainfo.ForEach(i => aa0.Add(this.setNewLightmapdata(i)));
		LightmapSettings.lightmaps = aa0.ToArray();
		
		//赋值模型性息 
		for (int i=0;i<_renderinfo.Count;i++)
		{
			_renderinfo[i].setvalue();
			// info.renderer.lightmapIndex = info.lightindex;
			// info.renderer.lightmapScaleOffset = info.lightoffsetscale;
		}

		
	}

	private LightmapData setNewLightmapdata(Texture2D a){
		LightmapData tt = new LightmapData();
		tt.lightmapColor = a;
		return tt;
	}

}

