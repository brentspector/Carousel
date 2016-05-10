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
     * Name: BindToInt
     * Prevent requested from being below min
     ***************************************/
    public static int BindToInt(int requested, int min)
    {
        return requested < min ? min : requested;
    } //end BindToInt(int requested, int min)

    /***************************************
     * Name: BindToFloat
     * Prevent requested from being below min
     ***************************************/
    public static float BindToFloat(float requested, float min)
    {
        return requested < min ? min : requested;
    } //end BindToFloat(float requested, float min)

    /***************************************
     * Name: CapAtInt
     * Prevent requested from being above requested
     ***************************************/
    public static int CapAtInt(int given, int max)
    {
        return given < max ? given : max;
    } //end CapAtInt(int given, int max)

    /***************************************
     * Name: CapAtFloat
     * Prevent requested from being above requested
     ***************************************/
    public static float CapAtFloat(float given, float max)
    {
        return given < max ? given : max;
    } //end CapAtFloat(float given, float max)

    /***************************************
     * Name: WithinIntRange
     * Prevent requested from being outside bounds
     ***************************************/
    public static int WithinIntRange(int given, int min, int max)
    {
        int result = CapAtInt (given, max);
        result = BindToInt (given, min);
        return result;
    } //end WithinIntRange(int given, int min, int max)

    /***************************************
     * Name: WithinFloatRange
     * Prevent requested from being outside bounds
     ***************************************/
    public static float WithinFloatRange(float given, float min, float max)
    {
        float result = CapAtFloat (given, max);
        result = BindToFloat (given, min);
        return result;
    } //end WithinFloatRange(float given, float min, float max)

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
     * Name: RemoveRange
     * Removes each element in a given list
     * from a provided list if it exists in 
     * the provided list
     ***************************************/
    public static List<T> RemoveRange<T>(List<T> theList, List<T> toBeRemoved)
    {
        for (int i = 0; i < toBeRemoved.Count; i++)
        {
            if (theList.Contains (toBeRemoved[i]))
            {
                theList.Remove (toBeRemoved[i]);
            } //end if
        } //end for

        return theList;
    } //end RemoveRange<T>(List<T> theList, List<T> toBeRemoved)

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
