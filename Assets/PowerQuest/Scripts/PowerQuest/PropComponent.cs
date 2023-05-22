using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerTools;
using UnityEngine.Video;

namespace PowerTools.Quest
{

//
// The component on the prop in scene
//

#region Component

//[SelectionBase]
//[HelpURL("http://powerquest.powerhoof.com")]
public class PropComponent : MonoBehaviour 
{
	#endregion
	#region Component: Variables

	[SerializeField] Prop m_data = new Prop();

	[Header("Parallax")]
	[ParallaxAttribute]
	[SerializeField] float m_parallaxDepth = 0;
	/* Parallax2d disabled for now /
	[HideInInspector][SerializeField] Vector2 m_parallaxDepth2 = new Vector2(0,0);
	/**/
	[Tooltip("eg: (-1,0) means it's drawn clamped to left middle of screen")]
	[SerializeField] Vector2 m_parallaxAlignment = Vector2.zero;

	SpriteRenderer m_sprite = null; 
	SpriteAnim m_spriteAnimator = null;
	#if ( UNITY_SWITCH == false )
	VideoPlayer m_video = null;
	#endif

	bool m_overrideAnimPlaying = false;
	int m_stopOverrideAnimDelay = -1;

	Vector2 m_snapOffset = Vector2.zero;

	ParticleSystem m_particle = null;
	Renderer[] m_renderers = null;
	SpriteRenderer[] m_sprites = null; 
	QuestText[] m_questTexts = null; 
	
	#endregion
	#region Component: Functions: Public

	public Prop GetData() { return m_data; }
	public void SetData(Prop data) 
	{ 
		m_data = data; 
		// Set initial position
		transform.position = m_data.Position.WithZ(transform.position.z);
		OnAnimationChanged();
		OnSetVisible();
	}

	public SpriteRenderer GetSprite() { return m_sprite; }
	public SpriteAnim GetSpriteAnimator() { return m_spriteAnimator; }


	// NB: Animation play/pause/resume/stop stuff doesn't get saved. If you want to permanently change anim, set the Animation property
	public bool GetAnimating()
	{
		return m_overrideAnimPlaying && m_spriteAnimator.Playing;
	}

	public void PlayAnimation(string animName)
	{
		if ( PlayAnimInternal(animName,true) == false && Debug.isDebugBuild )
			Debug.LogWarning("Failed to find prop animation: "+animName); // warn when trying to play anim
		m_overrideAnimPlaying = true;
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
			PlayAnimInternal(GetData().Animation);			
		m_overrideAnimPlaying = false;
	}

	public void OnAnimationChanged()
	{
		// Called when default animation changes
		m_stopOverrideAnimDelay = 0; // Reset any "stop override anim"
		if ( m_overrideAnimPlaying == false && string.IsNullOrEmpty( GetData().Animation ) == false )
			PlayAnimInternal(GetData().Animation, true);	
	}

	#if ( UNITY_SWITCH == false )
	public VideoPlayer GetVideoPlayer() { return m_video; }
	#endif

	#endregion
	#region Component: Functions: Unity 

	// Use this for initialization
	void Awake() 
	{	
		SetupComponents();
	}

	public void SetupComponents()
	{
		if ( m_sprites != null && m_renderers != null )
			return;
		m_sprites = GetComponentsInChildren<SpriteRenderer>(true);		
		m_questTexts = GetComponentsInChildren<QuestText>(true);
		m_renderers = GetComponentsInChildren<Renderer>(true);		
		m_sprite = GetComponentInChildren<SpriteRenderer>(true);
		m_particle = GetComponentInChildren<ParticleSystem>();

		if ( m_sprite != null )
			m_spriteAnimator = m_sprite.GetComponent<SpriteAnim>();
					
		#if ( UNITY_SWITCH == false )
		m_video = GetComponentInChildren<VideoPlayer>(true);
		#endif
	}

	public SpriteRenderer[] Sprites => m_sprites;
	public QuestText[] QuestTexts => m_questTexts;


	void Start()
	{

		#if ( UNITY_SWITCH == false )
		if ( m_video )
			m_video.Prepare();
		#endif

		OnAnimationChanged();
		OnSetVisible();
	}
	public float GetParallax() { return m_parallaxDepth; }

