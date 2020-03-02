using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using NetworkPuzzle;

public class PanelManager : NetworkBehaviour, CheckConfirmPanel.ICheckAgree, ResultPanel.IResultTrans, FinalResultPanel.IFinalResult, StageSettingPanel.IStageSetting, LevelConfigNode.ILevelNode  {
	
	public static bool CanStageSelect {
		get { return PlayerPrefs.GetInt("CanStageSelect", 0) != 0; }
		set { PlayerPrefs.SetInt("CanStageSelect", value? 1:0); }
	}

	[SerializeField]
	Text highScoreText;
	[SerializeField]
	GameObject waitPanel, startPanel, endPanel;

	[SerializeField]
	StageSettingPanel stageSettingPanel;

	[SerializeField]
	CheckConfirmPanel checkConfirmPanel;

	[SerializeField]
	ResultPanel resultPanel;

	[SerializeField]
	FinalResultPanel finalResultPanel;

	[SerializeField]
	GameObject levelSelectPanel;

	[SerializeField]
	Text startTime, playTimeText;

	public override void OnStartServer() {
		resultPanel.gameObject.SetActive(false);
		ActiveStageSetting();
	}

	public void ActiveStageSetting() {
		if (KanjiTime.TitleManager.mode != KanjiTime.TitleManager.Mode.Tutorial) {
			levelSelectPanel.SetActive(true);
			stageSettingPanel.gameObject.SetActive(true);
		}
	}

	/// <param name="level"></param>
	[Server]
	public void SelectLevel(int level) {
		IEnumerable<StageData> stageList = GameManager.Instance.SetLevel(level);
		if (!CanStageSelect && KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Single) {
			StartGame();
			stageSettingPanel.gameObject.SetActive(false);
		} else {
			stageSettingPanel.AddConfigNodeSource(stageList);
		}

		levelSelectPanel.SetActive(false);
	}

	[Server]
	public void StartGame() {
		GameManager.Instance.StartGame();
	}

	[Client]
	public void PushedCheck(bool isActive) {
		Piece.MyPointer.CmdPushCheck(isActive);
	}

	[Client]
	void CheckConfirmPanel.ICheckAgree.OnCheckAgreeClicked() {
		Piece.MyPointer.CmdAgreeCheck();
	}

	[Client]
	void CheckConfirmPanel.ICheckAgree.OnCheckDisagreeClicked() {
		Piece.MyPointer.CmdPushCheck(false);
	}
	
	public void SetWaitPanel(bool isActive){
		waitPanel.SetActive(isActive);
	}
	
	[ClientRpc]
	public void RpcSetCheckPanel(bool isActive) {
		if (isActive) {
			FindObjectOfType<LocalManager> ().OnEndStageClient();
		} else {
			FindObjectOfType<LocalManager> ().OnStartStageClient();
		}
		SetCheckPanel(isActive);
	}

	public void SetCheckPanel (bool isActive) {
		checkConfirmPanel.gameObject.SetActive(isActive);
	}

	public void SetStartPanel(bool active){
		startPanel.SetActive(active);
	}

	public void UpdateStartTime(float currentTime) {
        if (currentTime < 0.8f) {
            startTime.text = "スタート！";
        }
        else {
            startTime.text = currentTime.ToString("N0");
        }
	}

	public void UpdatePlayTime(float currentTime) {
		playTimeText.text = currentTime.ToString("N0");
	}

	public void SetRemainingCheckNum(int remainingCheckNum) {
		checkConfirmPanel.RemainCheckNum = remainingCheckNum;
	}

    public void SetEndPanel(bool active) {
        endPanel.SetActive(active);
    }

	public void SetHichScore(int highScore) {
		highScoreText.text = highScore.ToString();
	}

	public void SetResultPanel(bool active, bool CanGoNext) {
		SetCheckPanel(false);
		resultPanel.SetActive(active, CanGoNext);
	}

	bool LevelConfigNode.ILevelNode.HasCleared(int level) {
		throw new System.NotImplementedException();
	}

	bool LevelConfigNode.ILevelNode.CanSelectLevel(int level) {
		throw new System.NotImplementedException();
	}

	void LevelConfigNode.ILevelNode.OnLevelSelect(int level) {
		SelectLevel(level);
	}

	void StageSettingPanel.IStageSetting.OnOKButtonPushed() {
		StartGame();
	}

	bool ResultPanel.IResultTrans.IsActiveGoNextButton() {
		return KanjiTime.TitleManager.mode != KanjiTime.TitleManager.Mode.Client;
	}

	bool ResultPanel.IResultTrans.IsActiveFinishButton() {
		return KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Host
		 || KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Single;
	}

	void ResultPanel.IResultTrans.OnGoNextPushed() {
		GameManager.Instance.GoNextStage();
	}

	void ResultPanel.IResultTrans.OnFinishPushed() {
		GameManager.Instance.IsFinish = true;
		GameManager.Instance.GoNextStage();
	}

	public void AddStageInfo(int point) {
		finalResultPanel.AddStageResult(point);
	}

	[ClientRpc]
	public void RpcSetFinalResultPanel(bool isActive){
		SetWaitPanel(true);
		SetFinalResultPanel(isActive);
	}

	[Client]
	public void SetFinalResultPanel(bool isActive) {
		finalResultPanel.gameObject.SetActive(isActive);
	}

	public void GoFinalResult() {
		GameManager.Instance.SkipLevel();
	}

	bool FinalResultPanel.IFinalResult.IsActiveGoLevelSelectButton() {
		return KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Single 
			|| KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Host; 
	}

	bool FinalResultPanel.IFinalResult.IsActiveGoTitleButton() {
		return KanjiTime.TitleManager.mode != KanjiTime.TitleManager.Mode.Client;
	}

	void FinalResultPanel.IFinalResult.OnGoLevelSelectButtonPushed() {
		ActiveStageSetting();
		SetFinalResultPanel(false);
	}

	void FinalResultPanel.IFinalResult.OnGoTitleButtonPushed() {
		if (KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Tutorial) {
			KanjiTime.TitleManager.mode = KanjiTime.TitleManager.Mode.Single;
		}
		NetworkManager.singleton.StopHost();
		SceneManager.LoadScene("title");
	}

}
