namespace Tactics
{
	public interface INavTile
	{
		public int WalkCost { get; }
		public bool Walkable { get; }
	}
}