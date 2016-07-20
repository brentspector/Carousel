/***************************************************************************************** 
 * File:    Field.cs
 * Summary: Montiors battle field effects
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

public class Field
{
    #region Variables
	int activeField;				//What field is active
	List<int> defaultFieldBoosts;	//What are the applicable boosts of the field
    #endregion

    #region Methods
	/***************************************
	 * Name: Field
	 * Constructor for Field
	 ***************************************/
	public Field(int fieldStart)
	{
		activeField = fieldStart;
		ResetDefaultBoosts();
	} //end Field(int fieldStart)

	/***************************************
	 * Name: ResolveFieldEntrance
	 * Activates entrance effects of fields
	 ***************************************/
	public List<PokemonBattler> ResolveFieldEntrance(List<PokemonBattler> battlers)
	{
		switch (activeField)
		{
		//Default Field
			case 0:
				//Add buffs for every 5 levels of difference
				int modification = battlers[0].CurrentLevel - battlers[1].CurrentLevel;
				modification = ExtensionMethods.CapAtInt(modification / 5, 6);
				//Loop through opponent's stat boosts
				for (int i = 1; i < 6; i++)
				{
					//Opponent loses any default boost
					battlers[1].SetStage(-defaultFieldBoosts[i], i, true);

					//Set new boost
					if (modification > 0)
					{
						//Get modification value
						int modValue;
						if (modification + battlers[1].GetStage(i) > 6)
						{
							modValue = 6 - battlers[1].GetStage(i);
						} //end if
						else
						{
							modValue = modification;
						} //end else

						//Update battler
						battlers[1].SetStage(modValue, i, true);

						//Display result
						if (defaultFieldBoosts[i] < modValue)
						{
							GameManager.instance.WriteBattleMessage(string.Format("{0}'s stats rose by {1} to accomodate for " +
								"level difference!", battlers[1].Nickname, modValue - defaultFieldBoosts[i]));
						} //end if
						else if (defaultFieldBoosts[i] > modValue)
						{
							GameManager.instance.WriteBattleMessage(string.Format("{0}'s stats dropped by {1} to accomodate for " +
								"level difference!", battlers[1].Nickname, defaultFieldBoosts[i] - modValue));
						} //end else if
						else
						{
							GameManager.instance.WriteBattleMessage(battlers[1].Nickname + "'s stats were not affected by the " +
							"opponent's level!"); 
						} //end else

						//Update boosts
						defaultFieldBoosts[i] = modValue;
					} //end if
					else
					{
						//No field boosts given
						if (defaultFieldBoosts[i] != 0)
						{
							GameManager.instance.WriteBattleMessage(string.Format("{0} stats dropped by {1} to accomodate for " +
								"level difference!", battlers[1].Nickname, defaultFieldBoosts[i]));
						} //end if
						else
						{
							GameManager.instance.WriteBattleMessage(battlers[1].Nickname + "'s stats were not affected by the " +
								"opponent's level!"); 
						} //end else

						//Update boosts
						defaultFieldBoosts[i] = 0;
					} //end else
				} //end for
				break;
			//Grassy Terrain
			case 1:				
				break;
			//Electric Terrain
			case 2:				
				break;
			//Misty Terrain
			case 3:
				break;
		} //end switch

		//Return battlers list
		return battlers;
	} //end ResolveFieldEntrance

	/***************************************
	 * Name: ResetDefaultBoosts
	 * Resets boosts to 0 and recalculates
	 ***************************************/
	public void ResetDefaultBoosts()
	{
		defaultFieldBoosts = Enumerable.Repeat(0, 6).ToList();
	} //end ResetDefaultBoosts
    #endregion

	#region Properties
	/***************************************
	 * Name: ActiveField
	 ***************************************/
	public int ActiveField
	{
		get
		{
			return activeField;
		} //end get
		set
		{
			activeField = value;
		} //end set
	} //end ActiveField
	#endregion
} //end Field class

/***************************************************************************************** 
 * Enum:    FieldReference
 * Summary: Lists and organizes battle fields for integer reference
 *****************************************************************************************/ 
/***************************************
 * Name: FieldReference
 * List of battle fields available
 ***************************************/
public enum FieldReference
{
	Default				= 0,
	ElectricTerrain		= 1,
	MistyTerrain		= 2,
	GrassyTerrain		= 3,
	COUNT        		= 4
} //end FieldReference enum