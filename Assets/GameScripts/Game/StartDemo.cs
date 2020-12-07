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

public enum INTERACTIVE_STATE {
	INVALID = 0,
	SELECTING = 1,
	SELECTED = 2,
	PLACED = 3,
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
    public int power;
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
	public int health{set;get;}
	public int X{set;get;}
	public int Y{set;get;}
	public Role(GameObject obj, int _x, int _y, int health){
		_obj = obj;
		X = _x;
		Y = _y;
		this.health = health;
	}

	public GameObject getGo() { return _obj; }

	public void move() {
		_state = ROLE_STATE.WALK;
	}

	public void dead() {
		_state = ROLE_STATE.DIE;
		Transform transform = _obj.transform;
        ModelCustomData customData = _obj.GetComponent<ModelCustomData>();
        transform.DOKill();
        customData.getAnimator().Play("Die", 0, 0);
	}

	public bool isCollision(Vector3 pos) {
		Vector3 colliderPos = _obj.transform.position;
		float x = colliderPos.x;
		float y = colliderPos.y;
		float z = colliderPos.z;
		const float halfWidth = 0.5f;
		const float halfHeight = 0.5f;
		if (pos.x >= x - halfWidth && pos.x <= x + halfWidth && pos.z >= z - halfHeight && pos.z <= z + halfHeight) {
			return true;
		}
		return false;
	}
}

// public static boolean isCollision(int x1, int y1, int x2, int y2, int w, int h) {  
//     if (x1 >= x2 && x1 <= x2 + w && y1 >= y2 && y1 <= y2 + h) {
//         return true;  
//     }
//     return false;  
// }  

public class StartDemo : MonoBehaviour
{
	private GAME_STATE _gameState;
    private ConfigTeamParams _configTeamA;
    private ConfigTeamParams _configTeamB;
    private List<List<Role>> _instancesA;
    private List<List<Role>> _instancesB;

    private List<List<Role>> _tmpInstances;

    private List<Role> _deadInstances;


    private GameObject _roleNode1;
    private GameObject _roleNode2;
    private GameObject _placingGO;
    private GameObject _placedGO;
    private INTERACTIVE_STATE _interactiveState;


    // Start is called before the first frame update
    void Start()
    {
        _gameState = GAME_STATE.READY;
        _interactiveState = INTERACTIVE_STATE.INVALID;
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

        _instancesA = new List<List<Role>>();
        _instancesB = new List<List<Role>>();
        _deadInstances = new List<Role>();
		// _colliders = new Dictionary<>();
    }

