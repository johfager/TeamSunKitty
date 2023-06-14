using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerTools.Quest;

namespace PowerScript
{	
	// Shortcut access to SystemAudio.Get
	public class Audio : SystemAudio
	{
	}

	public static partial class C
	{
		// Access to specific characters (Auto-generated)
		public static ICharacter Dave           { get { return PowerQuest.Get.GetCharacter("Dave"); } }
		public static ICharacter Barney         { get { return PowerQuest.Get.GetCharacter("Barney"); } }
		public static ICharacter MainChar       { get { return PowerQuest.Get.GetCharacter("MainChar"); } }
		public static ICharacter Kettil         { get { return PowerQuest.Get.GetCharacter("Kettil"); } }
		// #CHARS# - Do not edit this line, it's used by the system to insert characters
	}

	public static partial class I
	{		
		// Access to specific Inventory (Auto-generated)
		public static IInventory Bucket         { get { return PowerQuest.Get.GetInventory("Bucket"); } }
		public static IInventory Key            { get { return PowerQuest.Get.GetInventory("Key"); } }

		public static IInventory DoorKnob       { get { return PowerQuest.Get.GetInventory("DoorKnob"); } }

		public static IInventory Matches        { get { return PowerQuest.Get.GetInventory("Matches"); } }
		
		// #INVENTORY# - Do not edit this line, it's used by the system to insert rooms for easy access
	}

	public static partial class G
	{
		// Access to specific gui (Auto-generated)
		public static IGui DialogTree     { get { return PowerQuest.Get.GetGui("DialogTree"); } }
		public static IGui SpeechBox      { get { return PowerQuest.Get.GetGui("SpeechBox"); } }
		public static IGui HoverText  { get { return PowerQuest.Get.GetGui("HoverText"); } }
		public static IGui DisplayBox    { get { return PowerQuest.Get.GetGui("DisplayBox"); } }
		public static IGui Prompt         { get { return PowerQuest.Get.GetGui("Prompt"); } }
		public static IGui Toolbar          { get { return PowerQuest.Get.GetGui("Toolbar"); } }
		public static IGui InventoryBar   { get { return PowerQuest.Get.GetGui("InventoryBar"); } }
		public static IGui Options        { get { return PowerQuest.Get.GetGui("Options"); } }
		public static IGui Save           { get { return PowerQuest.Get.GetGui("Save"); } }
		public static IGui Creatorbox     { get { return PowerQuest.Get.GetGui("Creatorbox"); } }
		// #GUI# - Do not edit this line, it's used by the system to insert rooms for easy access
	}

	public static partial class R
	{
		// Access to specific room (Auto-generated)
		public static IRoom Title          { get { return PowerQuest.Get.GetRoom("Title"); } }
		public static IRoom Forest         { get { return PowerQuest.Get.GetRoom("Forest"); } }
		public static IRoom Bedroom        { get { return PowerQuest.Get.GetRoom("Bedroom"); } }
		public static IRoom LivingRoom     { get { return PowerQuest.Get.GetRoom("LivingRoom"); } }
		public static IRoom FrontYard      { get { return PowerQuest.Get.GetRoom("FrontYard"); } }
		public static IRoom PipesPuzzle    { get { return PowerQuest.Get.GetRoom("PipesPuzzle"); } }
		public static IRoom BedroomPuzzle  { get { return PowerQuest.Get.GetRoom("BedroomPuzzle"); } }
		public static IRoom StateExperimentRoom { get { return PowerQuest.Get.GetRoom("StateExperimentRoom"); } }
		public static IRoom LitLivingRoom  { get { return PowerQuest.Get.GetRoom("LitLivingRoom"); } }
		public static IRoom StateStartRoom { get { return PowerQuest.Get.GetRoom("StateStartRoom"); } }
		public static IRoom LitKitchen     { get { return PowerQuest.Get.GetRoom("LitKitchen"); } }
		public static IRoom EndScene       { get { return PowerQuest.Get.GetRoom("EndScene"); } }
		public static IRoom KitchenNewspaper { get { return PowerQuest.Get.GetRoom("KitchenNewspaper"); } }
		public static IRoom CreditsScreen  { get { return PowerQuest.Get.GetRoom("CreditsScreen"); } }
		public static IRoom UnlitKitchen   { get { return PowerQuest.Get.GetRoom("UnlitKitchen"); } }
		// #ROOM# - Do not edit this line, it's used by the system to insert rooms for easy access
	}

	// Dialog
	public static partial class D
	{
		// Access to specific dialog trees (Auto-generated)
		public static IDialogTree ChatWithBarney       { get { return PowerQuest.Get.GetDialogTree("ChatWithBarney"); } }
		// #DIALOG# - Do not edit this line, it's used by the system to insert rooms for easy access	    	    
	}


}
