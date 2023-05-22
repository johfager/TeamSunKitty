using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;
using PowerTools.Quest;

namespace PowerTools.QuestGui
{

//
[System.Serializable] 
[AddComponentMenu("Quest Gui/Button")]
public partial class Button : GuiControl, IButton
{

	enum eState	{ Default, Hover, Click, Off }
	
	public enum eSizeSetting
	{
		Custom,           // Set Size-> it gets set in hotspot. No change to image.  No stretch component
		ResizableImage, // Set Size-> it gets set in hotspot, and image is scaled to match (or w/h set).  No stretch component
		Image,	      // Hotspot size taken from image. No change to image. No stretch component
		FitText,         // Image set up to contain gui text.  No stretch component
	}	
	public enum eColorUse { None, Text, Image, Both }

	#region Vars: Editor
	
	[Tooltip("Whether button can be clicked. When false, the button's anim/colour is set to the 'Inactive' one")]
	[SerializeField] bool m_clickable = true;


	[Header("Mouse-over Defaults")]
	[TextArea(1,10)]
	[SerializeField] string m_description = "New Button";
	[Tooltip("If set, changes the name of the cursor when moused over")]
	[SerializeField] string m_cursor = null;

	[Header("Visuals")]
	[SerializeField] string m_anim = null;
	[SerializeField] string m_animHover = null;
	[SerializeField] string m_animClick = null;
	[SerializeField] string m_animOff = null;
	
	[SerializeField] eColorUse m_colorWhat = eColorUse.Text;
	[UnityEngine.Serialization.FormerlySerializedAs("m_textColor")]
	[SerializeField] Color m_color = new Color(0,0,0,1);	
	[UnityEngine.Serialization.FormerlySerializedAs("m_textColorHover")]
	[SerializeField] Color m_colorHover = new Color(0,0,0,0);
	[UnityEngine.Serialization.FormerlySerializedAs("m_textColorClick")]
	[SerializeField] Color m_colorClick = new Color(0,0,0,0);
	[UnityEngine.Serialization.FormerlySerializedAs("m_textColorOff")]
	[SerializeField] Color m_colorOff = new Color(0,0,0,0);

	[Header("Hotspot size")]
	[SerializeField] Padding m_hotspotPadding = Padding.zero;
	[SerializeField, HideInInspector] eSizeSetting m_sizeSetting = eSizeSetting.Image; 
		
	[SerializeField, HideInInspector] RectCentered m_customSize = RectCentered.zero;
	
	// Callback on click: void OnClick(GuiControl button)
	public System.Action<GuiControl> OnClick = null;


	#endregion
	#region Vars: Private
	
	SpriteRenderer m_sprite = null; 
	SpriteAnim m_spriteAnimator = null;
	QuestText m_questText = null;
	FitToObject m_stretchComponent = null;
	BoxCollider2D m_boxCollider2D = null;
	eState m_state = eState.Default;	
	string m_cachedText = null;

	bool m_overrideAnimPlaying = false;
	int m_stopOverrideAnimDelay = -1;
	
	#endregion
	#region Funcs: IButton interface
	
	public IQuestClickable IClickable { get{ return this; } }
	
	public string Text 
	{
		get
		{
			if ( m_questText == null )
				m_questText = GetComponentInChildren<QuestText>();
			if ( m_questText != null )
				return m_questText.text;
			return string.Empty;
		}
		set
		{
			if ( m_questText == null )
				m_questText = GetComponentInChildren<QuestText>();
			if ( m_questText != null )
				m_questText.text = value;
		}
	}

	public string Anim	       { get { return m_anim;} set { m_anim = value; OnAnimationChanged(); } }
	public string AnimHover	   { get { return m_animHover;} set { m_animHover = value; OnAnimationChanged(); } }
	public string AnimClick	   { get { return m_animClick;} set { m_animClick = value; OnAnimationChanged(); } }
	public string AnimOff { get { return m_animOff;} set { m_animOff = value; OnAnimationChanged(); } }
	
	public Color Color	        { get{return m_color;} set { m_color = value; OnColorChanged(); } }
	public Color ColorHover    { get{return m_colorHover;} set { m_colorHover = value; OnColorChanged(); } }
	public Color ColorClick    { get{return m_colorClick;} set { m_colorClick = value; OnColorChanged(); } }
	public Color ColorOff { get{return m_colorOff;} set { m_colorOff = value; OnColorChanged(); } }

	public eColorUse ColorWhat => m_colorWhat;

	
	public bool Animating
	{
		get { return GetAnimating(); }
	}

	public void PauseAnimation()
	{
		if ( m_overrideAnimPlaying )
		{
			m_spriteAnimator.Pause();
		}
	}

	public void ResumeAnimation()
	{
		if ( m_overrideAnimPlaying )
		{
			m_spriteAnimator.Resume();
		}
	}

