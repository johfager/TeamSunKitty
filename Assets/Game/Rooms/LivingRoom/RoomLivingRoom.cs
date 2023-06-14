using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomLivingRoom : RoomScript<RoomLivingRoom>
{


	IEnumerator OnInteractHotspotDoorToBedRoom( IHotspot hotspot )
	{
		
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		
		E.ChangeRoomBG(R.Bedroom);
		C.Ulrika.SetPosition(600, -200);
		yield return E.Break;
	}

	void OnEnterRoom()
	{
	}

	IEnumerator OnEnterRoomAfterFade()
	{
		yield return C.Ulrika.Say("Hmm did the coders do anything to this room?");
		yield return C.Display("Nope");
		yield return C.Display("The bedroom is probably more interesting");
		yield return E.Break;
	}
}