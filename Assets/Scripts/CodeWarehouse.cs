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
    //Item database -> SQLite database coverter
    /*for(int i = 0; i < moveData.Count; i++)
    {
        dbCommand.CommandText = "INSERT INTO Moves (internalName, gameName, functionCode, baseDamage, " +
            "type, category, accuracy, totalPP, chanceEffect, target, priority, flags, description) " +
                "VALUES (@intName,@gName,@fCode,@bDamage,@tp,@cat,@acc,@tPP,@cEff,@tar,@pri,@fg,@desc)";
        dbCommand.CommandType = CommandType.Text;
        dbCommand.Parameters.Add(new SqliteParameter("@intName", moveData[i].internalName));
        dbCommand.Parameters.Add(new SqliteParameter("@gName", moveData[i].gameName));
        dbCommand.Parameters.Add(new SqliteParameter("@fCode", moveData[i].functionCode));
        dbCommand.Parameters.Add(new SqliteParameter("@bDamage", moveData[i].baseDamage));
        dbCommand.Parameters.Add(new SqliteParameter("@tp", moveData[i].type));
        dbCommand.Parameters.Add(new SqliteParameter("@cat", moveData[i].category));
        dbCommand.Parameters.Add(new SqliteParameter("@acc", moveData[i].accuracy));
        dbCommand.Parameters.Add(new SqliteParameter("@tPP", moveData[i].totalPP));
        dbCommand.Parameters.Add(new SqliteParameter("@cEff", moveData[i].chanceEffect));
        dbCommand.Parameters.Add(new SqliteParameter("@tar", moveData[i].target));
        dbCommand.Parameters.Add(new SqliteParameter("@pri", moveData[i].priority));
        dbCommand.Parameters.Add(new SqliteParameter("@fg", moveData[i].flags));
        dbCommand.Parameters.Add(new SqliteParameter("@desc", moveData[i].description));
        dbCommand.Prepare();
        dbCommand.ExecuteNonQuery();
        dbCommand.Parameters.Clear();
    }
    sysm.GetContents(Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/items.txt");
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

    #region SQLite code scraps
    /*//Manage SQL Database
    dbConnection=new SqliteConnection(dbPath);
    dbConnection.Open();
    dbCommand=dbConnection.CreateCommand();
    dbCommand.CommandText = "SELECT Count(*) FROM Moves";
    dbReader = dbCommand.ExecuteReader();
    
    while( dbReader.Read()){
        Debug.Log(dbReader.GetValue(0));
    }
    dbCommand.CommandText = "CREATE TABLE Pokemon(name text,type1 text,type2 text,health int,attack int," +
                "defence int,speed int,specialAttack int,specialDefence int,genderRate text,growthRate text," +
                "baseExp int,hpEffort int,attackEffort int,defenceEffort int,speedEffort int,specialAttackEffort int," +
                "specialDefenceEffort int,catchRate int,happiness int,ability1 text,ability2 text,hiddenAbility text," +
                "moves text,eggMoves text,compatibility1 text,compatibility2 text,steps int,height real,weight real," +
                "color text,habitat text,kind text,pokedex text,forms text,battlerPlayerY int,battlerEnemyY int," +
                    "battlerAltitude int,evolutions text)";
    for(int i = 0; i < speciesData.Count; i++)
    {
        dbCommand.CommandText = "INSERT INTO Pokemon(name,type1,type2,health,attack,defence,speed," +
            "specialAttack,specialDefence,genderRate,growthRate,baseExp,hpEffort,attackEffort," +
                "defenceEffort,speedEffort,specialAttackEffort,specialDefenceEffort,catchRate,happiness," +
                "ability1,ability2,hiddenAbility,compatibility1,compatibility2,steps,height,weight,color," +
                "habitat,kind,pokedex,battlerPlayerY,battlerEnemyY,battlerAltitude) " +
                "VALUES (@nm,@t1,@t2,@hp,@atk,@def,@spe,@spa,@spd,@gdr,@grr,@bex,@hpe,@atke,@defe,@spee," +
                "@spae,@spde,@cr,@hap,@a1,@a2,@ha,@c1,@c2,@step,@ht,@wt,@col,@hab,@kd,@pdx,@bpy,@bey,@ba)";
        dbCommand.Parameters.Add(new SqliteParameter("@nm",speciesData[i].name));
        dbCommand.Parameters.Add(new SqliteParameter("@t1",speciesData[i].type1));
        dbCommand.Parameters.Add(new SqliteParameter("@t2",speciesData[i].type2));
        dbCommand.Parameters.Add(new SqliteParameter("@hp",speciesData[i].baseStats[0]));
        dbCommand.Parameters.Add(new SqliteParameter("@atk",speciesData[i].baseStats[1]));
        dbCommand.Parameters.Add(new SqliteParameter("@def",speciesData[i].baseStats[2]));
        dbCommand.Parameters.Add(new SqliteParameter("@spe",speciesData[i].baseStats[3]));
        dbCommand.Parameters.Add(new SqliteParameter("@spa",speciesData[i].baseStats[4]));
        dbCommand.Parameters.Add(new SqliteParameter("@spd",speciesData[i].baseStats[5]));
        dbCommand.Parameters.Add(new SqliteParameter("@gdr",speciesData[i].genderRate));
        dbCommand.Parameters.Add(new SqliteParameter("@grr",speciesData[i].growthRate));
        dbCommand.Parameters.Add(new SqliteParameter("@bex",speciesData[i].baseExp));
        dbCommand.Parameters.Add(new SqliteParameter("@hpe",speciesData[i].effortPoints[0]));
        dbCommand.Parameters.Add(new SqliteParameter("@atke",speciesData[i].effortPoints[1]));
        dbCommand.Parameters.Add(new SqliteParameter("@defe",speciesData[i].effortPoints[2]));
        dbCommand.Parameters.Add(new SqliteParameter("@spee",speciesData[i].effortPoints[3]));
        dbCommand.Parameters.Add(new SqliteParameter("@spae",speciesData[i].effortPoints[4]));
        dbCommand.Parameters.Add(new SqliteParameter("@spde",speciesData[i].effortPoints[5]));
        dbCommand.Parameters.Add(new SqliteParameter("@cr",speciesData[i].catchRate));
        dbCommand.Parameters.Add(new SqliteParameter("@hap",speciesData[i].happiness));
        dbCommand.Parameters.Add(new SqliteParameter("@a1",speciesData[i].abilities[0]));
        try
        {
            dbCommand.Parameters.Add(new SqliteParameter("@a2",speciesData[i].abilities[1]));
        }
        catch(SystemException e)
        {
            dbCommand.Parameters.Add(new SqliteParameter("@a2",""));
        }
        dbCommand.Parameters.Add(new SqliteParameter("@ha",speciesData[i].hiddenAbility));
        dbCommand.Parameters.Add(new SqliteParameter("@c1",speciesData[i].compatibility[0]));
        try
        {
            dbCommand.Parameters.Add(new SqliteParameter("@c2",speciesData[i].compatibility[1]));
        }
        catch(SystemException e)
        {
            dbCommand.Parameters.Add(new SqliteParameter("@c2",""));
        }
        dbCommand.Parameters.Add(new SqliteParameter("@step",speciesData[i].steps));
        dbCommand.Parameters.Add(new SqliteParameter("@ht",speciesData[i].height));
        dbCommand.Parameters.Add(new SqliteParameter("@wt",speciesData[i].weight));
        dbCommand.Parameters.Add(new SqliteParameter("@col",speciesData[i].color));
        dbCommand.Parameters.Add(new SqliteParameter("@hab",speciesData[i].habitat));
        dbCommand.Parameters.Add(new SqliteParameter("@kd",speciesData[i].kind));
        dbCommand.Parameters.Add(new SqliteParameter("@pdx",speciesData[i].pokedex));
        dbCommand.Parameters.Add(new SqliteParameter("@bpy",speciesData[i].battlerPlayerY));
        dbCommand.Parameters.Add(new SqliteParameter("@bey",speciesData[i].battlerEnemyY));
        dbCommand.Parameters.Add(new SqliteParameter("@ba",speciesData[i].battlerAltitude));
        dbCommand.Prepare();
        dbCommand.ExecuteNonQuery();
        dbCommand.Parameters.Clear();
    }*/
    #endregion
    #region LevelUpFinder
    /*string[] arrayList = moveList.Split(',');
    int[] pos = arrayList.Select((b,i) => object.Equals(b,"13") ? i : -1).Where(i => i != -1).ToArray();
    for(int i = 0; i < pos.Length;i++)
    {
        Debug.Log(arrayList[pos[i]+1]);
    }*/
    #endregion
} //end CodeWarehose class
