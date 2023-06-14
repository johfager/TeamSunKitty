using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomUnlitKitchen : RoomScript<RoomUnlitKitchen>
{


	IEnumerator OnInteractPropBackground( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAtPropBackground( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnInteractPropNewspaper( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAtPropNewspaper( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnInteractPropMatchBox( IProp prop )
	{
		yield return E.Break;
	}

	IEnumerator OnLookAtPropMatchBox( IProp prop )
	{
		yield return C.FaceClicked();
		yield return C.Display("Ah, a box of matches, the most suicidal tool out there.");
		yield return C.Display("Providing both the tools and the means to destroy itself within itself.");
		yield return C.MainChar.Say("Think that says more about you than the matches.");
		yield return C.MainChar.Say("I've never burned any for fun.");
		yield return C.Display("Who says matches don't love being on fire, hm?");
		yield return C.MainChar.Say("You really are delusional.");
		yield return E.Break;
	}

	

	IEnumerator OnInteractPropFireplace( IProp prop )
	{
		yield return C.Display("Use:");
		yield return C.Display("You're gonna light it up with your hands?");
		yield return C.MainChar.Say("You don't know me or what I can do!");
		yield return C.Display("Most humans I've seen don't know pyromancy.");
		yield return C.Display("And you don't seem THAT special.");
		//MainChar: Fine, be that way!
		//MainChar: I'll find some tool like a normal human.
		yield return E.Break;
	}


	IEnumerator OnUseInvPropFireplace( IProp prop, IInventory item )
	{
		if(item == I.Matches)
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			E.StartCutscene();
			yield return C.MainChar.Say("This looks like a proper use for these little guys.");
			yield return C.Display("As long as they burn, I'm happy.");
			Globals.m_livingRoomLit = true;
		
			E.ChangeRoomBG(R.LitKitchen);
			C.MainChar.SetPosition(R.LitKitchen.GetPoint("LitSpawnPoint"));
		
			I.Matches.Remove(1);
			E.EndCutscene();
		}
		yield return E.Break;
	}

	IEnumerator OnLookAtPropFireplace( IProp prop )
	{
		yield return C.Display("How do you feel watching the flames?");
		yield return E.Break;
	}

	void OnEnterRoom()
	{
	}

	IEnumerator OnInteractHotspotDoorToBedroom( IHotspot hotspot )
	{
		yield return E.ChangeRoom(R.Bedroom);
		yield return E.Break;
	}

	IEnumerator OnLookAtPropWoodpile( IProp prop )
	{
		yield return C.Display("Organic but no intellect? Where’s the fun in that?");
		yield return E.Break;
	}

	IEnumerator OnInteractPropWoodpile( IProp prop )
	{
		yield return C.Display("You're so boring");
		yield return E.Break;
	}

	IEnumerator OnLookAtHotspotWindow( IHotspot hotspot )
	{
		yield return C.Display("Witness MY domain");
		yield return E.Break;
	}

	IEnumerator OnInteractHotspotWindow( IHotspot hotspot )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAtPropClock( IProp prop )
	{
		yield return C.Display("Time. Such a funny concept");
		yield return E.Break;
	}

	IEnumerator OnInteractPropClock( IProp prop )
	{

		yield return E.Break;
	}
}