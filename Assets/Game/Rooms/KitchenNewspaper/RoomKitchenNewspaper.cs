using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomKitchenNewspaper : RoomScript<RoomKitchenNewspaper>
{


	IEnumerator OnEnterRoomAfterFade()
	{
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

	IEnumerator OnInteractPropBackToKitchen( IProp prop )
	{
		C.Player.Room=R.LitKitchen;
		yield return E.Break;
	}

	

	IEnumerator OnExitRoom( IRoom oldRoom, IRoom newRoom )
	{
		
		yield return E.Break;
	}
}