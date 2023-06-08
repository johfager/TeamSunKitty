using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomLitLivingRoom : RoomScript<RoomLitLivingRoom>
{
	
	
	IEnumerator OnLookAtPropFireplace( IProp prop )
	{
		yield return C.FaceClicked();
		yield return C.Display("Nice and warm. The matches worked wonders.");
		yield return C.MainChar.Say("The ominous glow is concerning though.");
		yield return E.Break;
	}

	IEnumerator OnInteractPropFireplace( IProp prop )
	{
		yield return C.FaceClicked();
		yield return C.MainChar.Say("I don't feel like I should touch that.");
		yield return E.Break;
	}

	IEnumerator OnLookAtHotspotLeftDoor( IHotspot hotspot )
	{

		yield return E.Break;
	}

	IEnumerator OnInteractHotspotLeftDoor( IHotspot hotspot )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
			E.ChangeRoomBG(R.StateStartRoom);
			C.MainChar.SetPosition(R.StateStartRoom.GetPoint("Entry"));
		yield return E.Break;
	}
}