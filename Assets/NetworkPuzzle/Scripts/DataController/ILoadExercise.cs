using System.Collections;
using System.Collections.Generic;
using NetworkPuzzle;

public interface ILoadExercise {
    IEnumerable<StageData> GetStageList (int level);
    IEnumerable<string> GetPieceList(StageData stageData);
    void SetHasPlayed(int exerciseID);
    bool GetHasPlayed(int exerciseID);
}
