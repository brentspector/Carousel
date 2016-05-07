/***************************************************************************************** 
 * File:    SystemManager.cs
 * Summary: Controls error logging and player data
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Random = UnityEngine.Random;
#endregion

public class SystemManager : MonoBehaviour 
{
    #region Variables
    //INI reading
    string[] contents;      //Contents of the ini
    bool processing = false;//Whether INI is currently being read

	//Error logging
	string errorLog;		//Location of error log
	StreamWriter output;	//The error writer

    //RNG variables
    int resetRNG = 0;       //Reset the RNG values after this time
    List<float> results;    //Last results of RNG to prevent patterns

	//Persistent variables
    string dataLocation;    //Path to the save file
    Trainer pPlayer;        //The player's profile
    float pStartVersion;    //The version the game was started on
    float pPatchVersion;    //What is the most recent patch version
	float sTime;			//Where the game is when persistent was called

	//Text variables
	string message;			//Complete text to output
    GameObject textRegion;  //Region the text is displayed in
	Text textComp;			//Text currently output
	GameObject arrow;		//Arrow to signal end of text
	bool displaying;		//If text is currently being output
    bool closeBox;          //Whether or not to close the box at the end
    #endregion
    #region Methods
    #region FileReading
    /***************************************
     * Name: GetContents
     * Gathers all contents of a text files and stores them in an array of strings
     ***************************************/
    public void GetContents(string iniLocation)
    {
        if (!processing)
        {
            processing = true;
            contents = File.ReadAllLines(iniLocation);
            processing = false;
        } //end if
    } //end GetContents(string iniLocation)

    /***************************************
     * Name: ReadINI
     * Collects a line from the contents array and returns it as the generic type given
     * for an INI file
     ***************************************/
    public T ReadINI<T>(string section, string key)
    {
        //System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch ();
        //myStopwatch.Start ();
        object value = null;
        string sectionName = "[" + section + "]";
        for (int i = 0; i < contents.Length; i++)
        {
            //Check for section
            if (contents[i] == sectionName)
            {
                //Search through section
                while(true)
                {
                    //Increment and break if line starts with bracket
                    i++;
                    if(i >= contents.Length || contents[i].StartsWith("["))
                    {
                        break;
                    } //end if

                    //Check for key
                    if (contents[i].StartsWith((key)))
                    {
                        //Break line at delimiter, return 2nd part, break loop
                        string[] result = contents[i].Split ('=');
                        value = result [1];
                        break;
                    } //end if
                } //end while

                if(value == null)
                {
                    value = default(T);
                } //end if
                
                //Section finished search
                break;
            } //end if
        } //end for

        //Return the information in the type requested
        return (T)Convert.ChangeType (value, typeof(T));
    } //end ReadINI(string section, string key)

    /***************************************
     * Name: ReadCSV
     * Returns entire line of a section in contents array for a CSV file
     ***************************************/
    public string[] ReadCSV(int section)
    {
        return contents [section].Split(',');
    } //end ReadCSV(int section)
    #endregion

	#region ErrorLog
    /***************************************
     * Name: InitErrorLog
     * Initialize the error log for the play session, or create one from scratch
     ***************************************/
	public bool InitErrorLog()
	{
		//Init errorLog location
		errorLog = Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel/error.log";

        if (Directory.Exists (Environment.GetEnvironmentVariable ("USERPROFILE") + "/Saved Games/Pokemon Carousel"))
        {
            //Create or get errorLog
            output = new StreamWriter (errorLog, true);
        } //end if
        else
        {
            try
            {
                //Create directory
                Directory.CreateDirectory(Environment.GetEnvironmentVariable ("USERPROFILE") + 
                                          "/Saved Games/Pokemon Carousel");

                //Create or get errorLog
                output = new StreamWriter (errorLog, true);
            } //end try
            catch(SystemException ex)
            {
                Debug.LogError("Could not create directory because " + ex.ToString());
            } //end catch
        } //end else

		//Send a starting line
		LogErrorMessage (DateTime.Now.ToString () + " - Game was started - Version " + 
                         GameManager.instance.VersionNumber);
       
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

    /***************************************
     * Name: LogErrorMessage
     * Sends a message to the error log
     ***************************************/
	public void LogErrorMessage(string message)
	{
		//Make sure error log exists
		if(output != null)
		{
			output.WriteLine(message);
			output.Flush();
			
		} //end if
	} //end LogErrorMessage(string message)

    /***************************************
     * Name: OnApplicationQuit
     * Closes the error log
     ***************************************/
    void OnApplicationQuit()
    {
        if (output != null)
        {
            output.Close();
            output = null;
        } //end if
    } //end OnApplicationQuit
	#endregion

	#region Text
    /***************************************
     * Name: GetText
     * Prepares text for display
     ***************************************/
	public void GetText(GameObject textArea, GameObject endArrow)
	{
		//Initialize text reference and gate
        textRegion = textArea;
		textComp = textArea.transform.GetChild(0).GetComponent<Text>();
		displaying = false;

		//Disable arrow until text is finished
		arrow = endArrow;
		arrow.SetActive (false);
	} //end GetText(GameObject textArea, GameObject endArrow)

    /***************************************
     * Name: DisplayText
     * Plays the text using the animation
     ***************************************/
	public void DisplayText(string textMessage, bool closeAfter)
	{
		//If already displaying text, end function
		if(displaying)
		{
			return;
		} //end if

		//Setup message and begin typewriting
		displaying = true;
        textRegion.SetActive (true);
		arrow.SetActive (false);
		message = textMessage;
        closeBox = closeAfter;
		StartCoroutine(TypeText ());
	} //end DisplayText(string textMessage, bool closeAfter)

    /***************************************
     * Name: TypeText
     * Displays text letter by letter
     ***************************************/
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

    /***************************************
     * Name: ManageTextbox
     * Close box if requested
     ***************************************/
    public bool ManageTextbox(bool immediate)
    {
        //Proceed when text is finished and player hits enter
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonUp(0) || immediate) && !displaying)
        {
            //Close box if requested
            if(closeBox)
            {
                textRegion.SetActive (false);
                arrow.SetActive (false);
            } //end if

            //Finished displaying message
            return false;
        } //end if

        //Still displaying message
        return true;
    } //end ManageTextbox(bool immediate)
	#endregion

    #region Random
    /***************************************
     * Name: RandomInt
     * Creates a random integer between mix
     * and max, excluding max. Reseeds occasionally
     * to prevent the same number from appearing
     ***************************************/
    public int RandomInt(int min, int max)
    {
        //Reset the RNG if this is zero
        if (resetRNG == 0)
        {
            //Clear results
            results = new List<float>();

            uint value = (uint)(System.DateTime.Now.Ticks & 0xbbbbbbbb) + (uint)(Random.seed & 0x7f7f7f7f);
            if(value % 2 != 0)
            {
                value ^= (uint)Random.Range(0, Mathf.Pow(2, 31));
            } //end if

            //Set the value as the seed
            Random.seed = (int)value;

            //Reset counter
            resetRNG = Random.Range(10, 100);
        } //end if

        //Decrement counter
        resetRNG--;

        //Generate random number
        float generated = Random.value;

        //Check that random number isn't a repeating pattern
        if (results.Contains (generated))
        {
            LogErrorMessage("Repeating pattern detected. Seed: " + Random.seed +
                            " Value: " + generated);
            resetRNG = 0;
        } //end if
        else
        {
            results.Add(generated);
        } //end else

        generated = Random.Range (min, max);
        return (int)generated;
    } //end RandomInt(int min, int max)
    #endregion

	#region Persistent
    /***************************************
     * Name: Persist
     * Persists data to a binary file
     ***************************************/
	public void Persist()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		CalcTime ();
		//If a file is regional
		if(File.Exists(dataLocation + "game.dat"))
		{
            pPlayer.BackUps++;
			if(pPlayer.BackUps > 10)
			{
				File.Delete(dataLocation + pPlayer.PlayerName + (pPlayer.BackUps-10) + ".dat");
			} //end if
			FileStream npf = File.Create (dataLocation + "gameT.dat");
			PersistentSystem npfd = new PersistentSystem ();
            npfd.player = pPlayer;
            npfd.startVersion = pStartVersion;
            npfd.patchVersion = pPatchVersion;
			bf.Serialize (npf, npfd);
			npf.Close ();
			//File.Replace(dataLocation + "gameT.dat", dataLocation+ "game.dat", dataLocation + 
            //             pPlayer.PlayerName + pPlayer.BackUps + ".dat");
        } //end if
		//If not
		else
		{
			FileStream pf = File.Create (dataLocation + "game.dat");
			PersistentSystem pfd = new PersistentSystem ();
            pfd.player = pPlayer;
            pfd.startVersion = pStartVersion;
            pfd.patchVersion = pPatchVersion;
			bf.Serialize (pf, pfd);
			pf.Close ();
		} //end else
	} //end Persist

    /***************************************
     * Name: GetPersist
     * Loads data from binary file
     ***************************************/
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
            if(pfd.patchVersion != GameManager.instance.VersionNumber)
            {
                GameManager.instance.LogErrorMessage("File patch version doesn't match game version");
                pPlayer = Patch.PatchFile(pfd.player, pfd.patchVersion);
                GameManager.instance.LogErrorMessage("Updated trainer file");
            } //end if
            else
            {
                pPlayer = pfd.player;
                pStartVersion = pfd.startVersion;
                pPatchVersion = pfd.patchVersion;
            } //end else
			return true;
		} //end if
		else
		{
            NewGameReset();
			return false;
		} //end else
	} //end GetPersist

    /***************************************
     * Name: NewGameReset
     * Clears data to create new game file
     ***************************************/
    public void NewGameReset(bool savePrevious = false)
    {
        //Rename previous if requested to save
        if (savePrevious)
        {
            BinaryFormatter bf = new BinaryFormatter ();
            FileStream npf = File.Create (dataLocation + "gameT.dat");
            PersistentSystem npfd = new PersistentSystem ();
            npfd.player = pPlayer;
            npfd.startVersion = pStartVersion;
            npfd.patchVersion = pPatchVersion;
            bf.Serialize (npf, npfd);
            npf.Close ();
            //File.Replace(dataLocation + "gameT.dat", dataLocation + "game.dat", dataLocation + 
            //             pPlayer.PlayerName + pPlayer.HoursPlayed + "h" + pPlayer.MinutesPlayed + "m.dat");
            File.Delete(dataLocation + "game.dat");
        } //end if
        pPlayer = new Trainer();
        pStartVersion = GameManager.instance.VersionNumber;
        pPatchVersion = GameManager.instance.VersionNumber;
    } //end NewGameReset(bool savePrevious = false)

    /***************************************
     * Name: StartTime
     * Begins the player's play session time
     ***************************************/
    public void StartTime()
    {
        sTime = Time.time;
    } //end StartTime

    /***************************************
     * Name: PlayerTrainer
     * Get/Set the player trainer
     ***************************************/
    public Trainer PlayerTrainer
    {
        get
        {
            return pPlayer;
        } //end get
        set
        {
            pPlayer = value;
        } //end set
    } //end PlayerTrainer

    /***************************************
     * Name: CalcTime
     * Determines total play time from this session
     ***************************************/
	private void CalcTime()
	{
		//Calculate total seconds, rounded up
		float seconds = Mathf.Ceil((Time.time-sTime) + pPlayer.SecondsPlayed);

		//Calculate total minutes
		float minutes = Mathf.Floor (seconds / 60f) + pPlayer.MinutesPlayed;

		//Calculate total hours
		float hours = Mathf.Floor(minutes / 60f) + pPlayer.HoursPlayed;

		//Set hours
		pPlayer.HoursPlayed = (int)hours;

		//Set leftover minutes
		pPlayer.MinutesPlayed = (int)minutes % 60;

		//Set leftover seconds
		pPlayer.SecondsPlayed = (int)seconds % 60;
	} //end if
	#endregion
    #endregion
} //end SystemManager class

//Class of persistent data
[Serializable]
class PersistentSystem
{
    public float startVersion;
    public float patchVersion;
    public Trainer player;
} //end PersistentSystem class
