using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Reflection;
using PowerTools;

//
// PowerQuest Partial Class: Save/Restore functions
//

namespace PowerTools.Quest
{

public partial class PowerQuest
{
	public static readonly string SAV_SETTINGS = "Settings";
	public static readonly string SAV_SETTINGS_FILE = "Settings.sav";
	public static readonly int SAV_SETTINGS_VER = 0;
	public static readonly int SAV_SETTINGS_VER_REQ = 0;

	#region Variables: Private

	QuestSaveManager m_saveManager = new QuestSaveManager();

	#endregion
	#region Functions: Save/Load public functions
	

	public bool SaveSettings()
	{
		Dictionary<string, object> data = new Dictionary<string, object>();
		data.Add(SAV_SETTINGS,m_settings );
		return m_saveManager.Save(SAV_SETTINGS_FILE, SAV_SETTINGS, SAV_SETTINGS_VER, data);
	}

	// Settings are automatically restored on game start and reset.
	public bool RestoreSettings()
	{
		Dictionary<string, object> data = null;
		int restoredVersion = -1;
		bool result = m_saveManager.RestoreSave(SAV_SETTINGS_FILE, SAV_SETTINGS_VER_REQ, out restoredVersion, out data);		
		if ( result )
		{
			if ( data.ContainsKey(SAV_SETTINGS) )
				m_settings = data[SAV_SETTINGS] as QuestSettings;		
			
			m_settings.OnPostRestore(restoredVersion);			
		}

		return result;
	}

	public List<QuestSaveSlotData> GetSaveSlotData() { return m_saveManager.GetSaveSlotData(); }
	public QuestSaveSlotData GetSaveSlotData( int slot ) { return m_saveManager.GetSaveSlot(slot); }
	public QuestSaveSlotData GetLastSaveSlotData() 
	{ 
		QuestSaveSlotData lastData = null;
		foreach( QuestSaveSlotData data in GetSaveSlotData())
		{
			if ( lastData == null || data.m_timestamp > lastData.m_timestamp )
				lastData = data;
		}
		return lastData;
	}

	public bool Save(int slot, string description, Texture2D imageOverride = null)
	{
		// Check we're not currently saving a game. This could happen if "Save" is called in OnEnter (since that'll be called again when you restore)
		if ( GetRestoringGame() )
			return false;

		// Save settings when regular game is saved whynot
		SaveSettings();

		Dictionary<string, object> data = new Dictionary<string, object>();
		
		foreach( Character value in m_characters )
		{
			data.Add( "Char"+value.GetScriptName(), value );
		}
		foreach( Room value in m_rooms )
		{
			data.Add( "Room"+value.GetScriptName(), value );
		}
		foreach( Gui value in m_guis )
		{
			data.Add( "Gui"+value.GetScriptName(), value );
		}
		foreach( Inventory value in m_inventoryItems )
		{
			data.Add( "Inv"+value.GetScriptName(), value );
		}
		foreach( DialogTree value in m_dialogTrees )
		{
			data.Add( "Dlg"+value.GetScriptName(), value );
		}

		data.Add("Global",m_globalScript );
		data.Add("Camera", m_cameraData );
		data.Add("Cursor", m_cursor );
		data.Add("Audio", SystemAudio.Get.GetSaveData());
		//data.Add("Settings",m_settings );
		data.Add("SV", m_savedVars);
		data.Add("Extra", 
			new ExtraSaveData() 
			{
				//m_paused = Paused,
				m_player = m_player.ScriptName,
				m_currentDialog = m_currentDialog != null ? m_currentDialog.ScriptName : string.Empty,
				m_displayBoxGui = this.m_displayBoxGui,
				m_dialogTreeGui = this.m_dialogTreeGui,
				m_customSpeechGui = this.m_customSpeechGui,
				m_speechStyle = this.m_speechStyle,
				m_speechPortraitLocation = this.m_speechPortraitLocation,
				m_transitionFadeTime = this.m_transitionFadeTime,
			});

		Texture2D image = imageOverride;
		Camera cam = m_cameraData?.Camera;		
		if ( image == null && cam != null && m_saveScreenshotHeight > 0 )
		{
			int imageHeight = m_saveScreenshotHeight;			
			int imageWidth = Mathf.CeilToInt(imageHeight * cam.aspect);

			// Take screenshot for image
			RenderTexture currentRT = RenderTexture.active;				
			RenderTexture.active = new RenderTexture(imageWidth,imageHeight,16, RenderTextureFormat.ARGB32,0);
			
			RenderTexture currCamTex = cam.targetTexture;
			cam.targetTexture = RenderTexture.active;

			Texture2D tex = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
			cam.Render(); 
			image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
			image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
			image.Apply();
			RenderTexture.active = currentRT;
			cam.targetTexture = currCamTex;
			
			/*	Test code to preview screenshot as file /
			var Bytes = image.EncodeToPNG(); 
			System.IO.File.WriteAllBytes(Application.dataPath + "test.png", Bytes);
			/**/
		}

		return m_saveManager.Save(slot, description, m_saveVersion, data, image);
	}

