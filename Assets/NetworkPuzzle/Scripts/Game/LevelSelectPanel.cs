using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : MonoBehaviour {

	public static bool IsActiveGraphLevel {
		get { return PlayerPrefs.GetInt("IsActiveGraphLevel", 0) != 0; }
		set { PlayerPrefs.SetInt("IsActiveGraphLevel", value ? 1 : 0); }
	}

	[SerializeField]
	GameObject commandPanel;

	[SerializeField]
	GameObject graphLevelButtonObj;

	[SerializeField]
	Toggle CanStageSelectToggle;

	private void OnEnable() {
		PassReset();
		CanStageSelectToggle.isOn = CanStageSelect;
	}

	private void Start() {
		if (KanjiTime.TitleManager.mode != KanjiTime.TitleManager.Mode.Host
			&& !IsActiveGraphLevel) {
			// graphLevelButtonObj.SetActive(false);
		}
	}

	List<GameObject> droppedObjList = new List<GameObject> ();
	public void OnDrop(UnityEngine.EventSystems.BaseEventData eventData) {
		GameObject droppedObj = eventData.selectedObject;
		if (!droppedObjList.Contains(droppedObj)) {
			droppedObjList.Add(droppedObj);
		} else {
			droppedObjList = new List<GameObject> ();
		}

	}

	float time = 0f;
	public void OnDrag(UnityEngine.EventSystems.BaseEventData eventData) {
		if (eventData.selectedObject!=null && eventData.selectedObject.name=="husen") {
			time += Time.deltaTime;

			if (time > 3f && droppedObjList.Count>3) {
				commandPanel.SetActive(true);
				PassReset();
			}

		} else {
			time = 0f;
		}
	}

	public void PassReset() {
		time = 0f;
		droppedObjList = new List<GameObject> ();
	}

	public void GraphOpenButtonPushed() {
		IsActiveGraphLevel = !IsActiveGraphLevel;
		// graphLevelButtonObj.SetActive(IsActiveGraphLevel);
	}

	public bool CanStageSelect {
		get { return PlayerPrefs.GetInt("CanStageSelect", 0) != 0; }
		set { PlayerPrefs.SetInt("CanStageSelect", value? 1:0); }
	}

}
