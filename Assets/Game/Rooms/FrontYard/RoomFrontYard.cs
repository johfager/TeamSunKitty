using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomFrontYard : RoomScript<RoomFrontYard>
{


	IEnumerator OnUseInvPropStatue( IProp prop, IInventory item )
	{
		
		
		yield return E.Break;
	}

	IEnumerator OnLookAtPropStatue( IProp prop )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return C.MainChar.Say("It looks like an old fish...");
		yield return E.Break;
	}

	IEnumerator OnInteractPropStatue( IProp prop )
	{
		yield return C.MainChar.Say("It hard as a rock...");
		yield return E.Break;
	}
}