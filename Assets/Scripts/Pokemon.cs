/***************************************************************************************** 
 * File:    Pokemon.cs
 * Summary: General template for each pokemon's object's unique base data
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Random = UnityEngine.Random;
#endregion

[Serializable]
public class Pokemon
{
    #region Variables
	//Stats
	int currentHP;			//Current HP
	int totalHP;			//Max HP
	int attack;				//Standard attack
	int defense;			//Standard defense
    int speed;              //Standard speed
	int specialA;			//Standard special attack
	int specialD;			//Standard special defense
    int totalEV;            //Total amount of EVs the pokemon currently has
	int[] IV;				//Individual values array
	int[] EV;				//Effort values array

	//Information
    uint personalID;        //Defines a specific pokemon
	int natSpecies;			//National species number
	int trainerID;			//ID of trainer who obtained it first
	int currentEXP;			//Current EXP of pokemon
    int remainingEXP;       //The amount of EXP remaining to next level
    int currentLevel;       //Relative level of pokemon
	int item;				//What item is being held
	int status;				//What status the pokemon is under
	int statusCount;		//Turns until awaken/turns of toxic
	int ballUsed;			//What pokeball this pokemon is in
	int obtainType;			//How this pokemon was obtained
    int obtainFrom;         //Where this pokemon was obtained from
	int obtainLevel;		//What level the pokemon was obtained at
	int ability;			//What ability this pokemon has
	int gender;				//What gender the pokemon is
	int nature;				//What nature the pokemon has
	int happiness;			//Happiness level of pokemon
	int formNumber;			//What form this pokemon is in
	int[] moves;			//Move roster of pokemon
	int[] firstMoves;		//The moves this pokemon first knew when obtained
    int[] ppReamaining;     //The amount of uses each move has left
    List<int> ribbons;      //What ribbons have been obtained
	bool hasPokerus;		//Whether pokemon has pokerus
	bool isShiny;			//Whether pokemon is shiny
	bool[] markings;		//What markings this pokemon has
	string nickname;		//Nickname of pokemon
	string OtName;			//Name of the original trainer
    DateTime obtainTime;    //When this pokemon was obtained at

	[OptionalField(VersionAdded=2)]
	int abilityOn;			//What ability this pokemon is currently on
	[OptionalField(VersionAdded=2)]
	int[] vitamins;			//How many of each vitamin were added
	[OptionalField(VersionAdded=2)]
	int[] ppMax;			//The amount of uses each move has total
	[OptionalField(VersionAdded=2)]
	int[] ppUps;			//How many PP Ups and Maxes have been used
	[OptionalField(VersionAdded=2)]
	int itemRecycle;		//Stores an item valid to recycle
	[OptionalField(VersionAdded=2)]
	int type1;				//The primary type of this pokemon
	[OptionalField(VersionAdded=2)]
	int type2;				//The secondary type (if any) of this pokemon
	[OptionalField(VersionAdded=2)]
	int expForLevel;		//The amount of EXP needed for the next level
    #endregion

    #region Methods
    /***************************************
     * Name: Pokemon
     * Contructor for pokemon encounters
     ***************************************/
	public Pokemon(int species = 0, int tID = 0, int level = 5, int item = 0, int ball = 0, 
				   int oType = 6, int oWhere = 4, int ability = -1, int gender = -1, int form = 0,
	               int nature = -1, int happy = 70, bool pokerus = false, bool shiny = false)
	{
		//Set data fields
		if(species == 0)
		{
			natSpecies = GameManager.instance.RandomInt(1, 722);
		} //end if
		else
		{
			natSpecies = species;
		} //end else
        if (tID == 0)
        {
            trainerID = GameManager.instance.GetTrainer().PlayerID;
        } //end if
        else
        {
            trainerID = tID;
        } //end else
        if (gender == -1)
        {
            string genderRate = DataContents.ExecuteSQL<string>("SELECT genderRate FROM Pokemon WHERE rowid=" +
                                                                natSpecies);
            switch(genderRate)
            {
                case "AlwaysMale":
                {
                    Gender = 0;
                    break;
                } //end case AlwaysMale
                case "FemaleOneEighth":
                {
                    int randomNum = GameManager.instance.RandomInt(0,200);
                    if(randomNum < 26)
                    {
                        Gender = 1;
                    } //end if
                    else
                    {
                        Gender = 0;
                    } //end else
                    break;
                } //end case FemaleOneEighth
                case "Female25Percent":
                {
                    int randomNum = GameManager.instance.RandomInt(0,200);
                    if(randomNum < 51)
                    {
                        Gender = 1;
                    } //end if
                    else
                    {
                        Gender = 0;
                    } //end else
                    break;
                } //end case Female25Percent
                case "Female50Percent":
                {
                    int randomNum = GameManager.instance.RandomInt(0,200);
                    if(randomNum < 101)
                    {
                        Gender = 1;
                    } //end if
                    else
                    {
                        Gender = 0;
                    } //end else
                    break;
                } //end case Female50Percent
                case "Female75Percent":
                {
                    int randomNum = GameManager.instance.RandomInt(0,200);
                    if(randomNum < 151)
                    {
                        Gender = 1;
                    } //end if
                    else
                    {
                        Gender = 0;
                    } //end else
                    break;
                } //end case Female75Percent
                case "FemaleSevenEighths":
                {
                    int randomNum = GameManager.instance.RandomInt(0,200);
                    if(randomNum < 176)
                    {
                        Gender = 1;
                    } //end if
                    else
                    {
                        Gender = 0;
                    } //end else
                    break;
                } //end case FemaleSevenEighths
                case "AlwaysFemale":
                {
                    Gender = 1;
                    break;
                } //end case AlwaysFemale
                case "Genderless":
                {
                    Gender = 2;
                    break;
                } //end case Genderless
                default:
                {
                    GameManager.instance.LogErrorMessage("Default for gender reached for " + natSpecies + " " 
                                                         + genderRate);
                    Gender = 0;
                    break;
                } //end case default
            } //end switch
        } //end if
        else
        {
            Gender = gender;
        } //end else
        if (nature == -1)
        {
            Nature = GameManager.instance.RandomInt(0,(int)Natures.COUNT);
        } //end if
        else
        {
            Nature = nature;
        } //end else
        if (ability == -1)
        {
            //Choose a number from 0 to 2
            int temp = GameManager.instance.RandomInt (0, 3);

            //While the abilty doesn't exist, pick another number
            while(!CheckAbility(temp))
            {
                temp = GameManager.instance.RandomInt (0, 3);
            } //end while
        } //end if
        else
        {
            //Make sure the requested ability works, or set to first ability (guaranteed to exist)
            Ability = CheckAbility(ability) ? ability : 0;
        } //end else

        //Give pokemon a random ID
        personalID = (uint)GameManager.instance.RandomInt (0,256);
        personalID |= (uint)GameManager.instance.RandomInt (0,256) << 8;
        personalID |= (uint)GameManager.instance.RandomInt (0,256) << 16;

        totalEV = 0;
		currentLevel = ExtensionMethods.WithinIntRange(level, 1, 100);
		currentEXP = CalculateEXP (currentLevel);
		remainingEXP = CalculateRemainingEXP (currentLevel);
		Item = item;
		ballUsed = ball;
		obtainType = oType;
		obtainLevel = currentLevel;
        obtainFrom = oWhere;
		string formString = DataContents.ExecuteSQL<string>("SELECT forms FROM Pokemon WHERE rowid=" + natSpecies);
		int formCount = formString.Split(',').Count();
		formNumber = ExtensionMethods.WithinIntRange(form, 0, formCount);
		happiness = ExtensionMethods.WithinIntRange(happy, 0, 255);
		hasPokerus = pokerus;
		isShiny = shiny;
        nickname = DataContents.ExecuteSQL<string> ("SELECT name FROM Pokemon WHERE rowid=" + natSpecies);
        OtName = GameManager.instance.GetTrainer().PlayerName;
        obtainTime = DateTime.Now;

		//Initialize arrays
		IV = new int[6];
		EV = new int[6];
		moves = new int[4];
		firstMoves = new int[4];
        ppReamaining = new int[4];
		markings = new bool[DataContents.markingCharacters.Length];
		ribbons = new List<int>();
		vitamins = new int[6];
		ppMax = new int[4];
		ppUps = new int[4];

		for(int i = 0; i < 6; i++)
		{
            IV[i] = 0;
			EV[i] = 0;
			vitamins[i] = 0;
		} //end for

		for(int i = 0; i < 4; i++)
		{
			moves[i] = -1;
			firstMoves[i] = -1;
		} //end for

		for(int i = 0; i < markings.Length; i++)
		{
			markings[i] = false;
		} //end for

        //Initialize any values that can be given from data
        ChangeIVs (new int[] {-1});
        CalculateStats ();
		SetTypes();
        currentHP = totalHP;
        GiveInitialMoves (new int[] {-1, -1, -1, -1});
	} //end Pokemon constructor

    /***************************************
     * Name: ChangeIVs
     * Sets the pokemon's IVs depending on
     * the given parameters
     ***************************************/
    /* NOTE: You can provide a number that does not fall between
     * 0 and 31, and it will give a random result for that IV. 
     * If only one number that doesn't fall between 0 and 31
     * is provided, all IVs will be set to a different random number.
     * However, if changing a specific IV, and the value is not
     * valid, only the specified IV will change. If an invalid 
     * index is provided, it will default to randomize all IVs.
     * If the value is valid, but the index is not, all IVs will
     * all be set to the value.
     */
	public void ChangeIVs(int[] values, int index = -1)
	{
        //If only 1 value is provided, make all IVs that value
        if (values.Length == 1)
        {
            //Make sure value is between 0 and 31
            if(values[0] < 0 || values[0] > 31)
            {
                //Fill specified IV with random IV
                if(index > -1 && index < 6)
                {
                    IV[index] = GameManager.instance.RandomInt(0, 32);
                } //end if
                else
                {
                    //If not, fill with random IVs
                    for (int i = 0; i < 6; i++)
                    {
                        IV [i] = GameManager.instance.RandomInt(0, 32);
                    } //end for
                } //end else
            } //end if
            else
            {
                //Fill specified index with specified value
                if(index > -1 && index < 6)
                {
                    IV[index] = values[0];
                } //end if

                //Set all values to the provided value
                for (int i = 0; i < 6; i++)
                {
                    IV [i] = values [0];
                } //end for
            } //end else
        } //end if
        //Otherwise, set each to the specified value
        else
        {
            //Fill in  missing values
            if(values.Length < 6)
            {
                //Create new 6 int array
                int[] fixedValues = new int[6]; 

                //Fill in fixedValues with given values
                for(int i = 0; i < values.Length; i++)
                {
                    fixedValues[i] = values[i];
                } //end for

                //Fill in rest with a random number
                for(int i = values.Length; i < 6; i++)
                {
                    fixedValues[i] = GameManager.instance.RandomInt(0, 32);
                } //end for

                values = fixedValues;
            } //end if

            //Fill in values
            for(int i = 0; i < 6; i++)
            {
                //Make sure value is valid
                if(values[i] < 0 || values[i] > 31)
                {
                    IV[i] = GameManager.instance.RandomInt(0, 32);
                } //end if
                else
                {
                    IV[i] = values[i];
                } //end else
            } //end for
        } //end else
	} //end ChangeIVs(int[] values, int index = -1)

    /***************************************
     * Name: ChangeEVs
     * Sets the pokemon's EVs depending on
     * the given parameters
     ***************************************/
    /* NOTE: 
     * TotalEVs maximum is 510.
     * Individual EVs maximum is 255.
     * ---When providing 1 value---
     * Negative value with index sets that index to a random EV
     * Negative value without index sets all EVs to different random EVs
     * Excessive value with index sets that index to standard 85
     * Excessive value without index sets all EVs to 85
     * Acceptable value with index sets that index to that value
     * Acceptable value without index sets all EVs to that value
     * ---When providing more than 1 value---
     * If less than 6 values given, a 0 will be placed in empty slots
     * Otherwise it follows single EV methods, except ignores index param
     */
    public void ChangeEVs(int[] values, int index = -1)
    {
        //1 value provided
        if (values.Length == 1)
        {
            //If the value is invalid, so give random value
            if(values[0] < 0)
            {
                //If an index is provided
                if(index > -1 && index < 6)
                {
                    //Make sure value is within maximum value
                    int tempCount = totalEV - EV[index];
                    int tempRand = GameManager.instance.RandomInt(0, 256);
                    if((tempCount + tempRand) <= 510)
                    {
                        totalEV += tempRand - EV[index];
                        EV[index] = tempRand;
                    } //end if
                    else
                    {
                        tempRand = 510 - tempCount;
                        totalEV = 510;
                        EV[index] = tempRand;
                    } //end else
                } //end if
                //Valid index was not provided
                else
                {
                    //Set all EVs to a random amount
                    totalEV = 0;
                    for(int i = 0; i < 6; i++)
                    {
                        //Make a new random amount
                        int tempRand = GameManager.instance.RandomInt(0, 256);

                        //Make sure value is withing maximum value
                        if((totalEV + tempRand) <= 510)
                        {
                            EV[i] = tempRand;
                            totalEV += tempRand;
                        } //end if
                        //If not, cap the value set rest to 0
                        else
                        {
                            tempRand = 510 - totalEV;
                            totalEV = 510;
                            EV[i] = tempRand;

                            //Loop through rest and set to 0
                            for(int r = i+1; r < 6; r++)
                            {
                                EV[r] = 0;
                            } //end for

                            break;
                        } //end else
                    } //end for
                } //end else
            } //end if
            //Standard amount of 85 to be given
            else if(values[0] > 255)
            {
                //If a valid index is given
                if(index > -1 && index < 6)
                {
                    //Make sure value is within maximum value
                    int tempCount = totalEV - EV[index];
                    if((tempCount + 85) <= 510)
                    {
                        totalEV += 85 - EV[index];
                        EV[index] = 85;
                    } //end if
                    else
                    {
                        int tempNum = 510 - tempCount;
                        totalEV = 510;
                        EV[index] = tempNum;
                    } //end else
                } //end if
                //No valid index given
                else
                {
                    //Give all EVs a value of 85
                    totalEV = 510;
                    for(int i = 0; i < 6; i++)
                    {
                        EV[i] = 85;
                    } //end for
                } //end else
            } //end else if
            //Set EVs to the amount given, up to 510 total
            else
            {
                //If a valid index was given
                if(index > -1 && index < 6)
                {
                    int tempCount = totalEV - EV[index];
                    //Make sure value doesn't exceed 510
                    if((tempCount + values[0]) <= 510)
                    {
                        totalEV += values[0] - EV[index];
                        EV[index] = values[0];
                    } //end if
                    else
                    {
                        int tempNum = 510 - totalEV; 
                        totalEV = 510;
                        EV[index] = tempNum;
                    } //end else
                } //end if
                //No vald index given
                else
                {
                    //Set all EVs to value given
                    totalEV = 0;
                    for(int i = 0; i < 6; i++)
                    {
                        //Make sure values added doesn't exceed total
                        if((totalEV + values[0]) <= 510)
                        {
                            totalEV += values[0];
                            EV[i] = values[0];
                        } //end if
                        else
                        {
                            int tempNum = 510 - totalEV;
                            totalEV = 510;
                            EV[i] = tempNum;

                            //Loop through and set all remaining to 0
                            for(int r = i+1; r < 6; r++)
                            {
                                EV[r] = 0;
                            } //end for
                            
                            break;
                        } //end else
                    } //end for
                } //end else
            } //end else
        } //end if
        //Mulitple values provided
        else
        {
            //Initialize totalEV count
            totalEV = 0;

            //Fill in missing values
            if(values.Length < 6)
            {
                //Create new 6 int long array
                int[] fixedValues = new int[6]; 
                
                //Fill in fixedValues with given values
                for(int i = 0; i < values.Length; i++)
                {
                    fixedValues[i] = values[i];
                } //end for
                
                //Fill in rest with a random number
                for(int i = values.Length; i < 6; i++)
                {
                    fixedValues[i] = GameManager.instance.RandomInt(0, 256);
                } //end for
                
                values = fixedValues;
            } //end if

            //Fill in given values
            for (int i = 0; i < 6; i++)
            {
                //If the value is invalid
                if(values[i] < 0)
                {
                    //Make sure value is within maximum value
                    int tempCount = totalEV;
                    int tempRand = GameManager.instance.RandomInt(0, 256);
                    if((tempCount + tempRand) <= 510)
                    {
                        totalEV += tempRand;
                        EV[i] = tempRand;
                    } //end if
                    else
                    {
                        tempRand = 510 - tempCount;
                        totalEV = 510;
                        EV[i] = tempRand;

                        //Loop through and set all remaining to 0
                        for(int r = i+1; r < 6; r++)
                        {
                            EV[r] = 0;
                        } //end for
                        
                        break;
                    } //end else
                } //end if
                //If the value is invalid
                else if(values[i] > 255)
                {
                    //Make sure value is within maximum value
                    int tempCount = totalEV;
                    if((tempCount + 85) <= 510)
                    {
                        totalEV += 85;
                        EV[i] = 85;
                    } //end if
                    else
                    {
                        int tempNum = 510 - tempCount;
                        totalEV = 510;
                        EV[index] = tempNum;

                        //Loop through and set all remaining to 0
                        for(int r = i+1; r < 6; r++)
                        {
                            EV[r] = 0;
                        } //end for
                        
                        break;
                    } //end else
                } //end else if
                //The value is valid
                else
                {
                    //Make sure value doesn't exceed maximum
                    if((totalEV + values[i]) <= 510)
                    {
                        totalEV += values[i];
                        EV[i] = values[i];
                    } //end if
                    else
                    {
                        int tempNum = 510 - totalEV;
                        totalEV = 510;
                        EV[i] = tempNum;

                        //Loop through and set all remaining to 0
                        for(int r = i+1; r < 6; r++)
                        {
                            EV[r] = 0;
                        } //end for
                        
                        break;
                    } //end else
                } //end else
            } //end if
        } //end else
    } //end ChangeEVs(int[] values, int index = -1)

    /***************************************
     * Name: ChangeMoves
     * Sets the pokemon's moves depending on
     * given parameters
     ***************************************/
    /* NOTE: Moves are replaced from front to back. Thus providing
     * only 1 value will only replace the first moveslot. Provide
     * a valid index number to change that specific moveslot. If
     * multiple moves are provided, and an index is provided, only
     * the move at that index will be replaced with the first move
     * in the list of moves given.
     */
    public void ChangeMoves(int[] values, int index = -1)
    {
        //If a valid index is provided
        if (index > -1 && index < 4)
        {
            moves[index] = values[0];
            ppReamaining[index] = DataContents.ExecuteSQL<int> 
                ("SELECT totalPP FROM Moves WHERE rowid=" + moves[index]);
			ppMax[index] = ppReamaining[index];
			ppUps[index] = 0;
        } //end if
        //No valid index given
        else
        {
            for(int i = 0; i < values.Length; i++)
            {
                moves[i] = values[i];
                ppReamaining[i] = DataContents.ExecuteSQL<int> 
                    ("SELECT totalPP FROM Moves WHERE rowid=" + moves[i]);
				ppMax[i] = ppReamaining[i];
				ppUps[i] = 0;
            } //end for
        } //end else
    } //end ChangeMoves(int[] values, int index = -1)

    /***************************************
     * Name: GiveInitialMoves
     * Sets the pokemon's starting moveset
     ***************************************/
    //Given initial moves, including any special moves to be saved under first moves
    //NOTE: A value of -1 will give the last non-duplicate level-up move available.
    //If no level up move is available, it will be left blank
    public void GiveInitialMoves(int[] values)
    {
        //Fill in missing values
        if (values.Length < 4)
        {
            //Create new 4 int long array
            int[] fixedValues = new int[4]; 
            
            //Fill in fixedValues with given values
            for(int i = 0; i < values.Length; i++)
            {
                fixedValues[i] = values[i];
            } //end for
            
            //Fill in rest with a signal number
            for(int i = values.Length; i < 4; i++)
            {
                fixedValues[i] = -1;
            } //end for

            //Reassign values
            values = fixedValues;
        } //end if

        //Sort the values to avoid misplaced -1 values
        Array.Sort (values);
        Array.Reverse (values);

        //Loop through values
        for (int i = 0; i < 4; i++)
        {
            //Make sure value isn't -1
            if(values[i] != -1)
            {
                //Set the move to the given value
                moves[i] = values[i];
                firstMoves[i] = values[i];
                ppReamaining[i] = DataContents.ExecuteSQL<int> 
                    ("SELECT totalPP FROM Moves WHERE rowid=" + moves[i]);
            } //end if
            //If it equals -1, give level up move
            else
            {
                //Get levels closest to current level
                string moveList = DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=" + natSpecies);
                string[] arrayList = moveList.Split(',');
                int tempHolder = 0;
                List<string> pos = arrayList.Select((moveLevel,listIndex) => int.TryParse(moveLevel,out tempHolder) ? 
                    int.Parse(moveLevel) <= currentLevel ? arrayList[listIndex+1] : "null" : "null").
                    Where(listIndex => listIndex != "null").ToList();

                //Fill remaining moves with values
                for(int j = i, k = pos.Count-1; j < 4 && k > -1; j++, k--)
                {
                    int moveID = DataContents.GetMoveID(pos[k]);
                    if(!moves.Contains(moveID))
                    {
                        moves[j] = moveID;
                        firstMoves[j] = moveID;
                        ppReamaining[j] = DataContents.ExecuteSQL<int> 
                            ("SELECT totalPP FROM Moves WHERE rowid=" + moves[j]);
                    } //end if
                    else
                    {
                        j--;
                    } //end else
                } //end for

                //End function
                break;
            } //end else
        } //end for
    } //end GiveInitialMoves(int[] moves)

	/***************************************
     * Name: SetTypes
     * Sets the pokemon's types
     ***************************************/
	void SetTypes()
	{
		//Set the primary (first) type
		type1 = Convert.ToInt32(Enum.Parse(typeof(Types),
			DataContents.ExecuteSQL<string>("SELECT type1 FROM Pokemon WHERE rowid=" + natSpecies)));

		//Get the string for the secondary type
		string type2SQL = DataContents.ExecuteSQL<string>("SELECT type2 FROM Pokemon WHERE rowid=" + natSpecies);

		//If a second type exists, set it
		if (!String.IsNullOrEmpty(type2SQL))
		{
			type2 = Convert.ToInt32(Enum.Parse(typeof(Types), type2SQL));
		} //end if
		else
		{
			type2 = -1;
		} //end else
	} //end SetTypes

    /***************************************
     * Name: ChangeRibbons
     * Awards or removes a ribbon from the pokemon
     ***************************************/
    public void ChangeRibbons(int value, int index = -1)
    {
        //Remove ribbon
        if (value < 0)
        {
            ribbons.RemoveAt (index);
        } //end if
        //Add ribbon
        else if (index < 0)
        {
            ribbons.Add (value);
        } //end else if
        //Otherwise attempt to update
        else if (value < DataContents.ribbonSprites.Length && index < ribbons.Count)
        {
            ribbons[index] = value;
        } //end else if
    } //end ChangeRibbons(int value, int index = -1)
        
    /***************************************
     * Name: CalculateEXP
     * Determines the initial EXP for the 
     * pokemon's level
     ***************************************/
	int CalculateEXP(int level)
	{
        return DataContents.experienceTable.GetCurrentValue (
            DataContents.ExecuteSQL<string>("SELECT growthRate FROM Pokemon WHERE rowid=" + natSpecies),
            level);
	} //end CalculateEXP(int level)

    /***************************************
     * Name: CalculateRemainingEXP
     * Determines the remaining EXP for the 
     * pokemon's level
     ***************************************/
    int CalculateRemainingEXP(int level)
    {
		int nextLevel = DataContents.experienceTable.GetNextValue (
            DataContents.ExecuteSQL<string>("SELECT growthRate FROM Pokemon WHERE rowid=" + natSpecies),
            level);
		int thisLevel = DataContents.experienceTable.GetCurrentValue (
			DataContents.ExecuteSQL<string>("SELECT growthRate FROM Pokemon WHERE rowid=" + natSpecies),
			level);
		expForLevel = nextLevel - thisLevel;
		return ExtensionMethods.BindToInt(nextLevel - CurrentEXP, 0);
    } //end CalculateRemainingEXP(int level)

    /***************************************
     * Name: CalculateHP
     * Determines the HP stat of the pokemon
     ***************************************/
    void CalculateHP()
    {
        int evCalc = EV [0] / 4;
        int baseHP = DataContents.ExecuteSQL<int> ("SELECT health FROM Pokemon WHERE rowid=" + natSpecies);
        int firstCalc = (IV [0] + 2 * baseHP + evCalc + 100);
        int secondCalc = (firstCalc * currentLevel);
        totalHP = secondCalc/ 100 + 10;
        currentHP = totalHP;
    } //end CalculateHP

    /***************************************
     * Name: CalculateStats
     * Determines the values for all stats,
     * although HP is calculated in another
     * function, but is called here
     ***************************************/
    public void CalculateStats()
    {
        //Calculate nature impact
        int[] pvalues = {100,100,100,100,100};
        int[] results = new int[5];
        int nd5 = (int)Mathf.Floor (nature / 5);
        int nm5 = (int)Mathf.Floor (nature % 5);
        if (nd5 != nm5)
        {
            pvalues [nd5] = 110;
            pvalues [nm5] = 90;
        } //end if

        //Calculate HP
        CalculateHP ();

        //Loop through and set all non-HP stats
        for (int i = 1; i < 6; i++)
        {
            int baseStat = 0;
            if(i == 1)
            {
                baseStat = DataContents.ExecuteSQL<int>("SELECT attack FROM Pokemon WHERE rowid=" + natSpecies);
            } //end if
            else if(i == 2)
            {
                baseStat = DataContents.ExecuteSQL<int>("SELECT defence FROM Pokemon WHERE rowid=" + natSpecies);
            } //end else if
            else if(i == 3)
            {
                baseStat = DataContents.ExecuteSQL<int>("SELECT speed FROM Pokemon WHERE rowid=" + natSpecies);
            } //end else if
            else if(i == 4)
            {
                baseStat = DataContents.ExecuteSQL<int>("SELECT specialAttack FROM Pokemon WHERE rowid=" + natSpecies);
            } //end else if
            else
            {
                baseStat = DataContents.ExecuteSQL<int>("SELECT specialDefence FROM Pokemon WHERE rowid=" + natSpecies);
            } //end else if
            int evCalc = EV[i]/4;
            int firstCalc = (IV [i] + 2 * baseStat + evCalc);
            int secondCalc = (firstCalc*currentLevel / 100);
            results[i-1] = (secondCalc + 5) * pvalues[i - 1] / 100;
        } //end for

        //Set the values
        attack = results[0];
        defense = results[1];
        speed = results[2];
        specialA = results[3];
        specialD = results[4];

		//Calculate EXP
		remainingEXP = CalculateRemainingEXP(currentLevel);
    } //end CalculateStat

    /***************************************
     * Name: SwitchMoves
     * Switches position of two moves
     ***************************************/
    public void SwitchMoves(int move1, int move2)
    {
        int temp = moves [move1];
        moves [move1] = moves [move2];
        moves [move2] = temp;
        temp = ppReamaining [move1];
        ppReamaining [move1] = ppReamaining [move2];
        ppReamaining [move2] = temp;
		temp = ppMax[move1];
		ppMax[move1] = ppMax[move2];
		ppMax[move2] = temp;
		temp = ppUps[move1];
		ppUps[move1] = ppUps[move2];
		ppUps[move2] = temp;
    } //end SwitchMoves(int move1, int move2)

    /***************************************
     * Name: CheckPokemonFaint
     * Checks if Pokemon is fainted
     ***************************************/
	public bool CheckPokemonFaint()
    {
		if (currentHP < 1)
		{
			currentHP = 0;
			status = 1;
			return true;
		} //end if
		return false;
	} //end CheckPokemonFaint

	/***************************************
     * Name: RevivePokemon
     * Sets status to healthy and HP to 
     * parameter
     ***************************************/
	public void RevivePokemon(int amountRestored)
	{		
		currentHP = ExtensionMethods.BindToInt(amountRestored, 1);
		status = 0;
		statusCount = 0;
	} //end RevivePokemon(int amountRestored)

    /***************************************
     * Name: CheckAbility
     * Verifies the ability exists, and sets it
     ***************************************/
    public bool CheckAbility(int newAbility)
    {
        //Storage for ability name
        string abilityName = "";

        //Set string to first ability
        if (newAbility == 0)
        {
            abilityName = DataContents.ExecuteSQL<string>
                ("SELECT ability1 FROM Pokemon WHERE rowid=" + natSpecies);
			abilityOn = 0;
        } //end if
        //Check if pokemon has a second ability
        else if(newAbility == 1)
        {
            if (string.IsNullOrEmpty (DataContents.ExecuteSQL<string>
                ("SELECT ability2 FROM Pokemon WHERE rowid=" + natSpecies)))
            {
                return false;
            } //end if
            else
            {
                abilityName = DataContents.ExecuteSQL<string>
                    ("SELECT ability2 FROM Pokemon WHERE rowid=" + natSpecies);
				abilityOn = 1;
            } //end else
        } //end if
        
        //Check if pokemon has a hidden ability
        else if(newAbility == 2)
        {
            if (string.IsNullOrEmpty (DataContents.ExecuteSQL<string>
                ("SELECT hiddenAbility FROM Pokemon WHERE rowid=" + natSpecies)))
            {
                return false;
            } //end if
            else
            {
                abilityName = DataContents.ExecuteSQL<string>
                    ("SELECT hiddenAbility FROM Pokemon WHERE rowid=" + natSpecies);
				abilityOn = 2;
            } //end else
        } //end else if

        //Get rowid of ability
        ability = DataContents.ExecuteSQL<int>("SELECT rowid FROM Abilities WHERE internalName='" + abilityName + "'");

        //Passed the check
        return true;
    } //end CheckAbility(int ability)  

    /***************************************
     * Name: GetAbilityName
     * Returns ability name
     ***************************************/
    public string GetAbilityName()
    {
        return DataContents.ExecuteSQL<string> ("SELECT gameName FROM Abilities WHERE rowid=" + ability);
    } //end GetAbilityName

    /***************************************
     * Name: GetAbilityDescription
     * Returns ability description
     ***************************************/
    public string GetAbilityDescription()
    {
        return DataContents.ExecuteSQL<string> ("SELECT description FROM Abilities WHERE rowid=" + ability);
    } //end GetAbilityDescription

    /***************************************
     * Name: GetMarkingsString
     * Returns string of colored markings
     ***************************************/
    public string GetMarkingsString()
    {
        //String of markings that have been colored
        string coloredMarkings = "";

        //Loop through markings and add to string
        for(int i = 0; i < DataContents.markingCharacters.Length; i++)
        {
            coloredMarkings += markings[i] ? 
                "<color=aqua>" + DataContents.markingCharacters[i].ToString() + "</color>" :
                "<color=grey>" + DataContents.markingCharacters[i].ToString() + "</color>"; 
        } //end for
        return coloredMarkings;
    } //end GetMarkingsString

	/***************************************
     * Name: CheckEvolution
     * Check if pokemon can evolve, and return
     * what it evolves into
     ***************************************/
	public int CheckEvolution(int usedItem = 0)
	{
		//Get evolved forms
		string evolveList = DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=" + natSpecies);

		//End function if there are no evolutions
		if (string.IsNullOrEmpty(evolveList))
		{
			return -1;
		} //end if

		//Divide evolutions up
		string[] arrayList = evolveList.Split(',');

		//Non Item-based evolution
		if (usedItem == 0)
		{
			return -1;
		} //end if
	
		//Item-based evolution
		else
		{
			//Magetic field based evolution
			if (usedItem == 43)
			{
				//Check if any forms evolve by magnetic radiation
				for (int i = 1; i < arrayList.Length; i += 3)
				{
					//A magnetic field based evolution was found
					if (arrayList[i] == "Location" && int.Parse(arrayList[i + 1]) == 281)
					{
						return DataContents.GetPokemonID(arrayList[i - 1]);
					} //end if
				} //end for
			} //end if

			//Dawn, Dusk, Fire, Leaf, Moon, Shiny, Sun, Thunder,Water Stone evolution
			else if (usedItem == 60 || usedItem == 75 || usedItem == 89 || usedItem == 145 || item == 186 || item== 258 || item == 280
				|| item == 288 || item == 402)
			{
				//Check if any forms evolve by stone
				for (int i = 1; i < arrayList.Length; i += 3)
				{
					//A stone evolution was found
					if (arrayList[i] == "Item" && DataContents.ExecuteSQL<int>("SELECT rowid FROM Items WHERE internalName='" +
					    arrayList[i + 1] + "'") == usedItem)
					{
						return DataContents.GetPokemonID(arrayList[i - 1]);
					} //end if

					//A male only stone evolution was found
					else if (arrayList[i] == "ItemMale" && DataContents.ExecuteSQL<int>("SELECT rowid FROM Items WHERE internalName='" +
						arrayList[i + 1] + "'") == usedItem)
					{
						if (gender == 0)
						{
							return DataContents.GetPokemonID(arrayList[i - 1]);
						} //end if
						else
						{
							GameManager.instance.DisplayText(string.Format("{0} must be male to evolve with a {1}.", nickname, 
								DataContents.GetItemGameName(usedItem)), true);
						} //end else
					} //end else if

					//A female only stone evolution was found
					else if (arrayList[i] == "ItemFemale" && DataContents.ExecuteSQL<int>("SELECT rowid FROM Items WHERE internalName='" +
						arrayList[i + 1] + "'") == usedItem)
					{
						if (gender == 1)
						{
							return DataContents.GetPokemonID(arrayList[i - 1]);
						} //end if
						else
						{
							GameManager.instance.DisplayText(string.Format("{0} must be female to evolve with a {1}.", nickname, 
								DataContents.GetItemGameName(usedItem)), true);
						} //end else
					} //end else if
				} //end for
			} //end else if

			//Trade based evolution
			else if (usedItem == 64)
			{
				//Check if any forms evolve by trade
				for (int i = 1; i < arrayList.Length; i+=3)
				{
					//A Trade evolution was found
					if (arrayList[i] == "Trade")
					{
						return DataContents.GetPokemonID(arrayList[i - 1]);
					} //end if

					//An Item Trade evolution was found
					else if (arrayList[i] == "TradeItem")
					{
						//Check if holding the necessary item
						if (DataContents.ExecuteSQL<int>("SELECT rowid FROM Items WHERE internalName='" +
						    arrayList[i + 1] + "'") == item)
						{
							item = 0;
							return DataContents.GetPokemonID(arrayList[i - 1]);
						} //end if
						else
						{
							GameManager.instance.DisplayText(nickname + " is not holding the required " +
							arrayList[i + 1] + " to evolve by trade.", true);
						} //end else
					} //end else if
				} //end for
			} //end else if

			//No evolution found
			return -1;
		} //end else
	} //end CheckEvolution(int item = 0)

	/***************************************
     * Name: EvolvePokemon
     * Evolves the pokemon into requested
     ***************************************/
	public void EvolvePokemon(int newPokemon)
	{
		//Make sure a valid value was passed
		if (newPokemon < 0 || newPokemon > 721)
		{
			return;
		} //end if

		//Update national species
		natSpecies = newPokemon;

		//Update ability
		CheckAbility(abilityOn);

		//Update stats
		CalculateStats();
	} //end EvolvePokemon(int newPokemon)

	/***************************************
     * Name: UpdateAbilityOn
     * Legacy file fix
     ***************************************/
	public void UpdateAbilityOn()
	{
		string abilityName = DataContents.ExecuteSQL<string>("SELECT internalName FROM Abilities WHERE rowid=" + ability);
		//Check if first ability
		if (abilityName == DataContents.ExecuteSQL<string>
			("SELECT ability1 FROM Pokemon WHERE rowid=" + natSpecies))
		{
			abilityOn = 0;
		} //end if

		//Check if second ability
		else if(abilityName == DataContents.ExecuteSQL<string>
			("SELECT ability2 FROM Pokemon WHERE rowid=" + natSpecies))
		{
			abilityOn = 1;
		}

		//Hidden or custom ability
		else
		{
			abilityOn = 2;
		} //end else
	} //end UpdateAbilityOn

	/***************************************
     * Name: UpdateVitamins
     * Legacy file fix
     ***************************************/
	public void UpdateVitamins()
	{
		vitamins = new int[6];

		for(int i = 0; i < 6; i++)
		{
			vitamins[i] = 0;
		} //end for
	} //end UpdateVitamins

	/***************************************
     * Name: UpdatePP
     * Legacy file fix
     ***************************************/
	public void UpdatePP()
	{
		ppMax = new int[4];
		ppUps = new int[4];

		for(int i = 0; i < 4; i++)
		{
			ppMax[i] = ppReamaining[i];
			ppUps[i] = 0;
		} //end for
	} //end UpdateVitamins
	#region Accessors
	//Stats
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
     * Name: TotalEVs
     ***************************************/
    public int TotalEVs
    {
        get
        {
            return totalEV;
        } //end get
        set
        {
            totalEV = value;
        } //end set
    } //end TotalEVs

    /***************************************
     * Name: GetIV
     ***************************************/
	public int GetIV(int index)
	{
		return IV [index];
	} //end GetIV(int index)

    /***************************************
     * Name: SetIV
     ***************************************/
	public void SetIV(int index, int value)
	{
		IV [index] = value;
	} //end SetIV(int index, int value)

    /***************************************
     * Name: GetEV
     ***************************************/
	public int GetEV(int index)
	{
		return EV [index];
	} //end GetEV(int index)

    /***************************************
     * Name: SetEV
     ***************************************/
	public void SetEV(int index, int value)
	{
		EV [index] = value;
	} //end SetEV(int index, int value)

	//Information
    /***************************************
     * Name: NatSpecies
     ***************************************/
	public int NatSpecies
	{
		get
		{
			return natSpecies;
		} //end get
		set
		{
			natSpecies = value;
		} //end set
	} //end NatSpecies

    /***************************************
     * Name: PersonalID
     ***************************************/
	public int PersonalID
	{
		get
		{
			return (int)personalID;
		} //end get
		set
		{
			personalID = (uint)value;
		} //end set
	} //end PersonalID

    /***************************************
     * Name: TrainerID
     ***************************************/
	public int TrainerID
	{
		get
		{
			return trainerID;
		} //end get
		set
		{
			trainerID = value;
		} //end set
	} //end TrainerID

    /***************************************
     * Name: CurrentEXP
     ***************************************/
	public int CurrentEXP
	{
		get
		{
			return currentEXP;
		} //end get
		set
		{
			currentEXP = value;
		} //end set
	}//end CurrentEXP 

    /***************************************
     * Name: RemainingEXP
     ***************************************/
    public int RemainingEXP
    {
        get
        {
            return remainingEXP;
        } //end get
        set
        {
            remainingEXP = value;
        } //end set
    }//end RemainingEXP 

	/***************************************
     * Name: EXPForLevel
     ***************************************/
	public int EXPForLevel
	{
		get
		{
			return expForLevel;
		} //end get
		set
		{
			expForLevel = value;
		} //end set
	}//end EXPForLevel 

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
     * Name: Status
     ***************************************/
	public int Status
	{
		get
		{
			return status;
		} //end get
		set
		{
			status = value;
		} //end set
	} //end Status

    /***************************************
     * Name: StatusCount
     ***************************************/
	public int StatusCount
	{
		get
		{
			return statusCount;
		} //end get
		set
		{
			statusCount = value;
		} //end set
	} //end StatusCount

	/***************************************
     * Name: GetVitamin
     ***************************************/
	public int GetVitamin(int index)
	{
		return vitamins [index];
	} //end GetVitamin(int index)

	/***************************************
     * Name: SetVitamin
     ***************************************/
	public void SetVitamin(int index, int value)
	{
		vitamins [index] = value;
	} //end SetVitamin(int index, int value)

    /***************************************
     * Name: BallUsed
     ***************************************/
	public int BallUsed
	{
		get
		{
			return ballUsed;
		} //end get
		set
		{
			ballUsed = value;
		} //end set
	} //end BallUsed

    /***************************************
     * Name: ObtainType
     ***************************************/
	public int ObtainType
	{
		get
		{
			return obtainType;
		} //end get
		set
		{
			obtainType = value;
		} //end set
	} //end ObtainType
    
    /***************************************
     * Name: ObtainFrom
     ***************************************/
    public int ObtainFrom
    {
        get
        {
            return obtainFrom;
        } //end get
        set
        {
            obtainFrom = value;
        } //end set
    } //end ObtainFrom

    /***************************************
     * Name: ObtainLevel
     ***************************************/
	public int ObtainLevel
	{
		get
		{
			return obtainLevel;
		} //end get
		set
		{
			obtainLevel = value;
		} //end set
	} //end ObtainLevel

    /***************************************
     * Name: Ability
     ***************************************/
	public int Ability
	{
		get
		{
			return ability;
		} //end get
		set
		{
			ability = value;
		} //end set
	} //end Ability 

	/***************************************
     * Name: AbilityOn
     ***************************************/
	public int AbilityOn
	{
		get
		{
			return abilityOn;
		} //end get
		set
		{
			abilityOn = value;
		} //end set
	} //end AbilityOn 

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
     * Name: Nature
     ***************************************/
	public int Nature
	{
		get
		{
			return nature;
		} //end get
		set
		{
			nature = value;
		} //end set
	} //end Nature

    /***************************************
     * Name: Happiness
     ***************************************/
	public int Happiness
	{
		get
		{
			return happiness;
		} //end get
		set
		{
			happiness = ExtensionMethods.WithinIntRange(value, 0, 255);
		} //end set
	} //end Happiness

	/***************************************
     * Name: FormNumber
     ***************************************/
	public int FormNumber
	{
		get
		{
			return formNumber;
		} //end get
		set
		{
			formNumber = value;
		} //end set
	} //end Happiness

    /***************************************
     * Name: GetMove
     ***************************************/
	public int GetMove(int index)
	{
		return moves [index];
	} //end GetMove(int index)

    /***************************************
     * Name: GetMoveCount
     ***************************************/
    public int GetMoveCount()
    {
        return moves.Count(x => x >= 0);
    } //end GetMoveCount

    /***************************************
     * Name: GetFirstMove
     ***************************************/
	public int GetFirstMove(int index)
	{
		return firstMoves [index];
	} //end GetFirstMove(int index)
	
    /***************************************
     * Name: SetFirstMove
     ***************************************/
	public void SetFirstMove(int index, int value)
	{
		firstMoves [index] = value;
	} //end SetFirstMove(int index, int value)

    /***************************************
     * Name: GetMovePP
     ***************************************/
    public int GetMovePP(int index)
    {
        return ppReamaining [index];
    } //end GetMovePP(int index)
    
    /***************************************
     * Name: SetMovePP
     ***************************************/
    public void SetMovePP(int index, int value)
    {
        ppReamaining [index] = value;
    } //end SetMovePP(int index, int value)

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
     * Name: GetMovePPUps
     ***************************************/
	public int GetMovePPUps(int index)
	{
		return ppUps [index];
	} //end GetMovePPUps(int index)

	/***************************************
     * Name: SetMovePPUps
     ***************************************/
	public void SetMovePPUps(int index, int value)
	{
		ppUps [index] = value;
	} //end SetMovePPUps(int index, int value)

	/***************************************
     * Name: Type1
     ***************************************/
	public int Type1
	{
		get
		{
			return type1;
		} //end get
	} //end Type1

	/***************************************
     * Name: Type2
     ***************************************/
	public int Type2
	{
		get
		{
			return type2;
		} //end get
	} //end Type2

	/***************************************
     * Name: ItemRecycle
     ***************************************/
	public int ItemRecycle
	{
		get
		{
			return itemRecycle;
		} //end get
		set
		{
			itemRecycle = value;
		} //end set
	} //end ItemRecycle

    /***************************************
     * Name: HasPokerus
     ***************************************/
	public bool HasPokerus
	{
		get
		{
			return hasPokerus;
		} //end get
		set
		{
			hasPokerus = value;
		} //end set
	} //end HasPokerus

    /***************************************
     * Name: IsShiny
     ***************************************/
	public bool IsShiny
	{
		get
		{
			return isShiny;
		} //end get
		set
		{
			isShiny = value;
		} //end set
	} //end IsShiny

    /***************************************
     * Name: GetMarkings
     ***************************************/
    public bool[] GetMarkings()
    {
        return markings;
    } //end GetMarkings

    /***************************************
     * Name: SetMarkings
     ***************************************/
    public void SetMarkings(bool[] newMarkings)
    {
        markings = newMarkings; 
    } //end SetMarkings(bool[] newMarkings)

    /***************************************
     * Name: GetRibbon
     ***************************************/
	public int GetRibbon(int index)
	{
		return ribbons [index];
	} //end GetRibbon(int index)

    /***************************************
     * Name: GetRibbonCount
     ***************************************/
    public int GetRibbonCount()
    {
        return ribbons.Count;
    } //end GetRibbonCount()

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
     * Name: OTName
     ***************************************/
	public string OTName
	{
		get
		{
			return OtName;
		} //end get
		set
		{
			OtName = value;
		} //end set
	} //end OTName

    /***************************************
     * Name: ObtainTime
     ***************************************/
    public DateTime ObtainTime
    {
        get
        {
            return obtainTime;
        } //end get
        set
        {
            obtainTime = value;
        } //end set
    } //end ObtainTime
	#endregion
    #endregion
} //end Pokemon class
