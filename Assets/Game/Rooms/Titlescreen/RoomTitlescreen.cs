using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomTitlescreen : RoomScript<RoomTitlescreen>
{


	IEnumerator OnInteractPropNew( IProp prop )
	{
		// Turn on the inventory and info bar now that we're starting a game
		G.InventoryBar.Show();
		G.Save.Show();
		G.Toolbar.Show();
		Audio.StopMusic(2);
		// Move the player to the room
		E.ChangeRoomBG(R.Bedroom);
		yield return E.ConsumeEvent;
		
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

	void OnEnterRoom()
	{
		G.InventoryBar.Hide();
		G.Save.Hide();
		G.Toolbar.Hide();
		Audio.PlayMusic("SoundMain Theme [Mastered]");
		
	}

	IEnumerator OnInteractPropContinue( IProp prop )
	{
		// Restore most recent save game
		E.RestoreLastSave();
		yield return E.ConsumeEvent;
		yield return E.Break;
	}
}