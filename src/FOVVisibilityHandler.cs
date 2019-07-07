using GoRogue;
using GoRogue.GameFramework;
using GoRogue.MapViews;
using System;
using System.Linq;

namespace SadConsole
{
    /// <summary>
    /// A class that controls visibility of the map objects based on the map's FOV.  Create a subclass and implement abstract methods to determine what properties to set for what case.
    /// </summary>
    public abstract class FOVVisibilityHandler
    {
        /// <summary>
        /// The map that this handler manages visibility of objects for.
        /// </summary>
        public BasicMap Map { get; }

        /// <summary>
        /// Creates a FOVVisibilityHandler that will manage visibility of objects for the given map.
        /// </summary>
        /// <param name="map">The map this handler will manage visibility for.</param>
        public FOVVisibilityHandler(BasicMap map)
        {
            Map = map;

            map.DrawingComponentsHandleVisibility = false;

            map.ObjectAdded += Map_ObjectAdded;
            map.ObjectMoved += Map_ObjectMoved;
            map.FOVRecalculated += Map_FOVRecalculated;

            foreach (var pos in map.Positions())
            {
                var terrain = map.GetTerrain<BasicTerrain>(pos);
                if (terrain != null && map.FOV.BooleanFOV[pos])
                    UpdateTerrainSeen(terrain);
                else if (terrain != null)
                    UpdateTerrainUnseen(terrain);
            }

            foreach (var entity in map.Entities.Items.Cast<BasicEntity>())
            {
                if (map.FOV.BooleanFOV[entity.Position])
                    UpdateEntitySeen(entity);
                else
                    UpdateEntityUnseen(entity);
            }
        }

        /// <summary>
        /// Implement to make appropriate changes to a terrain tile that is now inside FOV.
        /// </summary>
        /// <param name="terrain">Terrain tile to modify.</param>
        protected abstract void UpdateTerrainSeen(BasicTerrain terrain);

        /// <summary>
        /// Implement to make appropriate changes to a terrain tile that is now outside FOV.
        /// </summary>
        /// <param name="terrain">Terrain tile to modify.</param>
        protected abstract void UpdateTerrainUnseen(BasicTerrain terrain);

        /// <summary>
        /// Implement to make appropriate changes to an entity that is now inside FOV.
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        protected abstract void UpdateEntitySeen(BasicEntity entity);

        /// <summary>
        /// Implement to make appropriate changes to an entity that is now outside FOV.
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        protected abstract void UpdateEntityUnseen(BasicEntity entity);

        private void Map_ObjectAdded(object sender, ItemEventArgs<IGameObject> e)
        {
            if (e.Item.Layer == 0) // Terrain
            {
                if (Map.FOV.BooleanFOV[e.Position])
                    UpdateTerrainSeen((BasicTerrain)(e.Item));
                else
                    UpdateTerrainUnseen((BasicTerrain)(e.Item));
            }
            else // Entities
            {
                if (Map.FOV.BooleanFOV[e.Position])
                    UpdateEntitySeen((BasicEntity)(e.Item));
                else
                    UpdateEntityUnseen((BasicEntity)(e.Item));
            }
        }

        // Only entities (not terrain) can move so this is ok to just assume entities.
        private void Map_ObjectMoved(object sender, ItemMovedEventArgs<IGameObject> e)
        {
            if (Map.FOV.BooleanFOV[e.NewPosition])
                UpdateEntitySeen((BasicEntity)(e.Item));
            else
                UpdateEntityUnseen((BasicEntity)(e.Item));
        }

        private void Map_FOVRecalculated(object sender, EventArgs e)
        {

            foreach (var position in Map.FOV.NewlySeen)
            {
                var terrain = Map.GetTerrain<BasicTerrain>(position);
                if (terrain != null)
                    UpdateTerrainSeen(terrain);
                foreach (var entity in Map.GetEntities<BasicEntity>(position))
                    UpdateEntitySeen(entity);
            }

            foreach (var position in Map.FOV.NewlyUnseen)
            {
                var terrain = Map.GetTerrain<BasicTerrain>(position);
                if (terrain != null)
                    UpdateTerrainUnseen(terrain);
                foreach (var entity in Map.GetEntities<BasicEntity>(position))
                    UpdateEntityUnseen(entity);
            }
        }
    }
}
