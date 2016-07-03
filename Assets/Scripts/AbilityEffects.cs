/***************************************************************************************** 
 * File:    AbilityEffects.cs
 * Summary: Static class for resolving ability effects
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

public static class AbilityEffects 
{
	#region Variables
	#endregion

	#region Methods
	/***************************************
     * Name: ResolveBlockingAbilities
     * Checks if an ability would block an
     * attack or effect
     ***************************************/
	public static bool ResolveBlockingAbilities(int ability, int move, PokemonBattler target)
	{
		//Switch to correct ability
		switch (ability)
		{
			//Bulletproof
			case 15:
				//Check for a move bulletproof blocks
				int[] blockedAttacks = new int[] { 4, 27, 32, 64, 141, 144, 151, 188, 227, 280, 327, 354, 359, 376, 456, 473, 
					476, 480, 506, 615, 632
				};
				if (blockedAttacks.Contains(move))
				{
					GameManager.instance.WriteBattleMessage(string.Format("{0}'s Bulletproof blocked {1}'s {2}!",
						target.Nickname, GameManager.instance.CheckMoveUser().Nickname, DataContents.GetMoveGameName(move)));
					return true;
				} //end if
				break;
			//Dry Skin
			case 35:
				//If a water type attack was used
				if (DataContents.GetMoveIcon(move) == 11)
				{
					//If HP can be restored
					if (target.CurrentHP < target.TotalHP && target.CheckEffect((int)LastingEffects.HealBlock))
					{
						//Restore HP
						target.RestoreHP(target.TotalHP / 4);

						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} healed {2} due to {3}!", 
							GameManager.instance.CheckMoveUser().Nickname, DataContents.GetMoveGameName(move),
							target.Nickname, target.GetAbilityName()));
					} //end if
					else
					{
						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to {3}!", 
							GameManager.instance.CheckMoveUser().Nickname, DataContents.GetMoveGameName(move),
							target.Nickname, target.GetAbilityName()));
					} //end else

					//Attack is negated
					return true;	
				} //end if
				break;
			//Flash Fire
			case 42:
				//Check if fire type attack used
				if (DataContents.GetMoveIcon(move) == 10)
				{
					//See if Flash Fire can be activated
					if (target.CheckEffect((int)LastingEffects.FlashFire))
					{
						//Flash Fire is activated
						target.ApplyEffect((int)LastingEffects.FlashFire, 1);

						//Disply message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} raised {2}'s Fire power with Flash Fire!", 
							GameManager.instance.CheckMoveUser().Nickname, DataContents.GetMoveGameName(move),
							target.Nickname));
					} //end if
					else
					{
						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to Flash Fire!", 
							GameManager.instance.CheckMoveUser().Nickname, DataContents.GetMoveGameName(move),
							target.Nickname));
					} //end else

					//Attack is negated
					return true;
				} //end if
				break;
			//Lightning Rod
			case 81:
				//If an electric type attack was used
				if(DataContents.GetMoveIcon(move) == 13)
				{				
					//If special attack can be increased
					if (!target.CheckType(4) && target.GetStage(4) < 6)
					{
						//Increase special attack stage
						target.SetStage(1, 4);

						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} raised {2}'s Special Attack with Lightning Rod!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end if
					else
					{
						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to Lightning Rod!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end else

					//Attack is negated
					return true;
				} //end if
				break;
			//Motor Drive
			case 94:
				//If an electric type attack was used
				if(DataContents.GetMoveIcon(move) == 13)
				{
					//If speed can be increased
					if (!target.CheckType(4) && target.GetStage(3) < 6)
					{
						//Increase speed stage
						target.SetStage(1, 3);

						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} raised {2}'s Speed with Motor Drive!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end if
					else
					{
						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to Motor Drive!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end else

					//Attack is negated
					return true;
				} //end if
				break;
			//Sap Sipper
			case 133:
				//If grass type attack used
				if(DataContents.GetMoveIcon(move) == 12)
				{
					//If attack can be increased
					if (target.GetStage(1) < 6)
					{
						//Increase attack stage
						target.SetStage(1, 1);

						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} raised {2}'s Attack with Sap Sipper!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end if
					else
					{
						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to Sap Sipper!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end else

					//Attack is negated
					return true;
				} //end if
				break;
			//Storm Drain
			case 157:
				//If water type attack used
				if (DataContents.GetMoveIcon(move) == 11)
				{
					//If special attack can be increased
					if (target.GetStage(4) < 6)
					{
						//Increase special attack stage
						target.SetStage(1, 4);

						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} raised {2}'s Special Attack with Storm Drain!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end if
					else
					{
						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to Storm Drain!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end else

					//Attack is negated
					return true;
				} //end if
				break;
			//Volt Absorb
			case 184:
				//If an electric type attack was used
				if(DataContents.GetMoveIcon(move) == 13)
				{
					//If HP can be restored
					if (target.CurrentHP < target.TotalHP && target.CheckEffect((int)LastingEffects.HealBlock))
					{
						//Restore HP
						target.RestoreHP(target.TotalHP / 4);

						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} healed {2} due to Volt Absorb!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end if
					else
					{
						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to Volt Absorb!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));
					} //end else

					//Attack is negated
					return true;
				} //end if
				break;
			//Water Absorb
			case 185:
				//If a water type attack was used
				if (DataContents.GetMoveIcon(move) == 11)
				{
					//If HP can be restored
					if (target.CurrentHP < target.TotalHP && target.CheckEffect((int)LastingEffects.HealBlock))
					{
						//Restore HP
						target.RestoreHP(target.TotalHP / 4);

						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} healed {2} due to {3}!", 
							GameManager.instance.CheckMoveUser().Nickname, DataContents.GetMoveGameName(move),
							target.Nickname, target.GetAbilityName()));
					} //end if
					else
					{
						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to {3}!", 
							GameManager.instance.CheckMoveUser().Nickname, DataContents.GetMoveGameName(move),
							target.Nickname, target.GetAbilityName()));
					} //end else

					//Attack is negated
					return true;	
				} //end if
				break;
		} //end switch

		//Nothing resolved, return false
		return false;
	} //end ResolveBlockingAbilities(int move)
	#endregion
} //end AbilityEffects class
