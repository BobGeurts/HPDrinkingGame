using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AddPlayer : MonoBehaviour {
	PlayerMenu PlayerMenu;
	Button Button;
	Image Image;
	public InputField InputField;

	// Use this for initialization
	void Start () {
		Button = this.GetComponent<Button>();
		Image = this.GetComponent<Image>();
		PlayerMenu = GameObject.Find("PlayerMenu").GetComponent<PlayerMenu>();
	}

	void Update(){
		if(PlayerMenu.SelectedIcon != string.Empty && InputField.text != string.Empty)
		{
			Button.enabled = true;
			Image.enabled = true;
		}
		else
		{
			Button.enabled = false;
			Image.enabled = false;
		}
	}
	
	public void InputPlayerName()
	{
		PlayerMenu.AddPlayer(InputField.text);
		InputField.text = string.Empty;
		Button.enabled = false;
		Image.enabled = false;
		this.gameObject.SetActive(false);
	}
}
