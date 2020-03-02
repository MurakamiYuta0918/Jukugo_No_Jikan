using System.Collections;
using System.Collections.Generic;
using NetworkPuzzle;

public interface IWritePlayLog {
	void SetPlayMemberLog(int playID);
	void SetPlayExerciseOnEndGame(int point, PlayStageState.EndConditionType endCondition);
	void SetPlayExerciseOnStart(StageData stageData);
	void SetMoveLog(int playerID, string kanjiPart, string answerField, string action, float time);
	void SetCheckPlayLog(int playID, float currentTime, string action);
	void SetJudgeLog(float currentTime, int? point, string action);
}

public class NullPlayLogWriter : IWritePlayLog {
	void IWritePlayLog.SetPlayMemberLog(int playID){}
	void IWritePlayLog.SetPlayExerciseOnEndGame(int point, PlayStageState.EndConditionType endCondition){}
	void IWritePlayLog.SetPlayExerciseOnStart(StageData stageData) {}
	void IWritePlayLog.SetMoveLog(int playerID, string kanjiPart, string answerField, string action, float time){}
	void IWritePlayLog.SetCheckPlayLog(int playID, float currentTime, string action){}
	void IWritePlayLog.SetJudgeLog(float currentTime, int? point, string action){}
}
