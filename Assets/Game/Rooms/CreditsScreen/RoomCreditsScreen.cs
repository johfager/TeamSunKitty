using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomCreditsScreen : RoomScript<RoomCreditsScreen>
{


	void OnEnterRoom()
	{
		Audio.PlayMusic("SoundLake Town [Mastered]");
	}
}