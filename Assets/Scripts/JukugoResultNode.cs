using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace NetworkPuzzle.JukugoMode {
	public class JukugoResultNode : NetworkBehaviour {

		[SerializeField]
		Sprite correctImage, incorrectImage;

		[SerializeField]
		Text jukugoText, phoneticText;

		[SerializeField]
		Image JudgeImage;

		[SerializeField]
		GameObject dataLoader;

		IJukugoGetPhonetic phoneticLoader;


		void Awake() {
			phoneticLoader = dataLoader.GetComponent<IJukugoGetPhonetic>();
		}

		[ClientRpc]
		public void RpcSetNode(string jukugo) {
			gameObject.SetActive(true);
			jukugoText.text = jukugo;

			string phonetic = phoneticLoader.GetPhonetic(jukugo[0], jukugo[1]);
			phoneticText.text = phonetic;

			if (phonetic != "") {
				JudgeImage.sprite = correctImage;
			} else {
				JudgeImage.sprite = incorrectImage;
			}
		}

		[ClientRpc]
		public void RpcStopShow() {
			gameObject.SetActive(false);
		}
	}
}
