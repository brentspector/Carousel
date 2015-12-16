using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntroMenu : MonoBehaviour 
{
	//Scene variables
	GameObject title;			//Title on Intro scene
	GameObject image;			//Image on Intro scene
	GameObject enter;			//Press enter to start
	int        checkpoint;		//Manages function progress
	bool	   processing;		//Currently performing task

	//Animation variables
	bool  playing = false;		//Animation is playing
	float damping = 0.8f;		//How fast it is
	float maxScale = 2f;		//Maximum scale
	int   bounces = 3;			//Bounces of title

	//Menu variables
	GameObject selection;		//Red selection rectangle
	GameObject choice;			//All the menu choices
	int choiceNumber = 0;		//What choice is highlighted 

	//Use for initialization
	void Awake()
	{
		//Begin checkpoint at zero
		checkpoint = 0;

		//Begin processing as false
		processing = false;
	} //end Awake

	//Runs intro animation
	public void Intro()
	{
		//If something is already happening, end it or return
		if(processing)
		{
			//If player cancels animation
			if(Input.GetKeyDown(KeyCode.Return) && playing)
			{
				playing = false;
				checkpoint = 3;
			} //end if
			else
			{
				return;
			} //end else
		} //end if

		//Get title screen objects
		if(checkpoint == 0)
		{
			processing = true;
			title = GameObject.Find("Title");
			image = GameObject.Find("Image");
			enter = GameObject.Find("PressEnter");
			checkpoint = 1;
			processing = false;
		} //end if
		//Black out starting image, shrink title, hide enter
		else if(checkpoint == 1)
		{
			processing = true;
			image.GetComponent<Image>().color = Color.black;
			title.transform.localScale = new Vector3(0.2f, 0.2f);
			enter.SetActive(false);
			checkpoint = 2;
			processing = false;
		} //end else if
		//Play animation
		else if(checkpoint == 2)
		{
			processing = true;
			playing = true;
			StartCoroutine(IntroAnimation());
		} //end else if
		//End animation and move to menu when player hits enter/return
		else if(checkpoint == 3)
		{
			processing = true;

			if(Input.GetKeyDown(KeyCode.Return))
			{
				Application.LoadLevel("StartMenu");
				checkpoint = 0;
			} //end if
		
			processing = false;
		} //end else if
	} //end Intro

	//Animates intro screen
	IEnumerator IntroAnimation()
	{
		//Keep internal count of bounces
		int numBounce = 0;
		
		//Internal timer for lerp
		float elapsedTime = 0f;
		
		//Colors
		Color bounceColor = Color.white / bounces;					//Divdes color up into equal parts
		Color targetColor = bounceColor;							//Initializes first target color
		Color lastColor = image.GetComponent<Image> ().color;		//Initializes starting color
		
		//Title
		Vector3 targetScale = new Vector3(maxScale, maxScale, 0);	//Initializes title to bounce to highest
		Vector3 lastScale = title.transform.localScale;				//Initializes last scale size

		//Loop doesn't break until break command is reached
		while(true)
		{
			//If player ends the animation early
			if(!playing)
			{
				break;
			} //end if
			
			//Reset bounce target if target met
			if(title.transform.localScale == targetScale)
			{
				//Go to goal if bounces have been met
				if(numBounce == bounces - 1)
				{
					targetScale = new Vector3(1f, 1f);
				} //end if
				//If final bounce is done, break
				else if(numBounce == bounces)
				{
					break;
				} //end else if
				//If even number of bounces
				else if(numBounce%2 == 0)
				{
					//Get a value between 0.2 and 1
					float value = 0.2f + ((0.8f/(float)bounces) * (numBounce + 1));
					targetScale = new Vector3(value, value, 1);
				} //end else if
				//If odd number
				else
				{
					//Get a value between 1 and maxScale
					float value = maxScale - (((maxScale-1)/bounces) * numBounce);
					targetScale = new Vector3(value, value, 1);
				} //end else
				
				//Reset color target
				targetColor = targetColor+bounceColor;
				
				//Reset last positions
				lastColor = image.GetComponent<Image>().color;
				lastScale = title.transform.localScale;
				
				//Increase bounce count
				numBounce++;
				
				//Reset elapsed time
				elapsedTime = 0f;
			} //end if
			
			//Lighten image over bounces
			image.GetComponent<Image>().color = Color.Lerp(lastColor, targetColor, elapsedTime * damping);
			
			//Bounce title
			title.transform.localScale = Vector3.Lerp(lastScale, targetScale, elapsedTime * damping);
			
			//Increase elapsedTime by dt
			elapsedTime += Time.deltaTime;
			
			yield return null;
		} //end while
		
		//Make sure image color is white
		image.GetComponent<Image>().color = Color.white;
		
		//Make sure title scale is 1
		title.transform.localScale = new Vector3 (1f, 1f);
		
		//Process is finished
		enter.SetActive (true);
		checkpoint = 3;
		playing = false;
		processing = false;
	} //end IntroAnimation

	//Runs menu animations and 
	public void Menu()
	{
		//If something is already happening, return
		if(processing)
		{
			return;
		} //end if

		// Get menu objects
		if(checkpoint == 0) 
		{
			processing = true;
			selection = GameObject.Find ("Selection");
			choice = GameObject.Find ("ChoiceGrid");
			selection.SetActive(false);
			checkpoint = 1;
			processing = false;
		} //end if
		// Initialize selection rectangle
		else if(checkpoint == 1)
		{
			processing = true;
			selection.SetActive(true);
			selection.GetComponent<RectTransform>().sizeDelta = 
				new Vector2(choice.transform.GetChild(choiceNumber).GetComponent<RectTransform>().sizeDelta.x, 40);
			selection.transform.position = new Vector3(choice.transform.GetChild(choiceNumber).position.x, 
			                                           choice.transform.GetChild(choiceNumber).position.y-8, 0);
			processing = false;
			checkpoint = 2;
		} //end else if
		//Run menu
		else if(checkpoint == 2)
		{
			processing = true;
			//If down arrow is pressed
			if(Input.GetKeyDown(KeyCode.DownArrow))
			{
				//Increase choice to show next option is highlighted
				choiceNumber++;
				//Loop back to start if it's higher than the amount of choices
				if(choiceNumber >= choice.transform.childCount)
				{
					choiceNumber = 0;
				} //end if

				//Resize to choice width
				selection.GetComponent<RectTransform>().sizeDelta = 
					new Vector2(choice.transform.GetChild(choiceNumber).GetComponent<RectTransform>().sizeDelta.x, 40);

				//Reposition to choice location
				selection.transform.position = new Vector3(choice.transform.GetChild(choiceNumber).position.x, 
				                                           choice.transform.GetChild(choiceNumber).position.y-8, 0);
			} //end if

			//If up arrow is pressed
			if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				//Decrease choice to show previous option is highlighted
				choiceNumber--;
				//Loop to last choice if it's lower than zero
				if(choiceNumber < 0)
				{
					choiceNumber = choice.transform.childCount-1;
				} //end if

				//Resize to choice width
				selection.GetComponent<RectTransform>().sizeDelta = 
					new Vector2(choice.transform.GetChild(choiceNumber).GetComponent<RectTransform>().sizeDelta.x, 40);

				//Reposition to choice location
				selection.transform.position = new Vector3(choice.transform.GetChild(choiceNumber).position.x, 
				                                           choice.transform.GetChild(choiceNumber).position.y-8, 0);
			} //end if

			//If an option was selected, process it
			if(Input.GetKeyDown(KeyCode.Return))
			{
				// First choice selected, this is usually continue
				if(choiceNumber == 0)
				{
					GameObject.Find("ContinueGrid").transform.GetChild(0).GetComponent<Text>().color = Color.blue;
				} //end if
				// Second choice selected, this is usually new game
				else if(choiceNumber == 1)
				{
					checkpoint = 0;
					Application.LoadLevel("NewGame");
				} //end else if
				// Third choice selected, this is usually options
				else if(choiceNumber == 2)
				{
					GameObject.Find("ContinueGrid").transform.GetChild(0).GetComponent<Text>().color = Color.yellow;
				} //end else if
			} //end if

			//Menu finished this check
			processing = false;
		} //end else if
	} //end Menu

	//Used for new game introduction
	public void NewGame()
	{
		//Beginning scene
		if(checkpoint == 0)
		{
			processing = true;
			GameManager.instance.InitText(GameObject.Find("Speech").GetComponent<Text>());
			processing = false;
			checkpoint = 1;
		} //end NewGame
	} //end NewGame
} //end IntroMenu class
