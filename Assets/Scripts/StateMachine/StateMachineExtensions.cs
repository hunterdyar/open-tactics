using UnityEditor;
using UnityEngine;

namespace Tactics.StateMachine
{
	public static class StateMachineExtensions
	{
		//https://forum.unity.com/threads/how-to-properly-add-remove-sub-assets-scriptable-objects-using-undo-redo.699737/
		public static void AddElement<T>(this ScriptableObject scriptableObject, SerializedProperty listProperty, string name = "Element", HideFlags hideFlags = HideFlags.None) where T : ScriptableObject
		{
			if (!listProperty.isArray)
				throw new System.Exception("\"listProperty\" is not a List.");

			T element = ScriptableObject.CreateInstance<T>();

			element.name = name;
			element.hideFlags = hideFlags;

			//todo: un-generic this whole function
			if (element is State state)
			{
				state.stateName = name;
			}
			
			string scriptableObjectPath = AssetDatabase.GetAssetPath(scriptableObject);

			AssetDatabase.AddObjectToAsset(element, scriptableObjectPath);
			AssetDatabase.SaveAssets();

			Undo.RegisterCreatedObjectUndo(element, "Add element to ScriptableObject");

			listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
			SerializedProperty lastElement = listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1);
			lastElement.objectReferenceValue = element;
			
		}

		public static void RemoveElement<T>(this ScriptableObject scriptableObject, int index, SerializedProperty listProperty) where T : ScriptableObject
		{
			if (!listProperty.isArray)
			{
				throw new System.Exception("\"listProperty\" is not a List.");
			}

			if (index < 0 || index > listProperty.arraySize - 1)
			{
				throw new System.Exception("\"index\" out of range.");
			}

			if (listProperty.arraySize == 0)
			{
				return;
			}

			SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(index);

			//Undo
			Undo.SetCurrentGroupName("Remove element from ScriptableObject");
			int group = Undo.GetCurrentGroup();

			Undo.RecordObject(listProperty.serializedObject.targetObject, "");
			
			var reff = elementProperty.objectReferenceValue;
			listProperty.DeleteArrayElementAtIndex((int)index);

			Undo.DestroyObjectImmediate(reff);
			
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Undo.CollapseUndoOperations(group);

			// AssetDatabase.SaveAssets();
			// AssetDatabase.Refresh();

		}
	}
}