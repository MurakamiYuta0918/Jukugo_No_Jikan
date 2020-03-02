using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSData : MonoBehaviour {
	[SerializeField] Text fpsText; 
	int frameCount;
	float prevTime;

	private void Start() {
		frameCount = 0;
		prevTime = 0.0f;
	}

	void Update() {
		frameCount++;
		float time = Time.realtimeSinceStartup - prevTime;

		if (time >= 0.5f) {
			fpsText.text = (frameCount/time).ToString();

			frameCount=0;
			prevTime = Time.realtimeSinceStartup;
		}
	}
}
