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
	//TODO:http://www.diranieh.com/NETSerialization/BinarySerialization.htm
    public static Trainer PatchFile(Trainer fTrainer, ref float patchVersion)
    {
		Trainer fixedTrainer = null;
        //Try to apply appropriate patches
        try
        {
			if(patchVersion > GameManager.instance.VersionNumber)
			{
				GameManager.instance.LogErrorMessage("Your file is from a newer version. Attempting to " +
					"apply your new data to the old format.");
				fixedTrainer = fTrainer;
				patchVersion = GameManager.instance.VersionNumber;
			} //end if
			else if(patchVersion < 0.3f)
			{
				GameManager.instance.LogErrorMessage ("Your file is older than patches are available for. Your patch version is " + 
					patchVersion + ".");
			} //end else if
			/***********************************
			 * Patch Notes   0.3 - 0.4
			 * - Added Inventory
			 * - Added Shop
			 * - Added abilityOn to provide effect
			 * 	 for ability capsule
			 * - Added vitamins to track use of
			 * 	 vitamins
			 * - Added ppMax and ppUp to track
			 *   how many total uses a move has
			 * - Added EXPToLevel to allow
			 *   experience bar to scale correctly
			 ***********************************/
			else if(patchVersion == 0.3f)
			{
				fixedTrainer = fTrainer;
				fixedTrainer.Bag = new Inventory();
				fixedTrainer.PShop = new Shop();
				fixedTrainer.PopulateStock(5);
				for(int i = 0; i < fixedTrainer.Team.Count; i++)
				{
					fixedTrainer.Team[i].UpdateAbilityOn();
					fixedTrainer.Team[i].UpdateVitamins();
					fixedTrainer.Team[i].UpdatePP();
					fixedTrainer.Team[i].CalculateStats();
				} //end for
				//Loop through pc boxes
				for(int i = 0; i < 50; i++)
				{
					//Loop through pokemon in box
					for(int j = 0; j < 30; j++)
					{
						if(fixedTrainer.GetPC(i, j) != null)
						{
							fixedTrainer.GetPC(i, j).UpdateAbilityOn();
							fixedTrainer.GetPC(i,j).UpdateVitamins();
							fixedTrainer.GetPC(i,j).UpdatePP();
							fixedTrainer.GetPC(i,j).CalculateStats();
						} //end  if
					} //end for
				} //end for
				patchVersion = 0.4f;
			} //end else if
        } //end try
        catch(System.Exception e)
        {
			GameManager.instance.LogErrorMessage("Attempted to patch file, but an error occured:");
			GameManager.instance.LogErrorMessage(e.ToString());
			GameManager.instance.LogErrorMessage("Please notify creator so they can fix it.");
        } //end catch

		return fixedTrainer != null ? fixedTrainer : new Trainer();
    } //end PatchFile(Trainer fTrainer, ref float patchVersion)
    #endregion
} //end class Patch
