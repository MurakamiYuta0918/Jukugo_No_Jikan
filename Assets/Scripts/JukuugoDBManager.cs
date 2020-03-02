using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkPuzzle.JukugoMode {

	public class JukuugoDBManager : NetworkPuzzle.DBManager, ICheckFunction, IJukugoGetPhonetic {

		void Awake() {
			sqlDB = new SqliteDatabase("Jukugo.db");
		}

		public override bool CheckFunction(string leftKanji, string rightKanji) {
			string query
			= string.Format("SELECT id FROM idioms_master ")
			+ string.Format("WHERE left_kanji='{0}' ", leftKanji)
			+ string.Format("AND right_kanji='{0}'", rightKanji);
			return sqlDB.ExecuteQuery(query).Rows.Count != 0;
		}

		public string GetPhonetic(char leftKanji, char rightKanji) {
			string query
			= string.Format("SELECT * FROM idioms_master ")
			+ string.Format("WHERE left_kanji='{0}' ", leftKanji)
			+ string.Format("AND right_kanji='{0}'", rightKanji);
			List<DataRow> tables = sqlDB.ExecuteQuery(query).Rows;

			if (tables.Count != 0) {
				return (string)tables[0]["phonetic"];
			} else {
				return "";
			}
		}
	}

}