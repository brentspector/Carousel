/***************************************************************************************** 
 * File:    DataContents.cs
 * Summary: Holds all databases for reference in-game. 
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.SqliteClient;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Linq;
#endregion

public static class DataContents : System.Object 
{
    #region Variables
    public static ExperienceTable experienceTable;  //Holds experience values for growth rates
    public static RibbonData ribbonData;            //Holds names and description data for ribbons
    public static TypeChart typeChart;              //Holds the type matchups
    public static char[] markingCharacters;         //Holds the characters you can mark with
    static int speciesCount;                        //Number of entries in Pokemon table
    static int moveCount;                           //Number of entries in Move table
    static int itemCount;                           //Number of entries in Items table
    static int abilityCount;                        //Number of entries in Abilities table

    //Sprite Arrays
    public static Sprite[] statusSprites;           //Sprites for each status ailment
    public static Sprite[] typeSprites;             //Sprites for each type
    public static Sprite[] categorySprites;         //Sprites for each category of move
    public static Sprite[] ribbonSprites;           //Sprites for each ribbon
    public static Sprite[] badgeSprites;            //Sprites for each badge
    public static Sprite[] trainerCardSprites;      //Sprites for full trainer for trainer card
	public static Sprite[] trainerBacks;			//Sprites for back of trainers for battle scene
	public static Sprite[] versusImages;			//Sprites for Versus images
	public static Sprite[] leaderSprites;			//Sprites for leaders in battle
	public static Sprite[] attackNonSelSprites;		//Sprites for attack backgrounds that are not selected
	public static Sprite[] attackSelSprites;		//Sprites for attack backgrounds that are selected

    //Shorthand for main data path
    static string dataLocation;                     

    //SQL Variables
    static string dbPath; 
    static IDbConnection dbConnection;              
    static IDbCommand dbCommand;
    static IDataReader dbReader;
    #endregion

    #region Methods
    /***************************************
     * Name: InitDataContents
     * Establishes SQL connection and loads experience table
     ***************************************/
    public static bool InitDataContents()
    {
        //Initalize data location
        //PossibleToDo: http://wiki.unity3d.com/index.php/Folder_Paths_Win_Mac
        //Find best place to save to for Windows and Mac
        #if UNITY_EDITOR
        dataLocation = Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/";
        #else
        //Gets the current folder location and sets path to where the binary files are
        dataLocation = Environment.CurrentDirectory + "/PokemonCarousel_Data/";
        #endif  

        //Setup SQL Database variables
        dbPath = "URI=file:" + dataLocation + "/Supplimental.db";
        dbConnection=new SqliteConnection(dbPath);
        dbConnection.Open();
        dbCommand=dbConnection.CreateCommand();

        //Verify SQL table loaded correctly
        dbCommand.CommandText = "SELECT Count(*) FROM sqlite_master WHERE type='table'";
        dbReader = dbCommand.ExecuteReader ();
        int returnValue = 0;
        while (dbReader.Read())
        {
            returnValue = dbReader.GetInt32(0);
        } //end while
        if (returnValue == 0)
        {
            return false;
        } //end if

        //Initialize markings
        markingCharacters = new char[] {'●','■','▲','♥','♦','☻'};

        //Initialize max count for each table
        speciesCount = ExecuteSQL<int>("SELECT Count(*) FROM Pokemon");
        moveCount = ExecuteSQL<int>("SELECT Count(*) FROM Moves");
        itemCount = ExecuteSQL<int>("SELECT Count(*) FROM Items");
        abilityCount = ExecuteSQL<int> ("SELECT Count(*) FROM Abilities");

        //Create an experience table
        experienceTable = new ExperienceTable();

        //Create ribbon data
        ribbonData = new RibbonData ();

        //Create a type chart
        typeChart = new TypeChart ();

        //Load sprites
        statusSprites       = Resources.LoadAll<Sprite> ("Sprites/Icons/statuses");
        typeSprites         = Resources.LoadAll<Sprite> ("Sprites/Icons/pokedexTypes");
        categorySprites     = Resources.LoadAll<Sprite> ("Sprites/Icons/category");
        ribbonSprites       = Resources.LoadAll<Sprite> ("Sprites/Icons/ribbons");
        badgeSprites        = Resources.LoadAll<Sprite> ("Sprites/Icons/Badges");
        trainerCardSprites  = Resources.LoadAll<Sprite> ("Sprites/Menus/FullTrainers");
		trainerBacks        = Resources.LoadAll<Sprite>("Sprites/Battle/TrainerBack");
		versusImages 		= Resources.LoadAll<Sprite>("Sprites/Battle/Leaders");
		leaderSprites 		= Resources.LoadAll<Sprite>("Sprites/Battle/FullLeaders");
		attackNonSelSprites	= Resources.LoadAll<Sprite>("Sprites/Battle/battleFightButtons");
		attackSelSprites 	= Resources.LoadAll<Sprite>("Sprites/Battle/battleFightButtonsSelect");

        return true;
    } //end InitDataContents()

    /***************************************
     * Name: ExecuteSQL
     * Runs SQL query 
     ***************************************/
    public static T ExecuteSQL<T> (string query)
    {
        //Run the query
        dbCommand.CommandText = query;
        dbReader = dbCommand.ExecuteReader ();

        //Store the result of the query
        object value = null;
        while (dbReader.Read())
        {
            value = dbReader.GetValue (0);
        } //end while

        //Verify non-null, or set to default
        if(value == null)
        {
            value = default(T);
        } //end if

        //Return the information in the type requested
        return (T)Convert.ChangeType (value, typeof(T));
    } //end ExecuteSQL<T> (string query)

    /***************************************
     * Name: GetLevel
     * Returns level according to current xp and growth rate
     ***************************************/
    public static int GetLevel(int exp, string growth)
    {
        return experienceTable.GetLevel (growth, exp);
    } //end GetLevel(int exp, string growth)

    /***************************************
     * Name: GeneratePokemonList
     * Returns a list of strings containing
     * each name of the pokemon in the table
     ***************************************/
    public static List<string> GeneratePokemonList()
    {
        List<string> nameList = new List<string> ();
        for (int i = 1; i < speciesCount+1; i++)
        {
            nameList.Add (ExecuteSQL<string> ("SELECT name FROM Pokemon WHERE rowid=" + i));
        } //end for
        return nameList;
    } //end GeneratePokemonList()

    /***************************************
     * Name: GenerateMoveList
     * Returns a list of strings containing
     * each name of the moves in the table
     ***************************************/
    public static List<string> GenerateMoveList()
    {
        List<string> nameList = new List<string> ();
		nameList.Add("---");
        for (int i = 1; i < moveCount+1; i++)
        {
            nameList.Add (ExecuteSQL<string> ("SELECT gameName FROM Moves WHERE rowid=" + i));
        } //end for
        return nameList;
    } //end GenerateMoveList()

    /***************************************
     * Name: GenerateAbilityList
     * Returns a list of strings containing
     * each name of the abilities in the table
     ***************************************/
    public static List<string> GenerateAbilityList()
    {
        List<string> nameList = new List<string> ();
        for (int i = 1; i < abilityCount+1; i++)
        {
            nameList.Add (ExecuteSQL<string> ("SELECT gameName FROM Abilities WHERE rowid=" + i));
        } //end for
        return nameList;
    } //end GenerateAbilityList()

    /***************************************
     * Name: GenerateItemList
     * Returns a list of strings containing
     * each name of the items in the table
     ***************************************/
    public static List<string> GenerateItemList()
    {
        List<string> nameList = new List<string> ();
		nameList.Add("No Item");
        for (int i = 1; i < itemCount+1; i++)
        {
            nameList.Add (ExecuteSQL<string> ("SELECT gameName FROM Items WHERE rowid=" + i));
        } //end for
        return nameList;
    } //end GenerateItemList()

    /***************************************
     * Name: GetMoveID
     * Returns numeric location of pokemon attack
     ***************************************/
    public static int GetMoveID(string moveName)
    {
        //Search moves for move location
        dbCommand.CommandText = "SELECT rowid FROM Moves WHERE internalName=@nm";
        dbCommand.Parameters.Add(new SqliteParameter("@nm", moveName));
        dbCommand.Prepare ();
        int moveID = ExecuteSQL<int> (dbCommand.CommandText);
        dbCommand.Parameters.Clear ();

        //Return location of attack, or -1 if not found
        return moveID;
    } //end GetMoveID(string moveName)

    /***************************************
     * Name: GetMoveGameName
     * Returns string name of given numeric location
     ***************************************/
    public static string GetMoveGameName(int moveNumber)
    {
        //Return NULL if a number goes beyond list boundaries
        if (moveNumber < 1 || moveNumber > moveCount)
        {
            return "NULL";
        } //end if
        //Return name of attack at numeric location
        else
        {
            string moveName = ExecuteSQL<string>("SELECT gameName FROM Moves WHERE rowid=" + moveNumber);
            return moveName;
        } //end else
    } //end GetMoveGameName(int moveNumber)

	/***************************************
     * Name: GetMoveIcon
     * Returns numeric sprite location for 
     * move given
     ***************************************/
	public static int GetMoveIcon(int moveNumber)
	{		
		return moveNumber < 0 ? -1 : Convert.ToInt32(Enum.Parse(typeof(Types), 
			DataContents.ExecuteSQL<string>("SELECT type FROM Moves WHERE rowid=" + moveNumber)));
	} //end GetMoveIcon(int moveNumber)

	/***************************************
     * Name: GetMovePriority
     * Returns priority of given move
     ***************************************/
	public static int GetMovePriority(int moveNumber)
	{		
		return moveNumber < 0 ? -1 : ExecuteSQL<int>("SELECT priority FROM Moves WHERE rowid=" + moveNumber);
	} //end GetMovePriority(int moveNumber)

	/***************************************
     * Name: GetMoveDamage
     * Returns base damage of given move
     ***************************************/
	public static int GetMoveDamage(int moveNumber)
	{		
		return moveNumber < 0 ? -1 : ExecuteSQL<int>("SELECT baseDamage FROM Moves WHERE rowid=" + moveNumber);
	} //end GetMoveAccuracy(int moveNumber)

	/***************************************
     * Name: GetMoveAccuracy
     * Returns base accuracy of given move
     ***************************************/
	public static int GetMoveAccuracy(int moveNumber)
	{		
		return moveNumber < 0 ? -1 : ExecuteSQL<int>("SELECT accuracy FROM Moves WHERE rowid=" + moveNumber);
	} //end GetMoveAccuracy(int moveNumber)

	/***************************************
     * Name: GetMoveFlag
     * Returns if a flag exists
     ***************************************/
	public static bool GetMoveFlag(int moveNumber, string flag)
	{		
		return ExecuteSQL<string>("SELECT flags FROM Moves WHERE rowid=" + moveNumber).Contains(flag);
	} //end GetMoveFlag(int moveNumber, string flag)

	/***************************************
     * Name: GetMoveCategory
     * Returns numeric sprite location for 
     * move given
     ***************************************/
	public static int GetMoveCategory(int moveNumber)
	{
		return Convert.ToInt32(Enum.Parse(typeof(Categories),
			DataContents.ExecuteSQL<string>("SELECT category FROM Moves WHERE rowid=" +
			moveNumber)));
	} //end GetMoveCategory(int moveNumber)

    /***************************************
     * Name: GetItemID
     * Returns numeric location of item
     ***************************************/
    public static int GetItemID(string itemName)
    {
        //No item found yet
        dbCommand.CommandText = "SELECT rowid FROM Items WHERE gameName=@nm";
        dbCommand.Parameters.Add(new SqliteParameter("@nm", itemName));
        dbCommand.Prepare ();
        int itemID = ExecuteSQL<int> (dbCommand.CommandText);
        dbCommand.Parameters.Clear ();
        
        //Return location of item, or -1 if not found
        return itemID;
    } //end GetItemID(string itemName)
    
    /***************************************
     * Name: GetItemGameName
     * Returns string name of given numeric 
     * location
     ***************************************/
    public static string GetItemGameName(int itemNumber)
    {
        //Return NULL if a number goes beyond list boundaries
        if (itemNumber < 1 || itemNumber > itemCount)
        {
            return "None";
        } //end if
        //Return name of item at numeric location
        else
        {
            string itemName = ExecuteSQL<string>("SELECT gameName FROM Items WHERE rowid=" + itemNumber);
            return itemName;
        } //end else
    } //end GetItemGameName(int itemNumber)

	/***************************************
     * Name: GetItemBagSlot
     * Returns item's bag spot
     ***************************************/
	public static int GetItemBagSpot(int itemNumber)
	{
		//Return NULL if a number goes beyond list boundaries
		if (itemNumber < 1 || itemNumber > itemCount)
		{
			return 0;
		} //end if
		//Return item bag spot 
		else
		{
			int itemLocation = ExecuteSQL<int>("SELECT bagNumber FROM Items WHERE rowid=" + itemNumber);
			return itemLocation;
		} //end else
	} //end GetItemBagSpot(int itemNumber)

	/***************************************
     * Name: GetPokemonID
     * Returns numeric location of Pokemon
     ***************************************/
	public static int GetPokemonID(string pokemonName)
	{
		//No pokemon found yet
		pokemonName = pokemonName.ToLower();
		char[] letters = pokemonName.ToCharArray();
		pokemonName = letters[0].ToString().ToUpper() + pokemonName.Remove(0, 1);
		dbCommand.CommandText = "SELECT rowid FROM Pokemon WHERE name=@nm";
		dbCommand.Parameters.Add(new SqliteParameter("@nm", pokemonName));
		dbCommand.Prepare ();
		int pokemonID = ExecuteSQL<int> (dbCommand.CommandText);
		dbCommand.Parameters.Clear ();

		//Return location of pokemon, or -1 if not found
		return pokemonID;
	} //end GetPokemonID(string pokemonName)
    #endregion
} //end DataContents class

