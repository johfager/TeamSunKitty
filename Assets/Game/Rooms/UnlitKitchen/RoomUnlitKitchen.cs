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
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return C.Display("You found some matches!");
		I.Matches.AddAsActive();
		prop.Disable();
		yield return E.WaitSkip();
		yield return C.MainChar.Say("Ooh, maybe I could burn the beast?");
		yield return C.Display("You quickly abolish the thought.");
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
		yield return C.MainChar.Say("Fine, be that way!");
		yield return C.MainChar.Say("I'll find some tool like a normal human.");
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
		yield return C.Display("Looking:");
		yield return C.Display("Looks like a fireplace.");
		yield return C.MainChar.Say("A sinister one, sure.");
		yield return C.MainChar.Say("Maybe I could light it somehow.");
		yield return E.Break;
	}
}