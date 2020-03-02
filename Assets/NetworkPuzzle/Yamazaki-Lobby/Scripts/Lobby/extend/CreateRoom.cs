using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* へやにはいる or つくるを行うクラス */
public class CreateRoom : MonoBehaviour {

	public GameObject ErrorSplashObj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/* もどるボタン */
	public void UndoButton(){
		Debug.Log ("Undo");
		SceneManager.LoadScene ("PlayerNumber");
	}

	/* へやをつくるボタン */
	public void CreateButton(){
		Debug.Log ("Create");
		SceneManager.LoadScene ("RoomList");
	}

	/* へやにはいるボタン */
	public void EnterButton(){
		Debug.Log ("Enter");
		SceneManager.LoadScene ("RoomEnter");
	}

	/* 通信エラー画面を出力 */
	public void RpcError(){
		Debug.Log ("エラー画面");

		ErrorSplashObj.SetActive (true);
	}

	/* 通信エラー画面のタイトルに戻るボタン */
	public void OnBackTitle() {
		DontDestroyManager.DestoryAll ();
		Debug.Log ("タイトルへ戻るボタン");
		SceneManager.LoadScene ("title");
	}
}
