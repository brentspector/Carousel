/***************************************************************************************** 
 * File:    Trainer.cs
 * Summary: Contains all player functionality
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

[Serializable]
public class Trainer
{
    #region Variables
    List<Pokemon> team;   //The player's current party
    List<int> seen;       //What pokemon has the player encountered
    List<int> owned;      //What pokemon does(has) the player own(ed)
    PC pPC;               //The PC for the player
    int bups;             //The number of backups made
    string pName;         //The player's name
    uint pID;             //The Trainer ID
    int pBadges;          //The number of badges the player has
    int pHours;           //Total number of hours played
    int pMinutes;         //Total number of minutes played
    int pSeconds;         //Total number of seconds played
    bool[] pEarnedBadges; //Which badges the player has obtained

    //Gym Battle count
    //Kalos
    int violaNormal = 0;
    int violaRegional = 0;
    int violaNational = 0;
    int grantNormal = 0;
    int grantRegional = 0;
    int grantNational = 0;
    int korrinaNormal = 0;
    int korrinaRegional = 0;
    int korrinaNational = 0;
    int ramosNormal = 0;
    int ramosRegional = 0;
    int ramosNational = 0;
    int clemontNormal = 0;
    int clemontRegional = 0;
    int clemontNational = 0;
    int valerieNormal = 0;
    int valerieRegional = 0;
    int valerieNational = 0;
    int olympiaNormal = 0;
    int olympiaRegional = 0;
    int olympiaNational = 0;
    int wulfricNormal = 0;
    int wulfricRegional = 0;
    int wulfricNational = 0;
    int malvaNormal = 0;
    int malvaRegional = 0;
    int malvaNational = 0;
    int wikstromNormal = 0;
    int wikstromRegional = 0;
    int wikstromNational = 0;
    int drasnaNormal = 0;
    int drasnaRegional = 0;
    int drasnaNational = 0;
    int sieboldNormal = 0;
    int sieboldRegional = 0;
    int sieboldNational = 0;
    int dianthaNormal = 0;
    int dianthaRegional = 0;
    int dianthaNational = 0;
    #endregion

    #region Methods
    /***************************************
     * Name: Trainer
     * Creates and initalizes trainer data
     ***************************************/
    public Trainer()
    {
        //If the game manager exists
        if (GameManager.instance != null)
        {
            //Initialize seen and owned
            seen = new List<int>();
            owned = new List<int>();

            //Initialize PC
            pPC = new PC ();

            //No backups made yet
            bups = 0;

            //Initialize team
            team = new List<Pokemon> ();

            //Player has no name yet
            pName = "-";

            //Give player a random ID
            pID = (uint)GameManager.instance.RandomInt (0, 255);
            pID |= (uint)GameManager.instance.RandomInt (0, 255) << 8;
            pID |= (uint)GameManager.instance.RandomInt (0, 255) << 16;

            //Player has no badges yet
            pBadges = 0;

            //Time played is zero
            pHours = 0;
            pMinutes = 0;
            pSeconds = 0;
        } //end if
    } //end Trainer

    /***************************************
     * Name: Swap
     * Swap pokemon location in party
     ***************************************/
    public void Swap(int locationA, int locationB)
    {
        Pokemon temp = team [locationA];
        team [locationA] = team [locationB];
        team [locationB] = temp;
    } //end Swap(int locationA, int locationB)

    /***************************************
     * Name: AddPokemon
     * Add a pokemon to the party
     ***************************************/
    public void AddPokemon(Pokemon newPokemon)
    {
        if (team.Count < 6)
        {
            team.Add (newPokemon);
        } //end if
        else
        {
            AddToPC (GetPCBox (), 0, newPokemon);
            GameManager.instance.DisplayText ("Team was full. Added to PC instead.", true);
        } //end else
    } //end AddPokemon(Pokemon newPokemon)

    /***************************************
     * Name: RemovePokemon
     * Remove a pokemon from the party
     ***************************************/
    public void RemovePokemon(int spot)
    {
        if (team.Count > 1)
        {
            team.RemoveAt (spot);
        } //end if
        else
        {
            GameManager.instance.DisplayText ("Unable to remove from team as you only have 1 pokemon left", true);
        } //end else
    } //end RemovePokemon(int spot)

    /***************************************
     * Name: ReplacePokemon
     * Replaces a pokemon in the party
     ***************************************/
    public void ReplacePokemon(Pokemon newPokemon, int spot)
    {
        team [spot] = newPokemon;
    } //end ReplacePokemon(Pokemon newPokemon, int spot)

    /***************************************
     * Name: RandomTeam
     * Add six random pokemon to the party
     ***************************************/
    public void RandomTeam()
    {
        team.Clear ();
        for (int i = 0; i < 6; i++)
        {
            Pokemon myPokemon = new Pokemon (level: GameManager.instance.RandomInt (1, 100), oType: 5, oWhere: 2);
            team.Add (myPokemon);
        }
    } //end AddPokemon(Pokemon newPokemon)

    /***************************************
     * Name: EmptyTeam
     * Clear all pokemon in team
     ***************************************/
    public void EmptyTeam()
    {
        team.Clear();
    } //end EmptyTeam

    /***************************************
     * Name: GetPC
     * Gets the pokemon at the requested 
     * box and spot
     ***************************************/
    public Pokemon GetPC(int box, int spot)
    {
        return pPC.GetPC (box, spot);
    } //end GetPC(int box, int spot)
    
    /***************************************
     * Name: AddToPC
     * Adds the pokemon to the spot in the
     * requested box
     ***************************************/
    public void AddToPC(int box, int spot, Pokemon newPokemon)
    {
        pPC.AddToPC (box, spot, newPokemon);
    } //end AddToPC(int box, int spot, Pokemon newPokemon)
    
    /***************************************
     * Name: RemoveFromPC
     * Deletes the pokemon (if it exists)
     * in the slot of the requested box
     ***************************************/
    public void RemoveFromPC(int box, int spot)
    {
        pPC.RemoveFromPC (box, spot);
    } //end RemoveFromPC(int box, int spot)
    
    /***************************************
     * Name: GetPCBox
     * Returns the current box
     ***************************************/
    public int GetPCBox()
    {
        return pPC.GetPCBox ();
    } //end GetPCBox
    
    /***************************************
     * Name: GetPCBoxName
     * Returns the current box name
     ***************************************/
    public string GetPCBoxName()
    {
        return pPC.GetPCBoxName();
    } //end GetPCBoxName
    
    /***************************************
     * Name: GetPCBoxWallpaper
     * Returns the current box wallpaper
     ***************************************/
    public int GetPCBoxWallpaper()
    {
        return pPC.GetPCBoxWallpaper();
    } //end GetPCBoxWallpaper

    /***************************************
     * Name: PreviousBox
     * Sets the PC to the previous box
     ***************************************/
    public void PreviousBox()
    {
        pPC.PreviousBox ();       
    } //end PreviousBox

    /***************************************
     * Name: NextBox
     * Sets the PC to the next box
     ***************************************/
    public void NextBox()
    {
        pPC.NextBox ();
    } //end NextBox
    
    /***************************************
     * Name: GetPreviousPokemon
     * Return the index of the nearest non null 
     * pokemon in box to the left of spot
     ***************************************/
    public int GetPreviousPokemon(int spot)
    {
        return pPC.GetPreviousPokemon (spot);
    } //end GetPreviousPokemon(int spot)
    
    /***************************************
     * Name: GetNextPokemon
     * Return the index of the nearest non null 
     * pokemon in box to the right of spot
     ***************************************/
    public int GetNextPokemon(int spot)
    {
        return pPC.GetNextPokemon (spot);
    } //end GetNextPokemon(int spot)
    #region Properties
    /***************************************
     * Name: Team
     ***************************************/
    public List<Pokemon> Team
    {
        get
        {
            return team;
        } //end get
        set
        {
            team = value;
        } //end set
    } //end Team

    /***************************************
     * Name: Seen
     ***************************************/
    public List<int> Seen
    {
        get
        {
            return seen;
        } //end get
        set
        {
            seen = value;
        } //end set
    } //end Seen

    /***************************************
     * Name: Owned
     ***************************************/
    public List<int> Owned
    {
        get
        {
            return owned;
        } //end get
        set
        {
            owned = value;
        } //end set
    } //end Owned

    /***************************************
     * Name: BackUps
     ***************************************/
    public int BackUps
    {
        get
        {
            return bups;
        } //end get
        set
        {
            bups = value;
        } //end set
    } //end BackUps

    /***************************************
     * Name: PlayerName
     ***************************************/
    public string PlayerName
    {
        get
        {
            return pName;
        } //end get
        set
        {
            pName = value;
        } //end set
    } //end PlayerName

    /***************************************
     * Name: PlayerID
     ***************************************/
    public int PlayerID
    {
        get
        {
            return (int)pID;
        } //end get
        set
        {
            pID = (uint)value;
        } //end set
    } //end PlayerID

    /***************************************
     * Name: PlayerBadges
     ***************************************/
    public int PlayerBadges
    {
        get
        {
            return pBadges;
        } //end get
        set
        {
            pBadges = value;
        } //end set
    } //end PlayerBadges

    /***************************************
     * Name: HoursPlayed
     ***************************************/
    public int HoursPlayed
    {
        get
        {
            return pHours;
        } //end get
        set
        {
            pHours = value;
        } //end set
    } //end HoursPlayed

    /***************************************
     * Name: MinutesPlayed
     ***************************************/
    public int MinutesPlayed
    {
        get
        {
            return pMinutes;
        } //end get
        set
        {
            pMinutes = value;
        } //end set
    } //end MinutesPlayed

    /***************************************
     * Name: SecondsPlayed
     ***************************************/
    public int SecondsPlayed
    {
        get
        {
            return pSeconds;
        } //end get
        set
        {
            pSeconds = value;
        } //end set
    } //end SecondsPlayed
    #endregion
    #endregion
} //end Trainer class
