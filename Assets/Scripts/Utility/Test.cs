using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Test : MonoBehaviour {
	
	BoxCollider2D bc;

	void Awake()
	{
		bc = this.gameObject.GetComponent<Button>().GetComponent<BoxCollider2D>();
		Debug.Log(bc.bounds.min + ", " + bc.bounds.max);
		Physics.queriesHitTriggers = true;
	} 

	void OnMouseEnter()
	{
		Debug.Log(EventSystem.current.currentSelectedGameObject.name);
		EventSystem.current.SetSelectedGameObject(this.gameObject);
		Debug.Log(EventSystem.current.currentSelectedGameObject.name);
	}
}

