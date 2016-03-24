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
     * Name: GymBattle
     * Brings up the gym battle menu
     ***************************************/ 
    public void GymBattle()
    {
        GameManager.instance.GymBattle ();       
    } //end GymBattle

    /***************************************
     * Name: TeamMenu
     * Brings up the party screen
     ***************************************/ 
    public void TeamMenu()
    {
        GameManager.instance.TeamMenu ();
    } //end TeamMenu

    /***************************************
     * Name: Shop
     * Switches to Shop scene
     ***************************************/ 
    public void Shop()
    {
        GameManager.instance.Shop ();
    } //end Shop

    /***************************************
     * Name: Pokedex
     * Switches to pokedex scene
     ***************************************/ 
    public void Pokedex()
    {
        GameManager.instance.Pokedex ();
    } //end Pokedex

    /***************************************
     * Name: TrainerCard
     * Brings up the trainer card screen
     ***************************************/ 
    public void TrainerCard()
    {
        GameManager.instance.TrainerCard ();
    } //end TrainerCard

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
