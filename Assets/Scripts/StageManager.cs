using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

namespace NetworkPuzzle.JukugoMode {
    public class StageManager : NetworkPuzzle.StageManager {

        [SerializeField]
        GameObject dataLoader;

        public override int PerfectScore { get { return 5; }}
        [SerializeField]
        KanjiPiece piecePrefab;

        [SerializeField]
        JukugoMode.AnswerField[] answerFields;
        
        [SerializeField]
        Transform[] kanjiFields;

        [SerializeField]
        Transform kanjiFieldTransform;

		[SerializeField]
		JukugoResultNode[] nodes;

		private void Awake() {
			exerciseLoader = dataLoader.GetComponent<ILoadExercise>();
			checker = dataLoader.GetComponent<ICheckFunction>();
		}

        public override int ShowCheckDetail() {
            int collectNum = 0;
            for(int i = 0; i < answerFields.Length - 1; i++) {
                string leftKanji = answerFields[i].PieceName;
                string rightKanji = answerFields[i+1].PieceName;
                bool check = checker.CheckFunction(leftKanji, rightKanji);

                if(check) {
                    collectNum++;
                }

				if (leftKanji != "" && rightKanji != "") {
					nodes[i].RpcSetNode(leftKanji + rightKanji);
				} else {
					nodes[i].RpcStopShow();
				}
            }

            return collectNum;
		}
        public override void DisactiveResults() { }

        protected override List<Piece> GenerateStage(StageData stageData) {
			List<string> partNameList = exerciseLoader.GetPieceList(stageData).ToList();

            if (partNameList.Count == 0) {
                return null;
            }

            List<Piece> pieceList = new List<Piece> ();
            for (int i=0; i<kanjiFields.Length; i++) {
                KanjiPiece kanjiPiece = Instantiate(piecePrefab, kanjiFieldTransform);
                kanjiPiece.StartParent = kanjiFields[i];
                KanjiData kanjiData = kanjiPiece.GetKanjiData();
                kanjiData.SetKanji(partNameList[i]);
                pieceList.Add(kanjiPiece);

                NetworkServer.Spawn(kanjiPiece.gameObject);
            }

            return pieceList;
        }

        public interface KanjiData {
            void SetKanji(string kanji);
        }

    }
}