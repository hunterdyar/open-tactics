using UnityEngine;

namespace HDyar.SimpleSOStateMachine
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