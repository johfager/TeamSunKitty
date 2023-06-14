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
		yield return C.Display("...Our dear friend Storsjöodjuret has yet again been spotted ...");
		yield return C.Display("...saw the shimmering of a large tail, long and thin and snake-like...");
		yield return C.Display(" ...These anecdotes are becoming ever so more frequent...");
		yield return C.Display("...individuals from our very own Östersund. ...");
		yield return C.Display("...Yet Storsjöodjuret is dearly loved and revered by many");
		yield return C.Display("...Länstidningen Östersund, Monday the 4th of June 1976");
		
		
		
		yield return E.Break;
	}

	IEnumerator OnInteractHotspotArticleDeaths( IHotspot hotspot )
	{
		yield return C.Display("Eivar Björklund's Daughter Missing - Presumed Dead Police rally the people for help");
		yield return C.Display("Maria Björklund, daughter of our very own Eivar Björklund, owner of Björnklund's Livs, is reported missing as of 7:45 Tuesday the 5th of June");
		yield return C.Display("Maria is 15 years old, 153cm and has shoulder-length blonde hair and freckles.");
		yield return C.Display("She was last seen wearing a beige sweater. The police are doing the best of their efforts but with evidence coming up short, they are reaching out to the community of Östersund to keep their eyes open for the missing child.");
		yield return C.Display(" Search parties are to be assembled at a moment's notice.");
		yield return C.Display("For any tips or sightings please call Östersun's Police directly at: 063 XXX XXXX.");
		yield return C.Display("Länstidningen Östersund, Tuesday the 15th of June 1980");
		yield return E.Break;
	}

	IEnumerator OnInteractPropNewspaper( IProp prop )
	{
		yield return C.Display("By the hands of a wizard, the creature spellbound");
		yield return C.Display("Imprisoned in waters, its sleeps unsound");
		yield return C.Display("Stone raised upwards of magic sail");
		yield return C.Display("They who decipher will lift the veil");
		yield return C.Display("The poem seems to be cut off here…");
		
		yield return E.Break;
		
		
		
	}
}