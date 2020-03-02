using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary> ステージ選択を管理 </summary>
public class StageSelect : MonoBehaviour {

	[SerializeField]
	private GameObject NormalModeObj, DifficultModeObj;
	/// <summary> 通信エラー画面を出力 </summary>
	private GameObject ErrorSplashObj;
	[SerializeField]
	private GameObject stageSplashObj;
	[SerializeField]
	public GameObject roomNamePanelObj;
	[SerializeField]
	private List<GameObject> stageControllerPrefabList;
	[SerializeField]
	private GameObject[] valueObjects;
	[SerializeField]
	private PlayerNumber playerNumber;
	[SerializeField]
	private Prototype.NetworkLobby.LobbyManager lobbyManager;
	[SerializeField]
	private GameObject stageTimeCounter;
	/// <summary> プレイするステージの番号。DBに登録する際に使用。</summary>
	private int level;
	public int Level {get { return level; }}
	List<int> easyClearedStageNum;

	void OnEnable() {
		var lobbyManager = Prototype.NetworkLobby.LobbyManager.s_Singleton;
		roomNamePanelObj.SetActive(true);
		lobbyManager.SetRoomName();
    }

	void Start () {
        List<GameObject> oList = Prototype.NetworkLobby.LobbyManager.s_Singleton.spawnPrefabs;
        stageControllerPrefabList = new List<GameObject> ();
        foreach (GameObject obj in oList) {
            if (obj.tag == "StageController") {
                stageControllerPrefabList.Add(obj);
            }
        }

		easyClearedStageNum = new List<int> ();
		for (int i=0; i<10; i++) {
			easyClearedStageNum.Add(i);
		}
		// TODO: ここにDBからクリア済みステージを読み出す処理を追加
	}
	
	/// <summary> もどるボタン </summary>
	public void UndoButton(){
		Debug.Log ("Undo");
		SceneManager.LoadScene ("title_scene");
	}

	/// <summary> エラー画面を出力する </summary>
	public void RpcError(){
		Debug.Log ("エラー画面");

		ErrorSplashObj.SetActive (true);
	}

	/// <summary> 通信エラー画面のタイトルに戻るボタン </summary>
	public void OnBackTitle() {
		DontDestroyManager.DestoryAll ();
		Debug.Log ("タイトルへ戻るボタン");
		SceneManager.LoadScene ("title_scene");
	}
		
	/// <summary> やさしいボタン </summary>
	public void EasyButton(){
		Debug.Log ("EasyButton");
		foreach (GameObject valueButtonObj in valueObjects) {
			valueButtonObj.GetComponent<Button> ().interactable = false;
		}
		foreach(int clearNum in easyClearedStageNum) {
			valueObjects[clearNum].GetComponent<Button> ().interactable = true;
		}
		
		stageSplashObj.gameObject.SetActive(true);
	}
		
	/// <summary> ふつうボタン </summary>
	public void NormalButton(){
		Debug.Log ("NormalButton");
		// ChangeDiffFlag();
		stageSplashObj.gameObject.SetActive(true);
	}
		
	/// <summary> むずかしいボタン </summary>
	public void DifficultButton(){
		Debug.Log ("DifficultButton");
		stageSplashObj.gameObject.SetActive(true);
	}
	public void CreateRoomButton(){
		this.gameObject.SetActive (true);
	}
	public void SampleCreateRoomButton(){
		this.gameObject.SetActive (true);
		roomNamePanelObj.SetActive(true);
		lobbyManager.SetRoomName();
	}
	public void SelectStageButton(GameObject stageNumberObj){
		level = Int32.Parse (stageNumberObj.GetComponentInChildren<Text> ().text);
		GameObject stageListPrefab = stageControllerPrefabList[level - 1];
		//GameManager.StageControllerPrefab = stageListPrefab;

		if (playerNumber.PlayMode) {
			RoomNameEnter();
			lobbyManager.RoomNameEnter ();
		} else {
			stageSplashObj.gameObject.SetActive (false);
			roomNamePanelObj.gameObject.SetActive (true);
		}
	}

	public void RoomNameEnter(){
		stageSplashObj.gameObject.SetActive(true);
		roomNamePanelObj.gameObject.SetActive(false);
		this.gameObject.SetActive (false);
	}
	public void InstanceStageTimeCount(int count){
		if (GameObject.FindGameObjectWithTag("TimeCounter") == null) {
			GameObject timeCounter = Instantiate (stageTimeCounter, Vector3.zero, Quaternion.identity);
		}
	}
}
