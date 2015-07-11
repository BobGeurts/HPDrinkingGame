using UnityEngine;
using System.Collections;

public class ButtonDown : MonoBehaviour {

	void OnMouseDown()
	{
		GameObject.Find("GameInfo").GetComponent<GameInfo>().amountOfPlayers++;
	}
}
