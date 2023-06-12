using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomUnlitKitchen : RoomScript<RoomUnlitKitchen>
{


	IEnumerator OnInteractPropBackground( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAtPropBackground( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnInteractPropNewspaper( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAtPropNewspaper( IProp prop )
	{

		yield return E.Break;
	}
}