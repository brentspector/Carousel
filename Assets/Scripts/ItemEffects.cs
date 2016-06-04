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
				GameManager.instance.DisplayText("This Pokemon isn't poisoned. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Antidote

		//Aspear Berry
		else if (item == 17)
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
				GameManager.instance.DisplayText("This Pokemon isn't frozen. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Aspear Berry

		//Awakening, Blue Flute
		else if (item == 20 || item == 32)
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
				GameManager.instance.DisplayText("This Pokemon isn't asleep. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Awakening, Blue Flute

		//Berry Juice
		else if (item == 24)
		{
			if (selectedPokemon.CurrentHP != selectedPokemon.TotalHP)
			{
				selectedPokemon.CurrentHP = ExtensionMethods.CapAtInt(selectedPokemon.CurrentHP + 20, selectedPokemon.TotalHP);
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " regained 20 HP!", true);
				return true;
			} //end if
			else
			{
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " is already at full HP. It won't have any effect.", true);
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
				GameManager.instance.DisplayText("This Pokemon isn't burned. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Burn Heal

		//Calcium
		else if (item == 37)
		{
			if (selectedPokemon.GetVitamin(4) < 10)
			{
				int result = ExtensionMethods.CapAtInt(selectedPokemon.GetEV(4) + 10, 255);
				selectedPokemon.SetEV(4, result);
				selectedPokemon.SetVitamin(4, selectedPokemon.GetVitamin(4) + 1);
				GameManager.instance.DisplayText(String.Format("{0}'s Special Attack EVs are now {1}!", selectedPokemon.Nickname, 
					result), true);
				return true;
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
				selectedPokemon.SetEV(3, result);
				selectedPokemon.SetVitamin(3, selectedPokemon.GetVitamin(3) + 1);
				GameManager.instance.DisplayText(String.Format("{0}'s Speed EVs are now {1}!", selectedPokemon.Nickname, 
					result), true);
				return true;
			} //end if
			else
			{					
				GameManager.instance.DisplayText(selectedPokemon.Nickname + " has consumed as many Carbos as possible. It " +
				"won't have any effect.", true);
				return false;
			} //end else
		} //end else if Carbos

		//Casteliacone
		else if (item == 40)
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
				GameManager.instance.DisplayText("This Pokemon isn't afflicted with a curable condition. It won't have any effect.", true);
				return false;
			} //end else
		} //end else if Casteliacone

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
		} //end else if Charge Stone

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
