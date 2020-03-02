using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NetworkPuzzle {
	public abstract class ScriptableObjectGenerator : MonoBehaviour {
		protected SqliteDatabase sqlDB;

		protected void GenerateExcesice() {
			exercise instance = ScriptableObject.CreateInstance<exercise>();
			instance.Data = new List<exercise.exercise_data>();

			foreach(DataRow row in sqlDB.ExecuteQuery("SELECT * FROM exercise").Rows) {
				int id = (int)row["id"];
				int level = (int)row["level"];
				int stage = (int)row["stage"];
				int time_limit = (int)row["time_limit"];
				int answer_size = (int)row["answer_size"];

				exercise.exercise_data newData = new exercise.exercise_data(id, level, stage, time_limit, answer_size);
				instance.Data.Add(newData);
			}
			#if UNITY_EDITOR
				AssetDatabase.CreateAsset(instance, "Assets/Resources/ScriptableObject/exersice.asset");
			#endif
		}

		protected void GenerateExcesiceDetail() {
			exercise_detail instance = ScriptableObject.CreateInstance<exercise_detail>();
			instance.Data = new List<exercise_detail.exercise_detail_data>();

			foreach(DataRow row in sqlDB.ExecuteQuery("SELECT * FROM exercise_detail").Rows) {
				int id = (int)row["id"];
				int exercise_id = (int)row["exercise_id"];
				int position = (int)row["position"];
				int piece_id = (int)row["piece_id"];

				exercise_detail.exercise_detail_data newData = new exercise_detail.exercise_detail_data(id, exercise_id, position, piece_id);
				instance.Data.Add(newData);
			}
			#if UNITY_EDITOR
				AssetDatabase.CreateAsset(instance, "Assets/Resources/ScriptableObject/exersice_detail.asset");
			#endif
		}

		protected abstract void GeneratePieceMaster();
	}
}
