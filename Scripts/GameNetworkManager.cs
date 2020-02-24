using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class GameNetworkManager : MonoBehaviourPunCallbacks
{
    #region GameVersion
    string gameVersion = "1";
    #endregion

    #region 序列化变量
    [Tooltip("最大房间玩家数量")]
    [SerializeField]
    byte maxPlayerPerRoom = 20;
    #endregion

    [SerializeField]
    Button joinButton;

    MessageBoard messageBoard;

    // Start is called before the first frame update
    void Start()
    {
        //Connect();
        PhotonNetwork.AutomaticallySyncScene = true;
        messageBoard = GetComponent<MessageBoard>();
        if (joinButton!=null)
        {
            if (PhotonNetwork.IsConnected)
            {
                joinButton.interactable = true;
            }
            else
            {
                joinButton.interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Already Connected!");
            messageBoard.AddText("Already Connected!");
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnected()
    {
        Debug.Log("Connected Successfully!");
        messageBoard.AddText("Connected Successfully!");
        if (joinButton != null)
        {
            joinButton.interactable = true;
        }
        base.OnConnected();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (joinButton != null)
        {
            joinButton.interactable = false;
        }
        base.OnDisconnected(cause);
    }

    public void JoinRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Connect();
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
        Debug.Log("Join room failed, create one!");
        messageBoard.AddText("Join room failed, create one!");

        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Join room successfully!");
        messageBoard.AddText("Join room successfully!");
        if (PhotonNetwork.CurrentRoom.PlayerCount==1)
        {
            Debug.Log("Initialize load level.");
            messageBoard.AddText("Initialize load level.");
            PhotonNetwork.LoadLevel(1);
        }
        base.OnJoinedRoom();
    }
}
