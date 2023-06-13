using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState : MonoBehaviour
{
    [SerializeField] private VoidEventSO lightRoomEvent;
    [SerializeField] private Sprite litRoomSprite;
    private SpriteRenderer _sr;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        lightRoomEvent.OnEventRaised += RoomIsLit;
    }

    private void OnDisable()
    {
        lightRoomEvent.OnEventRaised -= RoomIsLit;
    }

    private void RoomIsLit()
    {
        _sr.sprite = litRoomSprite;
    }
}
