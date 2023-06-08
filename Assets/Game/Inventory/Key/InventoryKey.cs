using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class InventoryKey : InventoryScript<InventoryKey>
{


	IEnumerator OnLookAtInventory( IInventory thisItem )
	{
		yield return C.Display("It's a key, wonder where it goes...");
		
		yield return E.Break;
	}

	IEnumerator OnUseInvInventory( IInventory thisItem, IInventory item )
	{
		
		yield return E.Break;
	}
}