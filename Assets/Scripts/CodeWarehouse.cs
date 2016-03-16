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
    #region SQLite
    /*dbCommand.CommandText = "CREATE TABLE Pokemon(name text,type1 text,type2 text,health int,attack int," +
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
