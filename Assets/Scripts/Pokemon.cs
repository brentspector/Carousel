using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public class Pokemon
{
	//Stats
	int currentHP;			//Current HP
	int totalHP;			//Max HP
	int attack;				//Standard attack
	int defense;			//Standard defense
	int specialA;			//Standard special attack
	int specialD;			//Standard special defense
	int speed;				//Standard speed
    int totalEV;            //Total amount of EVs the pokemon currently has
	int[] IV;				//Individual values array
	int[] EV;				//Effort values array

	//Information
	int natSpecies;			//National species number
	int personalID;			//Defines a specific pokemon
	int trainerID;			//ID of trainer who obtained it first
	int currentEXP;			//Current EXP of pokemon
	int item;				//What item is being held
	int status;				//What status the pokemon is under
	int statusCount;		//Turns until awaken/turns of toxic
	int ballUsed;			//What pokeball this pokemon is in
	int obtainType;			//How this pokemon was obtained
	int obtainLevel;		//What level the pokemon was obtained at
	int ability;			//What ability this pokemon is currently on
	int gender;				//What gender the pokemon is
	int nature;				//What nature the pokemon has
	int happiness;			//Happiness level of pokemon
	int[] moves;			//Move roster of pokemon
	int[] firstMoves;		//The moves this pokemon first knew when obtained
	bool hasPokerus;		//Whether pokemon has pokerus
	bool isShiny;			//Whether pokemon is shiny
	bool[] markings;		//What markings this pokemon has
	bool[] ribbons;			//What ribbons have been obtained
	string nickname;		//Nickname of pokemon
	string obtainFrom;		//Where this pokemon was obtained from
	string OtName;			//Name of the original trainer

	//Create a random pokemon
	public Pokemon(int species = 0, int tID = 0, int level = 5, int item = 0, int ball = 0, 
	               int oType = 0, int oLevel = 5, int ability = 0, int gender = 0,
	               int nature = 0, int happy = 0, bool pokerus = false, bool shiny = false)
	{
		//Set data fields
		if(species == 0)
		{
			natSpecies = Random.Range(1, 721);
		} //end if
		else
		{
			natSpecies = species;
		} //end else
        currentHP = 0;
        totalHP = 0;
        attack = 0;
        defense = 0;
        specialA = 0;
        specialD = 0;
        speed = 0;
        totalEV = 0;
		trainerID = tID;
		currentEXP = CalculateEXP (level);
		Item = item;
		ballUsed = ball;
		obtainType = oType;
		obtainLevel = oLevel;
		Ability = ability;
		Gender = gender;
		Nature = nature;
		happiness = happy;
		hasPokerus = pokerus;
		isShiny = shiny;

		//Initialize arrays
		IV = new int[6];
		EV = new int[6];
		moves = new int[4];
		firstMoves = new int[4];
		markings = new bool[GameManager.instance.NumberOfMarkings];
		ribbons = new bool[GameManager.instance.NumberOfRibbons];

		for(int i = 0; i < 6; i++)
		{
			IV[i] = 0;
			EV[i] = 0;
		} //end for

		for(int i = 0; i < 4; i++)
		{
			moves[i] = 0;
			firstMoves[i] = 0;
		} //end for

		for(int i = 0; i < markings.Length; i++)
		{
			markings[i] = false;
		} //end for

		for(int i = 0; i < ribbons.Length; i++)
		{
			ribbons[i] = false;
		} //end for
	} //end Pokemon constructor

	//Change IVs of pokemon
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
                    IV[index] = Random.Range(0, 31);
                } //end if
                else
                {
                    //If not, fill with random IVs
                    for (int i = 0; i < 6; i++)
                    {
                        IV [i] = Random.Range(0, 31);
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
                for(int i = values.Length-1; i < 6; i++)
                {
                    values[i] = Random.Range(0, 31);
                } //end for
            } //end if

            //Fill in values
            for(int i = 0; i < 6; i++)
            {
                //Make sure value is valid
                if(values[i] < 0 || values[i] > 31)
                {
                    IV[i] = Random.Range(0, 31);
                } //end if
                else
                {
                    IV[i] = values[i];
                } //end else
            } //end for
        } //end else
	} //end ChangeIVs(int[] values, int index = -1)

    //Change EVs of pokemon
    /* NOTE: Total EVs allowed is 510. Any given value must 
     * adhere to that requirement or the change will be
     * denied. If only 1 value is provided, all EVs will be
     * set to that value, up to the maximum, starting with HP.
     * If a value is less than 0, a random EV will be given for
     * that EV, or all EVs will be given a random amount should
     * only 1 value be provided. If the value is greater than
     * 255, a standard amount of 85 will be applied. If an 
     * invalid index is provided the EV will default to standard 
     * non-index method.
     */
    public void ChangeEVs(int[] values, int index = -1)
    {
        //1 value provided
        if (values.Length == 1)
        {
            //If the value is invalid
            if(values[0] < 0)
            {
                //If an index is provided
                if(index > -1 && index < 6)
                {
                    //Make sure value is within maximum value
                    int tempCount = totalEV - EV[index];
                    int tempRand = Random.Range(0, 255);
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
                        int tempRand = Random.Range(0, 255);

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
                for(int i = values.Length - 1; i < 6; i++)
                {
                    values[i] = 0;
                } //end for
            } //end if

            //Fill in given values
            for (int i = 0; i < 6; i++)
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
            } //end if
        } //end else
    } //end ChangeEVs(int[] values, int index = -1)

    //Change moves of pokemon
    /* NOTE: Moves are replaced from front to back. Thus providing
     * only 1 value will only replace the first moveslot. Provide
     * a valid index number to change that specific moveslot
     * 
     */
    public void ChangeMoves(int[] values, int index = -1)
    {
        //If a valid index is provided
        if (index > -1 && index < 4)
        {
            moves[index] = values[0];
        } //end if
        //No valid index given
        else
        {
            for(int i = 0; i < values.Length; i++)
            {
                moves[i] = values[i];
            } //end for
        } //end else
    } //end ChangeMoves(int[] values, int index = -1)

    //Given initial moves, including any special moves to be saved under first moves
    public void GiveInitialMoves(int[] values)
    {
        //Fill in missing values
        if (values.Length < 4)
        {
            for(int i = values.Length - 1; i < 4; i++)
            {
                values[i] = 0; 
            } //end for
        } //end if

        //Loop through and set moves
        for(int i = 0; i < 4; i++)
        {
            moves[i] = values[i];
            firstMoves[i] = values[i];
        } //end for
    } //end GiveInitialMoves(int[] moves)

    //Change markings
    public void ChangeMarkings(bool value, int index)
    {
        //Make sure index is valid
        if (index > -1 && index < markings.Length)
        {
            markings[index] = value;
        } //end if
    } //end ChangeMarkings(bool value, int index)

    //Changes ribbons
    public void ChangeRibbons(bool value, int index)
    {
        //Make sure index is valid
        if (index > -1 && index < ribbons.Length)
        {
            ribbons[index] = value;
        } //end if
    } //end ChangeRibbons(bool value, int index)

    //Test funtion to varify values are saved
	public string GetValues()
	{
		string result;

		result = "CurrentHP: " + currentHP;
		result += "\nMaxHP: " + totalHP;
		result += "\nAttack: " + attack;
		result += "\nDefense: " + defense;
		result += "\nSpA: " + specialA;
		result += "\nSpD: " + specialD;
		result += "\nSpeed: " + speed;
		result += "IVs: ";
		foreach (int number in IV) {
			result += number + ", ";
		}
		result += "\nEVs: ";
		foreach (int number in EV) {
			result += number + ", ";
		}

		return result;
	} //end GetValues

	int CalculateEXP(int level)
	{
        return 0;
	} //end CalculateEXP(int level)

	#region Accessors
	//Stats
	public int CurrentHP
	{
		get
		{
			return currentHP;
		} //end get
		set
		{
			currentHP = value;
		} //end set
	}//end Current HP

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

	public int GetIV(int index)
	{
		return IV [index];
	} //end GetIV(int index)

	public void SetIV(int index, int value)
	{
		IV [index] = value;
	} //end SetIV(int index, int value)

	public int GetEV(int index)
	{
		return EV [index];
	} //end GetEV(int index)

	public void SetEV(int index, int value)
	{
		EV [index] = value;
	} //end SetIV(int index, int value)

	//Information
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

	public int PersonalID
	{
		get
		{
			return personalID;
		} //end get
		set
		{
			personalID = value;
		} //end set
	} //end PersonalID

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

	public int Happiness
	{
		get
		{
			return happiness;
		} //end get
		set
		{
			happiness = value;
		} //end set
	} //end Happiness

	public int GetMove(int index)
	{
		return moves [index];
	} //end GetMove(int index)

	public void SetMove(int index, int value)
	{
		moves [index] = value;
	} //end SetMove(int index, int value)

	public int GetFirstMove(int index)
	{
		return firstMoves [index];
	} //end GetFirstMove(int index)
	
	public void SetFirstMove(int index, int value)
	{
		firstMoves [index] = value;
	} //end SetFirstMove(int index, int value)

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
	} //end HasPokerus

	public bool GetMarking(int index)
	{
		return markings [index];
	} //end GetMarkings(int index)

	public void SetMarking(int index, bool value)
	{
		markings [index] = value;
	} //end SetMarking(int index, bool value)

	public bool GetRibbon(int index)
	{
		return ribbons [index];
	} //end GetRibbon(int index)

	public void SetRibbon(int index, bool value)
	{
		ribbons [index] = value;
	} //end SetRibbon(int index, bool value)

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

	public string ObtainFrom
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
	#endregion
} //end Pokemon class