	public void OnSetVisible()
	{
		if ( gameObject.activeSelf == false && GetData().Visible)
			gameObject.SetActive(true);
		
		SetupComponents();

		foreach( Renderer renderer in m_renderers )
		{
			if ( renderer != null )
			renderer.GetComponent<Renderer>().enabled = GetData().Visible;
		}

		if ( GetData().Visible )
		{
			UpdateBaseline();
			UpdateAlpha();
		}
		
		if ( m_particle != null )
		{
			ParticleSystem.EmissionModule emission = m_particle.emission;
			emission.enabled = GetData().Visible;
			if ( GetData().Visible && m_particle.isPlaying == false )
			{
				m_particle.Play();
			}
			m_particle.GetComponent<Renderer>().sortingLayerName = "Default";
		}
	}

	public void OnSetPosition()
	{
		transform.position = m_data.Position.WithZ(transform.position.z);
		UpdateParallax();
		UpdateBaseline();
	}
		
	public void UpdateBaseline()
	{
		int sortOrder = GetData().SortOrder;
		
		if ( m_sprites != null )
			System.Array.ForEach( m_sprites, sprite => { if ( sprite != null ) sprite.sortingOrder = sortOrder; });
		if ( m_questTexts != null )
			System.Array.ForEach( m_questTexts, text => { if ( text != null ) text.OrderInLayer = sortOrder; });			
		if ( m_particle != null )
			m_particle.GetComponent<Renderer>().sortingOrder = GetData().SortOrder;
	}

	public void UpdateAlpha()
	{
		float alpha = GetData().Alpha;
		if ( m_sprites != null )
			System.Array.ForEach( m_sprites, sprite => { if ( sprite != null ) sprite.color = sprite.color.WithAlpha( alpha ); });
		if ( m_questTexts != null )
			System.Array.ForEach( m_questTexts, text => { if ( text != null ) text.color = text.color.WithAlpha( alpha ); });
	}

	// Called once room and everything in it has been created and PowerQuest has initialised references. After Start, Before OnEnterRoom.
	public void OnLoadComplete()
	{
		// Get Room pos
		//RoomComponent room = GetComponentInParent<RoomComponent>();
		LateUpdate();
	}


	void Update()
	{
		if ( m_overrideAnimPlaying  && PowerQuest.Get.GetSkippingCutscene() && m_spriteAnimator.GetCurrentAnimation().isLooping == false ) 
			StopAnimation();

	}

	void LateUpdate()
	{		
		UpdateParallax();

		// There's a delay before going back to original animation after an override, incase on the next line of script the animation is changed
		if ( m_stopOverrideAnimDelay > 0 )
		{
			m_stopOverrideAnimDelay--;
			if ( m_stopOverrideAnimDelay == 0 )
				PlayAnimInternal(GetData().Animation);		
		}

		// If animation has finished, return to default anim
		if ( m_overrideAnimPlaying && m_spriteAnimator.Playing == false )
		{
			if ( m_overrideAnimPlaying ) 
				m_stopOverrideAnimDelay = 1; // Set the delay
			m_overrideAnimPlaying = false;
		}
	}

	void UpdateParallax()
	{
		if (m_parallaxDepth != 0 )
		{			
			float snapAmount = 0;// PowerQuest.Get.SnapAmount;
			RectCentered maxOffset = PowerQuest.Get.GetCamera().GetInstance().GetParallaxOffsetLimits();
			Vector2 parallaxCameraOffset = Utils.Snap(new Vector2(
				maxOffset.Center.x + (m_parallaxAlignment.x * maxOffset.Width*0.5f),
				maxOffset.Center.y + (m_parallaxAlignment.y * maxOffset.Height*0.5f)), snapAmount);
			
			if ( PowerQuest.Get.UseFancyParalaxSnapping && PowerQuest.Get.GetSnapToPixel() && PowerQuest.Get.Camera.GetHasZoomOrTransition() == false )
			{
				// Do some fancy stuff with a "SnapOffset" so the prop will land on a snapped to pixel position, but will smoothly transition from previous position
				Vector2 snapOffsetTarget = m_data.Position + ((PowerQuest.Get.GetCamera().GetInstance().GetParallaxTargetPosition() - parallaxCameraOffset)*m_parallaxDepth); // Target parallax position
				snapOffsetTarget = Utils.Snap(snapOffsetTarget) - snapOffsetTarget;	// Difference between target parallax pos, and current pos
				if ( PowerQuest.Get.GetCamera().GetSnappedLastUpdate() )
				{
					m_snapOffset = snapOffsetTarget;
				}
				else if ( PowerQuest.Get.GetCamera().GetTargetPosChangedLastUpdate() == false ) // Move towards offset only when camera target's not still moving
				{
					m_snapOffset = Vector2.MoveTowards(m_snapOffset, snapOffsetTarget, 2.0f * Time.deltaTime ); // Move an offset towards the target snapped offset smoothly.
				}
				else
				{
					// While camera's moving, lerp back slowly to zero offeset slowly to reduce cases where going from different offset extremes (-1 to 1)
					m_snapOffset = Vector2.MoveTowards(m_snapOffset, Vector2.zero, 0.5f * Time.deltaTime );
				}
			}
			else 
			{
				m_snapOffset = Vector2.zero;
			}

			// Calc parallax offset
			Vector2 parallaxOffset = ((PowerQuest.Get.GetCamera().GetInstance().GetPositionForParallax() - parallaxCameraOffset)*m_parallaxDepth);

			// Apply position, with parallax offset and snapped offset
			transform.position = (m_data.Position + parallaxOffset + m_snapOffset).WithZ(transform.position.z);

		}
	}

