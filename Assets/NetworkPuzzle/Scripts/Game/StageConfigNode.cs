using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace  NetworkPuzzle {
	
	public class StageConfigNode : MonoBehaviour {

		[SerializeField]
		Text stageIdText, timeText;

		[SerializeField]
		Toggle toggle;

		public bool IsOn {
			get { return stageData.Use; }
			set { stageData.Use = value; }
		}

		StageData stageData = new StageData(-1,-1, true);
		public StageData StageData {
			set {
				stageData = value;
				stageIdText.text = "stage "+stageData.Id;
				timeText.text = ((int) stageData.TimeLimitSeconds/60).ToString();

				toggle.isOn = stageData.Use;
			}
		}

		public void SetLimitTimeMinute (int limitMinute) {
			stageData.TimeLimitSeconds = limitMinute * 60;
			timeText.text = limitMinute.ToString();
		}

	}

}
