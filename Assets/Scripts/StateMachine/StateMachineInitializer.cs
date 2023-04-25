using System;
using UnityEngine;

namespace Tactics.StateMachine
{
	public class StateMachineInitializer : MonoBehaviour
	{
		[SerializeField] private StateMachine _machine;
		private void Awake()
		{
			_machine.Init();
		}
	}
}