	#endregion
	#region Component: Functions: Private
	

	// Plays anim. Returns false if clip not found
	bool PlayAnimInternal(string animName, bool fromStart = true)
	{
		// Must be active to set animation
		if ( gameObject.activeSelf == false )
			gameObject.SetActive(true);

		m_stopOverrideAnimDelay = 0;

		if ( string.IsNullOrEmpty( animName ) )
			return false;

		// Find anim in room's list of anims
		if ( PowerQuest.Get.GetCurrentRoom() == null || PowerQuest.Get.GetCurrentRoom().GetInstance() == null )
			return false;

		AnimationClip clip = PowerQuest.Get.GetCurrentRoom().GetInstance().GetAnimation(animName);
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
			// If no anim found, try sprite
			Sprite sprite = PowerQuest.Get.GetCurrentRoom().GetInstance().GetSprite(animName);
			if ( sprite != null && m_sprite != null )
			{
				m_spriteAnimator.Stop();
				m_sprite.sprite = sprite;
			}			
		}

		return false;
	}
	

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
	
	// Listen for QuestAnimTrigger tags so can pass them up to Gui
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

}
#endregion



#region Prop
//
// Prop Data and functions. Persistant between scenes, as opposed to PropComponent which lives on a GameObject in a scene.
//
[System.Serializable] 
public partial class Prop : IQuestClickable, IProp, IQuestScriptable
{

	#endregion
	#region Prop: Variables
	//
	// Default values set in inspector
	//
	
	[Header("Mouse-over Defaults")]
	[TextArea(1,10)]
	[SerializeField] string m_description = "New Prop";
	[Tooltip("If set, changes the name of the cursor when moused over")]
	[SerializeField] string m_cursor = null;

	[Header("Starting State")]
	[SerializeField] bool m_visible = true;
	[Tooltip("Whether clicking on hotspot triggers an event")]
	[SerializeField] bool m_clickable = true;
	//[SerializeField] bool m_collidable = false;
	[SerializeField] string m_animation = null;	
	[SerializeField] float m_alpha = 1;
	[Header("Editable in Scene")]
	[Tooltip("Move the transform around to change this (unlike characters!)")]
	[ReadOnly][SerializeField] Vector2 m_position = Vector2.zero; // This is taken from the instance position in the scene first time it's
	[SerializeField] float m_baseline = 0;	
    [SerializeField, Tooltip("If true, the baseline will be in world position, instead of local to the object. So y position of the sortable is ignored")] bool m_baselineFixed = false;
	[SerializeField] Vector2 m_walkToPoint = Vector2.zero;
	[SerializeField] Vector2 m_lookAtPoint = Vector2.zero;	
	[ReadOnly,SerializeField] string m_scriptName = "PropNew";

	//
	// Private variables
	//
	PropComponent m_instance = null;
	int m_useCount = 0;
	int m_lookCount = 0;
	

