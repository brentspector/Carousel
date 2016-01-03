using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class DataContents : MonoBehaviour 
{
    //Pokemon species
    [SerializeField]public List<PokemonSpecies> speciesData;
    string dataLocation = Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/";

	// Use this for initialization
	void Start () 
    {
	
	} //end Start
	
	// Update is called once per frame
	void Update () 
    {
	
	} //end Update

    public void Persist()
    {
        BinaryFormatter bf = new BinaryFormatter ();
        FileStream npf = File.Create (dataLocation + "pokemonData.dat");
        bf.Serialize (npf, speciesData);
        npf.Close ();
    } //end Persist

    //Loads data from binary file
    public bool GetPersist()
    {
        //Initalize data location
        dataLocation = Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/";
        
        //Make sure file is regional
        if(File.Exists(dataLocation + "pokemonData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter ();
            FileStream pf = File.Open (dataLocation + "pokemonData.dat", FileMode.Open);
            List<PokemonSpecies> pfd = (List<PokemonSpecies>)bf.Deserialize(pf);
            pf.Close();
            speciesData = pfd;
            return true;
        } //end if
        else
        {

            return false;
        } //end else
    } //end GetPersist
} //end DataContents class

//Pokemon Species
[Serializable]
public class PokemonSpecies
{
    public string name;
    public string type1;
    public string type2;
    public int[] baseStats;
    public string genderRate;
    public string growthRate;
    public int baseExp;
    public int[] effortPoints;
    public int catchRate;
    public int happiness;
    public string[] abilities;
    public string hiddenAbility;
    public Dictionary<int, List<string>> moves;
    public string[] eggMoves;
    public string[] compatibility;
    public int steps;
    public float height;
    public float weight;
    public string color;
    public string habitat;
    public string kind;
    public string pokedex;
    public string[] forms;
    public int battlerPlayerY;
    public int battlerEnemyY;
    public int battlerAltitude;
    public List<Evolutions> evolutions;
} //end PokemonSpecies class

//Evolution class
[Serializable]
public class Evolutions
{
   public string species;
   public string method;
   public string trigger;
} //end Evolutions