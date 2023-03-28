﻿using System.Collections.Generic;
using Tactics.PriorityQueue;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace Tactics.Pathfinding
{
	/// <summary>
	/// A* pathfinding using (by default) a manhattan distance Heuristic that works on any square (or square-ish) map.
	/// </summary>
	public class AStarPathfinder<T> : Pathfinder<T> where T : INode
	{
		private readonly Dictionary<T, int> costSoFar = new Dictionary<T, int>();
		private readonly SimplePriorityQueue<T> frontier = new SimplePriorityQueue<T>();
		public AStarPathfinder(IGraph graph) : base(graph)
		{
		}

		public override bool TryFindPath(T start, T end, out List<T> path)
		{
			_pathStatus = PathStatus.Searching;
			costSoFar.Clear();
			costSoFar[start] = 0;
			cameFrom.Clear();
			cameFrom[start] = start;
			frontier.Clear();

			frontier.Enqueue(start, 0);
			
			while (frontier.Count > 0)
			{
				var current = frontier.Dequeue();

				if (Equals(current, end))
				{
					_pathStatus = PathStatus.PathFound;
					break;
				}

				foreach (T next in tilemap.GetNeighborNodes(current))
				{
					int newCost = costSoFar[current] + next.WalkCost; //cost algorithm generalized somewhere

					if (!costSoFar.ContainsKey(next))
					{
						costSoFar[next] = newCost;
						int priority = newCost + Heuristic(end,next);
						cameFrom[next] = current;
						frontier.Enqueue(next, priority);
					}else if (newCost < costSoFar[next])
					{
						costSoFar[next] = newCost;
						int priority = newCost + Heuristic(end, next);
						cameFrom[next] = current;
						frontier.UpdatePriority(next, priority);
					}
				}
			}
			
			//We made it this far and never found the end tile, we have failed.
			if (_pathStatus == PathStatus.Searching)
			{
				_pathStatus = PathStatus.NoPathFound;
			}

			path = GetPath(end);
			return _pathStatus == PathStatus.PathFound;
		}

		

		//overload for convenience
		public virtual int Heuristic(INode a, INode b, int stepUpLayerCost = 1)
		{
			return Heuristic(a.GridPosition, b.GridPosition,stepUpLayerCost);
		}

		//Manhattan Distance. StepCost is multiplier for going up or down on z.
		public virtual int Heuristic(Vector3Int a, Vector3Int b, int stepUpLayerCost = 1)
		{
			return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + stepUpLayerCost*Mathf.Abs(a.z - b.z);
		}
	}
}