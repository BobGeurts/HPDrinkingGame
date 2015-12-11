using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Houses : MonoBehaviour {
    public List<string> AvailableHouses;
	public List<House> houses;
	House Gryffindor;
	House Slytherin;
	House Ravenclaw;
	House Hufflepuff;
	// Use this for initialization

	void Start()
	{

	}

    public string AddPlayerToRandomHouse(string playername)
    {
		Debug.Log(AvailableHouses);
		int nr = Random.Range(0, this.AvailableHouses.Count-1);
		string housename = AvailableHouses[nr];
		houses.First(x => x.HouseName == housename).members.Add(playername);
		AvailableHouses.Remove(housename);
		Debug.Log(housename);
		CheckBalance();
		return housename;
    }

    void CheckBalance()
    {
        if(AvailableHouses.Count == 0)
		{
			AvailableHouses.Add("Gryffindor");
			AvailableHouses.Add("Slytherin");
			AvailableHouses.Add("Ravenclaw");
			AvailableHouses.Add("Hufflepuff");
		}
    }

	public List<string> GetHouseMembers(string HouseName)
	{
		return houses.First(x => x.HouseName == HouseName).members;
	}

    public House getHouse(string HouseName)
    {
        return houses.First(x => x.HouseName == HouseName);
    }
}
