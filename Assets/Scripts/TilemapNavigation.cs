using System;
using System.Collections.Generic;
using Tactics.Entities;
using Tactics.Pathfinding;
using Tactics.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

//runtime only. We create a dictionary of NavNodes that is fast to operate on, which reference the tile data.

//There are a few structural things to know. One, Tilemap is a sealed class, so we can't make our own version of tilemap and extend it. This tool should just work with how unity works.
//That means we have to hook into tilemap and read it's data. For now, the asset will only work at Runtime, and we do this initialization on awake.
//That has some downsides - mostly editor convenience. But the upside is we can use a dictionary, as we won't be serializing any data.

//the downside is updates at runtime. This system doesn't support fully adding/removing tiles during play yet (use walkable flag for now), but my goal is to be able to.
//Perhaps more importantly, one can optimize a grid to remove redundant nodes, only leaving nodes where a user may turn (or stop). By having navnodes and navTiles be independent, that's a possibility.

namespace Tactics
{
	[RequireComponent(typeof(Tilemap))]
	public class TilemapNavigation : MonoBehaviour, IGraph
	{
		private NavNode[] _getNeighborCache = new NavNode[8]; //"8" here needs to be the highest possible number of neighbors.
		public GridConnectionType ConnectionConnectionType => _connectionType = GridConnectionType.FlatCardinal; //default
		
		//todo: Editor script to set this to hexagon and readonly when appropriate
		[SerializeField]
		private GridConnectionType _connectionType;

		public Tilemap Tilemap => _tilemap;
		private Tilemap _tilemap;
		public Pathfinder<NavNode> Pathfinder => _pathfinder;
		private Pathfinder<NavNode> _pathfinder;
		public Grid Grid => _tilemap.layoutGrid;
		public int MaxNodeCount => _navMap.Count + 1; //used by the pathfinder.
		
		//Dictionaries cannot be serialized by Unity. 
		private readonly Dictionary<Vector3Int, NavNode> _navMap = new Dictionary<Vector3Int, NavNode>();
		[Tooltip("Should reference all entity maps that will be used, will initiate them, and allow tilemap to be access point for all entities on node")]
		[SerializeField] private EntityMap[] _entityMaps;
		private void Awake()
		{
			_tilemap = GetComponent<Tilemap>();
			InitiateNavMap();
			InitiateEntityMaps();
			_pathfinder = new AStarPathfinder<NavNode>(this);
		}

		private void InitiateEntityMaps()
		{
			if (_entityMaps.Length == 0)
			{
				Debug.LogWarning("No entity maps configured in Tilemap Navigation",this);
			}
			foreach (var map in _entityMaps)
			{
				map.Initiate(this);
			}
		}
		private void InitiateNavMap()
		{
			//we don't know if bounds have been reasonably set or not.
			_tilemap.CompressBounds();

			var bounds = _tilemap.cellBounds;
			//I was about to write an extension method to give me allPositionsWithin, until taking the 1/4 second to actually read documentation and go "oh, wait, that already exists"
			//This will work for all layouts (rectangular, hex, grid, etc)
			foreach (var location in bounds.allPositionsWithin)
			{
				var tile = _tilemap.GetTile<NavTile>(location);
				if (tile != null)
				{
					//we use grid cellposition for rectangular and isometric maps, but for hex, we convert to Cube.
					//This makes all the math and pathfinding much easier, at the inconvenience of these wrapper functions to do the conversion when needed.
					_navMap.Add(location, new NavNode(tile, location, this));
				}
			}
		}
		public INode[] GetNeighborNodes(INode node, bool walkableOnly = true)
		{
			switch (_connectionType)
			{
				case GridConnectionType.FlatCardinalAndDiagonal:
					return GetNeighborNodesUsingDirectionList(node, RectUtility.CardinalAndDiagonalDirections, walkableOnly);
				case GridConnectionType.FlatCardinal:
				default:
					return GetNeighborNodesUsingDirectionList(node, RectUtility.CardinalDirections, walkableOnly);
			}
		}

		private INode[] GetNeighborNodesUsingDirectionList(INode node, Vector3Int[] directions, bool walkableOnly = true)
		{
			// NavNode[] nodeCache = new NavNode[12];
			int n = 0;
			foreach (var dir in directions)
			{
				if (_navMap.TryGetValue(node.GridPosition + dir, out var neighbor))
				{
					if (!walkableOnly || node.Walkable)
					{
						_getNeighborCache[n] = neighbor;
						n++;
					}
				}
			}

			if (n == 0)
			{
				return Array.Empty<NavNode>();
			}

			var output = new NavNode[n];
			Array.Copy(_getNeighborCache, output, n);
			return output;
		}
		private void OnValidate()
		{
			_tilemap = GetComponent<Tilemap>();
			if (Grid.cellLayout == GridLayout.CellLayout.Hexagon && _connectionType != GridConnectionType.Hexagonal)
			{
				Debug.LogWarning("Hexagonal connection type is only valid option for grid.");
				_connectionType = GridConnectionType.Hexagonal;
			}

			if (Grid.cellLayout == GridLayout.CellLayout.Rectangle && _connectionType == GridConnectionType.Hexagonal)
			{
				Debug.LogWarning("Hexagonal connection type is not valid option for rectangular grid.");
				_connectionType = GridConnectionType.FlatCardinalAndDiagonal;
			}

			if (Grid.cellLayout == GridLayout.CellLayout.Isometric && _connectionType == GridConnectionType.Hexagonal)
			{
				Debug.LogWarning("Hexagonal connection type is not valid option for isometric grid.");
				_connectionType = GridConnectionType.FlatCardinal;
			}

			if (Grid.cellLayout == GridLayout.CellLayout.IsometricZAsY && _connectionType == GridConnectionType.Hexagonal)
			{
				Debug.LogWarning("Hexagonal connection type is not valid option for isometricZAsY grid.");
				_connectionType = GridConnectionType.FlatCardinal;
			}
		}

		public List<GridEntity> GetAllEntitiesOnNode(NavNode node)
		{
			var entities = new List<GridEntity>();
			
			foreach (var map in _entityMaps)
			{
				if (map.TryGetEntity(node, out var entity))
				{
					entities.Add(entity);
				}
			}
			
			return entities;
		}

		public NavTile GetNavTile(Vector3Int gridCellPosition)
		{
			return _tilemap.GetTile<NavTile>(gridCellPosition);
		}


		public bool TryGetNavNodeAtWorldPos(Vector3 worldPos, out NavNode node)
		{
			var pos = Grid.WorldToCell(worldPos);
			return TryGetNavNode(pos, out node);
		}

		public bool TryGetNavNode(Vector3Int gridCellPosition, out NavNode node)
		{
			return _navMap.TryGetValue(gridCellPosition, out node);
		}

		public NavNode GetNavNode(Vector3Int gridPosition)
		{
			if (_navMap.TryGetValue(gridPosition, out var node))
			{
				return node;
			}

			return null;
		}


		public bool HasNavCellLocation(Vector3Int navPosition)
		{
			return _navMap.ContainsKey(navPosition);
		}
	}
}