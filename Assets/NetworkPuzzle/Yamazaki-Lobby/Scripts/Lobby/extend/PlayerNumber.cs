using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> プレイヤー人数を選択するクラス </summary>
public class PlayerNumber : MonoBehaviour {
	[SerializeField]
	bool isTA=false;
	public bool IsTA {
		get { return isTA; }
		set { isTA = value; }
	}
	private bool playMode = true;//true: シングル,false: マルチ
	public bool PlayMode{ get{return playMode;} }

	[SerializeField]
	private GameObject singlePlayPanel;

	/// <summary> もどるボタン </summary>
	public void UndoButton(){
		Debug.Log ("Undo");
		SceneManager.LoadScene ("StageSelect");
	}

	/// <summary> ひとりボタン </summary>
	public void SingleButton(){
		Debug.Log ("Single");
		playMode = true;
		gameObject.SetActive (false);
		var lobbyManager = Prototype.NetworkLobby.LobbyManager.s_Singleton;
		lobbyManager.SetRoomName();
	}

	/// <summary> みんなボタン </summary>
	public void MultiButton(){
		Debug.Log ("Multi");
		gameObject.SetActive (false);
		playMode = false;
	}

	public void ClickBackButton(){
		gameObject.SetActive (true);
		singlePlayPanel.SetActive (false);
	}

	public void BackTitleButton(){
		SceneManager.LoadScene ("TitleScene");
	}
}
