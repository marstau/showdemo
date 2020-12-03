using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Loadlightmapdata))]
public class LoadlightmapdataToolsInspector : Editor
{
    private Loadlightmapdata _getlightmapdata { get { return target as Loadlightmapdata; } }
    private bool isload = false;
    private bool isobjview = false;
    private bool istexview = false;
    public override void OnInspectorGUI()
    {     
        EditorGUILayout.BeginHorizontal();
        // EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Mode:",GUILayout.Width(105));
        this.isload = EditorGUILayout.Toggle(this.isload);
        EditorGUILayout.EndHorizontal();
        if (this.isload){
            base.OnInspectorGUI();
        }
        else{
            // EditorGUILayout.LabelField("对象池：");
            isobjview = EditorGUILayout.Foldout(isobjview,"对象池：");
            if (isobjview){
                this._lithgmeshgui(); 
            }         
            // EditorGUILayout.LabelField("LightMap池：");
            istexview = EditorGUILayout.Foldout(istexview,"LightMap池：");
            if (istexview){
                this._lithgdatagui();
            }        
        }   
        this._gui(); 
        Repaint();
    }
   
    void _gui(){
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Get LightMap Data")){
            _getlightmapdata.Init();
        }
        if(GUILayout.Button("Set LightMap Data")){
            _getlightmapdata.setLightMapData(_getlightmapdata.get_renderDatainfo());
        }
        
        EditorGUILayout.EndHorizontal();        
    }
     void _lithgmeshgui(){
        LightingMapDataInfo[] aa = _getlightmapdata.get_renderinfo();
        for (int i = 0 ; i < aa.Length; i ++){
            EditorGUILayout.BeginVertical();
                // EditorGUILayout.LabelField("["+i.ToString()+"]",GUILayout.Width(20));
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("["+aa[i].lightindex.ToString()+"]Render:",GUILayout.Width(65));
                    // EditorGUILayout.LabelField("color",GUILayout.Width(20));
                    switch(aa[i].type){
                        case 1:
                            EditorGUILayout.ObjectField(aa[i].meshrender,typeof(MeshRenderer),false);
                        break;
                        case 2:
                            EditorGUILayout.ObjectField(aa[i].terrain,typeof(Terrain),false);
                        break;
                    }
                   
                    
                    // EditorGUILayout.LabelField("Dir",GUILayout.Width(20));
                    // EditorGUILayout.ObjectField(aa[i].lightmapDir,typeof(Texture2D),false);
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

    }
    void _lithgdatagui(){
        Texture2D[] aa = _getlightmapdata.get_renderDatainfo().ToArray();
        for (int i = 0 ; i < aa.Length; i ++){
            EditorGUILayout.BeginVertical();
                // EditorGUILayout.LabelField("["+i.ToString()+"]",GUILayout.Width(20));
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("["+i.ToString()+"]color:",GUILayout.Width(65));
                    // EditorGUILayout.LabelField("color",GUILayout.Width(20));
                    EditorGUILayout.ObjectField(aa[i],typeof(Texture2D),false);
                    // EditorGUILayout.LabelField("Dir",GUILayout.Width(20));
                    // EditorGUILayout.ObjectField(aa[i].lightmapDir,typeof(Texture2D),false);
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

    }


}