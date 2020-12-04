using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ROLE_STATE
{
    IDLE = 0,
    ATTACK = 1,
    DIE = 2,
    JUMP_DIE = 3,
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
	public Role(GameObject obj){
		_obj = obj;

	}

	public GameObject getGo() { return _obj; }
}

public class StartDemo : MonoBehaviour
{
	private GAME_STATE _gameState;
    private ConfigTeamParams _configTeamA;
    private ConfigTeamParams _configTeamB;
    private List<List<Role>> _instancesA;


    private GameObject _roleNode1;


    // Start is called before the first frame update
    void Start()
    {
        _gameState = GAME_STATE.READY;
		_roleNode1 = GameObject.Find("Terrain/RoleNode1");
    }

    void init(List<List<Role>> instances, ConfigTeamParams _config){
        switch(_config.formation) {
        	case FORMATION.RECT:
        	int width = _config.teamRect.x;
        	int height = _config.teamRect.y;
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
		            soldier_model_custom_data.getAnimator().Play("idle", 0, 0);
			        // soldier_model_custom_data.getAnimator().SetLookAtPosition(tarPos);
			        Role role = new Role(go);
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

    private void loopGame() {
        Vector3 tarPos = new Vector3(-1.0f, 0.0f, -1.0f);

    	Vector3 TargetPosition = new Vector3(1000,0,1000);
		float AvatarRange = 25;
		Debug.Log("_instancesA=" + _instancesA);
		Debug.Log("_instancesA size=" + _instancesA.Count);
    	foreach (List<Role> colRoles in _instancesA) {
    		foreach (Role role in colRoles) {
    			GameObject go = role.getGo();
    			Transform transform = go.transform;
	      //       if (Vector3.SqrMagnitude(TargetPosition - transform.position) > 25)
	      //       {

	      //           //avatar.SetFloat("Speed", 1, SpeedDampTime, Time.deltaTime);

	      //           Vector3 curentDir = transform.rotation * Vector3.forward;
	                Vector3 wantedDir = (TargetPosition - transform.position).normalized;
	                Debug.Log("wantedDir=" + wantedDir);
	      //           transform.Translate(wantedDir * 1.0f * Time.deltaTime);
	      //           transform.rotation.SetLookRotation(wantedDir);
	                // go.transform.Rotate(0, 180, 0, Space.Self);
	      //           go.transform.rotation = Quaternion.LookRotation(TargetPosition - transform.position);
	      //           // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(TargetPosition - transform.position), 8.0f);
	      //       }
	      //       else
	      //       {

	      //           //if (avatar.GetFloat("Speed") < 0.01f)
	      //           {
	      //               // instancing.PlayAnimation(UnityEngine.Random.Range(0, 2));
	      //               //instancing.PlayAnimation(1);
	      //               //instancing.CrossFade(1, 0.1f);
	      //               TargetPosition = new Vector3(UnityEngine.Random.Range(-AvatarRange, AvatarRange), 0, UnityEngine.Random.Range(-AvatarRange, AvatarRange));
	      //               transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(TargetPosition - transform.position), 0.1f);
	      //               //gameObject.transform.rotation = Quaternion.LookRotation(TargetPosition - transform.position);
	      //           }
	      //       }
    		}
    	}
    }
}
