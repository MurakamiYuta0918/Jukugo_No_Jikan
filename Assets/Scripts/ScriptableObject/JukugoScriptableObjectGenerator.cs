using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NetworkPuzzle.JukugoMode {
	public class JukugoScriptableObjectGenerator : NetworkPuzzle.ScriptableObjectGenerator {

		[ContextMenu("Generate")]
		void Generate() {
			sqlDB = new SqliteDatabase("Jukugo.db");

			GenerateExcesice();
			GenerateExcesiceDetail();
			GeneratePieceMaster();
			GenerateIdiomsMaster();
		}
		
		protected override void GeneratePieceMaster() {
			piece_master instance = ScriptableObject.CreateInstance<piece_master>();
			instance.Data = new List<piece_master.piece_master_data>();

			foreach(DataRow row in sqlDB.ExecuteQuery("SELECT * FROM piece_master").Rows) {
				int id = (int)row["id"];
				string name = (string)row["name"];
				int grade = (int)row["grade"];

				piece_master.piece_master_data newData = new piece_master.piece_master_data(id, name, grade);
				instance.Data.Add(newData);
			}
			#if UNITY_EDITOR
				AssetDatabase.CreateAsset(instance, "Assets/Resources/ScriptableObject/piece_master.asset");
			#endif
		}

		void GenerateIdiomsMaster() {
			idioms_master instance = ScriptableObject.CreateInstance<idioms_master>();
			instance.Data = new List<idioms_master.idioms_master_data>();

			foreach(DataRow row in sqlDB.ExecuteQuery("SELECT * FROM idioms_master").Rows) {
				int id = (int)row["id"];
				int? grade = (int?)((string)row["grade"] == "" ? null : row["grade"]);
				string left_kanji = (string)row["left_kanji"];
				string right_kanji = (string)row["right_kanji"];
				string phonetic = (string)row["phonetic"];
				string mean = (string)row["mean"];

				idioms_master.idioms_master_data newData = new idioms_master.idioms_master_data(id, grade, left_kanji, right_kanji, phonetic, mean);
				instance.Data.Add(newData);
			}
			#if UNITY_EDITOR
				AssetDatabase.CreateAsset(instance, "Assets/Resources/ScriptableObject/idioms_master.asset");
			#endif
		}
	}
}
