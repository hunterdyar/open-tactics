namespace HDyar.SimpleSOStateMachine
{
	public interface IStateListener
	{
		public void OnEnterState();
		public void OnExitState();
	}
}