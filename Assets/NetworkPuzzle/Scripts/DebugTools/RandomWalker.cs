using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using NetworkPuzzle;

/// <summary> 物体をランダムに移動する </summary>
public class RandomWalker : NetworkBehaviour, IEndStageClient {
	LocalManager localManager { get { return FindObjectOfType<LocalManager> ();}}
	Timer timer { get { return FindObjectOfType<Timer> (); }}

	/// <summary> Sliderの変更を適用するためのセッター </summary>
	public float MoveSpeed { set { moveSpeed = value; } }
    /// <summary> 全プレイヤー共通のAutoMove速度 </summary>
    [SyncVar]
	float moveSpeed=2.0f;

	AutoMove autoMove;

    // Use this for initialization
    void Start () {
		if (!isServer) {
			foreach (Transform child in transform) {
				child.gameObject.SetActive(false);
			}
		}

		autoMove = new AutoMove(this, localManager);
	}
	

	[Server]
	public void StartMove() {
		RpcMovePointer();
	}

	[ClientRpc]
	public void RpcMovePointer() {
		autoMove.SwitchAutoMove();
	}

	public void SwitchIsLoop(Image image) {
		GameManager.Instance.StageManager.IsLoop = !GameManager.Instance.StageManager.IsLoop;

		if (GameManager.Instance.StageManager.IsLoop) {
			image.color = Color.red;
		} else {
			image.color = Color.white;
		}
	}

	public void SwitchPlaying() {
		timer.SwitchPlayStop();
	}

	public void OnEndStageClient() {
		Invoke("GoNextStage", 4.0f);
	}

	public void ForceFinishStage() {
		Timer timer = FindObjectOfType<Timer> ();
		PlayStageState pss = timer.CurrentState as PlayStageState;
		if (pss != null) {
			pss.Stop();
		}
	}

	void GoNextStage () {
		GameManager.Instance.GoNextStage();
	}

	IEndStageClient isc;
	public void SwitchAutoStageTrans() {
		if (isc==null) {
			isc = this;
			localManager.AddEndStageClient(isc);
		} else {
			localManager.RemoveEndStageClient(isc);
			isc = null;
		}
	}

	class AutoMove : IMovePart, IStartStageClient, IEndStageClient {
		GameObject moveObj;
		float moveSpeed { get { return randomWalker.moveSpeed; } }
		RandomWalker randomWalker;
		LocalManager localManager;
		
		public AutoMove(RandomWalker randomWalker, LocalManager lm) {
			this.randomWalker = randomWalker;
			this.localManager = lm;

			randomWalker.StartCoroutine(MoveAround());
		}

		IEnumerator MoveAround() {
			moveObj = new GameObject();
			Vector2 cameraCenter = Camera.main.transform.position;

			moveObj.transform.position = (Vector3) cameraCenter + new Vector3(500, 0, -1);
			while (true) {
				moveObj.transform.RotateAround(cameraCenter, Vector3.back, moveSpeed);
				yield return null;
			}
		}

		public void SwitchAutoMove() {
			if (!IsAutoMove) {
				StartAutoMove();
				localManager.AddStartStageClient(this);
				localManager.AddEndStageClient(this);
			} else {
				StopAutoMove();
				localManager.RemoveStartStageClient(this);
				localManager.RemoveEndStageClient(this);
			}
		}

		void StartAutoMove() {
			if (IsAutoMove) return;
			IsAutoMove = true;

			Piece.MyPointer.moveWay = this;
			Piece part = SelectParts();
			Piece.MyPointer.StartCoroutine(Piece.MyPointer.MovePiece(part));
		}

		void StopAutoMove() {
			IsAutoMove = false;
			Piece.MyPointer.ClientPiece = null;
			if (Piece.MyPointer.moveWay != NullWay.Singleton) {
				Piece.MyPointer.SetGetPosWay();
			}
		}

		Piece SelectParts() {
	        List<PlayerPointer> pps = new List<PlayerPointer>(Component.FindObjectsOfType<PlayerPointer>());
	        int index = pps.IndexOf(Piece.MyPointer);

	        Piece[] parts = Component.FindObjectsOfType<Piece>();
	        return parts[index % parts.Length];
	    }

		bool IsAutoMove;

	    void IStartStageClient.OnStartStageClient() {
			StartAutoMove();
	    }

	    void IEndStageClient.OnEndStageClient() {
			StopAutoMove();
	    }

		#region MovePart
	    Piece IMovePart.ClientPiece { get; set; }
	    Vector2 IMovePart.NextPos { get { return moveObj.transform.position; } }
	    bool IMovePart.IsMove() { return true; }
	    bool IMovePart.IsDragging() { return ((IMovePart) this).ClientPiece; }
		#endregion
	}
}
