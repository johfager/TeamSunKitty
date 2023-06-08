using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Event_Holder : MonoBehaviour
{
    public VoidEventSO voidEvent;

    public RoomStateExperimentRoom _room;

    private void Start()
    {
        //_room = GetComponent<RoomStateExperimentRoom>();

        if (voidEvent != null && _room != null)
        {
            _room.LightRoomEvent = voidEvent;
            Debug.Log("Gave the event");
        }
    }
}
