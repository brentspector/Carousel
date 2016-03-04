/***************************************************************************************** 
 * File:    CodeWarehouse.cs
 * Summary: This script only serves as a storage space for code that is not necessary during 
 *          regular functioning, but might be useful in the future.
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System.Collections;
#endregion

public class CodeWarehouse : MonoBehaviour 
{
    #region MoveDataBuilder
    /*System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();
                    sysm.GetContents(Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/moves.txt");
                    for(int i = 0; i < 633; i++)
                    {
                        Move testMove = new Move();
                        string[] moveInfo = sysm.ReadCSV(i);
                        testMove.internalName = moveInfo[1];
                        testMove.gameName = moveInfo[2];
                        testMove.functionCode = int.Parse(moveInfo[3], System.Globalization.NumberStyles.HexNumber);
                        testMove.baseDamage = int.Parse(moveInfo[4]);
                        testMove.type = moveInfo[5];
                        testMove.category = moveInfo[6];
                        testMove.accuracy = int.Parse(moveInfo[7]);
                        testMove.totalPP = int.Parse(moveInfo[8]);
                        testMove.chanceEffect = int.Parse(moveInfo[9]);
                        testMove.target = int.Parse(moveInfo[10]);
                        testMove.priority = int.Parse(moveInfo[11]);
                        testMove.flags = moveInfo[12];
                        testMove.description = moveInfo[13];
                        for(int j = 14; j < moveInfo.Length; j++)
                            testMove.description += "," + moveInfo[j];
                        testMove.description = testMove.description.Replace("\"", "");
                        DataContents.moveData.Add(testMove);
                    } //end for
                    DataContents.PersistMoves();*/
    #endregion
    #region PokemonDataBuilder
    /*System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();
                    sysm.GetContents(Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/pokemon.txt");
                    for(int i = 0; i < 721; i++)
                    {
                        PokemonSpecies testSpecies = new PokemonSpecies();
                        string section = (i+1).ToString();
                        testSpecies.name = sysm.ReadINI<String>(section, "Name");
                        testSpecies.type1 = sysm.ReadINI<String>(section, "Type1");
                        testSpecies.type2 = sysm.ReadINI<String>(section, "Type2");
                        string holder = sysm.ReadINI<String>(section, "BaseStats");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.baseStats = Array.ConvertAll<string, int>(holder.Split(','), int.Parse);
                        } //end if
                        else
                        {
                            testSpecies.baseStats = new int[] {1, 1, 1, 1, 1, 1};
                        } //end else
                        testSpecies.genderRate = sysm.ReadINI<string>(section, "GenderRate");
                        testSpecies.growthRate = sysm.ReadINI<string>(section, "GrowthRate");
                        testSpecies.baseExp = sysm.ReadINI<int>(section, "BaseEXP");
                        holder = sysm.ReadINI<string>(section, "EffortPoints");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.effortPoints = Array.ConvertAll<string, int>(holder.Split(','), int.Parse);
                        } //end if
                        else
                        {
                            testSpecies.effortPoints = new int[] {1, 1, 1, 1, 1, 1};
                        } //end else
                        testSpecies.catchRate = sysm.ReadINI<int>(section, "Rareness");
                        testSpecies.happiness = sysm.ReadINI<int>(section, "Happiness");
                        holder = sysm.ReadINI<String>(section, "Abilities");  
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.abilities = holder.Split(',');
                        } //end if
                        else
                        {
                            testSpecies.abilities = new string[] {holder};
                        } //end else
                        testSpecies.hiddenAbility = sysm.ReadINI<String>(section, "HiddenAbility");
                        holder = sysm.ReadINI<String>(section, "Moves");
                        testSpecies.moves = new Dictionary<int, List<string>>();
                        if(holder != null && holder.Contains(","))
                        {
                            String[] breaks = holder.Split(',');
                            for(int j = 0; j < breaks.Length; j+=2)
                            {
                                if(!testSpecies.moves.ContainsKey(int.Parse(breaks[j])))
                                {
                                    List<string> myList = new List<string>();
                                    myList.Add(breaks[j+1]);
                                    testSpecies.moves.Add(int.Parse(breaks[j]), myList);   
                                } //end if
                                else
                                {
                                    testSpecies.moves[int.Parse(breaks[j])].Add(breaks[j+1]);
                                } //end else
                            } //end for
                        } //end if
                        else
                        {
                            //Pokemon will only have Tackle, and it will be available immediately
                            List<string> myList = new List<string>();
                            myList.Add("TACKLE");
                            testSpecies.moves.Add(1, myList);   
                        } //end else
                        holder = sysm.ReadINI<string>(section, "EggMoves");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.eggMoves = holder.Split(',');
                        } //end if
                        else
                        {
                            testSpecies.eggMoves = new string[] {holder};
                        } //end else
                        holder = sysm.ReadINI<string>(section, "Compatibility");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.compatibility = holder.Split(',');
                        } //end if
                        else
                        {
                            testSpecies.compatibility = new string[] {holder};
                        } //end else
                        testSpecies.steps = sysm.ReadINI<int>(section, "StepsToHatch");
                        testSpecies.height = sysm.ReadINI<float>(section, "Height");
                        testSpecies.weight = sysm.ReadINI<float>(section, "Weight");
                        testSpecies.color = sysm.ReadINI<string>(section, "Color");
                        testSpecies.habitat = sysm.ReadINI<string>(section, "Habitat");
                        testSpecies.kind = sysm.ReadINI<string>(section, "Kind");
                        testSpecies.pokedex = sysm.ReadINI<string>(section, "Pokedex");
                        holder = sysm.ReadINI<string>(section, "FormNames");
                        if(holder != null && holder.Contains(","))
                        {
                            testSpecies.forms = holder.Split(',');
                        } //end if
                        else
                        {
                            testSpecies.forms = new string[] {holder};
                        } //end else
                        testSpecies.battlerPlayerY = sysm.ReadINI<int>(section, "BattlerPlayerY");
                        testSpecies.battlerEnemyY = sysm.ReadINI<int>(section, "BattlerEnemyY");
                        testSpecies.battlerAltitude = sysm.ReadINI<int>(section, "BattlerAltitude");
                        holder = sysm.ReadINI<string>(section, "Evolutions");
                        testSpecies.evolutions = new List<Evolutions>();
                        if(holder != null && holder.Contains(","))
                        {
                            String[] breaks = holder.Split(',');
                            for(int j = 0; j < breaks.Length; j+=3)
                            {
                                Evolutions myEvo = new Evolutions();
                                myEvo.species = breaks[j];
                                myEvo.method = breaks[j+1];
                                try
                                {
                                    myEvo.trigger = breaks[j+2];
                                } //end try
                                catch(Exception)
                                {
                                    sysm.LogErrorMessage("Species " + i + " failed finding trigger for " + j);
                                } //end catch
                                testSpecies.evolutions.Add(myEvo);
                            } //end for
                        } //end if
                        DataContents.speciesData.Add(testSpecies);
                    } //end for
                    myStopwatch.Stop();
                    DataContents.PersistPokemon();
                    Debug.Log("Done saving data in " + myStopwatch.ElapsedMilliseconds);*/
    #endregion
    #region ItemDataBuilder
    /*sysm.GetContents(Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/items.txt");
                    for(int i = 0; i < 525; i++)
                    {
                        Item testItem = new Item();
                        string[] itemInfo = sysm.ReadCSV(i);
                        testItem.internalName = itemInfo[1];
                        testItem.gameName = itemInfo[2];
                        testItem.bagNumber = int.Parse(itemInfo[3]);
                        testItem.cost = int.Parse(itemInfo[4]);
                        testItem.description = itemInfo[5];
                        int j = 5;
                        if(itemInfo[j].Contains("\""))
                        {
                            while(!itemInfo[j].EndsWith("\""))
                            {
                                j++;
                                Debug.Log("J is " + j + " and I is " + i);
                                testItem.description += "," + itemInfo[j];
                            } 
                            testItem.description = testItem.description.Replace("\"", "");
                        }
                        j+=2;
                        try
                        {                        
                            testItem.battleUse = int.Parse(itemInfo[j]);
                        }
                        catch(SystemException e)
                        {
                            sysm.LogErrorMessage("Game Name: " + itemInfo[2] + " gives " + itemInfo[j] + " at J of " + j);
                        }
                        DataContents.itemData.Add(testItem);
                    }
                    DataContents.PersistItems();
                    Debug.Log("Done saving");

                    DataContents.itemData[69].description = "A long, thin, bright-red string to be held by a Pokémon. If the holder becomes infatuated, the foe does too.";
                    DataContents.PersistItems();
                    int randomNumber = 69;
                    Debug.Log(DataContents.itemData[randomNumber].internalName + 
                              "\n" + DataContents.itemData[randomNumber].gameName + "\n" +
                              DataContents.itemData[randomNumber].bagNumber + "\n" +
                              DataContents.itemData[randomNumber].cost + "\n" +
                              DataContents.itemData[randomNumber].description + "\n" +
                              DataContents.itemData[randomNumber].battleUse);*/
    #endregion
} //end CodeWarehose class
