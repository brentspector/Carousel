/***************************************************************************************** 
 * File:    Button Functions.cs
 * Summary: Contains the function references for buttons to use. Typically links to
 *          GameManager.instance to allow runtime use.
 *****************************************************************************************/
#region Using
using UnityEngine;
using System.Collections;
#endregion

public class ButtonFunctions : MonoBehaviour 
{
    #region Variables
    #endregion

    #region Methods
    /***************************************
     * Name: ProcessSelection
     * When selection rectangle is pressed,
     * run the appropriate function
     ***************************************/ 
    public void ProcessSelection()
    {
        GameManager.instance.ProcessSelection ();
    } //end ProcessSelection

    /***************************************
     * Name: ReturnHome
     * Brings up the main game button menu
     ***************************************/ 
    public void ReturnHome()
    {
        GameManager.instance.SetGameState(SceneManager.MainGame.HOME);
    } //end ReturnHome

    /***************************************
     * Name: GymBattle
     * Brings up the gym battle menu
     ***************************************/ 
    public void GymBattle()
    {
        GameManager.instance.SetGameState(SceneManager.MainGame.GYMBATTLE);       
    } //end GymBattle

    /***************************************
     * Name: TeamMenu
     * Brings up the party screen
     ***************************************/ 
    public void TeamMenu()
    {
        GameManager.instance.SetGameState(SceneManager.MainGame.TEAM);
    } //end TeamMenu

    /***************************************
     * Name: PlayerPC
     * Brings up the pokemon storage screen
     ***************************************/ 
    public void PlayerPC()
    {
        GameManager.instance.LoadScene ("PC", SceneManager.OverallGame.PC, true);
    } //end PlayerPC

    /***************************************
     * Name: Shop
     * Switches to Shop scene
     ***************************************/ 
    public void Shop()
    {
        GameManager.instance.LoadScene ("Shop", SceneManager.OverallGame.SHOP, true);
    } //end Shop

    /***************************************
     * Name: Pokedex
     * Switches to pokedex scene
     ***************************************/ 
    public void Pokedex()
    {
        GameManager.instance.LoadScene ("Pokedex", SceneManager.OverallGame.POKEDEX, true);
    } //end Pokedex

    /***************************************
     * Name: ContinueGame
     * Switches to MainGame scene
     ***************************************/ 
    public void ContinueGame()
    {
        GameManager.instance.LoadScene ("MainGame", SceneManager.OverallGame.CONTINUE);
    } //end ContinueGame

    /***************************************
     * Name: TrainerCard
     * Brings up the trainer card screen
     ***************************************/ 
    public void TrainerCard()
    {
        GameManager.instance.SetGameState(SceneManager.MainGame.TRAINERCARD);
    } //end TrainerCard

    /***************************************
     * Name: Quit
     * Exits the application
     ***************************************/ 
    public void Quit()
    {
        Application.Quit ();       
    } //end Quit

    /***************************************
     * Name: Debug
     * Brings up the debug mode options screen
     ***************************************/ 
    public void Debug()
    {
        GameManager.instance.SetGameState(SceneManager.MainGame.DEBUG);
    } //end Debug

    /***************************************
     * Name: Info
     * Brings up the Info summary screen
     ***************************************/ 
    public void Info()
    {
        GameManager.instance.SummaryChange(0);
    } //end Info

    /***************************************
     * Name: Memo
     * Brings up the Memo summary screen
     ***************************************/ 
    public void Memo()
    {
        GameManager.instance.SummaryChange(1);
    } //end Memo

    /***************************************
     * Name: Stats
     * Brings up the Stats summary screen
     ***************************************/ 
    public void Stats()
    {
        GameManager.instance.SummaryChange(2);
    } //end Stats

    /***************************************
     * Name: EVIV
     * Brings up the EVIV summary screen
     ***************************************/ 
    public void EVIV()
    {
        GameManager.instance.SummaryChange(3);
    } //end EVIV

    /***************************************
     * Name: Moves
     * Brings up the Moves summary screen
     ***************************************/ 
    public void Moves()
    {
        GameManager.instance.SummaryChange(4);
    } //end Moves

    /***************************************
     * Name: PartyOpen
     * Brings up the Party in PC box
     ***************************************/ 
    public void PartyOpen()
    {
        GameManager.instance.PartyState(true);
    } //end PartyOpen

    /***************************************
     * Name: PartyClose
     * Closes the Party in PC box
     ***************************************/ 
    public void PartyClose()
    {
        GameManager.instance.PartyState(false);
    } //end PartyClose

    /***************************************
     * Name: ToggleShown
     * Toggles Weakness/Resistance in Pokedex
     ***************************************/ 
    public void ToggleShown()
    {
        GameManager.instance.ToggleShown ();
    } //end ToggleShown

    /***************************************
     * Name: Persist
     * Persists the game
     ***************************************/ 
    public void Persist()
    {		
        GameManager.instance.Persist ();
    } //end Persist

    #region Debug
    /***************************************
     * Name: UpdateSprite
     * Changes sprite to reflect choice change
     ***************************************/ 
    public void UpdateSprite()
    {		
        GameManager.instance.UpdateSprite();
    } //end UpdateSprite

    /***************************************
     * Name: RandomTeam
     * Gives player a random team
     ***************************************/ 
    public void RandomTeam()
    {
        GameManager.instance.RandomTeam ();
    } //end RandomTeam

    /***************************************
     * Name: RandomPokemon
     * Gives player a random pokemon
     ***************************************/ 
    public void RandomPokemon()
    {
        GameManager.instance.RandomPokemon ();
    } //end RandomPokemon

	/***************************************
     * Name: EditPokemon
     * Grabs a pokemon out of team or pc
     * and populates debug with it
     ***************************************/ 
	public void EditPokemon()
	{		
		GameManager.instance.EditPokemon ();
	} //end EditPokemon

    /***************************************
     * Name: FinishEditing
     * Apply pokemon to requested spot
     ***************************************/ 
    public void FinishEditing()
    {
        GameManager.instance.FinishEditing ();
    } //end FinishEditing
    #endregion
    #endregion
} //end ButtonFunctions class
