/***************************************************************************************** 
 * File:    ItemEffects.cs
 * Summary: Applies appropriate effect based on item used
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

public static class ItemEffects
{
	#region Variables
	/******************************
	 * Outside Use
	 * 0 - Only able to be held
	 * 1 - Usable on a pokemon and dissapears after use
	 * 2 - Usable on a pokemon but remains after use
	 * Battle Use
	 * 0 - Not usable in battle
	 * 1 - Usable only by the player
	 * 2 - Usable by the player or pokemon
	 * 3 - Usable only by the pokemon
	 * 4 - Usable only by the player on an ally (doubles/triples battle)
	 ******************************/ 
	#endregion

	#region Methods
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

		//Antidote
		else if (item == 15)
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
		} //end else if Antidote

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

		//Berry Juice
		else if (item == 24)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 20, selectedPokemon.TotalHP);
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP!", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 20)), true);
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
		} //end else if Berry Juice

		//Burn Heal
		else if (item == 36)
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
		} //end else if Burn Heal

		//Calcium
		else if (item == 37)
		{
			if (selectedPokemon.GetVitamin(4) < 10)
			{
				int result = ExtensionMethods.CapAtInt(selectedPokemon.GetEV(4) + 10, 255);
				if (result - selectedPokemon.GetEV(4) != 0)
				{
					selectedPokemon.SetEV(4, result);
					selectedPokemon.SetVitamin(4, selectedPokemon.GetVitamin(4) + 1);
					int change = selectedPokemon.Happiness + 2;
					if (selectedPokemon.Happiness < 200)
					{
						if (selectedPokemon.Happiness < 100)
						{
							change += 2;
						}
						change += 1;
					} //end if
					selectedPokemon.Happiness = change;
					GameManager.instance.DisplayText(String.Format("{0}'s Special Attack EVs are now {1}!", selectedPokemon.Nickname, 
						result), true);
					return true;
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at maximum Special Attack EVs. " +
					"It won't have any effect.", true);
					return false;
				} //end else
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as many Calcium as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if Calcium

		//Carbos
		else if (item == 39)
		{
			if (selectedPokemon.GetVitamin(3) < 10)
			{
				int result = ExtensionMethods.CapAtInt(selectedPokemon.GetEV(3) + 10, 255);
				if (result - selectedPokemon.GetEV(3) != 0)
				{
					selectedPokemon.SetEV(3, result);
					selectedPokemon.SetVitamin(3, selectedPokemon.GetVitamin(3) + 1);
					int change = selectedPokemon.Happiness + 2;
					if (selectedPokemon.Happiness < 200)
					{
						if (selectedPokemon.Happiness < 100)
						{
							change += 2;
						}
						change += 1;
					} //end if
					selectedPokemon.Happiness = change;
					GameManager.instance.DisplayText(String.Format("{0}'s Speed EVs are now {1}!", selectedPokemon.Nickname, 
						result), true);
					return true;
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at maximum Speed EVs. " +
					"It won't have any effect.", true);
					return false;
				} //end else
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as many Carbos as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if Carbos

		//Casteliacone, Full Heal, Lava Cookie, Lum Berry
		else if (item == 40 || item == 97 || item == 143 || item == 158)
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
		} //end else if Casteliacone, Full Heal, Lava Cookie, Lum Berry

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

		//Cheri Berry
		else if (item == 47)
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
		} //end else if Cheri Berry

		//Clever Wing
		else if (item == 55)
		{
			int result = ExtensionMethods.CapAtInt(selectedPokemon.GetEV(5) + 1, 255);
			if (result - selectedPokemon.GetEV(5) != 0)
			{
				selectedPokemon.SetEV(5, result);
				int change = selectedPokemon.Happiness + 1;
				if (selectedPokemon.Happiness < 200)
				{
					if (selectedPokemon.Happiness < 100)
					{
						change += 1;
					}
					change += 1;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(String.Format("{0}'s Special Defence EVs are now {1}!", selectedPokemon.Nickname, 
					result), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at maximum Special Defence EVs. " +
				"It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Clever Wing

		//Dawn, Dusk, Fire, Leaf Stone
		else if (item == 60 || item == 75 || item == 89 || item == 145)
		{
			//Check if the pokemon evolves into anything with dawn stone
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
		} //end else if Dawn, Dusk, Fire, Leaf Stone

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

		//Ether = 83

		//Fresh Water
		else if (item == 96)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 50, selectedPokemon.TotalHP);
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP!", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 50)), true);
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
		} //end else if Fresh Water

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
			int result = ExtensionMethods.CapAtInt(selectedPokemon.GetEV(4) + 1, 255);
			if (result - selectedPokemon.GetEV(4) != 0)
			{
				selectedPokemon.SetEV(4, result);
				int change = selectedPokemon.Happiness + 1;
				if (selectedPokemon.Happiness < 200)
				{
					if (selectedPokemon.Happiness < 100)
					{
						change += 1;
					}
					change += 1;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(String.Format("{0}'s Special Attack EVs are now {1}!", selectedPokemon.Nickname, 
					result), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at maximum Special Attack EVs. " +
				"It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Genius Wing

		//Grepa Berry
		else if (item == 107)
		{
			//If there are EVs to erase
			if (selectedPokemon.GetEV(5) > 0)
			{
				int result = ExtensionMethods.BindToInt(selectedPokemon.GetEV(5) - 10, 0);

				//If happiness isn't at maximum
				if (selectedPokemon.Happiness < 255)
				{
					selectedPokemon.SetEV(5, result);
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
				GameManager.instance.DisplayText(string.Format("{0} loved the berry, and now has {1} Special Defence EVs.", 
					selectedPokemon.Nickname, result), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " adores you and has no Special Defence EVs to lose.", true);
				return false;
			} //end else
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
			int result = ExtensionMethods.CapAtInt(selectedPokemon.GetEV(0) + 1, 255);
			if (result - selectedPokemon.GetEV(0) != 0)
			{
				selectedPokemon.SetEV(0, result);
				int change = selectedPokemon.Happiness + 1;
				if (selectedPokemon.Happiness < 200)
				{
					if (selectedPokemon.Happiness < 100)
					{
						change += 1;
					}
					change += 1;
				} //end if
				selectedPokemon.Happiness = change;
				GameManager.instance.DisplayText(String.Format("{0}'s HP EVs are now {1}!", selectedPokemon.Nickname, 
					result), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at maximum HP EVs. " +
				"It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Health Wing

		//Hondew Berry
		else if (item == 118)
		{
			//If there are EVs to erase
			if (selectedPokemon.GetEV(4) > 0)
			{
				int result = ExtensionMethods.BindToInt(selectedPokemon.GetEV(4) - 10, 0);

				//If happiness isn't at maximum
				if (selectedPokemon.Happiness < 255)
				{
					selectedPokemon.SetEV(4, result);
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
				GameManager.instance.DisplayText(string.Format("{0} loved the berry, and now has {1} Special Attack EVs.", 
					selectedPokemon.Nickname, result), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " adores you and has no Special Attack EVs to lose.", true);
				return false;
			} //end else
		} //end else if Hondew Berry

		//HP Up
		else if (item == 120)
		{
			if (selectedPokemon.GetVitamin(0) < 10)
			{
				int result = ExtensionMethods.CapAtInt(selectedPokemon.GetEV(0) + 10, 255);
				if (result - selectedPokemon.GetEV(0) != 0)
				{
					selectedPokemon.SetEV(0, result);
					selectedPokemon.SetVitamin(0, selectedPokemon.GetVitamin(0) + 1);
					int change = selectedPokemon.Happiness + 2;
					if (selectedPokemon.Happiness < 200)
					{
						if (selectedPokemon.Happiness < 100)
						{
							change += 2;
						}
						change += 1;
					} //end if
					selectedPokemon.Happiness = change;
					GameManager.instance.DisplayText(String.Format("{0}'s HP EVs are now {1}!", selectedPokemon.Nickname, 
						result), true);
					return true;
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at maximum HP EVs. " +
					"It won't have any effect.", true);
					return false;
				} //end else
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as many HP Ups as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if HP Up

		//Hyper Potion
		else if (item == 121)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 200, selectedPokemon.TotalHP);
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP!", selectedPokemon.Nickname, 
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
		} //end else if Hyper Potion

		//Iron
		else if (item == 127)
		{
			if (selectedPokemon.GetVitamin(2) < 10)
			{
				int result = ExtensionMethods.CapAtInt(selectedPokemon.GetEV(2) + 10, 255);
				if (result - selectedPokemon.GetEV(2) != 0)
				{
					selectedPokemon.SetEV(2, result);
					selectedPokemon.SetVitamin(2, selectedPokemon.GetVitamin(2) + 1);
					int change = selectedPokemon.Happiness + 2;
					if (selectedPokemon.Happiness < 200)
					{
						if (selectedPokemon.Happiness < 100)
						{
							change += 2;
						}
						change += 1;
					} //end if
					selectedPokemon.Happiness = change;
					GameManager.instance.DisplayText(String.Format("{0}'s Defence EVs are now {1}!", selectedPokemon.Nickname, 
						result), true);
					return true;
				} //end if
				else
				{
					GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at maximum Defence EVs. " +
					"It won't have any effect.", true);
					return false;
				} //end else
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as many Irons as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if Iron

		//Kelpsy Berry
		else if (item == 137)
		{
			//If there are EVs to erase
			if (selectedPokemon.GetEV(1) > 0)
			{
				int result = ExtensionMethods.BindToInt(selectedPokemon.GetEV(1) - 10, 0);

				//If happiness isn't at maximum
				if (selectedPokemon.Happiness < 255)
				{
					selectedPokemon.SetEV(1, result);
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
				GameManager.instance.DisplayText(string.Format("{0} loved the berry, and now has {1} Attack EVs.", 
					selectedPokemon.Nickname, result), true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " adores you and has no Attack EVs to lose.", true);
				return false;
			} //end else
		} //end else if Kelpsy Berry

		//Lemonade
		else if (item == 147)
		{
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 80, selectedPokemon.TotalHP);
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP!", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 80)), true);
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
		} //end else if Lemonade

		//Leppa Berry - 148

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
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = selectedPokemon.TotalHP;
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP!", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, selectedPokemon.TotalHP)), true);
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
		} //end else if Max Potion

		//Max Revive
		else if (item == 171)
		{
			if (selectedPokemon.Status == (int)Status.FAINT)
			{
				selectedPokemon.Status = (int)Status.HEALTHY;
				selectedPokemon.StatusCount = 0;
				selectedPokemon.CurrentHP = selectedPokemon.TotalHP;
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " was revived to full health!", true);
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
			int restored = selectedPokemon.TotalHP - selectedPokemon.CurrentHP;
			if (restored > 0 && restored < selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 100, selectedPokemon.TotalHP);
				GameManager.instance.DisplayText(string.Format("{0} regained {1} HP!", selectedPokemon.Nickname, 
					ExtensionMethods.CapAtInt(restored, 100)), true);
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
		} //end else if Moomoo Milk

		//Anything else. This is what occurs when an item has no effect but is listed as usable
		else
		{
			GameManager.instance.DisplayText(DataContents.GetItemGameName(item) + " has no listed effect yet.", true);
			return false;
		} //end else
	} //end UseOnPokemon(Pokemon selectedPokemon, int item)

	public static void BattleUseOnPokemon(Pokemon selectedPokemon, int item)
	{

	} //end BattleUseOnPokemon(Pokemon selectedPokemon, int item)
	#endregion
} //end ItemEffects class
