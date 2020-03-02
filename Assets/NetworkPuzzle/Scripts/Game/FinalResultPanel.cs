using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalResultPanel : MonoBehaviour {

	[SerializeField]
	GameObject goTitleButtonObj, goLevelSelectButtonObj;

	[SerializeField]
	Text totalScoreText;
	int totalScore;
	int stageNum=0;

	[SerializeField]
	StageInfo stageInfoSource;

	IFinalResult iFinalResult;

	private void OnEnable() {
		totalScoreText.text = totalScore.ToString();
	}

	private void Start() {
		iFinalResult = GetComponentInParent<IFinalResult> ();	
		Debug.Log(KanjiTime.TitleManager.mode);
		goTitleButtonObj.SetActive(iFinalResult.IsActiveGoTitleButton());
	}

	public void GoLevelSelectButtonPushed() {
		iFinalResult.OnGoLevelSelectButtonPushed();
	}

	public void GoTitleButtonPushed() {
		iFinalResult.OnGoTitleButtonPushed();
	}

	public void AddStageResult(int score) {
		StageInfo stageInfo = Instantiate(stageInfoSource, stageInfoSource.transform.parent);
		stageInfo.stageNo = ++stageNum;
		stageInfo.grade = score;
		stageInfo.gameObject.SetActive(true);

		totalScore += score;
	}

	public interface IFinalResult {
		bool IsActiveGoLevelSelectButton();
		bool IsActiveGoTitleButton();
		void OnGoLevelSelectButtonPushed();
		void OnGoTitleButtonPushed();
	}

}
