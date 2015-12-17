﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : MonoBehaviour {
	public string HouseName;
	public List<string> members;
    public bool Army;

	public House(string name)
	{
		HouseName = name;
        Army = false;
	}

	// Use this for initialization
	void Start () {
		members = new List<string>();
	}
}
