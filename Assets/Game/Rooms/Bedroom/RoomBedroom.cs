using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomBedroom : RoomScript<RoomBedroom>
{


	IEnumerator OnInteractHotspotDoorToKitchen( IHotspot hotspot )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		
		E.ChangeRoomBG(R.LivingRoom);
		yield return E.Break;
	}

	IEnumerator OnUseInvHotspotDoorToKitchen( IHotspot hotspot, IInventory item )
	{

		yield return E.Break;
	}

	IEnumerator OnInteractPropBackground( IProp prop )
	{

		yield return E.Break;
	}
}