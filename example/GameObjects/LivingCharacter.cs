using System.Collections.Generic;
using GoRogue;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Maps;
using SadConsole.Tiles;

namespace BasicTutorial.GameObjects
{
    internal class GameFrameTileVisibilityRefresher : SadConsole.Components.GoRogue.GameFrameProcessor
    {
        public new LivingCharacter Parent => (LivingCharacter)base.Parent;
        public override void ProcessGameFrame() => Parent.RefreshVisibilityTiles();
    }

    internal abstract class LivingCharacter : BasicEntity
    {

        protected int baseHealthMax = 10;
        protected int baseAttack = 10;
        protected int baseDefense = 10;
        protected int baseVisibilityDistance = 10;
        protected int baseLightSourceDistance = 5;

        /// <summary>
        /// Current health of the character
        /// </summary>
        public int Health { get; protected set; }

        /// <summary>
        /// The health max of the character
        /// </summary>
        public int HealthMax => baseLightSourceDistance + GetInventoryHealthMods();

        /// <summary>
        /// The attack of the character
        /// </summary>
        public int Attack => baseLightSourceDistance + GetInventoryAttackMods();

        /// <summary>
        /// The defense of the character
        /// </summary>
        public int Defense => baseLightSourceDistance + GetInventoryDefenseMods();

        /// <summary>
        /// How far you can see 
        /// </summary>
        public int VisibilityDistance => baseVisibilityDistance + GetInventoryVisibilityMods();

        /// <summary>
        /// How far your light source goes
        /// </summary>
        public int LightSourceDistance => baseLightSourceDistance + GetInventoryLightingMods();

        /// <summary>
        /// Gets or sets a friendly short title for the object.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a longer description for the object.
        /// </summary>
        public string Description { get; set; }

        public new TileMap CurrentMap => (TileMap)base.CurrentMap;


        /// <summary>
        /// The inventory of a character.
        /// </summary>
        public Items.Inventory Inventory = new Items.Inventory();

        /// <summary>
        /// What tiles the character can see.
        /// </summary>
        public List<Tile> VisibleTiles = new List<Tile>();
        public List<Tile> LightSourceTiles = new List<Tile>();

        protected List<Tile> newVisibleTiles = new List<Tile>();
        protected List<Tile> newLightSourceTiles = new List<Tile>();

        protected Region currentRegion;

        protected GoRogue.FOV FOVSight;
        protected GoRogue.FOV FOVLighted;

        protected LivingCharacter(TileMap map, Coord position, Color foreground, Color background, int glyph)
            : base(foreground, background, glyph, position, 1, isWalkable: false, isTransparent: true)
        {
            Title = "Unknown";
            Description = "Not much is known about this object.";

            FOVSight = new GoRogue.FOV(map.TransparencyView);
            FOVLighted = new GoRogue.FOV(map.TransparencyView);

            map.AddEntity(this);

            AddGoRogueComponent(new GameFrameTileVisibilityRefresher());
        }

