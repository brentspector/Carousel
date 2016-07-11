/***************************************************************************************** 
 * File:    PokedexScene.cs
 * Summary: Handles process for Pokedex scene
 *****************************************************************************************/
#region Using
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

public class PokedexScene : MonoBehaviour
{
	#region Variables
	int checkpoint = 0;				//Manage function progress
	int pokedexIndex;               //The location in the pokedex currently highlighted
	bool processing = false;		//Whether a function is already processing something
    GameObject index;               //Region displaying overall pokedex location
    GameObject stats;               //The stat bars of the selected pokemon
    GameObject characteristics;     //The characteristics of the selected pokemon
    GameObject movesRegion;         //The moves the selected pokemon can learn
    GameObject evolutionsRegion;    //The pokemon the selected pokemon can evolve into
    GameObject shownButton;         //Whether weaknesses or resistances are shown
    GameObject weakTypes;           //Object containing all types selected pokemon is weak to
    GameObject resistTypes;         //Object containing all types selected pokemon is resistant to
    Image pokemonImage;             //The image of the selected pokemon
    Text abilitiesText;             //The abilities the selected pokemon can have
    Text pokedexText;               //The flavor text for the selected pokemon
	#endregion

	#region Methods
	/***************************************
	 * Name: RunPokedex
	 * Play the pokedex scene
	 ***************************************/
	public void RunPokedex()
	{
		//Initialize scene variables
		if (checkpoint == 0)
		{
			//Set checkpoint delegate
			GameManager.instance.checkDel = ChangeCheckpoint;

			//Initialize references
			index = GameObject.Find("Index");
			stats = GameObject.Find("Stats");
			characteristics = GameObject.Find("Characteristics");
            movesRegion  = GameObject.Find("MovesRegion").transform.FindChild("MovesContainer").gameObject;             
            evolutionsRegion  = GameObject.Find("Evolutions").transform.FindChild("EvolutionContainer").gameObject;        
            shownButton = GameObject.Find("Shown");             
            weakTypes = GameObject.Find("WeakTypes");               
            resistTypes = GameObject.Find("ResistanceTypes");             
            pokemonImage = GameObject.Find("Sprite").GetComponent<Image>();                 
            abilitiesText = GameObject.Find("AbilitiesText").GetComponent<Text>();
            pokedexText = GameObject.Find("PokedexText").GetComponent<Text>();
            pokedexIndex = 1;
                    
            //Move to next checkpoint
            checkpoint = 1;
		} //end if

		//Set up start of scene
        else if (checkpoint == 1)
        {
			//Return if processing
			if (processing)
			{
				return;
			} //end if

			//Begin processing
			processing = true;

            //Fill in index region
            FillInIndex();

            //Fill in highlighted sprite
            pokemonImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + 
                pokedexIndex.ToString("000"));

            //Adjust stat bars
            float dbStat = DataContents.ExecuteSQL<float>("SELECT health FROM Pokemon WHERE rowid=" + pokedexIndex);
            float scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            Transform barChild = stats.transform.GetChild(1).FindChild("HPBar");
            barChild.GetComponent<RectTransform>().localScale = new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 85 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/85f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-85f)/65f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT attack FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            barChild = stats.transform.GetChild(1).FindChild("AttackBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 100 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT defence FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            barChild = stats.transform.GetChild(1).FindChild("DefenceBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 100 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT specialAttack FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            barChild = stats.transform.GetChild(1).FindChild("SpecialAttackBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 100 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT specialDefence FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            barChild = stats.transform.GetChild(1).FindChild("SpecialDefenceBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 95 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95f)/55f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT speed FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/130f, 1);
            barChild = stats.transform.GetChild(1).FindChild("SpeedBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 95 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95)/35f);

            //Set types
            Transform typeRegion = characteristics.transform.GetChild(1).FindChild("Types");
            SetTypeSprites(typeRegion.GetChild(0).GetComponent<Image>(), typeRegion.GetChild(1).GetComponent<Image>(),
                pokedexIndex);

            //Set egg groups
            characteristics.transform.GetChild(1).FindChild("EggText").GetComponent<Text>().text = 
                DataContents.ExecuteSQL<string>("SELECT compatibility1 FROM Pokemon WHERE rowid=" + pokedexIndex) +
                "   " + DataContents.ExecuteSQL<string>("SELECT compatibility2 FROM Pokemon WHERE rowid=" + pokedexIndex);

            //Set height
            characteristics.transform.GetChild(1).FindChild("HeightText").GetComponent<Text>().text = 
                Math.Round(DataContents.ExecuteSQL<float>("SELECT height FROM Pokemon WHERE rowid=" + pokedexIndex), 1).
                ToString();

            //Set weight
            characteristics.transform.GetChild(1).FindChild("WeightText").GetComponent<Text>().text =
                Math.Round(DataContents.ExecuteSQL<float>("SELECT weight FROM Pokemon WHERE rowid=" +  pokedexIndex), 1).
                ToString();

            //Set species
            characteristics.transform.GetChild(1).FindChild("SpeciesText").GetComponent<Text>().text =
                DataContents.ExecuteSQL<string>("SELECT kind FROM Pokemon WHERE rowid=" + pokedexIndex) + " Pokemon";

            //Set level up moves
            string moveList = DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=" + pokedexIndex);
            string[] arrayList = moveList.Split(',');
            Text levelText = movesRegion.transform.FindChild("MoveLevel").GetComponent<Text>();
            Text moveText = movesRegion.transform.FindChild("MoveName").GetComponent<Text>();
            levelText.text = "";
            moveText.text = "";
            for(int i = 0; i < arrayList.Length-2; i+=2)
            {
                levelText.text += arrayList[i] + "\n";
                moveText.text += arrayList[i+1] + "\n";
            } //end for
            levelText.text += arrayList[arrayList.Length-2];
            moveText.text += arrayList[arrayList.Length-1];

            //Add egg moves to move list
            moveList = DataContents.ExecuteSQL<string>("SELECT eggMoves FROM Pokemon WHERE rowid=" + pokedexIndex);
            arrayList = moveList.Split(',');
            for(int i = 0; i < arrayList.Length; i++)
            {
                levelText.text += "\nEgg";
                moveText.text += "\n" + arrayList[i];
            } //end for

            //Add evolutions
            string evolveList = DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=" + pokedexIndex);
            arrayList = evolveList.Split(',');
            Text nameText = evolutionsRegion.transform.FindChild("EvolutionName").GetComponent<Text>();
            Text methodText = evolutionsRegion.transform.FindChild("EvolutionMethod").GetComponent<Text>();
            nameText.text = "";
            methodText.text = "";
            if(arrayList.Length == 1)
            {
                nameText.text = "None";
            } //end if
            else
            {
                for(int i = 0; i < arrayList.Length; i+=3)
                {
                    nameText.text += arrayList[i] + "\n";
                    methodText.text += string.IsNullOrEmpty(arrayList[i+2]) ? arrayList[i+1] + "\n" :
                        arrayList[i+2] + "\n";
                } //end for
            } //end else

            //Set pokedex text
            pokedexText.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=" + pokedexIndex);

            //Set abilities text
            abilitiesText.text = DataContents.ExecuteSQL<string>("SELECT ability1 FROM Pokemon WHERE rowid=" + pokedexIndex);
            string ability2 = DataContents.ExecuteSQL<string>("SELECT ability2 FROM Pokemon WHERE rowid=" + pokedexIndex);
            string hiddenAbility = DataContents.ExecuteSQL<string>("SELECT hiddenAbility FROM Pokemon WHERE rowid=" + pokedexIndex);
            abilitiesText.text += String.IsNullOrEmpty(ability2) ? String.IsNullOrEmpty(hiddenAbility) ? "" : 
                "," + hiddenAbility : String.IsNullOrEmpty(hiddenAbility) ? "," + ability2 : 
                "," + ability2 + "," + hiddenAbility;

            //Set weakness and resistances
            SetWeakResistSprites();

            //Turn off resistances
            resistTypes.SetActive(false);

            //Move to next checkpoint
			GameManager.instance.FadeInAnimation(2);
        } //end else if
        else if (checkpoint == 2)
        {			
            //Get player input
            GetInput();

            //Fill in index region
            FillInIndex();
            
            //Fill in highlighted sprite
            pokemonImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + 
                    pokedexIndex.ToString("000"));
            
