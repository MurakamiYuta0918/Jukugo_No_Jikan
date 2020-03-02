using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;
using NetworkPuzzle;

namespace Prototype.NetworkLobby
{
    public class LobbyManager : NetworkLobbyManager 
    {
        static short MsgKicked = MsgType.Highest + 1;
		private Discovery netdisc;
        static public LobbyManager s_Singleton;
		public GameObject RoomPrefab;
		int roomcount;
        [Header("Unity UI Lobby")]
        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        [Space]
        [Header("UI Reference")]
        public LobbyTopPanel topPanel;

        public RectTransform mainMenuPanel;
        public RectTransform lobbyPanel;

        public LobbyInfoPanel infoPanel;
        public LobbyCountdownPanel countdownPanel;
        public GameObject addPlayerButton;
		public GameObject titleback;
        protected RectTransform currentPanel;
		public GameObject RoomNameObj;
		public GameObject CreateRoomNameObj;
		[SerializeField]
		private GameObject RoomObj;
		public GameObject roomListObj;
		[SerializeField]
		private GameObject titleButton;
		[SerializeField]
		private PlayerNumber playerNumber;	
		[SerializeField]
		private GameObject singlePlayPanel;
		[SerializeField]
		private GameObject selectPlayPanel;
        [SerializeField]
		private GameObject selectStagePanel;
		[SerializeField]
		GameObject hidePanel;
        public Button backButton;
		private LobbyRoomList roomList;
        public LobbyRoomList RoomList { get { return roomList; }}
        public Text statusInfo;
        public Text hostInfo;

        //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
        //of players, so that even client know how many player there is.
        [HideInInspector]
        public int _playerNumber = 0;

        //used to disconnect a client properly when exiting the matchmaker
        [HideInInspector]
        public bool _isMatchmaking = false;

        protected bool _disconnectServer = false;
        
        protected ulong _currentMatchID;

        protected LobbyHook _lobbyHooks;

        void Start()
        {
			//SelectNetworkType ();
            netdisc = GetComponent<Discovery> ();
			netdisc.showGUI = false;

			roomList = roomListObj.GetComponent<LobbyRoomList> ();
			/* 画像を一番前に持ってくる関数 */
			// titleback.transform.SetAsLastSibling();
			countdownPanel.transform.SetAsLastSibling();

            s_Singleton = this;
            _lobbyHooks = GetComponent<Prototype.NetworkLobby.LobbyHook>();
            currentPanel = mainMenuPanel;

            backDelegate = BackToPlayerNumber;
            GetComponent<Canvas>().enabled = true;

			NetworkStart ();
			DontDestroyManager.DontDestroyOnLoad(gameObject);

            SetServerInfo("Offline", "None");


			if(KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Single || KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Tutorial) { //ソロプレイモードまたはチュートリアルモード
                hidePanel.SetActive(true);
				SetRoomName();
				selectStagePanel.SetActive(true);
				selectPlayPanel.GetComponent<PlayerNumber>().MultiButton();

				selectStagePanel.GetComponent<StageSelect>().RoomNameEnter();
				RoomNameEnter();
				Invoke("DisactiveHidePanel", 0.5f);
            } else if(KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Host) {  //ホストモード
                hidePanel.SetActive(false);
                SetRoomName();
				selectStagePanel.SetActive(true);
				selectPlayPanel.GetComponent<PlayerNumber>().MultiButton();
			} else if(KanjiTime.TitleManager.mode == KanjiTime.TitleManager.Mode.Client) { //クライアントモード
                hidePanel.SetActive(false);
				mainMenuPanel.GetComponent<LobbyMainMenu>().OnClickFindRoom();
				OnStartClient();
				roomList.RemoveRoomButton();
                backButton.gameObject.SetActive(false);
			}

        }

		void DisactiveHidePanel() {
			hidePanel.SetActive(false);
		}

        public void SelectPlayScene(string playSceneName) {
            playScene = playSceneName;
        }