/***************************************************************************************** 
 * Class: ExperienceTable
 * Summary: Lists the xp for each level that a pokemon needs
 *****************************************************************************************/ 
[Serializable]
public class ExperienceTable
{
    /***************************************
     * Name: Medium
     * Contains array of experience at each level of
     * a pokemon with the Medium experience rate
     ***************************************/
    int[] Medium = 
    {
        -1,0,8,27,64,125,216,343,512,729,
        1000,1331,1728,2197,2744,3375,4096,4913,5832,6859,
        8000,9261,10648,12167,13824,15625,17576,19683,21952,24389,
        27000,29791,32768,35937,39304,42875,46656,50653,54872,59319,
        64000,68921,74088,79507,85184,91125,97336,103823,110592,117649,
        125000,132651,140608,148877,157464,166375,175616,185193,195112,205379,
        216000,226981,238328,250047,262144,274625,287496,300763,314432,328509,
        343000,357911,373248,389017,405224,421875,438976,456533,474552,493039,
        512000,531441,551368,571787,592704,614125,636056,658503,681472,704969,
        729000,753571,778688,804357,830584,857375,884736,912673,941192,970299,
        1000000
    };

    /***************************************
     * Name: Erratic
     * Contains array of experience at each level of
     * a pokemon with the Erractic experience rate
     ***************************************/
    int[] Erratic =
    {
        -1,0,15,52,122,237,406,637,942,1326,
        1800,2369,3041,3822,4719,5737,6881,8155,9564,11111,
        12800,14632,16610,18737,21012,23437,26012,28737,31610,34632,
        37800,41111,44564,48155,51881,55737,59719,63822,68041,72369,
        76800,81326,85942,90637,95406,100237,105122,110052,115015,120001,
        125000,131324,137795,144410,151165,158056,165079,172229,179503,186894,
        194400,202013,209728,217540,225443,233431,241496,249633,257834,267406,
        276458,286328,296358,305767,316074,326531,336255,346965,357812,367807,
        378880,390077,400293,411686,423190,433572,445239,457001,467489,479378,
        491346,501878,513934,526049,536557,548720,560922,571333,583539,591882,
        600000
    };

