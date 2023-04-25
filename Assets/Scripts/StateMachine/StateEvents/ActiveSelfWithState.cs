using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Tactics.StateMachine
{
	public class ActiveSelfWithState : MonoBehaviour, IStateListener
	{
		public State state;

		private void Awake()
		{
			state.RegisterListener(this);
		}

		private void Start()
		{
			gameObject.SetActive(state.IsCurrentState);
		}

		private void OnDestroy()
		{
			state.DeregisterListener(this);
		}


		public void OnEnterState()
		{
			gameObject.SetActive(true);
		}

		public void OnExitState()
		{
			gameObject.SetActive(false);
		}
	}
}