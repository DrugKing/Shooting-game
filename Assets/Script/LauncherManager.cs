using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LauncherManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public GameObject GameLauncherPanel;
    public GameObject LobbyPanel;
    public GameObject ErrorPanel;
    public GameObject CreateRoomPanel;
    public GameObject NoRoom;
    public GameObject NoName;
    public GameObject NoRoomName;
    public GameObject NotEnough;
    public GameObject WaitingRoomPanel;
    public GameObject RoomListPanel;
    public GameObject roomListPrefab;
    public GameObject roomListParent;
    public GameObject playerListPrefab;
    public GameObject playerListContent;    

    public GameObject startGameButton;

    public Text roomInfoText;
    public static string randomRoomName;
    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGameobjects;
    private Dictionary<int, GameObject> playerListGameobjects;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(GameLauncherPanel.name);
        ErrorPanel.SetActive(false);

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameobjects = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    { 
        
    }

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    #region Public Methods
    public void ConnectToMainServer()
    {
        if (PhotonNetwork.NickName == "")
        {
            ErrorPanel.SetActive(true);
            NoName.SetActive(true);
            NoRoom.SetActive(false);
            NoRoomName.SetActive(false);
            NotEnough.SetActive(false);
            return;
        }

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateAndJoinRoom()
    {
        Debug.Log(randomRoomName);
        if (randomRoomName == null)
        {
            ErrorPanel.SetActive(true);
            NoName.SetActive(false);
            NoRoom.SetActive(false);
            NoRoomName.SetActive(true);
            NotEnough.SetActive(false);
            return;
        }
        else
        {
            string RoomName = "Room " + randomRoomName;

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = 4;

            PhotonNetwork.CreateRoom(RoomName, roomOptions);
        }        
    }
    #endregion

    #region UI Callbacks
    public void Close()
    {
        ErrorPanel.SetActive(false);
    }
    public void OnCancelButtonClicked()
    {
        ActivatePanel(LobbyPanel.name);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(RoomListPanel.name);
    }

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanel(LobbyPanel.name);
    }

    public void OnStartButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.LoadLevel("Battlefield");
            }
            else
            {
                ErrorPanel.SetActive(true);
                NoName.SetActive(false);
                NoRoom.SetActive(false);
                NoRoomName.SetActive(false);
                NotEnough.SetActive(true);
                return;
            }
        }
    }

    #endregion

    #region Panel Activation
    public void ActivatePanel(string panelToBeActivated)
    {
        GameLauncherPanel.SetActive(panelToBeActivated.Equals(GameLauncherPanel.name));
        LobbyPanel.SetActive(panelToBeActivated.Equals(LobbyPanel.name));
        CreateRoomPanel.SetActive(panelToBeActivated.Equals(CreateRoomPanel.name));
        WaitingRoomPanel.SetActive(panelToBeActivated.Equals(WaitingRoomPanel.name));
        RoomListPanel.SetActive(panelToBeActivated.Equals(RoomListPanel.name));
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + "'s Shit is Coming!!");
        ActivatePanel(LobbyPanel.name);
    }

    public override void OnConnected()
    {
        Debug.Log("Internet Getto");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);
        ErrorPanel.SetActive(true);
        NoName.SetActive(false);
        NoRoom.SetActive(true);
        NoRoomName.SetActive(false);
        NotEnough.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(WaitingRoomPanel.name);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }        
        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
                            "Players/Max.players: " +
                            PhotonNetwork.CurrentRoom.PlayerCount + "/" +
                            PhotonNetwork.CurrentRoom.MaxPlayers;
        if (playerListGameobjects == null)
        {
            playerListGameobjects = new Dictionary<int, GameObject>();
        }

        //Instantiating player list gameobjects
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameobject = Instantiate(playerListPrefab);
            playerListGameobject.transform.SetParent(playerListContent.transform);
            playerListGameobject.transform.localScale = Vector3.one;

            playerListGameobject.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(true);

            }
            else
            {
                playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
            }
            playerListGameobjects.Add(player.ActorNumber, playerListGameobject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
        //update room info text
        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
                       "Players/Max.players: " +
                       PhotonNetwork.CurrentRoom.PlayerCount + "/" +
                       PhotonNetwork.CurrentRoom.MaxPlayers;

        GameObject playerListGameobject = Instantiate(playerListPrefab);
        playerListGameobject.transform.SetParent(playerListContent.transform);
        playerListGameobject.transform.localScale = Vector3.one;

        playerListGameobject.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(true);

        }
        else
        {
            playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(false);

        }

        playerListGameobjects.Add(newPlayer.ActorNumber, playerListGameobject);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //update room info text
        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
                           "Players/Max.players: " +
                           PhotonNetwork.CurrentRoom.PlayerCount + "/" +
                           PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameobjects[otherPlayer.ActorNumber].gameObject);
        playerListGameobjects.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                //update cachedRoom list
                if (cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList[room.Name] = room;
                }
                //add the new room to the cached room list
                else
                {
                    cachedRoomList.Add(room.Name, room);
                }
            }
        }

        foreach (RoomInfo room in cachedRoomList.Values)
        {
            GameObject roomListEntryGameobject = Instantiate(roomListPrefab);
            roomListEntryGameobject.transform.SetParent(roomListParent.transform);
            roomListEntryGameobject.transform.localScale = Vector3.one;

            roomListEntryGameobject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomListEntryGameobject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount + " / " + room.MaxPlayers;
            roomListEntryGameobject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(room.Name));

            roomListGameobjects.Add(room.Name, roomListEntryGameobject);
        }
    }
    public override void OnLeftLobby()
    {
        ClearRoomListView();
        cachedRoomList.Clear();
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(LobbyPanel.name);

        foreach (GameObject playerListGameobject in playerListGameobjects.Values)
        {
            Destroy(playerListGameobject);
        }

        playerListGameobjects.Clear();
        playerListGameobjects = null;
    }
    #endregion

    #region Private Methods
    void OnJoinRoomButtonClicked(string _roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(_roomName);
    }

    void ClearRoomListView()
    {
        foreach (var roomListGameobject in roomListGameobjects.Values)
        {
            Destroy(roomListGameobject);
        }

        roomListGameobjects.Clear();
    }



    #endregion
}
