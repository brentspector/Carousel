﻿/***************************************************************************************** 
 * File:    PC.cs
 * Summary: Handles pokemon storage outside of team
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
#endregion

[Serializable]
public class PC
{
    #region Variables
    Pokemon[][] pokemonStorage;     //Storage for each box of pokemon
    string[] boxNames;              //Name of each storage box
    int[] wallpaper;                //Wallpaper each box has
    int currentBox;                 //The box active for storage/reading
    int totalPokemon;               //How many pokemon are in the box
    #endregion

    #region Methods
    /***************************************
     * Name: PC
     * Initializes PC variables
     ***************************************/
    public PC()
    {
        //If the game manager exists
        if (GameManager.instance != null)
        {
            //Set up pokemon storage
            pokemonStorage = new Pokemon[50][];

            for(int i = 0; i <pokemonStorage.Length; i++)
            {
                pokemonStorage[i] = new Pokemon[30];
            } //end for

            //No pokemon are in the box
            totalPokemon = 0;

            //Set up box names
            boxNames = new string[50];

            for (int i = 0; i < boxNames.Length; i++)
            {
                boxNames [i] = "Box " + i;
            } //end for

            //Set up wallpaper
            wallpaper = new int[50];

            for (int i = 0; i < wallpaper.Length; i++)
            {
                wallpaper [i] = i % GameManager.instance.NumberOfWallpaper;
            } //end for
        } //end if
    } //end PC

    /***************************************
     * Name: GetPC
     * Retrieves pokemon at specified box 
     * and slot
     ***************************************/
    public Pokemon GetPC(int box, int spot)
    {
        return pokemonStorage [box][spot];
    } //end GetPC(int box, int spot)

    /***************************************
     * Name: AddToPC
     * Places a pokemon at the specified 
     * spot in the specified box
     ***************************************/
    public void AddToPC(int box, int spot, Pokemon newPokemon)
    {
        //Make sure PC isn't full
        if (totalPokemon == pokemonStorage.Length)
        {
            GameManager.instance.DisplayText("PC is full. Pokemon could not be added");
            return;
        } //end if

        //If the spot is empty, fill it
        if (pokemonStorage [box][spot] == null)
        {
            pokemonStorage [box][spot] = newPokemon;
        } //end if
        
        //Otherwise look for the first unfilled spot
        else
        {
            //Search the requested box first
            for(int i = 0; i < 30; i++)
            {
                //Place pokemon in empty spot and end function
                if(pokemonStorage[box][i]  == null)
                {
                    pokemonStorage[box][i] = newPokemon;
                    return;
                } //end if
            } //end for

            //Search the entire pc
            for(int i = 0; i < pokemonStorage.Length; i++)
            {
                for(int j = 0; j < 30; j++)
                {
                    //Place pokemon in empty spot and end function
                    if(pokemonStorage[i][j] == null)
                    {
                        GameManager.instance.DisplayText(boxNames[box] + " is full." +
                           "Placed in box \"" + boxNames[i] + "\" instead.");
                        pokemonStorage [i][j] = newPokemon;
                        currentBox = i;
                        return;
                    } //end if
                } //end for
            } //end for

            //Increment totalPokemon
            totalPokemon++;
        } //end else
    } //end SetPC(int box, int spot, Pokemon newPokemon)

    /***************************************
     * Name: RemoveFromPC
     * Removes a pokemon from a box slot
     ***************************************/
    public void RemoveFromPC(int box, int spot)
    {
        //Set spot to null
        pokemonStorage [box][spot] = null;

        //Decrement total pokemon
        totalPokemon--;
    } //end RemoveFromPC(int box, int spot)

    /***************************************
     * Name: GetPCBox
     * Return the current box
     ***************************************/
    public int GetPCBox()
    {
        return currentBox;
    } //end GetPCBox

    /***************************************
     * Name: GetPCBoxName
     * Return the current box name
     ***************************************/
    public string GetPCBoxName()
    {
        return boxNames[currentBox];
    } //end GetPCBoxName

    /***************************************
     * Name: GetPCBoxWallpaper
     * Return the current box wallpaper
     ***************************************/
    public int GetPCBoxWallpaper()
    {
        return wallpaper[currentBox];
    } //end GetPCBoxWallpaper

    /***************************************
     * Name: PreviousBox
     * Sets the PC to the previous box
     ***************************************/
    public void PreviousBox()
    {
        //Decrease the currentBox
        currentBox--;

        //Clamp between 0 and 49
        if (currentBox < 0)
        {
            currentBox = 49;
        } //end if
    } //end PreviousBox
    
    /***************************************
     * Name: NextBox
     * Sets the PC to the next box
     ***************************************/
    public void NextBox()
    {
        //Increase the currentBox
        currentBox++;
        
        //Clamp between 0 and 49
        if (currentBox > 49)
        {
            currentBox = 0;
        } //end if
    } //end NextBox

    /***************************************
     * Name: GetLastPokemon
     * Return the last non null pokemon in box
     ***************************************/
    public Pokemon GetLastPokemon()
    {
        Pokemon[] boxArray = pokemonStorage [currentBox];
        return boxArray.Last (pokemon => pokemon != null);
    } //end GetLastPokemon

    /***************************************
     * Name: GetFirstPokemon
     * Return the first non null pokemon in box
     ***************************************/
    public Pokemon GetFirstPokemon()
    {
        Pokemon[] boxArray = pokemonStorage [currentBox];
        return boxArray.First (pokemon => pokemon != null);
    } //end GetFirstPokemon

    /***************************************
     * Name: GetLastPokemonIndex
     * Return the index of the last non null 
     * pokemon in box
     ***************************************/
    public int GetLastPokemonIndex()
    {
        Pokemon[] boxArray = pokemonStorage [currentBox];
        return Array.FindIndex (boxArray, pokemon => pokemon == GetLastPokemon ());
    } //end GetLastPokemonIndex
    
    /***************************************
     * Name: GetFirstPokemonIndex
     * Return the index of the first non null 
     * pokemon in box
     ***************************************/
    public int GetFirstPokemonIndex()
    {
        Pokemon[] boxArray = pokemonStorage [currentBox];
        return Array.FindIndex (boxArray, pokemon => pokemon == GetFirstPokemon ());
    } //end GetFirstPokemonIndex
    #endregion
} //end class PC