    /***************************************
     * Name: Fluctuating
     * Contains array of experience at each level of
     * a pokemon with the Fluctuating experience rate
     ***************************************/
    int[] Fluctuating =
    {        
        -1,0,4,13,32,65,112,178,276,393,
        540,745,967,1230,1591,1957,2457,3046,3732,4526,
        5440,6482,7666,9003,10506,12187,14060,16140,18439,20974,
        23760,26811,30146,33780,37731,42017,46656,50653,55969,60505,
        66560,71677,78533,84277,91998,98415,107069,114205,123863,131766,
        142500,151222,163105,172697,185807,196322,210739,222231,238036,250562,
        267840,281456,300293,315059,335544,351520,373744,390991,415050,433631,
        459620,479600,507617,529063,559209,582187,614566,639146,673863,700115,
        737280,765275,804997,834809,877201,908905,954084,987754,1035837,1071552,
        1122660,1160499,1214753,1254796,1312322,1354652,1415577,1460276,1524731,1571884,
        1640000
    };

    /***************************************
     * Name: Parabolic
     * Contains array of experience at each level of
     * a pokemon with the Parabolic experience rate
     ***************************************/
    int[] Parabolic =
    {
        -1,0,9,57,96,135,179,236,314,419,
        560,742,973,1261,1612,2035,2535,3120,3798,4575,
        5460,6458,7577,8825,10208,11735,13411,15244,17242,19411,
        21760,24294,27021,29949,33084,36435,40007,43808,47846,52127,
        56660,61450,66505,71833,77440,83335,89523,96012,102810,109923,
        117360,125126,133229,141677,150476,159635,169159,179056,189334,199999,
        211060,222522,234393,246681,259392,272535,286115,300140,314618,329555,
        344960,360838,377197,394045,411388,429235,447591,466464,485862,505791,
        526260,547274,568841,590969,613664,636935,660787,685228,710266,735907,
        762160,789030,816525,844653,873420,902835,932903,963632,995030,1027103,
        1059860
    };

