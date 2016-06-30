/***************************************************************************************** 
 * File:    Field.cs
 * Summary: Montiors battle field effects
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class Field
{
    #region Variables
	int activeField;	//What field is active
    #endregion

    #region Methods
	/***************************************
	 * Name: Field
	 * Constructor for Field
	 ***************************************/
	public Field(int fieldStart)
	{
		activeField = fieldStart;
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
				if (modification > 4)
				{					
					modification /= 5;
					for (int i = 1; i < 6; i++)
					{
						battlers[1].SetStage(modification, i, true);
					} //end for
					GameManager.instance.WriteBattleMessage("The foe " + battlers[1].Nickname + " was boosted by " +
						ExtensionMethods.CapAtInt(modification, 6).ToString() + " stages to accomodate for level difference!");
				} //end if
				break;
		} //end switch

		//Return battlers list
		return battlers;
	} //end ResolveFieldEntrance
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