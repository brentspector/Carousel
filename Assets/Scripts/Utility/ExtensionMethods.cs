/***************************************************************************************** 
 * File:    ExtensionMethods.cs
 * Summary: Contains additional methods for use in any program
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

public static class ExtensionMethods
{
    #region Variables
    #endregion

    #region Methods
    /***************************************
     * Name: ClampToZero
     * Prevent requested from being below zero
     ***************************************/
    public static int ClampToZero(int requested)
    {
        return requested < 0 ? 0 : requested;
    } //end ClampToZero(int requested)

    /***************************************
     * Name: AddUnique
     * Adds to list if element is unique
     ***************************************/
    public static List<T> AddUnique<T>(List<T> theList, T toBeAdded)
    {
        if (!theList.Contains (toBeAdded))
        {
            theList.Add(toBeAdded);
        } //end if

        return theList;
    } //end AddUnique<T>(List<T> theList, T toBeAdded)

    /***************************************
     * Name: ToggleList
     * Adds if doesn't exist, removes if it does
     ***************************************/
    public static List<T> ToggleList<T>(List<T> theList, T toToggle)
    {
        if (theList.Contains (toToggle))
        {
            theList.Remove (toToggle);
        } //end if
        else
        {
            theList.Add (toToggle);
        } //end if

        return theList;
    } //end ToggleList<T>(List<Test> theList, T toToggle)
    #endregion
} //end class ExtensionMethods
