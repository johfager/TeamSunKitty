using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class InventoryBucket : InventoryScript<InventoryBucket>
{

	public IEnumerator OnLookAtInventory( IInventory thisItem )
	{
		yield return C.Display("It's a blue cup");
		yield return C.Display("I mean... a bucket");
		yield return E.Break;
		
	}

	IEnumerator OnUseInvInventory( IInventory thisItem, IInventory item )
	{

		yield return E.Break;
	}

	IEnumerator OnInteractInventory( IInventory thisItem )
	{

		yield return E.Break;
	}
}
