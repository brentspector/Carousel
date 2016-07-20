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
    #region Pokedex SQLite Checker
    //TODO:http://blogs.unity3d.com/2014/05/21/unit-testing-part-1-unit-tests-by-the-book/
    //https://www.assetstore.unity3d.com/en/#!/content/13802
    /***************************************
     * Name: ContinueGame
     * Loads and plays the main game
     ***************************************
    public void ContinueGame()
    {
        //If something is already happening, return
        if(processing)
        {
            return;
        } //end if
        
        //Set up scene
        if (checkpoint == 0)
        {
            //Begin processing
            processing = true;
            
            //Get references for variables
            pokeFront = GameObject.Find ("PokemonFront").GetComponent<Image> ();
            pokeBack = GameObject.Find ("PokemonBack").GetComponent<Image> ();
            pokeName = GameObject.Find ("Name").GetComponent<Text> ();
            pokeType = GameObject.Find ("Type").GetComponent<Text> ();
            pokeBaseStats = GameObject.Find ("BaseStats").GetComponent<Text> ();
            pokeAbilities = GameObject.Find ("Abilities").GetComponent<Text> ();
            pokeHeightWeight = GameObject.Find ("HeightWeight").GetComponent<Text> ();
            pokeForms = GameObject.Find ("Forms").GetComponent<Text> ();
            pokeEvolutions = GameObject.Find ("Evolutions").GetComponent<Text> ();
            pokePokedex = GameObject.Find ("Pokedex").GetComponent<Text> ();
            pokeMoves = GameObject.Find ("Moves").GetComponent<Text> ();
            shinyFlag = GameObject.Find("Shiny").GetComponent<Toggle>();
            femaleFlag = GameObject.Find("Female").GetComponent<Toggle>();
            GameManager.instance.StartTime();
            
            //Fade in
            StartCoroutine (FadeInAnimation (1));
        } //end if
        else if (checkpoint == 1)
        {
            //Begin processing
            processing = true;
            
            //Disable fade screen
            fade.gameObject.SetActive (false);
            
            //Initialize first data
            pokeFront.sprite = Resources.Load<Sprite> ("Sprites/Pokemon/001");
            pokeBack.sprite = Resources.Load<Sprite> ("Sprites/Pokemon/001b");
            pokeName.text = DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=1");
            pokeType.text = DataContents.ExecuteSQL<string>("SELECT type1 FROM Pokemon WHERE rowid=1") + ", " 
                + DataContents.ExecuteSQL<string>("SELECT type2 FROM Pokemon WHERE rowid=1");
            pokeHeightWeight.text = DataContents.ExecuteSQL<string>("SELECT height FROM Pokemon WHERE rowid=1") 
                + " height, " + DataContents.ExecuteSQL<string>("SELECT weight FROM Pokemon WHERE rowid=1") + " weight.";
            pokePokedex.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=1");
            pokeBaseStats.text = DataContents.ExecuteSQL<string>("SELECT health FROM Pokemon WHERE rowid=1");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT attack FROM Pokemon WHERE rowid=1");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT defence FROM Pokemon WHERE rowid=0");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT speed FROM Pokemon WHERE rowid=0");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT specialAttack FROM Pokemon WHERE rowid=0");
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT specialDefence FROM Pokemon WHERE rowid=0");
            pokeAbilities.text = DataContents.ExecuteSQL<string>("SELECT ability1 FROM Pokemon WHERE rowid=0")
                + ", " + DataContents.ExecuteSQL<string>("SELECT ability2 FROM Pokemon WHERE rowid=0") + ", " +
                    DataContents.ExecuteSQL<string>("SELECT hiddenAbility FROM Pokemon WHERE rowid=0");      
            pokeEvolutions.text = "Evos: " + DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=0");
            pokeForms.text = "Forms: " + DataContents.ExecuteSQL<string>("SELECT forms FROM Pokemon WHERE rowid=0");       
            pokeMoves.text = "Moves: " + DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=0");
            string temp = DataContents.ExecuteSQL<string>("SELECT forms FROM Pokemon WHERE rowid=1");
            forms = temp.Split(',');
            
            //Set chosenPoke to 1
            chosenPoke = 1;
            
            //Move to next section 
            checkpoint = 2;
            
            //End processing
            processing = false;
        } //end else if
        else if (checkpoint == 2)
        {
            //Begin processing
            processing = true;
            
            //If right arrow is pressed, increment
            if(Input.GetKey(KeyCode.RightArrow))
            {
                formNum = 0;
                if(chosenPoke + 1 > 721)
                {
                    chosenPoke = 1;
                } //end if
                else
                {
                    chosenPoke++;
                } //end else
                string temp = DataContents.ExecuteSQL<string>("SELECT forms FROM Pokemon WHERE rowid=" + chosenPoke);
                forms = temp.Split(',');
            } //end if
            //If left arrow is pressed, decrement
            else if(Input.GetKey(KeyCode.LeftArrow))
            {
                formNum = 0;
                if(chosenPoke - 1 < 1)
                {
                    chosenPoke = 721;
                } //end if
                else
                {
                    chosenPoke--;
                } //end else
                string temp = DataContents.ExecuteSQL<string>("SELECT forms FROM Pokemon WHERE rowid=" + chosenPoke);
                forms = temp.Split(',');
            } //end else if
            //If up arrow is pressed, increase form
            else if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                if((formNum + 1) == forms.Length)
                {
                    if(forms[0] != string.Empty)
                    {
                        formNum++;
                    } //end if
                } //end if
                else if((formNum + 1) > forms.Length)
                {
                    formNum = 0;
                } //end else if
                else
                {
                    formNum++;
                } //end else
            } //end else if
            //If down arrow is pressed, decrease form
            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                if((formNum - 1) < 0)
                {
                    if(forms[0] != string.Empty)
                    {
                        formNum = forms.Length;
                    } //end if
                    else
                    {
                        formNum = 0;
                    } //end else
                } //end if
                else
                {
                    formNum--;
                } //end else
            } //end else if
            
            //Load appropriate data
            string chosenString = chosenPoke.ToString("000");
            chosenString += femaleFlag.isOn ? "f" : "";
            chosenString += shinyFlag.isOn ? "s" : "";
            chosenString += formNum > 0 ? "_" + formNum.ToString() : "";
            pokeFront.sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
            if(pokeFront.sprite == null)
            {
                chosenString = chosenString.Replace("f", "");
                pokeFront.sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
                if(pokeFront.sprite == null)
                {
                    pokeFront.sprite = Resources.Load<Sprite>("Sprites/Pokemon/0");
                } //end if
            } //end if
            pokeBack.sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString + "b");
            if(pokeBack.sprite == null)
            {
                chosenString = chosenString.Replace("f", "");
                pokeBack.sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString + "b");
                if(pokeBack.sprite == null)
                {
                    pokeBack.sprite = Resources.Load<Sprite>("Sprites/Pokemon/0b");
                } //end if
            } //end if
            pokeName.text = DataContents.ExecuteSQL<string>("SELECT name FROM Pokemon WHERE rowid=" +  chosenPoke);
            pokeType.text = DataContents.ExecuteSQL<string>("SELECT type1 FROM Pokemon WHERE rowid=" +  chosenPoke) 
                + ", " + DataContents.ExecuteSQL<string>("SELECT type2 FROM Pokemon WHERE rowid=" +  chosenPoke);
            pokeHeightWeight.text = Math.Round(DataContents.ExecuteSQL<float>("SELECT height FROM Pokemon WHERE rowid=" + chosenPoke), 1) 
                + " height, " + Math.Round(DataContents.ExecuteSQL<float>("SELECT weight FROM Pokemon WHERE rowid=" +  chosenPoke), 1) + " weight.";
            pokePokedex.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=" +  chosenPoke);
            pokeBaseStats.text = DataContents.ExecuteSQL<string>("SELECT health FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT attack FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT defence FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT speed FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT specialAttack FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeBaseStats.text += " " + DataContents.ExecuteSQL<string>("SELECT specialDefence FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeAbilities.text = DataContents.ExecuteSQL<string>("SELECT ability1 FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeAbilities.text += ", " + DataContents.ExecuteSQL<string>("SELECT ability2 FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeAbilities.text += ", " + DataContents.ExecuteSQL<string>("SELECT hiddenAbility FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeEvolutions.text = "Evolutions: " + DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeForms.text = "Forms: " + DataContents.ExecuteSQL<string>("SELECT forms FROM Pokemon WHERE rowid=" + chosenPoke);
            pokeMoves.text = "Moves: " + DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=" + chosenPoke);
            
            //End processing
            processing = false;
        } //end else if
    } //end ContinueGame*/
    #endregion
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
                "habitat,kind,pokedex,forms,battlerPlayerY,battlerEnemyY,battlerAltitude) " +
                "VALUES (@nm,@t1,@t2,@hp,@atk,@def,@spe,@spa,@spd,@gdr,@grr,@bex,@hpe,@atke,@defe,@spee," +
                "@spae,@spde,@cr,@hap,@a1,@a2,@ha,@c1,@c2,@step,@ht,@wt,@col,@hab,@kd,@pdx,@fm,@bpy,@bey,@ba)";
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
    }
    
           //Abilities
        SystemManager sysm = new SystemManager();
        sysm.GetContents (dataLocation + "abilities.txt");
        for (int i = 0; i < 191; i++)
        {
            string[] contents = sysm.ReadCSV(i);
            dbCommand.CommandText = "UPDATE Abilities SET internalName=@in,gameName=@gn,description=@desc WHERE rowid=" + (i+1);
            dbCommand.Parameters.Add(new SqliteParameter("@in", contents[1]));
            dbCommand.Parameters.Add(new SqliteParameter("@gn", contents[2]));
            string temp = contents[3].Replace("\"", "");
            for(int j = 4; j < contents.Length; j++)
            {
                temp += "," + contents[j].Replace("\"", "");
            } //end for
            dbCommand.Parameters.Add(new SqliteParameter("@desc", temp));
            dbCommand.Prepare();
            dbCommand.ExecuteNonQuery();
            dbCommand.Parameters.Clear();
        } //end for
        
		//Items
		SystemManager sysm = new SystemManager();
		sysm.GetContents (dataLocation + "items.txt");
		dbCommand.CommandText = "DROP TABLE Items";
		dbCommand.ExecuteNonQuery();
		dbCommand.CommandText = "CREATE TABLE Items(id int,internalName text,gameName text,bagNumber int,cost int,outsideUse int,battleUse int,description text)";
		dbCommand.ExecuteNonQuery();
		for (int i = 0; i < 438; i++)
		{
			string[] contents = sysm.ReadCSV(i);
			dbCommand.CommandText = "INSERT INTO Items(id,internalName,gameName,bagNumber,cost,outsideUse,battleUse,description) VALUES " +
				"(@id,@in,@gn,@bg,@cs,@ou,@bu,@desc)";
			dbCommand.Parameters.Add(new SqliteParameter("@id", contents[0]));
			dbCommand.Parameters.Add(new SqliteParameter("@in", contents[1]));
			dbCommand.Parameters.Add(new SqliteParameter("@gn", contents[2]));
			dbCommand.Parameters.Add(new SqliteParameter("@bg", contents[3]));
			dbCommand.Parameters.Add(new SqliteParameter("@cs", contents[4]));
			dbCommand.Parameters.Add(new SqliteParameter("@ou", contents[5]));
			dbCommand.Parameters.Add(new SqliteParameter("@bu", contents[6]));
			string temp = contents[7].Replace("\"", "");
			for(int j = 8; j < contents.Length-1; j++)
			{
				temp += "," + contents[j].Replace("\"", "");
			} //end for
			dbCommand.Parameters.Add(new SqliteParameter("@desc", temp));
			dbCommand.Prepare();
			dbCommand.ExecuteNonQuery();
			dbCommand.Parameters.Clear();
		} //end for

		SystemManager sysm = new SystemManager();
		sysm.GetContents (dataLocation + "items.txt");
		for (int i = 0; i < 440; i++)
		{
			string[] contents = sysm.ReadCSV(i);
			dbCommand.CommandText = "UPDATE Items SET cost=@cs WHERE rowid=" + (i+1);
			dbCommand.Parameters.Add(new SqliteParameter("@cs", contents[4]));
			string temp = contents[7].Replace("\"", "");
			for(int j = 8; j < contents.Length-1; j++)
			{
				temp += "," + contents[j].Replace("\"", "");
			} //end for
			dbCommand.Parameters.Add(new SqliteParameter("@desc", temp));
			dbCommand.Prepare();
			dbCommand.ExecuteNonQuery();
			dbCommand.Parameters.Clear();
		} //end for

        //Moves
        sysm.GetContents (dataLocation + "moves.txt");
        for (int i = 0; i < 633; i++)
        {
            string[] contents = sysm.ReadCSV(i);
            dbCommand.CommandText = "UPDATE Moves SET internalName=@in,gameName=@gn,functionCode=@fc,baseDamage=@bd,type=@ty,category=@ct,accuracy=@ac,totalPP=@tp,chanceEffect=@ce,target=@tg,priority=@pi,flags=@fg,description=@desc WHERE rowid=" + (i+1);
            dbCommand.Parameters.Add(new SqliteParameter("@in", contents[1]));
            dbCommand.Parameters.Add(new SqliteParameter("@gn", contents[2]));
            dbCommand.Parameters.Add(new SqliteParameter("@fc", contents[3]));
            dbCommand.Parameters.Add(new SqliteParameter("@bd", contents[4]));
            dbCommand.Parameters.Add(new SqliteParameter("@ty", contents[5]));
            dbCommand.Parameters.Add(new SqliteParameter("@ct", contents[6]));
            dbCommand.Parameters.Add(new SqliteParameter("@ac", contents[7]));
            dbCommand.Parameters.Add(new SqliteParameter("@tp", contents[8]));
            dbCommand.Parameters.Add(new SqliteParameter("@ce", contents[9]));
            dbCommand.Parameters.Add(new SqliteParameter("@tg", contents[10]));
            dbCommand.Parameters.Add(new SqliteParameter("@pi", contents[11]));
            dbCommand.Parameters.Add(new SqliteParameter("@fg", contents[12]));
            string temp = contents[13].Replace("\"", "");
            for(int j = 14; j < contents.Length; j++)
            {
                temp += "," + contents[j].Replace("\"", "");
            } //end for
            dbCommand.Parameters.Add(new SqliteParameter("@desc", temp));
            dbCommand.Prepare();
            dbCommand.ExecuteNonQuery();
            dbCommand.Parameters.Clear();
        } //end for
        
        SystemManager sysm = new SystemManager ();
        sysm.GetContents (dataLocation + "abilities.txt");
        dbCommand.CommandText = "DELETE FROM Abilities";
        dbCommand.ExecuteNonQuery();
        for (int i = 0; i < 191; i++)
        {
            string[] contents = sysm.ReadCSV(i);
            dbCommand.CommandText = "INSERT INTO Abilities(internalName,gameName,description) VALUES  (@in,@gn,@desc)";
            dbCommand.Parameters.Add(new SqliteParameter("@in", contents[1]));
            dbCommand.Parameters.Add(new SqliteParameter("@gn", contents[2]));
            string temp = contents[3].Replace("\"", "");
            for(int j = 4; j < contents.Length; j++)
            {
                temp += "," + contents[j].Replace("\"", "");
            } //end for
            dbCommand.Parameters.Add(new SqliteParameter("@desc", temp));
            dbCommand.Prepare();
            dbCommand.ExecuteNonQuery();
            dbCommand.Parameters.Clear();
        } //end for

		//Load trainers
		SystemManager sysm = new SystemManager();
		sysm.GetContents(dataLocation + "trainers.txt");
		dbCommand.CommandText = "DROP TABLE Trainers";
		dbCommand.ExecuteNonQuery();
		dbCommand.CommandText = "CREATE TABLE Trainers(enemyID int,name text,battle int,image int,items text,pokemon1 text," +
		"pokemon2 text,pokemon3 text,pokemon4 text,pokemon5 text,pokemon6 text)";
		dbCommand.ExecuteNonQuery();
		for (int i = 196; i < 235; i++)
		{
			dbCommand.CommandText = "INSERT INTO Trainers(enemyID,name,battle,image,items,pokemon1,pokemon2,pokemon3,pokemon4,pokemon5," +
				"pokemon6) VALUES (@ei,@nm,@bt,@im,@it,@p1,@p2,@p3,@p4,@p5,@p6)";
			dbCommand.Parameters.Add(new SqliteParameter("@ei",i.ToString()));
			dbCommand.Parameters.Add(new SqliteParameter("@nm",sysm.ReadINI<string>(i.ToString(),"Name")));
			dbCommand.Parameters.Add(new SqliteParameter("@bt",sysm.ReadINI<int>(i.ToString(),"Battle")));
			dbCommand.Parameters.Add(new SqliteParameter("@im",sysm.ReadINI<int>(i.ToString(),"Image")));
			dbCommand.Parameters.Add(new SqliteParameter("@it",sysm.ReadINI<string>(i.ToString(),"Items")));
			dbCommand.Parameters.Add(new SqliteParameter("@p1",sysm.ReadINI<string>(i.ToString(),"Pokemon1")));
			dbCommand.Parameters.Add(new SqliteParameter("@p2",sysm.ReadINI<string>(i.ToString(),"Pokemon2")));
			dbCommand.Parameters.Add(new SqliteParameter("@p3",sysm.ReadINI<string>(i.ToString(),"Pokemon3")));
			dbCommand.Parameters.Add(new SqliteParameter("@p4",sysm.ReadINI<string>(i.ToString(),"Pokemon4")));
			dbCommand.Parameters.Add(new SqliteParameter("@p5",sysm.ReadINI<string>(i.ToString(),"Pokemon5")));
			dbCommand.Parameters.Add(new SqliteParameter("@p6",sysm.ReadINI<string>(i.ToString(),"Pokemon6")));
			dbCommand.Prepare();
			dbCommand.ExecuteNonQuery();
			dbCommand.Parameters.Clear();
		} //end for

		//Load Forms
				dbCommand.CommandText = "CREATE TABLE Forms(baseSpecies int,form int,name text,type1 text,type2 text,health int,attack int," +
		"defence int,speed int,specialAttack int,specialDefence int,baseExp int,hpEffort int,attackEffort int,defenceEffort int," +
		"speedEffort int,specialAttackEffort int,specialDefenceEffort int,ability text,height real,weight real,item int," +
		"battlerPlayerY int,battlerEnemyY int,battlerAltitude int)";
		dbCommand.ExecuteNonQuery();
				SystemManager sysm = new SystemManager();
		sysm.GetContents(dataLocation + "Forms.txt");

		int[] toAdd = new int[]{282,334,460,475};
		for (int i = 0; i < 4; i++)
		{
			dbCommand.CommandText = "INSERT INTO Forms(baseSpecies,form,name,type1,type2,health,attack,defence,speed,specialAttack," +
			"specialDefence,baseExp,hpEffort,attackEffort,defenceEffort,speedEffort,specialAttackEffort,specialDefenceEffort,ability," +
			"height,weight,item,battlerPlayerY,battlerEnemyY,battlerAltitude) VALUES (@bs,@fm,@nm,@t1,@t2,@hp,@atk,@def,@spe,@spa," +
			"@spd,@bxp,@hpe,@atke,@defe,@spee,@spae,@spde,@ab,@ht,@wt,@it,@bpy,@bey,@ba)";
			dbCommand.Parameters.Add(new SqliteParameter("@bs", toAdd[i]));
			dbCommand.Parameters.Add(new SqliteParameter("@fm", sysm.ReadINI<int>(toAdd[i].ToString(), "Form")));
			dbCommand.Parameters.Add(new SqliteParameter("@nm", sysm.ReadINI<string>(toAdd[i].ToString(), "Name")));
			dbCommand.Parameters.Add(new SqliteParameter("@t1", sysm.ReadINI<string>(toAdd[i].ToString(), "Type1")));
			dbCommand.Parameters.Add(new SqliteParameter("@t2", sysm.ReadINI<string>(toAdd[i].ToString(), "Type2")));
			string result = sysm.ReadINI<string>(toAdd[i].ToString(), "BaseStats");
			string[] contents = result.Split(',');
			dbCommand.Parameters.Add(new SqliteParameter("@hp", int.Parse(contents[0])));
			dbCommand.Parameters.Add(new SqliteParameter("@atk", int.Parse(contents[1])));
			dbCommand.Parameters.Add(new SqliteParameter("@def", int.Parse(contents[2])));
			dbCommand.Parameters.Add(new SqliteParameter("@spe", int.Parse(contents[3])));
			dbCommand.Parameters.Add(new SqliteParameter("@spa", int.Parse(contents[4])));
			dbCommand.Parameters.Add(new SqliteParameter("@spd", int.Parse(contents[5])));
			dbCommand.Parameters.Add(new SqliteParameter("@bxp", sysm.ReadINI<int>(toAdd[i].ToString(), "BaseEXP")));
			result = sysm.ReadINI<string>(toAdd[i].ToString(), "EffortPoints");
			contents = result.Split(',');
			dbCommand.Parameters.Add(new SqliteParameter("@hpe", int.Parse(contents[0])));
			dbCommand.Parameters.Add(new SqliteParameter("@atke", int.Parse(contents[1])));
			dbCommand.Parameters.Add(new SqliteParameter("@defe", int.Parse(contents[2])));
			dbCommand.Parameters.Add(new SqliteParameter("@spee", int.Parse(contents[3])));
			dbCommand.Parameters.Add(new SqliteParameter("@spae", int.Parse(contents[4])));
			dbCommand.Parameters.Add(new SqliteParameter("@spde", int.Parse(contents[5])));
			dbCommand.Parameters.Add(new SqliteParameter("@ab", sysm.ReadINI<string>(toAdd[i].ToString(), "Abilities")));
			dbCommand.Parameters.Add(new SqliteParameter("@ht", sysm.ReadINI<float>(toAdd[i].ToString(), "Height")));
			dbCommand.Parameters.Add(new SqliteParameter("@wt", sysm.ReadINI<float>(toAdd[i].ToString(), "Weight")));
			dbCommand.Parameters.Add(new SqliteParameter("@it", sysm.ReadINI<int>(toAdd[i].ToString(), "Item")));
			dbCommand.Parameters.Add(new SqliteParameter("@bpy", sysm.ReadINI<int>(toAdd[i].ToString(), "BattlerPlayerY")));
			dbCommand.Parameters.Add(new SqliteParameter("@bey", sysm.ReadINI<int>(toAdd[i].ToString(), "BattlerEnemyY")));
			dbCommand.Parameters.Add(new SqliteParameter("@ba", sysm.ReadINI<int>(toAdd[i].ToString(), "BattlerAltitude")));
			dbCommand.Prepare();
			dbCommand.ExecuteNonQuery();
			dbCommand.Parameters.Clear();
		} //end for
     */
    #endregion
    #region LevelUpFinder
    /*string[] arrayList = moveList.Split(',');
    int[] pos = arrayList.Select((b,i) => object.Equals(b,"13") ? i : -1).Where(i => i != -1).ToArray();
    for(int i = 0; i < pos.Length;i++)
    {
        Debug.Log(arrayList[pos[i]+1]);
    }*/
    #endregion

	#region Shop
	/*dbCommand.CommandText = "CREATE TABLE Shop(region text,tier1 text,tier2 text,tier3 text,tier4 text,tier5 text,held text," +
		"megastones text,evolution text,medicine text,tms text,berries text,battle text,key text)";
	dbCommand.ExecuteNonQuery();
			SystemManager sysm = new SystemManager();
		sysm.GetContents(dataLocation + "KalosShop.txt");
		dbCommand.CommandText = "DELETE FROM Shop";
		dbCommand.ExecuteNonQuery();
		dbCommand.CommandText = "INSERT INTO Shop(region,tier1,tier2,tier3,tier4,tier5,held,megastones,evolution,medicine,tms,berries," +
			"battle,key) VALUES " +
			"(@rg,@t1,@t2,@t3,@t4,@t5,@hd,@ms,@ev,@md,@tm,@br,@bt,@key)";
		dbCommand.Parameters.Add(new SqliteParameter("@rg", string.Join(",",sysm.ReadCSV(0))));
		dbCommand.Parameters.Add(new SqliteParameter("@t1", string.Join(",",sysm.ReadCSV(1))));
		dbCommand.Parameters.Add(new SqliteParameter("@t2", string.Join(",",sysm.ReadCSV(2))));
		dbCommand.Parameters.Add(new SqliteParameter("@t3", string.Join(",",sysm.ReadCSV(3))));
		dbCommand.Parameters.Add(new SqliteParameter("@t4", string.Join(",",sysm.ReadCSV(4))));
		dbCommand.Parameters.Add(new SqliteParameter("@t5", string.Join(",",sysm.ReadCSV(5))));
		dbCommand.Parameters.Add(new SqliteParameter("@hd", string.Join(",",sysm.ReadCSV(6))));
		dbCommand.Parameters.Add(new SqliteParameter("@ms", string.Join(",",sysm.ReadCSV(7))));
		dbCommand.Parameters.Add(new SqliteParameter("@ev", string.Join(",",sysm.ReadCSV(8))));
		dbCommand.Parameters.Add(new SqliteParameter("@md", string.Join(",",sysm.ReadCSV(9))));
		dbCommand.Parameters.Add(new SqliteParameter("@tm", string.Join(",",sysm.ReadCSV(10))));
		dbCommand.Parameters.Add(new SqliteParameter("@br", string.Join(",",sysm.ReadCSV(11))));
		dbCommand.Parameters.Add(new SqliteParameter("@bt", string.Join(",",sysm.ReadCSV(12))));
		dbCommand.Parameters.Add(new SqliteParameter("@key", string.Join(",",sysm.ReadCSV(13))));
		dbCommand.Prepare();
		dbCommand.ExecuteNonQuery();*/
	#endregion
} //end CodeWarehose class
