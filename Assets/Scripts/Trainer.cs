/***************************************************************************************** 
 * File:    Trainer.cs
 * Summary: Contains all player functionality
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
#endregion

[Serializable]
public class Trainer
{
    #region Variables
    List<Pokemon> team;   //The player's current party
    List<int> seen;       //What pokemon has the player encountered
    List<int> owned;      //What pokemon does(has) the player own(ed)
    PC pPC;               //The PC for the player
    string pName;         //The player's name
    uint pID;             //The Trainer ID
    int bups;             //The number of backups made
    int pTrainer;         //What trainer image set the player wants
    int pPoints;          //THe player's point count (for use in shop)
    int pBadges;          //The number of badges the player has
    int pHours;           //Total number of hours played
    int pMinutes;         //Total number of minutes played
    int pSeconds;         //Total number of seconds played
    bool debugUnlocked;   //Whether the debug has been unlocked or not
    bool[] pEarnedBadges; //Which badges the player has obtained

	[OptionalField(VersionAdded=2)]
	Inventory bag;		  //The player's item bag
	[OptionalField(VersionAdded=2)]
	Shop shop;			  //The status of the shop for the player

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

			//Initialize Bag
			bag = new Inventory();

			//Initialize Shop
			shop = new Shop();

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

            //Player hasn't picked a trainer set yet
            pTrainer = 0;

            //Player has no badges yet
            pBadges = 0;
            pEarnedBadges = new bool[48];

            //Player has not unlocked the debug menu
            debugUnlocked = true;

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
     * Name: UpdatePC
     * Replaces a current pokemon with a 
     * provided pokemon
     ***************************************/
    public void UpdatePC(int box, int spot, Pokemon newPokemon)
    {
        pPC.UpdatePC (box, spot, newPokemon);
    } //end UpdatePC(int box, int spot, Pokemon newPokemon)

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
     * Name: SetPCBoxName
     * Sets the current box name
     ***************************************/
	public void SetPCBoxName(string requestedName)
	{
		pPC.SetPCBoxName(requestedName);
	} //end SetPCBoxName(string requestedName)

	/***************************************
     * Name: SetPCBoxWallpaper
     * Set the current box wallpaper
     ***************************************/
	public void SetPCBoxWallpaper(int requestedWallpaper)
	{
		pPC.SetPCBoxWallpaper(requestedWallpaper);
	} //end SetPCBoxWallpaper(int requestedWallpaper)

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
     * Name: ChangeBox
     * Sets the PC to the requested box
     ***************************************/
	public void ChangeBox(int requestedBox)
	{
		pPC.ChangeBox (requestedBox);
	} //end ChangeBox(int requestedBox)

    /***************************************
     * Name: GetLastPokemon
     * Return the last non null pokemon in box
     ***************************************/
    public Pokemon GetLastPokemon()
    {
        return pPC.GetLastPokemon ();
    } //end GetLastPokemon

    /***************************************
     * Name: GetFirstPokemon
     * Return the first non null pokemon in box
     ***************************************/
    public Pokemon GetFirstPokemon()
    {
        return pPC.GetFirstPokemon ();
    } //end GetFirstPokemon

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

	/***************************************
     * Name: AddItem
     * Adds an item to the inventory
     ***************************************/
	public void AddItem(int item, int quantity, int bagSpot = -1)
	{		
		bag.AddItem(item, quantity, bagSpot);
	} //end AddItem(int item, int quantity, int bagSpot = -1)

	/***************************************
     * Name: RemoveItem
     * Remove an item from the inventory
     ***************************************/
	public void RemoveItem(int item, int quantity, int bagSpot = -1)
	{
		bag.RemoveItem(item, quantity, bagSpot);
	} //end RemoveItem(int item, int quantity, int bagSpot = -1)

	/***************************************
     * Name: GetItem
     * Get an item from a spot in inventory
     ***************************************/
	public List<int> GetItem(int spot, int bagSpot = -1)
	{
		return bag.GetItem(spot, bagSpot);
	} //end GetItem(int spot, int bagSpot = -1)

	/***************************************
     * Name: ItemCount
     * How many of the item are in the inventory
     ***************************************/
	public int ItemCount(int item, int bagSpot = -1)
	{
		return bag.ItemCount(item, bagSpot);
	} //end ItemCount(int item, int bagSpot = -1)

	/***************************************
     * Name: SlotCount
     * How many items are in the inventory
     * slot
     ***************************************/
	public int SlotCount(int bagSpot = -1)
	{
		return bag.SlotCount(bagSpot);
	} //end SlotCount(int bagSpot = -1)

	/***************************************
     * Name: MoveItemPocket
     * Move an item to a different bag slot
     ***************************************/
	public void MoveItemPocket(int item, int bagPocketTo = -1)
	{
		bag.MoveItemPocket(item, bagPocketTo);
	} //end MoveItemPocket(int item, int bagPocketTo =-1)

	/***************************************
     * Name: MoveItemLocation
     * Move an item to a different position 
     * in bag pocket
     ***************************************/
	public void MoveItemLocation(int bagSpotFrom, int bagSpotTo)
	{
		bag.MoveItemLocation(bagSpotFrom, bagSpotTo);
	} //end MoveItemLocation(int bagSpotFrom, int bagSpotTo)

	/***************************************
     * Name: NextPocket
     * Move to next bag pocket
     ***************************************/
	public void NextPocket()
	{
		bag.NextPocket();
	} //end NextPocket

	/***************************************
     * Name: PreviousPocket
     * Move to previous bag pocket
     ***************************************/
	public void PreviousPocket()
	{
		bag.PreviousPocket();
	} //end PreviousPocket

	/***************************************
     * Name: ChangePocket
     * Moves to requested pocket
     ***************************************/
	public void ChangePocket(int requested)
	{
		bag.ChangePocket(requested);
	} //end ChangePocket(int requested)

	/***************************************
     * Name: GetCurrentBagPocket
     * Returns the active pocket
     ***************************************/
	public int GetCurrentBagPocket()
	{
		return bag.GetCurrentBagPocket();
	} //end GetCurrentBagPocket

    /***************************************
     * Name: GetPlayerBadges
     * Return whether player owns a badge or not 
     ***************************************/
    public bool GetPlayerBadges(int location)
    {
        return pEarnedBadges [location];
    } //end GetPlayerBadges(int location)

    /***************************************
     * Name: SetPlayerBadges
     * Set whether player owns a badge or not 
     ***************************************/
    public void SetPlayerBadges(int location, bool toSet)
    {
        //Check if anything changed
        if (pEarnedBadges [location] != toSet)
        {
            //Set badge value
            pEarnedBadges [location] = toSet;

            //Update badge count
            if (toSet)
            {
                pBadges++;
            } //end if
            else
            {
                pBadges--;
            } //end else
        } //end if
    } //end GetPlayerBadges(int location)
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
     * Name: Bag
     ***************************************/
	public Inventory Bag
	{
		set
		{
			bag = value;
		} //end set
	} //end Bag

	/***************************************
     * Name: PShop
     ***************************************/
	public Shop PShop
	{
		set
		{
			shop = value;
		} //end set
	} //end PShop

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
     * Name: PlayerImage
     ***************************************/
    public int PlayerImage
    {
        get
        {
            return pTrainer;
        } //end get
        set
        {
            pTrainer = value;
        } //end set
    } //end PlayerImage

    /***************************************
     * Name: PlayerPoints
     ***************************************/
    public int PlayerPoints
    {
        get
        {
            return pPoints;
        } //end get
        set
        {
            pPoints = value;
        } //end set
    } //end PlayerPoints

    /***************************************
     * Name: PlayerBadgeCount
     ***************************************/
    public int PlayerBadgeCount
    {
        get
        {
            return pBadges;
        } //end get
        set
        {
            pBadges = value;
        } //end set
    } //end PlayerBadgeCount

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

    /***************************************
     * Name: DebugUnlocked
     ***************************************/
    public bool DebugUnlocked
    {
        get
        {
            return debugUnlocked;
        } //end get
        set
        {
            debugUnlocked = value;
        } //end set
    } //end DebugUnlocked
    #endregion
    #endregion
} //end Trainer class
