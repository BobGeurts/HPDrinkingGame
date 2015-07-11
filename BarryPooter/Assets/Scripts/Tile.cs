using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    public bool RollTile;
    public bool Philosopher;
    public bool Move;
    public bool Sortinghat;
    public bool Faction;
	public bool Tricup;
	public bool SkipTurn;
	public bool Bus;
	public bool Black;
	public bool House;
	public int DrinkAmount;
    public string FactionName;
    public int MoveAmount;
    public string TileDesc;
    public string OptionRoll1;
    public string OptionRoll2;
    public int MaxLowerRange;
    public string Type;

	public string TileResponse(int diceRoll) 
	{
        switch (Type)
        {
            case "PhilosopherStone":
                return null;
            case "SortingHat":
                return null;
            case "Faction":
                return null;
            case "Tricup":
                return null;
            case "Bus":
                return null;
            default:
                return TileRoll(diceRoll);
        }
	}

    public string TileRoll(int diceRoll)
    {
        if (diceRoll >= 1 && diceRoll <= MaxLowerRange)
            return OptionRoll1;
        else
            return OptionRoll2;
    }

	public bool CalcBus(int diceRoll)
	{
		if (diceRoll >= 1 && diceRoll <= MaxLowerRange)
			return true;
		else
			return false;
	}
}
