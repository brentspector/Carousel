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
     * Name: ReturnHome
     * Brings up the main game button menu
     ***************************************/ 
    public void ReturnHome()
    {
        try
        {
			GameManager.instance.SetGameState(MainGameScene.MainGame.HOME);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end ReturnHome

    /***************************************
     * Name: GymBattle
     * Brings up the gym battle menu
     ***************************************/ 
    public void GymBattle()
    {
        try
        {
			GameManager.instance.SetGameState(MainGameScene.MainGame.GYMBATTLE);       
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end GymBattle

    /***************************************
     * Name: TeamMenu
     * Brings up the party screen
     ***************************************/ 
    public void TeamMenu()
    {
        try
        {
			GameManager.instance.SetGameState(MainGameScene.MainGame.TEAM);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end TeamMenu

    /***************************************
     * Name: PlayerPC
     * Brings up the pokemon storage screen
     ***************************************/ 
    public void PlayerPC()
    {
        try
        {
            GameManager.instance.LoadScene ("PC", true);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end PlayerPC

	/***************************************
     * Name: Inventory
     * Switches to Inventory scene
     ***************************************/ 
	public void Inventory()
	{
		try
		{
			GameManager.instance.LoadScene ("Inventory", true);
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end Inventory

    /***************************************
     * Name: Shop
     * Switches to Shop scene
     ***************************************/ 
    public void Shop()
    {
        try
        {
            GameManager.instance.LoadScene ("Shop", true);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end Shop

    /***************************************
     * Name: Pokedex
     * Switches to pokedex scene
     ***************************************/ 
    public void Pokedex()
    {
        try
        {
            GameManager.instance.LoadScene ("Pokedex", true);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end Pokedex

    /***************************************
     * Name: ContinueGame
     * Switches to MainGame scene
     ***************************************/ 
    public void ContinueGame()
    {
        try
        {
            GameManager.instance.LoadScene ("MainGame", true);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end ContinueGame

    /***************************************
     * Name: TrainerCard
     * Brings up the trainer card screen
     ***************************************/ 
    public void TrainerCard()
    {
        try
        {
			GameManager.instance.SetGameState(MainGameScene.MainGame.TRAINERCARD);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end TrainerCard

    /***************************************
     * Name: Quit
     * Exits the application
     ***************************************/ 
    public void Quit()
    {
        try
        {
            Application.Quit (); 
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end Quit

    /***************************************
     * Name: Debug
     * Brings up the debug mode options screen
     ***************************************/ 
    public void Debug()
    {
        try
        {
			GameManager.instance.SetGameState(MainGameScene.MainGame.DEBUG);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end Debug

    /***************************************
     * Name: Info
     * Brings up the Info summary screen
     ***************************************/ 
    public void Info()
    {
        try
        {
            GameManager.instance.SummaryChange(0);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end Info

    /***************************************
     * Name: Memo
     * Brings up the Memo summary screen
     ***************************************/ 
    public void Memo()
    {
        try
        {
            GameManager.instance.SummaryChange(1);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end Memo

    /***************************************
     * Name: Stats
     * Brings up the Stats summary screen
     ***************************************/ 
    public void Stats()
    {
        try
        {
            GameManager.instance.SummaryChange(2);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end Stats

    /***************************************
     * Name: EVIV
     * Brings up the EVIV summary screen
     ***************************************/ 
    public void EVIV()
    {
        try
        {
            GameManager.instance.SummaryChange(3);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end EVIV

    /***************************************
     * Name: Moves
     * Brings up the Moves summary screen
     ***************************************/ 
    public void Moves()
    {
        try
        {
            GameManager.instance.SummaryChange(4);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end Moves

    /***************************************
     * Name: PartyOpen
     * Brings up the Party in PC box
     ***************************************/ 
    public void PartyOpen()
    {
        try
        {
            GameManager.instance.PartyState(true);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end PartyOpen

    /***************************************
     * Name: PartyClose
     * Closes the Party in PC box
     ***************************************/ 
    public void PartyClose()
    {
        try
        {
            GameManager.instance.PartyState(false);
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end PartyClose

    /***************************************
     * Name: ToggleShown
     * Toggles Weakness/Resistance in Pokedex
     ***************************************/ 
    public void ToggleShown()
    {
        try
        {
            GameManager.instance.ToggleShown ();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end ToggleShown

	/***************************************
     * Name: ChangePocket
     * Changes pocket to requested
     ***************************************/ 
	public void ChangePocket(int requested)
	{
		try
		{
			GameManager.instance.ChangePocket(requested);
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end ChangePocket(int requested)

	/***************************************
     * Name: ChangeFilter
     * Changes shop to display only items
     * within the filter
     ***************************************/ 
	public void ChangeFilter(int requested)
	{
		try
		{
			GameManager.instance.ChangeFilter(requested);
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end ChangeFilter(int requested)

	/***************************************
     * Name: ItemsMode
     * Changes shop between buy and sell mode
     ***************************************/ 
	public void ItemMode()
	{
		try
		{
			GameManager.instance.ItemMode();
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end ItemMode

	/***************************************
     * Name: PreviousPage
     * Changes shop's display to previous page
     ***************************************/ 
	public void PreviousPage()
	{
		try
		{
			GameManager.instance.PreviousPage();
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end PreviousPage

	/***************************************
     * Name: NextPage
     * Changes shop's display to next page
     ***************************************/ 
	public void NextPage()
	{
		try
		{
			GameManager.instance.NextPage();
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end NextPage

	/***************************************
     * Name: Codes
     * Allows player to input codes
     ***************************************/ 
	public void Codes()
	{
		try
		{
			GameManager.instance.Codes();
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end Codes

	/***************************************
	 * Name: IncreaseQuantity
	 * Adds one to the purchase quantity
	 ***************************************/
	public void IncreaseQuantity()
	{
		try
		{
			GameManager.instance.IncreaseQuantity();
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end IncreaseQuantity

	/***************************************
	 * Name: DecreaseQuantity
	 * Subtracts one from the purchase quantity
	 ***************************************/
	public void DecreaseQuantity()
	{
		try
		{
			GameManager.instance.DecreaseQuantity();
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end DecreaseQuantity

	/***************************************
	 * Name: ConfirmPurchase
	 * Allows purchase and returns to 
	 * shop
	 ***************************************/
	public void ConfirmPurchase()
	{
		try
		{
			GameManager.instance.ConfirmPurchase();
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end ConfirmPurchase

	/***************************************
	 * Name: CancelPurchase
	 * Cancels purchase and returns to 
	 * shop
	 ***************************************/
	public void CancelPurchase()
	{
		try
		{
			GameManager.instance.CancelPurchase();
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end CancelPurchase

    /***************************************
     * Name: Persist
     * Persists the game
     ***************************************/ 
    public void Persist()
    {	
        try
        {
            GameManager.instance.Persist ();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end Persist

    #region Debug
    /***************************************
     * Name: EditPokemonMode
     * Activates the pokemon edit panel
     ***************************************/ 
    public void EditPokemonMode()
    {  
        try
        {
            GameManager.instance.EditPokemonMode();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end EditPokemonMode

    /***************************************
     * Name: EditTrainerMode
     * Activates the trainer edit panel
     ***************************************/ 
    public void EditTrainerMode()
    {  
        try
        {
            GameManager.instance.EditTrainerMode();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end EditTrainerMode

	/***************************************
     * Name: FillInventory
     * Fills inventory with one of each item
     ***************************************/ 
	public void FillInventory()
	{	
		try
		{
			GameManager.instance.FillInventory();
		} //end try
		catch(System.Exception e)
		{
			GameManager.instance.LogErrorMessage (e.ToString());
		} //end catch
	} //end FillInventory

    /***************************************
     * Name: UpdateSprite
     * Changes sprite to reflect choice change
     ***************************************/ 
    public void UpdateSprite()
    {	
        try
        {
            GameManager.instance.UpdateSprite();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end UpdateSprite

    /***************************************
     * Name: RandomTeam
     * Gives player a random team
     ***************************************/ 
    public void RandomTeam()
    {
        try
        {
            GameManager.instance.RandomTeam ();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end RandomTeam

    /***************************************
     * Name: RandomPokemon
     * Gives player a random pokemon
     ***************************************/ 
    public void RandomPokemon()
    {
        try
        {
            GameManager.instance.RandomPokemon ();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end RandomPokemon

	/***************************************
     * Name: EditPokemon
     * Grabs a pokemon out of team or pc
     * and populates debug with it
     ***************************************/ 
	public void EditPokemon()
    {	
        try
        {
		    GameManager.instance.EditPokemon ();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
	} //end EditPokemon

	/***************************************
     * Name: RemovePokemon
     * Remove a pokemon from PC or team
     ***************************************/ 
	public void RemovePokemon()
	{	
        try
        {
		    GameManager.instance.RemovePokemon ();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
	} //end RemovePokemon

    /***************************************
     * Name: FinishEditing
     * Apply pokemon to requested spot
     ***************************************/ 
    public void FinishEditing()
    {
        try
        {
            GameManager.instance.FinishEditing ();
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage (e.ToString());
        } //end catch
    } //end FinishEditing
    #endregion
    #endregion
} //end ButtonFunctions class