    /***************************************
     * Name: Fast
     * Contains array of experience at each level of
     * a pokemon with the Fast experience rate
     ***************************************/
    int[] Fast =
    {
        -1,0,6,21,51,100,172,274,409,583,
        800,1064,1382,1757,2195,2700,3276,3930,4665,5487,
        6400,7408,8518,9733,11059,12500,14060,15746,17561,19511,
        21600,23832,26214,28749,31443,34300,37324,40522,43897,47455,
        51200,55136,59270,63605,68147,72900,77868,83058,88473,94119,
        100000,106120,112486,119101,125971,133100,140492,148154,156089,164303,
        172800,181584,190662,200037,209715,219700,229996,240610,251545,262807,
        274400,286328,298598,311213,324179,337500,351180,365226,379641,394431,
        409600,425152,441094,457429,474163,491300,508844,526802,545177,563975,
        583200,602856,622950,643485,664467,685900,707788,730138,752953,776239,
        800000
    };

    /***************************************
     * Name: Slow
     * Contains array of experience at each level of
     * a pokemon with the Slow experience rate
     ***************************************/
    int[] Slow =
    {
        -1,0,10,33,80,156,270,428,640,911,
        1250,1663,2160,2746,3430,4218,5120,6141,7290,8573,
        10000,11576,13310,15208,17280,19531,21970,24603,27440,30486,
        33750,37238,40960,44921,49130,53593,58320,63316,68590,74148,
        80000,86151,92610,99383,106480,113906,121670,129778,138240,147061,
        156250,165813,175760,186096,196830,207968,219520,231491,243890,256723,
        270000,283726,297910,312558,327680,343281,359370,375953,393040,410636,
        428750,447388,466560,486271,506530,527343,548720,570666,593190,616298,
        640000,664301,689210,714733,740880,767656,795070,823128,851840,881211,
        911250,941963,973360,1005446,1038230,1071718,1105920,1140841,1176490,1212873,
        1250000
    };

    /***************************************
     * Name: GetCurrentValue
     * Returns how much experience the pokemon should have
     * at the given level
     ***************************************/
    public int GetCurrentValue(string experienceRate, int level)
    {
        if (experienceRate == "Medium")
        {
            return Medium [level];
        } //end if
        else if (experienceRate == "Erratic")
        {
            return Erratic [level];
        } //end else if
        else if (experienceRate == "Fluctuating")
        {
            return Fluctuating [level];
        } //end else if
        else if (experienceRate == "Parabolic")
        {
            return Parabolic [level];
        } //end else if
        else if (experienceRate == "Fast")
        {
            return Fast [level];
        } //end else if
        else if (experienceRate == "Slow")
        {
            return Slow [level];
        } //end else if
        else
        {
            GameManager.instance.LogErrorMessage("Error finding " + level + " in group " 
                                                 + experienceRate);
            return -1;
        } //end else
    } //end GetCurrentValue(string experienceRate, int level)

