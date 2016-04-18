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
        GameManager.instance.SetGameState(SceneManager.MainGame.PC);
    } //end PlayerPC

    /***************************************
     * Name: Shop
     * Switches to Shop scene
     ***************************************/ 
    public void Shop()
    {
        GameManager.instance.SetGameState(SceneManager.MainGame.SHOP);
    } //end Shop

    /***************************************
     * Name: Pokedex
     * Switches to pokedex scene
     ***************************************/ 
    public void Pokedex()
    {
        GameManager.instance.SetGameState(SceneManager.MainGame.POKEDEX);
    } //end Pokedex

    /***************************************
     * Name: TrainerCard
     * Brings up the trainer card screen
     ***************************************/ 
    public void TrainerCard()
    {
        GameManager.instance.SetGameState(SceneManager.MainGame.TRAINERCARD);
    } //end TrainerCard

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
     * Name: Persist
     * Persists the game
     ***************************************/ 
    public void Persist()
    {
        GameManager.instance.Persist ();
    } //end Persist
    #endregion
} //end ButtonFunctions class