		public void NetworkStart(){
			netdisc.showGUI = false;
			netdisc.Initialize ();
		}
		/// <summary>
		/// ボタン操作用
		/// 部屋の名前を入力するInputFieldとボタンをアクティブにする
		/// </summary>
		public void SetRoomName(){
			//RoomNameObj.SetActive (true);
			backDelegate = BackToLobbyScene;
		}
		public void BackToLobbyScene(){
			RoomNameObj.SetActive (false);
			backDelegate = BackToPlayerNumber;
		}
		/// <summary>
		/// 入力した部屋の名前を登録し、通信方式別に処理を行う関数
		/// </summary>
		public void RoomNameEnter(){
			netdisc.RoomName = RoomNameObj.transform.Find ("RoomName").transform.Find("Text").GetComponent<Text> ().text;
			RoomObj.GetComponent<Text> ().text = netdisc.RoomName;
			netdisc.Initialize ();
			Debug.Log (netdisc.broadcastData);
			if (CheckRommName ()) {
				return;
			} else {
				RoomNameObj.SetActive (false);
				StartHost ();
                backDelegate = StopHostClbk;
			}

		}
		public bool CheckRommName(){
			string name = RoomNameObj.transform.Find ("RoomName").transform.Find("Text").GetComponent<Text> ().text;
			return name.Contains(",") || name.Contains("-");
		}

        public void CreateRoomBack(){
            selectStagePanel.SetActive(false);
        }

		/// <summary>
		/// 戻るボタン
		/// </summary>
		public void BackToPlayerNumber(){
			NetworkServer.Reset ();
			singlePlayPanel.SetActive (false);
			//selectPlayPanel.SetActive (true);
			SceneManager.LoadScene ("Menu Screen");
		}
			
        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            if (SceneManager.GetSceneAt(0).name == lobbyScene)
            {
                if (topPanel.isInGame)
                {
                    ChangeTo(lobbyPanel);
                    if (_isMatchmaking)
                    {
                        if (conn.playerControllers[0].unetView.isServer)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                    else
                    {
                        if (conn.playerControllers[0].unetView.isClient)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                }
                else
                {
                    ChangeTo(mainMenuPanel);
                }

                topPanel.ToggleVisibility(true);
                topPanel.isInGame = false;
            }
            else
            {
                ChangeTo(null);

                Destroy(GameObject.Find("MainMenuUI(Clone)"));

                //backDelegate = StopGameClbk;
                topPanel.isInGame = true;

                topPanel.ToggleVisibility(false);
            }
        }

		public int MaxPlayers{
			get { return maxPlayers; }
		}

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel == mainMenuPanel)
            {
                backDelegate = BackToPlayerNumber;
                SetServerInfo("Offline", "None");
                _isMatchmaking = false;
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            statusInfo.text = status;
            hostInfo.text = host;
        }


        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton() {
            backDelegate ();
        }
		public void AddStopDelegate(){
			backDelegate += StopGameClbk;
		}
        // ----------------- Server management

        public void AddLocalPlayer()
        {
            TryToAddPlayer();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            player.RemovePlayer();
        }

        public void SimpleBackClbk()
        {
            ChangeTo(mainMenuPanel);
        }
                 
        public void StopNetWork() {
			netdisc.StopBroadcast ();
			topPanel.isInGame = false;
			netdisc.Initialize ();
        }
		public void StopGameClbk (){
			NetworkServer.Reset ();
			SceneManager.LoadScene ("title");
		}
        public void StopHostClbk()
        {
			bool InGameFlag = topPanel.isInGame;
            StopNetWork();

            if (_isMatchmaking)
            {
				matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
				_disconnectServer = true;
            }
            else
            {
                StopHost();
            }

			//LobbyMainMenu.Flag = true;
			if (InGameFlag == false) {
				ChangeTo (mainMenuPanel);
                //selectPlayPanel.SetActive(true);
			}
        }

        public void StopClientClbk()
        {
			bool InGameFlag = topPanel.isInGame;
            Debug.Log("STOPCLIENTCLBK CALLED");
            StopNetWork();

            StopClient();

            if (_isMatchmaking)
            {
                StopMatchMaker();
            }
			roomList.RemoveRoomButton ();
			//LobbyMainMenu.Flag = true;
			if (InGameFlag == false) {
				singlePlayPanel.SetActive (false);
				ChangeTo (mainMenuPanel);
                //selectPlayPanel.SetActive(true);
			}
        }

