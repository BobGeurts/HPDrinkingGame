using UnityEngine;
using System.Collections;

public class SelectIcon : MonoBehaviour {
	PlayerMenu PlayerMenu;

	void Start()
	{
		PlayerMenu = GameObject.Find("PlayerMenu").GetComponent<PlayerMenu>();
	}

	void OnMouseDown()
	{
		PlayerMenu.ResetIcons();
		PlayerMenu.SelectedIcon = this.name;
		this.transform.FindChild("Select").GetComponent<SpriteRenderer>().enabled = true;
	}
}
