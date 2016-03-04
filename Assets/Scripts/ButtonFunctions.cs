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
     * Name: Continue
     * Continue option from main screen
     ***************************************/ 
	public void Continue()
	{
		GameManager.instance.Continue ();
	} //end Continue

    /***************************************
     * Name: NewGame
     * New Game option from main screen
     ***************************************/ 
	public void NewGame()
	{
		GameManager.instance.NewGame ();
	} //end NewGame

    /***************************************
     * Name: Options
     * Options option from main screen
     ***************************************/ 
	public void Options()
	{
		GameManager.instance.Options ();
	} //end Options

    /***************************************
     * Name: Jump
     * Jump to pokedex number from game screen
     ***************************************/ 
    public void Jump()
    {
        GameManager.instance.Jump ();
    } //end Jump
    #endregion
} //end ButtonFunctions class
