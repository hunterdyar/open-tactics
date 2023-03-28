using Unity.VisualScripting;
using UnityEngine;

namespace Tactics.Pathfinding
{
	public interface INode
	{
		public bool Walkable { get; }
		public int WalkCost {get;}
		public Vector3Int GridPosition { get; }
	}
}