using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour {

	IResultTrans iResultTrans { get { return FindObjectOfType<PanelManager> (); }}

	[SerializeField]
	GameObject nextButtonObj, finishButtonObj;

	private void Start() {
		finishButtonObj.SetActive(false);
	}

	/// <summary> リザルトパネルの表示/非表示 </summary>
	/// <param name="isActive"> リザルトパネルの表示/非表示 </param>
	/// <param name="CanGoNext"> 次へボタンの表示/非表示 </param>
	public void SetActive(bool isActive, bool CanGoNext) {
		if (gameObject.activeSelf != isActive) {
			gameObject.SetActive(isActive);
		}
		
		if (!isActive) return;
		// 「次へボタン」の表示設定
		bool isActiveGoButton = CanGoNext && iResultTrans.IsActiveGoNextButton();
		if ((nextButtonObj.activeSelf != isActiveGoButton)) {
			nextButtonObj.SetActive(isActiveGoButton);
		}
	}

	/// <summary> 次へボタンが呼ばれたときの処理 </summary>
	public void GoNextPushed() {
		iResultTrans.OnGoNextPushed();
	}

	/// <summary> 終了ボタンが押されたときの処理 </summary>
	public void FinishPushed() {
		iResultTrans.OnFinishPushed();
	}

	public interface IResultTrans {
		bool IsActiveGoNextButton();
		bool IsActiveFinishButton();
		void OnGoNextPushed();
		void OnFinishPushed();
	}
}