	// Return to current animation set in data
	public void StopAnimation()
	{
		if ( m_overrideAnimPlaying )
			StartStateAnimation();		
		m_overrideAnimPlaying = false;
	}
	
	// NB: Animation play/pause/resume/stop stuff doesn't get saved. If you want to permanently change anim, set the Animation property
	public Coroutine PlayAnimation(string animName)
	{
		return PowerQuest.Get.StartCoroutine(CoroutinePlayAnimation(animName)); 
	}
	public void PlayAnimationBG(string animName) 
	{ 
		if ( PowerQuest.Get.GetSkippingCutscene() == false ) PlayOverrideAnim(animName); 
	}

	public void AddAnimationTrigger(string triggerName, bool removeAfterTriggering, System.Action action)
	{		
		QuestAnimationTriggers triggerComponent = GetComponent<QuestAnimationTriggers>();
		if ( triggerComponent == null )
			triggerComponent = gameObject.AddComponent<QuestAnimationTriggers>();
		if ( triggerComponent != null )
			triggerComponent.AddTrigger(triggerName, action, removeAfterTriggering);		
	}
	public void RemoveAnimationTrigger(string triggerName)
	{
		QuestAnimationTriggers triggerComponent = GetComponent<QuestAnimationTriggers>();
		if ( triggerComponent != null )
			triggerComponent.RemoveTrigger(triggerName);		
	}

	public Coroutine WaitForAnimTrigger(string triggerName) { return PowerQuest.Get.StartCoroutine(CoroutineWaitForAnimTrigger(triggerName)); }
	
	#endregion
	#region Functions: Public (Non interface)

	public eSizeSetting SizeSetting { get {return m_sizeSetting;} set{m_sizeSetting=value;} }	
	public Padding HotspotPadding { get {return m_hotspotPadding; } set{m_hotspotPadding=value;} }
	public override RectCentered CustomSize {get {return m_customSize;} set{m_customSize=value;}}

	public void UpdateHotspot()
	{
		if ( m_boxCollider2D == null )
		{
			m_boxCollider2D = GetComponent<BoxCollider2D>();
			if ( m_boxCollider2D == null )
			{
				Debug.LogWarning("Buttons need a BoxCollider2D to Auto-Scale their Hotspot");
				m_sizeSetting = eSizeSetting.Custom;
			}
		}
		if ( m_boxCollider2D != null )
		{			
			InitComponentReferences();

			RectCentered bounds = GuiUtils.CalculateGuiRectInternal( transform, false, m_sprite, m_questText == null ? null : m_questText.GetComponent<MeshRenderer>() );
			bounds.AddPadding(m_hotspotPadding);
			if ( bounds.Center != m_boxCollider2D.offset || m_boxCollider2D.size != bounds.Size )
			{
				m_boxCollider2D.offset=bounds.Center;
				m_boxCollider2D.size=bounds.Size;
			}
		}
	}	
			
	public SpriteRenderer GetSprite() { return m_sprite; }
	public SpriteAnim GetSpriteAnimator() { return m_spriteAnimator; }
	public QuestText GetQuestText() { return m_questText; }
	
	public void EditorUpdateSprite()
	{
		if ( m_sprite == null )
			m_sprite = GetComponentInChildren<SpriteRenderer>();
		if ( m_sprite != null )
		{			
			GuiComponent guiComponent = GetComponentInParent<GuiComponent>();				
			guiComponent.GetAnimation(m_anim);
		}
		// TODO: this doesn't do anything yet!
	}
		
	// Returns the "rect" of the control. For images, that's the image size/position. Used for aligning controls to each other
	public override RectCentered GetRect(Transform excludeChild = null)
	{		
		InitComponentReferences();
		MeshRenderer textRenderer = null;
		if ( m_questText != null )
			textRenderer = m_questText.GetComponent<MeshRenderer>();
					
		if( m_sprite != null || textRenderer != null )
		{
			RectCentered result = GuiUtils.CalculateGuiRectInternal(transform, false, m_sprite, textRenderer, excludeChild);
			result.Transform(transform);
			return result;
		}
		return RectCentered.zero;
	}
	
	#endregion
	#region Component: Functions: Unity
	

	void InitComponentReferences()
	{
		if ( m_sprite == null )
			m_sprite = GetComponentInChildren<SpriteRenderer>(true);
		if ( m_sprite != null && m_spriteAnimator == null )
			m_spriteAnimator = m_sprite.GetComponent<SpriteAnim>();
		if ( m_questText == null )
			m_questText = GetComponentInChildren<QuestText>();
	}

	// Use this for initialization
	void Awake() 
	{	
		InitComponentReferences();
		ExAwake();

		// Set state if clickable/not clickable.
		// NB: set this in Awake, since if color is changed by fading on show, it'll be overriden by this state change
		SetState(Clickable ? eState.Default : eState.Off);
	}

