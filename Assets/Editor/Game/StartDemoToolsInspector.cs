using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StartDemo))]
public class StartDemoToolsInspector : Editor
{
    private StartDemo _getStartDemo { get { return target as StartDemo; } }
    private bool isload = false;
    private bool isobjview = false;
    private bool istexview = false;
    private bool resetGame = false;


    private ConfigTeamParams configTeamA;
    private ConfigTeamParams configTeamB;

    private void initConfig() {
        EditorGUILayout.BeginHorizontal();
        resetGame = EditorGUILayout.Toggle("游戏重置：", resetGame);
        if (resetGame) {
            _getStartDemo.resetGame();
            resetGame = false;
        }
        EditorGUILayout.EndHorizontal();
        _initConfig(configTeamA, "A");
        _initConfig(configTeamB, "B");
    }
    private void _initConfig(ConfigTeamParams configTeam, string str) {
        if (configTeam == null) {
            init();
            return;
        }
        EditorGUILayout.BeginHorizontal();
        // EditorGUILayout.Space();
        EditorGUILayout.LabelField("部队"+str+"配置参数:",GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        configTeam.show = EditorGUILayout.Toggle("出现：", configTeam.show);
        EditorGUILayout.EndHorizontal();

        // if (showA)
        //     _getStartDemo.setBattleState(2);
        // else
        //     _getStartDemo.setBattleState(0);

        EditorGUILayout.BeginHorizontal();
        // configTeam.roleState = EditorGUILayout.Toggle("移动：", configTeam.roleState);
        ROLE_STATE move = (ROLE_STATE)EditorGUILayout.EnumPopup("动作：", configTeam.roleState);
        // if (GUILayout.Button("Create"))
        //     InstantiatePrimitive(configTeam.roleState);
        if (configTeam.roleState != move) {
            configTeam.roleState = move;
            switch (configTeam.roleState) {
                case ROLE_STATE.IDLE:
                break;
                case ROLE_STATE.WALK:
                    _getStartDemo.Move();
                break;
                default:
                break;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        configTeam.startPos = EditorGUILayout.Vector3Field("起点：", configTeam.startPos);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        configTeam.endPos = EditorGUILayout.Vector3Field("终点：", configTeam.endPos);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        configTeam.moveSpeed = EditorGUILayout.FloatField("移速：", configTeam.moveSpeed);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        configTeam.health = EditorGUILayout.IntField("生命值：", configTeam.health);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        configTeam.number = EditorGUILayout.IntField("数量：", configTeam.number);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        Vector2Int v2i = EditorGUILayout.Vector2IntField("阵容范围(长宽)：", configTeam.teamRect);
        if (configTeam.teamRect != v2i) {
            Debug.Log("configTeam.teamRect=" + configTeam.teamRect + ", v2i=" + v2i);
            configTeam.teamRect = v2i;
            configTeam.number = v2i.x * v2i.y;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        configTeam.modelName = EditorGUILayout.TextField("模型名：", configTeam.modelName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        configTeam.formation = (FORMATION)EditorGUILayout.EnumPopup("阵容：", configTeam.formation);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        configTeam.randomGenerate = EditorGUILayout.Toggle("阵容是否随机生成：", configTeam.randomGenerate);
        EditorGUILayout.EndHorizontal();

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("reload:",GUILayout.Width(105));
        this.isload = EditorGUILayout.Toggle(this.isload);
        if (this.isload){
            base.OnInspectorGUI();
        } else {
            init();
        }

        initConfig();
        Repaint();
    }

    private void init() {
        Debug.Log("OnInspectorGUI Init=" + this.isload);
        _getStartDemo.InitParams();
        configTeamA = _getStartDemo.getConfigTeamA();
        configTeamB = _getStartDemo.getConfigTeamB();

        this.isload = true;
    }

    //  void _lithgmeshgui(){
    //     LightingMapDataInfo[] aa = _getStartDemo.get_renderinfo();
    //     for (int i = 0 ; i < aa.Length; i ++){
    //         EditorGUILayout.BeginVertical();
    //             // EditorGUILayout.LabelField("["+i.ToString()+"]",GUILayout.Width(20));
    //             EditorGUILayout.BeginHorizontal();
    //                 EditorGUILayout.LabelField("["+aa[i].lightindex.ToString()+"]Render:",GUILayout.Width(65));
    //                 // EditorGUILayout.LabelField("color",GUILayout.Width(20));
    //                 switch(aa[i].type){
    //                     case 1:
    //                         EditorGUILayout.ObjectField(aa[i].meshrender,typeof(MeshRenderer),false);
    //                     break;
    //                     case 2:
    //                         EditorGUILayout.ObjectField(aa[i].terrain,typeof(Terrain),false);
    //                     break;
    //                 }
                   
                    
    //                 // EditorGUILayout.LabelField("Dir",GUILayout.Width(20));
    //                 // EditorGUILayout.ObjectField(aa[i].lightmapDir,typeof(Texture2D),false);
    //             EditorGUILayout.EndHorizontal();
    //         EditorGUILayout.EndVertical();
    //     }

    // }
    // void _lithgdatagui(){
    //     Texture2D[] aa = _getStartDemo.get_renderDatainfo().ToArray();
    //     for (int i = 0 ; i < aa.Length; i ++){
    //         EditorGUILayout.BeginVertical();
    //             // EditorGUILayout.LabelField("["+i.ToString()+"]",GUILayout.Width(20));
    //             EditorGUILayout.BeginHorizontal();
    //                 EditorGUILayout.LabelField("["+i.ToString()+"]color:",GUILayout.Width(65));
    //                 // EditorGUILayout.LabelField("color",GUILayout.Width(20));
    //                 EditorGUILayout.ObjectField(aa[i],typeof(Texture2D),false);
    //                 // EditorGUILayout.LabelField("Dir",GUILayout.Width(20));
    //                 // EditorGUILayout.ObjectField(aa[i].lightmapDir,typeof(Texture2D),false);
    //             EditorGUILayout.EndHorizontal();
    //         EditorGUILayout.EndVertical();
    //     }

    // }


}