    /***************************************
     * Name: GetNextValue
     * Returns how much experience is required for 
     * the next level
     ***************************************/
    public int GetNextValue(string experienceRate, int level)
    {
        if (level == 100)
        {
            return 0;
        } //end if
        else if (experienceRate == "Medium")
        {
            return Medium [level + 1];
        } //end else if
        else if (experienceRate == "Erratic")
        {
            return Erratic [level + 1];
        } //end else if
        else if (experienceRate == "Fluctuating")
        {
            return Fluctuating [level + 1];
        } //end else if
        else if (experienceRate == "Parabolic")
        {
            return Parabolic [level + 1];
        } //end else if
        else if (experienceRate == "Fast")
        {
            return Fast [level + 1];
        } //end else if
        else if (experienceRate == "Slow")
        {
            return Slow [level + 1];
        } //end else if
        else
        {
            GameManager.instance.LogErrorMessage("Error finding " + (level + 1) + " in group " 
                                                 + experienceRate);
            return -1;
        } //end else
    } //end GetNextValue(string experienceRate, int level)

    /***************************************
     * Name: GetLevel
     * Returns the level for the given experience
     * THIS IS A FALLBACK FUNCTION
     ***************************************/
    public int GetLevel(string experienceRate, int experience)
    {
        //Level for reference
        int level = -1;

        //Find level based on rate
        if (experienceRate == "Medium")
        {
            //Make sure experience isn't higher than top amount
            if(experience > Medium[100])
            {
                level = 100;
            } //end if
            else
            {
                //Go through all experience
                for(int i = 0; i < 101; i++)
                {
                    //Return level 
                    if(Medium[i] > experience)
                    {
                        level = i-1;
                        break;
                    } //end if
                } //end for
            } //end else
        } //end if
        else if (experienceRate == "Erratic")
        {
            //Make sure experience isn't higher than top amount
            if(experience >= Erratic[100])
            {
                level = 100;
            } //end if
            else
            {
                //Go through all experience
                for(int i = 0; i < 101; i++)
                {
                    //Return level 
                    if(Erratic[i] > experience)
                    {
                        level = i-1;
                        break;
                    } //end if
                } //end for
            } //end else
        } //end else if
        else if (experienceRate == "Fluctuating")
        {
            //Make sure experience isn't higher than top amount
            if(experience >= Fluctuating[100])
            {
                level = 100;
            } //end if
            else
            {
                //Go through all experience
                for(int i = 0; i < 101; i++)
                {
                    //Return level 
                    if(Fluctuating[i] > experience)
                    {
                        level = i-1;
                        break;
                    } //end if
                } //end for
            } //end else
        } //end else if
        else if (experienceRate == "Parabolic")
        {
            //Make sure experience isn't higher than top amount
            if(experience >= Fluctuating[100])
            {
                level = 100;
            } //end if
            else
            {
                //Go through all experience
                for(int i = 0; i < 101; i++)
                {
                    //Return level 
                    if(Fluctuating[i] > experience)
                    {
                        level = i-1;
                        break;
                    } //end if
                } //end for
            } //end else
        } //end else if
        else if (experienceRate == "Fast")
        {
            //Make sure experience isn't higher than top amount
            if(experience >= Fast[100])
            {
                level = 100;
            } //end if
            else
            {
                //Go through all experience
                for(int i = 0; i < 101; i++)
                {
                    //Return level 
                    if(Fast[i] > experience)
                    {
                        level = i-1;
                        break;
                    } //end if
                } //end for
            } //end else
        } //end else if
        else if (experienceRate == "Slow")
        {
            //Make sure experience isn't higher than top amount
            if(experience >= Slow[100])
            {
                level = 100;
            } //end if
            else
            {
                //Go through all experience
                for(int i = 0; i < 101; i++)
                {
                    //Return level 
                    if(Slow[i] > experience)
                    {
                        level = i-1;
                        break;
                    } //end if
                } //end for
            } //end else
        } //end else if
        else
        {
            GameManager.instance.LogErrorMessage("Error finding " + (experience) + " in group " 
                                                 + experienceRate);
        } //end else
        return level;
    } //end GetLevel(string experienceRate, int experience)
} //end ExperienceTable class

/***************************************************************************************** 
 * Class: TypeChart
 * Summary: Lists the type resistances and weaknesses
 *****************************************************************************************/ 
[Serializable]
public class TypeChart
{
    //Array of type matchups
    TypeMatch[] typeMatches;

    /***************************************
     * Name: TypeMatch
     * A struct for the basic type matchups
     ***************************************/
    struct TypeMatch
    {
        public int[] weaknesses;
        public int[] resistances;
        public int[] immunities;
    } //end struct TypeMatch
        
