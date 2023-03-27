using System.Collections.Generic;
using NavigationTiles.Entities;
using NavigationTiles.Pathfinding;
using UnityEngine;

namespace NavigationTiles
{
	public class NavNode : INode
	{
		//Tile/Configuration things.
		
		public NavTile NavTile => _tile;
		private NavTile _tile;
		public TilemapNavigation TilemapNavigation => _navigation;
		private TilemapNavigation _navigation;

		public int WalkCost => _tile.WalkCost;
		public bool Walkable => _tile.Walkable;
		
		
		// Positions
		
		/// <summary>
		/// TilemapPosition is the cell position in the Grid component. NavPosition is different for hex grids, and internal.
		/// </summary>
		public Vector3Int GridPosition => _gridPosition;
		private Vector3Int _gridPosition;
		public Vector3 WorldPosition => _navigation.Tilemap.CellToWorld(GridPosition)+_navigation.Tilemap.tileAnchor;

		public NavNode(NavTile tile,Vector3Int gridPosition,TilemapNavigation navigation)
		{
			this._tile = tile;
			_gridPosition = gridPosition;
			this._navigation = navigation;
		}
		
	}
}