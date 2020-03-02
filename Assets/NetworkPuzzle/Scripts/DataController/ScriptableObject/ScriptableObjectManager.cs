using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NetworkPuzzle {

	public abstract class ScriptableObjectManager : MonoBehaviour, ILoadExercise {

		protected List<exercise.exercise_data> exercise;
		protected List<exercise_detail.exercise_detail_data> exercise_detail;

		public bool GetHasPlayed(int exerciseID) {
			return false;
		}

		public IEnumerable<string> GetPieceList(StageData stageData) {
			return exercise_detail.Where(row => row.exercise_id == stageData.Id).OrderBy(row => row.position).Select(row => GetPieceMaster()[row.piece_id]);
		}

		public IEnumerable<StageData> GetStageList(int level) {
			List<StageData> stageDatas = new List<StageData>();
			IEnumerable<exercise.exercise_data> rows = new List<exercise.exercise_data> ();

			if (level == 0) { // チュートリアル
				rows = exercise.Where(row => row.level == 0).OrderBy(row => row.stage);
			} else if (level == 100) { // エクストラステージ
				rows = exercise.Where(row => row.level > 100 && row.level < 1000).OrderBy(row => row.stage);
			} else if (level == 1000) { // グラフ
				rows = exercise.Where(row => row.level == 1000).OrderBy(row => row.stage);
			} else { // 簡単・難しい
				rows = exercise.Where(row => row.level == level).OrderBy(row => row.stage);
			}

			foreach(exercise.exercise_data row in rows) {
				stageDatas.Add(new StageData(row.id, row.time_limit, GetHasPlayed(row.id)));
			}

			return stageDatas;
		}

		// TODO:プレイ履歴を保存する処理を実装する
		public void SetHasPlayed(int exerciseID) {}

		protected abstract Dictionary<int, string> GetPieceMaster();
	}
}
