using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomKitchenNewspaper : RoomScript<RoomKitchenNewspaper>
{


	IEnumerator OnEnterRoomAfterFade()
	{
		
		yield return E.Break;
	}

	IEnumerator OnInteractPropBackToKitchen( IProp prop )
	{
		C.Player.Room=R.LitKitchen;
		yield return E.Break;
	}

	

	IEnumerator OnExitRoom( IRoom oldRoom, IRoom newRoom )
	{
		
		yield return E.Break;
	}

	IEnumerator OnInteractHotspotArticleBeast( IHotspot hotspot )
	{
		yield return C.Display("Hotspot article beasts");
		yield return E.Break;
	}

	IEnumerator OnInteractHotspotArticleDeaths( IHotspot hotspot )
	{
		yield return C.Display("Hotspot article deaths");
		yield return E.Break;
	}

	IEnumerator OnInteractPropNewspaper( IProp prop )
	{
		yield return C.Display("By the hands of a wizard, the creature spellbound");
		yield return C.Display("Imprisoned in waters, its sleeps unsound");
		yield return C.Display("Stone raised upwards of magic sail");
		yield return C.Display("They who decipher will lift the veil");
		
		yield return E.Break;
		
		
		
	}
}