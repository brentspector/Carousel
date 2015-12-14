using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Test : MonoBehaviour {

	GameObject selection;
	GameObject choice;
	int childNumber = 0;

	// Use this for initialization
	void Start () 
	{
		selection = GameObject.Find ("Selection");
		choice = GameObject.Find ("ChoiceGrid");
		selection.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!selection.activeSelf)
		{
			selection.SetActive(true);
			selection.GetComponent<RectTransform>().sizeDelta = 
				new Vector2(choice.transform.GetChild(childNumber).GetComponent<RectTransform>().sizeDelta.x, 40);
			selection.transform.position = new Vector3(choice.transform.GetChild(childNumber).position.x, 
			                                           choice.transform.GetChild(childNumber).position.y-8, 0);
			if(selection.GetComponent<RectTransform>().sizeDelta.x <= 0)
			{
				selection.SetActive(false);
			} 
			return;
		}

		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			childNumber++;
			if(childNumber >= choice.transform.childCount)
			{
				childNumber = 0;
			}
			selection.SetActive(true);
			selection.GetComponent<RectTransform>().sizeDelta = 
				new Vector2(choice.transform.GetChild(childNumber).GetComponent<RectTransform>().sizeDelta.x, 40);
			selection.transform.position = new Vector3(choice.transform.GetChild(childNumber).position.x, 
			                                           choice.transform.GetChild(childNumber).position.y-8, 0);
		}
		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			childNumber--;
			if(childNumber < 0)
			{
				childNumber = 2;
			}
			selection.SetActive(true);
			selection.GetComponent<RectTransform>().sizeDelta = 
				new Vector2(choice.transform.GetChild(childNumber).GetComponent<RectTransform>().sizeDelta.x, 40);
			selection.transform.position = new Vector3(choice.transform.GetChild(childNumber).position.x, 
			                                           choice.transform.GetChild(childNumber).position.y-8, 0);
		}
		if(Input.GetKeyDown(KeyCode.Return))
		{
			if(childNumber == 0)
			{
				GameObject.Find("ContinueGrid").transform.GetChild(0).GetComponent<Text>().color = Color.blue;
			}
			else if(childNumber == 1)
			{
				GameObject.Find("ContinueGrid").transform.GetChild(0).GetComponent<Text>().color = Color.green;
			}
			else if(childNumber == 2)
			{
				GameObject.Find("ContinueGrid").transform.GetChild(0).GetComponent<Text>().color = Color.yellow;
			}
		}
	}
}
