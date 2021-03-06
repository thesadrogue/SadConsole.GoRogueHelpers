﻿using Microsoft.Xna.Framework;

namespace SadConsole
{
    /// <summary>
    /// Handler that will make all terrain/entities inside FOV visible as normal, all entities outside of FOV invisible, all
    /// terrain outside of FOV invisible if unexplored, and set its foreground to <see cref="ExploredColor"/> if explored but out of FOV.
    /// </summary>
    public class DefaultFOVVisibilityHandler : FOVVisibilityHandler
    {
        /// <summary>
        /// Foreground color to set to all terrain that is outside of FOV but has been explored.
        /// </summary>
        public Color ExploredColor { get; }

        /// <summary>
        /// Creates a DefaultFOVVisibilityHandler that will manage visibility of objects for the given map as noted in the class description.
        /// </summary>
        /// <param name="map">The map this handler will manage visibility for.</param>
        /// <param name="unexploredColor">Foreground color to set to all terrain tiles that are outside of FOV but have been explored.</param>
        /// <param name="startingState">The starting state to put the handler in.</param>
        public DefaultFOVVisibilityHandler(BasicMap map, Color unexploredColor, State startingState = State.Enabled)
            : base(map, startingState) => ExploredColor = unexploredColor;

        /// <summary>
        /// Makes entity visible.
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        protected override void UpdateEntitySeen(BasicEntity entity) => entity.IsVisible = true;

        /// <summary>
        /// Makes entity invisible.
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        protected override void UpdateEntityUnseen(BasicEntity entity) => entity.IsVisible = false;

        /// <summary>
        /// Makes terrain visible and sets its foreground color to its regular value.
        /// </summary>
        /// <param name="terrain">Terrain to modify.</param>
        protected override void UpdateTerrainSeen(BasicTerrain terrain)
        {
            terrain.IsVisible = true;
            terrain.RestoreState();
        }

        /// <summary>
        /// Makes terrain invisible if it is not explored.  Makes terrain visible but sets its foreground to
        /// <see cref="ExploredColor"/> if it is explored.
        /// </summary>
        /// <param name="terrain">Terrain to modify.</param>
        protected override void UpdateTerrainUnseen(BasicTerrain terrain)
        {
            if (Map.Explored[terrain.Position])
            {
                terrain.SaveState();
                terrain.Foreground = ExploredColor;
            }
            else
                terrain.IsVisible = false;
        }
    }
}
