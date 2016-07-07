/***************************************************************************************** 
 * File:    AI.cs
 * Summary: Contains the logic for the computer to decide battle strategies
******************************************************************************************/
#region Using
using UnityEngine;
using System.Collections;
#endregion

public class AI : MonoBehaviour 
{
    #region Variables
    #endregion

    #region Methods
	// Use this for initialization
	void Start () 
    {
	
	} //end Start
	
	// Update is called once per frame
	void Update () 
    {
	
	} //end Update
    #endregion
} //end AI class

public static class PrizeList
{
	public static void PointsPrize(Trainer player, int opponent)
	{		
		//Switch based on opponent fought
		switch (opponent)
		{
			case 1:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("Your victory count is " + player.LeaderWins[opponent] +
					". You got 500 points for winning!");
					player.PlayerPoints += 500;
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("You have scored your first win! You gain 900 points!");
					player.PlayerPoints += 900;
				} //end else
				break;
		} //end switch
	} //end PointsPrize(Trainer player, int opponent)

	public static void ItemsPrize(Trainer player, int opponent)
	{
		//Switch based on opponent fought
		switch (opponent)
		{
			case 1:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You earned no items for this victory.");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("You earned 10 " + DataContents.GetItemGameName(3) + ", and the " +
						"Bug Badge!");
					player.AddItem(3, 10);
					player.SetPlayerBadges(39, true);
					player.LeaderWins[opponent]++;
				} //end else
				break;
		} //end switch
	} //end ItemsPrize(Trainer player, int opponent)
} //end PrizeList class