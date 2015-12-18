using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public List<GameObject> Players;
    public GameObject[] Tiles;
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
    public Dice Dice;

    bool KeyPressed;
    bool RoundStarted;
    bool PlayerMoved;
    bool DiceRolled;
    bool GameEnded;
    bool ExtraThrow;

    // Use this for initialization
    void Start()
    {
        UI = Camera.main.GetComponent<UI>();
        Dice = GameObject.Find("GameManager").GetComponent<Dice>();
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
        
		StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        AnimPlayer();
    }

    // StartGame prepares game for start
    public void StartGame()
    {
        GameEnded = false;
        Vector3 Tile1Pos = Tiles[0].transform.position;
        Vector3 StartPos = new Vector3(Tile1Pos.x, Tile1Pos.y, -2);
        foreach (GameObject player in Players)
        {
            player.transform.position = StartPos;
            player.GetComponent<Player>().CurrentTile = 1;
        }
        CurrentPlayerNr = 0;
        MainCamera = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        MainCamera.currentPlayer = Players[CurrentPlayerNr];
        UI.Desc = "Druk op spatie om aan de eerste beurt te beginnen.";
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
        RoundStarted = true;
        if (!CurrentPlayerScript.SkipTurn)
        {
            CurrentTileNr = CurrentPlayerScript.CurrentTile;
            PlayerMoved = false;
            ExtraThrow = false;
            Dice.AbleToStop = false;
            MainCamera.currentPlayer = Players[CurrentPlayerNr];
            UI.Desc = CurrentPlayerScript.Name + "'s beurt is begonnen. Druk op spatie om de dobbelsteen te rollen.";
            if (!CurrentPlayerScript.RemainOnTile)
            {
                StartCoroutine(Wait());
            }
            else
            {              
                PlayerMoving = false;
                KeyPressed = false;
                PlayerMoved = true;
                DiceRolled = true;
                TargetTileNr = CurrentPlayerScript.CurrentTile;
                TileResponse();
            }
        }
        else
        {
            UI.Desc = CurrentPlayerScript.Name + " moet een beurt overslaan.";
            PlayerMoved = true;
            CurrentPlayerScript.SkipTurn = false;
            StartCoroutine(StateMachine("Jump"));
        }
    }

    public void EndRound()
    {
        KeyPressed = true;
        if (!CurrentPlayerScript.ExtraTurn)
        {
            if (CurrentPlayerNr + 1 == Players.Count)
                CurrentPlayerNr = 0;
            else
                CurrentPlayerNr++;
        }
        else
            CurrentPlayerScript.ExtraTurn = false;
        RoundStarted = false;
        DiceRolled = false;
        KeyPressed = false;
        StartCoroutine(StateMachine("Jump"));
    }

    public void MovePlayer(int MoveAmount)
    {
		Debug.Log("Move " + MoveAmount + " spaces");
        if (MoveAmount > 0)
        {
            UI.Desc = CurrentPlayer.GetComponent<Player>().Name + " zet " + MoveAmount + " stappen.";
        }
        else
        {
            UI.Desc = CurrentPlayer.GetComponent<Player>().Name + " zet " + MoveAmount + " stappen terug.";
        }

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
        Dice.StartAnimateDice();
		KeyPressed = false;
		StartCoroutine(WaitForDice());
    }

    public void TileResponse()
    {
        Tile Tile = GameObject.Find("Tile" + TargetTileNr).GetComponent<Tile>();
        if(TargetTileNr > 36 && TargetTileNr < 43)
        {
            Houses.getHouse(CurrentPlayerScript.House).Army = true;
        }
        else
        {
            bool army = false;
            if (CurrentPlayerScript.House != string.Empty)
            {
                foreach (string member in Houses.getHouse(CurrentPlayerScript.House).members)
                {
                    Debug.Log(member);
                    Player playerscript = null;
                    foreach(GameObject player in Players)
                    {
                        Player temp = player.GetComponent<Player>();
                        if(temp.Name == member)
                            playerscript = temp;
                    }
                    int tilenr = playerscript.CurrentTile;
                    if (tilenr > 36 && tilenr < 43)
                    {
                        army = true;
                    }
                }
                Houses.getHouse(CurrentPlayerScript.House).Army = army;
            }
        }

        if (CurrentPlayerScript.House != string.Empty)
            Debug.Log(Houses.getHouse(CurrentPlayerScript.House).Army);

		ExtraMoveAmount = 0;
        if (Tile.Type == "Roll") 
        {
            UI.Desc = CurrentPlayer.GetComponent<Player>().Name + " " + Tile.TileRoll(Random.Range(1,7));
            StartCoroutine(Wait());
        }
		else if (TargetTileNr == 73)
        {
            UI.Desc += " Druk op spatie om terug te keren naar het menu.";
            GameEnded = true;
            StartCoroutine(Wait());
        }
		else
		{
            string description = string.Empty;
            if (Tile.Type == "Everyone" || Tile.Type == "House" || Tile.Type == "Bazingarang")
                UI.Desc = Tile.TileResponse();
            else if (CurrentPlayerScript.House != string.Empty && Houses.getHouse(CurrentPlayerScript.House).Army)
            {
                description = CurrentPlayerScript.Name + " " + Tile.TileResponse() + " - Dumbledores Army - Indien de speler moet drinken, geldt dat ook voor:";
                List<string> PlayerNames = Houses.GetHouseMembers(CurrentPlayerScript.House);
                foreach (string woop in PlayerNames)
                {
                    Debug.Log(woop);
                }
                PlayerNames.Remove(CurrentPlayerScript.Name);
                foreach(string woop in PlayerNames)
                {
                    Debug.Log(woop);
                }

                if (PlayerNames.Count > 0)
                {
                    description += " ";
                    for (int i = 0; i < PlayerNames.Count; i++)
                    {
                        description += PlayerNames[i] + ", ";
                    }
                    description = description.Substring(0, description.Length - 2);
                }

                UI.Desc = description;
            }
            else
                UI.Desc = CurrentPlayer.GetComponent<Player>().Name + " " + Tile.TileResponse();
		}

        if(ExtraMoveAmount != 0)
        {
            PlayerMoved = false;
        }
        StartCoroutine(Wait());
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
				CheckPositionAvailable(TargetTileNr);
				PlayerMoving = false;
                KeyPressed = false;
                PlayerMoved = true;
                TileResponse();
            }
        }
    }

    public IEnumerator StateMachine(string Button)
    {
        KeyPressed = false;
        while (!KeyPressed)
        {
            if (Input.GetButtonDown(Button))
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
									MovePlayer(Dice.DiceNumbers[Dice.DiceNumbers.Length-1] + 1);
                                    break;
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
        while (Dice.AnimDice)
        {
            Debug.Log("Animating");
            if (Input.GetKeyDown(KeyCode.Space) && Dice.AbleToStop)
            {
                Dice.StopAnimateDice();
			}
			yield return 0;
		}
		DiceRolled = true;
		StartCoroutine(StateMachine("Jump"));
	}

	public void CheckPositionAvailable(int TileNumber)
	{  
		int PlayersOnTile = 0;
		foreach (GameObject player in Players) {
			if (TileNumber == player.GetComponent<Player>().CurrentTile)
			{
				int moveRightAmount = 0; 
				int moveUpAmount = 0;
				switch (PlayersOnTile) {
				case 0: break;
				case 1: moveRightAmount++;
					break;
				case 2: moveRightAmount--;
					break;
				case 3: moveUpAmount++;
					break;
				case 4: moveUpAmount--; 
					break;
				case 5: moveUpAmount++; moveRightAmount++;
					break;
				case 6: moveUpAmount++; moveRightAmount--;
					break;
				case 7: moveUpAmount--; moveRightAmount++;
					break;
				case 8: moveUpAmount--; moveRightAmount--;
                    break;
                default:
                    break;
				}
				Vector2 nieuwPos = player.transform.position;
				nieuwPos.Set (GameObject.Find("Tile"+TileNumber).transform.position.x + moveRightAmount, GameObject.Find("Tile"+TileNumber).transform.position.y + moveUpAmount);
				player.transform.position = nieuwPos;
				PlayersOnTile++;
			}
		}

	}
}
