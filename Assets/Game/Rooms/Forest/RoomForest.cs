using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomForest : RoomScript<RoomForest>
{
	// This area is where you can put variables you want to use for game logic in your room
	
	// Here's an example variable, an integer which is used when clicking the sky.
	// The 'm_' at the start is just a naming convention so you can tell it's not just a 'local' variable
	int m_timesClickedSky = 0;
	
	// enums like this are a nice way of keeping track of what's happened in a room
	enum eThingsYouveDone { Start, InsultedChimp, EatenSandwich, LoadedCrossbow, AttackedFlyingNun, PhonedAlbatross }
	eThingsYouveDone m_thingsDone = eThingsYouveDone.Start;
	
	public void OnEnterRoom()
	{
		// Put things here that you need to set up BEFORE the room fades in (but nothing "blocking")
		// Note, you can also just do this at the top of OnEnterRoomAfterFade
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		// Put things here that happen when you enter a room
		
		if ( FirstTimeVisited && EnteredFromEditor == false ) // Only run this part the first time you visit, and not when debugging
		{
		
			yield return C.Dave.Say("Well, I guess this is a test project for an adventure game");
			yield return C.Dave.WalkTo(Point("EntryWalk"));
			yield return C.Dave.Say("Sure looks adventurey!");
			Audio.PlayMusic("MusicExample");
			yield return E.WaitSkip();
			yield return C.Display("Left Click to Walk & Interact\nRight Click to Look At");
		}
		C.Dave.WalkToBG(Point("EntryWalk"));
		
		yield return E.Break;
	}

	public IEnumerator OnInteractHotspotForest( Hotspot hotspot )
	{
		// Here the player just walks to where the mouse is, rather than a particular walk-to point
		yield return C.Dave.WalkTo( E.GetMousePosition() ); 
		yield return C.Dave.FaceUp();
		
		yield return C.Dave.Say("Feels impenetrable");
		
		// Use to quickly check if something's been done before. See also E.FirstOccurrence(...)			
		// The string "useForest" can be anything you want, as long as it's unique the the "occurrence'
		if ( E.Occurrence("useForest") == 2 ) 
		{
			yield return C.Dave.Say("Same as it did the last two times");
		}
		
		yield return E.Break;
		
	}

	public IEnumerator OnInteractPropWell( Prop prop )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		E.StartCutscene();
		yield return E.WaitSkip();
		yield return C.Dave.Say("I can't see anything in the well");
		yield return E.WaitSkip();
		yield return C.Dave.Say("And I'm certainly not climbing down there");
		yield return E.WaitSkip();
		yield return C.Barney.Face(C.Dave);
		yield return C.Barney.Say("Oh go on!");
		yield return C.Dave.Face(C.Barney);
		yield return C.Dave.Say("Ummmm...");
		yield return E.WaitSkip();
		yield return C.FaceClicked();
		yield return E.WaitSkip(1.0f);
		yield return C.Dave.Face(C.Barney);
		yield return E.WaitSkip();
		yield return C.FaceClicked();
		yield return E.WaitSkip(1.0f);
		yield return C.Dave.Face(C.Barney);
		yield return E.WaitSkip(1.0f);
		yield return C.Dave.Say("No");
		yield return E.WaitSkip();
		E.EndCutscene();
		
		yield return E.Break;
		
	}

	public IEnumerator OnInteractHotspotCave( Hotspot hotspot )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return E.WaitSkip();
		yield return C.Dave.Say("No way am I going in there!");
		yield return C.Dave.FaceDown();
		yield return E.WaitSkip();
		yield return C.Dave.Say("There might be beetles");
		
		yield return E.Break;
	}


	public IEnumerator OnInteractPropBucket( Prop prop )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return C.Display("Dave stoops to pick up the bucket");
		Audio.Play("Bucket");
		prop.Disable();
		I.Bucket.AddAsActive();
		yield return E.WaitSkip();
		yield return C.Player.FaceDown();
		yield return C.Dave.Say("Yaaay! I got a bucket!");
		yield return E.WaitSkip();
		yield return C.Display("Access your Inventory from the top of the screen");
		
		yield return E.Break;
		
	}

	public IEnumerator OnUseInvPropWell( Prop prop, Inventory item )
	{
		// NB: You need to check they used the correct item!
		if ( item == I.Bucket )
		{ 
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Display("Dave lowers the bucket down, and collects some juicy well water");
			Globals.m_progressExample = eProgress.GotWater;
			yield return C.Dave.Say("Yaaay! I solved the real hard puzzle!");
			yield return E.Wait(1);
			yield return C.Display("THE END");
			yield return E.WaitSkip();
			yield return C.Dave.FaceDown();
			yield return E.WaitSkip();
			yield return C.Dave.Say("Yaay!");

			// Here we're setting a custom 'enum' so we could check it somewhere else to see if the player won yet
			Globals.m_progressExample = eProgress.WonGame; 		
		}
		yield return E.Break;
		
	}

	public IEnumerator OnUseInvPropBucket( Prop prop, Inventory item )
	{

		yield return E.Break;

	}

	public IEnumerator OnInteractHotspotSky( Hotspot hotspot )
	{
		// Increment the room's variable
		m_timesClickedSky++;  
		// Show some text including the variable. Use {braces} to include variables in text
		yield return C.Display($"You've clicked the sky {m_timesClickedSky} times");
		
	}


	public IEnumerator OnLookAtHotspotForest( IHotspot hotspot )
	{
		yield return C.Dave.FaceUp();

		// YOu can use FirstLook, FirstUse, LookCount, and UseCount to change what happens on subsequent clicks
		if ( hotspot.FirstLook )
			yield return C.Dave.Say("Looks impenetrable");
		else 
			yield return C.Dave.Say("Still looks impenetrable");
		yield return E.Break;
	}

	public IEnumerator OnEnterRegionCorner( IRegion region, ICharacter character )
	{
		// Here the player has entered a region. Regions can also just be used to scale or tint the character
		
		yield return E.WaitSkip();
		C.Dave.StopWalking();
		yield return E.WaitSkip();
		yield return C.Dave.FaceDown();
		yield return E.WaitSkip();
		yield return C.Dave.Say("This corner gives me the heebie jeebies");
		yield return E.WaitSkip(0.25f);
		yield return C.Barney.Face(C.Dave);
		yield return C.Barney.Say("Yeah, stay away from that corner dude, it has a Tint colour.");
		yield return E.WaitSkip(0.25f);
		yield return C.Dave.FaceUp();
		yield return C.Dave.Say("Good idea, Let's set it's Walkable property to false so I'll never make the same mistake again");
		yield return C.Plr.WalkTo(124, -67);
		
		// Setting a region's Walkable" as false will stop the player walking there
		Region("Corner").Walkable = false;
	}

	public IEnumerator OnLookAtPropWell( IProp prop )
	{
		yield return C.FaceClicked();		
		yield return C.Dave.Say("Well well well");
		yield return E.Break;
	}

	IEnumerator OnLookAtPropBucket( IProp prop )
	{

		yield return E.Break;
	}


	IEnumerator OnInteractCharacterBarney( ICharacter character )
	{
		// You can add character interactions like this one to a room script
		
		// In that room, clicking the character will call the room script first
		// If the room script doesn't do anything, it will fall back to the Charcter Script
		
		// In this case m_thingsDone is not 'EatenSandwich', so nothing happens here, 
		// and the OnInteract function in Barney's main script will be called instead
		if ( m_thingsDone == eThingsYouveDone.EatenSandwich )
		{
			yield return C.Dave.Say("I ate your sandwich");
			yield return C.Barney.Say("You monster");
		}
		
		yield return E.Break;
	}
}
