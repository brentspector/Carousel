﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SystemManager : MonoBehaviour 
{
	//Error logging
	string errorLog;		//Location of error log
	string errorData;		//The details of the error
	StreamWriter output;	//The error writer

	//Persistent variables
	string dataLocation;	//Location of persistent data
	int bups;				//Number of bups
	string pName;			//The player's name
	int pBadges;			//Amount of badges player has
	int pHours;				//Total hours player has spent
	int pMinutes;			//Remaining minutes player has that aren't hours
	int pSeconds;			//Remaining seconds player has that aren't minutes
	float sTime;			//Where the game is when persistent was called

	//Text variables
	string message;			//Complete text to output
	Text textComp;			//Text currently output
	GameObject arrow;		//Arrow to signal end of text
	bool displaying;		//If text is currently being output

	#region ErrorLog
	//Initialize the error log
	public bool InitErrorLog()
	{
		//Init errorLog location
		errorLog = Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/error.log";

		//Append data if a file already exists
		try
		{
			output = new StreamWriter (errorLog, true);
		} //end try
		//Manage exception
		catch(System.Exception ex)
		{
			//If directory doesn't exist
			if(ex.GetType() == typeof(DirectoryNotFoundException))
			{
				//Create directory
				System.IO.Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") + 
				                                    "/Saved Games/Pokemon Carousel");

				//Create error log
				output = new StreamWriter (errorLog, true);
			} //end if
		} //end catch
		//Send a starting line
		LogErrorMessage (DateTime.Now.ToString () + " - Game was started.");

		//Report whether successful
		if(output != null)
		{
			return true;
		} //end if
		else
		{
			return false;
		} //end if
	} //end InitErrorLog

	//Sends a message to error log
	public void LogErrorMessage(string message)
	{
		//Make sure error log exists
		if(output != null)
		{
			output.WriteLine(message);
			output.Flush();
			
		} //end if
	} //end LogErrorMessage(string message)

	//Closes error log
	void CloseErrorLog()
	{
		//Make sure an error log exists
		if(output != null)
		{
			output.Close();
			output = null;
		} //end if
	} //end CloseErrorLog
	#endregion

	#region Text
	//Used for text initialization
	public void GetText(GameObject textArea, GameObject endArrow)
	{
		//Initialize text reference and gate
		textComp = textArea.GetComponent<Text>();
		displaying = false;

		//Disable arrow until text is finished
		arrow = endArrow;
		arrow.SetActive (false);
	} //end GetText(GameObject textArea, GameObject endArrow)

	//Display text
	public bool PlayText(string textMessage)
	{
		//If already displaying text, end function
		if(displaying)
		{
			return false;
		} //end if

		//Setup message and begin typewriting
		displaying = true;
		arrow.SetActive (false);
		message = textMessage;
		StartCoroutine(TypeText ());
		return true;
	} //end PlayText(string textMessage)

	//Whether the text has finished displaying
	public bool GetDisplay()
	{
		return displaying;
	} //end GetDisplay

	//Types text a letter or more at a time
	IEnumerator TypeText () 
	{
		//Clear output and convert message to characters
		textComp.text = "";
		char[] letters = message.ToCharArray ();
		for (int i = 0; i < letters.Length; i++) 
		{
			//Display 1 letter at a time if FPS is high
			if(Time.deltaTime < 0.05f)
			{
				textComp.text += letters[i];		
			} //end if
			//Display 2 letters at a time if FPS is slower
			else if(Time.deltaTime < 0.1f)
			{
				textComp.text += letters[i];
				textComp.text += letters[i+1];
				i++;
			} //end else if
			//Display 3 letters at a time if FPS is terrible
			else if(Time.deltaTime <0.3f)
			{
				textComp.text += letters[i];
				textComp.text += letters[i+1];
				textComp.text += letters[i+2];
				i+=2;
			} //end else if
			//Display whole message if FPS is unbearable
			else
			{
				textComp.text = message;
				i = letters.Length;
			} //end else
			yield return null;
		} //end for

		//Finished displaying text
		displaying = false;
		arrow.SetActive (true);
	} //end TypeText
	#endregion

	#region Persistent
	//Persists to binary file
	public void Persist()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		CalcTime ();
		//If a file is regional
		if(File.Exists(dataLocation + "game.dat"))
		{
			bups++;
			if(bups > 10)
			{
				File.Delete(dataLocation + pName + (bups-10) + ".dat");
			} //end if
			FileStream npf = File.Create (dataLocation + "gameT.dat");
			PersistentSystem npfd = new PersistentSystem ();
			npfd.pName = pName;
			npfd.bups = bups;
			npfd.pBadges = pBadges;
			npfd.pHours = pHours;
			npfd.pMinutes = pMinutes;
			npfd.pSeconds = pSeconds;
			bf.Serialize (npf, npfd);
			npf.Close ();
			File.Replace(dataLocation + "gameT.dat", dataLocation+ "game.dat", dataLocation + pName + bups + ".dat");
		} //end if
		//If not
		else
		{
			FileStream pf = File.Create (dataLocation + "game.dat");
			PersistentSystem pfd = new PersistentSystem ();
			pfd.version = GameManager.instance.VersionNumber;
			pfd.pName = pName;
			pfd.bups = 0;
			pfd.pBadges = pBadges;
			pfd.pHours = pHours;
			pfd.pMinutes = pMinutes;
			pfd.pSeconds = pSeconds;
			bf.Serialize (pf, pfd);
			pf.Close ();
		} //end else
	} //end Persist

	//Loads data from binary file
	public bool GetPersist()
	{
		//Initalize data location
		dataLocation = Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/";

		//Make sure file is regional
		if(File.Exists(dataLocation + "game.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream pf = File.Open (dataLocation + "game.dat", FileMode.Open);
			PersistentSystem pfd = (PersistentSystem)bf.Deserialize(pf);
			pf.Close();
			pName = pfd.pName;
			bups = pfd.bups;
			pBadges = pfd.pBadges;
			pHours = pfd.pHours;
			pMinutes = pfd.pMinutes;
			pSeconds = pfd.pSeconds;
            Debug.Log(pfd.aPokemon.GetValues());
			sTime = Time.time;
			return true;
		} //end if
		else
		{
			pName = "-";
			pBadges = 0;
			pHours = 0;
			pMinutes = 0;
			pSeconds = 0;
			sTime = Time.time;
			return false;
		} //end else
	} //end GetPersist

	//Set persistent name
	public void SetName(string newName)
	{
		pName = newName;
	} //end SetName(string newName)

	//Get persistent name
	public string GetPName()
	{
		return pName;
	} //end GetPName

	//Set persistent badges
	public void SetBadges(int badges)
	{
		pBadges = badges;
	} //end SetBadges(int badges)

	//Get persistent badges
	public int GetBadges()
	{
		return pBadges;
	} //end GetBadges

	//Get persistent hours
	public int GetHours()
	{
		return pHours;
	} //end GetHours

	//Get persistent minutes
	public int GetMinutes()
	{
		return pMinutes;
	} //end GetMinutes

	//Calculate time
	private void CalcTime()
	{
		//Calculate total seconds, rounded up
		float seconds = Mathf.Ceil((Time.time-sTime) + pSeconds);

		//Calculate total minutes
		float minutes = Mathf.Floor (seconds / 60f) + pMinutes;

		//Calculate total hours
		float hours = Mathf.Floor(minutes / 60f) + pHours;

		//Set hours
		pHours = (int)hours;

		//Set leftover minutes
		pMinutes = (int)minutes % 60;

		//Set leftover seconds
		pSeconds = (int)seconds % 60;

	} //end if
	#endregion
} //end SystemManager class

//Class of persistent data
[Serializable]
class PersistentSystem
{
	public float version;
	public int bups;
	public string pName;
	public int pBadges;
	public int pHours;
	public int pMinutes;
	public int pSeconds;
    public Pokemon aPokemon;
} //end PersistentSystem class
