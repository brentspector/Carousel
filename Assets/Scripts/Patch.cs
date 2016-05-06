/***************************************************************************************** 
 * File:    Patch.cs
 * Summary: Allows use of older files by patching into new data
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System.Collections;
#endregion

public static class Patch
{
    #region Variables
    #endregion

    #region Methods
    public static Trainer PatchFile(Trainer fTrainer, float patchVersion)
    {
        //Try to apply appropriate patches
        try
        {
            if (patchVersion >= GameManager.instance.VersionNumber)
            {
                GameManager.instance.LogErrorMessage ("You encountered an error loading the save file and your patch " +
                    "matches the current version. Please notify creator of this problem.");
            } //end if
            else
            {
                GameManager.instance.LogErrorMessage ("Your file is old, please notify creator so they can fix it");
            } //end else
        } //end try
        catch(System.Exception e)
        {
            GameManager.instance.LogErrorMessage("Attempted to patch file, but an error occured: " + e.ToString() +
                                                 ". Please notify creator so they can fix it.");
        } //end catch

        return new Trainer();
    } //end PatchFile(Trainer fTrainer, float patchVersion)
    #endregion
} //end class Patch
