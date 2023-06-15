using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class GuiInventoryBar : GuiScript<GuiInventoryBar>
{


	void OnShow()
	{
	}

	void OnPostRestore( int version )
	{
	}

	IEnumerator OnClickBtnRight( IGuiControl control )
	{

		yield return E.Break;
	}
}