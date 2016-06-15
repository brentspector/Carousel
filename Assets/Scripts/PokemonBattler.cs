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
	} //end InitializeEffects

	/***************************************
     * Name: SwitchInPokemon
     * Switches out active pokemon for 
     * parameter
     ***************************************/
	public void SwitchInPokemon(Pokemon toSwitch, bool retainStats = false)
	{
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

			if (effects[(int)LastingEffects.PowerTrick] == 1)
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
	} //end UseItemOnPokemon(Pokemon toTarget)

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
		if (CheckAbility(164))
		{

		} //end if
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
		if (ability != abilityToCheck || effects[(int)LastingEffects.GastroAcid] == 1)
		{
			return false;
		} //end if

		//Ability is present and active
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
