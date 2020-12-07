using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public enum ROLE_STATE
{
    UNKNOWN = 0,
    IDLE = 1,
    ATTACK = 2,
    WALK = 3,
    DIE = 4,
    JUMP_DIE = 5,
    PLACING = 6,
}


public enum ROLE_SHOW_STATE
{
    HIDE = 0,
    SHOW = 1,
}

public enum FORMATION {
    CIRCLE = 0,
    RECT = 1,
    TRIANGLE = 2,
    ARROW = 3,
}

public class ConfigTeamParams {
    public bool show;
    public ROLE_STATE move;
    public Vector3 startPos;
    public Vector3 endPos;
    public double moveSpeed;
    public int number;
    public Vector2Int teamRect;
    public FORMATION formation;
}

public enum GAME_STATE {
	READY = 1,
	START = 2,
	INIT_SCENE = 3,
	DURATION = 4,
	END = 5,
}
public class Role {
	private GameObject _obj;
	private ROLE_STATE _state;
	public int X{set;get;}
	public int Y{set;get;}
	public Role(GameObject obj, int _x, int _y){
		_obj = obj;
		X = _x;
		Y = _y;
	}

	public GameObject getGo() { return _obj; }
	public void move() {

		_state = ROLE_STATE.WALK;
	}
}

public class StartDemo : MonoBehaviour
{
	private GAME_STATE _gameState;
    private ConfigTeamParams _configTeamA;
    private ConfigTeamParams _configTeamB;
    private List<List<Role>> _instancesA;
    private List<List<Role>> _instancesB;


    private GameObject _roleNode1;
    private GameObject _roleNode2;
    private GameObject _placingGO;


    // Start is called before the first frame update
    void Start()
    {
        _gameState = GAME_STATE.READY;
		_roleNode1 = GameObject.Find("Terrain/RoleNode1");
		_roleNode2 = GameObject.Find("Terrain/RoleNode2");
		// EXAMPLE A: initialize with the preferences set in DOTween's Utility Panel
		﻿﻿﻿﻿﻿DOTween.Init();
		﻿﻿﻿﻿﻿// EXAMPLE B: initialize with custom settings, and set capacities immediately
		﻿﻿﻿﻿﻿DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(200, 10);

		//获取需要监听的按钮对象
		GameObject button= GameObject.Find("Canvas/Button");
		                //设置这个按钮的监听，指向本类的ButtonClick方法中。
		EventTriggerListener.Get(button.gameObject).onClick= ButtonClick;
    }

	void ButtonClick(GameObject button)
	{
		Debug.Log("GameObject "+ button.name);
        GameObject go = (GameObject)Resources.Load("Model/model1001001/model1001001");
        go = Instantiate(go);
        go.transform.parent = _roleNode2.transform;
        go.transform.localPosition = new Vector3(0, 0, 0);
		Vector3 pos= Camera.main.WorldToScreenPoint(go.transform.position );//将对象坐标换成屏幕坐标
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pos.z);//让鼠标的屏幕坐标与对象坐标一致
		Debug.Log("mousePos=" + mousePos);
        go.transform.position = Camera.main.ScreenToWorldPoint(mousePos);//将正确的鼠标屏幕坐标换成世界坐标交给物体

