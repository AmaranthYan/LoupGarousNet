﻿namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(PhotonView))]
    public class GameSessionServiceView : PunBehaviour
    {
        [SerializeField]
        private GameConfigView m_GameConfigView = null;

        public UnityEvent onStartRemoteGameSession = new UnityEvent();
        public UnityEvent onEndRemoteGameSession = new UnityEvent();
        public UnityEvent onEndLocalGameSession = new UnityEvent();

        public void StartGameSession()
        {
            if (PhotonNetwork.player.isMasterClient)
            {
                photonView.RPC("StartGameSession", PhotonTargets.AllViaServer);
            }
        }

        [PunRPC]
        private void StartRemoteGameSession()
        {
            GameSessionService.StartGameSession(m_GameConfigView);
            GameSessionService.GameSession.transform.parent = this.transform;
            onStartRemoteGameSession.Invoke();
        }

        public void EndGameSession()
        {
            if (PhotonNetwork.player.isMasterClient)
            {
                photonView.RPC("EndGameSession", PhotonTargets.AllViaServer);
            }
        }

        [PunRPC]
        private void EndRemoteGameSession()
        {
            GameSessionService.EndGameSession();
            onEndRemoteGameSession.Invoke();
        }

        public void EndLocalGameSession()
        {
            GameSessionService.EndGameSession();
            onEndLocalGameSession.Invoke();
        }
    }
}