        public void RefreshVisibilityTiles()
        {

            // Check to see if have left a room
            if (currentRegion != null && !currentRegion.InnerPoints.Contains(Position))
            {
                // If player, handle room lighting
                if (this == CurrentMap.ControlledGameObject)
                {
                    foreach (Coord point in currentRegion.InnerPoints)
                    {
                        CurrentMap.GetTerrain<Tile>(point).UnsetFlag(TileFlags.Lighted, TileFlags.InLOS);
                    }
                    foreach (Coord point in currentRegion.OuterPoints)
                    {
                        CurrentMap.GetTerrain<Tile>(point).UnsetFlag(TileFlags.Lighted, TileFlags.InLOS);
                    }

                    foreach (Coord tile in FOVSight.CurrentFOV)
                    {
                        CurrentMap.GetTerrain<Tile>(tile).SetFlag(TileFlags.InLOS);
                    }

                    foreach (Coord tile in FOVLighted.CurrentFOV)
                    {
                        CurrentMap.GetTerrain<Tile>(tile).SetFlag(TileFlags.Lighted);
                    }
                }

                // We're not in this region anymore
                currentRegion = null;
            }

            // Not in a region, so find one.
            if (currentRegion == null)
            {
                // See if we're in a different region
                foreach (Region region in CurrentMap.Regions)
                {
                    if (region.InnerPoints.Contains(Position))
                    {
                        currentRegion = region;
                        break;
                    }
                }
            }

            // TODO: This code was placed here, got working, but the region code and
            //       newly unused variables have not been scrubbed.

            // BUG: If I exit a region and stand on the doorway, the tiles in the room
            //      that should be visible are not.

            // Visibility
            FOVSight.Calculate(Position, VisibilityDistance);

            // If player, handle LOS flags for tiles.
            if (this == CurrentMap.ControlledGameObject)
            {
                foreach (Coord tile in FOVSight.NewlyUnseen)
                {
                    CurrentMap.GetTerrain<Tile>(tile).UnsetFlag(TileFlags.InLOS);
                }

                foreach (Coord tile in FOVSight.NewlySeen)
                {
                    CurrentMap.GetTerrain<Tile>(tile).SetFlag(TileFlags.InLOS);
                }
            }

            // Lighting
            FOVLighted.Calculate(Position, LightSourceDistance);

            if (this == CurrentMap.ControlledGameObject)
            {
                foreach (Coord tile in FOVLighted.NewlyUnseen)
                {
                    CurrentMap.GetTerrain<Tile>(tile).UnsetFlag(TileFlags.Lighted);
                }

                foreach (Coord tile in FOVLighted.NewlySeen)
                {
                    CurrentMap.GetTerrain<Tile>(tile).SetFlag(TileFlags.Lighted, TileFlags.Seen);
                }
            }


            // Check and see if we're in a region, ensure these tiles are always visible and lighted.
            if (currentRegion != null)
            {
                Tile tile;

                // Make sure these are lit
                foreach (Coord point in currentRegion.InnerPoints)
                {
                    tile = CurrentMap.GetTerrain<Tile>(point);

                    // If player, handle room lighting
                    if (this == CurrentMap.ControlledGameObject)
                    {
                        tile.SetFlag(TileFlags.Lighted, TileFlags.InLOS, TileFlags.Seen);
                    }

                    // Add tile to visible list, for calculating if the player can see.
                    VisibleTiles.Add(tile);
                }

                foreach (Coord point in currentRegion.OuterPoints)
                {
                    tile = CurrentMap.GetTerrain<Tile>(point);

                    // If player, handle room lighting
                    if (this == CurrentMap.ControlledGameObject)
                    {
                        tile.SetFlag(TileFlags.Lighted, TileFlags.InLOS, TileFlags.Seen);
                    }

                    // Add tile to visible list, for calculating if the player can see.
                    VisibleTiles.Add(tile);
                }
            }
        }

        protected void AddVisibleTile(int x, int y)
        {
            if (CurrentMap.Bounds().Contains(x, y))
            {
                Tile tile = CurrentMap.GetTerrain<Tile>(x, y);
                tile.SetFlag(TileFlags.InLOS);
                newVisibleTiles.Add(tile);
            }
        }

        protected void AddLightVisbilityTile(int x, int y)
        {
            if (CurrentMap.Bounds().Contains(x, y))
            {
                Tile tile = CurrentMap.GetTerrain<Tile>(x, y);
                tile.SetFlag(TileFlags.Lighted);
                newLightSourceTiles.Add(tile);
            }
        }

        protected int GetInventoryHealthMods()
        {
            int result = 0;

            foreach (Items.Item item in Inventory.GetEquippedItems())
            {
                result += item.HealthModifier;
            }

            return result;
        }

        protected int GetInventoryAttackMods()
        {
            int result = 0;

            foreach (Items.Item item in Inventory.GetEquippedItems())
            {
                result += item.AttackModifier;
            }

            return result;
        }

        protected int GetInventoryDefenseMods()
        {
            int result = 0;

            foreach (Items.Item item in Inventory.GetEquippedItems())
            {
                result += item.DefenseModifier;
            }

            return result;
        }

        protected int GetInventoryVisibilityMods()
        {
            int result = 0;

            foreach (Items.Item item in Inventory.GetEquippedItems())
            {
                result += item.VisibilityModifier;
            }

            return result;
        }

        protected int GetInventoryLightingMods()
        {
            int result = 0;

            foreach (Items.Item item in Inventory.GetEquippedItems())
            {
                result += item.LightingModifier;
            }

            return result;
        }
    }
}