        _placingGO = go;
        // go.transform.Rotate(0, 180, 0, Space.Self);
	}
    void init(List<List<Role>> instances, ConfigTeamParams _config){
        switch(_config.formation) {
        	case FORMATION.RECT:
        	int width = _config.teamRect.x;
        	int height = _config.teamRect.y;
        	Vector3 starPos = new Vector3(1.0f, 0.0f, 0.0f);
        	Vector3 tarPos = new Vector3(1.0f, 0.0f, 0.0f);
        	for (int x = 0; x < width; x++) {
        		List<Role> colRoles = new List<Role>();
        		instances.Add(colRoles);
        		for (int y = 0; y < height; y++) {
        			Debug.Log("create object " + x + ", " + y);
			        GameObject go = (GameObject)Resources.Load("Model/model1001001/model1001001");
			        go = Instantiate(go);
			        go.transform.parent = _roleNode1.transform;
			        go.transform.localPosition = new Vector3(x, 0, y);
			        go.transform.Rotate(0, 180, 0, Space.Self);
		            ModelCustomData soldier_model_custom_data = go.GetComponent<ModelCustomData>();
		            soldier_model_custom_data.setPower(1);
		            soldier_model_custom_data.setLightDir(new Vector4(1.42f, 3.16f, 1.48f, 1.0f));
		            soldier_model_custom_data.setShadowColor(new Color(0.608f, 0.608f, 0.608f, 1f));
		            soldier_model_custom_data.getAnimator().Play("Idle", 0, 0);
			        // soldier_model_custom_data.getAnimator().SetLookAtPosition(tarPos);
			        Role role = new Role(go, x, y);
			        colRoles.Add(role);
        		}
        	}
        	break;
        }
    }

    void initTeamA(){
        _instancesA = new List<List<Role>>();
        init(_instancesA, _configTeamA);
    }


    void initBattleInfo(){
    	changeState(GAME_STATE.INIT_SCENE);
    	initTeamA();
    	changeState(GAME_STATE.DURATION);
    }
    // Update is called once per frame
    void Update()
    {
    	switch (_gameState) {
    		case GAME_STATE.READY:
    		if (_configTeamA != null && _configTeamA.show) {
    			changeState(GAME_STATE.START);
    		}
    		break;

    		case GAME_STATE.START:
    		initBattleInfo();
    		break;

    		case GAME_STATE.INIT_SCENE:
    		break;

    		case GAME_STATE.DURATION:
    			loopGame();
    		break;

    		case GAME_STATE.END:
    		break;
    	}
    	if (_configTeamA != null)
        	Debug.Log("_configTeamA=" + _configTeamA.show + ", _configTeamB=" + _configTeamB.number);
        else
        	Debug.Log("_configTeamA");

        if (_placingGO != null) {
			// 按下鼠标左键
        	GameObject go = _placingGO;
			Vector3 pos= Camera.main.WorldToScreenPoint(go.transform.position );//将对象坐标换成屏幕坐标
			Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pos.z);//让鼠标的屏幕坐标与对象坐标一致
			Debug.Log("mousePos=" + mousePos);
	        go.transform.position = Camera.main.ScreenToWorldPoint(mousePos);//将正确的鼠标屏幕坐标换成世界坐标交给物体
	        if (Input.GetMouseButtonDown(0)) {
	            Debug.Log ("你按下了鼠标左键");
	        }
	        // 按住鼠标左键
	        if (Input.GetMouseButton(0)) {
	            Debug.Log ("你按住了鼠标左键");
	        }
	        // 抬起鼠标左键
	        if (Input.GetMouseButtonUp(0)) {
	            Debug.Log ("你抬起了鼠标左键" + go);
	        }
        }
    }

    public void changeState(GAME_STATE state) {
    	_gameState = state;
    }

    public void Init() {
    	_configTeamA = new ConfigTeamParams();
    	_configTeamB = new ConfigTeamParams();
    }

    public void resetGame(){
    	changeState(GAME_STATE.READY);
    	Transform transform = _roleNode1.transform;
		for (int i = 0; i < transform.childCount; i++) {  
            Destroy(transform.GetChild (i).gameObject);  
        }
    }
    public ConfigTeamParams getConfigTeamA() {
    	return _configTeamA;
    }
    public ConfigTeamParams getConfigTeamB() {
    	return _configTeamB;
    }

    public void Move() {
		Debug.Log("_instancesA=" + _instancesA);
		Debug.Log("_instancesA size=" + _instancesA.Count);
    	foreach (List<Role> colRoles in _instancesA) {
    		foreach (Role role in colRoles) {
    			GameObject go = role.getGo();
			    go.transform.localPosition = new Vector3(role.X, 0, role.Y) + _configTeamA.startPos;
			    go.transform.DOMove(new Vector3(role.X, 0, role.Y) + new Vector3(0,0,-100), 1000);
		        ModelCustomData soldier_model_custom_data = go.GetComponent<ModelCustomData>();
		        soldier_model_custom_data.getAnimator().Play("Move", 0, 0);
    		}
    	}
    }


    private void loopGame() {
  //       Vector3 tarPos = new Vector3(-1.0f, 0.0f, -1.0f);

  //   	Vector3 TargetPosition = new Vector3(1000,0,1000);
		// float AvatarRange = 25;
		// Debug.Log("_instancesA=" + _instancesA);
		// Debug.Log("_instancesA size=" + _instancesA.Count);
  //   	foreach (List<Role> colRoles in _instancesA) {
  //   		foreach (Role role in colRoles) {
  //   			GameObject go = role.getGo();
  //   			Transform transform = go.transform;
  //   		}
  //   	}
    }
}
