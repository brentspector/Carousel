/***************************************************************************************** 
 * File:    ItemEffects.cs
 * Summary: Applies appropriate effect based on item used
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

public static class ItemEffects
{
	#region Variables
	/******************************
	 * Outside Use
	 * 0 - Only able to be held
	 * 1 - Usable on a pokemon and dissapears after use
	 * 2 - Usable on a pokemon but remains after use
	 * 3 - A TM or HM
	 * Battle Use
	 * 0 - Not usable in battle
	 * 1 - Usable only by the player
	 * 2 - Usable by the player or pokemon
	 * 3 - Usable only by the pokemon
	 * 4 - Usable only by the player on an ally (doubles/triples battle)
	 ******************************/ 
	#endregion

	#region Methods
	/***************************************
     * Name: UseOnPokemon
     * Uses an item from bag on Pokemon 
     * outside of battle
     ***************************************/ 
	public static bool UseOnPokemon(Pokemon selectedPokemon, int item)
	{
		//Ability Capsule
		if (item == 1)
		{
			//If on first ability
			if (selectedPokemon.AbilityOn == 0)
			{
				if (!selectedPokemon.CheckAbility(1))
				{
					if (!selectedPokemon.CheckAbility(2))
					{
						GameManager.instance.DisplayText(selectedPokemon.Nickname + " has no other abilities.", true);
						return false;
					} //end if
				} //end if					
			} //end if
			//If on second ability
			else if (selectedPokemon.AbilityOn == 1)
			{
				if (!selectedPokemon.CheckAbility(2))
				{
					if (!selectedPokemon.CheckAbility(0))
					{
						GameManager.instance.DisplayText(selectedPokemon.Nickname + " has no other abilities.", true);
						return false;
					} //end if
				} //end if					
			} //end else if

			//If on hidden ability or custom
			else if (selectedPokemon.AbilityOn == 2)
			{
				if (!selectedPokemon.CheckAbility(0))
				{
					if (!selectedPokemon.CheckAbility(1))
					{
						GameManager.instance.DisplayText(selectedPokemon.Nickname + " has no other abilities.", true);
						return false;
					} //end if
				} //end if					
			} //end else if

			//Ability was changed
			GameManager.instance.DisplayText(selectedPokemon.Nickname + " now has " + selectedPokemon.GetAbilityName() +
			".", true);
			return true;
		} //end if Ability Capsule

		//Antidote, Pecha Berry
		else if (item == 15 || item == 199)
		{
			if (selectedPokemon.Status == (int)Status.POISON)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of its poisoning!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't poisoned. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Antidote, Pecha Berry

		//Aspear Berry, Ice Heal
		else if (item == 17 || item == 123)
		{
			if (selectedPokemon.Status == (int)Status.FREEZE)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was thawed out!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't frozen. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Aspear Berry, Ice Heal

		//Awakening, Blue Flute, Chesto Berry
		else if (item == 20 || item == 32 || item == 48)
		{
			if (selectedPokemon.Status == (int)Status.SLEEP)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " woke up!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't asleep. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Awakening, Blue Flute, Chesto Berry

		//Berry Juice, Potion, RageCandyBar, Sweet Heart
		else if (item == 24 || item == 207 || item == 224 || item == 283)
		{
			return HealPokemon(selectedPokemon, 20);
		} //end else if Berry Juice, Potion, RageCandyBar, Sweet Heart

		//Burn Heal, Rawst Berry
		else if (item == 36 || item == 226)
		{
			if (selectedPokemon.Status == (int)Status.BURN)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + "'s burn was healed!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't burned. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Burn Heal, Rawst Berry

		//Calcium
		else if (item == 37)
		{
			if (selectedPokemon.GetVitamin(4) < 10)
			{
				return ApplyEVBoost(selectedPokemon, 10, 4, new int[]{ 2, 1, 2 });
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as much Calcium as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if Calcium

		//Carbos
		else if (item == 39)
		{
			if (selectedPokemon.GetVitamin(3) < 10)
			{
				return ApplyEVBoost(selectedPokemon, 10, 3, new int[]{ 2, 1, 2 });
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as much Carbos as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if Carbos

		//Casteliacone, Full Heal, Lava Cookie, Lum Berry, Old Gateau
		else if (item == 40 || item == 97 || item == 143 || item == 158 || item == 193)
		{
			if (selectedPokemon.Status != (int)Status.HEALTHY && selectedPokemon.Status != (int)Status.FAINT)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of its affliction!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't afflicted with a curable condition. " +
				"It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Casteliacone, Full Heal, Lava Cookie, Lum Berry, Old Gateau

		//Charge Stone
		else if (item == 43)
		{
			//Check if the pokemon evolves into anything with magnetic radiation (Location 281)
			int evolutionID = selectedPokemon.CheckEvolution(item);
			if (evolutionID != -1)
			{
				selectedPokemon.EvolvePokemon(evolutionID);
				GameManager.instance.DisplayText(string.Format("Congratulations! Your {0} evolved into {1}!", selectedPokemon.Nickname, 
					DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=" + evolutionID)), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " does not evolve by magnetic radiation. " +
				"It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Charge Stone

		//Cheri Berry, Paralyz Heal
		else if (item == 47 || item == 196)
		{
			if (selectedPokemon.Status == (int)Status.PARALYZE)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of paralysis!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't paralyzed. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Cheri Berry, Paralyz Heal

		//Clever Wing
		else if (item == 55)
		{
			return ApplyEVBoost(selectedPokemon, 1, 5, new int[]{ 1, 1, 1 });
		} //end else if Clever Wing

		//Dawn, Dusk, Fire, Leaf, Moon, Shiny, Sun, Thunder, Water Stone
		else if (item == 60 || item == 75 || item == 89 || item == 145 || item == 186 || item == 258 || item == 280 || item == 288
		         || item == 402)
		{
			//Check if the pokemon evolves into anything with the stone
			int evolutionID = selectedPokemon.CheckEvolution(item);
			if (evolutionID != -1)
			{
				selectedPokemon.EvolvePokemon(evolutionID);
				GameManager.instance.DisplayText(string.Format("Congratulations! Your {0} evolved into {1}!", selectedPokemon.Nickname, 
					DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=" + evolutionID)), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(string.Format("{0} does not evolve with a {1}. It won't have any effect.", 
					selectedPokemon.Nickname, DataContents.GetItemGameName(item)), true);
				return false;
			} //end else
		} //end else if Dawn, Dusk, Fire, Leaf, Moon, Shiny, Sun, Thunder, Water Stone

		//Destiny Stone
		else if (item == 64)
		{
			//Check if the pokemon evolves into anything with trade
			int evolutionID = selectedPokemon.CheckEvolution(item);
			if (evolutionID != -1)
			{
				selectedPokemon.EvolvePokemon(evolutionID);
				GameManager.instance.DisplayText(string.Format("Congratulations! Your {0} evolved into {1}!", selectedPokemon.Nickname, 
					DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=" + evolutionID)), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " does not evolve by trade. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Destiny Stone

		//Elixir
		else if (item == 79)
		{
			int ppRestored = 0;
			for (int i = 0; i < selectedPokemon.GetMoveCount(); i++)
			{
				ppRestored += selectedPokemon.GetMovePPMax(i) - selectedPokemon.GetMovePP(i);
				selectedPokemon.SetMovePP(i, ExtensionMethods.CapAtInt(selectedPokemon.GetMovePP(i) + 10, 
					selectedPokemon.GetMovePPMax(i)));
			} //end for

			//Make sure pp was restored
			if (ppRestored > 0)
			{
				GameManager.instance.DisplayText(string.Format("All of {0}'s moves were restored by 10!", selectedPokemon.Nickname), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " already has maximum PP. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Elixir

		//Energy Root
		else if (item == 80)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 200, selectedPokemon.TotalHP);
				int change = selectedPokemon.Happiness - 15;
				if (selectedPokemon.Happiness < 200)
				{
					change += 5;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP, but cringed badly.", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 200)), true);
				return true;
			} //end if
			else
			{
				if (restored <= 0)
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at full HP. It won't have any effect.", true);
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is fainted. It won't have any effect.", true);
				} //end else if
				return false;
			} //end else
		} //end else if Energy Root

		//Energy Powder
		else if (item == 81)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 50, selectedPokemon.TotalHP);
				int change = selectedPokemon.Happiness - 10;
				if (selectedPokemon.Happiness < 200)
				{
					change += 5;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP, but cringed.", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 200)), true);
				return true;
			} //end if
			else
			{
				if (restored <= 0)
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at full HP. It won't have any effect.", true);
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is fainted. It won't have any effect.", true);
				} //end else if
				return false;
			} //end else
		} //end else if Energy Powder

		//Ether, Leppa Berry
		else if (item == 83 || item == 148)
		{
			GameManager.instance.SetupPickMove();
			return false;
		} //end else if Ether, Leppa Berry

		//Fresh Water, Super Potion
		else if (item == 96 || item == 281)
		{
			return HealPokemon(selectedPokemon, 50);
		} //end else if Fresh Water, Super Potion

		//Full Restore
		else if (item == 99)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 50, selectedPokemon.TotalHP);
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP!", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 50)), true);
				return true;
			} //end if
			else if (selectedPokemon.Status != (int)Status.HEALTHY && selectedPokemon.Status != (int)Status.FAINT)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of its affliction!", true);
				return true;
			} //end else if
			else
			{
				if (restored <= 0)
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is healthy and at full HP. It won't have any effect.", true);
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is fainted. It won't have any effect.", true);
				} //end else if
				return false;
			} //end else
		} //end else if Full Restore

		//Genius Wing
		else if (item == 108)
		{
			return ApplyEVBoost(selectedPokemon, 1, 4, new int[]{ 1, 1, 1 });
		} //end else if Genius Wing

		//Grepa Berry
		else if (item == 107)
		{
			return ApplyHappyBerry(selectedPokemon, 5);
		} //end else if Grepa Berry

		//Heal Powder
		else if (item == 114)
		{
			if (selectedPokemon.Status != (int)Status.HEALTHY && selectedPokemon.Status != (int)Status.FAINT)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				int change = selectedPokemon.Happiness - 10;
				if (selectedPokemon.Happiness < 200)
				{
					change += 5;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of its affliction, but cringed.", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't afflicted with a curable condition. " +
				"It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Heal Powder

		//Health Wing
		else if (item == 115)
		{
			return ApplyEVBoost(selectedPokemon, 1, 0, new int[]{ 1, 1, 1 });
		} //end else if Health Wing

		//Hondew Berry
		else if (item == 118)
		{
			return ApplyHappyBerry(selectedPokemon, 4);
		} //end else if Hondew Berry

		//HP Up
		else if (item == 120)
		{
			if (selectedPokemon.GetVitamin(0) < 10)
			{
				return ApplyEVBoost(selectedPokemon, 10, 0, new int[]{ 2, 1, 2 });
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as much HP Up as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if HP Up

		//Hyper Potion
		else if (item == 121)
		{
			return HealPokemon(selectedPokemon, 200);
		} //end else if Hyper Potion

		//Iron
		else if (item == 127)
		{
			if (selectedPokemon.GetVitamin(2) < 10)
			{
				return ApplyEVBoost(selectedPokemon, 10, 2, new int[]{ 2, 1, 2 });
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as much Iron as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if Iron

		//Kelpsy Berry
		else if (item == 137)
		{
			return ApplyHappyBerry(selectedPokemon, 1);
		} //end else if Kelpsy Berry

		//Lemonade
		else if (item == 147)
		{
			return HealPokemon(selectedPokemon, 80);
		} //end else if Lemonade
			
		//Max Elixer
		else if (item == 168)
		{
			int ppRestored = 0;
			for (int i = 0; i < selectedPokemon.GetMoveCount(); i++)
			{
				ppRestored += selectedPokemon.GetMovePPMax(i) - selectedPokemon.GetMovePP(i);
				selectedPokemon.SetMovePP(i, selectedPokemon.GetMovePPMax(i));
			} //end for

			//Make sure pp was restored
			if (ppRestored > 0)
			{
				GameManager.instance.DisplayText(string.Format("All of {0}'s moves were restored by 10!", selectedPokemon.Nickname), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " already has maximum PP. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Max Elixer

		//Max Ether
		else if (item == 169)
		{
			GameManager.instance.SetupPickMove();
			return false;
		} //end else if max Ether

		//Max Potion
		else if (item == 170)
		{
			return HealPokemon(selectedPokemon, selectedPokemon.TotalHP);
		} //end else if Max Potion

		//Max Revive
		else if (item == 171)
		{
			if (selectedPokemon.Status == (int)Status.FAINT)
			{
				selectedPokemon.RevivePokemon(selectedPokemon.TotalHP / 2);
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was revived and fully restored!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is not fainted. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Max Revive

		//Moomoo Milk
		else if (item == 185)
		{
			return HealPokemon(selectedPokemon, 100);
		} //end else if Moomoo Milk

		//Muscle Wing
		else if (item == 188)
		{
			return ApplyEVBoost(selectedPokemon, 1, 1, new int[]{ 1, 1, 1 });
		} //end else if Muscle Wing

		//Oran Berry
		else if (item == 194)
		{
			return HealPokemon(selectedPokemon, 10);
		} //end else if Oran Berry

		//Pomeg Berry
		else if (item == 206)
		{
			return ApplyHappyBerry(selectedPokemon, 0);
		} //end else if Pomeg Berry

		//PP Max 
		else if (item == 215)
		{
			GameManager.instance.SetupPickMove();
			return false;
		} //end else if PP Max

		//PP Up
		else if (item == 216)
		{
			GameManager.instance.SetupPickMove();
			return false;
		} //end else if PP Up

		//Protein
		else if (item == 219)
		{
			if (selectedPokemon.GetVitamin(1) < 10)
			{
				return ApplyEVBoost(selectedPokemon, 10, 1, new int[]{ 2, 1, 2 });
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as much Protein as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if Protein

		//Qualot Berry
		else if (item == 221)
		{
			return ApplyHappyBerry(selectedPokemon, 2);
		} //end else if Qualot Berry

		//Rare Candy
		else if (item == 225)
		{
			if (selectedPokemon.CurrentLevel < 100)
			{
				selectedPokemon.CurrentLevel += 1;
				GameManager.instance.DisplayText(string.Format("{0} has advanced to level {1}!", selectedPokemon.Nickname, 
					selectedPokemon.CurrentLevel), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at maximum level. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Rare Candy

		//Resist Wing
		else if (item == 234)
		{
			return ApplyEVBoost(selectedPokemon, 1, 2, new int[]{ 1, 1, 1 });
		} //end else if Resist Wing

		//Revival Herb
		else if (item == 235)
		{
			if (selectedPokemon.Status == (int)Status.FAINT)
			{
				selectedPokemon.RevivePokemon(selectedPokemon.TotalHP);
				int change = selectedPokemon.Happiness - 20;
				if (selectedPokemon.Happiness < 200)
				{
					change += 5;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was revived and fully restored, but shook violently.", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is not fainted. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Revival Herb

		//Revive
		else if (item == 236)
		{
			if (selectedPokemon.Status == (int)Status.FAINT)
			{
				selectedPokemon.RevivePokemon(selectedPokemon.TotalHP / 2);
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was revived and restored to half health!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is not fainted. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Revive

		//Sacred Ash
		else if (item == 246)
		{
			//Track if a pokemon is revivable
			List<int> revivable = new List<int>();

			//Revive all pokemon in team if possible
			for (int i = 0; i < GameManager.instance.GetTrainer().Team.Count; i++)
			{
				if (GameManager.instance.GetTrainer().Team[i].Status == (int)Status.FAINT)
				{
					revivable.Add(i);
				} //end if
			} //end for

			//If a pokemon is revivable
			if (revivable.Any())
			{
				for (int i = 0; i < revivable.Count; i++)
				{
					GameManager.instance.GetTrainer().Team[revivable[i]].RevivePokemon(
						GameManager.instance.GetTrainer().Team[revivable[i]].TotalHP);
				} //end for

				GameManager.instance.DisplayText("Revived " + revivable.Count + " pokemon to full health!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText("You have no fainted pokemon. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Sacred Ash

		//Sitrus Berry
		else if (item == 263)
		{
			return HealPokemon(selectedPokemon, selectedPokemon.TotalHP / 4);
		} //end else if Sitrus Berry

		//Soda Pop
		else if (item == 268)
		{
			return HealPokemon(selectedPokemon, 60);
		} //end else if Soda Pop

		//Swift Wing
		else if (item == 284)
		{
			return ApplyEVBoost(selectedPokemon, 1, 3, new int[]{ 1, 1, 1 });
		} //end else if Swift Wing

		//Tamato Berry
		else if (item == 285)
		{
			return ApplyHappyBerry(selectedPokemon, 3);
		} //end else if Tamato Berry

		//TMs
		else if (item > 288 && item < 395)
		{
			GameManager.instance.SetupPickMove();
			return false;
		} //end else if TMs

		//Zinc
		else if (item == 437)
		{
			if (selectedPokemon.GetVitamin(5) < 10)
			{
				return ApplyEVBoost(selectedPokemon, 10, 5, new int[]{ 2, 1, 2 });
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as much Zinc as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if Zinc

		//Bitter Candy
		else if (item == 439)
		{
			if (selectedPokemon.CurrentLevel > 1)
			{
				selectedPokemon.CurrentLevel -= 1;
				int change = selectedPokemon.Happiness - 10;
				if (selectedPokemon.Happiness < 200)
				{
					change += 5;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(string.Format("{0} cringed while eating the candy. {0} has fallen to level {1}", 
					selectedPokemon.Nickname, selectedPokemon.CurrentLevel), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at minimum level. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Bitter Candy

		//Anything else. This is what occurs when an item has no effect but is listed as usable
		else
		{
			GameManager.instance.DisplayText(DataContents.GetItemGameName(item) + " has no listed effect yet.", true);
			return false;
		} //end else
	} //end UseOnPokemon(Pokemon selectedPokemon, int item)

	/***************************************
     * Name: BattleUseOnPokemon
     * Uses an item from bag on Pokemon 
     * inside of battle
     ***************************************/ 
	public static bool BattleUseOnPokemon(Pokemon selectedPokemon, int item)
	{		
		//Antidote, Pecha Berry
		if (item == 15 || item == 199)
		{
			if (selectedPokemon.Status == (int)Status.POISON)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of its poisoning!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't poisoned. It won't have any effect.", true);
				return false;
			} //end else
		} //end if Antidote, Pecha Berry

		//Aspear Berry, Ice Heal
		else if (item == 17 || item == 123)
		{
			if (selectedPokemon.Status == (int)Status.FREEZE)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was thawed out!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't frozen. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Aspear Berry, Ice Heal

		//Awakening, Blue Flute, Chesto Berry
		else if (item == 20 || item == 32 || item == 48)
		{
			if (selectedPokemon.Status == (int)Status.SLEEP)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " woke up!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't asleep. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Awakening, Blue Flute, Chesto Berry

		//Berry Juice, Potion, RageCandyBar, Sweet Heart
		else if (item == 24 || item == 207 || item == 224 || item == 283)
		{
			return HealPokemon(selectedPokemon, 20);
		} //end else if Berry Juice, Potion, RageCandyBar, Sweet Heart

		//Burn Heal, Rawst Berry
		else if (item == 36 || item == 226)
		{
			if (selectedPokemon.Status == (int)Status.BURN)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + "'s burn was healed!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't burned. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Burn Heal, Rawst Berry

		//Casteliacone, Full Heal, Lava Cookie, Lum Berry, Old Gateau
		else if (item == 40 || item == 97 || item == 143 || item == 158 || item == 193)
		{
			if (selectedPokemon.Status != (int)Status.HEALTHY && selectedPokemon.Status != (int)Status.FAINT)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of its affliction!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't afflicted with a curable condition. " +
				"It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Casteliacone, Full Heal, Lava Cookie, Lum Berry, Old Gateau

		//Cheri Berry, Paralyz Heal
		else if (item == 47 || item == 196)
		{
			if (selectedPokemon.Status == (int)Status.PARALYZE)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of paralysis!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't paralyzed. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Cheri Berry, Paralyz Heal

		//Elixir
		else if (item == 79)
		{
			int ppRestored = 0;
			for (int i = 0; i < selectedPokemon.GetMoveCount(); i++)
			{
				ppRestored += selectedPokemon.GetMovePPMax(i) - selectedPokemon.GetMovePP(i);
				selectedPokemon.SetMovePP(i, ExtensionMethods.CapAtInt(selectedPokemon.GetMovePP(i) + 10, 
					selectedPokemon.GetMovePPMax(i)));
			} //end for

			//Make sure pp was restored
			if (ppRestored > 0)
			{
				GameManager.instance.DisplayText(string.Format("All of {0}'s moves were restored by 10!", selectedPokemon.Nickname), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " already has maximum PP. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Elixir

		//Energy Root
		else if (item == 80)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 200, selectedPokemon.TotalHP);
				int change = selectedPokemon.Happiness - 15;
				if (selectedPokemon.Happiness < 200)
				{
					change += 5;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP, but cringed badly.", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 200)), true);
				return true;
			} //end if
			else
			{
				if (restored <= 0)
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at full HP. It won't have any effect.", true);
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is fainted. It won't have any effect.", true);
				} //end else if
				return false;
			} //end else
		} //end else if Energy Root

		//Energy Powder
		else if (item == 81)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 50, selectedPokemon.TotalHP);
				int change = selectedPokemon.Happiness - 10;
				if (selectedPokemon.Happiness < 200)
				{
					change += 5;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP, but cringed.", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 200)), true);
				return true;
			} //end if
			else
			{
				if (restored <= 0)
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at full HP. It won't have any effect.", true);
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is fainted. It won't have any effect.", true);
				} //end else if
				return false;
			} //end else
		} //end else if Energy Powder

		//Ether, Leppa Berry
		else if (item == 83 || item == 148)
		{
			GameManager.instance.SetupPickMove();
			return false;
		} //end else if Ether, Leppa Berry

		//Fresh Water, Super Potion
		else if (item == 96 || item == 281)
		{
			return HealPokemon(selectedPokemon, 50);
		} //end else if Fresh Water, Super Potion

		//Full Restore
		else if (item == 99)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 50, selectedPokemon.TotalHP);
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP!", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 50)), true);
				return true;
			} //end if
			else if (selectedPokemon.Status != (int)Status.HEALTHY && selectedPokemon.Status != (int)Status.FAINT)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of its affliction!", true);
				return true;
			} //end else if
			else
			{
				if (restored <= 0)
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is healthy and at full HP. It won't have any effect.", true);
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is fainted. It won't have any effect.", true);
				} //end else if
				return false;
			} //end else
		} //end else if Full Restore

		//Heal Powder
		else if (item == 114)
		{
			if (selectedPokemon.Status != (int)Status.HEALTHY && selectedPokemon.Status != (int)Status.FAINT)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				int change = selectedPokemon.Happiness - 10;
				if (selectedPokemon.Happiness < 200)
				{
					change += 5;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was cured of its affliction, but cringed.", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " isn't afflicted with a curable condition. " +
					"It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Heal Powder

		//Hyper Potion
		else if (item == 121)
		{
			return HealPokemon(selectedPokemon, 200);
		} //end else if Hyper Potion

		//Lemonade
		else if (item == 147)
		{
			return HealPokemon(selectedPokemon, 80);
		} //end else if Lemonade

		//Max Elixer
		else if (item == 168)
		{
			int ppRestored = 0;
			for (int i = 0; i < selectedPokemon.GetMoveCount(); i++)
			{
				ppRestored += selectedPokemon.GetMovePPMax(i) - selectedPokemon.GetMovePP(i);
				selectedPokemon.SetMovePP(i, selectedPokemon.GetMovePPMax(i));
			} //end for

			//Make sure pp was restored
			if (ppRestored > 0)
			{
				GameManager.instance.DisplayText(string.Format("All of {0}'s moves were restored by 10!", selectedPokemon.Nickname), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " already has maximum PP. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Max Elixer

		//Max Ether - 169

		//Max Potion
		else if (item == 170)
		{
			return HealPokemon(selectedPokemon, selectedPokemon.TotalHP);
		} //end else if Max Potion

		//Max Revive
		else if (item == 171)
		{
			if (selectedPokemon.Status == (int)Status.FAINT)
			{
				selectedPokemon.RevivePokemon(selectedPokemon.TotalHP / 2);
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was revived and fully restored!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is not fainted. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Max Revive

		//Moomoo Milk
		else if (item == 185)
		{
			return HealPokemon(selectedPokemon, 100);
		} //end else if Moomoo Milk

		//Oran Berry
		else if (item == 194)
		{
			return HealPokemon(selectedPokemon, 10);
		} //end else if Oran Berry

		//Revival Herb
		else if (item == 235)
		{
			if (selectedPokemon.Status == (int)Status.FAINT)
			{
				selectedPokemon.RevivePokemon(selectedPokemon.TotalHP);
				int change = selectedPokemon.Happiness - 20;
				if (selectedPokemon.Happiness < 200)
				{
					change += 5;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was revived and fully restored, but shook violently.", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is not fainted. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Revival Herb

		//Revive
		else if (item == 236)
		{
			if (selectedPokemon.Status == (int)Status.FAINT)
			{
				selectedPokemon.RevivePokemon(selectedPokemon.TotalHP / 2);
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was revived and restored to half health!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is not fainted. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Revive

		//Sacred Ash
		else if (item == 246)
		{
			//Track if a pokemon is revivable
			List<int> revivable = new List<int>();

			//Revive all pokemon in team if possible
			for (int i = 0; i < GameManager.instance.GetTrainer().Team.Count; i++)
			{
				if (GameManager.instance.GetTrainer().Team[i].Status == (int)Status.FAINT)
				{
					revivable.Add(i);
				} //end if
			} //end for

			//If a pokemon is revivable
			if (revivable.Any())
			{
				for (int i = 0; i < revivable.Count; i++)
				{
					GameManager.instance.GetTrainer().Team[revivable[i]].RevivePokemon(
						GameManager.instance.GetTrainer().Team[revivable[i]].TotalHP);
				} //end for

				GameManager.instance.DisplayText("Revived " + revivable.Count + " pokemon to full health!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText("You have no fainted pokemon. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Sacred Ash

		//Sitrus Berry
		else if (item == 263)
		{
			return HealPokemon(selectedPokemon, selectedPokemon.TotalHP / 4);
		} //end else if Sitrus Berry

		//Soda Pop
		else if (item == 268)
		{
			return HealPokemon(selectedPokemon, 60);
		} //end else if Soda Pop

		//Anything else. This is what occurs when an item has no effect but is listed as usable
		else
		{
			GameManager.instance.DisplayText(DataContents.GetItemGameName(item) + " has no listed effect yet.", true);
			return false;
		} //end else
	} //end BattleUseOnPokemon(Pokemon selectedPokemon, int item)

	/***************************************
     * Name: HealPokemon
     * Attempts to restore a pokemon by an amount
     ***************************************/ 
	static bool HealPokemon(Pokemon selectedPokemon, int amount)
	{
		int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
		if (restored > 0 && restored < selectedPokemon.TotalHP)
		{
			selectedPokemon.CurrentHP += amount;
			GameManager.instance.DisplayText(string.Format("{0} regained {1} HP!", selectedPokemon.Nickname, 
				ExtensionMethods.CapAtInt(restored, amount)), true);
			return true;
		} //end if
		else
		{
			if (restored <= 0)
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at full HP. It won't have any effect.", true);
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is fainted. It won't have any effect.", true);
			} //end else if
			return false;
		} //end else
	} //end HealPokemon(Pokemon selectedPokemon, int amount)

	/***************************************
     * Name: ApplyEVBoost
     * Attempts to add an amount of EVs to
     * a pokemon 
	 ***************************************/ 
	static bool ApplyEVBoost(Pokemon selectedPokemon, int amount, int EVTarget, int[] happyValues)
	{
		int result = ExtensionMethods.CapAtInt(selectedPokemon.GetEV(EVTarget) + amount, 255);
		string EVChanged = "";
		switch (EVTarget)
		{
			case 0:
				EVChanged = "HP";
				break;
			case 1:
				EVChanged = "Attack";
				break;
			case 2:
				EVChanged = "Defence";
				break;
			case 3:
				EVChanged = "Speed";
				break;
			case 4:
				EVChanged = "Special Attack";
				break;
			case 5:
				EVChanged = "Special Defence";
				break;
		} //end switch
		if (result - selectedPokemon.GetEV(EVTarget) > 0)
		{
			selectedPokemon.ChangeEVs(new int[]{ result }, EVTarget);
			int change = selectedPokemon.Happiness + happyValues[0];
			if (selectedPokemon.Happiness < 200)
			{
				if (selectedPokemon.Happiness < 100)
				{
					change += happyValues[2];
				}
				change += happyValues[1];
			} //end if
			selectedPokemon.Happiness = change;
			GameManager.instance.DisplayText(String.Format("{0}'s {1} EVs are now {2}!", selectedPokemon.Nickname, 
				EVChanged, result), true);
			return true;
		} //end if
		else
		{
			GameManager.instance.DisplayText(string.Format("{0} is already at maximum {1} EVs. It won't have any effect.", 
				selectedPokemon.Nickname, EVChanged), true);
			return false;
		} //end else
	} //end ApplyEVBoost(Pokemon selectedPokemon, int amount, int EVTarget, int[] happyValues)

	/***************************************
     * Name: ApplyHappyBerry
     * Attempts to apply effects of a
     * happiness berry to a pokemon
	 ***************************************/ 
	static bool ApplyHappyBerry(Pokemon selectedPokemon, int EVTarget)
	{
		//Get EVTarget as string
		string EVChanged = "";
		switch (EVTarget)
		{
			case 0:
				EVChanged = "HP";
				break;
			case 1:
				EVChanged = "Attack";
				break;
			case 2:
				EVChanged = "Defence";
				break;
			case 3:
				EVChanged = "Speed";
				break;
			case 4:
				EVChanged = "Special Attack";
				break;
			case 5:
				EVChanged = "Special Defence";
				break;
		} //end switch

		//If there are EVs to erase
		if (selectedPokemon.GetEV(EVTarget) > 0)
		{
			int result = ExtensionMethods.BindToInt(selectedPokemon.GetEV(EVTarget) - 10, 0);

			//If happiness isn't at maximum
			if (selectedPokemon.Happiness < 255)
			{
				selectedPokemon.ChangeEVs(new int[]{ result }, 2);
				int change = selectedPokemon.Happiness + 2;
				if (selectedPokemon.Happiness < 200)
				{
					if (selectedPokemon.Happiness < 100)
					{
						change += 5;
					}
					change += 3;
				} //end if
				selectedPokemon.Happiness = change;
				return true;
			} //end if
			GameManager.instance.DisplayText(string.Format("{0} loved the berry, and now has {1} {2} EVs.", 
				selectedPokemon.Nickname, result, EVChanged), true);
			return true;
		} //end if
		else
		{
			GameManager.instance.DisplayText(string.Format("{0} adores you and has no {1} EVs to lose.", selectedPokemon.Nickname, 
				EVChanged), true);
			return false;
		} //end else
	} //end ApplyHappyBerry(Pokemon selectedPokemon, int EVTarget)
	#endregion
} //end ItemEffects class
