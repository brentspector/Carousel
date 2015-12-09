using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntroMenu : MonoBehaviour 
{
	//Scene variables
	GameObject title;		//Title on Intro scene
	GameObject image;		//Image on Intro scene
	int        checkpoint;	//Manages function progress
	bool	   processing;	//Currently performing task

	//Animation speed
	float damping = 0.7f;		//How fast it is
	float maxScale = 2f;		//Maximum scale
	int   bounces = 3;			//Bounces of title

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
		//If something is already happening, return
		if(processing)
		{
			return;
		} //end if

		//Get title screen objects
		if(checkpoint == 0)
		{
			processing = true;
			title = GameObject.Find("Title");
			image = GameObject.Find("Image");
			checkpoint = 1;
			processing = false;
		} //end if
		//Black out starting image and shrink title
		else if(checkpoint == 1)
		{
			processing = true;
			image.GetComponent<Image>().color = Color.black;
			title.transform.localScale = new Vector3(0.2f, 0.2f);
			checkpoint = 2;
			processing = false;
		} //end else if
		else if(checkpoint == 2)
		{
			processing = true;
			StartCoroutine(IntroAnimation());
			checkpoint = 3;
		} //end else if
		else if(checkpoint == 3)
		{
			Debug.Log("Animation finished.");
		} //end else if
	} //end Intro

	//Runs menu animations and 
	public void Menu()
	{

	} //end Menu

	//Animates intro screen
	IEnumerator IntroAnimation()
	{
		//Keep internal count of bounces
		int numBounce = 0;

		//Internal timer for lerp
		float elapsedTime = 0f;

		//Color value each bounce adds
		Color bounceColor = Color.white / 3;
		Color targetColor = bounceColor;
		Color lastColor = image.GetComponent<Image> ().color;

		//Set goal of title bounce
		Vector3 targetScale = new Vector3(maxScale, maxScale, 0);
		Vector3 lastScale = title.transform.localScale;

		while(true)
		{
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
					targetScale = new Vector3(maxScale/2, maxScale/2, 1);
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
		processing = false;
	} //end IntroAnimation
} //end IntroMenu class
