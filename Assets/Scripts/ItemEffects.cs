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
	#endregion

	#region Methods
	public static bool UseOnPokemon(Pokemon selectedPokemon, int item)
	{
		//Awakening
		if (item == 20)
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
		} //end if Awakening
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
