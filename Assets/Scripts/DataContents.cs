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
#endregion

public static class DataContents : System.Object 
{
    #region Variables
    public static ExperienceTable experienceTable;  //Holds experience values for growth rates
    public static char[] markingCharacters;         //Holds the characters you can mark with
    static int speciesCount;                        //Number of entries in Pokemon table
    static int moveCount;                           //Number of entries in Move table
    static int itemCount;                           //Number of entries in Items table
    static int abilityCount;                        //Number of entries in Abilities table

    //Sprite Arrays
    public static Sprite[] statusSprites;           //Sprites for each status ailment
    public static Sprite[] typeSprites;             //Sprites for each type
    public static Sprite[] categorySprites;         //Sprites for each category of move

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

        //Manage SQL Database
        dbPath = "URI=file:" + dataLocation + "/Supplimental.db";
        dbConnection=new SqliteConnection(dbPath);
        dbConnection.Open();
        dbCommand=dbConnection.CreateCommand();
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
        markingCharacters = new char[] {'●','■','▲','♥'};

        //Initialize max count for each table
        speciesCount = ExecuteSQL<int>("SELECT Count(*) FROM Pokemon");
        moveCount = ExecuteSQL<int>("SELECT Count(*) FROM Moves");
        itemCount = ExecuteSQL<int>("SELECT Count(*) FROM Items");
        abilityCount = ExecuteSQL<int> ("SELECT Count(*) FROM Abilities");

        //Create an experience table
        experienceTable = new ExperienceTable();
                
        //Load sprites
        statusSprites   = Resources.LoadAll<Sprite> ("Sprites/Icons/statuses");
        typeSprites     = Resources.LoadAll<Sprite> ("Sprites/Icons/pokedexTypes");
        categorySprites = Resources.LoadAll<Sprite> ("Sprites/Icons/category");
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
     * Name: GetMoveID
     * Returns numeric location of pokemon attack
     ***************************************/
    public static int GetMoveID(string moveName)
    {
        //No move found yet
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
     * Name: GetItemID
     * Returns numeric location of item
     ***************************************/
    public static int GetItemID(string itemName)
    {
        //No move found yet
        dbCommand.CommandText = "SELECT rowid FROM Items WHERE internalName=@nm";
        dbCommand.Parameters.Add(new SqliteParameter("@nm", itemName));
        dbCommand.Prepare ();
        int itemID = ExecuteSQL<int> (dbCommand.CommandText);
        dbCommand.Parameters.Clear ();
        
        //Return location of item, or -1 if not found
        return itemID;
    } //end GetItemID(string itemName)
    
    /***************************************
     * Name: GetItemGameName
     * Returns string name of given numeric location
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
        if (experienceRate == "Medium")
        {
            return Medium [level + 1];
        } //end if
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
 * Enum:    ObtainType
 * Summary: Lists and organizes obtain methods for integer reference
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: ObtainType
 * List of methods pokemon can be obtained
 ***************************************/
public enum ObtainType
{
    Bought  = 0,
    Traded  = 1,
    Egg     = 2,
    Gift    = 3,
    Code    = 4,
    Function= 5,
    Unknown = 6,
    COUNT   = 7
} //end ObtainType enum

/***************************************************************************************** 
 * Enum:    ObtainFrom
 * Summary: Lists and organizes obtain locations for integer reference
 *****************************************************************************************/ 
[Serializable]
/***************************************
 * Name: ObtainFrom
 * List of locations pokemon can be obtained
 ***************************************/
public enum ObtainFrom
{
    Shop         = 0,
    MysteryEvent = 1,
    RandomTeam   = 2,
    AddPokemon   = 3,
    UnknownSource= 4,
    COUNT        = 5
} //end ObtainFrom enum