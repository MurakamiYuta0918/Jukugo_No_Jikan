using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

namespace NetworkPuzzle.JukugoMode {

	[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
	public class KanjiPiece : Piece {

		Vector2 partFieldScale;
        [SerializeField]
        TextMeshProUGUI kanjiText;

		public override string PieceName { get { return pieceName; }}
		
		[SyncVar(hook="SetPieceName")]
		string pieceName;
		void SetPieceName(string value) {
			pieceName = value;
			kanjiText.text = value;
		}

		protected override Graphic PieceGraphic {
			get { return kanjiText; }
		}

		public override void OnStartClient() {
			base.OnStartClient();
			// TODO: KanjiPartsと共通の処理。Pieceでやったほうが。。。？
			partFieldScale = transform.localScale;
		}

		public override void BeginMove() {
			transform.localScale = partFieldScale;
		}

		public StageManager.KanjiData GetKanjiData() { return new KanjiData(this); }

		class KanjiData : StageManager.KanjiData {
			readonly KanjiPiece kanjiPiece;
			public KanjiData (KanjiPiece piece) { this.kanjiPiece = piece; }

			public void SetKanji (string kanji) {
				kanjiPiece.pieceName = kanji;
			}
		}

	}
}