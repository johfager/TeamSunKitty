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
		Audio.Stop("SoundLakeTown");
		Audio.PlayMusic("SoundGame Over [Mastered]");
	}

	IEnumerator OnEnterRoomAfterFade()
	{
		//Audio.PlayMusic("SoundLakeTown");
		yield return E.Break;
	}

	IEnumerator OnInteractPropDoOver( IProp prop )
	{
		
			//copy globals initiateGlobals.eProgress m_progressExample = eProgress.None;
		
		//Globals.m_progressExample=eProgress.None;
		//E.ChangeRoomBG(R.Bedroom);
		//Prop("Background").Canvas.Disable();
		GuiSave.Script.ShowRestore();
		
		
		yield return E.Break;
	}
}