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
    Pokemon[,] PC;        //THe player's pokemon storage unit
    float version;        //What version this save started on     
    int bups;             //The number of backups made
    string pName;         //The player's name
    int pBadges;          //The number of badges the player has
    int pHours;           //Total number of hours played
    int pMinutes;         //Total number of minutes played
    int pSeconds;         //Total number of seconds played

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
     * Name: InitPC
     * Creates PC
     ***************************************/
    public void InitPC()
    {
        PC = new Pokemon[50, 30];
    } //end InitPC
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
     * Name: AddTeam
     * Add a pokemon to the party
     ***************************************/
    public void AddPokemon(Pokemon newPokemon)
    {
        team.Add (newPokemon);
    } //end AddPokemon(Pokemon newPokemon)

    /***************************************
     * Name: RemoveTeam
     * Remove a pokemon from the party
     ***************************************/
    public void RemovePokemon(int spot)
    {
        team.RemoveAt (spot);
    } //end RemovePokemon(int spot)

    /***************************************
     * Name: RandomTeam
     * Add six random pokemon to the party
     ***************************************/
    public void RandomTeam()
    {
        team.Clear ();
        for (int i = 0; i < 6; i++)
        {
            Pokemon myPokemon = new Pokemon (level: UnityEngine.Random.Range (1, 100));
            team.Add (myPokemon);
        }
    } //end AddPokemon(Pokemon newPokemon)

    /***************************************
     * Name: DisplayTeam
     * Show each pokemon in party
     ***************************************/
    public void DisplayPokemon()
    {
        for(int i = 0; i < team.Count; i++)
        {
            Debug.Log(team[i].Nickname + " " + team[i].CurrentLevel);
        } //end for
    } //end DisplayPokemon

    /***************************************
     * Name: DisplayPC
     * Show each pokemon in box
     ***************************************/
    public void DisplayPC(int box)
    {
        for(int i = 0; i < 30; i++)
        {
            if(PC[box, i] != null)
            {
                Debug.Log(PC[box, i].Nickname + " " + PC[box, i].CurrentLevel);
            } //end if
        } //end for
    } //end DisplayPC(int box)

    /***************************************
     * Name: EmptyPC
     * Clear all pokemon in box
     ***************************************/
    public void EmptyPC(int box)
    {
        for(int i = 0; i < 30; i++)
        {
            PC [box, i] = null;
        } //end for
    } //end EmptyPC(int box)

    /***************************************
     * Name: EmptyTeam
     * Clear all pokemon in team
     ***************************************/
    public void EmptyTeam()
    {
        team.Clear();
    } //end EmptyTeam

    /***************************************
     * Name: SetPCFromTeam
     * Move a pokemon from the team to the PC
     ***************************************/
    public void SetPCFromTeam(int box, int spot, int teamSpot)
    {
        //If the spot is empty, fill it
        if (PC [box, spot] == null)
        {
            //Place pokemon
            PC [box, spot] = team[teamSpot];

            //Remove pokemon from team
            RemovePokemon (teamSpot);
        } //end if
        
        //Otherwise look for the first unfilled spot
        else
        {
            for(int i = 0; i < 50; i++)
            {
                for(int j = 0; j < 30; j++)
                {
                    if(PC[ i, j] == null)
                    {
                        //Place pokemon
                        PC [i, j] = team[teamSpot];

                        //Remove pokemon from team
                        RemovePokemon (teamSpot);
                        return;
                    } //end if
                } //end for
            } //end for
        } //end else
    } //end SetPCFromTeam(int box, int spot, int teamSpot)

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
     * Name: GetPC
     ***************************************/
    public Pokemon GetPC(int box, int spot)
    {
        return PC [box, spot];
    } //end GetPC(int box, int spot)

    /***************************************
     * Name: SetPC
     ***************************************/
    public void SetPC(int box, int spot, Pokemon newPokemon)
    {
        //If the spot is empty, fill it
        if (PC [box, spot] == null)
        {
            PC [box, spot] = newPokemon;
        } //end if

        //Otherwise look for the first unfilled spot
        else
        {
            for(int i = 0; i < 50; i++)
            {
                for(int j = 0; j < 30; j++)
                {
                    if(PC[ i, j] == null)
                    {
                        PC [i, j] = newPokemon;
                        return;
                    } //end if
                } //end for
            } //end for
        } //end else
    } //end SetPC(int box, int spot, Pokemon newPokemon)

    /***************************************
     * Name: Version
     ***************************************/
    public float Version
    {
        get
        {
            return version;
        } //end get
        set
        {
            version = value;
        } //end set
    } //end Version

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