    /***************************************
     * Name: TypeChart
     * Initializes TypeMatch array when class
     * is created
     ***************************************/
    public TypeChart()
    {
        //Initialize type array
        typeMatches = new TypeMatch[19];

        //Set each array element
        //NORMAL  = 0
        typeMatches[0].weaknesses = new int[]{1};
        typeMatches[0].resistances = new int[]{};
        typeMatches[0].immunities = new int[]{7};
        //FIGHTING= 1
        typeMatches[1].weaknesses = new int[]{2,14,18};
        typeMatches[1].resistances = new int[]{5,6,17};
        typeMatches[1].immunities = new int[]{};
        //FLYING  = 2
        typeMatches[2].weaknesses = new int[]{5,13,15};
        typeMatches[2].resistances = new int[]{1,6,12};
        typeMatches[2].immunities = new int[]{4};
        //POISON  = 3
        typeMatches[3].weaknesses = new int[]{4,14};
        typeMatches[3].resistances = new int[]{1,3,6,12,18};
        typeMatches[3].immunities = new int[]{};
        //GROUND  = 4
        typeMatches[4].weaknesses = new int[]{11,12,15};
        typeMatches[4].resistances = new int[]{3,5};
        typeMatches[4].immunities = new int[]{13};
        //ROCK    = 5
        typeMatches[5].weaknesses = new int[]{1,4,8,11,12};
        typeMatches[5].resistances = new int[]{0,2,3,10};
        typeMatches[5].immunities = new int[]{};
        //BUG     = 6
        typeMatches[6].weaknesses = new int[]{2,5,10};
        typeMatches[6].resistances = new int[]{1,4,12};
        typeMatches[6].immunities = new int[]{};
        //GHOST   = 7
        typeMatches[7].weaknesses = new int[]{7,17};
        typeMatches[7].resistances = new int[]{3,6};
        typeMatches[7].immunities = new int[]{0,1};
        //STEEL   = 8
        typeMatches[8].weaknesses = new int[]{1,4,10};
        typeMatches[8].resistances = new int[]{0,2,5,6,8,12,14,15,16,18};
        typeMatches[8].immunities = new int[]{3};
        //UNKNOWN = 9
        typeMatches[9].weaknesses = new int[]{};
        typeMatches[9].resistances = new int[]{};
        typeMatches[9].immunities = new int[]{};
        //FIRE    = 10
        typeMatches[10].weaknesses = new int[]{4,5,11};
        typeMatches[10].resistances = new int[]{6,8,10,12,15,18};
        typeMatches[10].immunities = new int[]{};
        //WATER   = 11
        typeMatches[11].weaknesses = new int[]{12,13};
        typeMatches[11].resistances = new int[]{8,10,11,15};
        typeMatches[11].immunities = new int[]{};
        //GRASS   = 12
        typeMatches[12].weaknesses = new int[]{2,3,6,10,15};
        typeMatches[12].resistances = new int[]{4,11,12,13};
        typeMatches[12].immunities = new int[]{};
        //ELECTRIC= 13
        typeMatches[13].weaknesses = new int[]{4};
        typeMatches[13].resistances = new int[]{2,8,13};
        typeMatches[13].immunities = new int[]{};
        //PSYCHIC = 14
        typeMatches[14].weaknesses = new int[]{6,7,17};
        typeMatches[14].resistances = new int[]{1,14};
        typeMatches[14].immunities = new int[]{};
        //ICE     = 15
        typeMatches[15].weaknesses = new int[]{1,5,8,10};
        typeMatches[15].resistances = new int[]{15};
        typeMatches[15].immunities = new int[]{};
        //DRAGON  = 16
        typeMatches[16].weaknesses = new int[]{15,16,18};
        typeMatches[16].resistances = new int[]{10,11,12,13};
        typeMatches[16].immunities = new int[]{};
        //DARK    = 17
        typeMatches[17].weaknesses = new int[]{1,6,18};
        typeMatches[17].resistances = new int[]{7,17};
        typeMatches[17].immunities = new int[]{14};
        //FAIRY   = 18
        typeMatches[18].weaknesses = new int[]{3,8};
        typeMatches[18].resistances = new int[]{1,6,17};
        typeMatches[18].immunities = new int[]{16};
    } //end TypeChart()

    /***************************************
     * Name: DetermineWeaknesses
     * Returns list of weaknesses for given 
     * type(s)
     ***************************************/
    public List<int> DetermineWeaknesses(int type1, int type2=-1)
    {
        //Create list to store weaknesses in
        List<int> weakList = new List<int> ();

        //Add in all weaknesses of the first type
        weakList.AddRange (typeMatches [type1].weaknesses);

        //If a second type is given, remove resistances too
        if (type2 > -1)
        {
            //First add weaknesses of second type
            weakList.AddRange(typeMatches[type2].weaknesses);
         
            //Remove resistances of first type
            weakList = ExtensionMethods.RemoveRange(weakList, typeMatches[type1].resistances.ToList());

            //Remove resistances of the second type
            weakList = ExtensionMethods.RemoveRange(weakList, typeMatches[type2].resistances.ToList());

            //Remove immunities of the first type
            weakList.RemoveAll(item => typeMatches[type1].immunities.Contains(item));

            //Remove immunities of the second type
            weakList.RemoveAll(item => typeMatches[type2].immunities.Contains(item));
        } //end if

        return weakList;
    } //end DetermineWeaknesses(int type1, int type2=-1)

    /***************************************
     * Name: DetermineResistances
     * Returns list of resistances for given 
     * type(s)
     ***************************************/
    public List<int> DetermineResistances(int type1, int type2=-1)
    {
        //Create list to store resistances in
        List<int> resistList = new List<int> ();
        
        //Add in all resistances of the first type
        resistList.AddRange (typeMatches [type1].resistances);

        //If a second type is given, remove weaknesses too
        if (type2 > -1)
        {        
            //First add resistances of second type
            resistList.AddRange (typeMatches [type2].resistances);
            
            //Remove weaknesses of first type
            resistList = ExtensionMethods.RemoveRange (resistList, typeMatches [type1].weaknesses.ToList ());
            
            //Remove weaknesses of the second type
            resistList = ExtensionMethods.RemoveRange (resistList, typeMatches [type2].weaknesses.ToList ());

            //Add in all immunities of the first type
            resistList.AddRange (typeMatches [type1].immunities);

            //Add in all immunities of the second type
            resistList.AddRange (typeMatches [type2].immunities);
        } //end if
        else
        {
            //Add in all immunities of the first type
            resistList.AddRange (typeMatches [type1].immunities);
        } //end else

        return resistList;
    } //end DetermineWeaknesses(int type1, int type2=-1)

