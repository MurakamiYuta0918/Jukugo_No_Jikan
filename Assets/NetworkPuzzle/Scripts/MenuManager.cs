using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour {
    
    private void Awake() {
        if (NetworkManager.singleton) {
            Destroy(NetworkManager.singleton.gameObject);
        }
    }

    public void GoTutorial() {
        SceneManager.LoadScene(gameObject.scene.buildIndex-1);
    }

    public void GoLobby(int mode) {
        KanjiTime.TitleManager.mode = (KanjiTime.TitleManager.Mode)mode;
        SceneManager.LoadScene("lobby");
    }

}
