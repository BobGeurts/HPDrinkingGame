﻿using UnityEngine;
using System.Collections.Generic;

public class UI : MonoBehaviour {
    public bool lblSpace;
    public bool lblDesc;
    public List<Texture> DiceFace;
    public string space;
    public string Desc;
	public GUISkin skin;
	public int DiceNr;

    // Use this for initialization
    void Start()
    {
        lblDesc = true;
        Desc = "";
		DiceNr = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
		GUI.skin = skin;
        if (lblDesc)
            GUI.TextArea(new Rect(20, 0, 700, 400), Desc);
        GUI.DrawTexture(new Rect(620, 60, 80, 80), DiceFace[DiceNr]);
    }
}
