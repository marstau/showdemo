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
    public Vector2 teamRect;
    public FORMATION formation;
}

public enum GAME_STATE {
	READY = 1,
	START = 2,
	DURATION = 3,
	END = 4,
}

public class Team {
}

public class StartDemo : MonoBehaviour
{
	private GAME_STATE _gameState;
    private ConfigTeamParams configTeamA;
    private ConfigTeamParams configTeamB;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = GAME_STATE.READY;
    }

    void initBattleInfo(){
        GameObject role = (GameObject)Resources.Load("Model/model1001001/model1001001");
        GameObject roleNode1 = GameObject.Find("Terrain/RoleNode1");
        role = Instantiate(role);
        role.transform.parent = roleNode1.transform;
        role.transform.localPosition = new Vector3(0, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
    	switch (_gameState) {
    		case GAME_STATE.READY:
    		if (configTeamA != null && configTeamA.show) {
    			changeState(GAME_STATE.START);
    		}
    		break;

    		case GAME_STATE.START:
    		initBattleInfo();
    		break;

    		case GAME_STATE.DURATION:
    		break;

    		case GAME_STATE.END:
    		break;
    	}
    	if (configTeamA != null)
        	Debug.Log("configTeamA=" + configTeamA.show + ", configTeamB=" + configTeamB.number);
        else
        	Debug.Log("configTeamA");
    }

    public void changeState(GAME_STATE state) {
    	_gameState = state;
    }
    public void Init() {
    	configTeamA = new ConfigTeamParams();
    	configTeamB = new ConfigTeamParams();
    }

    public ConfigTeamParams getConfigTeamA() {
    	return configTeamA;
    }
    public ConfigTeamParams getConfigTeamB() {
    	return configTeamB;
    }
}
