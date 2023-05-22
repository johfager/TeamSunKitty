using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using PowerTools.Quest;
using PowerTools;
using PowerTools.QuestGui;


namespace PowerTools.Quest
{

[CanEditMultipleObjects]
[CustomEditor(typeof(QuestText))]
public class QuestTextEditor : Editor 
{

	public void OnEnable()
	{
		// Set charater size to 10 if snapping
		if ( PowerQuestEditor.Snap )
		{
			QuestText component = (QuestText)target;
			TextMesh tm = component != null ? component.GetComponent<TextMesh>() : null;
			if ( tm != null && tm.characterSize == 1 )
			{
				tm.characterSize = 10;
				EditorUtility.SetDirty(tm);					
			}
		}		
	}

	public override void OnInspectorGUI()
	{
		// NB: Quest text duplicates a bunch of textmesh controls so they can be in one place.

		QuestText component = (QuestText)target;
		SerializedObject serializedObj = new SerializedObject(component);
		TextMesh meshComponent = component.transform.GetComponent<TextMesh>();

		// Make the quest text component the first in the list.. Have to do some hackery to ensure it's not an unstaged prefab
		Component[] list = component.GetComponents<Component>();
		if ( list[1] != component && list[1] is MeshRenderer && (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.NotAPrefab || ( UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null && UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot == component.gameObject) ))
			UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
		
		EditorGUI.BeginChangeCheck();

		EditorGUILayout.LabelField("Text Properties", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_text"));
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_localize"));

		GUILayout.Space(10);		
		EditorGUILayout.LabelField("Font", EditorStyles.boldLabel);

		Font newFont = EditorGUILayout.ObjectField("Font", meshComponent.font, typeof(Font),false ) as Font;
		if ( newFont != meshComponent.font )
		{
			//newFont.material = null;
			meshComponent.font = newFont;
			meshComponent.GetComponent<MeshRenderer>().material = newFont.material;
		}
		meshComponent.fontSize = EditorGUILayout.IntField("Size", meshComponent.fontSize);		
		float lineSpacing = meshComponent.lineSpacing * Mathf.Max(meshComponent.fontSize,10);
		lineSpacing = EditorGUILayout.FloatField("Line spacing", lineSpacing);
		meshComponent.lineSpacing = lineSpacing / Mathf.Max(meshComponent.fontSize,10);
		meshComponent.color = EditorGUILayout.ColorField("Color", meshComponent.color);
		
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_outline"));
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_shaderOverride"));
		
		GUILayout.Space(10);	
		EditorGUILayout.LabelField("Alignment", EditorStyles.boldLabel);

		meshComponent.alignment = (TextAlignment)EditorGUILayout.EnumPopup("Alignment", meshComponent.alignment );
		meshComponent.anchor = (TextAnchor)EditorGUILayout.EnumPopup("Anchor", meshComponent.anchor );
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_sortingLayer"));
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_orderInLayer"));
		
		GUILayout.Space(10);	
		EditorGUILayout.LabelField("Wrap/Truncate Settings", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_wrapWidth"), new GUIContent("Width (Pixels)"));
		if ( component.WrapWidth > 0 )
		{
			EditorGUILayout.PropertyField(serializedObj.FindProperty("m_truncate"));
			EditorGUILayout.PropertyField(serializedObj.FindProperty("m_wrapUniformLineWidth"));
		}
		EditorGUILayout.PropertyField(serializedObj.FindProperty("m_keepOnScreen"));
		if ( serializedObj.FindProperty("m_keepOnScreen").boolValue == true || serializedObj.FindProperty("m_wrapUniformLineWidth").boolValue == true)
			EditorGUILayout.PropertyField(serializedObj.FindProperty("m_wrapWidthMin"));
		
		GUILayout.Space(10);	
		EditorGUILayout.HelpBox("Note, some fields are duplicated in the TextMesh component below, you can edit them in either place just fine ;)", MessageType.Info);

		if ( EditorGUI.EndChangeCheck() )
		{
			component.SendMessage("EditorUpdate");
			serializedObj.ApplyModifiedProperties();
			EditorUtility.SetDirty(meshComponent);
			EditorUtility.SetDirty(target);
		}

		
	}

}

}
