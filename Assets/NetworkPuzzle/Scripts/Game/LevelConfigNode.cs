using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelConfigNode : MonoBehaviour {

	[SerializeField]
	int level;

	[SerializeField]
	GameObject clearImageObj;

	[SerializeField]
	Button button;

	ILevelNode iLevelNode;

	private void Start() {
		iLevelNode = GetComponentInParent<ILevelNode> ();
		try {
			button.interactable = iLevelNode.CanSelectLevel(level);
			clearImageObj.SetActive(iLevelNode.HasCleared(level));
		} catch (System.NotImplementedException e) {
			Debug.Log(e);
		}
	}

	public void OnButtonClick() {
		iLevelNode.OnLevelSelect(level);
	}

	public interface ILevelNode {
		bool HasCleared(int level);
		bool CanSelectLevel(int level);
		void OnLevelSelect(int level);
	}
}
