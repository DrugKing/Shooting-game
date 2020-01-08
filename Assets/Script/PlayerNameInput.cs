using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNameInput : MonoBehaviour
{
    public void PLAYERNAME(string player)
    {
        if (string.IsNullOrEmpty(player))
        {
            Debug.Log("NO name input yet");
            player = "";
        }
        PhotonNetwork.NickName = player;
    }
}
