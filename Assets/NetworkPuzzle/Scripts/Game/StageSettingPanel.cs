using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkPuzzle;

public class StageSettingPanel : MonoBehaviour {

	[SerializeField]
	StageConfigNode stageConfigNodeSource;

	List<StageConfigNode> configNodeList = new List<StageConfigNode> ();

	IStageSetting iStageSetting;

	private void Start() {
		iStageSetting = GetComponentInParent<IStageSetting> ();
	}

	public void AddConfigNodeSource(StageData stageData) {
		var configNode = Instantiate(
			stageConfigNodeSource,
			stageConfigNodeSource.transform.parent);

		configNode.StageData = stageData;
		configNode.IsOn = !GameManager.Instance.SetUse(stageData);
		configNode.gameObject.SetActive(true);
		configNodeList.Add(configNode);
	}

	public void AddConfigNodeSource(IEnumerable<StageData> insertedList) {
		foreach (StageData stageData in insertedList) {
			AddConfigNodeSource(stageData);
		}
	}

	public void RemoveStageConfigNodeAll() {
		configNodeList.RemoveAll(scn => { Destroy(scn.gameObject); return true; });
	}

	public void OKButtonPushed() {
		RemoveStageConfigNodeAll();
		iStageSetting.OnOKButtonPushed();
	}

	public interface IStageSetting {
		void OnOKButtonPushed();
	}

}
