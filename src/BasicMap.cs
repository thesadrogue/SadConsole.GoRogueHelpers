using GoRogue;
using GoRogue.GameFramework;
using GoRogue.MapViews;
using SadConsole.Components;
using System;
using System.Collections.Generic;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole
{
    /// <summary>
    /// A basic map that has all the functionality of GoRogue.GameFramework.Map, and that can be rendered by SadConsole Consoles.
    /// </summary>
    public class BasicMap : Map
    {
        // One of these per layer, so we force the rendering order to be what we want (high layers
        // appearing on top of low layers). They're added to consoles in order of this array, first
        // to last, which controls the render order.
        private MultipleConsoleEntityDrawingComponent[] _entitySyncersByLayer;

        private List<Console> _renderers;

        private bool _drawingComponentsHandleVisibility;

        /// <summary>
        /// Whether or not the MultipleConsoleEntityDrawingComponents added to all consoles configured to render this map should handle setting the visibility
        /// of each entity as appropriate.  For cases where you want visibility to be controlled by some other means (for example, FOV), you may want to set this to false.
        /// <see cref="FOVVisibilityHandler"/> sets this to false for you.
        /// </summary>
        public bool DrawingComponentsHandleVisibility
        {
            get => _drawingComponentsHandleVisibility;

            set
            {
                if (_drawingComponentsHandleVisibility != value)
                {
                    _drawingComponentsHandleVisibility = value;

                    foreach (var syncComponent in _entitySyncersByLayer)
                        syncComponent.HandleIsVisible = value;
                }
            }
        }

        /// <summary>
        /// Fires whenever FOV is recalculated.
        /// </summary>
        public event EventHandler FOVRecalculated;

        private BasicEntity _controlledGameObject;
        /// <summary>
        /// The game object that will be controlled by the player.
        /// </summary>
        public BasicEntity ControlledGameObject
        {
            get => _controlledGameObject;
            set
            {
                if (_controlledGameObject != value)
                {
                    var oldObject = _controlledGameObject;
                    _controlledGameObject = value;
                    ControlledGameObjectChanged?.Invoke(this, new ControlledGameObjectChangedArgs(oldObject));
                }
            }
        }

        /// <summary>
        /// Fires whenever the value of <see cref="ControlledGameObject"/> is changed.
        /// </summary>
        public event EventHandler<ControlledGameObjectChangedArgs> ControlledGameObjectChanged;

        /// <summary>
        /// Creates a BasicMap.
        /// </summary>
        /// /// <param name="width">Width of the map.</param>
        /// <param name="height">Height of the map.</param>
        /// <param name="numberOfEntityLayers">Number of non-terrain layers for the map.</param>
        /// <param name="distanceMeasurement"><see cref="Distance"/> measurement to use for pathing/measuring distance on the map.</param>
        /// <param name="layersBlockingWalkability">Layer mask containing those layers that should be allowed to have items that block walkability.
        /// Defaults to all layers.</param>
        /// <param name="layersBlockingTransparency">Layer mask containing those layers that should be allowed to have items that block FOV.
        /// Defaults to all layers.</param>
        /// <param name="entityLayersSupportingMultipleItems">Layer mask containing those layers that should be allowed to have multiple objects at the same
        /// location on the same layer.  Defaults to all layers.</param>
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

            DrawingComponentsHandleVisibility = true;

            // Ensure sync components/IsDirty flag stay up to date
            ObjectAdded += GRMap_ObjectAdded;
            ObjectRemoved += GRMap_ObjectRemoved;
        }

        /// <summary>
        /// Read-only list of consoles that are rendering this map.
        /// </summary>
        public IReadOnlyList<Console> Renderers => _renderers.AsReadOnly();

        /// <summary>
        /// Exposed only to allow you to create consoles that use this as their rendering data. DO
        /// NOT set new cells via this array -- use <see cref="Map.SetTerrain(IGameObject)"/> instead.
        /// </summary>
        public Cell[] RenderingCellData { get; }

        /// <summary>
        /// Creates a ScrollingConsole that is configured to render this map.  The console will use the map's cell
        /// data, and all the console's entities will be properly synced.  If you have an existing Console
        /// that wasn't created via this function that you would like to configure to render the map, use
        /// <see cref="ConfigureAsRenderer(Console)"/>.
        /// </summary>
        /// <param name="viewport">Portion of map that should be displayed.</param>
        /// <param name="font">Font to use for the console.</param>
        /// <returns>A pre-configured ScrollingConsole that renders this map.</returns>
        public ScrollingConsole CreateRenderer(XnaRectangle viewport, Font font)
        {
            var renderer = new ScrollingConsole(Width, Height, font, viewport, RenderingCellData);
            ConfigureAsRenderer(renderer);
            return renderer;
        }

        /// <summary>
        /// Removes a console as a renderer of this map.  This should be done any time a console is no longer rendering
        /// the map, or if the console is being discarded and the map is not.
        /// </summary>
        /// <param name="renderer">The renderer to remove.</param>
        /// <param name="clearCellSurface">If set to true, the cell surface will be set to a new cell surface of the same width/height
        /// as the console.  You may want to disable this if you are immediately replacing the cell array to avoid duplicate allocations.</param>
        public void RemoveRenderer(Console renderer, bool clearCellSurface = true)
        {
            // Clear the cell surface to a new one if needed
            if (clearCellSurface)
                renderer.SetSurface(null, renderer.Width, renderer.Height);

            // Remove syncing components, and flag the console as needing re-rendered.
            _renderers.Remove(renderer);
            foreach (var syncer in _entitySyncersByLayer)
                renderer.Components.Remove(syncer);

            renderer.IsDirty = true;
        }

        /// <summary>
        /// Configures given existing console to render the current map and its entities by changing the surface it renders
        /// to the maps surface, and attaching entity sync components appropriately. A console retrieved via
        /// <see cref="CreateRenderer(XnaRectangle, Font)"/> is already configured and does not need to have this function called on it.
        /// </summary>
        /// <param name="renderer">Console to configure.</param>
        public void ConfigureAsRenderer(Console renderer)
        {
            // Ensure we don't add components twice
            if (_renderers.Contains(renderer))
                return;

            // Set new cell array if needed
            if (renderer.Cells != RenderingCellData)
                renderer.SetSurface(RenderingCellData, Width, Height);

            _renderers.Add(renderer);
            foreach (var syncer in _entitySyncersByLayer)
                renderer.Components.Add(syncer);
            renderer.IsDirty = true; // Make sure we re-render
        }

        /// <inheritdoc />
        public override void CalculateFOV(int x, int y, double radius, Distance radiusShape)
        {
            base.CalculateFOV(x, y, radius, radiusShape);

            FOVRecalculated?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public override void CalculateFOV(int x, int y, double radius, Distance radiusShape, double angle, double span)
        {
            base.CalculateFOV(x, y, radius, radiusShape, angle, span);

            FOVRecalculated?.Invoke(this, EventArgs.Empty);
        }

        // Create new map, and return as something GoRogue understands
        private static ISettableMapView<IGameObject> CreateTerrain(int width, int height)
        {
            var actualTerrain = new ArrayMap<BasicTerrain>(width, height);
            return new LambdaSettableTranslationMap<BasicTerrain, IGameObject>(actualTerrain, t => t, g => (BasicTerrain)g);
        }

        // Ensure entity console syncing components are tracking any new entities, and otherwise ensure IsDirty
        // is set if terrain is updated.
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

        // Ensure entities are removed from console-syncing components
        private void GRMap_ObjectRemoved(object sender, ItemEventArgs<IGameObject> e)
        {
            if (e.Item is BasicEntity entity)
                _entitySyncersByLayer[entity.Layer - 1].Entities.Remove(entity);
        }
    }

    /// <summary>
    /// Arguments to ControlledGameObjectChanged event.
    /// </summary>
    public class ControlledGameObjectChangedArgs : EventArgs
    {
        /// <summary>
        /// The old object that was previously assigned to the field.
        /// </summary>
        public BasicEntity OldObject { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldObject"/>
        public ControlledGameObjectChangedArgs(BasicEntity oldObject)
        {
            OldObject = oldObject;
        }
    }
}
