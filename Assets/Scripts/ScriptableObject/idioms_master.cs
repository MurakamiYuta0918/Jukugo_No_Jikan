using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkPuzzle.JukugoMode {

	[CreateAssetMenu(fileName = "idioms_master", menuName = "ScriptableObject/idioms_master")]
	public class idioms_master : ScriptableObject {

		public List<idioms_master.idioms_master_data> Data;

		[System.Serializable]
		public class idioms_master_data {
			public int id;
			public int? grade;
			public string left_kanji;
			public string right_kanji;
			public string phonetic;
			public string mean;

			public idioms_master_data(int id, int? grade, string left_kanji, string right_kanji, string phonetic, string mean) {
				this.id = id;
				this.grade = grade;
				this.left_kanji = left_kanji;
				this.right_kanji = right_kanji;
				this.phonetic = phonetic;
				this.mean = mean;
			}
		}
	}
}
