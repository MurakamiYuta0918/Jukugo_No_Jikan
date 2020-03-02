using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

namespace KanjiTime {
    public class TitleManager : MonoBehaviour {

        [SerializeField]
        Text startText;

		float enterTime = 0f;

		[SerializeField]
		GameObject taPanelObj;

        [SerializeField]
        Text displayIDText;

        private void Awake() {
            if (NetworkManager.singleton) {
                Destroy(NetworkManager.singleton.gameObject);
            }
        }

        // Use this for initialization
        void Start () {
            int playerId = PlayerPrefs.GetInt("player_id", 0);
            if (playerId == 0) {
                // taPanelObj.SetActive(true);
            }
            Debug.Log("PlayerID: "+playerId);
            displayIDText.text = "現在のID:" + playerId.ToString();
        }

        // Update is called once per frame
        void Update () {
                //テキストの透明度を変更する
                startText.color = new Color (0, 0, 0, Mathf.Abs(Mathf.Sin(Time.time)));
        }

        public void ReloadScene() {
            Scene loadScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(loadScene.name);
        }

        public void SetHostMode() {
            mode = Mode.Host;
            GoToLobby();
        }

        public void SetSoloPlayMode() {
            mode = Mode.Single;
            GoToLobby();
        }

        public void SetTutorialMode() {
            mode = Mode.Tutorial;
            GoToLobby();
        }

        public enum Mode { Tutorial = 0, Single = 1, Host = 2, Client = 3, Normal = 4 }
        public static Mode mode {
            get { return (Mode)PlayerPrefs.GetInt("mode", (int) Mode.Tutorial); }
            set { PlayerPrefs.SetInt("mode", (int)value); }
        }

        public void ReturnNormalMode() {
            mode = Mode.Client;
            ReloadScene();
        }

        public void PushNumButton(int num) {
            if(0 <= displayIDText.text.IndexOf("現在のID")) {
                displayIDText.text = "";
            }

            displayIDText.text += num.ToString();
        }

        public void RemoveNum() {
            int length = displayIDText.text.Length;
            if(length != 0) {
                displayIDText.text = displayIDText.text.Remove(length - 1);
            }
        }

        public void ChangePlayID() {
            try {
                PlayerPrefs.SetInt("player_id", int.Parse(displayIDText.text));
            } catch {
            }
        }

        [ContextMenu("RESET PlayID")]
        public void ResetId() {
            PlayerPrefs.DeleteKey("player_id");
        }

        [ContextMenu("RESET MODE")]
        public void ResetMode() {
            PlayerPrefs.DeleteKey("mode");
        }
        
        public void GoToLobby() {
            if (mode==Mode.Tutorial) {
                SceneManager.LoadScene(gameObject.scene.buildIndex+1);
            } else {
                SceneManager.LoadScene("Menu Screen");
            }
        }

        // TAモード発動用の関数(赤レンジャー)
        public void OnDrag() {
            enterTime += Time.deltaTime;
        }

        // TAモード発動用の関数(黄レンジャー)
        public void OnEnter() {
			if (enterTime > 3f) {
				taPanelObj.SetActive(true);
			}
        }

    }
}