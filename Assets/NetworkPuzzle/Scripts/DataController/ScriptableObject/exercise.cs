using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkPuzzle {

	[CreateAssetMenu(fileName = "exercise", menuName = "ScriptableObject/exercise")]
	public class exercise : ScriptableObject {

		public List<exercise_data> Data;

		[System.Serializable]
		public class exercise_data {
			public int id;
			public int level;
			public int stage;
			public int time_limit;
			public int answer_size;

			public exercise_data(int id, int level, int stage, int time_limit, int answer_size) {
				this.id = id;
				this.level = level;
				this.stage = stage;
				this.time_limit = time_limit;
				this.answer_size = answer_size;
			}
		}
	}
}


