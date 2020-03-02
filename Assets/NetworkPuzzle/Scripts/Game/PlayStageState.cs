using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkPuzzle {

    public class PlayStageState : GameState {
        PanelManager panelManager;
        LocalManager localManager;
        public readonly float timeLimit;
        Timer timer;
        public float RemainingTime { get; private set; }

        /// <summary> サーバーで点数表示処理を制御するコルーチン </summary>
        Coroutine checkCoroutine;

		public int HighScore { get; private set; }
        public int RemainingCheckNum { get; private set; }
        public EndConditionType EndCondition { get; private set; }

        public PlayStageState (Timer timer, PanelManager panelManager, LocalManager localManager, float timeLimit, int LimitOfCheck) : base(timer) {
            this.panelManager = panelManager;
            this.localManager = localManager;
            this.timer = timer;
            this.timeLimit = timeLimit;
            RemainingTime = timeLimit;
			HighScore = 0;
            RemainingCheckNum = LimitOfCheck;
            EndCondition = EndConditionType.time_over;
        }

        protected override void OnStart() {
            panelManager.SetStartPanel(false);
			panelManager.SetHichScore(HighScore);
			panelManager.SetRemainingCheckNum(RemainingCheckNum);
            localManager.OnStartStageClient();
        }

        protected override IEnumerator Routine() {
            System.Action<float> displayTime = (time =>
                {
                    panelManager.UpdatePlayTime(time);
                    RemainingTime = time;
                }
            );
            yield return StartTimeCount(timeLimit, displayTime);
        }

        public void SetActiveCheckPanel(bool isActive) {
            if (IsRunning) {
                panelManager.RpcSetCheckPanel(isActive);
            }
        }

        public void InterimResult(int collect) {
            if (IsRunning && checkCoroutine==null) {
				HighScore = HighScore<collect ? collect : HighScore;
				panelManager.SetHichScore(HighScore);

                panelManager.SetResultPanel(true, false);

				RemainingCheckNum--;
				panelManager.SetRemainingCheckNum(RemainingCheckNum);

                checkCoroutine = timer.StartCoroutine(InterimResultRoutine());
            }
        }

        IEnumerator InterimResultRoutine() {
            yield return new WaitForSeconds(5f);
            panelManager.SetResultPanel(false, false);
            localManager.OnStartStageClient();
            checkCoroutine = null;
        }

        protected override void OnStop() {
            if (checkCoroutine!=null) {
                timer.StopCoroutine (checkCoroutine);
                checkCoroutine = null;
            }

            localManager.OnEndStageClient();
            panelManager.SetEndPanel(true);
        }

        public void Stop(EndConditionType endCondition) {
            EndCondition = endCondition;
            Stop();
        }

        /// <summary> DB用終了条件 (SQL用にスネークケースで宣言）</summary>
        public enum EndConditionType {
            time_over, check_perfect, check_over
        }

    }
}
