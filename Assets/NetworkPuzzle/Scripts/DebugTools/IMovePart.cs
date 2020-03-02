using UnityEngine;
using NetworkPuzzle;

public interface IMovePart {
	bool IsMove();
	bool IsDragging();
	Piece ClientPiece { get; set; }
	Vector2 NextPos { get; }
}

abstract class InputWay : IMovePart {
	float threshold;
	protected Camera camera;
	protected Vector2 lastPos;
	protected Piece clientPiece;
	protected abstract Vector2 currentPos { get; }
	public Piece ClientPiece {
		get { return clientPiece; }
		set { 
			if (value!=null && clientPiece!=null) return;
			clientPiece = value;
		}
	}

	Vector2 IMovePart.NextPos { get { return lastPos = currentPos; } }

	public InputWay (float threshold, Camera camera) {
		this.threshold = threshold;
		this.camera = camera;
	}

	public virtual bool IsMove() {
		return Vector2.Distance(lastPos, currentPos)>threshold;
	}

	bool IMovePart.IsDragging() { return clientPiece != null; }
}

class MouseWay : InputWay {
    protected override Vector2 currentPos {
		get { return camera.ScreenToWorldPoint(Input.mousePosition); }
	}
    public MouseWay(float threshold, Camera camera) : base(threshold, camera) { }
}

class TouchWay : InputWay {
    protected override Vector2 currentPos {
		get {
			if (Input.touchCount<1) {
				return lastPos;
			} else {
				return camera.ScreenToWorldPoint(Input.GetTouch(0).position);
			}
		}
	}

    public TouchWay(float threshold, Camera camera) : base(threshold, camera) { }
}

public class NullWay : IMovePart {
	public readonly static NullWay Singleton = new NullWay();
	NullWay() {}
    Piece IMovePart.ClientPiece {
		get { return null; }
		set {}
	}
    Vector2 IMovePart.NextPos { get { return Vector2.zero; } }
    bool IMovePart.IsDragging() { return false; }
    bool IMovePart.IsMove() { return false; }
}