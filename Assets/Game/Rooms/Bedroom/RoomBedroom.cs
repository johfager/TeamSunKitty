using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomBedroom : RoomScript<RoomBedroom>
{

	
	// This area is where you can put variables you want to use for game logic in your room
	
	// Here's an example variable, an integer which is used when clicking the sky.
	// The 'm_' at the start is just a naming convention so you can tell it's not just a 'local' variable
	int m_timesClickedSky = 0;
	
	// enums like this are a nice way of keeping track of what's happened in a room
	enum eThingsYouveDone { Start, InsultedChimp, EatenSandwich, LoadedCrossbow, AttackedFlyingNun, PhonedAlbatross }
	eThingsYouveDone m_thingsDone = eThingsYouveDone.Start;
	public bool m_rugInteract = false;
	
	
	IEnumerator OnInteractHotspotDoorToKitchen( IHotspot hotspot )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		
		E.ChangeRoomBG(R.LivingRoom);
		C.MainChar.SetPosition(-600, -200);
		yield return E.Break;
	}

	IEnumerator OnUseInvHotspotDoorToKitchen( IHotspot hotspot, IInventory item )
	{

		yield return E.Break;
	}

	IEnumerator OnInteractPropBackground( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAtPropBed( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnInteractPropRug( IProp prop )
	{
		m_rugInteract = true;
		
		Prop("Rug").Disable();
		Hotspot("RuneInteraction").Enable();
		yield return E.Break;
	}

	IEnumerator OnInteractHotspotRuneInteraction( IHotspot hotspot )
	{
		yield return C.MainChar.Say("What is this? Why does it look so familiar...");
		yield return C.Display(" Yes, you are 'using' this rune");
		yield return E.Break;
	}

	IEnumerator OnEnterRoomAfterFade()
	{
		// Put things here that happen when you enter a room
		
		if ( FirstTimeVisited && EnteredFromEditor == false ) // Only run this part the first time you visit, and not when debugging
		{
			yield return C.MainChar.Say("Well, I guess this is a bedroom lol");
			yield return C.MainChar.WalkTo(Point("EntryWalk"));
			yield return C.MainChar.Say("Sure looks like a bed to me!");
			Audio.PlayMusic("MusicExample");
		
			yield return C.Display("Left Click to Walk & Interact\nRight Click to Look At");
		}
		yield return E.Break;
	}

	IEnumerator OnInteractPropBed( IProp prop )
	{
		yield return C.Display("Yes, you are seeing things correctly. This is a sprillans new IKEA model BengtOlof");
		yield return E.Break;
	}

	void OnEnterRoom()
	{
	}

	IEnumerator OnLookAtHotspotRuneInteraction( IHotspot hotspot )
	{

		yield return E.Break;
	}
}