using UnityEngine;
using System.Collections;

public class RemoveLabel : MonoBehaviour {

	void OnMouseDown()
	{
		GameObject.Find("PlayerMenu").GetComponent<PlayerMenu>().RemovePlayer(this.gameObject.transform.parent.gameObject);
		GameObject.Destroy(this.gameObject.transform.parent.gameObject);
	}
}