	void ButtonClick(GameObject button)
	{
		Debug.Log("GameObject "+ button.name);
  //       GameObject go = (GameObject)Resources.Load("Model/model1001001/model1001001");
  //       go = Instantiate(go);
  //       go.transform.parent = _roleNode2.transform;
  //       go.transform.localPosition = new Vector3(0, 0, 0);
		// Vector3 pos= Camera.main.WorldToScreenPoint(go.transform.position);//将对象坐标换成屏幕坐标
		// Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pos.z);//让鼠标的屏幕坐标与对象坐标一致
		// Debug.Log("mousePos=" + mousePos);
  //       go.transform.position = Camera.main.ScreenToWorldPoint(mousePos);//将正确的鼠标屏幕坐标换成世界坐标交给物体

  //       ModelCustomData customData = go.GetComponent<ModelCustomData>();
  //       customData.setPower(2);
  //       customData.setLightDir(new Vector4(1.42f, 3.16f, 1.48f, 1.0f));
  //       customData.setShadowColor(new Color(0.608f, 0.608f, 0.608f, 1f));
  //       customData.getAnimator().Play("Idle", 0, 0);

        initTmpTeamB();
        _interactiveState = INTERACTIVE_STATE.SELECTING;

        // go.transform.Rotate(0, 180, 0, Space.Self);
	}
    void init(List<List<Role>> instances, ConfigTeamParams config, GameObject parent){
        switch(config.formation) {
        	case FORMATION.RECT:
        	int width = config.teamRect.x;
        	int height = config.teamRect.y;
        	Vector3 startPos = config.startPos;
        	Vector3 tarPos = config.endPos;
        	for (int x = 0; x < width; x++) {
        		List<Role> colRoles = new List<Role>();
        		instances.Add(colRoles);
        		for (int y = 0; y < height; y++) {
        			Debug.Log("create object " + x + ", " + y);
			        GameObject go = (GameObject)Resources.Load("Model/model1001001/model1001001");
			        go = Instantiate(go);
			        go.transform.parent = parent.transform;
			        go.transform.localPosition = new Vector3(x, 0, y) + startPos;

			        Role role;
			        if (config.power == 1) {
			        	go.transform.Rotate(0, 90, 0, Space.Self);
			        	role = new Role(go, x, y, 1);
		        	} else {
			        	go.transform.Rotate(0, 270, 0, Space.Self);
			        	role = new Role(go, x, y, 2);
		        	}
			        
		            ModelCustomData customData = go.GetComponent<ModelCustomData>();
		            customData.setPower(config.power);
		            customData.setLightDir(new Vector4(1.42f, 3.16f, 1.48f, 1.0f));
		            customData.setShadowColor(new Color(0.608f, 0.608f, 0.608f, 1f));
		            customData.getAnimator().Play("Idle", 0, 0);
			        // customData.getAnimator().SetLookAtPosition(tarPos);
			        colRoles.Add(role);
        		}
        	}
        	break;
        }
    }

	private static Ray ray;
    /// <summary>
    /// 和地面交点
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetPlaneInteractivePoint(float plane=0)
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 dir = ray.direction;
        
