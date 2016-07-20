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
	public static void LosingSpeech(Trainer player, int opponent)
	{
		//Switch based on opponent fought
		switch (opponent)
		{
			case 196:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("You always manage to beat me, " + player.PlayerName + ". I'll " +
						"get you next time!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". Great job!");
				} //end else
				break;
			case 199:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("You sure have some impressive skills, " + player.PlayerName + "!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". I'll try harder!");
				} //end else
				break;
			case 202:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("How did you manage to topple me, " + player.PlayerName + "? I'll " +
						"improve for next time!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". Fight on!");
				} //end else
				break;
			case 205:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("Yer quite a talented trainer, " + player.PlayerName + ". Yer gonna " +
						"be a mighty force!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". Upstart whippersnapper!");
				} //end else
				break;
			case 208:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("Your abilities are really sharp, " + player.PlayerName + "! I'll " +
						"to get my pokemon into my Mach 2 training!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". Time to make another version!");
				} //end else
				break;
			case 211:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("Now there's a smile I can appreciate! " + player.PlayerName + ", I " +
						"look forward to a rematch!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". It's always a captivating " +
						"match!");
				} //end else
				break;
			case 214:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("Always seek your own way, " + player.PlayerName + "!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". Continue your journey!");
				} //end else
				break;
			case 217:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("You've burned so hot my Pokemon have melted, " + player.PlayerName + "!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". Another chance to improve!");
				} //end else
				break;
			case 220:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("You've got a firey passion, " + player.PlayerName + "!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". Such an amazing talent!");
				} //end else
				break;
			case 223:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage(player.PlayerName + ", you're a force that can shake even my rigid Pokemon!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". A will of Steel!");
				} //end else
				break;
			case 226:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("Not even the beasts of legend will ignore your strength, " + 
						player.PlayerName + "!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". You are truely a legend!");
				} //end else
				break;
			case 229:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("I will not be able to forget the triumph you accomplished here, " + 
						player.PlayerName + "!");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". Although defeat is bitter, " +
						"the joy of watching you grow is sweet!");
				} //end else
				break;
			case 232:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] == 0)
				{
					GameManager.instance.WriteBattleMessage("My word! You've defeated even the highest authority in the land, " + 
						player.PlayerName + "! I can only hope we'll meet again.");
				} //end if
				else
				{
					GameManager.instance.WriteBattleMessage("Well, you beat me, " + player.PlayerName + ". You clearly have raised your " +
						"Pokemon with love!");
				} //end else
				break;
			default:
				GameManager.instance.WriteBattleMessage("This person has nothing to say about losing.");
				break;
		} //end switch
	} //end LosingSpeech(Trainer player, int opponent)

	public static void PointsPrize(Trainer player, int opponent)
	{		
		//Switch based on opponent fought
		switch (opponent)
		{
			case 196:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(400 - (200 * player.LeaderWins[opponent]), 25);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 400;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 199:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(500 - (200 * player.LeaderWins[opponent]), 25);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 500;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 202:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(600 - (200 * player.LeaderWins[opponent]), 25);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 600;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 205:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(700 - (160 * player.LeaderWins[opponent]), 25);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 700;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 208:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(800 - (170 * player.LeaderWins[opponent]), 25);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 800;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 211:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(900 - (180 * player.LeaderWins[opponent]), 25);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 900;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 214:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(1000 - (180 * player.LeaderWins[opponent]), 5);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 1000;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 217:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(1000 - (180 * player.LeaderWins[opponent]), 50);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 1000;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 220:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(1200 - (180 * player.LeaderWins[opponent]), 100);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 1200;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 223:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(1200 - (180 * player.LeaderWins[opponent]), 100);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 1200;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 226:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(1200 - (180 * player.LeaderWins[opponent]), 100);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 1200;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 229:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(1200 - (180 * player.LeaderWins[opponent]), 100);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 1200;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			case 232:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					player.LeaderWins[opponent]++;
					int points = ExtensionMethods.BindToInt(1500 - (200 * player.LeaderWins[opponent]), 150);
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("Your victory count is now " + player.LeaderWins[opponent] + ". You " +
						"have earned " + points + " points!");
				} //end if
				else
				{
					int points = 1500;
					player.PlayerPoints += points;
					GameManager.instance.WriteBattleMessage("You have scored your first win! You have earned " + points + " points!");
				} //end else
				break;
			default:
				GameManager.instance.WriteBattleMessage("You got 50 points for winning!");
				player.PlayerPoints += 50;
				break;
		} //end switch
	} //end PointsPrize(Trainer player, int opponent)

	public static void ItemsPrize(Trainer player, int opponent)
	{
		//Switch based on opponent fought
		switch (opponent)
		{
			case 196:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(334, 1);
					player.SetPlayerBadges(39, true);
					GameManager.instance.WriteBattleMessage("You were given the Bug Badge and TM45 - Attact! Bugs are attracted to " +
						"light, so it seems fitting to give this to the brightest light!");
				} //end else
				break;
			case 199:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(312, 1);
					player.SetPlayerBadges(40, true);
					GameManager.instance.WriteBattleMessage("You were given the Cliff Badge and TM23 - Smack Down! It brings the lofty " +
						"fliers down to earth.");
				} //end else
				break;
			case 202:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(296, 1);
					player.SetPlayerBadges(41, true);
					GameManager.instance.WriteBattleMessage("You were given the Rumble Badge and TM08 - Bulk Up! Even the best " +
						"combatants need to keep up their training!");
				} //end else
				break;
			case 205:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(311, 1);
					player.SetPlayerBadges(42, true);
					GameManager.instance.WriteBattleMessage("You were given the Grass Badge and TM22 - Solarbeam! Light gives us the " +
						"energy to grow, but we can also release that power to do right in the world.");
				} //end else
				break;
			case 208:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(362, 1);
					player.SetPlayerBadges(43, true);
					GameManager.instance.WriteBattleMessage("You were given the Voltage Badge and TM73 - Thunder Wave! While not as " +
						"potent as Petrificus Totalus, your opponent may not be able to respond to your attacks.");
				} //end else
				break;
			case 211:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(347, 1);
					player.SetPlayerBadges(44, true);
					GameManager.instance.WriteBattleMessage("You were given the Fairy Badge and TM58 - Sky Drop! Fly with your dreams " +
						"and let nothing hold you back!");
				} //end else
				break;
			case 214:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(350, 1);
					player.SetPlayerBadges(45, true);
					GameManager.instance.WriteBattleMessage("You were given the Psychic Badge and TM61 - Will O Wisp! Its got a " +
						"cosmic energy that will leave your opponent hurting.");
				} //end else
				break;
			case 217:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(368, 1);
					player.SetPlayerBadges(46, true);
					GameManager.instance.WriteBattleMessage("You were given the Iceberg Badge and TM79 - Frost Breath! Chill your " +
						"opponent right to the core and let them feel your might!");
				} //end else
				break;
			case 220:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(225, 1);
					GameManager.instance.WriteBattleMessage("You were given a Rare Candy! Keep your momentum going!");
				} //end else
				break;
			case 223:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(225, 1);
					GameManager.instance.WriteBattleMessage("You were given a Rare Candy! Keep your momentum going!");
				} //end else
				break;
			case 226:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(225, 1);
					GameManager.instance.WriteBattleMessage("You were given a Rare Candy! Keep your momentum going!");
				} //end else
				break;
			case 229:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(225, 1);
					GameManager.instance.WriteBattleMessage("You were given a Rare Candy! Keep your momentum going!");
				} //end else
				break;
			case 232:
				//Check if player has fought this opponent before
				if (player.LeaderWins[opponent] > 0)
				{
					GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				} //end if
				else
				{
					player.LeaderWins[opponent]++;
					player.AddItem(174, 1);
					GameManager.instance.WriteBattleMessage("You were given a Mega Ring! Unlock the hidden potential in your " +
						"loyal battle partners!");
					for (int i = 0; i < GameManager.instance.GetTrainer().Team.Count; i++)
					{
						GameManager.instance.GetTrainer().Team[i].ChangeRibbons(0);
					} //end for
				} //end else
				break;
			default:
				GameManager.instance.WriteBattleMessage("You did not gain any items for this victory.");
				break;
		} //end switch
	} //end ItemsPrize(Trainer player, int opponent)
} //end PrizeList class