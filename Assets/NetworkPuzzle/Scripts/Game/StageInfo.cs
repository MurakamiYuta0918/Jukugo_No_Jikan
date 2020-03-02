using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfo : MonoBehaviour {
	[SerializeField]
	Text stageNoText, gradeText;
	
	public int stageNo { set { stageNoText.text = "ステージ"+value; }}
	public int grade { set { gradeText.text = value.ToString(); }}

}