        if(dir.y.Equals(0)) return Vector3.zero;
        float num=(plane-ray.origin.y)/dir.y;
        return ray.origin + ray.direction * num;
    }

    void initTeamA(){
        init(_instancesA, _configTeamA, _roleNode1);
    }

    void initTeamB(){
        init(_instancesB, _configTeamB, _roleNode2);
    }

    void initTmpTeamB(){
        _tmpInstances = new List<List<Role>>();
        init(_tmpInstances, _configTeamB, _roleNode2);
    }

    void initBattleInfo(){
    	changeState(GAME_STATE.INIT_SCENE);
    	initTeamA();
    	changeState(GAME_STATE.DURATION);
    }

    void FixedUpdate() {

    	switch (_gameState) {
    		case GAME_STATE.DURATION:
    			fixedLoopGame();
    		break;
    	}
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
    		break;

    		case GAME_STATE.END:
    		break;
    	}
    	if (_configTeamA != null)
        	Debug.Log("_configTeamA=" + _configTeamA.show + ", _configTeamB=" + _configTeamB.number);
        else
        	Debug.Log("_configTeamA");

        if (_tmpInstances != null) {
        	Vector3 rayPos = GetPlaneInteractivePoint();
	    	foreach (List<Role> colRoles in _tmpInstances) {
	    		foreach (Role role in colRoles) {
	    			GameObject go = role.getGo();
			        go.transform.position = rayPos + new Vector3(role.X, 0, role.Y);
	    		}
	    	}

	        if (Input.GetMouseButtonDown(0)) {
	            Debug.Log ("你按下了鼠标左键");
	        }
	        if (Input.GetMouseButton(0)) {
	            Debug.Log ("你按住了鼠标左键");
	        }

	        if (Input.GetMouseButtonUp(0)) {
	            Debug.Log ("你抬起了鼠标左键" + _tmpInstances + ", state=" + _interactiveState);
	           	switch (_interactiveState) {
	           		case INTERACTIVE_STATE.SELECTING:
	           			changeInteractiveState();
	           		break;
	           		case INTERACTIVE_STATE.SELECTED:
	           			changeInteractiveState();
	           			_instancesB = _tmpInstances;
	           			_tmpInstances = null;
	           		break;
	           		case INTERACTIVE_STATE.PLACED:
	           			changeInteractiveState();
	           		break;
	           		default:
	           		break;
	           	}
	        }
        }
    }

    public void changeState(GAME_STATE state) {
    	_gameState = state;
    }

    public void changeInteractiveState() {
    	if (_interactiveState == INTERACTIVE_STATE.SELECTING) {
    		_interactiveState = INTERACTIVE_STATE.SELECTED;
    	} else if (_interactiveState == INTERACTIVE_STATE.SELECTED) {
    		_interactiveState = INTERACTIVE_STATE.PLACED;
    	} else if (_interactiveState == INTERACTIVE_STATE.PLACED) {
    		_interactiveState = INTERACTIVE_STATE.INVALID;
    	}
    	Debug.Log("interactive state=" + _interactiveState);
    }

    public void InitParams() {
    	_configTeamA = new ConfigTeamParams();
    	_configTeamA.power = 1;
    	_configTeamA.formation = FORMATION.RECT;
    	_configTeamA.teamRect = new Vector2Int(25, 25);
    	_configTeamA.startPos = new Vector3(-30,0,0);
    	_configTeamA.endPos = new Vector3(100,0,0);

    	_configTeamB = new ConfigTeamParams();
    	_configTeamB.power = 2;
    	_configTeamB.formation = FORMATION.RECT;
    	_configTeamB.startPos = Vector3.zero;
    	_configTeamB.endPos = Vector3.zero;
    	_configTeamB.teamRect = new Vector2Int(2, 10);
    }

    public void resetGame(){
    	changeState(GAME_STATE.READY);
    	Transform transform = _roleNode1.transform;
		for (int i = 0; i < transform.childCount; i++) {  
            Destroy(transform.GetChild (i).gameObject);  
        }
    	transform = _roleNode2.transform;
		for (int i = 0; i < transform.childCount; i++) {  
            Destroy(transform.GetChild (i).gameObject);  
        }
        _instancesA.Clear();
        _instancesB.Clear();
        _deadInstances.Clear();
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
			    go.transform.DOMove(new Vector3(role.X, 0, role.Y) + _configTeamA.endPos, 1000);
		        ModelCustomData customData = go.GetComponent<ModelCustomData>();
		        customData.getAnimator().Play("Move", 0, 0);
    		}
    	}

    }


    private void fixedLoopGame() {
		Debug.Log("_instancesA=" + _instancesA + ", " + _instancesB);
		Debug.Log("_instancesA size=" + _instancesA.Count + ", " + _instancesB.Count);

    	bool role2Dead = false;
		for (int j2 = _instancesB.Count-1; j2 >= 0; j2--) {
			List<Role> roleList2 = _instancesB[j2];
			for (int k2 = roleList2.Count - 1; k2 >= 0; k2--) {
				Role role2 = roleList2[k2];
    			GameObject go2 = role2.getGo();
    			Transform transform2 = go2.transform;

				role2Dead = false;
				for (int j = _instancesA.Count-1; j >= 0; j--) {
					List<Role> roleList = _instancesA[j];
					for (int k = roleList.Count - 1; k >= 0; k--) {
						Role role = roleList[k];
		    			GameObject go = role.getGo();
		    			Transform transform = go.transform;
						if (role.isCollision(transform2.position)) {
							Debug.Log("Collision=" + j + "," + k + ", A position=" + transform.position);
							role2.health -= role.health;
							// Destroy(go);
							_deadInstances.Add(role);
							role.dead();
							roleList.RemoveAt(k);
							if (role2.health <= 0) {
								role2Dead = true;
							 	break;
							}
						}
					}
					if (role2Dead) break;
				}

				if (role2Dead) {
					// Destroy(go2);
					_deadInstances.Add(role2);
					role2.dead();
					roleList2.RemoveAt(k2);
				}
			}
		}

    }
}
