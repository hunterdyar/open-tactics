namespace Tactics.StateMachine
{
	public interface IStateListener
	{
		public void OnEnterState();
		public void OnExitState();
	}
}