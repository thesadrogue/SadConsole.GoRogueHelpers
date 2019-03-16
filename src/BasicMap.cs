using GoRogue;
using GoRogue.GameFramework;
using GoRogue.MapViews;
using SadConsole.Components;
using System.Collections.Generic;

namespace SadConsole
{
	public class BasicMap : Map
	{
		// One of these per layer, so we force the rendering order to be what we want (high layers
		// appearing on top of low layers). They're added to consoles in order of this array, first
		// to last, which controls the render order.
		private MultipleConsoleEntityDrawingComponent[] _entitySyncersByLayer;

		private List<Console> _renderers;

		public BasicMap(int width, int height, int numberOfEntityLayers, Distance distanceMeasurement, uint layersBlockingWalkability = uint.MaxValue,
				   uint layersBlockingTransparency = uint.MaxValue, uint entityLayersSupportingMultipleItems = uint.MaxValue)
			: base(CreateTerrain(width, height), numberOfEntityLayers, distanceMeasurement, layersBlockingWalkability,
				  layersBlockingTransparency, entityLayersSupportingMultipleItems)
		{
			// Cast it to what we know it really is and store it so we have the reference for later.
			RenderingCellData = ((ArrayMap<BasicTerrain>)((LambdaSettableTranslationMap<BasicTerrain, IGameObject>)Terrain).BaseMap);

			// Initialize basic components
			_renderers = new List<Console>();
			_entitySyncersByLayer = new MultipleConsoleEntityDrawingComponent[numberOfEntityLayers];
			for (int i = 0; i < _entitySyncersByLayer.Length; i++)
				_entitySyncersByLayer[i] = new MultipleConsoleEntityDrawingComponent();

			// Sync existing renderers when things are added
			ObjectAdded += GRMap_ObjectAdded;
			ObjectRemoved += GRMap_ObjectRemoved;
		}

		public IReadOnlyList<Console> Renderers => _renderers.AsReadOnly();

		// TODO: There is actually no reason this HAS to be public at this point, since the ConfigureAsRenderer function
		// takes care of setting the backing surface -- it just seems convenient to be ablet o initialize consoles based on this
		/// <summary>
		/// Exposed only to allow you to create consoles that use this as their rendering data. DO
		/// NOT set new cells via this array -- use <see cref="SetTerrain(IGameObject)"/> instead.
		/// </summary>
		public Cell[] RenderingCellData { get; }

		/// <summary>
		/// Configures the given console to render the current map and its entities by changing the surface it renders
		/// to the maps surface, and attaching entity sync components appropriately. 
		/// </summary>
		/// <param name="renderer">Console to configure.</param>
		public void ConfigureAsRenderer(Console renderer)
		{
			if (renderer.Cells != RenderingCellData)
				renderer.SetSurface(RenderingCellData, Width, Height);

			_renderers.Add(renderer);
			foreach (var syncer in _entitySyncersByLayer)
				renderer.Components.Add(syncer);
			renderer.IsDirty = true; // Make sure we re-render - SadConsole bug doesn't set this when constructed
		}

		// Create new map, and return as something GoRogue understands
		private static ISettableMapView<IGameObject> CreateTerrain(int width, int height)
		{
			var actualTerrain = new ArrayMap<BasicTerrain>(width, height);
			return new LambdaSettableTranslationMap<BasicTerrain, IGameObject>(actualTerrain, t => t, g => (BasicTerrain)g);
		}

		private void GRMap_ObjectAdded(object sender, ItemEventArgs<IGameObject> e)
		{
			if (e.Item is BasicEntity entity)
				_entitySyncersByLayer[entity.Layer - 1].Entities.Add(entity);
			else if (e.Item.Layer == 0)
			{
				foreach (var renderer in _renderers)
					renderer.IsDirty = true;
			}
		}

		private void GRMap_ObjectRemoved(object sender, ItemEventArgs<IGameObject> e)
		{
			if (e.Item is BasicEntity entity)
				_entitySyncersByLayer[entity.Layer - 1].Entities.Remove(entity);
		}
	}
}
