using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkPuzzle.JukugoMode {

    public class AnswerField : NetworkPuzzle.AnswerField {

        Piece kanjiPiece;

        public string PieceName {
        	get { return kanjiPiece ? kanjiPiece.PieceName : ""; }
        }

        public override bool IsSettable(Piece otherPiece) { return kanjiPiece==null; }

        public override void RemoveParts(Piece otherPiece) {
			if (otherPiece == kanjiPiece) {
				kanjiPiece = null;
			}
        }

        public override bool SetParts(Piece otherPiece) {
			if (!IsSettable(otherPiece)) {
				return false;
			}

			kanjiPiece = otherPiece;
			return true;
        }
    }
}