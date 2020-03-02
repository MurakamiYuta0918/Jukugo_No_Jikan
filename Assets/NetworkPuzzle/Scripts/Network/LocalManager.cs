using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStartStageClient { void OnStartStageClient(); }
public interface IEndStageClient { void OnEndStageClient(); }

public interface IStartStageServer { void OnStartStageServer(); }
public interface IEndStageServer { void OnEndStageServer(); }

public class LocalManager : MonoBehaviour {

	List<IStartStageClient> startStageClientList = new List<IStartStageClient> ();
	List<IEndStageClient> endStageClientList = new List<IEndStageClient> ();


	public void OnStartStageClient() {
		startStageClientList.ForEach(ibs => ibs.OnStartStageClient());
	}

	public void OnEndStageClient() {
		endStageClientList.ForEach(ies => ies.OnEndStageClient());
	}

	public void AddStartStageClient(IStartStageClient iBeginStage) {
		if (!startStageClientList.Contains(iBeginStage)) {
			startStageClientList.Add(iBeginStage);
		}
	}

	public void RemoveStartStageClient(IStartStageClient iBeginStage) {
		if (startStageClientList.Contains(iBeginStage)) {
			startStageClientList.Remove(iBeginStage);
		}
	}

	public void AddEndStageClient(IEndStageClient iEndStage) {
		if (!endStageClientList.Contains(iEndStage)) {
			endStageClientList.Add(iEndStage);
		}
	}

	public void RemoveEndStageClient(IEndStageClient iEndStage) {
		if (endStageClientList.Contains(iEndStage)) {
			endStageClientList.Remove(iEndStage);
		}
	}

	List<IStartStageServer> startStageServerList = new List<IStartStageServer> ();
	List<IEndStageServer> endStageServerList = new List<IEndStageServer> ();

	public void OnStartStageServer() {
		startStageServerList.ForEach(ibs => ibs.OnStartStageServer());
	}

	public void OnEndStageServer() {
		endStageServerList.ForEach(ies => ies.OnEndStageServer());
	}

	public void AddStartStageServer(IStartStageServer iBeginStage) {
		if (!startStageServerList.Contains(iBeginStage)) {
			startStageServerList.Add(iBeginStage);
		}
	}

	public void RemoveStartStageServer(IStartStageServer iBeginStage) {
		if (startStageServerList.Contains(iBeginStage)) {
			startStageServerList.Remove(iBeginStage);
		}
	}

	public void AddEndStageServer(IEndStageServer iEndStage) {
		if (!endStageServerList.Contains(iEndStage)) {
			endStageServerList.Add(iEndStage);
		}
	}

	public void RemoveEndStageServer(IEndStageServer iEndStage) {
		if (endStageServerList.Contains(iEndStage)) {
			endStageServerList.Remove(iEndStage);
		}
	}

}
