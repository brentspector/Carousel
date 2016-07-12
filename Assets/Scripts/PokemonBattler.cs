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
	int status;				//Standard status
	int ability;			//What ability does this pokemon currently have
	int turnCount;			//How many turns have passed
	int lastHPLost;			//The last mount of HP this pokemon lost
	int lastMoveUsed;		//The last attack this pokemon used
	int sideOn;				//What side the pokemon is on
	int item;				//What is the currently held item
	int gender;				//The displayed gender of the pokemon (for  Illusion)
	int currentLevel;		//The current level for the pokemon (for Illusion)
	string nickname;		//The current name of the pokemon (for Illusion)
	List<int> moves;		//What moves does this pokemon currently have
	List<int> ppRemaining;	//How many uses does this move have remaining
	List<int> ppMax;		//The amount of uses each move has total
	List<int> types;		//What types does this pokemon have
	List<int> stages;		//Buffs and debuffs to the stats of the pokemon
	List<int> effects;		//Attack effects this pokemon is under
	bool hasSubstitute;		//Is there a substitute active for this pokemon
	bool isMega;			//Is this pokemon mega-evolved
	bool justEntered;		//Has this pokemon just entered battle
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
		status = battler.Status;
		ability = battler.Ability;
		gender = battler.Gender;
		currentLevel = battler.CurrentLevel;
		turnCount = 0;
		lastHPLost = 0;
		lastMoveUsed = -1;
		nickname = battler.Nickname;
		moves = new List<int>();
		ppRemaining = new List<int>();
		ppMax = new List<int>();
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
				ppRemaining.Add(battler.GetMovePP(i));
				ppMax.Add(battler.GetMovePPMax(i));
			} //end if
			else
			{
				moves.Add(-1);
				ppRemaining.Add(0);
				ppMax.Add(0);
			} //end else
		} //end for

		//Set types
		types.Add(battler.Type1);
		if (battler.Type2 != -1)
		{
			types.Add(battler.Type2);
		} //end if

		//Set stages
		for (int i = 0; i < 8; i++)
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
		if (CheckAbility(99) && battler != null)
		{
			battler.Status = (int)Status.HEALTHY;
			battler.StatusCount = 0;
		} //end if

		//Check for Regenerator
		if (CheckAbility(124) && battler != null)
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
		status = battler.Status;
		ability = battler.Ability;
		gender = battler.Gender;
		currentLevel = battler.CurrentLevel;
		nickname = battler.Nickname;

		//Initialize moves
		for (int i = 0; i < 4; i++)
		{
			if (i < battler.GetMoveCount())
			{
				moves[i] = battler.GetMove(i);
				ppRemaining[i] = battler.GetMovePP(i);
				ppMax[i] = battler.GetMovePPMax(i);
			} //end if
			else
			{
				moves[i] = -1;
				ppRemaining[i] = 0;
				ppMax[i] = 0;
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
			for (int i = 0; i < 8; i++)
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
		isMega = false;
		justEntered = true;
	} //end SwitchInPokemon(bool retainStats = false)

	/***************************************
     * Name: UpdateActiveBattler
     * Mimics changes from team member to
     * active battler
     ***************************************/
	public void UpdateActiveBattler()
	{
		if (battler == null)
		{
			return;
		} //end if
		currentHP = battler.CurrentHP;
		totalHP = battler.TotalHP;
		attack = battler.Attack;
		defense = battler.Defense;
		speed = battler.Speed;
		specialA = battler.SpecialA;
		specialD = battler.SpecialD;
		status = battler.Status;
		currentLevel = battler.CurrentLevel;
		CalculateStats();

		//Initialize moves
		for (int i = 0; i < 4; i++)
		{
			if (i < battler.GetMoveCount())
			{
				moves[i] = battler.GetMove(i);
				ppRemaining[i] = battler.GetMovePP(i);
				ppMax[i] = battler.GetMovePPMax(i);
			} //end if
			else
			{
				moves[i] = -1;
				ppRemaining[i] = 0;
				ppMax[i] = 0;
			} //end else
		} //end for
	} //end UpdateActiveBattler

	/***************************************
     * Name: ApplyEffect
     * Changes effect to given
     ***************************************/
	public void ApplyEffect(int effect, int toSet)
	{
		effects[effect] = toSet;
	} //end ApplyEffect(int effect, int toSet)

	/***************************************
     * Name: CheckDefenderTyping
     * Determine the impact a move will have
     * this target pokemon
     ***************************************/
	public float CheckDefenderTyping(int moveType, PokemonBattler moveUser)
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

		//Loop through and get the modifiers for each of the battler's types
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

				//Switch based on move type
				switch(moveType)
				{
					//Normal attack
					case 0: 
						//Check for Scrappy/Foresight
						if (types[i] == 7 && (moveUser.CheckAbility(134) || 
							effects[(int)LastingEffects.Foresight] > 0))
						{
							mods[i] = 1;										
						} //end if
						break;
					//Fighting attack
					case 1:
						//Check for Scrappy/Foresight
						if (types[i] == 7 && (moveUser.CheckAbility(134) || 
							effects[(int)LastingEffects.Foresight] > 0))
						{
							mods[i] = 1;										
						} //end if
						break;
					//Ground attack
					case 4:
						//Check for Ingrain/Smack Down/Gravity
						if(types[i] == 2 && (effects[(int)LastingEffects.Ingrain] > 0 ||
							effects[(int)LastingEffects.SmackDown] > 0 || GameManager.instance.CheckEffect((int)FieldEffects.Gravity, 
							sideOn)))
						{
							mods[i] = 1;
						} //end if
						break;
					//Rock attack
					case 5:
						//Check for Strong Winds
						if (GameManager.instance.CheckEffect((int)FieldEffects.StrongWinds, sideOn) && types[i] == 2)
						{
							mods[i] = 1;
						} //end if
						break;
					//Electric attack
					case 13:
						//Check for Strong Winds
						if (GameManager.instance.CheckEffect((int)FieldEffects.StrongWinds, sideOn) && types[i] == 2)
						{
							mods[i] = 1;
						} //end if
						break;
					//Psychic attack
					case 14:
						//Check for Miracle Eye
						if(types[i] == 17 && effects[(int)LastingEffects.MiracleEye] > 0)
						{
							mods[i] = 1;
						} //end if
						break;
					//Ice attack
					case 15:
						//Check for Strong Winds
						if (GameManager.instance.CheckEffect((int)FieldEffects.StrongWinds, sideOn) && types[i] == 2)
						{
							mods[i] = 1;
						} //end if

						//Check for Freeze Dry
						else if (GameManager.instance.CheckMoveUsed() == "Freeze Dry" && types[i] == 11)
						{
							mods[i] = 2;
						} //end else if
						break;
				} //end switch
			} //end else

			//If mod still is immune, return
			if (mods[i] == 0)
			{
				return 0;
			} //end if
		} //end for

		//Evaluate mod
		float finalMod = 1f;
		for (int i = 0; i < mods.Count; i++)
		{
			if (mods[i] > 0)
			{
				finalMod *= (float)mods[i];
			} //end if
			else
			{
				finalMod *= 0.5f;
			} //end else
		} //end for

		//Return final mod
		return finalMod;
	} //end CheckDefenderTyping(int moveType, PokemonBattler moveUser)

	/***************************************
     * Name: CheckAccuracy
     * Checks if the move hits this target
     * or not
     ***************************************/
	public bool CheckAccuracy(int moveAccuracy, int attackerAccuracyStage, int moveType, int moveCategory, bool ignoreEvasion)
	{
		//If move is guaranteed to hit
		if (moveAccuracy == 0 || ((GameManager.instance.CheckMoveUsed() == "Flying Press" || GameManager.instance.CheckMoveUsed() == 
			"Stomp" || GameManager.instance.CheckMoveUsed() == "Dragon Rush") && effects[(int)LastingEffects.Minimize] > 0))
		{
			return true;
		} //end if

		//Store the evasion 
		int evasionValue = 0;
		if (!ignoreEvasion || effects[(int)LastingEffects.Foresight] > 0 || effects[(int)LastingEffects.MiracleEye] > 0)
		{
			evasionValue = stages[7];
			if (GameManager.instance.CheckEffect((int)FieldEffects.Gravity, sideOn))
			{
				evasionValue -= 2;
			} //end if
		} //end if
		evasionValue = evasionValue > -1 ? ((evasionValue + 3) * 100) / 3 : 300 / (3 - evasionValue);

		//Check for Tangled Feet
		if (CheckAbility(167) && effects[(int)LastingEffects.Confusion] > 0)
		{
			evasionValue = (evasionValue * 12) / 10;
		} //end if

		//Check for Sand Veil
		else if (CheckAbility(132) && GameManager.instance.CheckEffect((int)FieldEffects.Sand, sideOn))
		{
			evasionValue = (evasionValue * 12) / 10;
		} //end else if

		//Check for Snow Cloak
		else if (CheckAbility(145) && GameManager.instance.CheckEffect((int)FieldEffects.Hail, sideOn))
		{
			evasionValue = (evasionValue * 12) / 10;
		} //end else if

		//Check for BrightPowder or Lax Incense
		if (CheckItem(34) || CheckItem(144))
		{
			evasionValue = (evasionValue * 11) / 10;
		} //end if

		//Check for Unaware
		int attackerAccuracy = CheckAbility(179) ? 0 : attackerAccuracyStage;

		//Store the accuracy ammount
		attackerAccuracy = attackerAccuracy > -1 ? ((attackerAccuracy + 3) * 100) / 3 : 300 / (3 - attackerAccuracy);

		//Check for Wonder Skin
		if (CheckAbility(190) && moveCategory == (int)Categories.Status && moveAccuracy > 50)
		{
			moveAccuracy = 50;
		} //end if

		//Calculate and return if move hits
		return GameManager.instance.RandomInt(0, 100) < (moveAccuracy*attackerAccuracy/evasionValue);
	} //end CheckAccuracy(int moveAccuracy, int attackerAccuracyStage, int moveType, bool ignoreEvasion)

	/***************************************
     * Name: CheckCritical
     * Checks if this battler scores a critical hit
     ***************************************/
	public bool CheckCritical(int critStage)
	{
		//Track critical hit stage
		int cHit = critStage;

		//Increase based on Focus Energy
		cHit += effects[(int)LastingEffects.FocusEnergy];

		//Check for Super Luck
		cHit += CheckAbility(161) ? 1 : 0;

		//Check for Farfetch'd holding Stick
		if (CheckItem(277) && battler.NatSpecies == 83)
		{
			cHit += 2;
		} //end if

		//Check for Chansey holding Lucky Punch
		else if(CheckItem(157) && battler.NatSpecies == 113)
		{
			cHit += 2;
		} //end else if

		//Check Razor Claw and Scope Lens
		cHit += CheckItem(227) || CheckItem(252) ? 1 : 0;

		//Calculate if critical occurs
		cHit = ExtensionMethods.CapAtInt(cHit, 3);
		switch (cHit)
		{
			case 0:
				int store = GameManager.instance.RandomInt(0, 16);
				return store == 0;
			case 1:
				store = GameManager.instance.RandomInt(0, 8);
				return store == 0;
			case 2:
				store = GameManager.instance.RandomInt(0, 2);
				return store == 0;
			case 3:
				return true;
			default:
				return false;
		} //end switch
	} //end CheckCritical(int critStage)

	/***************************************
     * Name: CheckSTAB
     * Check if the move used gets a same type
     * attack bonus
     ***************************************/
	public bool CheckSTAB(int move)
	{
		return types.Contains(DataContents.GetMoveIcon(move)) ? true : false;
	} //end CheckSTAB(int move)

	/***************************************
     * Name: ProcessAttackerAttackEffect
     * Attempt to apply the effect of a move
     * used by this pokemon to this pokemon
     ***************************************/
	public void ProcessAttackerAttackEffect(int moveNumber)
	{
		//Protect
		if (moveNumber == 413)
		{
			effects[(int)LastingEffects.Protect] = 1;
		} //end if
	} //end ProcessAttackerAttackEffect(int moveNumber)

	/***************************************
     * Name: ProcessDefenderAttackEffect
     * Attempt to apply the effect of a move
     * used against this pokemon to this pokemon
     ***************************************/
	public void ProcessDefenderAttackEffect(int moveNumber)
	{
		//Bubble
		if (moveNumber == 57)
		{
			//10% chance to lower defender's speed 1 stage
			if (GameManager.instance.RandomInt(0, 9) == 0)
			{
				stages[3] = ExtensionMethods.BindToInt(stages[3]-1, -6);
			} //end if
		} //end if
	} //end ProcessDefenderAttackEffect(int moveNumber)

	/***************************************
     * Name: EndOfRoundReset
     * Resets effects at end of round
     ***************************************/
	public void EndOfRoundReset()
	{
		effects[(int)LastingEffects.Protect] = 0;
	} //end EndOfRoundReset

	/***************************************
     * Name: RestoreHP
     * Attempts to restore HP
     ***************************************/
	public void RestoreHP(int amount)
	{
		if (effects[(int)LastingEffects.HealBlock] == 0)
		{
			battler.CurrentHP += amount;
			currentHP = battler.CurrentHP;
		} //end if
	} //end RestoreHP(int amount)

	/***************************************
     * Name: RemoveHP
     * Attempts to remove HP
     ***************************************/
	public void RemoveHP(int amount)
	{
		battler.CurrentHP -= amount;
		currentHP = battler.CurrentHP;
	} //end RemoveHP(int amount)

	/***************************************
     * Name: GetStage
     * Gets the requested stage
     ***************************************/
	public int GetStage(int stage)
	{
		return stages[stage];
	} //end GetStage(int stage)

	/***************************************
     * Name: SetStage
     * Sets the requested stage to the given
     * value, or adjusts by the amount given
     ***************************************/
	public void SetStage(int amount, int stage, bool adjust = true)
	{
		stages[stage] = adjust ? stages[stage] + amount : amount;
		stages[stage] = ExtensionMethods.CapAtInt(stages[stage], 6);
		CalculateStats();
	} //end SetStage(int amount, int stage, bool adjust = true)

	/***************************************
     * Name: CalculateStats
     * Determines the values for all stats,
     * using the base battler as a reference
     ***************************************/
	public void CalculateStats()
	{
		float value = (float)battler.Attack * (1f + (0.5f * stages[1]));
		attack = (int)value;
		value = (float)battler.Defense * (1f + (0.5f * stages[2]));
		defense = (int)value;
		value = (float)battler.Speed * (1f + (0.5f * stages[3]));
		speed = (int)value;
		value = (float)battler.SpecialA * (1f + (0.5f * stages[4]));
		specialA = (int)value;
		value = (float)battler.SpecialD * (1f + (0.5f * stages[5]));
		specialD = (int)value;
		/*Debug.Log("Base Attack: " + battler.Attack + ", Current Attack: " + attack + ", " +
		"Base Defense: " + battler.Defense + ", Current Defense: " + defense + ", " +
		"Base Speed: " + battler.Speed + ", Current Speed: " + speed + ", " +
		"Base SpecialA: " + battler.SpecialA + ", Current SpecialA: " + specialA + ", " +
		"Base SpecialD: " + battler.SpecialD + ", Current SpecialD: " + specialD);*/
	} //end CalculateStat

	/***************************************
     * Name: GetAttack
     * Returns the working value of the attack
     * of the pokemon
     ***************************************/
	public int GetAttack(bool critical)
	{
		//Return non-debuff attack for critical
		if (critical && stages[1] < 0)
		{
			return battler.Attack;
		} //end if
		else
		{
			return attack;
		} //end else
	} //end GetAttack(bool critical)

	/***************************************
     * Name: GetDefense
     * Returns the working value of the defense
     * of the pokemon
     ***************************************/
	public int GetDefense(bool critical)
	{
		//Return non-buff defense for critical
		if (critical && stages[2] > 0)
		{
			return battler.Defense;
		} //end if
		else
		{
			return defense;
		} //end else
	} //end GetDefense(bool critical)

	/***************************************
     * Name: GetSpecialAttack
     * Returns the working value of the
     * special attack of the pokemon
     ***************************************/
	public int GetSpecialAttack(bool critical)
	{
		//Return non-debuff special attack for critical
		if (critical && stages[4] < 0)
		{
			return battler.SpecialA;
		} //end if
		else
		{
			return specialA;
		} //end else
	} //end GetSpecialAttack(bool critical)

	/***************************************
     * Name: GetSpecialDefense
     * Returns the working value of the 
     * special defense of the pokemon
     ***************************************/
	public int GetSpecialDefense(bool critical)
	{
		//Return non-buff special defense for critical
		if (critical && stages[5] > 0)
		{
			return battler.SpecialD;
		} //end if
		else
		{
			return specialD;
		} //end else
	} //end GetSpecialDefense(bool critical)

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
     * Name: GetAbility
     * Returns ability value
     ***************************************/
	public int GetAbility()
	{
		return ability;
	} //end GetAbility

	/***************************************
     * Name: GetAbilityName
     * Returns ability name
     ***************************************/
	public string GetAbilityName()
	{
		return DataContents.ExecuteSQL<string> ("SELECT gameName FROM Abilities WHERE rowid=" + ability);
	} //end GetAbilityName

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
	} //end CheckItem(int itemToCheck)

	/***************************************
     * Name: CheckEffect
     * Checks if effect is active
     ***************************************/
	public bool CheckEffect(int effectToCheck)
	{
		//Check if effect is disabled
		if (effects[effectToCheck] < 1)
		{
			return false;
		} //end if

		//Effect is active
		return true;
	} //end CheckEffect(int effectToCheck)

	/***************************************
     * Name: CheckType
     * Checks if type exists
     ***************************************/
	public bool CheckType(int type)
	{
		return types.Contains(type) ? true : false;
	} //end CheckType(int type)

	/***************************************
     * Name: FaintPokemon
     * Sets values to nothing when battler 
     * has fainted
     ***************************************/
	public void FaintPokemon()
	{
		battler.Status = (int)Status.FAINT;
		battler.StatusCount = 0;
		battler = null;
	} //end FaintPokemon
	#endregion

	#region Properties
	/***************************************
     * Name: BattlerPokemon
     ***************************************/
	public Pokemon BattlerPokemon
	{
		get
		{
			return battler;
		} //end get
	} //end BattlerPokemon

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
     * Name: CurrentHP
     ***************************************/
	public int CurrentHP
	{
		get
		{
			return currentHP;
		} //end get
		set
		{
			currentHP = ExtensionMethods.CapAtInt(value, totalHP);
		} //end set
	}//end Current HP

	/***************************************
     * Name: TotalHP
     ***************************************/
	public int TotalHP
	{
		get
		{
			return totalHP;
		} //end get
		set
		{
			totalHP = value;
		} //end set
	} //end TotalHP

	/***************************************
     * Name: Attack
     ***************************************/
	public int Attack
	{
		get
		{
			return attack;
		} //end get
		set
		{
			attack = value;
		} //end set
	} //end Attack

	/***************************************
     * Name: Defense
     ***************************************/
	public int Defense
	{
		get
		{
			return defense;
		} //end get
		set
		{
			defense = value;
		} //end set
	} //end Defense

	/***************************************
     * Name: SpecialA
     ***************************************/
	public int SpecialA
	{
		get
		{
			return specialA;
		} //end get
		set
		{
			specialA = value;
		} //end set
	} //end SpecialA

	/***************************************
     * Name: SpecialD
     ***************************************/
	public int SpecialD
	{
		get
		{
			return specialD;
		} //end get
		set
		{
			specialD = value;
		} //end set
	} //end SpecialD

	/***************************************
     * Name: Speed
     ***************************************/
	public int Speed
	{
		get
		{
			return speed;
		} //end get
		set
		{
			speed = value;
		} //end set
	} //end Speed

	/***************************************
     * Name: Item
     ***************************************/
	public int Item
	{
		get
		{
			return item;
		} //end get
		set
		{
			item = value;
		} //end set
	} //end Item

	/***************************************
     * Name: BattlerStatus
     ***************************************/
	public int BattlerStatus
	{
		get
		{
			return status;
		} //end get
		set
		{
			status = value;
		} //end set
	} //end BattlerStatus

	/***************************************
     * Name: Nickname
     ***************************************/
	public string Nickname
	{
		get
		{
			return nickname;
		} //end get
		set
		{
			nickname = value;
		} //end set
	} //end Nickname

	/***************************************
     * Name: Gender
     ***************************************/
	public int Gender
	{
		get
		{
			return gender;
		} //end get
		set
		{
			gender = value;
		} //end set
	} //end Gender

	/***************************************
     * Name: CurrentLevel
     ***************************************/
	public int CurrentLevel
	{
		get
		{
			return currentLevel;
		} //end get
		set
		{
			currentLevel = ExtensionMethods.WithinIntRange(value, 1, 100);
		} //end set
	} //end CurrentLevel

	/***************************************
     * Name: GetMove
     ***************************************/
	public int GetMove(int index)
	{
		return moves [index];
	} //end GetMove(int index)

	/***************************************
     * Name: SetMove
     ***************************************/
	public void SetMove(int index, int move)
	{
		moves [index] = move;
	} //end SetMove(int index, int move)

	/***************************************
     * Name: GetMovePP
     ***************************************/
	public int GetMovePP(int index)
	{
		return ppRemaining [index];
	} //end GetMovePP(int index)

	/***************************************
     * Name: SetMovePP
     ***************************************/
	public void SetMovePP(int index, int amount)
	{
		ppRemaining [index] = amount;

		//Update battler if possible
		if (moves[index] == battler.GetMove(index))
		{
			battler.SetMovePP(index, battler.GetMovePP(index) - 1);
		} //end if
	} //end SetMovePP(int index, int amount)

	/***************************************
     * Name: GetMovePPMax
     ***************************************/
	public int GetMovePPMax(int index)
	{
		return ppMax [index];
	} //end GetMovePPMax(int index)

	/***************************************
     * Name: SetMovePPMax
     ***************************************/
	public void SetMovePPMax(int index, int value)
	{
		ppMax [index] = value;
	} //end SetMovePPMax(int index, int value)

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

	/***************************************
     * Name: JustEntered
     ***************************************/
	public bool JustEntered
	{
		get
		{
			return justEntered;
		} //end get
		set
		{
			justEntered = value;
		} //end set
	} //end JustEntered
	#endregion
} //end PokemonBattler class