            //Adjust stat bars
            float dbStat = DataContents.ExecuteSQL<float>("SELECT health FROM Pokemon WHERE rowid=" + pokedexIndex);
            float scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            Transform barChild = stats.transform.GetChild(1).FindChild("HPBar");
            barChild.GetComponent<RectTransform>().localScale = new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 85 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/85f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-85f)/65f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT attack FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            barChild = stats.transform.GetChild(1).FindChild("AttackBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 100 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT defence FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            barChild = stats.transform.GetChild(1).FindChild("DefenceBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 100 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT specialAttack FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            barChild = stats.transform.GetChild(1).FindChild("SpecialAttackBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 100 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT specialDefence FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
            barChild = stats.transform.GetChild(1).FindChild("SpecialDefenceBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 95 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95f)/55f);
            dbStat = DataContents.ExecuteSQL<float>("SELECT speed FROM Pokemon WHERE rowid=" + pokedexIndex);
            scale = ExtensionMethods.CapAtFloat(dbStat/130f, 1);
            barChild = stats.transform.GetChild(1).FindChild("SpeedBar");
            barChild.GetComponent<RectTransform>().localScale = 
                new Vector3(scale, 1, 1);
            barChild.GetComponent<Image>().color = dbStat < 95 ?
                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95)/35f);
            
            //Set types
            Transform typeRegion = characteristics.transform.GetChild(1).FindChild("Types");
            SetTypeSprites(typeRegion.GetChild(0).GetComponent<Image>(), typeRegion.GetChild(1).GetComponent<Image>(),
                pokedexIndex);
            
            //Set egg groups
            characteristics.transform.GetChild(1).FindChild("EggText").GetComponent<Text>().text = 
                DataContents.ExecuteSQL<string>("SELECT compatibility1 FROM Pokemon WHERE rowid=" + pokedexIndex) +
                "   " + DataContents.ExecuteSQL<string>("SELECT compatibility2 FROM Pokemon WHERE rowid=" + pokedexIndex);
            
            //Set height
            characteristics.transform.GetChild(1).FindChild("HeightText").GetComponent<Text>().text = 
                Math.Round(DataContents.ExecuteSQL<float>("SELECT height FROM Pokemon WHERE rowid=" + pokedexIndex), 1).
                ToString();
            
            //Set weight
            characteristics.transform.GetChild(1).FindChild("WeightText").GetComponent<Text>().text =
                Math.Round(DataContents.ExecuteSQL<float>("SELECT weight FROM Pokemon WHERE rowid=" +  pokedexIndex), 1).
                ToString();
            
            //Set species
            characteristics.transform.GetChild(1).FindChild("SpeciesText").GetComponent<Text>().text =
                DataContents.ExecuteSQL<string>("SELECT kind FROM Pokemon WHERE rowid=" + pokedexIndex) + " Pokemon";
            
            //Set level up moves
            string moveList = DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=" + pokedexIndex);
            string[] arrayList = moveList.Split(',');
            Text levelText = movesRegion.transform.FindChild("MoveLevel").GetComponent<Text>();
            Text moveText = movesRegion.transform.FindChild("MoveName").GetComponent<Text>();
            levelText.text = "";
            moveText.text = "";
            for(int i = 0; i < arrayList.Length-2; i+=2)
            {
                levelText.text += arrayList[i] + "\n";
                moveText.text += arrayList[i+1] + "\n";
            } //end for
            levelText.text += arrayList[arrayList.Length-2];
            moveText.text += arrayList[arrayList.Length-1];
            
            //Add egg moves to move list
            moveList = DataContents.ExecuteSQL<string>("SELECT eggMoves FROM Pokemon WHERE rowid=" + pokedexIndex);
            arrayList = moveList.Split(',');
            for(int i = 0; i < arrayList.Length; i++)
            {
                levelText.text += "\nEgg";
                moveText.text += "\n" + arrayList[i];
            } //end for
            
            //Add evolutions
            string evolveList = DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=" + pokedexIndex);
            arrayList = evolveList.Split(',');
            Text nameText = evolutionsRegion.transform.FindChild("EvolutionName").GetComponent<Text>();
            Text methodText = evolutionsRegion.transform.FindChild("EvolutionMethod").GetComponent<Text>();
            nameText.text = "";
            methodText.text = "";
            if(arrayList.Length == 1)
            {
                nameText.text = "None";
            } //end if
            else
            {
                for(int i = 0; i < arrayList.Length; i+=3)
                {
                    nameText.text += arrayList[i] + "\n";
                    methodText.text += string.IsNullOrEmpty(arrayList[i+2]) ? arrayList[i+1] + "\n" :
                        arrayList[i+2] + "\n";
                } //end for
            } //end else
            
            //Set pokedex text
            pokedexText.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=" + pokedexIndex);
            
            //Set abilities text
            abilitiesText.text = DataContents.ExecuteSQL<string>("SELECT ability1 FROM Pokemon WHERE rowid=" + pokedexIndex);
            string ability2 = DataContents.ExecuteSQL<string>("SELECT ability2 FROM Pokemon WHERE rowid=" + pokedexIndex);
            string hiddenAbility = DataContents.ExecuteSQL<string>("SELECT hiddenAbility FROM Pokemon WHERE rowid=" + pokedexIndex);
            abilitiesText.text += String.IsNullOrEmpty(ability2) ? String.IsNullOrEmpty(hiddenAbility) ? "" : 
                "," + hiddenAbility : String.IsNullOrEmpty(hiddenAbility) ? "," + ability2 : 
                "," + ability2 + "," + hiddenAbility;
            
            //Set weakness and resistances
            SetWeakResistSprites();

            //End processing
            processing = false;
        } //end else if
	} //end RunPokedex

	/***************************************
	 * Name: GetInput
	 * Gather user input and set variables
	 * as necessary
	 ***************************************/
	void GetInput()
	{
		/*********************************************
		 * Left Arrow
		 *********************************************/
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			//Regular processing
			if(checkpoint == 2)
			{
				pokedexIndex = ExtensionMethods.BindToInt(pokedexIndex-10, 1);
			} //end if
		} //end if Left Arrow

		/*********************************************
		* Right Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			//Regular processing
			if(checkpoint == 2)
			{
				if(pokedexIndex < 712)
				{
					pokedexIndex = ExtensionMethods.CapAtInt(pokedexIndex+10, 712);
				} //end if
				else
				{
					pokedexIndex = 721;
				} //end else
			} //end if
		} //end else if Right Arrow

		/*********************************************
		* Up Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			//Regular processing
			if(checkpoint == 2)
			{
				pokedexIndex = ExtensionMethods.BindToInt(pokedexIndex-1, 1);
			} //end if
		} //end else if Up Arrow

		/*********************************************
		* Down Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			//Regular processing
			if(checkpoint == 2)
			{
				pokedexIndex = ExtensionMethods.CapAtInt(pokedexIndex+1, 721);
			} //end if
		} //end else if Down Arrow

		/*********************************************
		* Mouse Moves Left
		**********************************************/
		else if (Input.GetAxis("Mouse X") < 0)
		{

		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		else if (Input.GetAxis("Mouse X") > 0)
		{

		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		else if (Input.GetAxis("Mouse Y") > 0)
		{

		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		else if (Input.GetAxis("Mouse Y") < 0)
		{

		} //end else if Mouse Moves Down

		/*********************************************
		* Mouse Wheel Up
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			//Regular processing
			if(checkpoint == 2)
			{
				pokedexIndex = ExtensionMethods.BindToInt(pokedexIndex-1, 1);
			} //end if
		} //end else if Mouse Wheel Up

		/*********************************************
		* Mouse Wheel Down
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			//Regular processing
			if(checkpoint == 2)
			{
				pokedexIndex = ExtensionMethods.CapAtInt(pokedexIndex+1, 721);
			} //end if
		} //end else if Mouse Wheel Down

		/*********************************************
		* Left Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(0))
		{

		} //end else if Left Mouse Button

		/*********************************************
		* Right Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(1))
		{
			//Regular processing
			if(checkpoint == 2)
			{
				GameManager.instance.LoadScene("MainGame", true);
			} //end if
		} //end else if Right Mouse Button

		/*********************************************
		* Enter/Return Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.Return))
		{
			//Regular processing
			if(checkpoint == 2)
			{
				ToggleShown();
			} //end if
		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{
			//Regular processing
			if(checkpoint == 2)
			{
				GameManager.instance.LoadScene("MainGame", true);
			} //end if
		} //end else if X Key
	} //end GetInput

    /***************************************
     * Name: FillInIndex
     * Sets the content for the left portion
     * of the pokedex
     ***************************************/
    void FillInIndex()
    {
        //Make sure bottom of pokedex is blocked
        if (pokedexIndex < 713)
        {
            //Fill in each slot
            for (int i = 0; i < 10; i++)
            {
                Transform child = index.transform.GetChild (i);
                int chosenPoke = pokedexIndex + i;
                child.FindChild ("Name").GetComponent<Text> ().text = chosenPoke.ToString ("000") + ":" +
                    DataContents.ExecuteSQL<string> ("SELECT name FROM Pokemon WHERE rowid=" + chosenPoke);
                child.FindChild ("Ball").GetComponent<Image> ().sprite = 
                    GameManager.instance.GetTrainer ().Owned.Contains (chosenPoke) ? 
                Resources.Load<Sprite> ("Sprites/Battle/ballnormal") :
					GameManager.instance.GetTrainer().Seen.Contains (chosenPoke) ?
                Resources.Load<Sprite> ("Sprites/Battle/ballfainted") :
					Resources.Load<Sprite> ("Sprites/Battle/ballempty");
				child.FindChild("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + 
					chosenPoke.ToString("000"));
            } //end for
        } //end if
    } //end FillInIndex

    /***************************************
     * Name: SetWeakResistSprites
     * Activates appropriate sprites for 
     * weakness/resistances
     ***************************************/
    void SetWeakResistSprites()
    {
        //Set all to inactive
        for(int i = 0; i < weakTypes.transform.childCount; i++)
        {
            weakTypes.transform.GetChild(i).gameObject.SetActive(false);
            resistTypes.transform.GetChild(i).gameObject.SetActive(false);
        } //end for

        //Get types
        int type1 = Convert.ToInt32 (Enum.Parse (typeof(Types), DataContents.ExecuteSQL<string> 
            ("SELECT type1 FROM Pokemon WHERE rowid=" + pokedexIndex)));
        int type2 = -1;
        try
        {
            type2 = Convert.ToInt32(Enum.Parse (typeof(Types), DataContents.ExecuteSQL<string> 
                ("SELECT type2 FROM Pokemon WHERE rowid=" + pokedexIndex)));
        } //end try
		catch(SystemException e){}

        //Get weaknesses
        List<int> weakList = DataContents.typeChart.DetermineWeaknesses (type1, type2);

        //Fill in weaknesses
        for (int i = 0; i < weakList.Count; i++)
        {
            weakTypes.transform.GetChild (weakList [i]).gameObject.SetActive (true);
        } //end for

        //Get resistances
        List<int> resistList = DataContents.typeChart.DetermineResistances (type1, type2);

        //Fill in resistances
        for (int i = 0; i < resistList.Count; i++)
        {
            resistTypes.transform.GetChild (resistList [i]).gameObject.SetActive (true);
        } //end for
    } //end SetWeakResistSprites

	/***************************************
	 * Name: SetTypeSprites
	 * Sets the correct sprite, or disables
	 * if a type isn't found
	 ***************************************/
	void SetTypeSprites(Image type1, Image type2, int natSpecies)
	{
		//Set the primary (first) type
		type1.gameObject.SetActive(true);
		type1.sprite = DataContents.typeSprites[Convert.ToInt32(Enum.Parse(typeof(Types),
			DataContents.ExecuteSQL<string>("SELECT type1 FROM Pokemon WHERE rowid=" + natSpecies)))];

		//Get the string for the secondary type
		string type2SQL = DataContents.ExecuteSQL<string>("SELECT type2 FROM Pokemon WHERE rowid=" + natSpecies);

		//If a second type exists, load the appropriate sprite
		if (!String.IsNullOrEmpty(type2SQL))
		{
			type2.gameObject.SetActive(true);
			type2.sprite = DataContents.typeSprites[Convert.ToInt32(Enum.Parse(typeof(Types), type2SQL))];
		} //end if
		//Otherwise disable the image
		else
		{
			type2.gameObject.SetActive(false);
		} //end else
	} //end SetTypeSprites(Image type1, Image type2, int natSpecies)

    /***************************************
     * Name: ToggleShown
     * Toggles Weakness/Resistance in Pokedex
     ***************************************/ 
    public void ToggleShown()
    {
        //If weakeness is shown, show resistances
        if (weakTypes.activeSelf)
        {
            weakTypes.SetActive (false);
            resistTypes.SetActive (true);
            shownButton.GetComponent<Text>().text = "Resists";
        } //end if
        else
        {
            weakTypes.SetActive(true);
            resistTypes.SetActive(false);
            shownButton.GetComponent<Text>().text = "Weakness";
        } //end else
    } //end ToggleShown

	/***************************************
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
		processing = false;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end class PokedexScene

