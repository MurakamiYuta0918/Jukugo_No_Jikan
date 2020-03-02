using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
/// <summary>
/// ロビーで作成された部屋を管理するクラス
/// </summary>
public class LobbyRoomList : MonoBehaviour {
	private Discovery netdisc;

	public GameObject roombutton;
	public GameObject activebutton;
	public GameObject roomristpanel;
	public GameObject NoRoomsFoundObj;
	public GameObject mainmenu;
	public GameObject lobbyRoomNameObj;
	public List<string> roomListAddress;
	int roomCount;
	private GameObject button;
	void Start () {
		roomListAddress = new List<string> ();
		roomCount = 0;
		netdisc = GameObject.Find ("LobbyManager").GetComponent<Discovery> ();
	}

	void OnDisable() {
		RemoveRoomButton();
	}
	void Update () {
		if (roomListAddress.Count == 0) {
			NoRoomsFoundObj.SetActive (true);
		} else {
			NoRoomsFoundObj.SetActive (false);
		}
	}
	/// <summary>
	/// BroadCast用の関数
	/// 部屋のボタンを追加する
	/// </summary>
	/// <param name="count">count</param>
	public void AddButtonRoom(string address,string roomName){
		button = Instantiate (roombutton,new Vector3(0,0,0),Quaternion.identity);
		button.transform.SetParent(roomristpanel.transform.Find("RoomList").transform);
		RectTransform rect = button.GetComponent<RectTransform> ();
		rect.localScale = new Vector3 (1f, 1f, 1f);
		rect.localPosition = new Vector3((rect.localPosition.x),(rect.localPosition.y + 50f),0);
		Transform trans = button.transform;
		trans.Find("HostIP").GetComponentInChildren<Text> ().text = "なまえ: " + roomName + " アドレス: " + address;
		trans.Find ("JoinButton").GetComponent<Button> ().onClick.AddListener (() => {
			netdisc.OnJoin(address);
		});
		roomListAddress.Add (address);
	}
	/// <summary>
	/// 部屋一覧を消す関数
	/// 部屋検索画面で、戻るボタンを押したときと再検索ボタンを押したときに呼ぶ
	/// </summary>
	public void RemoveRoomButton(){
		if (roomListAddress != null) {
			roomListAddress.Clear ();
		}
		roomCount = 0;
		//Debug.Log ("Button消します！");
		GameObject[] roomlist = GameObject.FindGameObjectsWithTag ("Room");
		foreach (GameObject room in roomlist) {
			//Debug.Log("roombutton消したよ");
			Destroy (room);
		}
	}
}
