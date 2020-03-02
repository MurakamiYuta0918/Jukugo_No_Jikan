using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof (Selectable))]
public class FirstSelectButton : MonoBehaviour {
	[SerializeField]
	Selectable nextButton;
	void OnEnable() {
		Selectable button = GetComponent<Selectable> ();
		button.Select();
		EventSystem.current.SetSelectedGameObject(gameObject);
		button.OnSelect(new BaseEventData(EventSystem.current));
	}

	public void SelectNext() {
		nextButton.Select();
		EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
		nextButton.OnSelect(new BaseEventData(EventSystem.current));
	}
}
