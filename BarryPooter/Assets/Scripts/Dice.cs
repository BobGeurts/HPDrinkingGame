using UnityEngine;
using System.Collections;

public class Dice : MonoBehaviour {
    public int[] DiceNumbers;
    public bool AnimDice;
    public bool AbleToStop;
    public double DiceTimer;
    
    public UI UI;

	// Use this for initialization
	void Start () {
	    AnimDice = false;
        AbleToStop = false;
        UI = Camera.main.GetComponent<UI>();
	}
	
	// Update is called once per frame
	void Update () {
	    AnimDiceRoll();
	}

    public void StartAnimateDice()
    {
        DiceNumbers = new int[15];
        for (int i = 0; i < DiceNumbers.Length; i++)
        {
            int DiceRoll = Random.Range(1, 6);
            if (i != 0)
                while (DiceRoll == DiceNumbers[i - 1])
                {
                    DiceRoll = Random.Range(1, 7);
                }

            DiceNumbers[i] = (DiceRoll - 1);
        }
        AnimDice = true;
    }

    public void StopAnimateDice()
    {
        DiceTimer = 0.0f;
        UI.DiceNr = DiceNumbers[14];
        AbleToStop = false;
        AnimDice = false;
    }

    public void AnimDiceRoll()
    {
        if (AnimDice)
        {
            DiceTimer += 0.1f;
            if (DiceTimer >= 0.0f && DiceTimer < 0.3f)
                UI.DiceNr = DiceNumbers[0];
            if (DiceTimer >= 0.3f && DiceTimer < 0.6f)
                UI.DiceNr = DiceNumbers[1];
            if (DiceTimer >= 1.0f && DiceTimer < 1.4f)
            {
                UI.DiceNr = DiceNumbers[2];
                AbleToStop = true;
            }
            if (DiceTimer >= 1.4f && DiceTimer < 1.8f)
                UI.DiceNr = DiceNumbers[3];
            if (DiceTimer >= 1.8f && DiceTimer < 2.2f)
                UI.DiceNr = DiceNumbers[4];
            if (DiceTimer >= 2.2f && DiceTimer < 2.7f)
                UI.DiceNr = DiceNumbers[5];
            if (DiceTimer >= 3.3f && DiceTimer < 4.0f)
                UI.DiceNr = DiceNumbers[6];
            if (DiceTimer >= 4.8f && DiceTimer < 5.7f)
                UI.DiceNr = DiceNumbers[7];
            if (DiceTimer >= 5.7f && DiceTimer < 6.7f)
                UI.DiceNr = DiceNumbers[8];
            if (DiceTimer >= 6.7f && DiceTimer < 7.8f)
                UI.DiceNr = DiceNumbers[9];
            if (DiceTimer >= 8.9f && DiceTimer < 10.1f)
                UI.DiceNr = DiceNumbers[10];
            if (DiceTimer >= 11.4f && DiceTimer < 12.8f)
                UI.DiceNr = DiceNumbers[11];
            if (DiceTimer >= 14.5f && DiceTimer < 16.2f)
                UI.DiceNr = DiceNumbers[12];
            if (DiceTimer >= 18.0f && DiceTimer < 19.9f)
                UI.DiceNr = DiceNumbers[13];
            if (DiceTimer >= 21.6f && DiceTimer < 22.0f)
                UI.DiceNr = DiceNumbers[14];
            if (DiceTimer >= 22.0f)
            {
                DiceTimer = 0.0f;
                AbleToStop = false;
                AnimDice = false;
            }
        }
    }
}
