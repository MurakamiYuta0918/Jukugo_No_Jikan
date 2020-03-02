using UnityEngine;
using System;
using System.Collections;

public class Database_Controller : MonoBehaviour {

    private static Database_Controller dbController;
    private SqliteDatabase dbAccess;

    /// <summary> データベースコントローラを取得する関数 </summary>
    public static Database_Controller DBController {
        get {
            if (!dbController) {
                dbController = Component.FindObjectOfType<Database_Controller> ();
            }
            return dbController;
        }
    }

    /// <summary> 書き込むデータベースのインスタンスを返す </summary>
    private SqliteDatabase DBAccess {
        get {
            if (dbAccess == null) {
                dbAccess = new SqliteDatabase ("SpanningTree.db");
            }
            return dbAccess;
        }
    }
    void Awake() {
        if (dbController!=null) {
            Destroy(gameObject);
            return;
        }
        dbController = this;
        DontDestroyOnLoad(this);
    }

    /// <summary> Unixtime(ミリ秒)を生成する関数 </summary>
    public long CurrentTime{
        get {
            string query = "select (strftime('%s', 'now')+strftime('%f', 'now') - strftime('%S', 'now'))*1000 as dtime";
            DataTable dTable = DBAccess.ExecuteQuery(query);
            //DataRow dr = dTable.Rows[0];
            return 0;//Convert.ToInt64(dr["dtime"].ToString());
        }
    }

    /// <summary> 部屋作成時 (部屋のプライマリーキーはunixtime) </summary>
    public long DBAct_RoomStart(string roomName, string stageControllerName) {
        long roomId = CurrentTime;
        string query = String.Format ("INSERT INTO room(start_time, room_name, stage_controller) VALUES('{0}','{1}', '{2}')", roomId, roomName, stageControllerName);
        DBAccess.ExecuteNonQuery(query);

        return roomId;
    }

    public void DBAct_RoomEnd(long roomId) {
        long EndTime = CurrentTime;
        string query = String.Format ("UPDATE room set end_time = {0} where start_time = {1}", EndTime, roomId);
        DBAccess.ExecuteQuery(query);
    }

    /// <summary> Exerciseへのトライ開始時 </summary>
    public long DBAct_TryStart(long roomId, int exersise, string exerciseName) {
        long tryId = CurrentTime;
        string query = String.Format("INSERT INTO try(try_id, room, exercise, exercise_name, start_time) VALUES('{0}', '{1}', '{2}', '{3}', '{4}')", tryId, roomId, exersise, exerciseName, tryId);
        DBAccess.ExecuteQuery(query);

        return tryId;
    }

    /// <summary> Exerciseへのトライ終了時（正解時） </summary>
    public void DBAct_TryEnd(long tryId) {
        long EndTime = CurrentTime;
        string query = String.Format("UPDATE try set end_time = {0} where try_id ={1}", EndTime, tryId);
        DBAccess.ExecuteQuery(query);
    }

    public long DBAct_RoomStartOnClient(long startTimeOnServer) {
        long timeOnClient = CurrentTime;
        string query = String.Format ("INSERT INTO room_on_client(time_on_client, time_on_server) VALUES('{0}','{1}')", timeOnClient, startTimeOnServer);
        DBAccess.ExecuteNonQuery(query);

        return timeOnClient;
    }

    public void DBAct_RoomEndOnClient(long startTimeOnClient, string stageControllerName) {
        long EndTime = CurrentTime;
        string query = String.Format ("UPDATE room_on_client set end_time = {0} where time_on_client = {1}", EndTime, startTimeOnClient);
        DBAccess.ExecuteQuery(query);
        query = String.Format ("UPDATE room_on_client set stage_controller = '{0}' where time_on_client = {1}", stageControllerName, startTimeOnClient);
        DBAccess.ExecuteQuery(query);
    }
    /// <summary> 部屋へ参加したとき(もしくはRoomの最初のトライが開始したとき) </summary>
    public void DBAct_Join(int playerId, long roomId) {
        string query = String.Format("INSERT INTO member(player, room) VALUES('{0}','{1}')", playerId, roomId);
        DBAccess.ExecuteQuery(query);
    }

    /// <summary> クリック時 </summary>
    public void DBAct_ClickOnServer(int player, long tryId, long timeOnClient, int status, int line) {
        string query = String.Format("INSERT INTO click_on_server(player, try, time_on_client, time_on_server, status, line) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", player, tryId, timeOnClient, CurrentTime, status, line);
        DBAccess.ExecuteQuery(query);
    }

    private long DBAct_Hint(string hintType, int player, long tryId, int? rail, int? anchorStation) {
        long cTime = CurrentTime;
        string query = 
			String.Format("INSERT INTO hint_on_client(time_on_client, player, try, hint_type, rail, anchor_station) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", cTime, player, tryId, hintType, rail, anchorStation);
        DBAccess.ExecuteNonQuery(query);
        return cTime;
    }

    public void DBAct_SortScroll(int player, long tryId, int anchor) {
        DBAct_Hint("scroll", player, tryId, null, anchor);
    }

    public void DBAct_SortClick(int player, long tryId, int rail, int anchor) {
        DBAct_Hint("click", player, tryId, rail, anchor);
    }

    public void DBAct_SetAnchor(int player, long tryId, int? anchor) {
        DBAct_Hint("anchor", player, tryId, null, anchor);
    }

    public long DBAct_Cycle(int player, long tryId, int? anchor) {
        return DBAct_Hint("cycle", player, tryId, null, anchor);
    }

    public void DBAct_Group(int player, long tryId, int? anchor) {
        DBAct_Hint("group", player, tryId, null, anchor);
    }

    public void DBAct_Check(long tryId, int playerId, string status) {
        string query = String.Format("INSERT INTO judge(time_on_server, try, action, target, status) VALUES('{0}', '{1}', 'check', {2}', '{3}');", CurrentTime, tryId, playerId, status);
        DBAccess.ExecuteNonQuery(query);
    }

    public void DBAct_Judge(long tryId, int cost, string status) {
        string query = String.Format("INSERT INTO judge(time_on_server, try, action, target, status) VALUES('{0}', '{1}', 'judge', '{2}', '{3}');", CurrentTime, tryId, cost, status); 
        DBAccess.ExecuteNonQuery(query);
    }

    public void DBAct_TimeUp(long tryId, int cost, string status) {
        string query = String.Format("INSERT INTO judge(time_on_server, try, action, target, status) VALUES('{0}', '{1}', 'time_up', '{2}', '{3}');", CurrentTime, tryId, cost, status); 
        DBAccess.ExecuteNonQuery(query);
    }

    public void DBAct_Camera(int playerId, long tryId, float centerX, float centerY, float size) {
        string query =
            String.Format("INSERT INTO camera(time_on_client, player, try, center_x, center_y, size) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", CurrentTime, playerId, tryId, centerX, centerY, size);
        DBAccess.ExecuteNonQuery(query);
    }

    public bool DBSelect_IsClear(string stageControllerName, string conditionUpDateTime) {
        string query = String.Format("SELECT * FROM room_on_client where stage_controller='{0}' AND end_time>'{1}'", stageControllerName, conditionUpDateTime);
        DataTable dt =  DBAccess.ExecuteQuery(query);
        Debug.Log(dt.Rows.Count);
        return dt.Rows.Count>0;
    }
}
