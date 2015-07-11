using UnityEngine;
using System.Collections.Generic;

public class MenuUI : MonoBehaviour {
	public List<Texture> DiceFace;
	public bool RollDice;
	public int DiceNr;

	// Use this for initialization
	void Start () {
		RollDice = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if(RollDice)
			GUI.DrawTexture(new Rect(100, 100, 80, 80), DiceFace[DiceNr]);
	}
}