	void Start()
	{		
		InitComponentReferences();

		StartStateAnimation();

		if ( m_sizeSetting == eSizeSetting.FitText )
		{
			// Update stretched image first			
			if ( m_stretchComponent == null )
				m_stretchComponent = GetComponentInChildren<FitToObject>();
			if ( m_stretchComponent != null )
				m_stretchComponent.UpdateSize();
			m_cachedText = Text;
		}
		if ( m_sizeSetting == eSizeSetting.FitText || m_sizeSetting == eSizeSetting.Image )
		{
			UpdateHotspot();
		}
	}	

	void Update()
	{
		UpdateOverrideAnim();

		// Set state if clickable/not clickable
		if ( (m_state != eState.Off) != Clickable )			
			SetState(Clickable ? eState.Default : eState.Off);

		switch ( m_state )
		{
			case eState.Default:
			{
				if ( Focused )
					SetState(eState.Hover);
			} break;
			case eState.Hover:
			{
				// check for click
				if ( Focused == false )
					SetState(eState.Default);				
				else if ( Input.GetMouseButtonDown(0) )
					SetState(eState.Click);				

			} break;
			case eState.Click:
			{
				if ( Input.GetMouseButton(0) == false )
				{
					if ( Focused )
					{
						PowerQuest.Get.ProcessGuiClick(GuiData, this);
						// Also send a message upwards for gui components to use
						SendMessageUpwards(PowerQuest.SCRIPT_FUNCTION_CLICKGUI+ScriptName, this, SendMessageOptions.DontRequireReceiver );
						if ( OnClick != null )
							OnClick.Invoke(this);
					}
					SetState(eState.Default);
				}

			}break;
		}

		if ( Input.GetMouseButtonDown(1) && Focused )
		{
			// Handle right clicking when focused
			PowerQuest.Get.ProcessGuiClick(GuiData, this);
		}
		
		ExUpdate();
	}

	void SetState(eState newState)
	{
		m_state = newState;
		UpdateColor();
		StartStateAnimation();		
	}
	
	void LateUpdate()
	{
		LateUpdateOverrideAnim();
		if ( m_sizeSetting == eSizeSetting.FitText && m_cachedText != Text )
		{
			if ( m_stretchComponent == null )
				m_stretchComponent = GetComponentInChildren<FitToObject>();
			if ( m_stretchComponent != null )
				m_stretchComponent.UpdateSize();
			UpdateHotspot();
			m_cachedText = Text;
		}
	}	

	#endregion
	#region Funcs: Private Internal
		

	void OnColorChanged()
	{
		UpdateColor();
	}
	void OnAnimationChanged()
	{
		StartStateAnimation();
	}	

	void UpdateColor()
	{	
		Color color = m_color;		
		switch ( m_state )
		{
			case eState.Hover: color = m_colorHover; break;
			case eState.Click: color = m_colorClick; break;
			case eState.Off: color = m_colorOff; break;
		}			
		if ( m_questText != null && (m_colorWhat == eColorUse.Text || m_colorWhat == eColorUse.Both) )
			m_questText.color = color;
		if ( m_sprite != null && (m_colorWhat == eColorUse.Image || m_colorWhat == eColorUse.Both) )
			m_sprite.color = color;
	}

	// 
	void StartStateAnimation()
	{
		// Called when default animation changes
		m_stopOverrideAnimDelay = 0; // Reset any "stop override anim"

		// Try finding animation based on state- 
		 
		bool foundAnim = false;
		switch( m_state )
		{
			case eState.Hover: foundAnim = PlayAnimInternal(m_animHover); break;
			case eState.Click:
			{
				foundAnim = PlayAnimInternal(m_animClick);
				if ( foundAnim == false )
					foundAnim = PlayAnimInternal(m_animHover);
			} break;	
			case eState.Off: foundAnim = PlayAnimInternal(m_animOff); break;
		}
		if ( foundAnim == false )
			PlayAnimInternal(m_anim);		
	}
	
	// Plays anim. Returns false if clip not found	
	bool PlayAnimInternal(string animName, bool fromStart = true)
	{
		if ( m_spriteAnimator == null )
			return true;

		m_stopOverrideAnimDelay = 0;
		
		if ( string.IsNullOrEmpty( animName ) )
			return false;

		// Find anim in gui's list of anims
		AnimationClip clip = GetAnimation(animName);
		if ( clip != null && m_spriteAnimator != null )
		{
			if ( fromStart || m_spriteAnimator.Clip == null  )
			{
				m_spriteAnimator.Play(clip);
			}
			else
			{
				float animTime = m_spriteAnimator.Time;
				m_spriteAnimator.Play(clip);
				m_spriteAnimator.Time = animTime;
			}
			return true;
		}
		else
		{
		
			Sprite sprite = GetSprite(animName);
			if ( sprite != null )
			{
				m_spriteAnimator.Stop();
				m_sprite.sprite=sprite;
				return true;
			}
		}
		
		return false;
	}
	
	
	#endregion
	#region Partial Functions for extentions
	
