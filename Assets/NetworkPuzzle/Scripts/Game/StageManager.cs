using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

namespace NetworkPuzzle {
    public abstract class StageManager : NetworkBehaviour {

        public StageController CurrentStage { get; protected set; }
        public StageData CurrentStageData { get; protected set; }
        public virtual bool IsFinish { get { return stages.Count==0; } }
        public abstract int PerfectScore { get; }
        public virtual int LimitOfCheck { get { return 4; }}
        Queue<StageData> stages;
		public IEnumerable<StageData> StageList { get { return stages; }}
		protected ILoadExercise exerciseLoader;
		protected ICheckFunction checker;

        public void SetStageList(int level) {
			List<StageData> stageList = exerciseLoader.GetStageList(level).ToList();
            stages = new Queue<StageData> (stageList);
        }

        public void RemoveUnUseStage() {
            var stageList = stages.ToList();
            stageList.RemoveAll(stage => !stage.Use);
            stages = new Queue<StageData> (stageList);
        }

        public bool IsLoop { get; set; }

        public virtual void GoNextStage() {
            if (CurrentStage != null) {
                CurrentStage.Remove();
                CurrentStage = null;
            }

            if (!IsLoop) {
                CurrentStageData = stages.Dequeue();
            }

            List<Piece> pieceList = GenerateStage(CurrentStageData);
            int timeLimit = CurrentStageData.TimeLimitSeconds;
            CurrentStage = new StageController(timeLimit, pieceList);
        }

        protected abstract List<Piece> GenerateStage(StageData stageData);

        public Piece FindParts(string partsName) {
            return CurrentStage.FindPart(partsName);
        }

        public abstract int ShowCheckDetail();
        public abstract void DisactiveResults();

        public class StageController {
            public int TimeLimit { get; private set; }
            readonly Dictionary<string, Piece> parts = new Dictionary<string, Piece> ();

            public List<Piece> Pieces { get { return parts.Values.ToList(); } }

            internal StageController(int timeLimit, List<Piece> pieceList) {
                TimeLimit = timeLimit;
                pieceList.ForEach(piece => parts.Add(piece.PieceName, piece));
            }

            public Piece FindPart(string partName) {
                return parts[partName];
            }

            internal void Remove() {
                foreach (var part in parts.Values) {
                    NetworkServer.Destroy(part.gameObject);
                }
                parts.Clear();
            }

        }
    }
}