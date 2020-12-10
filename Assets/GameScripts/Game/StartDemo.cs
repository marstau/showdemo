using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using UnityEditor;

/*
阴影
其他阵容
碰撞优化
*/
public enum ROLE_STATE {
    IDLE = 0,
    ATTACK = 1,
    WALK = 2,
    DIE = 3,
    JUMP_DIE = 4,
    PLACING = 5,
    UNKNOWN = 0,
}


public enum ROLE_SHOW_STATE {
    HIDE = 0,
    SHOW = 1,
}

public enum INTERACTIVE_STATE {
	INVALID = 0,
	SELECTINGA = 1,
	SELECTINGB = 2,
	SELECTEDA = 3,
	SELECTEDB = 4,
	PLACED = 5,
}
public enum FORMATION {
    CIRCLE = 0,
    RECT = 1,
    TRIANGLE = 2,
    ARROW = 3,
}

public enum TEAM {
	TEAM1 = 1,
	TEAM2 = 2,
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
	public ROLE_STATE state{set;get;}
	public int health{set;get;}
	public float X{set;get;}
	public float Y{set;get;}
	public float moveSpeed{set;get;}
	public TEAM team{set;get;}
	public Role(GameObject obj, int _x, int _y, int health){
		_obj = obj;
		X = _x;
		Y = _y;
		moveSpeed = 0f;
		this.health = health;
	}

	public GameObject getGo() { return _obj; }

	public void move() {
		state = ROLE_STATE.WALK;
	}

	public void die(ROLE_STATE targetState) {
		if (targetState == ROLE_STATE.WALK) {
			jumpDie();
		} else {
			state = ROLE_STATE.DIE;
			Transform transform = _obj.transform;
	        ModelCustomData customData = _obj.GetComponent<ModelCustomData>();
	        transform.DOKill();
	        customData.getAnimator().Play("Die", 0, 0);
		}
	}

	private void jumpDie() {
		state = ROLE_STATE.DIE;
		int resolution = 20;
		float height = 0.5f;
		// direction
		float x = 3f;
		if (team == TEAM.TEAM1)
			x = -3f;
		float time = 0.4f;

		// 绘制抛物线
		Transform transform = _obj.transform;
        transform.DOKill();
        
		Vector3 localPosition = transform.localPosition;
        ModelCustomData customData = _obj.GetComponent<ModelCustomData>();
        customData.getAnimator().Play("Die", 0, 0);
		Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMoveX(x + localPosition.x, time).SetEase(Ease.Linear));
        s.Insert(0, transform.DOLocalMoveY(height, time/2).SetEase(Ease.OutCirc));
        //下落
        s.Insert(time/2, transform.DOLocalMoveY(height/2, time/2).SetEase(Ease.InCirc));
        s.SetLoops(0);
	}

	public bool isCollisionBox(Vector3 pos) {
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

	public bool isCollisionSphere(Vector3 pos) {
		Vector3 colliderPos = _obj.transform.position;
		float x = colliderPos.x;
		float y = colliderPos.y;
		float z = colliderPos.z;
		float r = 0.5f;

		if (Mathf.Pow(x - pos.x, 2) + Mathf.Pow(z - pos.z, 2) <= r*r) {
            return true;
        }
		return false;
	}

}

public class Team {

}

public class DelayToInvoke : MonoBehaviour
{

	public static IEnumerator DelayToInvokeDo(Action action, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		action();
	}
}

public class StartDemo : MonoBehaviour
{
	private GAME_STATE _gameState;
    public ConfigTeamParams _configTeamA;
    private ConfigTeamParams _configTeamB;
    private List<List<Role>> _instancesA;
    private List<List<Role>> _instancesB;

    private List<List<Role>> _tmpInstances;
    private Formation _tmpFormation;

    private List<Role> _deadInstances;

    private GameObject _placingGO;
    private GameObject _placedGO;
    private INTERACTIVE_STATE _interactiveState;

    private ConfigTeamParams _curConfigTeamA;
    private ConfigTeamParams _curConfigTeamB;
    private int _rootGoIndex;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = GAME_STATE.START;
        _interactiveState = INTERACTIVE_STATE.INVALID;
		// EXAMPLE A: initialize with the preferences set in DOTween's Utility Panel
		﻿﻿﻿﻿﻿DOTween.Init();
		﻿﻿﻿﻿﻿// EXAMPLE B: initialize with custom settings, and set capacities immediately
		﻿﻿﻿﻿﻿DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(200, 10);

