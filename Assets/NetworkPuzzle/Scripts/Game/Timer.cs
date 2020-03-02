using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

namespace NetworkPuzzle {
    
    public class Timer : NetworkBehaviour {

        [SerializeField]
        PanelManager panelManager;

        [SerializeField]
        LocalManager localManager;

        [SyncVar]
        public int LimitOfCheck=4;

        public bool IsPlay { get; private set; }
        public void SwitchPlayStop() { IsPlay = !IsPlay; }

        private void Start() {
            IsPlay = true;
        }

        [Server]
        public void ActiveStageSetting() {
            panelManager.ActiveStageSetting();
        }

        public GameState CurrentState { get; private set; }
        public void GoNextState(GameState nextState) {
            if (CurrentState!=null && CurrentState.IsRunning) {
                CurrentState.Stop();
            } 
            CurrentState = nextState;
            CurrentState.Start();
        }

        [ClientRpc]
        public void RpcInterimResult(int collect) {
            PlayStageState ps = CurrentState as PlayStageState;
            if (ps != null) {
                ps.InterimResult(collect);
            }
        }

        [ClientRpc]
        public void RpcReadyToStart() {
            GoNextState(new ReadyToStart(this, panelManager));
        }

        [ClientRpc]
        public void RpcStageStart (int timeLimit) {
            GoNextState(new PlayStageState(this, panelManager, localManager, timeLimit, LimitOfCheck));
        }

        [ClientRpc]
        public void RpcStageEnd (int collect) {
			PlayStageState ps = CurrentState as PlayStageState;
			int highScore = 0;
			if (ps != null) {
				highScore = ps.HighScore;
			}

            GoNextState(new StageResult(this, panelManager, collect, highScore));
        }

        [Server]
        public void GoFinalResult() {
            panelManager.RpcSetFinalResultPanel(true);
        }

    }

    public class ReadyToStart : GameState {
        PanelManager panelManager;
        public readonly float LimitTime = 5.2f;

        public ReadyToStart(Timer timer, PanelManager panelManager) : base(timer) {
            this.panelManager = panelManager;
        }

        protected override void OnStart() {
            panelManager.SetWaitPanel(false);
            panelManager.SetFinalResultPanel(false);
            panelManager.SetResultPanel(false, false);
            panelManager.SetStartPanel(true);
        }

        protected override IEnumerator Routine() {
            System.Action<float> displayTime = (time => panelManager.UpdateStartTime(time));
            yield return StartTimeCount(LimitTime, displayTime);
        }

    }

    public class StageResult : GameState {
        PanelManager panelManager;
        int result;
		int highScore;

        public StageResult(Timer timer, PanelManager panelManager, int result, int highScore) : base(timer) {
            this.panelManager = panelManager;
            this.result = result;
			this.highScore = result > highScore ? result : highScore;
        }

        protected override void OnStart() {
			panelManager.SetHichScore(highScore);
			panelManager.AddStageInfo(highScore);
            panelManager.SetResultPanel(true, true);
        }

        protected override IEnumerator Routine() {
            yield return new WaitForSeconds(2.0f);
            panelManager.SetEndPanel(false);

            while (true) yield return null;
        }

        protected override void OnStop() {
            panelManager.SetEndPanel(false);
        }
    }
}
