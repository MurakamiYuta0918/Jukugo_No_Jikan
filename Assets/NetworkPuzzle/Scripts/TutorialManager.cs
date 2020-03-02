using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

    public void GoLobbyScene() {
        if (KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Tutorial) {
            SceneManager.LoadScene("lobby");
        } else {
            SceneManager.LoadScene(gameObject.scene.buildIndex+1);
        }
    }

}