	partial void ExAwake();
	//partial void ExOnDestroy();
	partial void ExUpdate();
	
	#endregion	
	#region Implementing IQuestClickable
	
	public override string Description { get{ return m_description;} set{m_description = value;} }
	public override bool Clickable { get{ return m_clickable;} set{ m_clickable = value; } }
	public override string Cursor { get { return m_cursor; } set { m_cursor = value; }  }
	
	#endregion
	#region Funcs: Coroutines


	IEnumerator CoroutinePlayAnimation(string animName) 
	{
		PlayOverrideAnim(animName);
		while ( GetAnimating() && PowerQuest.Get.GetSkippingCutscene() == false )
		{				
			yield return new WaitForEndOfFrame();
		}
		if ( PowerQuest.Get.GetSkippingCutscene() )
		{
			SpriteAnim animComponent = GetComponent<SpriteAnim>();
			if ( animComponent != null )
			{
				// Skip to "end" of animation, and force update so that any animation changes are applied
				animComponent.NormalizedTime = 1;
				GetComponent<Animator>().Update(0);
			}

			StopAnimation();
		}
		yield break;
	}
	

	IEnumerator CoroutineWaitForAnimTrigger(string triggerName)
	{
		if ( PowerQuest.Get.GetSkippingCutscene() == false )
		{
			bool hit = false;
			AddAnimationTrigger(triggerName,true,()=>hit=true);
			yield return PowerQuest.Get.WaitUntil(()=> hit || GetSpriteAnimator().Playing == false );
		}
		yield break;
	}	
	
	
	#endregion
	#region Funcs: Private Internal
	
	void UpdateOverrideAnim()
	{
		if ( m_overrideAnimPlaying  && PowerQuest.Get.GetSkippingCutscene() && m_spriteAnimator.GetCurrentAnimation().isLooping == false ) 
			StopAnimation();
	}

	void LateUpdateOverrideAnim()
	{
		// There's a delay before going back to original animation after an override, incase on the next line of script the animation is changed
		if ( m_stopOverrideAnimDelay > 0 )
		{
			m_stopOverrideAnimDelay--;
			if ( m_stopOverrideAnimDelay == 0 )
				StartStateAnimation();
		}

		// If animation has finished, return to default anim
		if ( m_overrideAnimPlaying && m_spriteAnimator.Playing == false )
		{
			if ( m_overrideAnimPlaying ) 
				m_stopOverrideAnimDelay = 1; // Set the delay
			m_overrideAnimPlaying = false;
		}
	}

	void PlayOverrideAnim(string animName)
	{
		if ( PlayAnimInternal(animName,true) == false && Debug.isDebugBuild )
			Debug.LogWarning("Failed to find Button animation: "+animName); // warn when trying to play anim
		m_overrideAnimPlaying = true;
	}

	bool GetAnimating()
	{
		return m_overrideAnimPlaying && m_spriteAnimator.Playing;
	}
	/* NB: This was never used
	void OnSetVisible()
	{
		if ( gameObject.activeSelf == false && Visible)
			gameObject.SetActive(true);
		
		if ( GetSprite() )
			GetSprite().GetComponent<Renderer>().enabled = Visible;

		Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
		foreach( Renderer renderer in renderers )
		{   
			renderer.GetComponent<Renderer>().enabled = Visible;
		}
	}*/

	// Handles setting up defaults incase items have been added or removed since last loading a save file
	/*[System.Runtime.Serialization.OnDeserializing]
	void CopyDefaults( System.Runtime.Serialization.StreamingContext sc )
	{
		QuestUtils.InitWithDefaults(this);
	}*/
	
	#endregion
	#region Functions: Anim Events

	void AnimSound(Object obj)
	{
		if ( obj != null && (obj as GameObject) != null )
		{
			SystemAudio.Play((obj as GameObject).GetComponent<AudioCue>());
		}
	}

	void AnimSound(string sound)
	{
		SystemAudio.Play(sound);	    
	}
		
	// Listen for QuestAnimTrigger tags so can pass them up
	QuestAnimationTriggers m_animTriggerComponent = null;
	void _Anim(string function)
	{
		if ( m_animTriggerComponent == null )
		{
			m_animTriggerComponent = transform.GetComponent<QuestAnimationTriggers>();
			if ( m_animTriggerComponent == null )
				m_animTriggerComponent = transform.gameObject.AddComponent<QuestAnimationTriggers>();
		}
	}

	#endregion
}



}
