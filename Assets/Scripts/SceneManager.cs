///***************************************************************************************** 
// * File:    SceneManager.cs
// * Summary: Controls behavior for each scene in the game
// *****************************************************************************************/ 
//#region Using
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//#endregion
//
//public class SceneManager : MonoBehaviour 
//{
//    #region Variables
//    //Scene Variables
//
//    //Pokedex variables
//    GameObject index;               //Region displaying overall pokedex location
//    GameObject stats;               //The stat bars of the selected pokemon
//    GameObject characteristics;     //The characteristics of the selected pokemon
//    GameObject movesRegion;         //The moves the selected pokemon can learn
//    GameObject evolutionsRegion;    //The pokemon the selected pokemon can evolve into
//    GameObject shownButton;         //Whether weaknesses or resistances are shown
//    GameObject weakTypes;           //Object containing all types selected pokemon is weak to
//    GameObject resistTypes;         //Object containing all types selected pokemon is resistant to
//    Image pokemonImage;             //The image of the selected pokemon
//    Text abilitiesText;             //The abilities the selected pokemon can have
//    Text pokedexText;               //The flavor text for the selected pokemon
//    int pokedexIndex;               //The location in the pokedex currently highlighted
//    #endregion
//
//    #region Methods
//	#region Scenes
//
//    /***************************************
//     * Name: PC
//     * Loads and plays the PC scene 
//     ***************************************/
//    public void PC()
//    {
//    } //end PC
//
//    /***************************************
//     * Name: Pokedex
//     * Loads and plays the Pokedex scene
//     ***************************************/
//    public void Pokedex()
//    {
//        //End function if processing
//        if (processing)
//        {
//            return;
//        } //end if
//
//        //Handle each stage of the scene
//        if (checkpoint == 0)
//        {
//            //Begin processing
//            processing = true;
//
//            //Initialize references
//            index = GameObject.Find("Index");
//            stats = GameObject.Find("Stats");
//            characteristics = GameObject.Find("Characteristics");         
//            movesRegion  = GameObject.Find("MovesRegion").transform.FindChild("MovesContainer").gameObject;             
//            evolutionsRegion  = GameObject.Find("Evolutions").transform.FindChild("EvolutionContainer").gameObject;        
//            shownButton = GameObject.Find("Shown");             
//            weakTypes = GameObject.Find("WeakTypes");               
//            resistTypes = GameObject.Find("ResistanceTypes");             
//            pokemonImage = GameObject.Find("Sprite").GetComponent<Image>();                 
//            abilitiesText = GameObject.Find("AbilitiesText").GetComponent<Text>();
//            pokedexText = GameObject.Find("PokedexText").GetComponent<Text>();
//            pokedexIndex = 1;
//                    
//            //Move to next checkpoint
//            checkpoint = 1;
//
//            //End processing
//            processing = false;
//        } //end if
//        else if (checkpoint == 1)
//        {
//            //Begin processing
//            processing = true;
//
//            //Fill in index region
//            FillInIndex();
//
//            //Fill in highlighted sprite
//            pokemonImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + 
//                pokedexIndex.ToString("000"));
//
//            //Adjust stat bars
//            float dbStat = DataContents.ExecuteSQL<float>("SELECT health FROM Pokemon WHERE rowid=" + pokedexIndex);
//            float scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            Transform barChild = stats.transform.GetChild(1).FindChild("HPBar");
//            barChild.GetComponent<RectTransform>().localScale = new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 85 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/85f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-85f)/65f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT attack FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("AttackBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT defence FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("DefenceBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT specialAttack FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpecialAttackBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT specialDefence FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpecialDefenceBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 95 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95f)/55f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT speed FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/130f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpeedBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 95 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95)/35f);
//
//            //Set types
//            Transform typeRegion = characteristics.transform.GetChild(1).FindChild("Types");
//            SetTypeSprites(typeRegion.GetChild(0).GetComponent<Image>(), typeRegion.GetChild(1).GetComponent<Image>(),
//                pokedexIndex);
//
//            //Set egg groups
//            characteristics.transform.GetChild(1).FindChild("EggText").GetComponent<Text>().text = 
//                DataContents.ExecuteSQL<string>("SELECT compatibility1 FROM Pokemon WHERE rowid=" + pokedexIndex) +
//                "   " + DataContents.ExecuteSQL<string>("SELECT compatibility2 FROM Pokemon WHERE rowid=" + pokedexIndex);
//
//            //Set height
//            characteristics.transform.GetChild(1).FindChild("HeightText").GetComponent<Text>().text = 
//                Math.Round(DataContents.ExecuteSQL<float>("SELECT height FROM Pokemon WHERE rowid=" + pokedexIndex), 1).
//                ToString();
//
//            //Set weight
//            characteristics.transform.GetChild(1).FindChild("WeightText").GetComponent<Text>().text =
//                Math.Round(DataContents.ExecuteSQL<float>("SELECT weight FROM Pokemon WHERE rowid=" +  pokedexIndex), 1).
//                ToString();
//
//            //Set species
//            characteristics.transform.GetChild(1).FindChild("SpeciesText").GetComponent<Text>().text =
//                DataContents.ExecuteSQL<string>("SELECT kind FROM Pokemon WHERE rowid=" + pokedexIndex) + " Pokemon";
//
//            //Set level up moves
//            string moveList = DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string[] arrayList = moveList.Split(',');
//            Text levelText = movesRegion.transform.FindChild("MoveLevel").GetComponent<Text>();
//            Text moveText = movesRegion.transform.FindChild("MoveName").GetComponent<Text>();
//            levelText.text = "";
//            moveText.text = "";
//            for(int i = 0; i < arrayList.Length-2; i+=2)
//            {
//                levelText.text += arrayList[i] + "\n";
//                moveText.text += arrayList[i+1] + "\n";
//            } //end for
//            levelText.text += arrayList[arrayList.Length-2];
//            moveText.text += arrayList[arrayList.Length-1];
//
//            //Add egg moves to move list
//            moveList = DataContents.ExecuteSQL<string>("SELECT eggMoves FROM Pokemon WHERE rowid=" + pokedexIndex);
//            arrayList = moveList.Split(',');
//            for(int i = 0; i < arrayList.Length; i++)
//            {
//                levelText.text += "\nEgg";
//                moveText.text += "\n" + arrayList[i];
//            } //end for
//
//            //Add evolutions
//            string evolveList = DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=" + pokedexIndex);
//            arrayList = evolveList.Split(',');
//            Text nameText = evolutionsRegion.transform.FindChild("EvolutionName").GetComponent<Text>();
//            Text methodText = evolutionsRegion.transform.FindChild("EvolutionMethod").GetComponent<Text>();
//            nameText.text = "";
//            methodText.text = "";
//            if(arrayList.Length == 1)
//            {
//                nameText.text = "None";
//            } //end if
//            else
//            {
//                for(int i = 0; i < arrayList.Length; i+=3)
//                {
//                    nameText.text += arrayList[i] + "\n";
//                    methodText.text += string.IsNullOrEmpty(arrayList[i+2]) ? arrayList[i+1] + "\n" :
//                        arrayList[i+2] + "\n";
//                } //end for
//            } //end else
//
//            //Set pokedex text
//            pokedexText.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=" + pokedexIndex);
//
//            //Set abilities text
//            abilitiesText.text = DataContents.ExecuteSQL<string>("SELECT ability1 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string ability2 = DataContents.ExecuteSQL<string>("SELECT ability2 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string hiddenAbility = DataContents.ExecuteSQL<string>("SELECT hiddenAbility FROM Pokemon WHERE rowid=" + pokedexIndex);
//            abilitiesText.text += String.IsNullOrEmpty(ability2) ? String.IsNullOrEmpty(hiddenAbility) ? "" : 
//                "," + hiddenAbility : String.IsNullOrEmpty(hiddenAbility) ? "," + ability2 : 
//                "," + ability2 + "," + hiddenAbility;
//
//            //Set weakness and resistances
//            SetWeakResistSprites();
//
//            //Turn off resistances
//            resistTypes.SetActive(false);
//
//            //Move to next checkpoint
//            StartCoroutine (FadeInAnimation (2));
//                        
//            //End processing
//            processing = false;
//        } //end else if
//        else if (checkpoint == 2)
//        {
//            //Begin processing
//            processing = true;
//
//            //Get player input
//            //GatherInput();
//
//            //Fill in index region
//            FillInIndex();
//            
//            //Fill in highlighted sprite
//            pokemonImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pokemon/" + 
//                    pokedexIndex.ToString("000"));
//            
//            //Adjust stat bars
//            float dbStat = DataContents.ExecuteSQL<float>("SELECT health FROM Pokemon WHERE rowid=" + pokedexIndex);
//            float scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            Transform barChild = stats.transform.GetChild(1).FindChild("HPBar");
//            barChild.GetComponent<RectTransform>().localScale = new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 85 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/85f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-85f)/65f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT attack FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("AttackBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT defence FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("DefenceBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT specialAttack FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpecialAttackBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 100 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/100f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-100f)/50f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT specialDefence FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/150f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpecialDefenceBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 95 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95f)/55f);
//            dbStat = DataContents.ExecuteSQL<float>("SELECT speed FROM Pokemon WHERE rowid=" + pokedexIndex);
//            scale = ExtensionMethods.CapAtFloat(dbStat/130f, 1);
//            barChild = stats.transform.GetChild(1).FindChild("SpeedBar");
//            barChild.GetComponent<RectTransform>().localScale = 
//                new Vector3(scale, 1, 1);
//            barChild.GetComponent<Image>().color = dbStat < 95 ?
//                Color.Lerp(Color.red, Color.yellow, (float)dbStat/95f) : 
//                Color.Lerp(Color.yellow, Color.cyan, ((float)dbStat-95)/35f);
//            
//            //Set types
//            Transform typeRegion = characteristics.transform.GetChild(1).FindChild("Types");
//            SetTypeSprites(typeRegion.GetChild(0).GetComponent<Image>(), typeRegion.GetChild(1).GetComponent<Image>(),
//                pokedexIndex);
//            
//            //Set egg groups
//            characteristics.transform.GetChild(1).FindChild("EggText").GetComponent<Text>().text = 
//                DataContents.ExecuteSQL<string>("SELECT compatibility1 FROM Pokemon WHERE rowid=" + pokedexIndex) +
//                "   " + DataContents.ExecuteSQL<string>("SELECT compatibility2 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            
//            //Set height
//            characteristics.transform.GetChild(1).FindChild("HeightText").GetComponent<Text>().text = 
//                Math.Round(DataContents.ExecuteSQL<float>("SELECT height FROM Pokemon WHERE rowid=" + pokedexIndex), 1).
//                ToString();
//            
//            //Set weight
//            characteristics.transform.GetChild(1).FindChild("WeightText").GetComponent<Text>().text =
//                Math.Round(DataContents.ExecuteSQL<float>("SELECT weight FROM Pokemon WHERE rowid=" +  pokedexIndex), 1).
//                ToString();
//            
//            //Set species
//            characteristics.transform.GetChild(1).FindChild("SpeciesText").GetComponent<Text>().text =
//                DataContents.ExecuteSQL<string>("SELECT kind FROM Pokemon WHERE rowid=" + pokedexIndex) + " Pokemon";
//            
//            //Set level up moves
//            string moveList = DataContents.ExecuteSQL<string>("SELECT moves FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string[] arrayList = moveList.Split(',');
//            Text levelText = movesRegion.transform.FindChild("MoveLevel").GetComponent<Text>();
//            Text moveText = movesRegion.transform.FindChild("MoveName").GetComponent<Text>();
//            levelText.text = "";
//            moveText.text = "";
//            for(int i = 0; i < arrayList.Length-2; i+=2)
//            {
//                levelText.text += arrayList[i] + "\n";
//                moveText.text += arrayList[i+1] + "\n";
//            } //end for
//            levelText.text += arrayList[arrayList.Length-2];
//            moveText.text += arrayList[arrayList.Length-1];
//            
//            //Add egg moves to move list
//            moveList = DataContents.ExecuteSQL<string>("SELECT eggMoves FROM Pokemon WHERE rowid=" + pokedexIndex);
//            arrayList = moveList.Split(',');
//            for(int i = 0; i < arrayList.Length; i++)
//            {
//                levelText.text += "\nEgg";
//                moveText.text += "\n" + arrayList[i];
//            } //end for
//            
//            //Add evolutions
//            string evolveList = DataContents.ExecuteSQL<string>("SELECT evolutions FROM Pokemon WHERE rowid=" + pokedexIndex);
//            arrayList = evolveList.Split(',');
//            Text nameText = evolutionsRegion.transform.FindChild("EvolutionName").GetComponent<Text>();
//            Text methodText = evolutionsRegion.transform.FindChild("EvolutionMethod").GetComponent<Text>();
//            nameText.text = "";
//            methodText.text = "";
//            if(arrayList.Length == 1)
//            {
//                nameText.text = "None";
//            } //end if
//            else
//            {
//                for(int i = 0; i < arrayList.Length; i+=3)
//                {
//                    nameText.text += arrayList[i] + "\n";
//                    methodText.text += string.IsNullOrEmpty(arrayList[i+2]) ? arrayList[i+1] + "\n" :
//                        arrayList[i+2] + "\n";
//                } //end for
//            } //end else
//            
//            //Set pokedex text
//            pokedexText.text = DataContents.ExecuteSQL<string>("SELECT pokedex FROM Pokemon WHERE rowid=" + pokedexIndex);
//            
//            //Set abilities text
//            abilitiesText.text = DataContents.ExecuteSQL<string>("SELECT ability1 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string ability2 = DataContents.ExecuteSQL<string>("SELECT ability2 FROM Pokemon WHERE rowid=" + pokedexIndex);
//            string hiddenAbility = DataContents.ExecuteSQL<string>("SELECT hiddenAbility FROM Pokemon WHERE rowid=" + pokedexIndex);
//            abilitiesText.text += String.IsNullOrEmpty(ability2) ? String.IsNullOrEmpty(hiddenAbility) ? "" : 
//                "," + hiddenAbility : String.IsNullOrEmpty(hiddenAbility) ? "," + ability2 : 
//                "," + ability2 + "," + hiddenAbility;
//            
//            //Set weakness and resistances
//            SetWeakResistSprites();
//
//            //End processing
//            processing = false;
//        } //end else if
//    } //end Pokedex
//	#endregion
//
//    #region Processing
//    /***************************************
//     * Name: FillInIndex
//     * Sets the content for the left portion
//     * of the pokedex
//     ***************************************/
//    void FillInIndex()
//    {
//        //Make sure bottom of pokedex is blocked
//        if (pokedexIndex < 713)
//        {
//            //Fill in each slot
//            for (int i = 0; i < 10; i++)
//            {
//                Transform child = index.transform.GetChild (i);
//                int chosenPoke = pokedexIndex + i;
//                child.FindChild ("Name").GetComponent<Text> ().text = chosenPoke.ToString ("000") + ":" +
//                    DataContents.ExecuteSQL<string> ("SELECT name FROM Pokemon WHERE rowid=" + chosenPoke);
//                child.FindChild ("Ball").GetComponent<Image> ().sprite = 
//                    GameManager.instance.GetTrainer ().Owned.Contains (chosenPoke) ? 
//                Resources.Load<Sprite> ("Sprites/Icons/ballnormal") :
//                Resources.Load<Sprite> ("Sprites/Icons/ballfainted");
//				child.FindChild("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/icon" + 
//					chosenPoke.ToString("000"));
//            } //end for
//        } //end if
//    } //end FillInIndex
//
//    /***************************************
//     * Name: SetWeakResistSprites
//     * Activates appropriate sprites for 
//     * weakness/resistances
//     ***************************************/
//    void SetWeakResistSprites()
//    {
//        //Set all to inactive
//        for(int i = 0; i < weakTypes.transform.childCount; i++)
//        {
//            weakTypes.transform.GetChild(i).gameObject.SetActive(false);
//            resistTypes.transform.GetChild(i).gameObject.SetActive(false);
//        } //end for
//
//        //Get types
//        int type1 = Convert.ToInt32 (Enum.Parse (typeof(Types), DataContents.ExecuteSQL<string> 
//            ("SELECT type1 FROM Pokemon WHERE rowid=" + pokedexIndex)));
//        int type2 = -1;
//        try
//        {
//            type2 = Convert.ToInt32(Enum.Parse (typeof(Types), DataContents.ExecuteSQL<string> 
//                ("SELECT type2 FROM Pokemon WHERE rowid=" + pokedexIndex)));
//        } //end try
//        catch(SystemException e){}//end catch
//
//        //Get weaknesses
//        List<int> weakList = DataContents.typeChart.DetermineWeaknesses (type1, type2);
//
//        //Fill in weaknesses
//        for (int i = 0; i < weakList.Count; i++)
//        {
//            weakTypes.transform.GetChild (weakList [i]).gameObject.SetActive (true);
//        } //end for
//
//        //Get resistances
//        List<int> resistList = DataContents.typeChart.DetermineResistances (type1, type2);
//
//        //Fill in resistances
//        for (int i = 0; i < resistList.Count; i++)
//        {
//            resistTypes.transform.GetChild (resistList [i]).gameObject.SetActive (true);
//        } //end for
//    } //end SetWeakResistSprites
//
//
//	/***************************************
//     * Name: GetCorrectSprite
//     * Returns the sprite for the pokemon,
//     * based on species, gender, shiny, and
//     * form
//     ***************************************/
//	Sprite GetCorrectSprite(Pokemon myPokemon)
//	{
//		//Get requested sprite
//		string chosenString = myPokemon.NatSpecies.ToString("000");
//		chosenString += myPokemon.Gender == 1 ? "f" : "";
//		chosenString += myPokemon.IsShiny ? "s" : "";
//		chosenString += myPokemon.FormNumber > 0 ? "_" + myPokemon.FormNumber.ToString() : "";
//
//		//Change sprite, and fix if sprite is null
//		Sprite result = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
//		if(result == null)
//		{
//			chosenString = chosenString.Replace("f", "");
//			result = Resources.Load<Sprite>("Sprites/Pokemon/" + chosenString);
//			if(result == null)
//			{
//				result = Resources.Load<Sprite>("Sprites/Pokemon/0");
//			} //end if
//		} //end if
//
//		return result;
//	} //end GetCorrectSprite(Pokemon myPokemon)
//

