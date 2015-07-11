using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public List<GameObject> Players;
    public List<GameObject> Tiles;
    public Vector2 TargetPos;
    public Vector2 StepPos;
    public Vector2 CurrentPos;
    public Vector2 NewPosition;
    public GameObject CurrentPlayer;
    public GameObject PlayerObject;
    public Player CurrentPlayerScript;
    public CameraFollow MainCamera;
    public UI UI;
    public bool PlayerMoving;
	public List<int> RequiredTiles;
    public int CurrentTileNr;
    public int TargetTileNr;
    public int NextTileNr;
    public int CurrentPlayerNr;
    public int ExtraMoveAmount;
	public Houses Houses;

    bool KeyPressed;
    bool RoundStarted;
    bool PlayerMoved;
    bool DiceRolled;
	double DiceTimer;
	int[] DiceNumbers;
	bool AnimDice;
    bool GameEnded;
	bool SpecialTile;
	bool AbleToStop;

    // Use this for initialization
    void Start()
    {
        UI = Camera.main.GetComponent<UI>();
		List<PlayerInfo> PlayersInfo = GameObject.Find("PlayerMenu").GetComponent<PlayerMenu>().Players;
        int PlayerNr = 1;
		Houses = GameObject.Find("Houses").GetComponent<Houses>();

        foreach (PlayerInfo PlayerInformation in PlayersInfo)
        {
            GameObject Player = (GameObject)GameObject.Instantiate(PlayerObject);
            Player.GetComponent<Player>().Name = PlayerInformation.Name;
			Player.GetComponent<Player>().PlayerNr = PlayerNr;
			Debug.Log(PlayerInformation.Icon);
			Player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(PlayerInformation.Icon);
            Players.Add(Player);
            PlayerNr++;
        }
        
		AnimDice = false;
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
		StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        AnimPlayer();
		AnimDiceRoll();
    }

    // StartGame prepares game for start
    public void StartGame()
    {
        GameEnded = false;
        Vector3 Tile1Pos = Tiles. .Find("Tile1").transform.position;
        Vector3 StartPos = new Vector3(Tile1Pos.x, Tile1Pos.y, -2);
        foreach (GameObject player in Players)
        {
            player.transform.position = StartPos;
            player.GetComponent<Player>().CurrentTile = 1;
        }
        CurrentPlayerNr = 0;
        MainCamera = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        MainCamera.currentPlayer = Players[CurrentPlayerNr];
        UI.Desc = "Press space to start the first turn.";
        StartCoroutine(StateMachine("Jump"));
    }

    // EndGame ends the current game
    public void EndGame()
    {
        Application.LoadLevel("Menu");
    }


    // StartRound prepares new round
    public void StartRound()
    {
        KeyPressed = true;
        CurrentPlayer = Players[CurrentPlayerNr];
        CurrentPlayerScript = CurrentPlayer.GetComponent<Player>();
        CurrentTileNr = CurrentPlayerScript.CurrentTile;
        RoundStarted = true;
        PlayerMoved = false;
		SpecialTile = false;
		AbleToStop = false;
        MainCamera.currentPlayer = Players[CurrentPlayerNr];
        UI.Desc = CurrentPlayer.GetComponent<Player>().Name + "'s turn has started. Press space to roll the dice.";
        StartCoroutine(Wait());
    }

    public void EndRound()
    {
        KeyPressed = true;
        if (CurrentPlayerNr + 1 == Players.Count)
            CurrentPlayerNr = 0;
        else
            CurrentPlayerNr++;
        RoundStarted = false;
        DiceRolled = false;
        KeyPressed = false;
        StartCoroutine(StateMachine("Jump"));
    }

    public void MovePlayer(int MoveAmount)
    {
		Debug.Log("Move " + MoveAmount + " spaces");
		UI.Desc = CurrentPlayer.GetComponent<Player>().Name + " moves " + MoveAmount + " spaces.";
		TargetTileNr = CurrentTileNr + MoveAmount;
		foreach(int required in RequiredTiles)
		{
			if(CurrentTileNr < required && TargetTileNr >= required)
				TargetTileNr = required;
		}
        if (TargetTileNr > 73)
            TargetTileNr = 73;
        NewPosition = CurrentPlayer.transform.position;
        PlayerMoving = true;
    }

    public void RollDice()
    {
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
		KeyPressed = false;
		StartCoroutine(WaitForDice());
    }

    public void TileResponse()
    {
        Tile Tile = GameObject.Find("Tile" + TargetTileNr).GetComponent<Tile>();
		ExtraMoveAmount = 0;
        if (Tile.MoveAmount != 0)
        {
            PlayerMoved = false;
            ExtraMoveAmount = Tile.MoveAmount;
            StartCoroutine(Wait());
        }
		else if(Tile.Sortinghat)
		{
			CurrentPlayerScript.House = Houses.AddPlayerToRandomHouse(CurrentPlayerScript.Name);
			UI.Desc = CurrentPlayer.GetComponent<Player>().Name + " is placed in " + CurrentPlayerScript.House;

		}
		else if(Tile.Philosopher)
		{
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
			StartCoroutine(WaitForDice());
		}
		else if (TargetTileNr == 73)
        {
            UI.Desc += " Press space to return to the main menu.";
            GameEnded = true;
            StartCoroutine(Wait());
        }
		else
		{
			UI.Desc = CurrentPlayer.GetComponent<Player>().Name + " " + Tile.TileDesc;
		}
        StartCoroutine(StateMachine("Jump"));
    }

    public void AnimPlayer()
    {
        if (PlayerMoving)
        {
            Vector2 CurrentPos = CurrentPlayer.transform.position;

            if (CurrentPos == NewPosition)
                if (CurrentTileNr != TargetTileNr)
                {
                    if (CurrentTileNr > TargetTileNr)
                        NextTileNr = CurrentPlayer.GetComponent<Player>().CurrentTile - 1;
                    else
                        NextTileNr = CurrentPlayer.GetComponent<Player>().CurrentTile + 1;
                    NewPosition = GameObject.Find("Tile" + NextTileNr).transform.position;
                    StepPos = new Vector2(NewPosition.x - CurrentPlayer.transform.position.x, NewPosition.y - CurrentPlayer.transform.position.y);
                    StepPos.Normalize();
                }

            Vector3 newCurrentPos = new Vector3();

            if (CurrentPos != NewPosition)
            {
                newCurrentPos = new Vector3(CurrentPos.x + (StepPos.x * Time.deltaTime * 2), CurrentPos.y + (StepPos.y * Time.deltaTime * 2), -3);
            }

            bool xCorrect = false;
            bool yCorrect = false;
            if (StepPos.x > 0.0f)
            {
                if (newCurrentPos.x >= NewPosition.x)
                    xCorrect = true;
            }
            else
            {
                if (newCurrentPos.x <= NewPosition.x)
                    xCorrect = true;
            }
            if (StepPos.y > 0.0f)
            {
                if (newCurrentPos.y >= NewPosition.y)
                    yCorrect = true;
            }
            else
            {
                if (newCurrentPos.y <= NewPosition.y)
                    yCorrect = true;
            }

            if (xCorrect && yCorrect)
            {
                CurrentTileNr = NextTileNr;
                CurrentPlayer.transform.position = new Vector3(NewPosition.x, NewPosition.y, -3);
                CurrentPlayer.GetComponent<Player>().CurrentTile = NextTileNr;
            }
            else
                CurrentPlayer.transform.position = newCurrentPos;

            if (CurrentTileNr == TargetTileNr)
            {
                CurrentPlayer.transform.position = new Vector3(NewPosition.x, NewPosition.y, -2);
                PlayerMoving = false;
                KeyPressed = false;
                PlayerMoved = true;
                TileResponse();
            }
        }
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

    public IEnumerator StateMachine(string Button)
    {
        while (!KeyPressed)
        {
			if(SpecialTile)
			{

			}
			else if (Input.GetButtonDown(Button))
            {
                if (!GameEnded)
                {
                    if (!RoundStarted)
                    {
                        Debug.Log("Round Started");
                        StartRound();
                        break;
                    }
                    else
                    {
                        if (!DiceRolled)
                        {
                            Debug.Log("Dice Rolled");
                            RollDice();
                            break;
                        }
                        else
                        {
                            if (!PlayerMoved)
                            {
					 			if(ExtraMoveAmount == 0)
								{
									MovePlayer(DiceNumbers[DiceNumbers.Length-1] + 1);
								}
								else
								{
                                	Debug.Log("Extra Move");
                                	MovePlayer(ExtraMoveAmount);
                                	break;
								}
                            }
                            else
                            {
                                Debug.Log("Round Ended");
                                EndRound();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("Game Ended");
                    EndGame();
                }
            }
            yield return 0;
        }
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        KeyPressed = false;
        Debug.Log("Wait");
        StartCoroutine(StateMachine("Jump"));
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
		DiceRolled = true;
		StartCoroutine(StateMachine("Jump"));
	}
}