	#endregion
	#region Prop: Properties
	//
	//  Properties
	//
	public eQuestClickableType ClickableType { get {return eQuestClickableType.Prop; } }
	public string Description { get{ return m_description;} set{m_description = value;} }
	public string ScriptName { get{ return m_scriptName;} }
	public MonoBehaviour Instance { get{ return m_instance; } }
	public Prop Data {get {return this;} }
	public IQuestClickable IClickable { get{ return this; } }
	public bool Visible 
	{ 
		get{return m_visible;} 
		set	{m_visible = value;	if ( m_instance ) m_instance.OnSetVisible(); }
	}
	//public bool Collidable { get{ return m_collidable;} set{ m_collidable = value;} }
	public bool Clickable { get{ return m_clickable;} set{m_clickable = value;} }
	/// Set's visible & clickable (same as `Enable()`)
	public void Show( bool clickable = true ) { Enable( clickable ); }
	/// Set's invisible & non-clickable (same as `Disable()`)
	public void Hide() { Disable(); }
	/// Set's visible & clickable
	public void Enable( bool clickable = true )
	{
		Visible = true;
		if ( clickable ) Clickable = true;
	}
	/// Set's invisible & non-clickable
	public void Disable() { Visible = false; Clickable = false; }
	public Vector2 Position { get{ return m_position;} set
	{ 
		float oldY = m_position.y;
		m_position = value; 
		if ( m_instance != null ) 
		{ 
			m_instance.OnSetPosition(); 
			if ( m_baselineFixed == false && oldY != value.y )
				m_instance.UpdateBaseline();
		} 
	} }
	public void SetPosition(float x, float y) { Position = new Vector2(x,y); }
	public float Baseline { get{ return m_baseline;} set{m_baseline = value; if ( m_instance != null ) m_instance.UpdateBaseline(); } }
	public bool BaselineFixed { get { return m_baselineFixed; } set { m_baselineFixed=value; if ( m_instance != null ) m_instance.UpdateBaseline(); } }
	public int SortOrder => -Mathf.RoundToInt(((m_baselineFixed ? 0.0f: Position.y) + Baseline)*10.0f);

	public Vector2 WalkToPoint { get{ return m_walkToPoint;} set{m_walkToPoint = value;} }
	public Vector2 LookAtPoint { get{ return m_lookAtPoint;} set{m_lookAtPoint = value;} }


	public string Cursor { get { return m_cursor; } set { m_cursor = value; }  }
	public bool FirstUse { get { return UseCount == 0; } } 
	public bool FirstLook { get { return LookCount == 0; } }
	public int UseCount { get { return m_useCount - (PowerQuest.Get.GetInteractionInProgress(this,eQuestVerb.Use) ? 1 : 0); } }
	public int LookCount { get { return m_lookCount - (PowerQuest.Get.GetInteractionInProgress(this,eQuestVerb.Look) ? 1 : 0); } }

	public string Animation
	{ 
		get{return m_animation;}
		set
		{
			m_animation = value;
			if ( m_instance != null ) m_instance.OnAnimationChanged();
		}
	}
	public bool Animating
	{
		get { return m_instance != null && m_instance.GetAnimating(); }
	}

	#endregion
	#region Partial functions for extentions
	
	partial void ExOnInteraction(eQuestVerb verb);
	partial void ExOnCancelInteraction(eQuestVerb verb);

	#endregion
	#region Prop: Functions: Public 
	//
	// Getters/Setters - These are used by the engine. Scripts mainly use the properties
	//
	public PropComponent GetInstance() { return m_instance; }
	public void SetInstance(PropComponent instance) 
	{ 
		m_instance = instance; 
		instance.SetData(this);
	}
	// Return room's script
	public QuestScript GetScript() { return (PowerQuest.Get.GetCurrentRoom() == null) ? null : PowerQuest.Get.GetCurrentRoom().GetScript(); } 
	public IQuestScriptable GetScriptable() { return this; }

	//
	// Public Functions
	//
	
	public void OnInteraction( eQuestVerb verb )
	{		
		if ( verb == eQuestVerb.Look ) ++m_lookCount;
		else if ( verb == eQuestVerb.Use) ++m_useCount;

		ExOnInteraction(verb);
	}
	public void OnCancelInteraction( eQuestVerb verb )
	{		
		if ( verb == eQuestVerb.Look ) --m_lookCount;
		else if ( verb == eQuestVerb.Use) --m_useCount;

		ExOnCancelInteraction(verb);
	}
	public void IsCollidingWith() {throw new System.NotImplementedException();} // TODO: object collision

	// NB: Animation play/pause/resume/stop stuff doesn't get saved. If you want to permanently change anim, set the Animation property
	public Coroutine PlayAnimation(string animName)
	{
		if ( m_instance != null ) return PowerQuest.Get.StartCoroutine(CoroutinePlayAnimation(animName)); 
		return null;
	}

