using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using UnityEditor;

public class Formation : MonoBehaviour
{
	public bool show;
	public float moveSpeed;
	public ROLE_STATE roleState;
    public Vector3 startPos;
    public Vector3 endPos;
    public bool clear;

    private ROLE_STATE _curState;
    private bool _curShow;
    private float _curMoveSpeed;
    private Vector3 _curStartPos;
    public TEAM team {set;get;}

    private List<List<Role>> _instances;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Formation Start");
    }

    public void Init(List<List<Role>> instances, ConfigTeamParams config) {
    	_instances = instances;
    	initConfig(config);
    	show = true;
    }

    void initConfig(ConfigTeamParams config) {
    	show = config.show;
    	moveSpeed = config.moveSpeed;
    	roleState = config.roleState;
    	startPos = config.startPos;
    	endPos = config.endPos;
    	team = config.team;
    }

    // Update is called once per frame
    void Update()
    {
    	bool updateTeam = false;
    	if (_curState != roleState) {
    		changeState();
    		updateTeam = true;
    	}

    	if (moveSpeed != _curMoveSpeed) {
    		updateSpeedInfo();
    	}

    	if (_curShow != show) {
    		updateShowInfo();
    	}

    	if (startPos != _curStartPos && _instances != null) {
    		Vector3 intervalPos = startPos - _curStartPos;
    		_curStartPos = startPos;
	    	foreach (List<Role> colRoles in _instances) {
	    		foreach (Role role in colRoles) {
	    			GameObject go = role.getGo();
	    			Transform transform = go.transform;
	    			transform.localPosition += intervalPos;
	    		}
	    	}
    	}

    	if (updateTeam)
    		_updateTeamInfo();

    	if (clear) {
	        StartDemo startDemo = transform.parent.GetComponent<StartDemo>();
    		Destroy(gameObject);
    		startDemo.Detect();
    	}
    }

    public void updateParams(ConfigTeamParams config) {
    	initConfig(config);
    	_curState = ROLE_STATE.UNKNOWN;
    }

    public void setStartPos(Vector3 pos) {
    	startPos = pos;
    	_curStartPos = startPos;
    }

    void updateSpeedInfo() {
    	Debug.Log("Formation updateSpeedInfo=" + moveSpeed + ", _curState=" + _curState);
    	_curMoveSpeed = moveSpeed;

    	if (_curState == ROLE_STATE.WALK) {
			float dis = (startPos - endPos).magnitude;
			float time = 1000;
			if (_curMoveSpeed > 0) {
				time = dis/_curMoveSpeed;
			}
	    	foreach (List<Role> colRoles in _instances) {
	    		foreach (Role role in colRoles) {
			        StartCoroutine(DelayToInvoke.DelayToInvokeDo(() => {
		    			GameObject go = role.getGo();
		    			Transform transform = go.transform;
		    			if (role.state == ROLE_STATE.WALK) {
			    			Debug.Log("move direction=" + (endPos - startPos));
			    			transform.DOKill();
						    transform.DOLocalMove(endPos, time);
		    			}
						}, UnityEngine.Random.Range(0.0f, 1.0f))
			        );
	    		}
	    	}
    	}

    }


    void updateShowInfo() {
    	Debug.Log("Formation updateShowInfo=" + show);
    	_curShow = show;

    	foreach (List<Role> colRoles in _instances) {
    		foreach (Role role in colRoles) {
	    		GameObject go = role.getGo();
	    		go.SetActive(_curShow);

	            ModelCustomData customData = go.GetComponent<ModelCustomData>();
	            customData.setPower((int)team);
	            customData.setLightDir(new Vector4(1.42f, 3.16f, 1.48f, 1.0f));
	            customData.setShadowColor(new Color(0.608f, 0.608f, 0.608f, 1f));
    		}
    	}
    }

    void _updateTeamInfo() {
    	if (_instances == null) return;
    	switch (_curState) {
    		case ROLE_STATE.IDLE:
				Debug.Log("_instances=" + _instances);
				Debug.Log("_instances size=" + _instances.Count);
		    	foreach (List<Role> colRoles in _instances) {
		    		foreach (Role role in colRoles) {
		    			GameObject go = role.getGo();
		    			Transform transform = go.transform;
		    			transform.DOKill();
				        ModelCustomData customData = go.GetComponent<ModelCustomData>();
				        customData.getAnimator().Play("Idle");
		    		}
		    	}
    		break;
    		case ROLE_STATE.ATTACK:
				Debug.Log("_instances=" + _instances);
				Debug.Log("_instances size=" + _instances.Count);
		    	foreach (List<Role> colRoles in _instances) {
		    		foreach (Role role in colRoles) {
				        StartCoroutine(DelayToInvoke.DelayToInvokeDo(() => {
			    			GameObject go = role.getGo();
			    			Transform transform = go.transform;
			    			transform.DOKill();
					        ModelCustomData customData = go.GetComponent<ModelCustomData>();
					        Animator animator = customData.getAnimator();
					        animator.Play("Attack0", 0, 0);
							}, UnityEngine.Random.Range(0.0f, 1.0f))
				        );
		    		}
		    	}
    		break;
    		case ROLE_STATE.WALK:
				Debug.Log("_instances=" + _instances);
				Debug.Log("_instances size=" + _instances.Count);
				float dis = (startPos - endPos).magnitude;
				float time = 1000;
				if (_curMoveSpeed > 0) {
					time = dis/_curMoveSpeed;
				}
		    	foreach (List<Role> colRoles in _instances) {
		    		foreach (Role role in colRoles) {
				        StartCoroutine(DelayToInvoke.DelayToInvokeDo(() => {
			    			GameObject go = role.getGo();
			    			Transform transform = go.transform;
			    			Debug.Log("move direction=" + (endPos - startPos));
			    			transform.DOKill();
						    transform.DOLocalMove(endPos, time);
					        ModelCustomData customData = go.GetComponent<ModelCustomData>();
					        customData.getAnimator().Play("Move", 0, 0);
							}, UnityEngine.Random.Range(0.0f, 1.0f))
				        );
		    		}
		    	}
    		break;
    	}
    }
    void changeState() {
    	Debug.Log("Formation changeState=" + roleState);
		_curState = roleState;
    }
}
