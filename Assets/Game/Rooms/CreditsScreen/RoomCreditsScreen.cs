using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomCreditsScreen : RoomScript<RoomCreditsScreen>
{


	void OnEnterRoom()
	{
		G.InventoryBar.Hide();
		G.Toolbar.Hide();
		Audio.PlayMusic("SoundLake Town [Mastered]");
	}

	IEnumerator OnEnterRoomAfterFade()
	{
		Audio.PlayMusic("SoundLake Town [Mastered]");
		yield return E.Break;
	}
}