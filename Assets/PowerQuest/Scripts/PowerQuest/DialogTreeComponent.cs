using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PowerScript;
using PowerTools;

namespace PowerTools.Quest
{

#region Class: DialogOption

[System.Serializable]
public partial class DialogOption : IDialogOption, IQuestClickable
{	
	[SerializeField] string m_name = string.Empty;
	[SerializeField,Multiline] string m_text = string.Empty;
	[SerializeField] bool m_visible = true;
	[SerializeField,HideInInspector] bool m_disabled = false;
	[SerializeField,HideInInspector] bool m_used = false;
	[SerializeField,HideInInspector] int m_timesUsed = 0;
	int m_inlineId = -1;
	

	public string Name { get { return m_name;} set { m_name = value; } }
	public string Text { get { return m_text;} set { m_text = value; } }
	/// The description here is identical to Text, but the localization code will replace lines where x.Description = "blah" whereas it won't for x.Text = "blah"
	public string Description { get { return m_text;} set { m_text = value; } } 
	public bool Visible { get { return m_visible;} set { m_visible = value; } }
	public bool Disabled {get { return m_disabled;} set { m_disabled = value; } }
	public bool Used {get => m_used; set => m_used = value; }
	public int TimesUsed {get => m_timesUsed; set => m_timesUsed = value; }

	public void Show() { if ( Disabled == false) Visible = true; }
	public void Hide() { Visible = false; }
	public void HideForever() { Visible = false; Disabled = true; }

	public void On() { if ( Disabled == false) Visible = true; }
	public void Off() { Visible = false; }
	public void OffForever() { Visible = false; Disabled = true; }

	// Inline id is used for E.StartInlineDialog();
	public int InlineId { get { return m_inlineId; } set { m_inlineId = value; } }

	// Implementing IQuestClickable just so can get the description automatially... probably not really worth doing that ha ha.
	public eQuestClickableType ClickableType { get { return eQuestClickableType.Gui; } }
	public MonoBehaviour Instance { get { return null; } }
	public string ScriptName { get { return m_name; } }
	public Vector2 Position { get{ return Vector2.zero; } }
	public Vector2 WalkToPoint { get; set; }
	public Vector2 LookAtPoint { get; set; }
	public float Baseline { get; set; }
	public bool Clickable { get; set; }
	public string Cursor { get { return null; } set{} }
	public void OnInteraction( eQuestVerb verb ){}
	public void OnCancelInteraction( eQuestVerb verb ){}
	public QuestScript GetScript() { return null; }
	public IQuestScriptable GetScriptable() { return null; }
	
	

}

#endregion

#region Class: DialogTreeScript

// Script class to inherit from for gui scripts
[System.Serializable]
public class DialogTreeScript<T> : QuestScript where T : QuestScript
{

	
	protected IDialogTree m_data = null;
	
	/// Get the DialogTree associated with this script. 
	/// eg. `Data.Hide()` 
	/// \sa PowerTools.Quest.IDialogTree
	//public IDialogTree Data { get{return m_data;} }
		
	/// True the first time the dialog tree is shown (or if its never been shown). 
	public bool FirstTimeShown { get => D.Current.FirstTimeShown; }
	/// The number of times the dialog tree has been shown
	public int TimesShown { get => D.Current.TimesShown; }

	/// Access to option with specified id. Eg: `Option(3).On();`. This example in the QuestScript editor is simplified to `O.3.On();`
	public DialogOption Option(int id) { return m_data.GetOption(id); }
	/// Access to option with specified id. Eg: `Option("NiceHat").On();` This example in the QuestScript editor is simplified to `O.NiceHat.On();`
	public DialogOption Option(string id) 
	{
		if ( m_data == null ) Debug.LogError("Data not set up yet in Dialog. Can't retrieve option"); 
		return m_data.GetOption(id); 
	}

	/// Turns on one or more options. Eg: `D.ChatWithBarney.OptionOn(1,2,3);` \sa OptionOff() \sa OptionOffForever
	public void OptionOn(params int[] id){ m_data.OptionOn(id);  }
	/// Turns off one or more options. Eg: `D.ChatWithBarney.OptionOff(1,2,3);` \sa OptionOn() \sa OptionOffForever
	public void OptionOff(params int[] id){ m_data.OptionOff(id); }
	/// Turns one or more options off permanantly. Future OptionOn calls will be ignored. Eg: `D.ChatWithBarney.OptionOffForever(1,2,3);` \sa OptionOn() \sa OptionOff
	public void OptionOffForever(params int[] id){ m_data.OptionOffForever(id); }

