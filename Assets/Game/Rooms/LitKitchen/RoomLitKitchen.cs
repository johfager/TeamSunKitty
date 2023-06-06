using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomLitKitchen : RoomScript<RoomLitKitchen>
{

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
		yield return C.Display("Long ago by Storsjön shore");
		yield return C.Display("Trolls Jata and Kata whose cauldron bore");
		yield return C.Display("A boiling brew they curiously tend");
		yield return C.Display("Ingredients galore holding no end");
		
		yield return C.Display("One day the cauldron shouted a cry");
		yield return C.Display("A serpent, a creature, a cat head held high");
		yield return C.Display("It vanishe in waters growing in might");
		yield return C.Display("Eyes avert, true horror in sight");
		
		yield return C.Display("By the hands of a wizard the creature spellbound");
		yield return C.Display("Imprisoned in a watery rest unsound.");
		yield return C.Display("Stone raised, Magic concealed");
		yield return C.Display("Decipher, set free, wield");
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
}