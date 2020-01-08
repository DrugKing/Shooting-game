using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNameInput : MonoBehaviour
{
    // Start is called before the first frame update
    public void ROOMNAME(string room)
    {
        if (string.IsNullOrEmpty(room))
        {
            Debug.Log("NO name input yet");
            room = null;
        }
        LauncherManager.randomRoomName = room;
    }
}
