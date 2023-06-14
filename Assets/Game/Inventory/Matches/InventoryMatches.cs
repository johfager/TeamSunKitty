using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class InventoryMatches : InventoryScript<InventoryMatches>
{


	IEnumerator OnLookAtInventory( IInventory thisItem )
	{
		yield return C.Display("A seemingly infinite supply of matches, how convenient.");
		yield return C.Ulrika.Say("I'm not lighting one for fun, there's gotta be a purpose!");
		yield return E.Break;
	}

	IEnumerator OnInteractInventory( IInventory thisItem )
	{

		yield return E.Break;
	}

	IEnumerator OnUseInvInventory( IInventory thisItem, IInventory item )
	{

		yield return E.Break;
	}
}