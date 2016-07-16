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
	int lastHPLost;			//The last amount of HP this pokemon lost
	int lastDamageDealt;	//The last amount of damage this pokemon did
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
	bool partyFainted;		//Did this pokemon replace a fainted party member
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
		partyFainted = false;

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

		//TwoTurnAttack is move based
		effects[(int)LastingEffects.TwoTurnAttack] = -1;
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

		//Previous fainted
		if (battler == null)
		{
			partyFainted = true;
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
		turnCount = 0;

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
			effects[(int)LastingEffects.MultiTurn] = 0;
			effects[(int)LastingEffects.MultiTurnUser] = 0;
			effects[(int)LastingEffects.MultiTurnAttack] = 0;
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
		effects[(int)LastingEffects.Attract] = 0;
		effects[(int)LastingEffects.Unburden] = 0;
		effects[(int)LastingEffects.Torment] = 0;
		effects[(int)LastingEffects.TwoTurnAttack] = -1;
		lastHPLost = 0;
		lastMoveUsed = -1;
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
     * Name: GetEffectValue
     * Returns value at effect location
     ***************************************/
	public int GetEffectValue(int effectToCheck)
	{
		return effects[effectToCheck];
	} //end GetEffectValue(int effectToCheck)

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
		//Absorb
		if (moveNumber == 1)
		{
			//Recover half the damage dealt
			int restore = lastDamageDealt / 2;
			if (currentHP + restore > totalHP)
			{
				restore = totalHP - currentHP;
			} //end if
			currentHP += restore;
			GameManager.instance.WriteBattleMessage(string.Format("{0} recovered {1} HP!", nickname, restore));
		} //end if

		//Brave Bird
		else if (moveNumber == 54)
		{
			//25% recoil
			int damage = lastDamageDealt / 4;
			if (currentHP - damage < 0)
			{
				damage = currentHP;
			} //end if
			currentHP -= damage;
			GameManager.instance.WriteBattleMessage(string.Format("{0} lost {1} HP from recoil!", nickname, damage));
		} //end else if

		//Calm Mind
		else if (moveNumber == 65)
		{
			//Increase special attack 1 stage
			if (stages[4] < 6)
			{
				//Increase special defense 1 stage
				if (stages[5] < 6)
				{
					SetStage(1, 4);
					SetStage(1, 5);
					GameManager.instance.WriteBattleMessage(nickname + "'s Special Attack and Special Defense rose!");
				} //end if
				else
				{
					SetStage(1, 4);
					GameManager.instance.WriteBattleMessage(nickname + "'s Special Attack rose!");
				} //end else
			} //end if

			//Increase special defense 1 stage
			else if (stages[5] < 6)
			{
				SetStage(1, 5);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Defense rose!");
			} //end else if
			else
			{
				GameManager.instance.WriteBattleMessage("...but nothing happened.");
			} //end else
		} //end else if

		//Cotton Guard
		else if (moveNumber == 87)
		{
			//Increase defense 3 stages
			if (stages[2] < 6)
			{
				SetStage(3, 2);
				GameManager.instance.WriteBattleMessage(nickname + "'s Defense drastically rose!");
			} //end if
		} //end else if

		//Curse
		else if (moveNumber == 98)
		{
			//Increase attack 1 stage
			if (stages[1] < 6)
			{
				//Increase defense 1 stage
				if (stages[2] < 6)
				{
					//Decrease speed 1 stage
					if (stages[3] > -6)
					{
						SetStage(1, 1);
						SetStage(1, 2);
						SetStage(-1, 3);
						GameManager.instance.WriteBattleMessage(nickname + "'s Attack and Defense rose, but Speed fell!");
					} //end if
					else
					{
						SetStage(1, 1);
						SetStage(1, 2);
						GameManager.instance.WriteBattleMessage(nickname + "'s Attack and Defense rose!");
					} //end else
				} //end if

				//Decrease speed 1 stage
				else if (stages[3] > -6)
				{
					SetStage(1, 1);
					SetStage(-1, 3);
					GameManager.instance.WriteBattleMessage(nickname + "'s Attack rose, but Speed fell!");
				} //end else if

				else
				{
					SetStage(1, 1);
					GameManager.instance.WriteBattleMessage(nickname + "'s Attack rose!");
				} //end else
			} //end if

			//Increase defense 1 stage
			else if (stages[2] < 6)
			{
				//Decrease speed 1 stage
				if (stages[3] > -6)
				{
					SetStage(1, 2);
					SetStage(-1, 3);
					GameManager.instance.WriteBattleMessage(nickname + "'s Defense rose, but Speed fell!");
				} //end else if
				else
				{
					SetStage(1, 2);
					GameManager.instance.WriteBattleMessage(nickname + "'s Defense rose!");
				} //end else
			} //end else if

			//Decrease speed 1 stage
			else if (stages[3] > -6)
			{
				SetStage(-1, 3);
				GameManager.instance.WriteBattleMessage(nickname + "'s Speed fell!");
			} //end else if

			else
			{
				GameManager.instance.WriteBattleMessage("...but nothing happened.");
			} //end else
		} //end else if

		//Dragon Dance
		else if (moveNumber == 125)
		{
			//Increase attack 1 stage
			if (stages[1] < 6)
			{
				//Increase speed 1 stage
				if (stages[3] < 6)
				{
					SetStage(1, 1);
					SetStage(1, 3);
					GameManager.instance.WriteBattleMessage(nickname + "'s Attack and Speed rose!");
				} //end if
				else
				{
					SetStage(1, 1);
					GameManager.instance.WriteBattleMessage(nickname + "'s Attack rose!");
				} //end else
			} //end if

			//Increase speed 1 stage
			else if (stages[3] < 6)
			{
				SetStage(1, 3);
				GameManager.instance.WriteBattleMessage(nickname + "'s Speed rose!");
			} //end else if
			else
			{
				GameManager.instance.WriteBattleMessage("...but nothing happened.");
			} //end else
		} //end else if

		//Flare Blitz
		else if (moveNumber == 180)
		{
			//25% recoil
			int damage = lastDamageDealt / 4;
			if (currentHP - damage < 0)
			{
				damage = currentHP;
			} //end if
			currentHP -= damage;
			GameManager.instance.WriteBattleMessage(string.Format("{0} lost {1} HP from recoil!", nickname, damage));
		} //end else if

		//Harden
		else if (moveNumber == 230)
		{
			//Increase defense 2 stages
			if (stages[2] < 6)
			{
				SetStage(2, 2);
				GameManager.instance.WriteBattleMessage(nickname + "'s Defense sharply rose!");
			} //end if
			else
			{
				GameManager.instance.WriteBattleMessage("...but nothing happened.");
			} //end else
		} //end else if

		//Head Smash
		else if (moveNumber == 233)
		{
			//25% recoil
			int damage = lastDamageDealt / 4;
			if (currentHP - damage < 0)
			{
				damage = currentHP;
			} //end if
			currentHP -= damage;
			GameManager.instance.WriteBattleMessage(string.Format("{0} lost {1} HP from recoil!", nickname, damage));
		} //end else if

		//Hone Claws
		else if (moveNumber == 267)
		{
			//Increase attack 1 stage
			if (stages[1] < 6)
			{
				//Increase accuracy 1 stage
				if (stages[6] < 6)
				{
					SetStage(1, 1);
					SetStage(1, 6);
					GameManager.instance.WriteBattleMessage(nickname + "'s Attack and accuracy rose!");
				} //end if
				else
				{
					SetStage(1, 1);
					GameManager.instance.WriteBattleMessage(nickname + "'s Attack rose!");
				} //end else
			} //end if

			//Increase accuracy 1 stage
			else if (stages[6] < 6)
			{
				SetStage(1, 6);
				GameManager.instance.WriteBattleMessage(nickname + "'s accuracy rose!");
			} //end else if
			else
			{
				GameManager.instance.WriteBattleMessage("...but nothing happened.");
			} //end else
		} //end else if

		//Iron Defense
		else if (moveNumber == 295)
		{
			//Increase defense 2 stages
			if (stages[2] < 6)
			{
				SetStage(2, 2);
				GameManager.instance.WriteBattleMessage(nickname + "'s Defense sharply rose!");
			} //end if
		} //end else if

		//King's Shield
		else if (moveNumber == 302)
		{
			//Switch form
			if (battler.FormNumber != 0)
			{
				ApplyFormCalculateStats();
			} //end if

			//Determine if it protects itself
			if (GameManager.instance.RandomInt(0, 10) < (10 / (1 + effects[(int)LastingEffects.ProtectRate])))
			{
				effects[(int)LastingEffects.KingsShield] = 1;
				effects[(int)LastingEffects.ProtectRate]++;
				GameManager.instance.WriteBattleMessage(nickname + " is protecting itself!");
			} //end if
			else
			{
				effects[(int)LastingEffects.Protect] = 0;
				effects[(int)LastingEffects.ProtectRate] = 0;
				GameManager.instance.WriteBattleMessage("But it failed!");
			} //end else
		} //end else if

		//Power-Up Punch
		else if (moveNumber == 410)
		{
			//Increase attack 1 stage
			if (stages[1] < 6)
			{
				SetStage(1, 1);
				GameManager.instance.WriteBattleMessage(nickname + "'s Attack rose!");
			} //end if
		} //end else if

		//Protect
		else if (moveNumber == 413)
		{
			//Determine if it protects itself
			if (GameManager.instance.RandomInt(0, 10) < (10 / (1 + effects[(int)LastingEffects.ProtectRate])))
			{
				effects[(int)LastingEffects.Protect] = 1;
				effects[(int)LastingEffects.ProtectRate]++;
				GameManager.instance.WriteBattleMessage(nickname + " is protecting itself!");
			} //end if
			else
			{
				effects[(int)LastingEffects.Protect] = 0;
				effects[(int)LastingEffects.ProtectRate] = 0;
				GameManager.instance.WriteBattleMessage("But it failed!");
			} //end else
		} //end else if

		//Swords Dance
		else if (moveNumber == 558)
		{
			//Increase attack 2 stages
			if (stages[1] < 6)
			{
				SetStage(2, 1);
				GameManager.instance.WriteBattleMessage(nickname + "'s Attack sharply rose!");
			} //end if
		} //end else if

		//Take Down
		else if (moveNumber == 566)
		{
			//25% recoil
			int damage = lastDamageDealt / 4;
			if (currentHP - damage < 0)
			{
				damage = currentHP;
			} //end if
			currentHP -= damage;
			GameManager.instance.WriteBattleMessage(string.Format("{0} lost {1} HP from recoil!", nickname, damage));
		} //end else if

		//Wild Charge
		else if (moveNumber == 619)
		{
			//25% recoil
			int damage = lastDamageDealt / 4;
			if (currentHP - damage < 0)
			{
				damage = currentHP;
			} //end if
			currentHP -= damage;
			GameManager.instance.WriteBattleMessage(string.Format("{0} lost {1} HP from recoil!", nickname, damage));
		} //end else if
	} //end ProcessAttackerAttackEffect(int moveNumber)

	/***************************************
     * Name: ProcessDefenderAttackEffect
     * Attempt to apply the effect of a move
     * used against this pokemon to this pokemon
     ***************************************/
	public void ProcessDefenderAttackEffect(int moveNumber)
	{		
		//Acid
		if (moveNumber == 2 && !CheckAbility(140) && !CheckAbility(188))
		{
			//10% chance to lower defender's defense 1 stage
			if (GameManager.instance.RandomInt(0, 10) == 1 && stages[2] > -6)
			{
				SetStage(-1, 2);
				GameManager.instance.WriteBattleMessage(nickname + "'s Defense fell!");
			} //end if
		} //end if

		//Air Slash
		else if (moveNumber == 12 && !CheckAbility(140))
		{
			//30% chance to flinch
			if (GameManager.instance.RandomInt(0, 10) < 3 && effects[(int)LastingEffects.Flinch] == 0)
			{
				effects[(int)LastingEffects.Flinch] = 1;
			} //end if
		} //end else if

		//Aurora Beam
		else if (moveNumber == 28 && !CheckAbility(140) && !CheckAbility(63) && !CheckAbility(188))
		{
			//10% chance to lower defender's attack 1 stage
			if (GameManager.instance.RandomInt(0, 10) == 1 && stages[1] > -6)
			{
				SetStage(-1, 1);
				GameManager.instance.WriteBattleMessage(nickname + "'s Attack fell!");
			} //end if
		} //end else if

		//Bite
		else if (moveNumber == 41 && !CheckAbility(140))
		{
			//30% chance to flinch
			if (GameManager.instance.RandomInt(0, 10) < 3 && effects[(int)LastingEffects.Flinch] == 0)
			{
				effects[(int)LastingEffects.Flinch] = 1;
			} //end if
		} //end else if

		//Blizzard
		else if (moveNumber == 44 && status == (int)Status.HEALTHY && !types.Contains((int)Types.ICE) && !CheckAbility(140))
		{
			//10% chance to freeze
			if (GameManager.instance.RandomInt(0, 10) == 1)
			{
				status = (int)Status.FREEZE;
				GameManager.instance.WriteBattleMessage(nickname + " became frozen!");
			} //end if
		} //end else  if

		//Bubble
		else if (moveNumber == 57 && !CheckAbility(140) && !CheckAbility(188))
		{
			//10% chance to lower defender's speed 1 stage
			if (GameManager.instance.RandomInt(0, 10) == 1 && stages[3] > -6)
			{
				SetStage(-1, 3);
				GameManager.instance.WriteBattleMessage(nickname + "'s Speed fell!");
			} //end if
		} //end else if

		//Bulldoze
		else if (moveNumber == 62 && !CheckAbility(140) && !CheckAbility(188))
		{
			//100% chance to lower defender's speed 1 stage
			if (stages[3] > -6)
			{
				SetStage(-1, 3);
				GameManager.instance.WriteBattleMessage(nickname + "'s Speed fell!");
			} //end if
		} //end else if

		//Charm
		else if (moveNumber == 70)
		{
			//Hyper Cutter
			if (CheckAbility(63))
			{
				GameManager.instance.WriteBattleMessage(nickname + "'s Hyper Cutter prevented Charm from working!");
			} //end if

			//White Smoke
			else if (CheckAbility(188))
			{
				GameManager.instance.WriteBattleMessage(nickname + "'s White Smoke prevented Charm from working!");
			} //end else if

			//100% chance to lower defender's Attack 2 stages
			else if (stages[1] > -6)
			{
				SetStage(-2, 1);
				GameManager.instance.WriteBattleMessage(nickname + "'s Attack sharply fell!");
			} //end else if
		} //end else if

		//Confide
		else if (moveNumber == 79)
		{
			//100% chance to lower defender's special attack by 1 stage
			if (stages[4] > -6)
			{
				SetStage(-1, 4);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Attack fell!");
			} //end if
		} //end else if

		//Confuse Ray
		else if (moveNumber == 80)
		{
			if (effects[(int)LastingEffects.Confusion] == 0)
			{
				effects[(int)LastingEffects.Confusion] = GameManager.instance.RandomInt(2, 6);
				GameManager.instance.WriteBattleMessage(nickname + " became confused!");
			} //end if
			else
			{
				GameManager.instance.WriteBattleMessage("...but it failed!");
			} //end else
		} //end else if

		//Crunch
		else if (moveNumber == 95)
		{
			//20% chance to lower defender's Defense 1 stage
			if (GameManager.instance.RandomInt(0, 10) < 2 && stages[2] > -6 && !CheckAbility(140) && !CheckAbility(188))
			{
				SetStage(-1, 2);
				GameManager.instance.WriteBattleMessage(nickname + "'s Defense fell!");
			} //end if
		} //end else if

		//Curse
		else if (moveNumber == 98)
		{
			if (effects[(int)LastingEffects.Curse] == 0)
			{
				effects[(int)LastingEffects.Curse] = 1;
				GameManager.instance.WriteBattleMessage(nickname + " was cursed!");
			} //end if
			else
			{
				GameManager.instance.WriteBattleMessage(nickname + " is already under a Curse!");
			} //end else
		} //end else if

		//Dark Pulse
		else if (moveNumber == 100 && !CheckAbility(140))
		{
			//20% chance to flinch
			if (GameManager.instance.RandomInt(0, 10) < 2 && effects[(int)LastingEffects.Flinch] == 0)
			{
				effects[(int)LastingEffects.Flinch] = 1;
			} //end if
		} //end else if

		//Discharge
		else if (moveNumber == 112 && status == (int)Status.HEALTHY && !types.Contains((int)Types.ELECTRIC) && !CheckAbility(140)
		         && !CheckAbility(82))
		{
			//30% chance to paralyze
			if (GameManager.instance.RandomInt(0, 10) < 3)
			{
				status = (int)Status.PARALYZE;
				GameManager.instance.WriteBattleMessage(nickname + " is paralyzed! It may be unable to move.");
			} //end if
		} //end else  if

		//Earth Power
		else if (moveNumber == 137 && !CheckAbility(140) && !CheckAbility(188))
		{
			//10% chance to lower defender's special defense by 1 stage
			if (GameManager.instance.RandomInt(0, 10) == 1 && stages[5] > -6)
			{
				SetStage(-1, 5);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Defense fell!");
			} //end if
		} //end else if

		//Energy Ball
		else if (moveNumber == 151 && !CheckAbility(140) && !CheckAbility(188))
		{
			//10% chance to lower defender's special defense by 1 stage
			if (GameManager.instance.RandomInt(0, 10) == 1 && stages[5] > -6)
			{
				SetStage(-1, 5);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Defense fell!");
			} //end if
		} //end else if

		//Fake Out
		else if (moveNumber == 160)
		{
			//100% chance to flinch
			effects[(int)LastingEffects.Flinch] = 1;
		} //end else if

		//Fire Blast
		else if (moveNumber == 169 && status == (int)Status.HEALTHY && !types.Contains((int)Types.FIRE) && !CheckAbility(140))
		{
			//10% chance to burn
			if (GameManager.instance.RandomInt(0, 10) == 1)
			{
				status = (int)Status.BURN;
				GameManager.instance.WriteBattleMessage(nickname + " sustained a burn!");
			} //end if
		} //end else  if

		//Flamethrower
		else if (moveNumber == 179 && status == (int)Status.HEALTHY && !types.Contains((int)Types.FIRE) && !CheckAbility(140))
		{
			//10% chance to burn
			if (GameManager.instance.RandomInt(0, 10) == 1)
			{
				status = (int)Status.BURN;
				GameManager.instance.WriteBattleMessage(nickname + " sustained a burn!");
			} //end if
		} //end else  if

		//Flame Wheel
		else if (moveNumber == 178)
		{
			//Defrost if frozen
			if (status == (int)Status.FREEZE)
			{
				status = (int)Status.HEALTHY;
				GameManager.instance.WriteBattleMessage(nickname + " was defrosted!");
			} //end if

			//10% chance to burn
			if (GameManager.instance.RandomInt(0, 10) == 1)
			{
				status = (int)Status.BURN;
				GameManager.instance.WriteBattleMessage(nickname + " sustained a burn!");
			} //end if
		} //end else if

		//Flare Blitz
		else if (moveNumber == 180)
		{
			//Defrost if frozen
			if (status == (int)Status.FREEZE)
			{
				status = (int)Status.HEALTHY;
				GameManager.instance.WriteBattleMessage(nickname + " was defrosted!");
			} //end if

			//10% chance to burn
			if (GameManager.instance.RandomInt(0, 10) == 1)
			{
				status = (int)Status.BURN;
				GameManager.instance.WriteBattleMessage(nickname + " sustained a burn!");
			} //end if
		} //end else if

		//Flash Cannon
		else if (moveNumber == 182 && !CheckAbility(140) && !CheckAbility(188))
		{
			//10% chance to lower defender's special defense by 1 stage
			if (GameManager.instance.RandomInt(0, 10) == 1 && stages[5] > -6)
			{
				SetStage(-1, 5);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Defense fell!");
			} //end if
		} //end else if

		//Focus Blast
		else if (moveNumber == 188 && !CheckAbility(140) && !CheckAbility(188))
		{
			//10% chance to lower defender's special defense by 1 stage
			if (GameManager.instance.RandomInt(0, 10) == 1 && stages[5] > -6)
			{
				SetStage(-1, 5);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Defense fell!");
			} //end if
		} //end else if

		//Gastro Acid
		else if (moveNumber == 207)
		{
			if (effects[(int)LastingEffects.GastroAcid] == 0)
			{
				effects[(int)LastingEffects.GastroAcid] = 1;
				GameManager.instance.WriteBattleMessage(nickname + "'s ability was suppressed!");
			} //end if
		} //end else if

		//Ice Beam
		else if (moveNumber == 281 && status == (int)Status.HEALTHY && !types.Contains((int)Types.ICE) && !CheckAbility(140))
		{
			//10% chance to freeze
			if (GameManager.instance.RandomInt(0, 10) == 1)
			{
				status = (int)Status.FREEZE;
				GameManager.instance.WriteBattleMessage(nickname + " became frozen!");
			} //end if
		} //end else  if

		//Ice Fang
		else if (moveNumber == 281 && !CheckAbility(140))
		{
			//10% chance to freeze
			if (GameManager.instance.RandomInt(0, 10) == 1 && status == (int)Status.HEALTHY && !types.Contains((int)Types.ICE))
			{
				status = (int)Status.FREEZE;
				GameManager.instance.WriteBattleMessage(nickname + " became frozen!");
			} //end if

			//10% chance to finch
			if (GameManager.instance.RandomInt(0, 10) == 1 && effects[(int)LastingEffects.Flinch] == 0)
			{
				effects[(int)LastingEffects.Flinch] = 1;
			} //end if
		} //end else  if

		//Infestation
		else if (moveNumber == 292)
		{
			if (effects[(int)LastingEffects.MultiTurn] == 0)
			{
				effects[(int)LastingEffects.MultiTurn] = 4 + GameManager.instance.RandomInt(0, 2);
				effects[(int)LastingEffects.MultiTurnAttack] = 292;
				effects[(int)LastingEffects.MultiTurnUser] = GameManager.instance.CheckMoveUser().BattlerPokemon.PersonalID;
				GameManager.instance.WriteBattleMessage(string.Format("{0} was trapped in an infestation by {1}!", nickname, 
					GameManager.instance.CheckMoveUser().Nickname));
			} //end if
		} //end else if

		//Iron Head
		else if (moveNumber == 296 && !CheckAbility(140))
		{
			//30% chance to flinch
			if (GameManager.instance.RandomInt(0, 10) < 3 && effects[(int)LastingEffects.Flinch] == 0)
			{
				effects[(int)LastingEffects.Flinch] = 1;
			} //end if
		} //end else if

		//Leech Seed
		else if (moveNumber == 311)
		{
			if (effects[(int)LastingEffects.LeechSeed] != 0 && !types.Contains((int)Types.GRASS))
			{
				effects[(int)LastingEffects.LeechSeed] = GameManager.instance.CheckMoveUser().SideOn;
				GameManager.instance.WriteBattleMessage(nickname + " was seeded!");
			} //end if
			else
			{
				GameManager.instance.WriteBattleMessage(nickname + " evaded the attack!");
			} //end else
		} //end else if

		//Leer
		else if (moveNumber == 312)
		{
			//White Smoke
			if (CheckAbility(188))
			{
				GameManager.instance.WriteBattleMessage(nickname + "'s White Smoke prevented Leer from working!");
			} //end if

			//100% chance to lower defender's defense 1 stage
			else if (stages[2] > -6)
			{
				SetStage(-1, 2);
				GameManager.instance.WriteBattleMessage(nickname + "'s Defense fell!");
			} //end else if
		} //end else if

		//Mirror Shot
		else if (moveNumber == 352)
		{
			//30% chance to lower defender's accuracy 1 stage
			if (GameManager.instance.RandomInt(0, 10) < 3 && stages[6] > -6 && !CheckAbility(140) && !CheckAbility(188))
			{
				SetStage(-1, 6);
				GameManager.instance.WriteBattleMessage(nickname + "'s accuracy fell!");
			} //end if
		} //end else if

		//Moonblast
		else if (moveNumber == 356)
		{
			//30% chance to lower defender's special attack by 1 stage
			if (GameManager.instance.RandomInt(0, 10) < 3 && stages[4] > -6)
			{
				SetStage(-1, 4);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Attack fell!");
			} //end if
		} //end else if

		//Muddy Water
		else if (moveNumber == 362)
		{
			//30% chance to lower defender's accuracy 1 stage
			if (GameManager.instance.RandomInt(0, 10) < 3 && stages[6] > -6 && !CheckAbility(140) && !CheckAbility(188))
			{
				SetStage(-1, 6);
				GameManager.instance.WriteBattleMessage(nickname + "'s accuracy fell!");
			} //end if
		} //end else if

		//Noble Roar
		else if (moveNumber == 373)
		{
			//Decrease attack 1 stage
			if (stages[1] > -6)
			{
				//Decrease special attack 1 stage
				if (stages[4] > -6)
				{
					SetStage(-1, 1);
					SetStage(-1, 4);
					GameManager.instance.WriteBattleMessage(nickname + "'s Attack and Special Attack fell!");
				} //end if
				else
				{
					SetStage(-1, 1);
					GameManager.instance.WriteBattleMessage(nickname + "'s Attack fell!");
				} //end else
			} //end if

			//Decrease special attack 1 stage
			else if (stages[4] > -6)
			{
				SetStage(-1, 4);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Attack fell!");
			} //end else if
			else
			{
				GameManager.instance.WriteBattleMessage("...but nothing happened.");
			} //end else
		} //end else if

		//Poison Jab
		else if (moveNumber == 398 && status == (int)Status.HEALTHY && !types.Contains((int)Types.POISON) && !CheckAbility(140))
		{
			//30% chance to poison
			if (GameManager.instance.RandomInt(0, 10) < 3)
			{
				status = (int)Status.POISON;
				GameManager.instance.WriteBattleMessage(nickname + " is poisoned!");
			} //end if
		} //end else  if

		//Psychic
		else if (moveNumber == 416 && !CheckAbility(140))
		{
			//10% chance to lower defender's Special Defense 1 stage
			if (GameManager.instance.RandomInt(0, 10) == 1 && stages[5] > -6 && !CheckAbility(140) && !CheckAbility(188))
			{
				SetStage(-1, 5);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Defense fell!");
			} //end if
		} //end else if

		//Razor Shell
		else if (moveNumber == 434)
		{
			//50% chance to lower defender's Defense 1 stage
			if (GameManager.instance.RandomInt(0, 10) < 5 && stages[2] > -6 && !CheckAbility(140) && !CheckAbility(188))
			{
				SetStage(-1, 2);
				GameManager.instance.WriteBattleMessage(nickname + "'s Defense fell!");
			} //end if
		} //end else if

		//Rock Tomb
		else if (moveNumber == 455 && !CheckAbility(140))
		{
			//100% chance to lower defender's speed 1 stage
			if (stages[3] > -6 && !CheckAbility(140) && !CheckAbility(188))
			{
				SetStage(-1, 3);
				GameManager.instance.WriteBattleMessage(nickname + "'s Speed fell!");
			} //end if
		} //end else if

		//Shadow Ball
		else if (moveNumber == 480 && !CheckAbility(140))
		{
			//20% chance to lower defender's special defense by 1 stage
			if (GameManager.instance.RandomInt(0, 10) < 2 && stages[5] > -6 && !CheckAbility(14) && !CheckAbility(188))
			{
				SetStage(-1, 5);
				GameManager.instance.WriteBattleMessage(nickname + "'s Special Defense fell!");
			} //end if
		} //end else if

		//Sludge Bomb
		else if (moveNumber == 506 && status == (int)Status.HEALTHY && !types.Contains((int)Types.POISON) && !CheckAbility(140))
		{
			//30% chance to poison
			if (GameManager.instance.RandomInt(0, 10) < 3)
			{
				status = (int)Status.POISON;
				GameManager.instance.WriteBattleMessage(nickname + " is became poisoned!");
			} //end if
		} //end else  if

		//Stomp
		else if (moveNumber == 535 && !CheckAbility(140))
		{
			//30% chance to flinch
			if (GameManager.instance.RandomInt(0, 10) < 3 && effects[(int)LastingEffects.Flinch] == 0)
			{
				effects[(int)LastingEffects.Flinch] = 1;
			} //end if
		} //end else if

		//Thunder
		else if (moveNumber == 576 && status == (int)Status.HEALTHY && !types.Contains((int)Types.ELECTRIC) && !CheckAbility(140)
		         && !CheckAbility(82))
		{
			//30% chance to paralyze
			if (GameManager.instance.RandomInt(0, 10) < 3)
			{
				status = (int)Status.PARALYZE;
				GameManager.instance.WriteBattleMessage(nickname + " is paralyzed! It may be unable to move.");
			} //end if
		} //end else  if

		//Thunderbolt
		else if (moveNumber == 581 && status == (int)Status.HEALTHY && !types.Contains((int)Types.ELECTRIC) && !CheckAbility(140)
		         && !CheckAbility(82))
		{
			//10% chance to paralyze
			if (GameManager.instance.RandomInt(0, 10) == 1)
			{
				status = (int)Status.PARALYZE;
				GameManager.instance.WriteBattleMessage(nickname + " is paralyzed! It may be unable to move.");
			} //end if
		} //end else  if

		//Torment
		else if (moveNumber == 584)
		{
			if (effects[(int)LastingEffects.Torment] == 0)
			{
				effects[(int)LastingEffects.Torment] = 1;
				GameManager.instance.WriteBattleMessage(nickname + " fell for the Torment!");
			} //end if
			else
			{
				GameManager.instance.WriteBattleMessage(nickname + " is already affected by Torment!");
			} //end
		} //end else if

		//Trick or Treat
		else if (moveNumber == 591)
		{
			if (!types.Contains((int)Types.GHOST))
			{
				types.Add((int)Types.GHOST);
				GameManager.instance.WriteBattleMessage(nickname + " was taken Trick or Treating and became part Ghost type!");
			} //end if
			else
			{
				GameManager.instance.WriteBattleMessage(nickname + " is already a Ghost type!");
			} //end else
		} //end else if

		//Water Pulse
		else if (moveNumber == 610)
		{
			//20% chance to confuse
			if (GameManager.instance.RandomInt(0, 10) < 2 && effects[(int)LastingEffects.Confusion] == 0)
			{
				effects[(int)LastingEffects.Confusion] = GameManager.instance.RandomInt(2, 6);
				GameManager.instance.WriteBattleMessage(nickname + " became confused!");
			} //end if
		} //end else if

		//Waterfall
		else if (moveNumber == 614 && !CheckAbility(140))
		{
			//20% chance to flinch
			if (GameManager.instance.RandomInt(0, 10) < 2 && effects[(int)LastingEffects.Flinch] == 0)
			{
				effects[(int)LastingEffects.Flinch] = 1;
			} //end if
		} //end else if

		//Yawn
		else if (moveNumber == 631)
		{
			if (status == (int)Status.SLEEP)
			{
				GameManager.instance.WriteBattleMessage(nickname + " is already asleep!");
			} //end if
			else if (effects[(int)LastingEffects.Yawn] > 0)
			{
				GameManager.instance.WriteBattleMessage(nickname + " is already drowsy!");
			} //end else if
			else if (status != (int)Status.HEALTHY && status != (int)Status.FAINT)
			{
				GameManager.instance.WriteBattleMessage(nickname + " is already afflicted with a status!");
			} //end else if
			else
			{
				effects[(int)LastingEffects.Yawn] = 2;
				GameManager.instance.WriteBattleMessage(nickname + " became drowsy!");
			} //end else
		} //end else if
	} //end ProcessDefenderAttackEffect(int moveNumber)

	/***************************************
     * Name: CureConfusion
     * Reduce confusion counter and decide if
     * confusion is cured
     ***************************************/
	public bool CureConfusion()
	{
		effects[(int)LastingEffects.Confusion]--;
		return effects[(int)LastingEffects.Confusion] == 0 ? true : false;
	} //end CureConfusion

	/***************************************
     * Name: ProcessConfusion
     * Decides if an attack goes through 
     * while suffering confusion
     ***************************************/
	public bool ProcessConfusion()
	{
		return GameManager.instance.RandomInt(0, 1) == 1 ? true : false;
	} //end ProcessConfusion

	/***************************************
     * Name: ResolveOpponentLeaveField
     * Resolves effects based on the opponent
     ***************************************/
	public void ResolveOpponentLeaveField(PokemonBattler opponent)
	{
		//Check for multiturn attack
		if (effects[(int)LastingEffects.MultiTurnUser] == opponent.BattlerPokemon.PersonalID)
		{
			GameManager.instance.WriteBattleMessage(nickname + " was freed from the " + DataContents.GetMoveGameName(
				effects[(int)LastingEffects.MultiTurnAttack]) + "!");
			effects[(int)LastingEffects.MultiTurn] = 0;
			effects[(int)LastingEffects.MultiTurnUser] = 0;
			effects[(int)LastingEffects.MultiTurnAttack] = 0;
		} //end if

		//Check for infatuation
		if (effects[(int)LastingEffects.Attract] == opponent.BattlerPokemon.PersonalID)
		{
			GameManager.instance.WriteBattleMessage(nickname + " snapped out of infatuation over " + opponent.Nickname + "!");
			effects[(int)LastingEffects.Attract] = 0;
		} //end if
	} //end ResolveOpponentLeaveField(PokemonBattler opponent)

	/***************************************
     * Name: EndOfRoundResolve
     * Resolves effects at end of round
     ***************************************/
	public void EndOfRoundResolve()
	{
		//Increment turn count
		turnCount++;

		//Reset effects
		partyFainted = false;
		effects[(int)LastingEffects.KingsShield] = 0;
		effects[(int)LastingEffects.Protect] = 0;
		effects[(int)LastingEffects.Flinch] = 0;

		//Resolve multiturn attacks
		if (effects[(int)LastingEffects.MultiTurn] > 0)
		{
			//Reduce count
			effects[(int)LastingEffects.MultiTurn]--;
			if (effects[(int)LastingEffects.MultiTurn] == 0)
			{
				GameManager.instance.WriteBattleMessage(nickname + " was freed from the " + DataContents.GetMoveGameName(
					effects[(int)LastingEffects.MultiTurnAttack]) + "!");
				effects[(int)LastingEffects.MultiTurn] = 0;
				effects[(int)LastingEffects.MultiTurnUser] = 0;
				effects[(int)LastingEffects.MultiTurnAttack] = 0;
			} //end if
			else if(!CheckAbility(85))
			{				
				//Infestation
				if (effects[(int)LastingEffects.MultiTurnAttack] == 292)
				{
					RemoveHP(ExtensionMethods.BindToInt(totalHP / 8, 1));
					GameManager.instance.WriteBattleMessage(nickname + " was hurt by the infestation!");
				} //end if

				//Check for faint
				GameManager.instance.CheckForFaint(sideOn);
			} //end else if
		} //end if

		//Resolve Leech Seed
		if (effects[(int)LastingEffects.LeechSeed] > -1)
		{
			//Check for Magic Guard
			if (!CheckAbility(85))
			{		
				int sapped = ExtensionMethods.BindToInt(totalHP / 8, 1);
				RemoveHP(sapped);
				GameManager.instance.AdjustTargetHealth(effects[(int)LastingEffects.LeechSeed], sapped);
				GameManager.instance.WriteBattleMessage(nickname + "'s health was sapped by Leech Seed!");

				//Check for faint
				GameManager.instance.CheckForFaint(sideOn);
			} //end if
		} //end if

		//Resolve poison
		if (status == (int)Status.POISON)
		{
			//Check for Magic Guard
			if (!CheckAbility(85))
			{	
				RemoveHP(ExtensionMethods.BindToInt(totalHP / 8, 1));
				GameManager.instance.WriteBattleMessage(nickname + " was hurt by the poison!");

				//Check for faint
				GameManager.instance.CheckForFaint(sideOn);
			} //end if
		} //end if

		//Resolve burn
		if (status == (int)Status.BURN)
		{
			//Check for Magic Guard
			if (!CheckAbility(85))
			{	
				RemoveHP(ExtensionMethods.BindToInt(totalHP / 8, 1));
				GameManager.instance.WriteBattleMessage(nickname + " was hurt by the burn!");

				//Check for faint
				GameManager.instance.CheckForFaint(sideOn);
			} //end if
		} //end if

		//Resolve Curse
		if (effects[(int)LastingEffects.Curse] > 0)
		{
			//Check for Magic Guard
			if (!CheckAbility(85))
			{	
				RemoveHP(ExtensionMethods.BindToInt(totalHP / 4, 1));
				GameManager.instance.WriteBattleMessage(nickname + " was hurt by the Curse!");

				//Check for faint
				GameManager.instance.CheckForFaint(sideOn);
			} //end if
		} //end if

		//Resolve Yawn
		if (effects[(int)LastingEffects.Yawn] > 0)
		{
			effects[(int)LastingEffects.Yawn]--;
			if (effects[(int)LastingEffects.Yawn] == 0)
			{
				if (status != (int)Status.HEALTHY)
				{
					GameManager.instance.WriteBattleMessage(nickname + " is already afflicted with a status!");
				} //end if
				else if (GameManager.instance.CheckField() == 2)
				{
					GameManager.instance.WriteBattleMessage(nickname + " can't sleep due to the Electric Terrain!");
				} //end else if
				else
				{
					BattlerStatus = (int)Status.SLEEP;
					StatusCount = GameManager.instance.RandomInt(1, 4);
					GameManager.instance.WriteBattleMessage(nickname + " fell asleep!");
				} //end else
			} //end if
		} //end if

		//Resolve held items
		string result = ItemEffects.EndRoundItem(this); 
		if (result != bool.FalseString && battler.Item == 0)
		{
			item = 0;
			if (CheckAbility(180))
			{
				effects[(int)LastingEffects.Unburden] = 1;
			} //end if
		} //end if

		//Reset damges
		lastHPLost = 0;
		lastDamageDealt = 0;
	} //end EndOfRoundResolve

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
		lastHPLost = currentHP - battler.CurrentHP;
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
     * Name: ApplyFormCalculateStats
     * Changes the stats to match the form
     ***************************************/
	public void ApplyFormCalculateStats()
	{
		//Calculate nature impact
		int[] pvalues = {100,100,100,100,100};
		int[] results = new int[5];
		int nd5 = (int)Mathf.Floor (battler.Nature / 5);
		int nm5 = (int)Mathf.Floor (battler.Nature % 5);
		if (nd5 != nm5)
		{
			pvalues [nd5] = 110;
			pvalues [nm5] = 90;
		} //end if

		//Switch based on species
		switch(battler.NatSpecies)
		{			
			//Aegislash
			case 681:
				if (battler.FormNumber == 0)
				{
					//Change to Attack Stance
					battler.FormNumber = 1;

					//Calculate Attack
					int baseStat = 150;
					int evCalc = battler.GetEV(1) / 4;
					int firstCalc = (battler.GetIV(1) + 2 * baseStat + evCalc);
					int secondCalc = (firstCalc * currentLevel / 100);
					int bValue = (secondCalc + 5) * pvalues[0] / 100;
					float value = (float)bValue * (1f + (0.5f * stages[1]));
					attack = (int)value;

					//Calculate Defense
					baseStat = 50;
					evCalc = battler.GetEV(2) / 4;
					firstCalc = (battler.GetIV(2) + 2 * baseStat + evCalc);
					secondCalc = (firstCalc * currentLevel / 100);
					bValue = (secondCalc + 5) * pvalues[1] / 100;
					value = (float)bValue * (1f + (0.5f * stages[2]));
					defense = (int)value;

					//Calculate Speed
					value = (float)battler.Speed * (1f + (0.5f * stages[3]));
					speed = (int)value;

					//Calculate Special Attack
					baseStat = 150;
					evCalc = battler.GetEV(4) / 4;
					firstCalc = (battler.GetIV(4) + 2 * baseStat + evCalc);
					secondCalc = (firstCalc * currentLevel / 100);
					bValue = (secondCalc + 5) * pvalues[3] / 100;
					value = (float)bValue * (1f + (0.5f * stages[4]));
					specialA = (int)value;

					//Calculate Special Defense
					baseStat = 50;
					evCalc = battler.GetEV(5) / 4;
					firstCalc = (battler.GetIV(5) + 2 * baseStat + evCalc);
					secondCalc = (firstCalc * currentLevel / 100);
					bValue = (secondCalc + 5) * pvalues[4] / 100;
					value = (float)bValue * (1f + (0.5f * stages[5]));
					specialD = (int)value;
				} //end if
				else
				{
					//Change to Defense Stance
					battler.FormNumber = 0;

					//Calculate based on base battler
					CalculateStats();
				} //end else
				break;
		} //end switch
	} //end ApplyStanceChange

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
			//Guts
			if (CheckAbility(54) && status != (int)Status.HEALTHY && status != (int)Status.FAINT)
			{
				return (attack * 15) / 10;
			} //end if
			//Burn
			else if (status == (int)Status.BURN)
			{
				return attack / 2;
			} //end else if
			return attack;
		} //end else
	} //end GetAttack(bool critical)

	/***************************************
     * Name: GetDefense
     * Returns the working value of the defense
     * of the pokemon
     ***************************************/
	public int GetDefense(bool critical, int move)
	{
		//Return non-buff defense for critical
		if (critical && stages[2] > 0)
		{
			return battler.Defense;
		} //end if
		else if (move == 72)
		{
			return battler.Defense;
		} //end else if
		else
		{
			return defense;
		} //end else
	} //end GetDefense(bool critical, int move)

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
			result /= 10 - (stages[3] * 5);
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
		if (CheckAbility(143) && turnCount < 5)
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

		//Check for Unburden
		if (effects[(int)LastingEffects.Unburden] > 0)
		{
			speed *= 2;
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
		if (ability != abilityToCheck)
		{
			return false;
		} //end if
		else if (effects[(int)LastingEffects.GastroAcid] > 0)
		{
			//Magic Bounce, Multitype, Pickup, and Stance Change are immune to Gastro Acid
			if (ability != 84 && ability != 97 && ability != 108 && ability != 152)
			{
				return false;
			} //end if
		} //end else if

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
     * Name: CheckAirborne
     * Checks if pokemon is grounded or not
     ***************************************/
	public bool CheckAirborne()
	{
		//Iron Ball, Ingrain, Smack Down, Gravity
		if (CheckItem(128) || effects[(int)LastingEffects.Ingrain] > 0 || effects[(int)LastingEffects.SmackDown] > 0 ||
		    GameManager.instance.CheckEffect((int)FieldEffects.Gravity, sideOn))
		{
			return false;
		} //end if

		//Flying and no Roost, Levitate, Air Balloon, Magnet Rise, Telekinesis
		else if ((types.Contains(2) && effects[(int)LastingEffects.Roost] > 0) || CheckAbility(79) || CheckItem(10) ||
		         effects[(int)LastingEffects.MagnetRise] > 0 || effects[(int)LastingEffects.Telekinesis] > 0)
		{
			return true;
		} //end else if

		//Definitely not airborne
		else
		{
			return false;
		} //end else
	} //end CheckAirborne

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
			if (item != 0 && value == 0 && CheckAbility(180))
			{
				effects[(int)LastingEffects.Unburden] = 1;
			} //end if
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
			battler.Status = value;
			battler.StatusCount = 0;
		} //end set
	} //end BattlerStatus

	/***************************************
     * Name: StatusCount
     ***************************************/
	public int StatusCount
	{
		get
		{
			return battler.StatusCount;
		} //end get
		set
		{
			battler.StatusCount = value;
		} //end set
	} //end BattlerStatus

	/***************************************
     * Name: LastHPLost
     ***************************************/
	public int LastHPLost
	{
		get
		{
			return lastHPLost;
		} //end get
	} //end LastHPLost

	/***************************************
     * Name: LastDamageDealt
     ***************************************/
	public int LastDamageDealt
	{
		get
		{
			return lastDamageDealt;
		} //end get
		set
		{
			lastDamageDealt = value;
		} //end set
	} //end LastDamageDealt

	/***************************************
     * Name: LastMoveused
     ***************************************/
	public int LastMoveUsed
	{
		get
		{
			return lastMoveUsed;
		} //end get
		set
		{
			lastMoveUsed = value;
		} //end set
	} //end LastMoveUsed

	/***************************************
     * Name: TurnCount
     ***************************************/
	public int TurnCount
	{
		get
		{
			return turnCount;
		} //end get
		set
		{
			turnCount = value;
		} //end set
	} //end TurnCount

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

	/***************************************
     * Name: PartyFainted
     ***************************************/
	public bool PartyFainted
	{
		get
		{
			return partyFainted;
		} //end get
		set
		{
			partyFainted = value;
		} //end set
	} //end PartyFainted
	#endregion
} //end PokemonBattler class