	/// Turns on one or more options. Eg: `D.ChatWithBarney.OptionOn("Yes","No","Maybe");` \sa OptionOff() \sa OptionOffForever()
	public void OptionOn(params string[] id){ m_data.OptionOn(id);  }
	/// Turns off one or more options. Eg: `D.ChatWithBarney.OptionOff("Yes","No","Maybe");` \sa OptionOn() \sa OptionOffForever()
	public void OptionOff(params string[] id){ m_data.OptionOff(id); }
	/// Turns one or more options off permanantly. Future OptionOn calls will be ignored. Eg: `D.ChatWithBarney.OptionOffForever("Yes","No","Maybe");` \sa OptionOn() \sa OptionOff()
	public void OptionOffForever(params string[] id){ m_data.OptionOffForever(id); }

	/// Changes the active dialog tree to the new one specified. Useful for switching to a sub-tree. Eg: `Goto(D.AskAboutQuests);`. \sa GotoPrevious()
	public void Goto(IDialogTree dialog) { if ( dialog != null ) dialog.Start(); }

	/// Changes the active dialog tree back to the previous one. Useful for returning from a sub-tree. \sa Goto() \sa Stop()
	public void GotoPrevious() { if ( PowerQuest.Get.GetPreviousDialog() != null ) PowerQuest.Get.GetPreviousDialog().Start(); }

	/// Stops the active dialog tree and returns the player to the game
	public void Stop() { PowerQuest.Get.StopDialog(); }
	
	/// Used internally to access one script from another
	public static T Script { get { return E.GetScript<T>(); } }
}

#endregion
#region Class: DialogTree

//
// Prop Data and functions. Persistant between scenes, as opposed to GuiComponent which lives on a GameObject in a scene.
//
[System.Serializable] 
public partial class DialogTree : IQuestScriptable, IDialogTree
{
	//
	// Default values set in inspector
	//
	[SerializeField] List<DialogOption> m_options = new List<DialogOption>();
	[ReadOnly][SerializeField] string m_scriptName = "DialogNew";
	[ReadOnly][SerializeField] string m_scriptClass = "DialogNew";

	//
	// Private variables
	//
	int m_timesShown = 0;
	QuestScript m_script = null;
	GameObject m_prefab = null;
	DialogTreeComponent m_instance = null;

	//
	//  Properties
	//
	public string ScriptName { get{ return m_scriptName;} }
	public DialogTree Data { get{ return this; } }
	public List<DialogOption> Options { get { return m_options; } }
	public int NumOptionsEnabled { get {
		int result = 0;
		m_options.ForEach(item=> {if (item.Visible) ++result;} );
		return result;
	} }
	public int NumOptionsUnused { get {
		int result = 0;
		m_options.ForEach(item=> {if (item.Used == false) ++result;} );
		return result;
	} }
	
	public bool FirstTimeShown {get=> m_timesShown <= 1; }
	public int TimesShown {get => m_timesShown;}