	/***************************************
     * Name: DetermineTyping
     * Returns the modifier the move will have
     * on the list of types
     ***************************************/
	public int DetermineTyping(int moveType, int type)
	{
		//Check if immune
		if (typeMatches[type].immunities.Contains(moveType))
		{
			return 0;
		} //end if

		//Check if weak
		else if (typeMatches[type].weaknesses.Contains(moveType))
		{
			return 2;
		} //end else if

		//Check if resists
		else if (typeMatches[type].resistances.Contains(moveType))
		{
			return -1;
		} //end else if

		return 1;
	} //end DetermineTyping(int moveType, int type)
} //end ExperienceTable class

/***************************************************************************************** 
 * Class: RibbonData
 * Summary: Lists the name and description of each ribbon
 *****************************************************************************************/ 
[Serializable]
public class RibbonData
{
    /***************************************
     * Name: RibbonNames
     * An array of ribbon names
     ***************************************/
    string[] RibbonNames =
    {
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon", 
        "Default", "That One Ribbon", "Default", "That One Ribbon"
    }; //end string[] RibbonNames

    /***************************************
     * Name: RibbonDescriptions
     * An array of ribbon descriptions
     ***************************************/
    string[] RibbonDescriptions =
    {
        "A default ribbon", "A ribbon given for doing things pertaining to testing",
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
        "A default ribbon", "A ribbon given for doing things pertaining to testing",  
    }; //end string[] RibbonDescriptions

    /***************************************
     * Name: GetRibbonName
     * Returns the name for the requested ribbon
     ***************************************/
    public string GetRibbonName(int ribbonLocation)
    {
        if (ribbonLocation > -1 && ribbonLocation < RibbonNames.Length)
        {
            return RibbonNames[ribbonLocation];
        } // end if
        else
        {
            GameManager.instance.LogErrorMessage("Error finding " + ribbonLocation + " in ribbon names");
            return "N/A";
        } //end else
    } //end GetRibbonName(int ribbonLocation)

    /***************************************
     * Name: GetRibbonDescription
     * Returns the description for the requested ribbon
     ***************************************/
    public string GetRibbonDescription(int ribbonLocation)
    {
        if (ribbonLocation > -1 && ribbonLocation < RibbonDescriptions.Length)
        {
            return RibbonDescriptions[ribbonLocation];
        } // end if
        else
        {
            GameManager.instance.LogErrorMessage("Error finding " + ribbonLocation + " in ribbon descriptions");
            return "N/A";
        } //end else
    } //end GetRibbonDescription(int ribbonLocation)
} //end RibbonData class

/***************************************************************************************** 
 * Enum:    Natures
 * Summary: Lists and organizes natures according to buffs and debuffs
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: Natures
 * List of natures, numbered for easy boost/nerf calculation
 ***************************************/
public enum Natures
{
    HARDY   = 0,
    LONELY  = 1,
    BRAVE   = 2,
    ADAMANT = 3,
    NAUGHTY = 4,
    BOLD    = 5,
    DOCILE  = 6,
    RELAXED = 7,
    IMPISH  = 8,
    LAX     = 9,
    TIMID   = 10,
    HASTY   = 11,
    SERIOUS = 12,
    JOLLY   = 13,
    NAIVE   = 14,
    MODEST  = 15,
    MILD    = 16,
    QUIET   = 17,
    BASHFUL = 18,
    RASH    = 19,
    CALM    = 20,
    GENTLE  = 21,
    SASSY   = 22,
    CAREFUL = 23,
    QUIRKY  = 24,
    COUNT   = 25
} //end Natures enum

/***************************************************************************************** 
 * Enum:    Status
 * Summary: Lists and organizes status for integer reference
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: Status
 * List of status pokemon can be in
 ***************************************/
public enum Status
{
    HEALTHY = 0,
    FAINT   = 1,
    SLEEP   = 2,
    POISON  = 3,
    BURN    = 4,
    PARALYZE= 5,
    FREEZE  = 6,
    COUNT   = 7
} //end Status enum

/***************************************************************************************** 
 * Enum:    Types
 * Summary: Lists and organizes types for integer reference
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: Types
 * List of types pokemon or moves can be
 ***************************************/
public enum Types
{
    NORMAL  = 0,
    FIGHTING= 1,
    FLYING  = 2,
    POISON  = 3,
    GROUND  = 4,
    ROCK    = 5,
    BUG     = 6,
    GHOST   = 7,
    STEEL   = 8,
    UNKNOWN = 9,
    FIRE    = 10,
    WATER   = 11,
    GRASS   = 12,
    ELECTRIC= 13,
    PSYCHIC = 14,
    ICE     = 15,
    DRAGON  = 16,
    DARK    = 17,
    FAIRY   = 18,
    COUNT   = 19
} //end Types enum

/***************************************************************************************** 
 * Enum:    Categories
 * Summary: Lists and organizes categories for integer reference
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: Categories
 * List of categories moves can be
 ***************************************/
public enum Categories
{
    Physical= 0,
    Special = 1,
    Status  = 2,
    COUNT   = 3
} //end Categories enum

/***************************************************************************************** 
 * Enum:    ObtainTypeEnum
 * Summary: Lists and organizes obtain methods for integer reference
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: ObtainTypeEnum
 * List of methods pokemon can be obtained
 ***************************************/
