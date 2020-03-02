using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkPuzzle {
	public abstract class DBManager : MonoBehaviour, ILoadExercise, IWritePlayLog {

		protected SqliteDatabase sqlDB;
		int currentPlayExerciseId = 0;
		Dictionary<int, int> players = new Dictionary<int, int>();

		/// <summary> プレイメンバーのログをDBに保存 </summary>
		public void SetPlayMemberLog(int playID) {
			string query
			= string.Format("INSERT INTO play_member (play_exercise_id ,playID) ")
			+ string.Format("VALUES({0},{1})", currentPlayExerciseId, playID);
			sqlDB.ExecuteQuery(query);

			query
			= "SELECT id FROM play_member "
			+ "ORDER BY id DESC";
			players.Add(playID, (int)sqlDB.ExecuteQuery(query).Rows[0]["id"]);
		}

		// プレイログで書き込んでいるため空実装
		public void SetHasPlayed(int exerciseID) {}

		/// <summary> ステージ終了時のログがちゃんと保存してあるか確認 </summary>
		public bool GetHasPlayed(int exerciseId) {
			string query
			= string.Format("SELECT id FROM play_exercise ")
			+ string.Format("WHERE exercise_id={0} ", exerciseId)
			+ string.Format("AND point IS NOT NULL");

			return sqlDB.ExecuteQuery(query).Rows.Count != 0;
		}

		/// <summary> ゲーム終了時に終了時間などのログをDBに保存 </summary>
		public void SetPlayExerciseOnEndGame(int point, PlayStageState.EndConditionType endCondition) {
			string query
			= string.Format("UPDATE play_exercise SET ")
			+ string.Format("end_time={0},point={1},end_condition='{2}' ", CurrentUnixTime, point, endCondition)
			+ string.Format("WHERE id={0}", currentPlayExerciseId);
			sqlDB.ExecuteQuery(query);
		}

		/// <summary> ゲーム開始時に開始時間などのログをDBに保存 </summary>
		public void SetPlayExerciseOnStart(StageData stageData) {
			players.Clear();

			string query
			= string.Format("INSERT INTO play_exercise(exercise_id,start_time) ")
			+ string.Format("VALUES({0},{1})",stageData.Id , CurrentUnixTime);
			sqlDB.ExecuteQuery(query);

			query
			= "SELECT id FROM play_exercise "
			+ "ORDER BY id DESC LIMIT 1";
			currentPlayExerciseId = (int)sqlDB.ExecuteQuery(query).Rows[0]["id"];
		}

		/// <summary> 部首の動きをログとしてDBに保存 </summary>
		public void SetMoveLog(int playerID, string kanjiPart, string answerField, string action, float time) {
			string query
			= string.Format("INSERT INTO play_exercise_detail(play_member_id, piece, answer_field, action, time, unix_time) ")
			+ string.Format("VALUES({0},'{1}','{2}','{3}',{4},{5})", players[playerID], kanjiPart, answerField, action, time, CurrentUnixTime);
			sqlDB.ExecuteQuery(query);
		}

		/// <summary> チェックボタンが押された時のログ </summary>
		public void SetCheckPlayLog(int playID, float currentTime, string action) {
			string query
			= string.Format("INSERT INTO check_play_log (play_exercise_id,play_member_id,time,unix_time,action) ")
			+ string.Format("VALUES({0},{1},{2},{3},'{4}')", currentPlayExerciseId, players[playID], currentTime, CurrentUnixTime, action);
			sqlDB.ExecuteQuery(query);
		}

		/// <summary> 正解判定時、Pieceがどこに入っていたかをログに保存 </summary>
		public void SetJudgeLog(float currentTime, int? point, string action) {
			string query = "";
			if (point != null) {
				query
				= string.Format("INSERT INTO judge(play_exercise_id,time,unix_time,point,action) ")
				+ string.Format("VALUES({0},{1},{2},{3},'{4}')", currentPlayExerciseId, currentTime, CurrentUnixTime, point, action);
			} else {
				query
				= string.Format("INSERT INTO judge(play_exercise_id,time,unix_time,action) ")
				+ string.Format("VALUES({0},{1},{2},'{3}')", currentPlayExerciseId, currentTime, CurrentUnixTime, action);
			}
			sqlDB.ExecuteQuery(query);

			query
			= string.Format("SELECT id FROM judge ")
			+ string.Format("ORDER BY id DESC LIMIT 1");
			int currentJudgeId = (int)sqlDB.ExecuteQuery(query).Rows[0]["id"];

			foreach(Piece p in GameManager.Instance.StageManager.CurrentStage.Pieces) {
				string answerFieldName = "";

				if (p.collidedField.Value != null) {
					answerFieldName = p.collidedField.Value.name;
				}

				query
				= string.Format("INSERT INTO judge_detail(judge_id,piece_name,answer_field_name) ")
				+ string.Format("VALUES({0},'{1}','{2}')", currentJudgeId, p.PieceName, answerFieldName);
				sqlDB.ExecuteQuery(query);
			}
		}

		/// <summary> Unixtime(ミリ秒)を生成する関数 </summary>
		long CurrentUnixTime {
			get {
				string query = "select (strftime('%s', 'now')+strftime('%f', 'now') - strftime('%S', 'now'))*1000 as dtime";
				DataTable dTable = sqlDB.ExecuteQuery(query);
				return long.Parse(dTable.Rows[0]["dtime"].ToString());
			}
		}

		/// <summary> ステージリストを取得 </summary>
		public IEnumerable<StageData> GetStageList (int level) {
			List<StageData> stageDatas = new List<StageData>();

			string query = "";
			if (level == 0) { // チュートリアル
				query
				= string.Format("SELECT * FROM exercise ")
				+ string.Format("WHERE level=0 ")
				+ string.Format("ORDER BY stage ASC");
			} else if (level == 100) { // エクストラステージ
				query
				= string.Format("SELECT * FROM exercise ")
				+ string.Format("WHERE level>100 AND level<1000 ")
				+ string.Format("ORDER BY level ASC");
			} else if (level == 1000) { // グラフ
				query
				= string.Format("SELECT * FROM exercise ")
				+ string.Format("WHERE level=1000 ")
				+ string.Format("ORDER BY stage ASC");
			} else { // 簡単・難しい
				query
				= string.Format("SELECT * FROM exercise ")
				+ string.Format("WHERE level={0} ", level)
				+ string.Format("ORDER BY RANDOM()");
			}

			foreach(DataRow d in sqlDB.ExecuteQuery(query).Rows) {
				int id = (int)d["id"];
				int timeLimit = (int)d["time_limit"];
				bool HasBePlayed = GetHasPlayed(id);
				stageDatas.Add(new StageData(id, timeLimit, HasBePlayed));
			}

			return stageDatas;
		}
		public IEnumerable<string> GetPieceList(StageData stageData) {
			string query
			= string.Format("SELECT piece_master.name FROM exercise_detail ")
			+ string.Format("INNER JOIN piece_master ON exercise_detail.piece_id=piece_master.id ")
			+ string.Format("WHERE exercise_detail.exercise_id={0} ", stageData.Id)
			+ string.Format("ORDER BY exercise_detail.position ASC");

			return sqlDB.ExecuteQuery(query).Rows.ConvertAll(row => (string) row["name"]);
		}

		public abstract bool CheckFunction(string leftKanji, string rightKanji);

	}
}