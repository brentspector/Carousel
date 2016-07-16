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
				if (blockedAttacks.Contains(move) && target.CheckAbility(15))
				{
					GameManager.instance.WriteBattleMessage(string.Format("{0}'s Bulletproof blocked {1}'s {2}!",
						target.Nickname, GameManager.instance.CheckMoveUser().Nickname, DataContents.GetMoveGameName(move)));
					return true;
				} //end if
				break;
			//Dry Skin
			case 35:
				//If a water type attack was used
				if (DataContents.GetMoveIcon(move) == 11 && target.CheckAbility(35))
				{
					//If HP can be restored
					if (target.CurrentHP < target.TotalHP && !target.CheckEffect((int)LastingEffects.HealBlock))
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
				if (DataContents.GetMoveIcon(move) == 10 && target.CheckAbility(42))
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
			//Levitate
			case 79:
				//If a ground type attack was used
				if (DataContents.GetMoveIcon(move) == 4 && target.CheckAbility(79))
				{
					//Display message
					GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to Levitate!", 
						GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(), target.Nickname));

					//Attack is negated
					return true;
				} //end if
				break;
			//Lightning Rod
			case 81:
				//If an electric type attack was used
				if(DataContents.GetMoveIcon(move) == 13 && target.CheckAbility(81))
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
				if(DataContents.GetMoveIcon(move) == 13 && target.CheckAbility(94))
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
				if(DataContents.GetMoveIcon(move) == 12 && target.CheckAbility(133))
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
			//Soundproof
			case 149:
				//Check if moved is flagged as sound based
				if (DataContents.GetMoveFlag(move, "k"))
				{
					//Attack is negated
					GameManager.instance.WriteBattleMessage(string.Format("{0}'s Soundproof blocked {1}'s {2}!", 
						target.Nickname, GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed()));
					return true;
				} //end if
				break;
			//Storm Drain
			case 157:
				//If water type attack used
				if (DataContents.GetMoveIcon(move) == 11 && target.CheckAbility(157))
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
				if(DataContents.GetMoveIcon(move) == 13 && target.CheckAbility(184))
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
				if (DataContents.GetMoveIcon(move) == 11 && target.CheckAbility(185))
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

	/***************************************
     * Name: ResolveDamageBoostingAbilities
     * Checks if an ability would activate
     * based on the move used
     ***************************************/
	public static float ResolveDamageBoostingAbilities(int ability, int move, int moveDamage, PokemonBattler battler, 
		PokemonBattler opponent, ref int moveType)
	{
		//Switch to appropriate ability
		switch(ability)
		{
			//Aerialate
			case 2:
				if (moveType == (int)Types.NORMAL && battler.CheckAbility(2))
				{
					moveType = (int)Types.FLYING;
					return 1.3f;
				} //end if
				return 1f;
			//Mega launcher
			case 90:
				//Check for a move Mega Launcher boosts
				int[] boostedAttacks = new int[] {27, 100, 126, 379, 610 
				};
				if (boostedAttacks.Contains(move) && battler.CheckAbility(90))
				{
					return 1.5f;
				} //end if 
				return 1f;
				break;
			//Pixilate
			case 109:
				if (moveType == (int)Types.NORMAL && battler.CheckAbility(109))
				{
					moveType = (int)Types.FAIRY;
					return 1.3f;
				} //end if
				return 1f;
			//Refrigerate
			case 123:
				if (moveType == (int)Types.NORMAL && battler.CheckAbility(123))
				{
					moveType = (int)Types.ICE;
					return 1.3f;
				} //end if 
				return 1f;
			//Rivalry
			case 125:
				if (battler.CheckAbility(125))
				{
					if (battler.Gender == 2 || opponent.Gender == 2)
					{
						return 1f;
					} //end if
					else if (battler.Gender == opponent.Gender)
					{
						return 1.25f;
					} //end else if
					else
					{
						return 0.75f;
					} //end else
				} //end if
				return 1f;
			//Strong Jaw
			case 158:
				//Check for a move Strong Jaw boosts
				boostedAttacks = new int[] {41, 95, 170, 283, 396, 577
				};
				if (boostedAttacks.Contains(move) && battler.CheckAbility(158))
				{
					return 1.5f;
				} //end if 
				return 1f;
			//Technician
			case 168:
				if (moveDamage <= 60)
				{
					return 1.5f;
				} //end if
				return 1f;
				break;
			//Tough Claws
			case 174:
				if (DataContents.GetMoveFlag(move, "a") && battler.CheckAbility(174))
				{
					return 1.33f;
				} //end if
				return 1f;
				break;
			//No boosting ability 
			default:
				return 1f;
		} //end switch
	} //end ResolveDamageBoostingAbilities(int ability, int move, int moveDamage, PokemonBattler battler, PokemonBattler opponent,
		//ref int moveType)

	/***************************************
     * Name: ResolveContactAbilities
     * Checks if an ability would activate
     * once the battler is hit by a contact
     * move
     ***************************************/
	public static void ResolveContactAbilities(PokemonBattler user, PokemonBattler target)
	{
		//Switch to appropriate ability
		switch (user.GetAbility())
		{
			//Cute Charm
			case 25:
				//30% chance to infatuate
				if (GameManager.instance.RandomInt(0, 10) < 3 && user.Gender != target.Gender && user.Gender != 2 
					&& !target.CheckAbility(102) && !target.CheckEffect((int)LastingEffects.Attract))
				{
					target.ApplyEffect((int)LastingEffects.Attract, user.BattlerPokemon.PersonalID);
					GameManager.instance.WriteBattleMessage(string.Format("{0} fell in love with {1} due to Cute Charm! " +
					"It may be unable to attack.", target.Nickname, user.Nickname));
				} //end if
				break;
			//Flame Body
			case 40:
				//30% chance to burn
				if (GameManager.instance.RandomInt(0, 10) < 3 && target.BattlerStatus == (int)Status.HEALTHY && 
					!target.CheckType((int)Types.FIRE))
				{
					target.BattlerStatus = (int)Status.BURN;
					GameManager.instance.WriteBattleMessage(target.Nickname + " was burned on contact with the Flame Body!");

					//Check for Syncronize
					if (target.CheckAbility(166) && user.BattlerStatus == (int)Status.HEALTHY && !user.CheckType((int)Types.FIRE))
					{
						user.BattlerStatus = (int)Status.BURN;
						GameManager.instance.WriteBattleMessage(target.Nickname + "'s Synchronize burned " +  
							user.Nickname + "!");
					} //end if
				} //end if
				break;
			//Poison Point
			case 112:
				//30% chance to poison
				if (GameManager.instance.RandomInt(0, 10) < 3 && target.BattlerStatus == (int)Status.HEALTHY && 
					!target.CheckType((int)Types.POISON))
				{
					target.BattlerStatus = (int)Status.POISON;
					GameManager.instance.WriteBattleMessage(target.Nickname + " was poisoned on contact with the Poison Point!");

					//Check for Syncronize
					if (target.CheckAbility(166) && user.BattlerStatus == (int)Status.HEALTHY && !user.CheckType((int)Types.POISON))
					{
						user.BattlerStatus = (int)Status.POISON;
						GameManager.instance.WriteBattleMessage(target.Nickname + "'s Synchronize poisoned " +  
							user.Nickname + "!");
					} //end if
				} //end if
				break;
			//Rough Skin
			case 127:
				//100% chance to inflict damage
				if (!target.CheckAbility(85))
				{
					target.RemoveHP(ExtensionMethods.BindToInt(target.TotalHP / 8, 1));
					GameManager.instance.WriteBattleMessage(target.Nickname + " was hurt by the Rough Skin!");
				} //end if
				break;
			//Static
			case 153:
				//30% chance to paralyze
				if (GameManager.instance.RandomInt(0, 10) < 3 && target.BattlerStatus == (int)Status.HEALTHY && 
					!target.CheckType((int)Types.ELECTRIC) && !target.CheckAbility(82))
				{
					target.BattlerStatus = (int)Status.PARALYZE;
					GameManager.instance.WriteBattleMessage(string.Format("{0}'s Static paralyzed {1}! It may be unable to move.",
						user.Nickname, target.Nickname));

					//Check for Syncronize
					if (target.CheckAbility(166) && user.BattlerStatus == (int)Status.HEALTHY && !user.CheckType((int)Types.ELECTRIC) &&
						!user.CheckAbility(82))
					{
						user.BattlerStatus = (int)Status.PARALYZE;
						GameManager.instance.WriteBattleMessage(target.Nickname + "'s Synchronize paralyzed " +  
							user.Nickname + "!");
					} //end if
				} //end if
				break;
		} //end switch
	} //end ResolveFaintedAbilities(PokemonBattler user, PokemonBattler target)

	/***************************************
     * Name: ResolveFaintedAbilities
     * Checks if an ability would activate
     * once the battler faints
     ***************************************/
	public static void ResolveFaintedAbilities(int ability, int move, PokemonBattler target)
	{

	} //end ResolveFaintedAbilities(int ability, int move, PokemonBattler target)
	#endregion
} //end AbilityEffects class
