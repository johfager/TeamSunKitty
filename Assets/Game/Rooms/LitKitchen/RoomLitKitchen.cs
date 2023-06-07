using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomLitKitchen : RoomScript<RoomLitKitchen>
{
	[SerializeField]
    

	void OnEnterRoom()
	{
		C.Player.SetPosition(Point("Enter"));
	}

	IEnumerator OnInteractHotspotDoorToBedroom( IHotspot hotspot )
	{
		yield return C.WalkToClicked();
		C.Player.Room=R.Bedroom;
		yield return E.Break;
	}

	IEnumerator OnInteractPropNewspaper( IProp prop )
	{
		
		yield return C.WalkToClicked();
		C.Player.Room=R.KitchenNewspaper;
		
		yield return E.Break;
	}

	IEnumerator OnInteractPropFireplace( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAtPropFireplace( IProp prop )
	{
		yield return C.Display("“How do you feel watching the flames?”");
		yield return E.Break;
	}

	IEnumerator OnLookAtHotspotWindow( IHotspot hotspot )
	{
		yield return C.Display("“Witness MY domain.”");
		yield return E.Break;
	}

	IEnumerator OnLookAtPropClock( IProp prop )
	{
		yield return C.Display("“Time. Such a funny concept.”");
		yield return E.Break;
	}

	IEnumerator OnLookAtPropNewspaper( IProp prop )
	{
		yield return C.WalkToClicked();
		yield return C.Display("“Breaking news! I’m going to kill you.”");
		yield return E.Break;
	}

	IEnumerator OnInteractPropWoodpile( IProp prop )
	{
		yield return E.Break;
	}

	IEnumerator OnLookAtPropWoodpile( IProp prop )
	{
		yield return C.Display("“Organic but no intellect? Where’s the fun in that?”");
		
		yield return E.Break;
	}

	IEnumerator OnInteractHotspotDoorToTimeskip( IHotspot hotspot )
	{
		yield return C.WalkToClicked();
		C.Player.Room=R.EndScene;
		yield return E.Break;
	}

	IEnumerator OnInteractHotspotToReadNewspaper( IHotspot hotspot )
	{
		yield return C.WalkToClicked();
		C.Player.Room=R.KitchenNewspaper;
		yield return E.Break;
	}

	IEnumerator OnEnterRoomAfterFade()
	{

		yield return E.Break;
	}
}