	public void PlayAnimationBG(string animName) { if ( m_instance != null && PowerQuest.Get.GetSkippingCutscene() == false ) m_instance.PlayAnimation(animName); }
	public void PauseAnimation() { if ( m_instance != null ) m_instance.PauseAnimation(); }
	public void ResumeAnimation() { if ( m_instance != null ) m_instance.ResumeAnimation(); }

	public void AddAnimationTrigger(string triggerName, bool removeAfterTriggering, System.Action action)
	{
		if ( m_instance != null )
		{
			QuestAnimationTriggers triggerComponent = m_instance.GetComponent<QuestAnimationTriggers>();
			if ( triggerComponent == null )
				triggerComponent = m_instance.gameObject.AddComponent<QuestAnimationTriggers>();
			if ( triggerComponent != null )
				triggerComponent.AddTrigger(triggerName, action, removeAfterTriggering);
		}
	}
	public void RemoveAnimationTrigger(string triggerName)
	{
		if ( m_instance != null )
		{
			QuestAnimationTriggers triggerComponent = m_instance.GetComponent<QuestAnimationTriggers>();
			if ( triggerComponent != null )
				triggerComponent.RemoveTrigger(triggerName);
		}
	}

	public Coroutine WaitForAnimTrigger(string triggerName) { return PowerQuest.Get.StartCoroutine(CoroutineWaitForAnimTrigger(triggerName)); }

	// NB: These don't save/load correctly, or handle changing room, etc. They should store their state ("TargetPosition" and "Speed") and handle movement in update.
	public Coroutine MoveTo(float x, float y, float speed, eEaseCurve curve = eEaseCurve.None) {  return MoveTo(new Vector2(x,y),speed, curve); }
	public Coroutine MoveTo(Vector2 toPos, float speed, eEaseCurve curve = eEaseCurve.None) {  return PowerQuest.Get.StartQuestCoroutine(CoroutineMoveTo(toPos,speed, curve)); }
	public void MoveToBG(Vector2 toPos, float speed, eEaseCurve curve = eEaseCurve.None) {  PowerQuest.Get.StartQuestCoroutine(CoroutineMoveTo(toPos,speed, curve)); }

	#if ( UNITY_SWITCH == false )

	/// Starts video playback if the prop has a video component. Returns once the video has completed, or on mouse click if skippableAfterTime is greater than zero
	/// NB: Video playback position isn't currently saved
	public Coroutine PlayVideo(float skippableAfterTime = -1)
	{
		if ( m_instance != null ) return PowerQuest.Get.StartCoroutine(CoroutinePlayVideo(skippableAfterTime)); 
		return null;
	}

	/// Starts video playback if the prop has a video component
	public void PlayVideoBG() 
	{ 
		if ( m_instance == null ) return;
		VideoPlayer video = m_instance.GetVideoPlayer();
		if ( video == null ) 
		{
			Debug.LogWarning("Video Playback failed- No VideoPlayer component added to prop "+ ScriptName); 
			return;
		}
		video.enabled = true;
		video.Play();
	}

	/// Gets the prop's VideoPlayer component (if it has one). This can be used to pause/resume/stop video playback
	public VideoPlayer VideoPlayer
	{
		get { 
			if ( m_instance == null ) return null;
			return m_instance.GetVideoPlayer();
		} 
	}
	#endif

	public void EditorInitialise( string name )
	{
		m_description = name;
		m_scriptName = name;
		m_animation = name;
	}
	public void EditorRename(string name)
	{
		// Could also rename the default animation, but perhaps better to leave that manual
		//if ( m_animation == m_scriptName )
			//m_animation = name;
		m_scriptName = name;	
	}
		
	/// Fade the sprite's alpha
	public Coroutine Fade(float start, float end, float duration, eEaseCurve curve = eEaseCurve.Smooth ) { return PowerQuest.Get.StartCoroutine(CoroutineFade(start, end, duration, curve)); }
	/// Fade the sprite's alpha (non-blocking)
	public void FadeBG(float start, float end, float duration, eEaseCurve curve = eEaseCurve.Smooth ) { PowerQuest.Get.StartCoroutine(CoroutineFade(start, end, duration, curve)); }
	
	public float Alpha { 
		get { return m_alpha; } 
		set 
		{ 
			m_alpha = value; 			
			if (m_instance != null )
				m_instance.UpdateAlpha();
		} 
	}

