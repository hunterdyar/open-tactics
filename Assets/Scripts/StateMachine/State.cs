using System;
using UnityEngine;
using UnityEngine.Events;

namespace Tactics.StateMachine
{
	public class State : ScriptableObject
	{
		[Tooltip("Name of the state object. Sub-ScriptableObject will be updated to match this name, but you may have to hit save to force the refresh in the editor/project window.")]
		public string stateName;

		[Tooltip("Called on the State ScriptableObject when it becomes the active state for it's machine.")]
		public UnityEvent OnEnterState;
		[Tooltip("Called on the State ScriptableObject when it stops being the active state for it's machine. Not called during initialization.")]
		public UnityEvent OnExitState;
		
		
		public void OnValidate()
		{
			if (name != stateName)
			{
				name = stateName;
			}
		}
		
	}
}