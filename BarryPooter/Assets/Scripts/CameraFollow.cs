using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public GameObject currentPlayer;
	// Update is called once per frame
	void Update () {
		if (currentPlayer != null)
		{
			Vector3 currentPos = currentPlayer.transform.position;
			transform.position = new Vector3(currentPos.x, currentPos.y, -5);
		}
	}
}