		GameObject button1= GameObject.Find("Canvas/Button1");
		EventTriggerListener.Get(button1.gameObject).onClick= ButtonClick1;

		GameObject button2= GameObject.Find("Canvas/Button2");
		EventTriggerListener.Get(button2.gameObject).onClick= ButtonClick2;

        _instancesA = new List<List<Role>>();
        _instancesB = new List<List<Role>>();
        _deadInstances = new List<Role>();
		// _colliders = new Dictionary<>();

		_rootGoIndex = 1;
    }

	void ButtonClick1(GameObject button)
	{
		Debug.Log("GameObject "+ button.name);
        initTmpTeamA();
        _interactiveState = INTERACTIVE_STATE.SELECTINGA;
	}

	void ButtonClick2(GameObject button)
	{
		Debug.Log("GameObject "+ button.name);
        initTmpTeamB();
        _interactiveState = INTERACTIVE_STATE.SELECTINGB;
	}

    void init(List<List<Role>> instances, ConfigTeamParams config, GameObject parent){
    	if (config == null) return;

        switch(config.formation) {
        	case FORMATION.RECT:
        	parent.AddComponent<Formation>();
        	int width = config.teamRect.x;
        	int height = config.teamRect.y;
        	Vector3 startPos = config.startPos;
        	Vector3 tarPos = config.endPos;
        	bool [,] array = new bool[width,height];
        	Debug.Log("config.number=" + config.number + ", " + width*height);
        	if (config.randomGenerate) {
	        	if (config.number < width * height) {
		        	for (int ignoreIdx = config.number; ignoreIdx < width*height; ignoreIdx++) {
		        		int w = UnityEngine.Random.Range(0, width);
		        		int h = UnityEngine.Random.Range(0, height);
		        		array[w, h] = true;
		        	}
	        	}
        	}

    		List<Role> colRoles = new List<Role>();
    		instances.Add(colRoles);
        	for (int x = 0; x < width; x++) {
        		for (int y = 0; y < height; y++) {
        			if (array[x, y]) {
	        			Debug.Log("ignore " + x + ", " + y + ", " + array[x, y]);
        				continue;
        			}
			        GameObject go = (GameObject)Resources.Load("Model/"+config.modelName+"/"+config.modelName);
        			Debug.Log("create object " + x + ", " + y + ", modelName=" + config.modelName);
			        go = Instantiate(go);
			        go.transform.parent = parent.transform;

			        Role role = new Role(go, x, y, config.health);
			        if (config.team == TEAM.TEAM1) {
			        	go.transform.Rotate(0, 90, 0, Space.Self);
		        	} else {
			        	go.transform.Rotate(0, 270, 0, Space.Self);
		        	}

		        	if (config.randomGenerate)
		        		go.transform.localPosition = new Vector3(x, 0, y) + new Vector3(UnityEngine.Random.Range(0.0f, 1.0f), 0, UnityEngine.Random.Range(0.0f, 1.0f));
		        	else
		        		go.transform.localPosition = new Vector3(x, 0, y);
		        	role.X = go.transform.localPosition.x;
		        	role.Y = go.transform.localPosition.z;
			        
		            ModelCustomData customData = go.GetComponent<ModelCustomData>();
		            customData.setPower((int)config.team);
		            customData.setLightDir(new Vector4(1.42f, 3.16f, 1.48f, 1.0f));
		            customData.setShadowColor(new Color(0.608f, 0.608f, 0.608f, 1f));
		            customData.getAnimator().Play("Idle", 0, 0);
			        // customData.getAnimator().SetLookAtPosition(tarPos);
			        colRoles.Add(role);
        		}
        	}

	        Formation formation = parent.GetComponent<Formation>();
	        formation.Init(instances,config);
	        _tmpFormation = formation;
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

    // void initTeamA(){
    //     init(_instancesA, _configTeamA, _roleNode1);
    // }

    // void initTeamB(){
    //     init(_instancesB, _configTeamB, _roleNode2);
    // }

    void initTmpTeamB(){
		GameObject rootGo = new GameObject("FormationB" + _rootGoIndex++);
		rootGo.transform.parent = this.gameObject.transform;
        _tmpInstances = new List<List<Role>>();
        init(_tmpInstances, _configTeamB, rootGo);
    }

    void initTmpTeamA(){
		GameObject rootGo = new GameObject("FormationA" + _rootGoIndex++);
		rootGo.transform.parent = this.gameObject.transform;
        _tmpInstances = new List<List<Role>>();
        init(_tmpInstances, _configTeamA, rootGo);
    }


    void initBattleInfo(){
    	changeState(GAME_STATE.INIT_SCENE);
    	// initTeamA();
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

    	if (_curConfigTeamA != _configTeamA) {
	    	_curConfigTeamA = GameUtil.CloneModel<ConfigTeamParams>(_configTeamA);
	    	foreach (Transform child in this.transform) {
	    		Formation formation = child.gameObject.GetComponent<Formation>();
	    		if (formation.team == TEAM.TEAM1) {
	    			formation.updateParams(_configTeamA);
	    		}
	    		Debug.Log(child.name + ", " + formation);
	    	}
    	}

    	if (_curConfigTeamB != _configTeamB) {
	    	_curConfigTeamB = GameUtil.CloneModel<ConfigTeamParams>(_configTeamB);
	    	foreach (Transform child in this.transform) {
	    		Formation formation = child.gameObject.GetComponent<Formation>();
	    		if (formation.team == TEAM.TEAM2) {
	    			formation.updateParams(_configTeamB);
	    		}
	    		Debug.Log(child.name + ", " + formation);
	    	}
    	}

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
	           		case INTERACTIVE_STATE.SELECTINGB:
	           		case INTERACTIVE_STATE.SELECTINGA:
	           			changeInteractiveState();
	           		break;
	           		case INTERACTIVE_STATE.SELECTEDB:
	           			changeInteractiveState();
	           			_instancesB.AddRange(_tmpInstances);
	           			_tmpFormation.setStartPos(rayPos);
	           			_tmpInstances = null;
	           		break;
	           		case INTERACTIVE_STATE.SELECTEDA:
	           			changeInteractiveState();
	           			_instancesA.AddRange(_tmpInstances);
	           			_tmpFormation.setStartPos(rayPos);
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
    	switch (_interactiveState) {
    		case INTERACTIVE_STATE.SELECTINGB:
    			_interactiveState = INTERACTIVE_STATE.SELECTEDB;
    		break;
    		case INTERACTIVE_STATE.SELECTINGA:
    			_interactiveState = INTERACTIVE_STATE.SELECTEDA;
    		break;
    		case INTERACTIVE_STATE.SELECTEDA:
    		case INTERACTIVE_STATE.SELECTEDB:
    			_interactiveState = INTERACTIVE_STATE.PLACED;
    		break;
    		case INTERACTIVE_STATE.PLACED:
    			_interactiveState = INTERACTIVE_STATE.INVALID;
    		break;
    	}

    	Debug.Log("interactive state=" + _interactiveState);
    }

    public void InitParams() {
    	_configTeamA = new ConfigTeamParams();
    	_configTeamA.formation = FORMATION.RECT;
    	_configTeamA.teamRect = new Vector2Int(5, 5);
    	_configTeamA.startPos = new Vector3(-30,0,0);
    	_configTeamA.endPos = new Vector3(100,0,0);
    	_configTeamA.health = 1;
    	_configTeamA.moveSpeed = 1.0f;
    	_configTeamA.number = _configTeamA.teamRect.x * _configTeamA.teamRect.y;
    	_configTeamA.team = TEAM.TEAM1;
    	_configTeamA.modelName = "model1001001";

    	_curConfigTeamA = GameUtil.CloneModel<ConfigTeamParams>(_configTeamA);
    	// Debug.Log("_curConfigTeamA=" + _curConfigTeamA.moveSpeed + ", _configTeamA=" + _configTeamA.moveSpeed);

    	_configTeamB = new ConfigTeamParams();
    	_configTeamB.formation = FORMATION.RECT;
    	_configTeamB.startPos = Vector3.zero;
    	_configTeamB.endPos = new Vector3(-100,0,0);
    	_configTeamB.moveSpeed = 0.0f;
    	_configTeamB.health = 2;
    	_configTeamB.teamRect = new Vector2Int(2, 10);
    	_configTeamB.number = _configTeamB.teamRect.x * _configTeamB.teamRect.y;
    	_configTeamB.team = TEAM.TEAM2;
    	_configTeamB.modelName = "model1001001";

    	_curConfigTeamB = GameUtil.CloneModel<ConfigTeamParams>(_configTeamB);
    }

    public void resetGame(){
    	changeState(GAME_STATE.READY);

    	Transform transform = gameObject.transform;
		for (int i = 0; i < transform.childCount; i++) {  
            Destroy(transform.GetChild (i).gameObject);  
        }
        _configTeamA.roleState = ROLE_STATE.UNKNOWN;
        _instancesA.Clear();
        _instancesB.Clear();
        _deadInstances.Clear();
        _rootGoIndex = 1;
    }
    public ConfigTeamParams getConfigTeamA() {
    	return _configTeamA;
    }
    public ConfigTeamParams getConfigTeamB() {
    	return _configTeamB;
    }

    public void Move() {
		// Debug.Log("_instancesA=" + _instancesA);
		// Debug.Log("_instancesA size=" + _instancesA.Count);
		// float dis = (_configTeamA.startPos - _configTeamA.endPos).magnitude;
		// float time = 1000;
		// if (_configTeamA.moveSpeed > 0) {
		// 	time = dis/_configTeamA.moveSpeed;
		// }
  //   	foreach (List<Role> colRoles in _instancesA) {
  //   		foreach (Role role in colRoles) {
		//         StartCoroutine(DelayToInvoke.DelayToInvokeDo(() => {
	 //    			GameObject go = role.getGo();
	 //    			Debug.Log("move direction=" + (_configTeamA.endPos - _configTeamA.startPos) );
		// 		    go.transform.DOLocalMove(_configTeamA.endPos - _configTeamA.startPos, time);
		// 	        ModelCustomData customData = go.GetComponent<ModelCustomData>();
		// 	        customData.getAnimator().Play("Move", 0, 0);
		// 			}, UnityEngine.Random.Range(0.0f, 1.0f))
		//         );
  //   		}
  //   	}

    }

    public void Detect() {
    }


    private void fixedLoopGame() {
    	if (_instancesA == null || _instancesB == null) {
			Debug.Log("fixedLoopGame=" + _instancesA + ", " + _instancesB);	
			return;
    	}
		Debug.Log("fixedLoopGame _instancesA=" + _instancesA + ", " + _instancesB);
		Debug.Log("fixedLoopGame _instancesA size=" + _instancesA.Count + ", " + _instancesB.Count);

    	bool role2Dead = false;
    	ROLE_STATE targetRoleState = ROLE_STATE.UNKNOWN;
		for (int j2 = _instancesB.Count-1; j2 >= 0; j2--) {
			List<Role> roleList2 = _instancesB[j2];
			for (int k2 = roleList2.Count - 1; k2 >= 0; k2--) {
				Role role2 = roleList2[k2];
    			GameObject go2 = role2.getGo();
    			if (go2 == null) {
    				roleList2.RemoveAt(k2);
    				continue;
    			}
    			Transform transform2 = go2.transform;

				role2Dead = false;
				for (int j = _instancesA.Count-1; j >= 0; j--) {
					List<Role> roleList = _instancesA[j];
					for (int k = roleList.Count - 1; k >= 0; k--) {
						Role role = roleList[k];
		    			GameObject go = role.getGo();
		    			if (go == null) {
		    				roleList.RemoveAt(k);
		    				continue;
		    			}
		    			Transform transform = go.transform;
						if (role.isCollisionBox(transform2.position)) {
							Debug.Log("Collision=" + j + "," + k + ", A position=" + transform.position);
							role2.health -= role.health;
							// Destroy(go);
							_deadInstances.Add(role);

							if (role2.health <= 0) {
								role2Dead = true;
								targetRoleState = role.state;
							}
							StartCoroutine(DelayToInvoke.DelayToInvokeDo(() => {
								_deadInstances.Remove(role);
            					Destroy(role.getGo());
							}, 3.0f));
							role.die(role2.state);
							roleList.RemoveAt(k);

							if (role2Dead) break;
						}
					}
					if (role2Dead) break;
				}

				if (role2Dead) {
					// Destroy(go2);
					_deadInstances.Add(role2);

					StartCoroutine(DelayToInvoke.DelayToInvokeDo(() => {
						_deadInstances.Remove(role2);
            			Destroy(role2.getGo());
					}, 3.0f));
					role2.die(targetRoleState);
					roleList2.RemoveAt(k2);
				}
			}
		}

    }
}
