using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	public int TricupAmount;
    public int DumbleAmount;
    public string Building;
    public string FactionName;
    public int MoveAmount;
    public string TileDesc;
    public string OptionRoll1;
    public string OptionRoll2;
    public int MaxLowerRange;
    public string Type;
	public GameManager gameManager;
    public Dice dice;

	void Start()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dice = GameObject.Find("GameManager").GetComponent<Dice>();
        TricupAmount = 6;
        DumbleAmount = 0;
        Building = string.Empty;
	}

	public string TileResponse() 
	{
        int random = Random.Range(1, 6);
        Player playerscript = gameManager.CurrentPlayerScript;
        string response = string.Empty;

        switch (Type)
        {
            case "9'3/4":
                playerscript.ExtraTurn = true;
                return "mag een geven en krijgt een extra beurt";

            case "Philosophers Stone":
                return "moet " + Random.Range(1,6) + " drinken.";

            case "SortingHat":
                playerscript.House = gameManager.Houses.AddPlayerToRandomHouse(playerscript.Name);
                return "is geplaatst in " + playerscript.House + ".";

            case "House":
                List<string> PlayerNames = gameManager.Houses.GetHouseMembers(playerscript.House);
                if (PlayerNames.Count > 1)
                {
                    for (int i = 0; i < PlayerNames.Count - 1; i++)
                    {
                        response += PlayerNames[i] + ", ";
                    }
                    response = response.Substring(0, response.Length - 2);
                    response += " en " + PlayerNames[PlayerNames.Count - 1] + " moeten " + TileDesc;
                }
                else
                {
                    response = playerscript.Name + " moet " + TileDesc;
                }

                return response;

            case "Tricup":
                if (TricupAmount-1 != 0)
                {
                    TricupAmount--;
                    return "moet " + TricupAmount + " drinken.";
                }
                else
                    return "hoeft niks te drinken.";

            case "Dumbledore":
                    DumbleAmount++;
                    return "moet " + DumbleAmount + " drinken.";

            case "Parseltongue":
                if (random < 4)
                {
                    return "moet een shot nemen. De speler heeft " + random + " gegooid";
                } 
                else
                {
                    playerscript.ExtraTurn = true;
                    return "krijgt een extra beurt. De speler heeft " + random + " gegooid";
                }

            case "Bus":
                if (random < 4)
                {
                    playerscript.SkipTurn = true;
                    return "verliest zijn volgende beurt. De speler heeft " + random + " gegooid";
                } 
                else
                {
                    playerscript.ExtraTurn = true;
                    return "krijgt een extra beurt. De speler heeft " + random + " gegooid";
                }

            case "Bazingarang":
                return "Iedereen moet hetzelfde drinken als " + playerscript.Name + " tot zijn volgende beurt.";

            case "Dementor":
                playerscript.SkipTurn = true;
                return TileDesc;

            case "SwordA":
                playerscript.Sword = true;
                return TileDesc;

            case "Time Travel":
                if(random < 6)
                {
                    gameManager.ExtraMoveAmount = -5;
                    return OptionRoll1 +  " De speler heeft " + random + " gegooid";
                }
                else
                {
                    gameManager.ExtraMoveAmount = 1;
                    return OptionRoll2 + " De speler heeft " + random + " gegooid";
                }

            case "Felix":
                gameManager.ExtraMoveAmount = 6;
                return TileDesc;

            case "SwordB":
                if(playerscript.Sword)
                {
                    return "mag iemand kiezen die zijn drinken moet finishen.";
                }
                else
                {
                    return "moet eens goed nadenken waar hij dat zwaard heeft gelaten.";
                }

            case "In the Building":
                if(Building == string.Empty)
                    Building = playerscript.House;

                if (Building != playerscript.House)
                    return "Je huis moet allemaal 2 drinken.";
                else
                    return "Je mag 3 geven";


            case "Horcrux":
                if(random <= MaxLowerRange)
                {
                    playerscript.RemainOnTile = true;
                    return OptionRoll1 + " De speler heeft " + random + " gegooid";
                }
                else
                {
                    playerscript.RemainOnTile = false;
                    return OptionRoll2 + " De speler heeft " + random + " gegooid";
                }

            default:
                return TileDesc;
        }
	}

    public string TileRoll(int diceRoll)
    {
        if (diceRoll >= 1 && diceRoll <= MaxLowerRange)
            return OptionRoll1  +  " De speler heeft " + diceRoll + " gegooid";
        else
            return OptionRoll2  +  " De speler heeft " + diceRoll + " gegooid";
    }
}
