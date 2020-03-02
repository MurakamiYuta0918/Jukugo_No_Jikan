using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
/* 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す */
/* ロゴは左右左右...の順に押していけば良い． */

/// <summary>
/// タイトル画面のクラス
/// </summary>
public class TitleManager : MonoBehaviour {
	/// <summary>
	/// 隠しコマンド判定
	/// </summary>
	private bool[] HideFlags = new bool[10];

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 30;//fpsを30に設定

		gameObject.transform.SetAsFirstSibling();
		Destroy(GameObject.Find("LobbyManager"));

		GameObject resultManagerObj = GameObject.Find("Result Manager");
		if (resultManagerObj != null) {
			// gameObject.SetActive(false);
			GameObject resultSplashObj = GameObject.Find("Result Splash");
			resultSplashObj.transform.SetParent(GameObject.Find("Canvas").transform);
			resultSplashObj.transform.SetAsLastSibling();

			GameObject titleButtonObj = GameObject.Find("TitleButton");
			Button titleButton = titleButtonObj.GetComponent<Button> ();
			titleButton.onClick.AddListener(DeleteResultManager);

			DestroyImmediate(resultManagerObj);
		}

		/* 隠しコマンド判定の初期化 */
		for (int i = 0; i < HideFlags.Length; i++) {
			HideFlags [i] = false;
		}
	}

	void DeleteResultManager() {
		GameObject resultSplashObj = GameObject.Find("Result Splash");
		resultSplashObj.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
	}

	/// <summary>
	/// スタートボタン
	/// </summary>
	public void OnStartButton(){
		DontDestroyManager.DestoryAll ();
		Debug.Log ("スタートボタン");
		//databaseController.DBAct_Start(1, 1, "press");
		SceneManager.LoadScene ("LobbyScene");
	}

	/// <summary>
	/// 決められた順番で隠しコマンドが押されているかどうかを判定
	/// </summary>
	private void HideOrder(int check){
		/// <summary>
		/// 決められた順番で隠しコマンドが押されているかを判定するフラグ
		/// </summary>
		bool OrderFlag = true;

		/* 今までのコマンドが順番通りに入力されているか */
		for (int i = 0; i < check - 1; i++) {
			if (!HideFlags [i]) {
				Debug.Log ("隠しコマンド失敗！");

				for (int j = 0; j < check; j++) {
					HideFlags [j] = false;
					OrderFlag = false;
				}

				break;
			}
		}

		/* 同じコマンドを連続して押していないか */
		if (HideFlags [check - 1]) {
			Debug.Log ("隠しコマンド失敗！");

			for (int j = 0; j < check; j++) {
				HideFlags [j] = false;
				OrderFlag = false;
			}
		}

		/* 大丈夫そうならtrueにする */
		if (OrderFlag) {
			HideFlags [check - 1] = true;

			/* コンプレッドまで押し終わったら難易度を全開放して，シーン遷移 */
			if (check == 10) {
				DontDestroyManager.DestoryAll ();
				Debug.Log ("隠しコマンド成功！");
				// StageSelect.ChangeNormalFlag ();
				// StageSelect.ChangeDiffFlag ();
				SceneManager.LoadScene ("StageSelect");
			}
		}
	}

	/// <summary>
	/// 隠しコマンド1番目「デ」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton1(){
		Debug.Log ("デ");
		HideOrder (1);
	}

	/// <summary>
	/// 隠しコマンド3番目「ン」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton2(){
		Debug.Log ("ン");
		HideOrder (3);
	}

	/// <summary>
	/// 隠しコマンド5番目「デ」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton3(){
		Debug.Log ("デ");
		HideOrder (5);
	}

	/// <summary>
	/// 隠しコマンド7番目「ン」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton4(){
		Debug.Log ("ン");
		HideOrder (7);
	}

	/// <summary>
	/// 隠しコマンド8番目「ト」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton5(){
		Debug.Log ("ト");
		HideOrder (8);
	}

	/// <summary>
	/// 隠しコマンド6番目「レ」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton6(){
		Debug.Log ("レ");
		HideOrder (6);
	}

	/// <summary>
	/// 隠しコマンド4番目「イ」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton7(){
		Debug.Log ("イ");
		HideOrder (4);
	}

	/// <summary>
	/// 隠しコマンド 2番目「ン」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton8(){
		Debug.Log ("ン");
		HideOrder (2);
	}

	/// <summary>
	/// 隠しコマンド10番目 「コンプレッド」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton9(){
		Debug.Log ("コンプレッド");
		HideOrder (10);
	}

	/// <summary>
	/// 隠しコマンド 9番目「コンプイエロー」ボタン
	/// 隠しコマンド: デ ン ン イ デ レ ン ト コンプイエロー コンプレッドの順に押す
	/// ロゴは左右左右...の順に押していけば良い
	/// </summary>
	public void HideButton10(){
		Debug.Log ("コンプイエロー");
		HideOrder (9);
	}
}
