using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkPuzzle.JukugoMode {

	[CreateAssetMenu(fileName = "piece_master", menuName = "ScriptableObject/piece_master")]
	public class piece_master : ScriptableObject {

		public List<piece_master_data> Data;

		[System.Serializable]
		public class piece_master_data {
			public int id;
			public string name;
			public int grade;

			public piece_master_data(int id, string name, int grade) {
				this.id = id;
				this.name = name;
				this.grade = grade;
			}
		}
	}
}
