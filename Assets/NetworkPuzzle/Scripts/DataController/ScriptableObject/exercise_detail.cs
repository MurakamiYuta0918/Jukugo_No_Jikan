using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkPuzzle {

	[CreateAssetMenu(fileName = "exercise_detail", menuName = "ScriptableObject/exercise_detail")]
	public class exercise_detail : ScriptableObject {
		public List<exercise_detail_data> Data;

		[System.Serializable]
		public class exercise_detail_data {
			public int id;
			public int exercise_id;
			public int position;
			public int piece_id;

			public exercise_detail_data(int id, int exercise_id, int position, int piece_id) {
				this.id = id;
				this.exercise_id = exercise_id;
				this.position = position;
				this.piece_id = piece_id;
			}
		}
	}
}
