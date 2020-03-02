using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DontDestroyOnLoadに保存機能を設けたクラス
/// </summary>
public class DontDestroyManager : MonoBehaviour {

	/// <summary>
	/// DontDestroyOnLoadをしているオブジェクトを格納するリスト
	/// </summary>
	private static List<GameObject> DestroyObjects = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// DontDestroyOnLoadに保存機能を設けた関数
	/// </summary>
	public static void DontDestroyOnLoad(GameObject obj) {
		Debug.Log ("DontDestroyOnLoad");

		Object.DontDestroyOnLoad(obj);
		DestroyObjects.Add (obj);
	}

	/// <summary>
	/// DontDestroyOnLoadで保存したオブジェクトを全消去
	/// </summary>
	public static void DestoryAll() {
		Debug.Log ("DestoryAll");

		foreach (var obj in DestroyObjects) {
			GameObject.Destroy (obj);
		}

		DestroyObjects.Clear ();
	}
}
