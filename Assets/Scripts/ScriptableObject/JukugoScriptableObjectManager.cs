using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NetworkPuzzle.JukugoMode {

	public class JukugoScriptableObjectManager : NetworkPuzzle.ScriptableObjectManager, ICheckFunction, IJukugoGetPhonetic {
		List<piece_master.piece_master_data> piece_master;
		List<idioms_master.idioms_master_data> idioms_master;

		void Awake() {
			exercise = Resources.Load<exercise>("ScriptableObject/exersice").Data;
			exercise_detail = Resources.Load<exercise_detail>("ScriptableObject/exersice_detail").Data;
			piece_master = Resources.Load<piece_master>("ScriptableObject/piece_master").Data;
			idioms_master = Resources.Load<idioms_master>("ScriptableObject/idioms_master").Data;
		}

		protected override Dictionary<int, string> GetPieceMaster() {
			Dictionary<int, string> pieces = new Dictionary<int, string>();

			foreach(piece_master.piece_master_data row in piece_master) {
				pieces.Add(row.id, row.name);
			}

			return pieces;
		}

		public bool CheckFunction(string leftKanji, string rightKanji) {
			return idioms_master.Any(row => row.left_kanji == leftKanji && row.right_kanji == rightKanji);
		}

		public string GetPhonetic(char leftKanji, char rightKanji) {
			idioms_master.idioms_master_data result = idioms_master.FirstOrDefault(row => row.left_kanji == leftKanji.ToString() && row.right_kanji == rightKanji.ToString());
			return result == null ? "" : result.phonetic;
		}
	}
}

