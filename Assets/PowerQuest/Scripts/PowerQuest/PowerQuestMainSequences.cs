using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Reflection;
using PowerTools;


//
// PowerQuest Partial Class: Main loop coroutine
//

namespace PowerTools.Quest
{

public partial class PowerQuest
{

	#region Coroutine: Load Room

	IEnumerator LoadRoomSequence( string sceneName )
	{
		/* Order Here:
			- Create Room
			- Set m_currentRoom
			- Create/Setup Camera, Guis, Cursor
			- Spawn and position characters
			- m_initialised = true
			- Room instance OnLoadComplete
			- Debug Play-From function
			- Block
			- Wait a frame
			- OnEnterRoom
			- OnEnterRoomAfterFade (initial call)
			- Update regions
			- Camera onEnterRoom
			- start fading in
			- m_transitioning = false (allows Update to be called again)
			- Yield to OnEnterRoomAfterFade
			- Update starts being called
			- Unblock
			- m_roomLoopStarted = true;
			- Start MainLoop()
		*/
		bool firstRoomLoad = (m_initialised == false);

		// Set up flag for save/restore during "OnEnter" - Moved to onTransition
		//if ( m_restoring == false )
		//	SV.m_savedInOnEnter = true;

		//
		// Get the camera and canvas
		//		
		//GameObject guiCamObj = GameObject.Find("QuestGuiCamera");
		Camera[] cameras = GameObject.FindObjectsOfType<Camera>(false);

		// Destroy duplicate gui camera if		
		for ( int i = 0; i < cameras.Length; ++i )
		{
			Camera cam = cameras[i];
			if ( cam.gameObject.name == "QuestGuiCamera" )
			{
				if ( m_cameraGui == null )
				{
					// Set up gui camera (first time)
					m_cameraGui = cam;
					// Get the canvas
					m_canvas = m_cameraGui.GetComponentInChildren<Canvas>();
					DontDestroyOnLoad(m_cameraGui.gameObject);
				}	
				else if ( cam != m_cameraGui )
				{
					// Destroy duplicate gui camera in scene
					DestroyImmediate(cam.gameObject);
				}
			}					
		}
			
		Debug.Assert(m_cameraGui != null, "Faled to load room- Couldn't find QuestGuiCamera in the scene");
		

		//
		// Set up room object
		//
		RoomComponent roomInstance = GameObject.FindObjectOfType<RoomComponent>();
		Debug.Assert(roomInstance != null, "Failed to find room instance in scene");
		string roomName = roomInstance.GetData().ScriptName;
		// Find the room's data
		Room room = m_rooms.Find( item=> string.Equals(item.ScriptName, roomName, System.StringComparison.OrdinalIgnoreCase) );
		Debug.Assert(room != null, "Failed to load room '"+roomName+"'");
		m_currentRoom = room;
		room.SetInstance(roomInstance);

		//
		// Set up camera object (after room so room is set up right)
		//
		QuestCameraComponent cameraInstance = GameObject.FindObjectOfType<QuestCameraComponent>();
		Debug.Assert(cameraInstance != null);
		m_cameraData.SetInstance(cameraInstance);
		m_cameraData.SetCharacterToFollow(GetPlayer());

		//
		// Setup GUIs (First time only)
		//
		foreach ( Gui gui in m_guis ) 
		{
			// Dont' recreate guis- NB: we're not checking for duplicates inside a room, which we should do...
			if ( gui.Instance != null )
				continue;

			GameObject guiInstance = GameObject.Find(gui.GetPrefab().name);			
			if ( guiInstance == null )
			{
				guiInstance = GameObject.Instantiate(gui.GetPrefab() ) as GameObject;
			}
			gui.SetInstance(guiInstance.GetComponent<GuiComponent>());
			guiInstance.SetActive( gui.Visible && gui.VisibleInCutscenes );
			if ( guiInstance.GetComponent<RectTransform>() )
			{
				if ( m_canvas != null )
					guiInstance.transform.SetParent(m_canvas.transform, false);
			}
			else if( m_cameraGui != null )
			{
				guiInstance.transform.SetParent(m_cameraGui.transform, false);
				guiInstance.transform.position = guiInstance.transform.position.WithZ(0);
			}
			//DontDestroyOnLoad(guiInstance);
		}

		// 
		// Set up cursor object
		//
		if( m_cursorPrefab != null )
		{
			GameObject cursorObj = GameObject.Instantiate(m_cursor.GetPrefab()) as GameObject;
			QuestCursorComponent cursorInstance = cursorObj.GetComponent<QuestCursorComponent>();
			m_cursor.SetInstance(cursorInstance);
		}

		//
		// Spawn and position characters
		//

		// For now force player to be in the current room. maybe this could be handled nicer though.
		m_player.Room = GetRoom(roomName);

		// Spawn characters that are supposed to be in this room, and remove those that ain't
		foreach ( Character character in m_characters ) 
		{									
			if ( character.Room == GetRoom(roomName) )
			{
				// Spawn the character
				character.SpawnInstance();
			}
			else
			{
				GameObject characterInstance = GameObject.Find(character.GetPrefab().name);
				if ( characterInstance != null )
				{
					// Character's not supposed to be here, so remove them
					characterInstance.name = "deleted"; // so it doesn't turn up in searches in same frame
					GameObject.Destroy(characterInstance);
				}
			}
		}
		
		// Now rooms and characters, etc are set up, mark as initialised (this is done once per application load)
		m_initialised = true;

		if ( room.GetInstance() != null ) room.GetInstance().OnLoadComplete();

		if ( m_restoring && SV.m_callEnterOnRestore == false )
		{
			// When restoring a game, the scene is reloaded, but we don't call onEnterRoom, unless we saved FROM onEnterRoom
			FadeInBG(TransitionFadeTime/2.0f, "ENTER");

			yield return new WaitForSeconds(0.05f);// Wait(0.05f); Game might start paused- dont' do this.
			m_restoring = false;
			SV.m_callEnterOnRestore = false;
			m_transitioning = false;
		}
		else 
		{
			//
			// Check for and call Debug Startup Function.
			//
			bool debugSkipEnter = false;
			if ( firstRoomLoad && Debug.isDebugBuild && room != null && room.GetScript() != null )
			{
				// If 'has restarted' then use the restartPlayFrom function, not the one set in the editor. This is used when the E.Restart function is called with a PlayFrom function
				string restartFunction = null;
				if ( s_hasRestarted )
					restartFunction = s_restartPlayFromFunction;
				else 
					restartFunction = room.GetInstance().m_debugStartFunction;
				if ( string.IsNullOrEmpty(restartFunction ) == false )
				{
					System.Reflection.MethodInfo debugMethod = room.GetScript().GetType().GetMethod( restartFunction, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
					if ( debugMethod != null ) 
					{
						var result = debugMethod.Invoke(room.GetScript(),null);
						if ( result != null && result.Equals(true) )
							debugSkipEnter = true;
					}
				}
			}
			

			//
			// On enter room
			//
			Block();

			// Remove any camera override set in the previous room
			m_cameraData.ResetPositionOverride();

			// Necessary, incase things haven't completely loaded yet (first time game loads)
			yield return new WaitForEndOfFrame(); 

			System.Reflection.MethodInfo method = null;
			if ( m_globalScript != null )
			{
				method = m_globalScript.GetType().GetMethod( "OnEnterRoom", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
				if ( method != null ) method.Invoke(m_globalScript,null);
			}

			if ( room != null && room.GetScript() != null && debugSkipEnter == false )
			{
				method = room.GetScript().GetType().GetMethod( "OnEnterRoom", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
				if ( method != null ) method.Invoke(room.GetScript(),null);
			}	
			SV.m_callEnterOnRestore = false;
			
			// Start actual fade in
			FadeInBG(TransitionFadeTime/2.0f, "ENTER");
			
			// Now call OnEnterRoomAfterFade functions, but without yielding yet. That way, plrs can be positioned here and camera will still snap to correct position.
			Coroutine onEnter = StartScriptInteractionCoroutine(GetScript(), "OnEnterRoomAfterFade");
			Coroutine onEnterRoom = null;
			if ( room != null && room.GetScript() != null && debugSkipEnter == false )
				onEnterRoom = StartScriptInteractionCoroutine(room.GetScript(), "OnEnterRoomAfterFade");			

			// update region collision to initial state (So don't get "OnEnter" on first frame of room)
			{
				UpdateRegions();
				room.GetInstance().GetRegionComponents().ForEach( item=>item.OnRoomLoaded() );
			}

			// Moved Camera's OnEnterRoom to after OnEnterRoomAfterFade is called (but before yielding), incase you set plr pos in that.
			m_cameraData.GetInstance().OnEnterRoom();

			//
			//	Room transition - yield until complete 
			//

			// Start actual fade in - Moved 2021/26/11
			//FadeInBG(TransitionFadeTime/2.0f, "ENTER");			

			m_transitioning = false;
			m_restoring = false;

			//
			// on enter room after fadein
			//
			/* 5/1/2021 - moved onEnterRoomAfteFade functions from here to before region/camera stuff */
			SetAutoLoadScript( this, "OnEnterRoomAfterFade", onEnter != null, false );
			if ( onEnter != null )
				yield return onEnter;

			if ( room != null && room.GetScript() != null )
			{
				//onEnterRoom = StartScriptInteractionCoroutine(room.GetScript(), "OnEnterRoomAfterFade"); // moved above 
				SetAutoLoadScript( room, "OnEnterRoomAfterFade", onEnterRoom != null, false );
				if ( onEnterRoom != null )
					yield return onEnterRoom;
			}
			Unblock();
		}

		//
		// Main loop
		//
		m_roomLoopStarted = true;

		// There might be a previous current sequenced running, stopping the previous main loop from stopping. In which case we don't want to start the new loop yet.
		Block();
		yield return WaitUntil( ()=>m_currentSequence == null );
		Unblock();

		m_coroutineMainLoop = StartCoroutine( MainLoop() );
	}

	#endregion
	#region Coroutine: Main loop


	IEnumerator MainLoop()
	{
		while ( true )
		{
			//
			// Main Update
			//

			Block();
			bool yielded = false;

			if ( SystemTime.Paused == false)
			{

				//
				// Finish Current Sequence
				//
				if ( m_currentSequence != null )
				{
					yield return CoroutineWaitForCurrentSequence();
				}

				ExOnMainLoop();
				ExtentionOnMainLoop();

				//
				// Run through any queued interactions. 
				// These happen when a non-yielding function like StopDialog() is called and results in a yielding function like IEnumerator OnStopDialog() being started
				//
				while ( m_queuedScriptInteractions.Count > 0 )
				{
					m_currentSequence = m_queuedScriptInteractions[0];
					m_queuedScriptInteractions.RemoveAt(0);

					if ( m_currentSequence != null )
					{
						yielded = true;
						yield return CoroutineWaitForCurrentSequence();
					}
				}
				m_queuedScriptInteractions.Clear();

				//
				// Mouse triggered sequences
				//

				// Handle left/right click seperately here because the main loop may take more than 1 normal frame, so getmouseButtonDown won't cut it
				bool leftClick = m_leftClickPrev == false && Input.GetMouseButton(0);
				bool rightClick = m_rightClickPrev == false && Input.GetMouseButton(1);
				m_leftClickPrev = Input.GetMouseButton(0);
				m_rightClickPrev = Input.GetMouseButton(1);

				bool clickHandled = false;

				if ( GetModalGuiActive() )
					clickHandled = true;

				if ( SV.m_captureInputSources.Count > 0 )
					clickHandled = true;

				//
				// Handle holding down click to walk
				//
				if ( m_walkClickDown )
				{
					// Was holding click-to-walk
					if ( Input.GetMouseButton(0) == false || clickHandled )
					{
						m_walkClickDown = false;
					}
					else if ( (m_player.Position - m_mousePos).magnitude > 10 )
					{
						// Holding left click- keep walking			
						m_player.WalkToBG(m_mousePos);
					}
					clickHandled = true;
				}

				if ( clickHandled == false && (leftClick || rightClick) )
				{
					System.Reflection.MethodInfo method = null;
					if ( m_globalScript != null )
					{
						method = m_globalScript.GetType().GetMethod( "OnMouseClick", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
						if ( method != null ) 
							method.Invoke(m_globalScript,new object[]{ leftClick,rightClick });
						else 
							OnMouseClick(leftClick, rightClick);
					}
				}

				//
				// Run through any queued interactions. 
				// These would have been added in "OnMouseClick" from ProcessClick calls. This could optionally be added after "update" I guess, incase wanna do controls in update instead.
				//
				while ( m_queuedScriptInteractions.Count > 0 )
				{
					m_currentSequence = m_queuedScriptInteractions[0];
					m_queuedScriptInteractions.RemoveAt(0);

					if ( m_currentSequence != null )
					{
						yielded = true;
						yield return CoroutineWaitForCurrentSequence();
					}
				}
				m_queuedScriptInteractions.Clear();

				//
				// Game Update Blocking
				//
				if ( StartScriptInteraction( this, "UpdateBlocking",null,false,true ) )
				{
					yielded = true;
					yield return CoroutineWaitForCurrentSequence();
				}

				//
				// Room Update Blocking
				//
				if ( m_currentRoom != null)
				{
					if ( StartScriptInteraction(m_currentRoom, "UpdateBlocking",null,false,true ) )
					{
						yielded = true;
						yield return CoroutineWaitForCurrentSequence();
					}
				}


				//
				// Run through any queued interactions, in case there were any ProcessClick calls in the update
				//
				while ( m_queuedScriptInteractions.Count > 0 )
				{
					m_currentSequence = m_queuedScriptInteractions[0];
					m_queuedScriptInteractions.RemoveAt(0);

					if ( m_currentSequence != null )
					{
						yielded = true;
						yield return CoroutineWaitForCurrentSequence();
					}
				}
				m_queuedScriptInteractions.Clear();

				//
				// Region enter/exit blocking
				//

				if ( m_currentRoom != null)
				{
					// not using collision system, just looping over characters in the room and triggers in the room
					List<RegionComponent> regionComponents = m_currentRoom.GetInstance().GetRegionComponents();
					int regionCount = regionComponents.Count;
					RegionComponent region = null;
					for ( int charId = 0; charId < m_characters.Count; ++charId )
					{
						Character character = m_characters[charId];

						for ( int regionId = 0; regionId < regionCount; ++regionId )
						{					
							region = regionComponents[regionId];
							RegionComponent.eTriggerResult result = region.UpdateCharacterOnRegionState(charId);
							if ( region.GetData().Enabled )
							{
								if ( result == RegionComponent.eTriggerResult.Enter )
								{
									if ( StartScriptInteraction( m_currentRoom, SCRIPT_FUNCTION_ENTER_REGION+region.GetData().ScriptName, new object[] {region.GetData(), character}, false,true ) )
									{
										yielded = true;
										yield return CoroutineWaitForCurrentSequence();
									}
								} 
								else if ( result == RegionComponent.eTriggerResult.Exit )
								{
									if ( StartScriptInteraction( m_currentRoom, SCRIPT_FUNCTION_EXIT_REGION+region.GetData().ScriptName, new object[] {region.GetData(), character}, false,true ) )
									{
										yielded = true;
										yield return CoroutineWaitForCurrentSequence();
									}
								}
							}
						}
					}

				}

			}


			//
			// end of the main loop sequences
			//

			Unblock();

			// Reset gui click flag
			m_guiConsumedClick = false;

			// if cutscene is being skipped, it's now finished, so reset it
			if ( m_skipCutscene == true )
			{
				OnEndCutscene();
			}

			if ( yielded == false && m_currentDialog != null )
			{
				// Show dialog gui again if one active
				GetGui(DialogTreeGui).Visible = true;
			}

			// Yield until the next frame
			if ( yielded == false )
				yield return new WaitForEndOfFrame();
		}			
	}

	// Seperate coroutine to wait for the current sequence so that even if MainLoop is Stopped, the m-currentSequence is set null when it completes.
	IEnumerator CoroutineWaitForCurrentSequence()
	{
		yield return m_currentSequence;
		m_currentSequence = null;
	}

	#endregion
	#region Main sequence helpers

	// NB: This is the fallback, only used if GlobalScript doesn't have an OnMouseClick
	void OnMouseClick( bool leftClick, bool rightClick )
	{
		// Clear inventory on Right click, or left click on empty space
		if ( m_player.HasActiveInventory && ( rightClick || (GetMouseOverClickable() == null && leftClick ) || Cursor.NoneCursorActive ) )
		{
			SystemAudio.Play("InventoryCursorClear");						
			m_player.ActiveInventory = null;
			return;
		}

		// Don't do anything if clicking on something with "none" cursor
		if ( m_cursor.NoneCursorActive )
			return;

		if ( leftClick )
		{
			// Handle left click
			if ( GetMouseOverClickable() != null )
			{	
				// Left click something
				if ( m_player.HasActiveInventory && m_cursor.InventoryCursorOverridden == false )
				{
					ProcessClick( eQuestVerb.Inventory );
				}
				else
				{
					ProcessClick(eQuestVerb.Use);
				}
			}
			else 
			{
				// Left click empty space
				ProcessClick( eQuestVerb.Walk );
			}
		}
		else if ( rightClick && GetActionEnabled(eQuestVerb.Look) )
		{
			// Handle right click
			if ( GetMouseOverClickable() != null )
			{
				ProcessClick( eQuestVerb.Look );
			}
		}		
	}
	#endregion

}

}