	//
	// Getters/Setters - These are used by the engine. Scripts mainly use the properties
	//
	public QuestScript GetScript() { return m_script; }
	public IQuestScriptable GetScriptable() { return this; }
	public T GetScript<T>() where T : DialogTreeScript<T> {  return ( m_script != null ) ? m_script as T : null; }
	public string GetScriptName(){ return m_scriptName; }
	public string GetScriptClassName() { return m_scriptClass; }	
	public void HotLoadScript(Assembly assembly) 
	{ 
		QuestUtils.HotSwapScript( ref m_script, m_scriptClass, assembly ); 
		if ( m_script != null )
		{
			// Set data in script
			System.Reflection.FieldInfo fi = m_script.GetType().GetField("m_data", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
			fi.SetValue(m_script,Data);
		}
	}

	public GameObject GetPrefab() { return m_prefab; }

	/* NB: this is maybe not used? */
	public DialogTreeComponent GetInstance() { return m_instance; }
	public void SetInstance(DialogTreeComponent instance) 
	{ 
		m_instance = instance; 
		m_instance.SetData(this);
	}
	
	//
	// Public Functions
	//

	// Called from PowerQuest when the dialog is started
	public void OnStart()
	{	
		m_timesShown++;
	}

	// Shortcut to Start/Stop dialog
	public void Start() { PowerQuest.Get.StartDialog(ScriptName); }
	public void Stop() { PowerQuest.Get.StopDialog(); }

	public DialogOption GetOption(string name) { return this[name]; }
	public DialogOption GetOption(int index) { return this[index]; }

	// Shortcut access to options. (NB: Trying to be too clever. I've found these confusing to use. GetOption() makes more sense)
	public DialogOption this[int index]
	{
	    get { return this[index.ToString()]; }
	}

	// Shortcut access to options
	public DialogOption this[string name]
	{
		get 
		{ 
			DialogOption result = m_options.Find(item=>string.Compare( item.Name,name,true)== 0);
			if ( result == null )
				Debug.LogError("Failed to find option "+name+" in dialog "+m_scriptName);
			return result; 
		}
	}

	//
	// AGS style option on/off functions
	//
	public void OptionOn(params int[] id){ System.Array.ForEach(id,item=>this[item].On()); }
	public void OptionOff(params int[] id){System.Array.ForEach(id,item=>this[item].Off()); }
	public void OptionOffForever(params int[] id){ System.Array.ForEach(id,item=>this[item].OffForever()); }
   
	public void OptionOn(params string[] id){ System.Array.ForEach(id,item=>this[item].Show()); }
	public void OptionOff(params string[] id){ System.Array.ForEach(id,item=>this[item].Hide()); }
	public void OptionOffForever(params string[] id){ System.Array.ForEach(id,item=>this[item].HideForever()); }

	public bool GetOptionOn(int option) { return this[option]?.Visible ?? false; }
	public bool GetOptionOffForever(int option) { return this[option]?.Disabled ?? false; }
	public bool GetOptionUsed(int option)	{ return this[option]?.Used ?? false; }
	
	public bool GetOptionOn(string option) { return this[option]?.Visible ?? false; }
	public bool GetOptionOffForever(string option) { return this[option]?.Disabled ?? false; }
	public bool GetOptionUsed(string option)	{ return this[option]?.Used ?? false; }
	

	//
	// Internal Functions
	//

	//
	// Initialisation
	//

	public void EditorInitialise( string name )
	{
		m_scriptName = name;
		m_scriptClass = "Dialog"+name;
	}
	public void EditorRename(string name)
	{
		m_scriptName = name;
		m_scriptClass = "Dialog"+name;	
	}

	public void OnPostRestore( int version, GameObject prefab )
	{
		m_prefab = prefab;
		if ( m_script == null ) // script could be null if it didn't exist in old save game, but does now.
			m_script = QuestUtils.ConstructByName<QuestScript>(m_scriptClass);			
		if ( m_script != null )
		{
			// Set data in script
			System.Reflection.FieldInfo fi = m_script.GetType().GetField("m_data", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
			fi.SetValue(m_script,Data);
		}
	}

	public void Initialise( GameObject prefab )
	{
		m_prefab = prefab;

		// Construct the script
		m_script = QuestUtils.ConstructByName<QuestScript>(m_scriptClass);

		// Deep copy Options
		List<DialogOption> defaultItems = m_options;
		m_options = new List<DialogOption>(defaultItems.Count);
		for ( int i = 0; i < defaultItems.Count; ++i )
		{
			m_options.Add( new DialogOption() );
			QuestUtils.CopyFields(m_options[i], defaultItems[i]);
		}
				
		if ( m_script != null )
		{
			// Set data in script
			System.Reflection.FieldInfo fi = m_script.GetType().GetField("m_data", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
			fi.SetValue(m_script,Data);
		}
	}

	// Handles setting up defaults incase items have been added or removed since last loading a save file
	[System.Runtime.Serialization.OnDeserializing]
	void CopyDefaults( System.Runtime.Serialization.StreamingContext sc )
	{
		QuestUtils.InitWithDefaults(this);
	}
}

#endregion
#region Class: DialogTreeComponent

//
// The Dialog data from the editor
//
[SelectionBase]
public class DialogTreeComponent : MonoBehaviour 
{
	[SerializeField] DialogTree m_data = new DialogTree();

	public DialogTree GetData() { return m_data; }
	public void SetData(DialogTree data) { m_data = data; }
}

#endregion
}
