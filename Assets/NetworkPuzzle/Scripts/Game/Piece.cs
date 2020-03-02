using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace NetworkPuzzle {

    public abstract class Piece : NetworkBehaviour {

        public virtual Transform StartParent {
            set {
                    pieceFieldPos = value.position;
            }
        }

        #region USED ONLY SERVER

        
        /// <summary> 所属しているAnswerField(ドラッグ中はnull)</summary>
        public Vector2 ansFieldScale;
        AnswerField answerField;
        AnswerField lastField;

        public abstract string PieceName { get; }

        #endregion
        
        #region USED ONLY CLIENT 

        public static NetworkPuzzle.PlayerPointer MyPointer;

        /// <summary> 子が持つ文字画像 </summary>
        [SerializeField]
        SpriteRenderer spriteRenderer;

        protected abstract Graphic PieceGraphic { get; }
        Graphic graphic;

        [SyncVar]
        Vector2 pieceFieldPos;

        #endregion

        #region USED BOTH SERVER and CLIENT 

        public CollidedField collidedField = new CollidedField();
        
        #endregion


        public override void OnStartServer() {
            transform.position = (Vector2) pieceFieldPos;
            RpcBackStartPos();
        }

        public override void OnStartClient() {
            GameObject parentObj = GameObject.Find("KanjiField");
            transform.SetParent(parentObj.transform);
        }


        //漢字フィールドに衝突時
        void OnTriggerEnter2D(Collider2D collision) {
            var enteranswerField = collision.GetComponent<AnswerField>();

            if (enteranswerField) {
                collidedField.Value = enteranswerField;

                if(MyPointer.ClientPiece == this) {
                    MyPointer.CmdEnterPiece(PieceName);
                }
            }
        }

        //漢字フィールドから出た時
        void OnTriggerExit2D(Collider2D collision) {
            AnswerField exitAnswerfield = collision.GetComponent<AnswerField>();
            if(exitAnswerfield != null && exitAnswerfield == collidedField.Value){
                collidedField.Value = null;
            }
        }

		public void StartMove() {
            collidedField.IsLocalEnter = true;
            if (!MyPointer.ClientPiece) {
                transform.SetAsLastSibling();
                StartCoroutine(MyPointer.MovePiece(this));
            }
        }

        public void EndMove() {
            collidedField.IsLocalEnter = false;
            if (MyPointer.ClientPiece == this) {
                MyPointer.ClientPiece = null;
            }
        }

        [Server]
        public void BeginDrag() {
            if (!answerField) {
                return;
            }

            answerField.RemoveParts(this);
            lastField = answerField;
            answerField = null;
        }

        // ステージ終了時
        [Server]
        public void StopDrag() {
            answerField = lastField;
            collidedField.Value = lastField;
            Debug.Log(answerField);
            TryEnter();
        }

        //ドラッグ終了時
        [Server]
        public void EndDrag() {
            answerField = collidedField.Value;
            TryEnter();
        }
        
        [Server]
        void TryEnter() {
            if (answerField!=null && answerField.IsSettable(this)) {
                answerField.SetParts(this);
                //transform.position = (Vector2) Camera.main.WorldToScreenPoint(answerField.transform.position);
                transform.position = (Vector2) answerField.transform.position;
                RpcDropOnAnswer(answerField.transform.lossyScale);
            } else {
                transform.position =  pieceFieldPos;
                RpcBackStartPos();
            }
            lastField = null;
        }

        [ClientRpc]
        public void RpcBeginMove(Color color) {
            //spriteRenderer.color = color;
            PieceGraphic.color = color;
            transform.localScale = Vector2.one;
            BeginMove();
        }

        public abstract void BeginMove();

        [ClientRpc]
        public void RpcDropOnAnswer(Vector2 checkPairScale) {
            transform.localScale = checkPairScale;
        }

        [ClientRpc]
        protected void RpcBackStartPos() {
            BackStartPos();
        }

        protected virtual void BackStartPos() {
            PieceGraphic.color = Color.black;
        }

        [Server]
        public void Move(Vector2 newPos) {
            transform.position = newPos;
        }

        private void OnDestroy() {
            collidedField.Value = null;
        }

        public class CollidedField {
            bool isLocalEnter=false;
            public bool IsLocalEnter {
                set {
                    isLocalEnter = value;
                    if (!value && target) {
                        target.FrameColor = Color.clear;
                    }
                }
            }
            AnswerField target;
            public AnswerField Value {
                get { return target; }
                set {
                    if (target && isLocalEnter) { target.FrameColor = Color.clear; }
                    target = value;
                    if (target && isLocalEnter) { target.FrameColor = MyPointer.playerColor; }
                }
            }

        }

    }
}
