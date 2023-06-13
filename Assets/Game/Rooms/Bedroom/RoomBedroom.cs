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
	bool m_doorKnobUsedOnDoor = false;
	bool m_runePuzzleFinishedBedRoom = false;
	// enums like this are a nice way of keeping track of what's happened in a room
	enum eThingsYouveDone { Start, InsultedChimp, EatenSandwich, LoadedCrossbow, AttackedFlyingNun, PhonedAlbatross, DoorKnobUsedOnDoor}
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
		
		if ( item == I.DoorKnob)
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Display("MainChar pushes the doorknob into the hole hearing a clunk");
			m_doorKnobUsedOnDoor = true;
			Prop("DoorKnobForDoor").Enable();
			yield return E.Wait(1);
			yield return E.WaitSkip();
			yield return C.MainChar.FaceDown();
			yield return E.WaitSkip();
			yield return C.MainChar.Say("Yaay!");
			I.DoorKnob.Remove();
		}
		
		// NB: You need to check they used the correct item!
		if ( item == I.Key & m_doorKnobUsedOnDoor == true )
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
			I.Key.Remove();
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
		yield return C.WalkToClicked();
		yield return C.MainChar.Say("Looks like there is something under this rug");
		yield return E.WaitSkip();
		Prop("Rug").Disable();
		Hotspot("RuneInteraction").Enable();
		yield return E.Break;
	}

	IEnumerator OnInteractHotspotRuneInteraction( IHotspot hotspot )
	{
		yield return C.MainChar.Say("I don't remember seeing this before");
		yield return E.WaitSkip();
		yield return C.MainChar.Say("It looks like they can be moved");
		
		yield return E.ChangeRoom(R.BedroomPuzzle);
		
		
		
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
		
		if(m_doorKnobUsedOnDoor == false)
		{
			Prop("DoorKnobForDoor").Disable();
			Hotspot("RuneInteraction").Disable();
			Prop("Key").Disable();
			Prop("Matches").Disable();
			Prop("Cupboard_open").Disable();
			Prop("DoorKnob").Disable();
			Prop("RunePuzzleFinished").Disable();
		
		}
		if(Globals.m_runePuzzleFinishedBedRoom == false & m_doorKnobUsedOnDoor == false)
		{
			Prop("DoorKnob").Disable();
			Prop("RunePuzzleFinished").Disable();
		
		}
		else
		{
			C.Display("As the rune shifted into its original place, pieces of the floor could be removed");
			E.WaitSkip();
			C.MainChar.MoveTo(Point("PositionAfterPuzzle"));
			C.MainChar.Enable();
			Prop("RunePuzzleFinished").Enable();
			Prop("Cupboard_open").Enable();
			Prop("DoorKnob").Enable();
			Hotspot("RuneInteraction").Disable();
		
		}
		
		
		
	}

	IEnumerator OnLookAtHotspotRuneInteraction( IHotspot hotspot )
	{

		yield return E.Break;
	}

	IEnumerator OnInteractPropKey( IProp prop )
	{
		yield return C.WalkToClicked();
		yield return C.Display("Surprise, key was picked up");
		Audio.Play("Bucket");
		prop.Disable();
		I.Key.Add();
		yield return E.WaitSkip();
		yield return C.Player.FaceDown();
		yield return C.MainChar.Say("Yaaay! I got a key! I wish it was a bucket tho :(");
		yield return E.WaitSkip();
		yield return C.Display("Access your Inventory from the top of the screen");
		
		
		yield return E.Break;
	}

	IEnumerator OnLookAtHotspotDoorToKitchen( IHotspot hotspot )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAtPropKey( IProp prop )
	{

		yield return E.Break;
	}


	IEnumerator OnInteractPropDoorKnob( IProp prop )
	{
		prop.Disable();
		I.DoorKnob.Add();
		yield return E.Break;
	}

	IEnumerator OnInteractPropCupboard( IProp prop )
	{
		yield return C.MainChar.Say("Something seems to be inside of this cupboard...");
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		Prop("Cupboard").Disable();
		Prop("Cupboard_open").Enable();
		Prop("Key").Enable();
		Prop("Matches").Enable();
		yield return E.Break;
	}

	IEnumerator OnInteractPropMatches( IProp prop )
	{
		yield return C.WalkToClicked();
		Audio.Play("Bucket");
		prop.Disable();
		I.Matches.Add();
		yield return E.WaitSkip();
		yield return C.Player.FaceDown();
		yield return C.MainChar.Say("A pack of matches? This might come in use later.");
		yield return E.WaitSkip();
		
		yield return E.Break;
	}

	IEnumerator OnLookAtPropDoorKnob(IProp prop)
	{
		yield return E.Break;
	}
	IEnumerator OnUseInvHotspotRuneInteraction( IHotspot hotspot, IInventory item )

	{

		yield return E.Break;
	}

	IEnumerator OnInteractPropCupboard_open( IProp prop )
	{

		yield return E.Break;
	}
}