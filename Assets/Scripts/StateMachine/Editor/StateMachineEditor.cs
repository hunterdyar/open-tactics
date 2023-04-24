using System;
using Tactics.StateMachine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using StateMachine = Tactics.StateMachine.StateMachine;

[CustomEditor(typeof(Tactics.StateMachine.StateMachine))]
	public class StateMachineEditor : Editor
	{
		private ReorderableList stateList;
		private SerializedProperty statesProp;
		private StateMachine _machine;
		private struct StateCreationParams
		{
			public string Path;
		}

		public void OnEnable()
		{
			_machine = (target as StateMachine);
			statesProp = serializedObject.FindProperty("states");
			stateList = new ReorderableList(
				serializedObject,
				statesProp,
				draggable: true,
				displayHeader: true,
				displayAddButton: true,
				displayRemoveButton: true);
		

			stateList.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, "States");
			};
			stateList.onRemoveCallback = (ReorderableList list) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(list.index);
				//todo: error handling
				
				_machine.RemoveElement<Tactics.StateMachine.State>(list.index,statesProp);
				ReorderableList.defaultBehaviours.DoRemoveButton(list);
				
				//todo: testing this
				if (_machine.DefaultState == null)
				{
					if (_machine.states.Count > 0)
					{
						_machine.SetDefaultState(_machine.states[0]);
					}
				}
			};

			stateList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				SerializedProperty element = statesProp.GetArrayElementAtIndex(index);

				rect.y += 2;
				rect.width -= 10;
				rect.height = EditorGUIUtility.singleLineHeight;

				if (element.objectReferenceValue == null)
				{
					return;
				}

				string label = element.objectReferenceValue.name;
				EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);

				// Convert this element's data to a SerializedObject so we can iterate
				// through each SerializedProperty and render a PropertyField.
				SerializedObject nestedObject = new SerializedObject(element.objectReferenceValue);

				// Loop over all properties and render them
				// SerializedProperty prop = nestedObject.GetIterator();
				// float y = rect.y;
				// while (prop.NextVisible(true))
				// {
				// 	if (prop.name == "m_Script")
				// 	{
				// 		continue;
				// 	}
				// 	Debug.Log(prop.type);
				// 	if (prop.type == "UnityEvent" || prop.type == "PersistentCallGroup")
				// 	{
				// 		continue;
				// 	}
				//
				// 	rect.y += EditorGUIUtility.singleLineHeight;
				// 	EditorGUI.PropertyField(rect, prop);
				// }
				var nameProp = nestedObject.FindProperty("stateName");
				rect.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(rect, nameProp);
				

				nestedObject.ApplyModifiedProperties();

				// Mark edits for saving
				if (GUI.changed)
				{
					EditorUtility.SetDirty(target);
				}

			};

			stateList.elementHeightCallback = (int index) =>
			{
				float baseProp = EditorGUI.GetPropertyHeight(stateList.serializedProperty.GetArrayElementAtIndex(index), true);

				float additionalProps = 0;
				SerializedProperty element = statesProp.GetArrayElementAtIndex(index);
				if (element.objectReferenceValue != null)
				{
					SerializedObject ability = new SerializedObject(element.objectReferenceValue);
					
					//+1 for the name Label
					additionalProps += EditorGUIUtility.singleLineHeight;
				}

				float spacingBetweenElements = EditorGUIUtility.singleLineHeight / 2;

				return baseProp + spacingBetweenElements + additionalProps;
			};

			stateList.onAddCallback = (ReorderableList list) =>
			{
				var s = target as StateMachine;
				s.AddElement<Tactics.StateMachine.State>(statesProp, "New State");
				
				//first created state should be the default.
				//todo: this isn't working until assetdatabase gets refreshed or something like that.
				//Instead, we should do OnValidate
				// if (s.states.Count == 1)
				// {
				// 	s.SetDefaultState(s.states[0]);
				// }
			};
			
		}



		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawDefaultInspector();
			

			var currentDefault = _machine.states.IndexOf(_machine.DefaultState);
			var states = _machine.states;
			var options = new string[states.Count];
			for (var i = 0; i < states.Count; i++)
			{
				options[i] = states[i].stateName;
			}

			int selectedIndex = EditorGUILayout.Popup("Default State", currentDefault, options);
			if (currentDefault != selectedIndex)
			{
				_machine.SetDefaultState(states[selectedIndex]);
			}

			
			EditorGUILayout.Space();
			
			stateList.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
		}
	}
