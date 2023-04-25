using System;
using Tactics.StateMachine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using State = Tactics.StateMachine.State;
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
			Undo.undoRedoPerformed += UndoRedoPerformed;
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

				// Convert this element's data to a SerializedObject so we can iterate
				// through each SerializedProperty and render a PropertyField.
				SerializedObject nestedObject = new SerializedObject(element.objectReferenceValue);
				bool isActiveState = _machine.CurrentState == nestedObject.targetObject;

				string label = (!isActiveState) ? element.objectReferenceValue.name : (element.objectReferenceValue.name + " (current)");
				EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);

				
				var nameProp = nestedObject.FindProperty("stateName");
				rect.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(rect, nameProp);

				
				if (EditorApplication.isPlaying && !isActiveState)
				{
					rect.y += EditorGUIUtility.singleLineHeight;
					if (GUI.Button(rect,"Enter")) 
					{
						_machine.EnterState((Tactics.StateMachine.State)nestedObject.targetObject);
					}
					// if (EditorGUI.LinkButton(rect, "enter state"))
					// {
					// 		_machine.EnterState((Tactics.StateMachine.State)nestedObject.targetObject);
					// }
				}

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
					//+1 for the name Label
					additionalProps += EditorGUIUtility.singleLineHeight;


					if (EditorApplication.isPlaying && _machine.CurrentState != (State)element.objectReferenceValue)
					{
						additionalProps += EditorGUIUtility.singleLineHeight;
					}
				}

				float spacingBetweenElements = EditorGUIUtility.singleLineHeight / 2;

				return baseProp + spacingBetweenElements + additionalProps;
			};

			stateList.onAddCallback = (ReorderableList list) =>
			{
				var s = target as StateMachine;
				s.AddElement<Tactics.StateMachine.State>(statesProp, "New State");
			};
			
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed += UndoRedoPerformed;
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

		private void UndoRedoPerformed()
		{
			// Debug.Log("Undo Redo Performed");
		}

		
	}
