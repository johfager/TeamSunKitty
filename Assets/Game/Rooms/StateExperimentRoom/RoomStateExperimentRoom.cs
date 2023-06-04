using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomStateExperimentRoom : RoomScript<RoomStateExperimentRoom>
{


	IEnumerator OnInteractPropMatchBox( IProp prop )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return C.Display("You found some matches!");
		I.Matches.AddAsActive();
		prop.Disable();
		yield return E.WaitSkip();
		yield return C.MainChar.Say("Ooh, maybe I could burn the beast?");
		yield return C.Display("You quickly stop having that thought.");
		yield return E.Break;
	}

	IEnumerator OnLookAtPropMatchBox( IProp prop )
	{
		yield return C.FaceClicked();
		yield return C.Display("Ah, a box of matches, the most suicidal tool out there.");
		yield return C.Display("Providing both the tools and the means to destroy itself within itself.");
		yield return C.MainChar.Say("Think that says more about you than the matches.");
		yield return C.MainChar.Say("I've taken great care of mine.");
		yield return C.Display("But now you're curious, right?");
		yield return C.MainChar.Say("...");
		yield return E.Break;
	}

	IEnumerator OnUseInvPropMatchBox( IProp prop, IInventory item )
	{

		yield return E.Break;
	}
}