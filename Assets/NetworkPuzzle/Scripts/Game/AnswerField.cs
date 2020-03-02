using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace NetworkPuzzle {

    public abstract class AnswerField : NetworkBehaviour {
        [SerializeField]
        SpriteRenderer frameRenderer;

        [SerializeField]
        Image frameImg;

        public Color FrameColor { set { frameImg.color = value; } }

        public abstract bool IsSettable(Piece otherPiece);
        public abstract bool SetParts(Piece otherPiece);
        public abstract void RemoveParts(Piece otherPiece);
    }
}