using System;
using System.Linq;
using GoRogue;
using GoRogue.GameFramework;
using GoRogue.MapViews;

namespace SadConsole
{
    /// <summary>
    /// A class that controls visibility of the map objects based on the map's FOV.  Create a subclass and implement abstract methods to determine what properties to set for what case.
    /// </summary>
    public abstract class FOVVisibilityHandler
    {
        /// <summary>
        /// Possible states for the FOVVisibilityHandler to be in.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Enabled state -- FOVVisibilityHandler will actively set things as seen/unseen when appropriate.
            /// </summary>
            Enabled,
            /// <summary>
            /// Disabled state.  All items in the map will be set as seen, and the FOVVisibilityHandler
            /// will not set visibility of any items as FOV changes or as items are added/removed.
            /// </summary>
            DisabledResetVisibility,
            /// <summary>
            /// Disabled state.  No changes to the current visibility of terrain/entities will be made, and the FOVVisibilityHandler
            /// will not set visibility of any items as FOV changes or as items are added/removed.
            /// </summary>
            DisabledNoResetVisibility
        }

        /// <summary>
        /// Whether or not the FOVVisibilityHandler is actively setting things to seen/unseen as appropriate.
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// The map that this handler manages visibility of objects for.
        /// </summary>
        public BasicMap Map { get; }

        /// <summary>
        /// Creates a FOVVisibilityHandler that will manage visibility of objects for the given map.
        /// </summary>
        /// <param name="map">The map this handler will manage visibility for.</param>
        /// <param name="startingState">The starting state to put the handler in.</param>
        public FOVVisibilityHandler(BasicMap map, State startingState = State.Enabled)
        {
            Map = map;

            map.DrawingComponentsHandleVisibility = false;

            map.ObjectAdded += Map_ObjectAdded;
            map.ObjectMoved += Map_ObjectMoved;
            map.FOVRecalculated += Map_FOVRecalculated;

            SetState(startingState);
        }

        /// <summary>
        /// Sets the state of the FOVVisibilityHandler, affecting its behavior appropriately.
        /// </summary>
        /// <param name="state">The new state for the FOVVisibilityHandler.  See <see cref="State"/> documentation for details.</param>
        public void SetState(State state)
        {
            switch (state)
            {
                case State.Enabled:
                    Enabled = true;

                    foreach (Coord pos in Map.Positions())
                    {
                        BasicTerrain terrain = Map.GetTerrain<BasicTerrain>(pos);
                        if (terrain != null && Map.FOV.BooleanFOV[pos])
                            UpdateTerrainSeen(terrain);
                        else if (terrain != null)
                            UpdateTerrainUnseen(terrain);
                    }

                    foreach (Console renderer in Map.Renderers)
                        renderer.IsDirty = true;

                    foreach (BasicEntity entity in Map.Entities.Items.Cast<BasicEntity>())
                    {
                        if (Map.FOV.BooleanFOV[entity.Position])
                            UpdateEntitySeen(entity);
                        else
                            UpdateEntityUnseen(entity);
                    }

                    break;

                case State.DisabledNoResetVisibility:
                    Enabled = false;
                    break;

                case State.DisabledResetVisibility:
                    foreach (Coord pos in Map.Positions())
                    {
                        BasicTerrain terrain = Map.GetTerrain<BasicTerrain>(pos);
                        if (terrain != null)
                            UpdateTerrainSeen(terrain);
                    }

                    foreach (Console renderer in Map.Renderers)
                        renderer.IsDirty = true;

                    foreach (BasicEntity entity in Map.Entities.Items.Cast<BasicEntity>())
                        UpdateEntitySeen(entity);

                    Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Sets the state to enabled.
        /// </summary>
        public void Enable() => SetState(State.Enabled);

        /// <summary>
        /// Sets the state to disabled.  If <paramref name="resetVisibilityToSeen"/> is true, all items will be set to seen before
        /// the FOVVisibilityHandler is disabled.
        /// </summary>
        /// <param name="resetVisibilityToSeen">Whether or not to set all items in the map to seen before disabling the FOVVisibilityHandler.</param>
        public void Disable(bool resetVisibilityToSeen = true)
            => SetState(resetVisibilityToSeen ? State.DisabledResetVisibility : State.DisabledNoResetVisibility);

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
            if (!Enabled) return;

            if (e.Item.Layer == 0) // Terrain
            {
                if (Map.FOV.BooleanFOV[e.Position])
                    UpdateTerrainSeen((BasicTerrain)(e.Item));
                else
                    UpdateTerrainUnseen((BasicTerrain)(e.Item));
            } // No need to set IsDirty on renderers, SetTerrain would have done that for us.
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
            if (!Enabled) return;

            if (Map.FOV.BooleanFOV[e.NewPosition])
                UpdateEntitySeen((BasicEntity)(e.Item));
            else
                UpdateEntityUnseen((BasicEntity)(e.Item));
        }

        private void Map_FOVRecalculated(object sender, EventArgs e)
        {
            if (!Enabled) return;

            foreach (Coord position in Map.FOV.NewlySeen)
            {
                BasicTerrain terrain = Map.GetTerrain<BasicTerrain>(position);
                if (terrain != null)
                    UpdateTerrainSeen(terrain);

                foreach (BasicEntity entity in Map.GetEntities<BasicEntity>(position))
                    UpdateEntitySeen(entity);
            }

            foreach (Console renderer in Map.Renderers)
                renderer.IsDirty = true;

            foreach (Coord position in Map.FOV.NewlyUnseen)
            {
                BasicTerrain terrain = Map.GetTerrain<BasicTerrain>(position);
                if (terrain != null)
                    UpdateTerrainUnseen(terrain);

                foreach (BasicEntity entity in Map.GetEntities<BasicEntity>(position))
                    UpdateEntityUnseen(entity);
            }
        }
    }
}