        public void Re_SearchRoom() {
			bool InGameFlag = topPanel.isInGame;
            StopNetWork();

            StopClient();

            if (_isMatchmaking)
            {
                StopMatchMaker();
            }
			roomList.RemoveRoomButton ();
            mainMenuPanel.GetComponent<LobbyMainMenu> ().OnClickFindRoom();
            OnStartClient();
        }

        public void StopServerClbk()
        {
			bool InGameFlag = topPanel.isInGame;
            StopServer();
			if (InGameFlag == false) {
				singlePlayPanel.SetActive (false);
				//selectPlayPanel.SetActive (true);
				ChangeTo (mainMenuPanel);
			}
        }

        class KickMsg : MessageBase { }
        public void KickPlayer(NetworkConnection conn)
        {
            conn.Send(MsgKicked, new KickMsg());
        }




        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            infoPanel.Display("Kicked by Server", "Close", null);
            netMsg.conn.Disconnect();
        }

        //===================

        public override void OnStartHost()
        {
			if (playerNumber.PlayMode) {
				singlePlayPanel.SetActive (true);
			}
			else {
				netdisc.StartAsServer ();
			}
            base.OnStartHost();

            ChangeTo(lobbyPanel);
            backDelegate = StopHostClbk;
            SetServerInfo("Hosting", networkAddress);
        }

		public void OnStartClient(){
			netdisc.OnJoinRoom ();
            backDelegate = StopClientClbk;
		}

		public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			base.OnMatchCreate(success, extendedInfo, matchInfo);
            _currentMatchID = (System.UInt64)matchInfo.networkId;
		}

		public override void OnDestroyMatch(bool success, string extendedInfo)
		{
			base.OnDestroyMatch(success, extendedInfo);
			if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        //allow to handle the (+) button to add/remove player
        public void OnPlayersNumberModified(int count)
        {
            _playerNumber += count;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
        }

        // ----------------- Server callbacks ------------------

        //we want to disable the button JOIN if we don't have enough player
        //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
            newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);


            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }

            return obj;
        }

        public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers >= minPlayers);
                }
            }

        }
		/// <summary>Lobbyで決めた色設定はここで引き継げる</summary>
        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayerObj, GameObject gamePlayer)
        {
			if (_lobbyHooks)
                _lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayerObj, gamePlayer);

            PlayerPointer pPointer = gamePlayer.GetComponent<PlayerPointer>();
            LobbyPlayer lPlayer = lobbyPlayerObj.GetComponent<LobbyPlayer>();
            
            pPointer.playerColor = lPlayer.playerColor;
            return true;
        }

        // --- Countdown management

        public override void OnLobbyServerPlayersReady()
        {
			bool allready = true;
			for(int i = 0; i < lobbySlots.Length; ++i)
			{
				if(lobbySlots[i] != null)
					allready &= lobbySlots[i].readyToBegin;
			}

			if (allready)
				netdisc.StopBroadcast ();
				StartCoroutine(ServerCountdownCoroutine());
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
                            (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
                }
            }

            ServerChangeScene(playScene);
            backDelegate = StopClient;
			backDelegate += StopGameClbk;
        }
		public void StartSearch(){
			netdisc.StartAsClient ();
		}

        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            infoPanel.gameObject.SetActive(false);

            conn.RegisterHandler(MsgKicked, KickedMessageHandler);

            if (!NetworkServer.active)
            {//only to do on pure client (not self hosting client)
                ChangeTo(lobbyPanel);
                backDelegate = StopClientClbk;
                SetServerInfo("Client", networkAddress);
            }
        }


        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            //ChangeTo(mainMenuPanel);
			SceneManager.LoadScene ("title");
			// titleButton.SetActive (false);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(mainMenuPanel);
            infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }

        [ContextMenu("AddPartsPrefabs")]
        public void AddPartsPrefabs() {
            spawnPrefabs.Clear();
            Object[] prefabs =  Resources.LoadAll("KanjiPrefab",typeof(GameObject));

            foreach (GameObject go in prefabs) {
                spawnPrefabs.Add(go);
            }
        }

        public void StartSinglePlay() {
            SceneManager.LoadScene(playScene);
            Destroy(gameObject);
        }
    }
}
