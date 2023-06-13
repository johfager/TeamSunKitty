using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomBedroomPuzzle : RoomScript<RoomBedroomPuzzle>
{


	IEnumerator OnInteractPropBedroomKey( IProp prop )
	{
		yield return E.ChangeRoom(R.Bedroom);
		yield return E.Break;
	}

	IEnumerator OnLookAtPropBedroomKey( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnUseInvPropBedroomKey( IProp prop, IInventory item )
	{
		yield return C.MainChar.Say("I'm not sure if I should use this...");
		yield return E.Break;
	}

	void Update()
	{
	}

	IEnumerator OnInteractPropBackground( IProp prop )
	{
		
		yield return E.Break;
	}

	void OnEnterRoom()
	{
	}

	IEnumerator OnInteractPropWinCondition( IProp prop )
	{
		if(Globals.m_runePuzzleFinishedBedRoom == true){
			yield return E.ChangeRoom(R.Bedroom);
		}
		yield return E.Break;
	}
}