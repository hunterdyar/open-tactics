using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HDyar.SimpleSOStateMachine
{
	public class State : ScriptableObject
	{
		[Tooltip("Name of the state object. Sub-ScriptableObject will be updated to match this name, but you may have to hit save to force the refresh in the editor/project window.")]
		public string stateName;

		//C# Actions
		public Action OnEnterState;
		public Action OnExitState;
		
		//Unity Events
		[Tooltip("Called on the State ScriptableObject when it becomes the active state for it's machine.")]
		public UnityEvent OnEnter;
		[Tooltip("Called on the State ScriptableObject when it stops being the active state for it's machine. Not called during initialization.")]
		public UnityEvent OnExit;
		
		//References/Utility
		public StateMachine StateMachine => _stateMachine;
		private StateMachine _stateMachine;//injected or serialized?
		public bool IsCurrentState => _stateMachine.CurrentState == this;

		private List<IStateListener> _listeners = new List<IStateListener>();
		
		public void Enter()
		{
			foreach (var listener in _listeners)
			{
				listener.OnEnterState();
			}
			OnEnterState?.Invoke();
			OnEnter.Invoke();
		}

		public void Exit()
		{
			foreach (var listener in _listeners)
			{
				listener.OnExitState();
			}
			OnExitState?.Invoke();
			OnExit.Invoke();
		}

		public void RegisterListener(IStateListener listener)
		{
			if (!_listeners.Contains(listener))
			{
				_listeners.Add(listener);
			}
			else
			{
				Debug.LogWarning("Can't register listener for state, already registered",this);
			}
		}

		public void DeregisterListener(IStateListener listener)
		{
			if (_listeners.Contains(listener))
			{
				_listeners.Remove(listener);
			}
			else
			{
				Debug.LogWarning("Tried to remove listener from state where it was not registered.",this);
			}
		}

#if UNITY_EDITOR
		public void OnValidate()
		{
			if (name != stateName)
			{
				name = stateName;
			}

			if (_stateMachine == null)
			{
				_stateMachine = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(this)) as StateMachine;
				
			}
		}
#endif
		
	}
}