using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class PlayerMenu : MonoBehaviour {
	public List<PlayerInfo> Players;
	public string PlayerName;
	public string SelectedIcon;
	public GameObject AddPlayerWindow;
	public GameObject AddPlayerCanvas;
	public GameObject MenuAddPlayer;
	public GameObject CanvasAddPlayer;
	public GameObject PlayerLabel;
	public GameObject StartButton;
	public List<GameObject> PlayerLabels;
	public int AmountOfPlayers;
	public List<Sprite> Icons;
    public List<Sprite> DiceFaces;
	public InputField InputField;
	public MenuUI UI;
	double DiceTimer;
	int[] DiceNumbers;
	bool AnimDice;
	bool KeyPressed;
	bool AbleToStop;
	public int CurrentPlayerNr;


	// Use this for initialization
	void Start () 
	{
		UI = Camera.main.GetComponent<MenuUI>();
		Players = new List<PlayerInfo>();
		AmountOfPlayers = 0;
		DiceTimer = 0.0f;
		DiceNumbers = new int[15];
		KeyPressed = false;
		AbleToStop = false;
		DontDestroyOnLoad(this);
	}

	void Update() 
	{
		AnimDiceRoll();
	}
	
	public void AddPlayer(string Name)
	{
		PlayerInfo PlayerInfo = new PlayerInfo();
		PlayerInfo.Name = Name;
		PlayerInfo.Icon = SelectedIcon;
		Players.Add(PlayerInfo);
		GameObject Label = (GameObject)GameObject.Instantiate(PlayerLabel);
		Label.transform.position = new Vector3(0, 2.135f + (AmountOfPlayers * -0.34f), -9);
		Label.transform.FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Icons.Find(x => x.name == SelectedIcon);
		Label.transform.FindChild("Name").GetComponent<TextMesh>().text = Name;
		PlayerLabels.Add(Label);
		AmountOfPlayers++;
		ResetIcons();
		CloseWindow();
		if(AmountOfPlayers > 1)
			StartButton.SetActive(true);
	}

	public void OpenWindow()
	{
		AddPlayerWindow.SetActive(true);
		AddPlayerCanvas.SetActive(true);
		CanvasAddPlayer.SetActive(true);
		MenuAddPlayer.SetActive(false);
		InputField.enabled = true;
	}

	public void CloseWindow()
	{
		AddPlayerWindow.SetActive(false);
		AddPlayerCanvas.SetActive(false);
		CanvasAddPlayer.SetActive(false);
		MenuAddPlayer.SetActive(true);
		InputField.enabled = false;
	}

	public void SortLabels()
	{
		for(int i=0; i<AmountOfPlayers; i++)
		{
			PlayerLabels[i].transform.position = new Vector3(0, 2.135f + (i * -0.34f), -9);
		}
	}

	public void RemovePlayer(GameObject Label)
	{
		PlayerLabels.Remove(Label);
		AmountOfPlayers--;
		if(AmountOfPlayers < 2)
			StartButton.SetActive(false);
		Players.Remove(Players.Find(x => x.Name == Label.transform.FindChild("Name").GetComponent<TextMesh>().text));
		SortLabels();
	}

	public void ResetIcons()
	{
		GameObject Icons = GameObject.Find("Icons");
		foreach(Transform child in Icons.transform)
			foreach(Transform select in child.transform)
				select.GetComponent<SpriteRenderer>().enabled = false;
		SelectedIcon = string.Empty;
	}

	public void StartGame()
	{
        foreach(GameObject Label in PlayerLabels)
        {
            Label.transform.FindChild("Remove").gameObject.SetActive(false);
        }
		StartButton.SetActive(false);
		MenuAddPlayer.SetActive(false);
		CurrentPlayerNr = 0;
		RollDice();
	}

	public void RollDice()
	{
		Debug.Log(CurrentPlayerNr);
		Debug.Log(Players[CurrentPlayerNr].Name + "rolling");
		KeyPressed = true;
		DiceNumbers = new int[15];
		for(int i=0; i<DiceNumbers.Length; i++)
		{
			int DiceRoll = Random.Range(1,6);
			if(i != 0)
				while(DiceRoll == DiceNumbers[i-1])
				{
					DiceRoll = Random.Range(1,6);
				}	      

			DiceNumbers[i] = (DiceRoll -1);
		}	
		AnimDice = true;
		UI.RollDice = true;
		StartCoroutine(WaitForDice());
	}

	public void UpdatePlayerPosition()
	{
        int rolledNr = DiceNumbers[DiceNumbers.Length-1] + 1;
		Debug.Log(Players[CurrentPlayerNr].Name + " : " + rolledNr);
		Players[CurrentPlayerNr].DiceNr = (DiceNumbers[DiceNumbers.Length-1] + 1);
        PlayerLabels[CurrentPlayerNr].transform.FindChild("Dice").GetComponent<SpriteRenderer>().sprite = DiceFaces.Find(x => x.name == ("die" + rolledNr));
		UpdatePlayerPositions();
		KeyPressed = false;
		StartCoroutine(NextRoll("Jump"));
	}

	public void UpdatePlayerPositions()
	{
		List<PlayerInfo> tempPlayers = new List<PlayerInfo>();
		foreach(PlayerInfo Player in Players)
		{
			tempPlayers.Add(Player);
		}
		tempPlayers.Sort(delegate(PlayerInfo y, PlayerInfo x) 
		{
			if(x.DiceNr == null)
			{
				if(y.DiceNr == null)
					return 0;
				else
					return -1;
			}
			else
			{
				if(y.DiceNr == null)
					return 1;
				else
					return x.DiceNr.CompareTo(y.DiceNr);
			}
		});
		if(CurrentPlayerNr == Players.Count-1)
		{
			for(int i=0; i<Players.Count; i++)
			{
				Players[i].PosNr = i++;
			}
		}
		for(int i=0; i<tempPlayers.Count; i++)
		{
			tempPlayers[i].PosNr = i++;
		}
		List<GameObject> tempLabelList = new List<GameObject>();
		foreach(PlayerInfo Player in tempPlayers)
		{
			tempLabelList.Add(PlayerLabels.Find(x => x.transform.FindChild("Name").GetComponent<TextMesh>().text == Player.Name));
		}
		PlayerLabels = tempLabelList;
		SortLabels();
	}

	public void AnimDiceRoll()
	{
		if(AnimDice)
		{
			DiceTimer += 0.1f;
			if(DiceTimer >= 0.0f && DiceTimer < 0.3f)	
				UI.DiceNr = DiceNumbers[0];
			if(DiceTimer >= 0.3f && DiceTimer < 0.6f)	
				UI.DiceNr = DiceNumbers[1];
			if(DiceTimer >= 1.0f && DiceTimer < 1.4f)
			{
				UI.DiceNr = DiceNumbers[2];
				AbleToStop = true;
			}
			if(DiceTimer >= 1.4f && DiceTimer < 1.8f)	
				UI.DiceNr = DiceNumbers[3];
			if(DiceTimer >= 1.8f && DiceTimer < 2.2f)	
				UI.DiceNr = DiceNumbers[4];
			if(DiceTimer >= 2.2f && DiceTimer < 2.7f)	
				UI.DiceNr = DiceNumbers[5];
			if(DiceTimer >= 3.3f && DiceTimer < 4.0f)	
				UI.DiceNr = DiceNumbers[6];
			if(DiceTimer >= 4.8f && DiceTimer < 5.7f)
				UI.DiceNr = DiceNumbers[7];
			if(DiceTimer >= 5.7f && DiceTimer < 6.7f)
				UI.DiceNr = DiceNumbers[8];
			if(DiceTimer >= 6.7f && DiceTimer < 7.8f)
				UI.DiceNr = DiceNumbers[9];
			if(DiceTimer >= 8.9f && DiceTimer < 10.1f)
				UI.DiceNr = DiceNumbers[10];
			if(DiceTimer >= 11.4f && DiceTimer < 12.8f)
				UI.DiceNr = DiceNumbers[11];
			if(DiceTimer >= 14.5f && DiceTimer < 16.2f)
				UI.DiceNr = DiceNumbers[12];
			if(DiceTimer >= 18.0f && DiceTimer < 19.9f)
				UI.DiceNr = DiceNumbers[13];
			if(DiceTimer >= 21.6f && DiceTimer < 22.0f)
				UI.DiceNr = DiceNumbers[14];
			if(DiceTimer >= 22.0f)
			{
					DiceTimer = 0.0f;
					AbleToStop = false;
					AnimDice = false;
			}
		}
	}

	public IEnumerator NextRoll(string Button)
	{
		while (!KeyPressed)
		{
			if (Input.GetButtonDown(Button))
			{
				CurrentPlayerNr++;
				if(CurrentPlayerNr == Players.Count)
					Application.LoadLevel("Main");
				else
					RollDice();
				break;
			}
			Debug.Log("Waiting");
			yield return 0;
		}
	}
	
	public IEnumerator WaitForDice()
	{
		while(AnimDice)
		{
			Debug.Log("Animating");
			if(Input.GetKeyDown(KeyCode.Space) && AbleToStop)
			{
				DiceTimer = 0.0f;
				UI.DiceNr = DiceNumbers[14];
				AbleToStop = false;
				AnimDice = false;
			}
			yield return 0;
		}
		UpdatePlayerPosition();
	}
	
}
