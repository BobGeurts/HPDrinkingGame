using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInfo : MonoBehaviour {
	public int amountOfPlayers;
	public List<Player> players;

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}
}
