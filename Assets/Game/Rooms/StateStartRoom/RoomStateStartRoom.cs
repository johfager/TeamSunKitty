using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomStateStartRoom : RoomScript<RoomStateStartRoom>
{


	IEnumerator OnInteractHotspotDoor( IHotspot hotspot )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		
		if(Globals.m_livingRoomLit == false)
		{
			E.ChangeRoomBG(R.StateExperimentRoom);
			C.Ulrika.SetPosition(R.StateExperimentRoom.GetPoint("Entry"));
		}
		else
		{
			E.ChangeRoomBG(R.LitLivingRoom);
			C.Ulrika.SetPosition(R.LitLivingRoom.GetPoint("LeftEntry"));
		}
		
		yield return E.Break;
	}
}