	public bool RestoreLastSave()
	{
		// Restores the most recently saved game, based on timestamp
		QuestSaveSlotData lastData = GetLastSaveSlotData();
		if ( lastData == null )
			return false;
		return RestoreSave(lastData.m_slotId);
	}

	public bool RestoreSave(int slot)
	{
		if ( GetSaveSlotData(slot) == null )
			return false;

		Dictionary<string, object> data = null;
		int restoredVersion = -1;

		// Stop all coroutines
		StopAllCoroutines();
		GetMenuManager().ResetFade(); // Reset fade, so if faded out when restored the game doesn't start faded out. Note that this means you might want to do another 'FadeOutBG(0)' after this function is called, if you want a slower fadein.
		m_consumedInteraction = null;
		m_coroutineMainLoop = null;
		m_backgroundSequence = null;
		m_backgroundSequences.Clear();
		m_currentSequence = null;
		m_currentSequences.Clear();

		bool result = m_saveManager.RestoreSave(slot, m_saveVersionRequired, out restoredVersion, out data);

		object[] onPostRestoreParams = {restoredVersion};

		if ( result )
		{
			m_restoring = true;

			//
			// Load quest objects
			//
			for( int i = 0; i < m_characters.Count; ++i )
			{
				Character value = m_characters[i];
				string name = "Char"+value.GetScriptName();
				if ( data.ContainsKey(name ) )
					m_characters[i] = data[name] as Character;
			}
			m_player = m_characters[0];

			for( int i = 0; i < m_rooms.Count; ++i )
			{
				Room value = m_rooms[i];
				string name = "Room"+value.GetScriptName();
				if ( data.ContainsKey(name ) )
					m_rooms[i] = data[name] as Room;
			}
			for( int i = 0; i < m_guis.Count; ++i )
			{
				Gui value = m_guis[i];
				MonoBehaviour guiInstance = value.Instance;
				string name = "Gui"+value.GetScriptName();
				if ( data.ContainsKey(name) )
				{
					m_guis[i] = data[name] as Gui;
					if ( guiInstance != null )
						m_guis[i].SetInstance(guiInstance as GuiComponent);
				}
			}
			for( int i = 0; i < m_inventoryItems.Count; ++i )
			{
				Inventory value = m_inventoryItems[i];
				string name = "Inv"+value.GetScriptName();
				if ( data.ContainsKey(name) )
					m_inventoryItems[i] = data[name] as Inventory;
			}
			for( int i = 0; i < m_dialogTrees.Count; ++i )
			{
				DialogTree value = m_dialogTrees[i];
				string name = "Dlg"+value.GetScriptName();
				if ( data.ContainsKey(name) )
					m_dialogTrees[i] = data[name] as DialogTree;
			}
			{
				string name = "Global";
				if ( data.ContainsKey(name) )
					m_globalScript = data[name] as GlobalScript;
			}
			{
				string name = "Camera";
				if ( data.ContainsKey(name) )
					m_cameraData = data[name] as QuestCamera;
			}
			{
				string name = "Cursor";
				if ( data.ContainsKey(name) )
					m_cursor = data[name] as QuestCursor;
			}			
			{
				string name = "Audio";
				if ( data.ContainsKey(name) )
					SystemAudio.Get.RestoreSaveData(data[name]);
			}
			// Settings moved to its own save
			/*{
				string name = "Settings";
				if ( data.ContainsKey(name) )
					m_settings = data[name] as QuestSettings;
			}*/
			{
				string name = "SV";
				if ( data.ContainsKey(name) )
					m_savedVars = data[name] as SavedVarCollection;
			}
			{
				string name = "Extra";

				if ( data.ContainsKey(name) )
				{
					ExtraSaveData extraSaveData = data[name] as ExtraSaveData;
					SetPlayer( GetCharacter(extraSaveData.m_player) );
					m_currentDialog = GetDialogTree(extraSaveData.m_currentDialog);
					
					m_displayBoxGui = extraSaveData.m_displayBoxGui;
					m_dialogTreeGui = extraSaveData.m_dialogTreeGui;
					m_customSpeechGui = extraSaveData.m_customSpeechGui;
					m_speechStyle = extraSaveData.m_speechStyle;
					m_speechPortraitLocation = extraSaveData.m_speechPortraitLocation;
					m_transitionFadeTime = extraSaveData.m_transitionFadeTime;
					// Paused = extraSaveData.m_paused;
				}
			}

			//
			// Swap back in hotloaded scripts if running in editor.
			//
			if ( Application.isEditor )
			{
				if ( m_hotLoadAssembly != null )
				{
					List<IQuestScriptable> scriptables = GetAllScriptables();
					foreach ( IQuestScriptable scriptable in scriptables )
					{
						scriptable.HotLoadScript(m_hotLoadAssembly);
					}
				}
			}

			//
			// Call post-restore functions
			//

			
			// Call post restore on quest objects
			{
				for( int i = 0; i < m_characters.Count; ++i )
					m_characters[i].OnPostRestore(restoredVersion, m_characterPrefabs[i].gameObject);

				for( int i = 0; i < m_rooms.Count; ++i )
					m_rooms[i].OnPostRestore(restoredVersion, m_roomPrefabs[i].gameObject);

				for( int i = 0; i < m_guis.Count; ++i )
					m_guis[i].OnPostRestore(restoredVersion, m_guiPrefabs[i].gameObject);

				for( int i = 0; i < m_inventoryItems.Count; ++i )
					m_inventoryItems[i].OnPostRestore(restoredVersion, m_inventoryPrefabs[i].gameObject);

				for( int i = 0; i < m_dialogTrees.Count; ++i )
					m_dialogTrees[i].OnPostRestore(restoredVersion, m_dialogTreePrefabs[i].gameObject);

				QuestCameraComponent cameraInstance = GameObject.FindObjectOfType<QuestCameraComponent>();
				if ( cameraInstance != null )
					m_cameraData.SetInstance(cameraInstance);

				m_cursor.OnPostRestore(restoredVersion, m_cursorPrefab.gameObject);				

				// Settings moved
				//m_settings.OnPostRestore(restoredVersion/*, m_cursorPrefab.gameObject*/);

				// update region collision to initial state
				{
					UpdateRegions();
					if ( m_currentRoom != null )
						m_currentRoom.GetInstance().GetRegionComponents().ForEach( item=>item.OnRoomLoaded() );
				}
			}

			// Call post restore on game scripts
			{
				List<IQuestScriptable> scriptables = GetAllScriptables();
				scriptables.ForEach( item => CallScriptPostRestore(item, onPostRestoreParams) );
			}

			// Call post restore on SaveManager (which calls it to custom save data)
			m_saveManager.OnPostRestore();

			// unblock again
			Unblock();
			
			// Need to load the scene the player's in, and start main loop
			StartRoomTransition((Room)GetPlayer().Room, true);
		}
		Unblock();

		return result;
	}

