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
    public static void PatchFile(Trainer pTrainer, float patchVersion, string exception)
    {
        //Apply appropriate patches
        if (patchVersion >= 0.3f)
        {
            GameManager.instance.LogErrorMessage ("You encountered an error loading the save file and your patch " +
                "matches the current version. Please notify creator of this problem. The error encountered was " +
                exception);
        } //end if
        else
        {
            GameManager.instance.LogErrorMessage ("Your file is old, please notify creator to they can fix it. " +
                "The error encountered was " + exception);
        } //end else
    } //end PatchFile(Trainer pTrainer)
    #endregion
} //end class Patch
