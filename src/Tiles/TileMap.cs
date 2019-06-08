using GoRogue;
using GoRogue.MapViews;
using System;
using System.Collections.Generic;

namespace SadConsole.Tiles
{
	// TODO: This isn't just tile map functionality, also includes blueprint integration and regions and stuff.
	// This stuff could probably be pulled out, but for a get-it-working off of a minimal base, this is a start
	/// <summary>
	/// A map designed to contain the terrain as <see cref="Tile"/> instances.
	/// </summary>
	public class TileMap : BasicMap
	{
		public List<Maps.Region> Regions;

		/// <summary>
		/// Fires when a map tile is replaced or gets an alert that its state has changed.
		/// </summary>
		public event EventHandler<TileChangedEventArgs> MapTileChanged;

		public TileMap(int width, int height, int numberOfEntityLayers, Distance distanceMeasurement, string defaultTileBlueprint = "wall", 
						uint layersBlockingWalkability = uint.MaxValue, uint layersBlockingTransparency = uint.MaxValue,
						uint entityLayersSupportingMultipleItems = uint.MaxValue)
			: base(width, height, numberOfEntityLayers, distanceMeasurement, layersBlockingWalkability, layersBlockingTransparency,
				   entityLayersSupportingMultipleItems)
		{
			Regions = new List<Maps.Region>();

			// Be efficient by not using factory.Create each tile below. Instead, get the blueprint and use that to create each tile.
			var defaultTile = Tile.Factory.GetBlueprint(defaultTileBlueprint);

			// Configure to set up event forwarding properly on tile add/remove
			ObjectAdded += TileMap_ObjectAdded;
			ObjectRemoved += TileMap_ObjectRemoved;

			// Fill the map with walls
			foreach (var pos in this.Positions())
				SetTerrain(defaultTile.Create(pos));
		}

		public Tile FindEmptyTile() => GetTerrain<Tile>(WalkabilityView.RandomPosition(true));

		// If objects are terrain, handle syncing up their tilechanged events
		private void TileMap_ObjectAdded(object sender, ItemEventArgs<GoRogue.GameFramework.IGameObject> e)
		{
			// Configure new tile, and fire event
			if (e.Item is Tile tile)
			{
				tile.TileChanged += Tile_TileChanged;
				MapTileChanged?.Invoke(this, new TileChangedEventArgs(this, tile));
			}
		}

		private void TileMap_ObjectRemoved(object sender, ItemEventArgs<GoRogue.GameFramework.IGameObject> e)
		{
			// Remove event handler to cleanup nicely
			// TODO: This depends on added object to make sure the event fires on replacement, however this breaks
			// if a tile is set to null (for whatever reason)...
			if (e.Item is Tile tile)
				tile.TileChanged -= Tile_TileChanged;
		}

		// Fire map-based event for either tile being set, or its state changing.
		private void Tile_TileChanged(object sender, EventArgs e) => MapTileChanged?.Invoke(this, new TileChangedEventArgs(this, (Tile)sender));

		/// <summary>
		/// Event arguments for when a tile changed event fires.
		/// </summary>
		public class TileChangedEventArgs : EventArgs
		{
			/// <summary>
			/// The tile that changed.
			/// </summary>
			public readonly Tile Tile;

			/// <summary>
			/// The map that owns the tile.
			/// </summary>
			public readonly TileMap Map;

			public TileChangedEventArgs(TileMap map, Tile tile)
			{
				Map = map;
				Tile = tile;
			}
		}
	}
}
