﻿using UnityEngine;

namespace Tactics.Entities
{
	public class Agent : GridEntity
	{
		[SerializeField] private EntityMap _agentLayer;
		private TilemapNavigation TilemapNavigation => _agentLayer.TilemapNavNavigation;
		private NavNode _currentNode;
		void Start()
		{
			if (TilemapNavigation == null)
			{
				Debug.LogWarning("No tilemap for agent. You probably need to add the entity layer to the Tilemap Navigation component to initialize it.");
			}
			if (TilemapNavigation.TryGetNavNodeAtWorldPos(transform.position, out var node))
			{
				_currentNode = node;
				_agentLayer.AddEntityToMap(_currentNode,this);
			}
			else
			{
				Debug.LogWarning("Agent not on map",this);
			}
		}
		public bool TryMoveInDirection(Vector3Int direction)
		{
			if (TilemapNavigation.TryGetNavNode(_currentNode.GridPosition + direction,out var node))
			{
				if (node.Walkable && !_agentLayer.HasAnyEntity(node))
				{
					MoveToNode(node, true);
					return true;
				}
			}
			return false;
		}

		public void MoveToNode(NavNode node, bool animate = true)
		{
			_agentLayer.MoveEntityToNode(this,node);
			_currentNode = node;
			//snap... for now
			transform.position = _currentNode.WorldPosition;
		}
	}
}