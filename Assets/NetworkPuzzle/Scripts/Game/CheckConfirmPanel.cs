using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckConfirmPanel : MonoBehaviour {

	[SerializeField]
	Text remainingCheckNumText;

	public int RemainCheckNum {
		set { remainingCheckNumText.text = value.ToString(); }
	}

	[SerializeField]
	Text waitText, confirmText;

	[SerializeField]
	Button agreeButton;

	ICheckAgree iCheckAgree;

	private void OnEnable() {
		agreeButton.gameObject.SetActive(true);
		waitText.gameObject.SetActive(false);
		confirmText.gameObject.SetActive(true);
	}

	private void Awake() {
		iCheckAgree = GetComponentInParent<ICheckAgree> ();
		if (iCheckAgree == null) {
			Debug.LogError("Could not find ICheckAgree");
		}
	}

	public void OnAgreePushed() {
		agreeButton.gameObject.SetActive(false);
		waitText.gameObject.SetActive(true);
		confirmText.gameObject.SetActive(false);
		iCheckAgree.OnCheckAgreeClicked();
	}

	public void OnDisAgreePushed() {
		iCheckAgree.OnCheckDisagreeClicked();
	}
	
	private void Reset() {
		if (iCheckAgree == null) {
			Debug.LogError("Could not find ICheckAgree");
		}
	}

	public interface ICheckAgree {
		void OnCheckAgreeClicked();
		void OnCheckDisagreeClicked();
	}

}
