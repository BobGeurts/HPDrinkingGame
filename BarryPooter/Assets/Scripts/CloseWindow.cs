using UnityEngine;
using System.Collections;

public class CloseWindow : MonoBehaviour {
	PlayerMenu PlayerMenu;

	void Start()
	{
		PlayerMenu = GameObject.Find("PlayerMenu").GetComponent<PlayerMenu>();
	}

	void OnMouseDown()
	{
		PlayerMenu.CloseWindow();
	}
}
