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

    public void Jump()
    {
        GameManager.instance.Jump ();
    }
    #endregion
} //end ButtonFunctions class
