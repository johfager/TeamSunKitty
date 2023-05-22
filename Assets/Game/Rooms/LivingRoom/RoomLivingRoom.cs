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
		yield return E.Break;
	}
}