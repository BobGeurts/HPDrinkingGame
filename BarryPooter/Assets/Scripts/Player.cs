using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public string Name;
    public int PlayerNr;
    public int CurrentTile;
    public string House;
	public bool SkipTurn;

	// Use this for initialization
	void Start () {
        CurrentTile = 1;
        House = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
