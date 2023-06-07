using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomTitlescreen : RoomScript<RoomTitlescreen>
{


	IEnumerator OnInteractPropNew( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAtPropNew( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnUseInvPropNew( IProp prop, IInventory item )
	{

		yield return E.Break;
	}
}