/***************************************************************************************** 
 * File:    PokemonBattler.cs
 * Summary: Represents a pokemon in battle
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

public class PokemonBattler
{
	#region Variables
	int currentHP;			//Current HP
	int totalHP;			//Max HP
	int attack;				//Standard attack
	int defense;			//Standard defense
	int speed;              //Standard speed
	int specialA;			//Standard special attack
	int specialD;			//Standard special defense
	int ability;			//What ability does this pokemon currently have
	int turnCount;			//How many turns have passed
	int lastHPLost;			//The last mount of HP this pokemon lost
	int lastMoveUsed;		//The last attack this pokemon used
	int sideOn;				//What side the pokemon is on
	List<int> moves;		//What moves does this pokemon currently have
	List<int> types;		//What types does this pokemon have
	List<int> stages;		//Buffs and debuffs to the stats of the pokemon
	List<int> effects;		//Attack effects this pokemon is under
	bool hasSubstitute;		//Is there a substitute active for this pokemon
	bool isMega;			//Is this Pokemon mega-evolved
	Pokemon battler;		//The pokemon this object is representing
	#endregion

	#region Methods
	/***************************************
     * Name: PokemonBattler
     * Creates a new PokemonBattler with a
     * pokemon to represent
     ***************************************/
	public PokemonBattler(Pokemon toRepresent)
	{
		//Initialize fields
		battler = toRepresent;
		currentHP = battler.CurrentHP;
		totalHP = battler.TotalHP;
		attack = battler.Attack;
		defense = battler.Defense;
		speed = battler.Speed;
		specialA = battler.SpecialA;
		specialD = battler.SpecialD;
		ability = battler.Ability;
		turnCount = 0;
		lastHPLost = 0;
		lastMoveUsed = -1;
		moves = new List<int>();
		types = new List<int>();
		stages = new List<int>();
		effects = new List<int>();
		hasSubstitute = false;
		isMega = false;

		//Set moves
		for (int i = 0; i < 4; i++)
		{
			if (i < battler.GetMoveCount())
			{
				moves.Add(battler.GetMove(i));
			} //end if
			else
			{
				moves.Add(-1);
			} //end else
		} //end for

		//Set types
		types.Add(battler.Type1);
		if (battler.Type2 != -1)
		{
			types.Add(battler.Type2);
		} //end if

		//Set stages
		for (int i = 0; i < 6; i++)
		{
			stages.Add(0);
		} //end for

		//Initialize effects
		for (int i = 0; i < (int)LastingEffects.COUNT; i++)
		{
			effects.Add(0);
		} //end for
		InitializeEffects();
	} //end PokemonBattler(Pokemon toRepresent)

	/***************************************
     * Name: InitializeEffects
     * Sets each effect to the proper start
     ***************************************/
	void InitializeEffects()
	{
		//LeechSeed is target based
		effects[(int)LastingEffects.LeechSeed] = -1;

		//LockOnPos is based on ID of target
		effects[(int)LastingEffects.LockOnPos] = -1;

		//Attract is based on ID of target
		effects[(int)LastingEffects.Attract] = -1;

		//BideTarget is target based
		effects[(int)LastingEffects.BideTarget] = -1;

		//Choice is move based
		effects[(int)LastingEffects.Choice] = -1;

		//CounterTarget is target based
		effects[(int)LastingEffects.CounterTarget] = -1;

		//MirrorCoatTarget is target based
		effects[(int)LastingEffects.MirrorCoatTarget] = -1;

		//MultiturnUser is target based
		effects[(int)LastingEffects.MultiTurnUser] = -1;

		//Protect rate starts at 100%
		effects[(int)LastingEffects.ProtectRate] = 1;

		//Weight multiplier starts at 1
		effects[(int)LastingEffects.WeightDivisor] = 1;

		//Illusion is target based
		effects[(int)LastingEffects.Illusion] = -1;

		//MeanLookTarget is target based
		effects[(int)LastingEffects.MeanLookTarget] = -1;
	} //end InitializeEffects

	/***************************************
     * Name: SwitchInPokemon
     * Switches out active pokemon for 
     * parameter
     ***************************************/
	public void SwitchInPokemon(Pokemon toSwitch, bool retainStats = false)
	{
		//Check for Natural Cure
		if (CheckAbility(99))
		{
			battler.Status = (int)Status.HEALTHY;
			battler.StatusCount = 0;
		} //end if

		//Check for Regenerator
		if (CheckAbility(124))
		{
			battler.CurrentHP += battler.CurrentHP / 3;
		} //end if

		//Initialize fields
		battler = toSwitch;
		currentHP = battler.CurrentHP;
		totalHP = battler.TotalHP;
		attack = battler.Attack;
		defense = battler.Defense;
		speed = battler.Speed;
		specialA = battler.SpecialA;
		specialD = battler.SpecialD;
		ability = battler.Ability;

		//Initialize moves
		for (int i = 0; i < 4; i++)
		{
			if (i < battler.GetMoveCount())
			{
				moves[i] = battler.GetMove(i);
			} //end if
			else
			{
				moves[i] = -1;
			} //end else
		} //end for

		//Set types
		types.Clear();
		types.Add(battler.Type1);
		if (battler.Type2 != -1)
		{
			types.Add(battler.Type2);
		} //end if

		//Reset fields if not requested to retain
		if (!retainStats)
		{
			//Set stages
			for (int i = 0; i < 6; i++)
			{
				stages[i] = 0;
			} //end for

			//Set effects
			effects[(int)LastingEffects.AquaRing] = 0;
			effects[(int)LastingEffects.Confusion] = 0;
			effects[(int)LastingEffects.Curse] = 0;
			effects[(int)LastingEffects.Embargo] = 0;
			effects[(int)LastingEffects.FocusEnergy] = 0;
			effects[(int)LastingEffects.GastroAcid] = 0;
			effects[(int)LastingEffects.HealBlock] = 0;
			effects[(int)LastingEffects.Ingrain] = 0;
			effects[(int)LastingEffects.LeechSeed] = -1;
			effects[(int)LastingEffects.LockOn] = 0;
			effects[(int)LastingEffects.LockOnPos] = -1;
			effects[(int)LastingEffects.MagnetRise] = 0;
			effects[(int)LastingEffects.MeanLook] = 0;
			effects[(int)LastingEffects.MeanLookTarget] = -1;
			effects[(int)LastingEffects.PerishSong] = 0;
			effects[(int)LastingEffects.PerishSongUser] = 0;
			effects[(int)LastingEffects.PowerTrick] = 0;
			effects[(int)LastingEffects.Substitute] = 0;
			effects[(int)LastingEffects.Telekinesis] = 0;
		} //end if

		//Change fields if retained stats
		else
		{
			if (effects[(int)LastingEffects.LockOn] > 0)
			{
				effects[(int)LastingEffects.LockOn] = 2;
			} //end if
			else
			{
				effects[(int)LastingEffects.LockOn] = 0;
			} //end else

			if (effects[(int)LastingEffects.PowerTrick] > 0)
			{
				int temp = attack;
				attack = defense;
				defense = temp;
			} //end if
		} //end else

		//Reset these fields regardless of retain stats
		lastHPLost = 0;
		lastMoveUsed = 0;
		turnCount = 0;
	} //end SwitchInPokemon(bool retainStats = false)

	/***************************************
     * Name: CheckTyping
     * Determine the impact a move will have
     * this pokemon
     ***************************************/
	public float CheckTyping(int moveType)
	{
		//Make sure attack type exists
		if (moveType < 0)
		{
			return 1;
		} //end if

		//If Flying type holding Iron Ball is hit by a Ground Move
		if (moveType == 4 && types.Contains(2) && CheckItem(128))
		{
			return 1;
		} //end if

		//Keep a list of modifiers
		List<int> mods = new List<int>();

		//Loop through and get the modifiers for each
		for (int i = 0; i < types.Count; i++)
		{
			//Check for Roost
			if (effects[(int)LastingEffects.Roost] > 0 && types[i] == 2)
			{
				//Change to Normal Type if pure Flying
				if (types.Count == 1)
				{
					mods.Add(DataContents.typeChart.DetermineTyping(moveType, 0));
				} //end if
				//Remove typing otherwise
				else
				{
					mods.Add(1);
				} //end else
			} //end if				

			//Otherwise process normally
			else
			{
				//Get base mod
				mods.Add(DataContents.typeChart.DetermineTyping(moveType, types[i]));

				//Check for Ring Target
				if (CheckItem(238) && mods[i] == 0)
				{
					mods[i] = 1;
				} //end if

				//Check for Sap Sipper
				if (moveType == 12 && CheckAbility(133))
				{
					//If attack can be increased
					if (stages[1] < 6)
					{
						//Increase attack stage
						stages[1]++;

						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} raised {2}'s Attack with Sap Sipper!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(),
							battler.Nickname));
					} //end if
					else
					{
						//Display message
						GameManager.instance.WriteBattleMessage(string.Format("{0}'s {1} had no effect on {2} due to Sap Sipper!", 
							GameManager.instance.CheckMoveUser().Nickname, GameManager.instance.CheckMoveUsed(),
							battler.Nickname));
					} //end else

					//Attack is negated
					return 0;
				} //end if

				//Check for Scrappy/Foresight
				if ((moveType == 0 || moveType == 1) && types[i] == 7 &&
					(GameManager.instance.CheckMoveUser().Ability == 134 || effects[(int)LastingEffects.Foresight] > 0))
				{
					mods[i] = 1;										
				} //end if

				//Check for Miracle Eye
				if(moveType == 14 && types[i] == 17 && effects[(int)LastingEffects.MiracleEye] > 0)
				{
					mods[i] = 1;
				} //end if

				//Check for Ingrain/Smack Down/Gravity
				if(moveType == 4 && types[i] == 2 && (effects[(int)LastingEffects.Ingrain] > 0 || 
					effects[(int)LastingEffects.SmackDown] > 0 || GameManager.instance.CheckEffect((int)FieldEffects.Gravity, sideOn)))
				{
					mods[i] = 1;
				} //end if
			} //end else

			//If mod still is immune, return
			if (mods[i] == 0)
			{
				return 0;
			} //end if
		} //end for
	} //end CheckTyping(int moveType)

	/***************************************
     * Name: UseItemOnPokemon
     * Attempts to use an item on the target
     * pokemon in battle
     ***************************************/
	public bool UseItemOnPokemon(Pokemon toTarget, int itemNumber)
	{
		//Try to use on pokemon in battle
		if (ItemEffects.BattleUseOnPokemon(toTarget, itemNumber))
		{
			//If item is only one use
			if (DataContents.ExecuteSQL<int>("SELECT battleUse FROM Items WHERE id=" + itemNumber) == 1 ||
				DataContents.ExecuteSQL<int>("SELECT battleUse FROM Items WHERE id=" + itemNumber) == 2 ||
				DataContents.ExecuteSQL<int>("SELECT battleUse FROM Items WHERE id=" + itemNumber) == 4)
			{
				GameManager.instance.GetTrainer().RemoveItem(itemNumber, 1);
			} //end if

			//Success
			return true;
		} //end if

		//Failed
		return false;
	} //end UseItemOnPokemon(Pokemon toTarget, int itemNumber)

	/***************************************
     * Name: RestoreHP
     * Attempts to restore HP
     ***************************************/
	public void RestoreHP(int amount)
	{
		if (effects[(int)LastingEffects.HealBlock] == 0)
		{
			battler.CurrentHP += amount;
		} //end if
	} //end RestoreHP(int amount)

	/***************************************
     * Name: SetStage
     * Sets the requested stage to the given
     * value, or adjusts by the amount given
     ***************************************/
	public void SetStage(int amount, int stage, bool adjust = true)
	{
		stages[stage] = adjust ? stages[stage] + amount : amount;
		stages[stage] = ExtensionMethods.CapAtInt(stages[stage], 6);
	} //end SetStage(int amount, int stage, bool adjust = true)

	/***************************************
     * Name: GetSpeed
     * Returns the working value of the speed
     * of the pokemon
     ***************************************/
	public int GetSpeed()
	{
		//Get base speed
		int result = speed * 10;

		//Apply stage effect
		if (stages[3] < 0)
		{
			result /= 10 + (stages[3] * 5);
		} //end if
		else
		{
			result *= 10 + (stages[3] * 5);
			result /= 100;
		} //end else

		//Check for tailwind
		if (GameManager.instance.CheckEffect((int)FieldEffects.Tailwind, sideOn))
		{
			result *= 2;
		} //end if

		//Check for Swift Swim
		if (CheckAbility(164) && GameManager.instance.CheckEffect((int)FieldEffects.Rain, sideOn))
		{
			result *= 2;
		} //end if

		//Check for Chlorophyll
		if (CheckAbility(17) && GameManager.instance.CheckEffect((int)FieldEffects.Sun, sideOn))
		{
			result *= 2;
		} //end if

		//Check for Sand Rush
		if (CheckAbility(130) && GameManager.instance.CheckEffect((int)FieldEffects.Sand, sideOn))
		{
			result *= 2;
		} //end if

		//Check for Quick Feet
		if (CheckAbility(119) && battler.Status != (int)Status.HEALTHY && battler.Status != (int)Status.FAINT)
		{
			result *= 15;
			result /= 10;
		} //end if

		//Check for Slow Start
		if (CheckAbility(143) && turnCount > 0)
		{
			result /= 2;
		} //end if

		//Check for Choice Scarf
		if (CheckItem(52))
		{
			result *= 15;
			result /= 10;
		} //end if

		//Check for Macho Brace, Power items, or Iron Ball
		if (CheckItem(128) || CheckItem(161) || CheckItem(208) || CheckItem(209) || CheckItem(210) || CheckItem(211) || 
			CheckItem(213) || CheckItem(214))
		{
			result /= 2;
		} //end if

		//Check for Quick Powder
		if (CheckItem(223) && battler.NatSpecies == 132 && effects[(int)LastingEffects.Transform] == 0)
		{
			result *= 2;
		} //end if

		//Check for Paralysis
		if (battler.Status == (int)Status.PARALYZE && !CheckAbility(119))
		{
			speed /= 4;
		} //end if
		return result;
	} //end GetSpeed

	/***************************************
     * Name: GetWeight
     * Returns the working value of the weight
     * of the pokemon
     ***************************************/
	public float GetWeight()
	{
		//Get base weight
		float weight = DataContents.ExecuteSQL<float>("SELECT weight FROM Pokemon WHERE rowid=" + battler.NatSpecies);

		//Heavy Metal
		if (CheckAbility(58))
		{
			weight *= 2;
		} //end if

		//Light Metal
		if (CheckAbility(80))
		{
			weight /= 2;
		} //end if

		//Automize
		weight /= effects[(int)LastingEffects.WeightDivisor];

		//Return
		return weight;
	} //end GetWeight

	/***************************************
     * Name: CheckAbility
     * Checks if ability is present and active
     ***************************************/
	public bool CheckAbility(int abilityToCheck)
	{
		//Check for ability, then if active
		if (ability != abilityToCheck || effects[(int)LastingEffects.GastroAcid] > 0)
		{
			return false;
		} //end if

		//Ability is present and active
		return true;
	} //end CheckAbility(int abilityToCheck)

	/***************************************
     * Name: CheckItem
     * Checks if item is present and active
     ***************************************/
	public bool CheckItem(int itemToCheck)
	{
		//Check for item, then if active
		if (battler.Item != itemToCheck || effects[(int)LastingEffects.Embargo] == 0 || 
			GameManager.instance.CheckEffect((int)FieldEffects.MagicRoom, sideOn) || 
			CheckAbility(77))
		{
			return false;
		} //end if

		//Item is present and active
		return true;
	} //end CheckAbility(int abilityToCheck)
	#endregion

	#region Properties
	/***************************************
     * Name: SideOn
     ***************************************/
	public int SideOn
	{
		get
		{
			return sideOn;
		} //end get
		set
		{
			sideOn = value;
		} //end set
	} //end SideOn

	/***************************************
     * Name: IsMega
     ***************************************/
	public bool IsMega
	{
		get
		{
			return isMega;
		} //end get
		set
		{
			isMega = value;
		} //end set
	} //end IsMega
	#endregion
} //end PokemonBattler class
