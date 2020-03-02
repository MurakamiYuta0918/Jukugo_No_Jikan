using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

namespace NetworkPuzzle {

    public class PlayerPointer : NetworkBehaviour, IStartStageClient, IEndStageClient {
        [SyncVar]
        public int playerId;
        Piece serverPiece;
        public bool AgreeCheck { get; private set; }

        public Piece ClientPiece {
            get { return moveWay.ClientPiece; }
            set { moveWay.ClientPiece = value; }
        }
        public IMovePart moveWay = NullWay.Singleton;
        StageManager stageManager;

        
        [SyncVar]
        public Color playerColor=Color.blue;

        public override void OnStartServer() {
            if (playerColor.Equals(Color.clear)) { // プレイヤーカラーの設定
                playerColor = Color.blue;
            }

        }

        public override void OnStartLocalPlayer() {
            CmdSetPlayerId(PlayerPrefs.GetInt("player_id"));
            Piece.MyPointer = this; // カラーを設定

            // 操作方法の決定
            LocalManager localManager = FindObjectOfType<LocalManager> ();
            localManager.AddStartStageClient(this);
            localManager.AddEndStageClient(this);
        }

        [Command]
        public void CmdEnterPiece(string pieceName) {
            Piece piece = GameManager.Instance.StageManager.FindParts(pieceName);
            GameManager.Instance.SetMoveLog(playerId, piece.PieceName, piece.collidedField.Value.name, "enter");
        }

        [Command]
        void CmdSetPlayerId(int playerId) {
            this.playerId = playerId;
        }

        [Client]
        public IEnumerator MovePiece (Piece clientPiece) {
            ClientPiece = clientPiece;
            CmdSetPiece(clientPiece.PieceName);
            yield return StartCoroutine(MovePoint());
            CmdReleasePiece();
        }

        [Client]
        IEnumerator MovePoint() {
            while (moveWay.IsDragging()) {
                if (moveWay.IsMove()) {
                    CmdMove(moveWay.NextPos);
                }
                yield return null;
            }
        } 

        [Command]
        void CmdSetPiece(string pieceName) {
            Piece piece = GameManager.Instance.StageManager.FindParts(pieceName);
            PlayerPointer[] ps = Component.FindObjectsOfType<PlayerPointer> ();
            if (ps.Count(p => p.serverPiece == piece)==0) {
                ReleasePiece();
                serverPiece = piece;
                serverPiece.BeginDrag();
                serverPiece.RpcBeginMove(playerColor);

                if(serverPiece.collidedField.Value) {
                    GameManager.Instance.SetMoveLog(playerId, serverPiece.PieceName, serverPiece.collidedField.Value.name, "catch");
                } else {
                    GameManager.Instance.SetMoveLog(playerId, serverPiece.PieceName, "null", "catch");
                }
            }
        }

        [Command]
        void CmdReleasePiece() {
            ReleasePiece();
        }

        [Server]
        void ReleasePiece() {
            if (serverPiece) {
                if(serverPiece.collidedField.Value) {
                    GameManager.Instance.SetMoveLog(playerId, serverPiece.PieceName, serverPiece.collidedField.Value.name, "release");
                } else {
                    GameManager.Instance.SetMoveLog(playerId, serverPiece.PieceName, "null", "release");
                }

                serverPiece.EndDrag();
                serverPiece = null;
            }
        }

        [Server]
        public void StopDrag() {
            if (serverPiece) {
                serverPiece.StopDrag();

                if(serverPiece.collidedField.Value) {
                    GameManager.Instance.SetMoveLog(playerId, serverPiece.PieceName, serverPiece.collidedField.Value.name, "release");
                } else {
                    GameManager.Instance.SetMoveLog(playerId, serverPiece.PieceName, "null", "release");
                }

                serverPiece = null;
            }
        }
        
        [Command]
        void CmdMove(Vector2 piecePos) {
            if (serverPiece) {
                serverPiece.Move(piecePos);
            }
        }

        public void SetGetPosWay() {
            if (Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor
                || Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.OSXPlayer) {
                moveWay = new MouseWay(10f, Camera.main);
            } else {
                moveWay = new TouchWay(10f, Camera.main);
            }
        }

        [Command]
        public void CmdPushCheck(bool isActive) {
            GameManager.Instance.SetActiveCheckPanel(isActive);
            if(isActive) {
                GameManager.Instance.SetCheckPlayLog(playerId, "suggest");
                GameManager.Instance.Judge("suggest");
            } else {
                GameManager.Instance.SetCheckPlayLog(playerId, "disagree");
                foreach (PlayerPointer p in FindObjectsOfType<PlayerPointer> ()) {
                    p.AgreeCheck = false;
                }
            }
        }

		[Command]
		public void CmdAgreeCheck() {
			AgreeCheck = true;
            GameManager.Instance.SetCheckPlayLog(playerId, "agree");
			if (FindObjectsOfType<PlayerPointer>().All(p => p.AgreeCheck)) {
				GameManager.Instance.Check();
			}
		}

        public override void OnNetworkDestroy() {
            if (isServer && serverPiece) {
                if(serverPiece.collidedField.Value) {
                    GameManager.Instance.SetMoveLog(playerId, serverPiece.PieceName, serverPiece.collidedField.Value.name, "release");
                } else {
                    GameManager.Instance.SetMoveLog(playerId, serverPiece.PieceName, "null", "release");
                }

                serverPiece.StopDrag();
                serverPiece = null;
            }
        }

        void IStartStageClient.OnStartStageClient() {
            SetGetPosWay();
        }

        void IEndStageClient.OnEndStageClient() {
            if (isServer) {
                FindObjectsOfType<PlayerPointer> ().ToList().ForEach(pp => pp.AgreeCheck=false);
            }
            moveWay = NullWay.Singleton;
        }
    }
}