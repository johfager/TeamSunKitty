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
	bool m_bedRoomDoorUnlocked = false;
	// enums like this are a nice way of keeping track of what's happened in a room
	enum eThingsYouveDone { Start, InsultedChimp, EatenSandwich, LoadedCrossbow, AttackedFlyingNun, PhonedAlbatross}
	eThingsYouveDone m_thingsDone = eThingsYouveDone.Start;
	IEnumerator OnInteractHotspotDoorToKitchen( IHotspot hotspot )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		
		if (m_bedRoomDoorUnlocked == true)
		{
			E.ChangeRoomBG(R.LivingRoom);
			C.MainChar.SetPosition(-600, -200);
		}
		else
		{
			yield return C.Display(" Door seems to be locked");
		}
		
		
		yield return E.Break;
	}

	IEnumerator OnUseInvHotspotDoorToKitchen( IHotspot hotspot, IInventory item )
	{
		
		// NB: You need to check they used the correct item!
		if ( item == I.Active )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Display("MainChar swiftly turns the key hearing a click sound");
			Globals.m_progressExample = eProgress.BedRoomUnlocked;
			yield return C.Dave.Say("Yaaay! I solved the real hard puzzle!");
			m_bedRoomDoorUnlocked = true;
		
			yield return E.Wait(1);
			yield return E.WaitSkip();
			yield return C.MainChar.FaceDown();
			yield return E.WaitSkip();
			yield return C.MainChar.Say("Yaay!");
		}
		
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
		//m_rugInteract = true;
		
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

	IEnumerator OnInteractPropKey( IProp prop )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return C.Display("Dave stoops to pick up the bucket");
		Audio.Play("Bucket");
		prop.Disable();
		I.Key.AddAsActive();
		yield return E.WaitSkip();
		yield return C.Player.FaceDown();
		yield return C.MainChar.Say("Yaaay! I got a bucket!");
		yield return E.WaitSkip();
		yield return C.Display("Access your Inventory from the top of the screen");
		
		yield return E.Break;
		
		yield return E.Break;
	}

	IEnumerator OnLookAtHotspotDoorToKitchen( IHotspot hotspot )
	{

		yield return E.Break;
	}
}