//
//    /***************************************
//     * Name: WaitForResize
//     * Waits for choice menu to resize before 
//     * setting selection dimensions
//     ***************************************/
//    IEnumerator WaitForResize()
//    {
//        yield return new WaitForEndOfFrame ();
//        if (gameState == MainGame.POKEMONSUBMENU || pcState == PCGame.POKEMONSUBMENU)
//        {
//            Vector3 scale = new Vector3 (choices.GetComponent<RectTransform> ().rect.width,
//                choices.GetComponent<RectTransform> ().rect.height /
//                choices.transform.childCount, 0);
//            selection.GetComponent<RectTransform> ().sizeDelta = scale;
//            selection.transform.position = choices.transform.GetChild (0).
//                GetComponent<RectTransform> ().position;
//            selection.SetActive (true);
//        } //end if
//        else if (pcState == PCGame.POKEMONMARKINGS)
//        {
//            //Reposition choices to bottom right
//            choices.GetComponent<RectTransform>().position = new Vector3(
//                choices.GetComponent<RectTransform>().position.x,
//                choices.GetComponent<RectTransform>().rect.height/2);
//
//            Vector3 scale = new Vector3 (choices.GetComponent<RectTransform> ().rect.width,
//                choices.GetComponent<RectTransform> ().rect.height /
//                choices.transform.childCount, 0);
//            selection.GetComponent<RectTransform> ().sizeDelta = scale;
//            selection.transform.position = choices.transform.GetChild (0).
//                GetComponent<RectTransform> ().position;
//            selection.SetActive (true);
//        } //end else if
//        else if (gameState == MainGame.POKEMONRIBBONS || pcState == PCGame.POKEMONRIBBONS)
//        {
//            Vector3 scale = new Vector3(ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).
//                                        GetComponent<RectTransform>().rect.width,
//                                        ribbonScreen.transform.FindChild("RibbonRegion").GetChild(0).
//                                        GetComponent<RectTransform>().rect.height
//                                        , 0);
//            selection.GetComponent<RectTransform>().sizeDelta = scale;
//            selection.transform.position = Camera.main.WorldToScreenPoint(ribbonScreen.transform.
//                FindChild("RibbonRegion").GetChild(0).GetComponent<RectTransform>().position);
//        } //end else if
//    } //end WaitForResize
//
//    /***************************************
//     * Name: WaitForFontResize
//     * Waits for move description font to
//     * resize to best fit
//     ***************************************/
//    IEnumerator WaitForFontResize(Transform moveScreen, Pokemon teamMember)
//    {
//        yield return new WaitForEndOfFrame ();
//        detailsSize = moveScreen.FindChild("MoveDescription").GetComponent<Text>().cachedTextGenerator.
//            fontSizeUsedForBestFit;
//        moveScreen.FindChild("MoveDescription").GetComponent<Text>().resizeTextForBestFit = false;
//        moveScreen.FindChild("MoveDescription").GetComponent<Text>().fontSize = detailsSize;
//        moveScreen.FindChild ("MoveDescription").GetComponent<Text> ().text = 
//            DataContents.ExecuteSQL<string> ("SELECT description FROM Moves WHERE rowid=" +
//            teamMember.GetMove (moveChoice));
//    } //end WaitForFontResize(Transform moveScreen, Pokemon teamMember)    
//    #endregion
//
//	//Miscellaneous functions
//	#region Misc
//
//    /***************************************
//     * Name: ToggleShown
//     * Toggles Weakness/Resistance in Pokedex
//     ***************************************/ 
//    public void ToggleShown()
//    {
//        //If weakeness is shown, show resistances
//        if (weakTypes.activeSelf)
//        {
//            weakTypes.SetActive (false);
//            resistTypes.SetActive (true);
//            shownButton.GetComponent<Text>().text = "Resists";
//        } //end if
//        else
//        {
//            weakTypes.SetActive(true);
//            resistTypes.SetActive(false);
//            shownButton.GetComponent<Text>().text = "Weakness";
//        } //end else
//    } //end ToggleShown
//	#endregion
//    #endregion
//} //end SceneManager class