public enum ObtainTypeEnum
{
    Bought  = 0,
    Traded  = 1,
    Egg     = 2,
    Gift    = 3,
    Code    = 4,
    Function= 5,
    Unknown = 6,
    COUNT   = 7
} //end ObtainTypeEnum enum

/***************************************************************************************** 
 * Enum:    ObtainFromEnum
 * Summary: Lists and organizes obtain locations for integer reference
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: ObtainFromEnum
 * List of locations pokemon can be obtained
 ***************************************/
public enum ObtainFromEnum
{
    Shop         = 0,
    MysteryEvent = 1,
    RandomTeam   = 2,
    Debug        = 3,
    UnknownSource= 4,
    COUNT        = 5
} //end ObtainFromEnum enum

/***************************************************************************************** 
 * Enum:    LastingEffects
 * Summary: Lists and organizes lingering attack or field effects for integer reference
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: LastingEffects
 * LIst of effects a pokemon can be under
 ***************************************/
public enum LastingEffects
{
	AquaRing          = 0,
	Attract           = 1,
	Bide              = 2,
	BideDamage        = 3,
	BideTarget        = 4,
	Charge            = 5,
	Choice        	  = 6,
	Confusion         = 7,
	Counter           = 8,
	CounterTarget     = 9,
	Curse             = 10,
	DefenseCurl       = 11,
	DestinyBond       = 12,
	Disable           = 13,
	DisableMove       = 14,
	EchoedVoice       = 15,
	Embargo           = 16,
	Encore            = 17,
	EncoreIndex       = 18,
	EncoreMove        = 19,
	Endure            = 20,
	FlashFire         = 21,
	Flinch            = 22,
	FocusEnergy       = 23,
	FollowMe          = 24,
	Foresight         = 25,
	FuryCutter        = 26,
	FutureSight       = 27,
	FutureSightDamage = 28,
	FutureSightMove   = 29,
	FutureSightUser   = 30,
	GastroAcid        = 31,
	Grudge            = 32,
	HealBlock         = 33,
	HealingWish       = 34,
	HelpingHand       = 35,
	HyperBeam         = 36,
	Imprison          = 37,
	Ingrain           = 38,
	LeechSeed         = 39,
	LockOn            = 40,
	LockOnPos         = 41,
	LunarDance        = 42,
	MagicCoat         = 43,
	MagnetRise        = 44,
	MeanLook          = 45,
	Metronome         = 46,
	Minimize          = 47,
	MiracleEye        = 48,
	MirrorCoat        = 49,
	MirrorCoatTarget  = 50,
	MeFirst           = 51,
	MultiTurn         = 52,
	MultiTurnAttack   = 53,
	MultiTurnUser     = 54,
	Nightmare         = 55,
	Outrage           = 56,
	PerishSong        = 57,
	PerishSongUser    = 58,
	Pinch             = 59,
	PowerTrick        = 60,
	Protect           = 61,
	ProtectNegation   = 62,
	ProtectRate       = 63,
	Pursuit           = 64,
	Rage              = 65,
	Revenge           = 66,
	Rollout           = 67,
	Roost             = 68,
	SkyDrop           = 69,
	SmackDown         = 70,
	Snatch            = 71,
	Stockpile         = 72,
	StockpileDef      = 73,
	StockpileSpDef    = 74,
	Substitute        = 75,
	Taunt             = 76,
	Telekinesis       = 77,
	Torment           = 78,
	Toxic             = 79,
	Trace             = 80,
	Transform         = 81,
	Truant            = 82,
	TwoTurnAttack     = 83,
	Uproar            = 84,
	Electrify		  = 85,
	WeightDivisor 	  = 86,
	Wish              = 87,
	WishAmount        = 88,
	WishMaker         = 89,
	Yawn              = 90,
	Illusion          = 91,
	StickyWeb         = 92,
	KingsShield       = 93,
	SpikyShield       = 94,
	FairyLockRate     = 95,
	ParentalBond      = 96,
	Round             = 97,
	Powder            = 98,
	MeanLookTarget    = 99,
	COUNT			  = 100
} //end LastingEffects enum

/***************************************************************************************** 
 * Enum:    FieldEffects
 * Summary: Lists and organizes field effects for integer reference
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: Fie;dEffects
 * List of effects that can affect one side
 * of the field
 ***************************************/
public enum FieldEffects
{
	LightScreen   = 0,
	LuckyChant    = 1,
	Mist          = 2,
	Reflect       = 3,
	Safeguard     = 4,
	Spikes        = 5,
	StealthRock   = 6,
	Tailwind      = 7,
	ToxicSpikes   = 8,
	WideGuard     = 9,
	QuickGuard    = 10,
	Retaliate     = 11,
	CraftyShield  = 12,
	MatBlock      = 13,
	Gravity       = 14,
	MagicRoom     = 15,
	TrickRoom     = 16,
	WonderRoom    = 17,
	Terrain       = 18,
	FairyLock     = 19,  
	IonDeluge     = 20,
	HarshSunlight = 21,
	HeavyRain     = 22,
	MudSport      = 23,
	WaterSport    = 24,
	Rain		  = 25,
	Sun			  = 26,
	Sand		  = 27,
	Hail    	  = 28,
	StrongWinds   = 29,
	COUNT         = 30
} //end FieldEffects enum