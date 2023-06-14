using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomSplashScreen : RoomScript<RoomSplashScreen>
{


	void OnEnterRoom()
	{
	}

	IEnumerator OnEnterRoomAfterFade()
	{
		G.InventoryBar.Hide();
		G.Toolbar.Hide();
		Cursor.Visible = false;
		// Start cutscene, so this can be skipped by pressing ESC
		E.StartCutscene();
		// Play Jingle
		Audio.Play("SoundLogo Jingle [Mastered]");
		// Fade in the title prop
		Prop("Logo").Visible = true;
		yield return Prop("Logo").Fade(0,1,1.0f);
		
		// Wait a moment
		yield return E.Wait(1.0f);
		yield return E.Wait(1.0f);
		// This is the point the game will skip to if ESC is pressed
		E.EndCutscene();
		
		yield return E.ChangeRoom(R.Title);
		
		yield return E.Break;
	}
}