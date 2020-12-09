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

    private ROLE_STATE _curState;
    public TEAM team {set;get;}

    private List<List<Role>> _instances;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(List<List<Role>> instances, ConfigTeamParams config) {
    	_instances = instances;
    	initConfig(config);
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
    	if (_curState != roleState) {
    		changeState();
    	}
    }

    public void updateParams(ConfigTeamParams config) {
    	initConfig(config);
    	_curState = ROLE_STATE.UNKNOWN;
    }

    void changeState() {
    	Debug.Log("Formation changeState=" + roleState);
    	if (_instances == null) return;
		_curState = roleState;
    	switch (roleState) {
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
				if (moveSpeed > 0) {
					time = dis/moveSpeed;
				}
		    	foreach (List<Role> colRoles in _instances) {
		    		foreach (Role role in colRoles) {
				        StartCoroutine(DelayToInvoke.DelayToInvokeDo(() => {
			    			GameObject go = role.getGo();
			    			Transform transform = go.transform;
			    			Debug.Log("move direction=" + (endPos - startPos));
			    			transform.DOKill();
						    transform.DOLocalMove(endPos - startPos, time);
					        ModelCustomData customData = go.GetComponent<ModelCustomData>();
					        customData.getAnimator().Play("Move", 0, 0);
							}, UnityEngine.Random.Range(0.0f, 1.0f))
				        );
		    		}
		    	}
    		break;
    	}

    }
}
