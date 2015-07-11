using UnityEngine;
using System.Collections;

public class ButtonUp : MonoBehaviour {

	void OnMouseDown()
	{
		GameObject.Find("GameInfo").GetComponent<GameInfo>().amountOfPlayers--;
	}
}
