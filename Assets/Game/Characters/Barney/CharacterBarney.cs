using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class CharacterBarney : CharacterScript<CharacterBarney>
{


	public IEnumerator OnInteract()
	{
		yield return C.FaceClicked();
		yield return C.Dave.Say("Hey Barney!");
		yield return C.Barney.Face(C.Dave);
		yield return C.Barney.Say("Yeah?");
		yield return C.WalkToClicked();
		D.ChatWithBarney.Start();
		Globals.m_spokeToBarney = true;
		yield return E.Break;
		
	}

	IEnumerator OnUseInv( IInventory item )
	{

		yield return E.Break;
	}

	IEnumerator OnLookAt()
	{

		yield return E.Break;
	}
}