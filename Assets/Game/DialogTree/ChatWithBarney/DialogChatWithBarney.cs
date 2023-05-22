using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class DialogChatWithBarney : DialogTreeScript<DialogChatWithBarney>
{
	public IEnumerator OnStart()
	{
		yield return C.WalkToClicked();
		C.FaceClicked();
		C.Barney.Face(C.Dave);
		yield return C.Barney.Say("What is it?");
		yield return E.Break;
		
	}

	public IEnumerator Option1( IDialogOption option )
	{
		yield return C.Dave.Say("Rad cave");
		yield return E.WaitSkip();
		yield return C.Barney.FaceUp();
		yield return E.WaitSkip();
		yield return C.Barney.Say("Uh-huh");
		yield return E.WaitSkip();
		yield return C.Barney.Face(C.Dave);
		yield return E.Break;
	}


	public IEnumerator Option2( IDialogOption option )
	{
		yield return C.Dave.Say("What's with this forest anyway?");
		yield return C.Barney.Say("Whaddaya mean?");
		
		// Here we start a seperate branch of dialog (about forests):
		// Turn off the main options
		OptionOff(1,2,3);
		OptionOff("bye");
		// Turn on the 'forest' branch options
		OptionOn("tree","leaf","forestdone");
		
		yield return E.Break;
		
	}


	public IEnumerator Option3( IDialogOption option )
	{
		yield return C.Dave.Say("Nice well there, huh?");
		yield return C.Barney.Say("No. I hate it. Lets never speak of it again");
		yield return C.Dave.Say("Alright");
		option.OffForever();
		yield return E.Break;
		
	}



	public IEnumerator OptionForestDone( IDialogOption option )
	{
		// Here we're returning from the 'forest' dialog branch to the main dialog
		
		// Turn off the 'forest' options
		OptionOff("tree","leaf","forestdone");
		
		// Turn on the main options. If they've had 'OptionOffForever' they wont turn on again.
		OptionOn(1,2,3);
		OptionOn("bye");
		
		// Only set the 'Forest' option as used (which changes the color) if all its child options have been used.
		Option(2).Used = Option("tree").Used && Option("leaf").Used;
		yield return E.Break;
		
	}


	public IEnumerator OptionTree( IDialogOption option )
	{
		yield return C.Dave.Say("The trees are cool");
		yield return C.Barney.Say(" I guess");
		yield return E.Break;
		
	}


	public IEnumerator OptionLeaf( IDialogOption option )
	{
		yield return C.Dave.Say("I like the foliage");
		yield return C.Barney.Say("Yes. It is pleasant foliage");
		yield return E.Break;
		
	}

	public IEnumerator OptionBye( IDialogOption option )
	{
		yield return C.Dave.Say("Later!");
		yield return E.WaitSkip();
		yield return C.Barney.FaceRight();
		yield return E.WaitSkip();
		yield return C.Barney.Say("Whatever");
		
		// Don't mark the 'end' option as used
		option.Used = false;
		
		// stop the dialog
		Stop();
	}
}