	public bool DeleteSave(int slot)
	{
		return m_saveManager.DeleteSave(slot);
	}

	/// Advanced save/restore function: For saving data not in a QuestScript...
	/**
	To use, call AddSaveData in Start, passing in a name for what you want to save, and the object containing the data you want to save.
	 
	If you want to do things when the game is restored, pass the function you want ot be called as OnPostRestore
	
	Notes:
	- The object to be saved must be a class containing the data to be saved (can't just pass in a value type like an int, float or Vector2).
	- By default all data in a class is saved, except for:
		- GameObjects, and MonoBehaviours
		- Variables with the [QuestDontSave] attribute (NOTE: THIS IS NOT YET IMPLEMENTED, BUG DAVE IF NEEDED!)
		- If you store references to other things that shouldnt be saved in your scripts, that may cause problems. Best thing is to dave know, he can add a feature tohelp with that
	-  you can add the [QuestSave] attribute to the class
		- When you do that, ONLY the variables that also have the [QuestSave] attribute are saved.
		- You can put this tag on a Monobehaviour class, when you just want to save a few of its variables without having to put them in their own seperate class		

	**Examples saving a simple data class:**
	~~~
	class MyComponent : Monobehaviour
	{
		// Class to store the save data in
	 	class SaveData
	 	{
			public int myInt;
			public float myFloat;
			[QuestDontSave] public bool myNotSavedBool;
	 	}
	 
		SaveData m_saveData;

		void Start()
		{
			PowerQuest.Get.AddSaveData( "myData", m_saveData );
		}
		void OnDestroy()
		{
			Powerquest.Get.RemoveSaveData("myData");
		}
	}
	~~~

	**Example using the [QuestSave] attribute:**
	~~~
	[QuestSave]
	class MyComponent : Monobehaviour
	{
		[QuestSave] int myInt;
		[QuestSave] float myFloat;
		bool myNotSavedBool;
	 
		SaveData m_saveData;

		void Start()
		{
			PowerQuest.Get.AddSaveData( "myData", this );
		}
		void OnDestroy()
		{
			Powerquest.Get.RemoveSaveData("myData");
		}
	}
	~~~
	*/
	public void AddSaveData(string name, object data, System.Action OnPostRestore = null ) { m_saveManager.AddSaveData(name,data,OnPostRestore); }

	/// Advanced save/restore function: For aving data not in a QuestScript. Call this when you've called AddSaveData, but no longer want to save that data.
	public void RemoveSaveData(string name) { m_saveManager.RemoveSaveData(name); }

	#endregion
	#region Save/Load helpers

	void CallScriptPostRestore(IQuestScriptable scriptable, object[] onPostRestoreParams )
	{
		if ( scriptable.GetScript() != null )
		{
			MethodInfo method = scriptable.GetScript().GetType().GetMethod( "OnPostRestore", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			if ( method != null ) method.Invoke(scriptable.GetScript(), onPostRestoreParams);					
		}
	}
	#endregion

}

}
