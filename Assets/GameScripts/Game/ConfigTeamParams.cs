using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using UnityEditor;
public class ConfigTeamParams {
    public bool show;
    public ROLE_STATE roleState;
    public Vector3 startPos;
    public Vector3 endPos;
    public float moveSpeed;
    public int number;
    public Vector2Int teamRect;
    public FORMATION formation;
    public int power;
    public bool randomGenerate;
    public int health;
    public TEAM team;

	public static bool operator == (ConfigTeamParams a, ConfigTeamParams b)
	{	
		if (!(a is ConfigTeamParams) && !(b is ConfigTeamParams)) return true;
		if (!(a is ConfigTeamParams) || !(b is ConfigTeamParams)) return false;
		return a.moveSpeed == b.moveSpeed && a.roleState == b.roleState;
	}

	public static bool operator != (ConfigTeamParams a, ConfigTeamParams b)
	{
		if (!(a is ConfigTeamParams) && !(b is ConfigTeamParams)) return false;
		if (!(a is ConfigTeamParams) || !(b is ConfigTeamParams)) return true;
		return a.moveSpeed != b.moveSpeed || a.roleState != b.roleState;
	}
}
