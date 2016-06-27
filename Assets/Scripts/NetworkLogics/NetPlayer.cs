﻿using Photon;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class NetPlayer : PunBehaviour
{
    public const string ANONYMOUS = "匿名爆狼";
    public const string ANONYMOUS_FORMAT = "{0}#{1:00}";

    [SerializeField]
    private PhotonIdentifier m_PhotonIdentifier = null;
    private IEnumerator m_RequestRoomList_Coroutine = null;

    [Header("Events")]
    public UnityTypedEvent.StringEvent onPhotonEvent = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.HashtableEvent onRoomListUpdate = new UnityTypedEvent.HashtableEvent();
    public UnityTypedEvent.HashtableEvent onPlayerListUpdate = new UnityTypedEvent.HashtableEvent();
    public UnityEvent onJoinRoom = new UnityEvent();
    public UnityEvent onLeaveRoom = new UnityEvent();

    void Awake()
    {
        if (!m_PhotonIdentifier) {
            Debug.LogError("PUN项目配置错误！");
            this.enabled = false;
        };
    }
        
    void Start()
    {
        ConnectToPUN();
    }

    void Update()
    {

    }

    #region PhotonCallbacks
    public override void OnConnectedToPhoton()
    {
        base.OnConnectedToPhoton();
        onPhotonEvent.Invoke("已连接至PUN。");
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        base.OnFailedToConnectToPhoton(cause);
        Debug.LogError("连接PUN失败，原因[" + cause + "]");
        onPhotonEvent.Invoke("<color=#800000ff>连接PUN失败！</color>");
    }

    public override void OnDisconnectedFromPhoton()
    {
        base.OnDisconnectedFromPhoton();
        onPhotonEvent.Invoke("已从PUN断开。");
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        base.OnConnectionFail(cause);
        onPhotonEvent.Invoke("<color=#800000ff>连接被中断！</color>");
    }

    public override void OnPhotonMaxCccuReached()
    {
        base.OnPhotonMaxCccuReached();
        onPhotonEvent.Invoke("<color=#800000ff>当前服务器连接已达到上限！</color>");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        onPhotonEvent.Invoke("已接入大厅。");
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        onPhotonEvent.Invoke("已离开大厅。");
    }

    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate();
        FetchRoomList();
        onPhotonEvent.Invoke("房间列表已更新。");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        PhotonNetwork.SetMasterClient(PhotonNetwork.player);
        onPhotonEvent.Invoke("已创建房间\"" + PhotonNetwork.room.name + "\"。");
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        base.OnPhotonCreateRoomFailed(codeAndMsg);
        //short code = (short)codeAndMsg[0];
        //string msg = (string)codeAndMsg[1];
        onPhotonEvent.Invoke("<color=#800000ff>创建房间失败！</color>");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        onJoinRoom.Invoke();
        if (PhotonNetwork.masterClient == null)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.player);
        }
        FetchPlayerList();
        onPhotonEvent.Invoke("已加入房间\"" + PhotonNetwork.room.name + "\"，当前人数为" + PhotonNetwork.room.playerCount + "人。");
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {            
        base.OnMasterClientSwitched(newMasterClient);
        if (PhotonNetwork.player.Equals(newMasterClient))
        {
            onPhotonEvent.Invoke("已获得房主权限。");
        }
        else
        {
            onPhotonEvent.Invoke("玩家\"" + FetchPlayerName(newMasterClient) + "\"已成为房主。");
        }
            
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        base.OnPhotonPlayerConnected(newPlayer);
        FetchPlayerList();
        onPhotonEvent.Invoke("玩家\"" + FetchPlayerName(newPlayer) + "\"已加入房间，当前人数为" + PhotonNetwork.room.playerCount + "人。");
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);
        FetchPlayerList();
        onPhotonEvent.Invoke("玩家\"" + FetchPlayerName(otherPlayer) + "\"已离开房间，当前人数为" + PhotonNetwork.room.playerCount + "人。");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        onLeaveRoom.Invoke();
        onPhotonEvent.Invoke("已离开房间。");
    }        
    #endregion

    public static string FetchPlayerName(PhotonPlayer photonPlayer)
    {
        string anonymous = photonPlayer.ID > 0 ? string.Format(ANONYMOUS_FORMAT, ANONYMOUS, photonPlayer.ID) : ANONYMOUS;
        return string.IsNullOrEmpty(photonPlayer.name) ? anonymous : photonPlayer.name;
    }

    public void FetchRoomList()
    {
        RoomInfo[] roomInfo = PhotonNetwork.GetRoomList();
        Hashtable hashtable = new Hashtable();
        foreach (RoomInfo rI in roomInfo)
        {
            hashtable.Add(rI.name, rI);
        }
        onRoomListUpdate.Invoke(hashtable);
    }

    public void FetchPlayerList()
    {
        if (PhotonNetwork.room == null) { return; }

        PhotonPlayer[] photonPlayer = PhotonNetwork.playerList;
        Hashtable hashtable = new Hashtable();
        foreach (PhotonPlayer pP in photonPlayer)
        {
            hashtable.Add(pP.ID.ToString(), pP);
        }
        onPlayerListUpdate.Invoke(hashtable);
    }

    private void ConnectToPUN()
    {
        onPhotonEvent.Invoke("正在连接PUN...");
        PhotonNetwork.ConnectUsingSettings(m_PhotonIdentifier.GameIdentifier + '_' + m_PhotonIdentifier.GameVersion);
    }
}