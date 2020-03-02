using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

namespace NetworkPuzzle {
    public class GameManager : NetworkBehaviour {
        public static GameManager Instance { get; private set; }
        [SerializeField]
        StageManager stageManager;
        public StageManager StageManager { get { return stageManager; }}

        [SerializeField]
        Timer timer;

        [SerializeField]
        GameObject dataLoader;

        ILoadExercise exerciseLoader;
        IWritePlayLog playLogWriter;

        private void Awake() {
            Instance = this;
            exerciseLoader = dataLoader.GetComponent<ILoadExercise>();
            playLogWriter = new NullPlayLogWriter();
        }

        public override void OnStartServer() {
            if (KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Tutorial) {
                SetLevel(0);
                Invoke("StartGame", 2.0f);
            } 
        }

        void StopPlayersDrag() {
            PlayerPointer[] pps = FindObjectsOfType<PlayerPointer> ();
            foreach (PlayerPointer pp in pps) {
                pp.StopDrag();
            }
        }

        public void SetCheckPlayLog(int playerID, string action) {
            PlayStageState ps = timer.CurrentState as PlayStageState;
            if (ps != null) {
                playLogWriter.SetCheckPlayLog(playerID, ps.RemainingTime, action);
            }
        }

        public void SetMoveLog(int playerID, string kanjiParts, string answerfield, string action) {
            PlayStageState ps = timer.CurrentState as PlayStageState;
            if (ps != null) {
                playLogWriter.SetMoveLog(playerID, kanjiParts, answerfield, action, ps.RemainingTime);
            }
        }

        [Server]
        public IEnumerable<StageData> SetLevel(int level) {
            stageManager.SetStageList(level);
            return stageManager.StageList;
        }

        [Server]
        public bool SetUse(StageData stageData) {
            return exerciseLoader.GetHasPlayed(stageData.Id);
        }

        [Server]
        public void StartGame() {
            timer.LimitOfCheck = stageManager.LimitOfCheck;
            stageManager.RemoveUnUseStage();
            mainCoroutine = StartCoroutine(GameMain());
        }

        Coroutine mainCoroutine;
        IEnumerator GameMain() {
            while (!stageManager.IsFinish && !IsFinish) {
                stageManager.DisactiveResults();
                stageManager.GoNextStage();

                playLogWriter.SetPlayExerciseOnStart(stageManager.CurrentStageData);
                WritePlayerID();

                timer.RpcReadyToStart();
                while (timer.CurrentState == null || !timer.CurrentState.IsRunning) yield return null;
                while (timer.CurrentState.IsRunning) yield return null;

                // ステージ終了まで待つ
                timer.RpcStageStart(stageManager.CurrentStage.TimeLimit);
                while (timer.CurrentState == null || !timer.CurrentState.IsRunning) yield return null;
                while (timer.CurrentState.IsRunning) yield return null;

                StopPlayersDrag();

                int collect = stageManager.ShowCheckDetail();
                PlayStageState.EndConditionType endCondition = (timer.CurrentState as PlayStageState).EndCondition;
                if(endCondition != PlayStageState.EndConditionType.check_perfect) {
                    playLogWriter.SetJudgeLog(0.0f, collect, "time_over");
                }
                
                playLogWriter.SetPlayExerciseOnEndGame(collect, endCondition);

                timer.RpcStageEnd(collect);
                while (timer.CurrentState == null || !timer.CurrentState.IsRunning) yield return null;
                while (timer.CurrentState.IsRunning) yield return null;
            }
            IsFinish = false;

            timer.GoFinalResult();

            mainCoroutine = null;
        }

        public bool IsFinish=false;

        public void SetActiveCheckPanel(bool isActive) {
            PlayStageState ps = timer.CurrentState as PlayStageState;
            if (ps != null) {
                ps.SetActiveCheckPanel(isActive);
            }
        }

        public void Judge(string action) {
            StopPlayersDrag();
            PlayStageState ps = timer.CurrentState as PlayStageState;
            if (ps != null) {
                playLogWriter.SetJudgeLog(ps.RemainingTime, null, action);
            }
        }

        public void Check() {
            int collect = stageManager.ShowCheckDetail();
            PlayStageState ps = timer.CurrentState as PlayStageState;
            if (ps != null) {
                if (collect == stageManager.PerfectScore) {
                    ps.Stop(PlayStageState.EndConditionType.check_perfect);
                } else if (ps.RemainingCheckNum <= 1) {
                    ps.Stop(PlayStageState.EndConditionType.check_over);
                } else {
                    timer.RpcInterimResult(collect); 
                }
            }
        }

        public void GoNextStage() {
            if (timer.CurrentState is StageResult) {
                timer.CurrentState.Stop();
            }
        }

        public void SkipLevel() {
            if (mainCoroutine==null) {
                timer.GoFinalResult();
            }
        }

        void WritePlayerID() {
            PlayerPointer[] players = FindObjectsOfType(typeof(PlayerPointer)) as PlayerPointer[];

            foreach(PlayerPointer p in players) {
                playLogWriter.SetPlayMemberLog(p.playerId);
            }
        }

    }

    public class StageData {
        public readonly int Id;
        public readonly int DefaultTimeLimitSeconds;
        public readonly bool HasBePlayed;

        public int TimeLimitSeconds { get; set; }
        public bool Use { get; set; }

        public StageData (int stageId, int defaultTimeLimit, bool hasBePlayed) {
            Id = stageId;
            DefaultTimeLimitSeconds = defaultTimeLimit;
            TimeLimitSeconds = DefaultTimeLimitSeconds;
            HasBePlayed = hasBePlayed;
            Use = !hasBePlayed;
        }
    }
}