	#endregion
	#region Prop: Coroutines

	IEnumerator CoroutinePlayAnimation(string animName) 
	{
		
		if ( m_instance == null ) yield break;
		m_instance.PlayAnimation(animName);
		while ( m_instance != null && m_instance.GetAnimating() && PowerQuest.Get.GetSkippingCutscene() == false )
		{				
			yield return new WaitForEndOfFrame();
		}
		if ( PowerQuest.Get.GetSkippingCutscene() && m_instance != null )
		{
			SpriteAnim animComponent = m_instance.GetComponent<SpriteAnim>();
			if ( animComponent != null )
			{
				// Skip to "end" of animation, and force update so that any animation changes are applied
				animComponent.NormalizedTime = 1;
				m_instance.GetComponent<Animator>().Update(0);
			}

			m_instance.StopAnimation();
		}
		yield break;
	}


	#if ( UNITY_SWITCH == false )
	IEnumerator CoroutinePlayVideo(float skippableAfterTime = -1) 
	{
		if ( PowerQuest.Get.GetSkippingCutscene() )
			yield break;
		
		if ( m_instance == null ) yield break;
		VideoPlayer video = m_instance.GetVideoPlayer();
		if ( video == null ) 
		{
			Debug.LogWarning("Video Playback failed- No VideoPlayer component added to prop "+ ScriptName); 
			yield break;
		}
		bool wasEnabled = video.enabled;
		video.enabled = true;
		video.Play();
		yield return PowerQuest.Get.WaitUntil( () => video.isPlaying ); // Wait until video starts playing
		if ( skippableAfterTime >= 0 )
			yield return PowerQuest.Get.Wait( skippableAfterTime );
		yield return PowerQuest.Get.WaitWhile( () => video.isPlaying || Application.isFocused == false, skippableAfterTime >= 0); // Wait until video stops playing
		video.Stop();
		if ( wasEnabled == false )
			video.enabled = false;
		yield break;
	}
	#endif

	IEnumerator CoroutineMoveTo(Vector2 toPos, float speed, eEaseCurve curve = eEaseCurve.None)
	{
		Debug.Assert(speed>0,"Prop move speed must be greater than zero");

		Vector2 startPos = Position;
		float time = Vector2.Distance(startPos,toPos)/speed;
		float currTime = 0;
		while ( currTime < time && PowerQuest.Get.GetSkippingCutscene() == false )
		{			
			if ( SystemTime.Paused == false )			
			{
				currTime += Time.deltaTime;
			}
			Position = Vector2.Lerp(startPos,toPos, QuestUtils.Ease(currTime/time,curve) );
			
			yield return new WaitForEndOfFrame();
		}
		Position = toPos;
	}

	

	IEnumerator CoroutineFade(float start, float end, float duration, eEaseCurve curve = eEaseCurve.Smooth )
	{
		if ( Instance == null )
			yield break;

		float time = 0;
		
		Alpha = start;
		while ( time < duration && PowerQuest.Get.GetSkippingCutscene() == false )
		{
			yield return new WaitForEndOfFrame();
					
			if ( SystemTime.Paused == false )
				time += Time.deltaTime;
			Alpha = Mathf.Lerp(start,end, QuestUtils.Ease(time/duration,curve));
		}
		Alpha = end;
	}

	IEnumerator CoroutineWaitForAnimTrigger(string triggerName)
	{
		if ( PowerQuest.Get.GetSkippingCutscene() == false )
		{
			bool hit = false;
			AddAnimationTrigger(triggerName,true,()=>hit=true);
			yield return PowerQuest.Get.WaitUntil(()=> hit || m_instance == null || m_instance.GetSpriteAnimator().Playing == false );
		}
		yield break;
	}

	#endregion
	#region Functions: Implementing IQuestScriptable

	// Doesn't use all functions
	public string GetScriptName() { return m_scriptName; }
	public string GetScriptClassName() { return "Prop"+m_scriptName; }
	public void HotLoadScript(System.Reflection.Assembly assembly) { /*No-op*/ }

	#endregion
	#region Prop: Functions: Private 
	// Handles setting up defaults incase items have been added or removed since last loading a save file
	[System.Runtime.Serialization.OnDeserializing]
	void CopyDefaults( System.Runtime.Serialization.StreamingContext sc )
	{
		QuestUtils.InitWithDefaults(this);
	}
}

#endregion

}
